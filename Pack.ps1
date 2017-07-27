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

$ErrorActionPreference = 'Stop'

echo "build: Build started"

if(Test-Path .\artifacts) {
	echo "build: Cleaning .\artifacts"
	Remove-Item .\artifacts -Force -Recurse
}

$version = @{ $true = $env:APPVEYOR_BUILD_VERSION; $false = "1.0.0" }[$env:APPVEYOR_BUILD_VERSION -ne $NULL];

echo "build: Version suffix is $suffix"

Get-ChildItem -include project.json -Recurse | Resolve-Path -Relative |
ForEach-Object{
	$path = (Get-Item $_).DirectoryName
	Write-Host "Found: $_ in $path"
	
	echo "build: Packaging project in $src"
    Invoke-Exe -cmd dotnet -args "pack $path -o artifacts"
}
	
		