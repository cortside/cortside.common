[cmdletBinding()]
param(
	[switch]$createpullrequest
)

$ErrorActionPreference = "Stop"

Function Invoke-BuildError {
Param(
	[parameter(Mandatory=$true)][string] $text
)
	# cleanup and undo changes
	./clean.ps1
	git checkout -- *
	
	Write-Error $text 
	exit 1
}

Function check-result {
    if ($LastExitCode -ne 0) { Invoke-BuildError "ERROR: Exiting with error code $LastExitCode"	}
    return $true
}

Function Invoke-Exe {
    Param(
        [parameter(Mandatory = $true)][string] $cmd,
        [parameter(Mandatory = $true)][string] $args
    )
	
    Write-Output "Executing: `"$cmd`" --% $args"
    & "$cmd" "--%" "$args";
    $result = check-result
    if (!$result) {
        throw "ERROR executing EXE"
        exit 1
    }
}

# check for uncommitted changed files
git checkout -- *
git status

# make sure this is done from current develop branch
Invoke-Exe git -args "checkout develop"
Invoke-Exe git -args "pull"

cp C:\work\cortside\cortside.templates\templates\api-editorconfig\src\.editorconfig .\src\.editorconfig
cp C:\work\cortside\cortside.templates\templates\api-powershell\clean.ps1
cp C:\work\cortside\cortside.templates\templates\api-powershell\update-nugetpackages.ps1

if (Test-Path -path "cleanup.ps1") {
	rm cleanup.ps1
}
 
.\update-nugetpackages.ps1
$result = check-result

.\clean.ps1
$result = check-result

#dotnet build src
#$result = check-result

dotnet test src
$result = check-result

git status

$bot = "BOT-{0:yyyyMMdd}" -f (Get-Date)
$branch = "feature/$bot"

git add *.csproj
git add clea*.ps1 
git add update-nugetpackages.ps1 
git add dependabot.ps1
git add ./src/.editorconfig
git status
git checkout -b $branch
git commit -m "[$branch] updated nuget packages"
git push --set-upstream origin $branch

.\clean.ps1
git checkout develop

echo "should create the pr here -- everything passed - $branch"
