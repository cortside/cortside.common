[CmdletBinding()]
Param 
(
    [Parameter(Mandatory = $false)][string]$package = "",
	[Parameter(Mandatory = $false)][switch]$createpullrequest
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

echo "prepping"

.\clean.ps1 -quiet
$result = check-result

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

$files = (git status *.csproj | grep "modified:" | wc -l)
if ($files -ne "0") {
	dotnet test src
	$result = check-result

	git status

	$bot = "BOT-{0:yyyyMMdd}" -f (Get-Date)
	$branch = "feature/$bot"
	if ($package -ne "") {
		$branch = "feature/$bot-$package"
	}

	git add *.csproj
	git status
	git checkout -b $branch
	if ($package -eq "") { 
		git commit -m "[$branch] updated nuget packages"
	} else {
		git commit -m "[$branch] update $package"
	}
	git push --set-upstream origin $branch

	$remote = (git remote -v)
	if ($remote -like "*github.com*") {
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
