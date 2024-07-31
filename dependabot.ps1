[CmdletBinding()]
Param 
(
    [Parameter(Mandatory = $false)][string]$package = "",
	[Parameter(Mandatory = $false)][string]$preupdateExpression = "",
	[Parameter(Mandatory = $false)][switch]$createPullRequest,
	[Parameter(Mandatory = $false)][switch]$ignoreChanges
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
$changes = (git status --porcelain)
if ($changes.Count -ne 0 -and -not $ignoreChanges.IsPresent) {
	Write-Output "Exiting, sandbox has $($changes.Count) changes"
	Write-Output $changes
	exit 1
}

# make sure this is done from current develop branch
Invoke-Exe git -args "checkout develop"
Invoke-Exe git -args "pull"

# check for branch first
$bot = "BOT-{0:yyyyMMdd}" -f (Get-Date)
$branch = "feature/$bot"
if ($package -ne "") {
	$branch = "feature/$bot-$package"
}

$exists = (git branch -r | sls $branch)
if ($exists -ne $null) {
	Write-Output "Exiting, $branch already exists"
	exit 1
}

echo "prepping"

.\clean.ps1 -quiet
$result = check-result

echo "preupdate"

if ($preupdateExpression -ne "") {
	echo "running $preupdateExpression"
	Invoke-Expression "& $preupdateExpression"
}

echo "about to restore"

Invoke-Exe dotnet -args "restore src --verbosity quiet"
$result = check-result

echo "ready to update nuget packages"
if ($package -eq "") { 
	$body = (.\update-nugetpackages.ps1)
} else {
	$body = (Invoke-Exe dotnet -args "outdated src --include $package --pre-release Never --upgrade")
}
echo $body
$result = check-result

echo "checking to see if anything changed"

$changes = (git status --porcelain)
if ($changes.Count -ne 0) {
	dotnet test src
	$result = check-result

	git status

	$bot = "BOT-{0:yyyyMMdd}" -f (Get-Date)
	$branch = "feature/$bot"
	if ($package -ne "") {
		$branch = "feature/$bot-$package"
	}

	git add -u -u
	git status
	git checkout -b $branch
	if ($package -eq "") { 
		git commit -m "[$branch] updated nuget packages"
	} else {
		git commit -m "[$branch] update $package"
	}
	git push --set-upstream origin $branch

	$remote = (git remote -v)
	$ghexists = if (Get-Command "gh.exe" -ErrorAction SilentlyContinue) { $true } else { $false }
	if ($remote -like "*github.com*" -and $ghexists) {
		gh repo set-default
		gh pr create --title "$bot" --body "$body" --base develop
	} else {
		echo "should create the pr here -- everything passed - $branch"	
		echo $body	
	}

	.\clean.ps1
	git checkout develop
} else {
	echo "no files changed"
}
