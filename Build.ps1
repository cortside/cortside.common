Function check-result {
	if ($LastExitCode -ne 0) {
		$e = [char]27
		$start = "$e[1;31m"
		$end = "$e[m"
		$text = "ERROR: Exiting with error code $LastExitCode"
		Write-Host "$start$text$end"
		return $false
	}
	return $true
}

Function Invoke-Exe {
Param(
    [parameter(Mandatory=$true)][string] $cmd,
    [parameter(Mandatory=$true)][string] $args
)

	Write-Host "Executing: `"$cmd`" --% $args"
	Invoke-Expression "& `"$cmd`" $args"
	$result = check-result
	if (!$result) {
		throw "ERROR executing EXE"
	}
}

#$ErrorActionPreference = 'Stop'

echo "build: Build started"

if(Test-Path .\artifacts) {
	echo "build: Cleaning .\artifacts"
	Remove-Item .\artifacts -Force -Recurse
}

Invoke-Exe -cmd dotnet -args restore

#$branch = @{ $true = $env:APPVEYOR_REPO_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$env:APPVEYOR_REPO_BRANCH -ne $NULL];
#$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
#$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision"}[$branch -eq "master" -and $revision -ne "local"]

$version = @{ $true = $env:APPVEYOR_BUILD_VERSION; $false = "1.0.0" }[$env:APPVEYOR_BUILD_VERSION -ne $NULL];

echo "build: Version suffix is $suffix"

Get-ChildItem -include project.json -Recurse | Resolve-Path -Relative |
ForEach-Object{
	$path = (Get-Item $_).DirectoryName
	Write-Host "Found: $_ in $path"
	
#	Push-Location -Path $path
#	Invoke-Exe -cmd dotnet -args "version $version"
#	Pop-Location
	
	Invoke-Exe -cmd dotnet -args "build $path"
}
	
		