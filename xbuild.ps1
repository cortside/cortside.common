[cmdletbinding()]
param(
[string]$branch = "local",
[string]$buildCounter = "0",
[string]$msbuildconfig = "Debug"
)

Function check-result {
	if ($LastExitCode -ne 0) {
		$e = [char]27
		$start = "$e[1;31m"
		$end = "$e[m"
		$text = "ERROR: Exiting with error code $LastExitCode"
		Write-Error "$start$text$end"
		return $false
		exit 1 
	}
	return $true
}

Function Invoke-Exe {
Param(
    [parameter(Mandatory=$true)][string] $cmd,
    [parameter(Mandatory=$true)][string] $args
	
)
	Write-Host "Executing: `"$cmd`" --% $args"
	Invoke-Expression "& `"$cmd`" --% $args"
	$result = check-result
	if (!$result) {
		throw "ERROR executing EXE"
	}
}

function New-BuildJson{
Param (
	[string]$versionjsonpath,
  	[string]$buildjsonpath,
    [string]$buildCounter
)

	$version = Get-Content $versionjsonpath -raw | Convertfrom-json
	$BuildObject = New-Object -TypeName psobject        
	$Build = New-Object -TypeName psobject
	$builditems = [ordered] @{
		"version" = ""
		"timestamp" = ""
		"tag" = ""
		"suffix" = ""
	}
	
	$LeadingVersion = $version.version
	$NewBuildVersion = "$LeadingVersion.$buildCounter"
	$buildTime = (Get-Date).ToUniversalTime().ToString("u")
	$builditems.version = $NewBuildVersion
	$builditems.timestamp = $buildTime 
	
	Foreach ( $item in $builditems.Keys ) {
		 $build | Add-Member -MemberType NoteProperty -Name $item  -Value $builditems.$item
	}

	$BuildObject | Add-Member -MemberType NoteProperty -Name Build -Value $build
	
	$BuildObject| convertto-json -depth 5 | out-file $buildjsonpath -force 
	
	return $buildobject
}

function Set-DockerTag {
Param(
	[string]$branch,
    [string]$buildNumber,
    [string]$buildjsonpath
)

	$lastOctet = $buildNumber.lastindexof(".")
    $LeadingVersion = $buildNumber.substring(0,$lastOctet)
	$dockertagprefix = "$LeadingVersion-"
	
	# overrides by branch
	if ($branch.contains("master"))
	{
		$suffix =$null
		$dockertagprefix = "$buildNumber"	
		$msbuildconfig = "Release"
		$OctopusChannel = "EBLS-Production"
	}
	elseif ($branch.contains("feature") -or $branch.contains("hotfix") -or $branch.contains("bugfix"))
	{
	    write-output "using $branch"
	    $JiraKey = ($branch | Select-String -Pattern '((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)' | % matches).value
		
	    if ($JiraKey -eq $null)
	    {
	        write-error "Please re-create branch via jira for valid jira ticket"
	    }
	    else
	    {
             $JiraBuildKey = $JiraKey.Replace("-","")	  
			 $suffix = "$JiraBuildkey"
			 $OctopusChannel = "EBLS-Interim"
	    }
	}
	elseif ($branch.contains("develop") -or $branch.contains("Develop"))
	{
			$suffix = "develop"
			$OctopusChannel = "EBLS-Development"
	}
	elseif ($branch.contains("release"))
	{
			$suffix = "release"
			$OctopusChannel = "EBLS-Staging"
	}
    elseif ($branch.contains("local"))
	{
			$suffix = "local"
			$OctopusChannel = "EBLS-Interim"
	}
	else
	{
	    write-warning "using custom $branch please use one of the existing base branches or properly use a defined branch type"
	}

	 $dockertag = "${dockertagprefix}${suffix}"
    
     $build = Get-Content $buildjsonpath -raw | Convertfrom-json
     $build.build.tag = $dockertag 
	 $build.build.suffix = $suffix
  
     $build | convertto-json -depth 5 | out-file $buildjsonpath -force 
	 
	 # After the outfile as I do not need this on the JSON for build, only for CI 
	 $build.build | Add-Member -MemberType NoteProperty -Name channel  -Value $OctopusChannel 
	 
     return $build
}

# generate build.json
$BuildNumber = (New-BuildJson -versionJsonPath $PSScriptRoot\repository.json -BuildJsonPath $PSScriptRoot\src\build.json -buildCounter $buildCounter).build.version
Write-Host "##teamcity[buildNumber '$BuildNumber']"
$build = Set-DockerTag -branch $branch -buildNumber $BuildNumber -BuildJsonPath $PSScriptRoot\src\build.json
$dockertag = $build.build.tag
$suffix = $build.build.suffix
$OctopusChannel = $build.build.channel
Write-output = "
--------

Dockertag for this build: $dockertag

++++++++

Suffix for this build: $suffix

++++++++

OctopusChannel for this build: $OctopusChannel

--------
"

Write-Host "##teamcity[setParameter name='env.dockertag' value='$dockertag']"
Write-Host "##teamcity[setParameter name='env.OctopusChannel' value='$OctopusChannel']"
Write-Host "##teamcity[setParameter name='env.suffix' value='$suffix']"
Write-Host "##teamcity[setParameter name='env.MsBuildConfig' value='$msbuildconfig']"

if ($suffix){
	$OctopusVersion = "$BuildNumber-$suffix"
	Write-Host "##teamcity[setParameter name='env.OctopusVersion' value='$OctopusVersion']"	
}elseif(!$suffix){
	$OctopusVersion = "$BuildNumber"
	Write-Host "##teamcity[setParameter name='env.OctopusVersion' value='$OctopusVersion']"
}

# copy generated build.json to needed applications
#cp .\src\build.json .\src\Cortside.Common.WebApi\build.json -force

# build
$args = "clean $PSScriptRoot\src"
Invoke-Exe -cmd dotnet -args $args
$args = "restore $PSScriptRoot\src --packages $PSScriptRoot\src\packages"
Invoke-Exe -cmd dotnet -args $args
$args = "build $PSScriptRoot\src --no-restore --configuration $msbuildconfig /p:Version=$BuildNumber"
Invoke-Exe -cmd dotnet -args $args
#$args = "publish $PSScriptRoot\src\Cortside.Common.WebApi\Cortside.Common.WebApi.csproj --no-restore /p:Version=$BuildNumber"
#Invoke-Exe -cmd dotnet -args $args
