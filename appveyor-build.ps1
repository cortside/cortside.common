##
# Build library repo
##

$branch = $ENV:APPVEYOR_REPO_BRANCH;
$version = $env:APPVEYOR_BUILD_VERSION;
$fileVersion = $version;
if($branch -ne 'master') {
	$fileVersion = "${fileVersion}-${branch}"; 
}   
$fileVersion = $fileVersion.replace("release/", "");
$fileVersion = $fileVersion.replace("feature/", "");
$fileVersion = $fileVersion.replace("bugfix/", "");
$env:PACKAGE_VERSION = $fileVersion   

$args = @()
$args += "/d:sonar.verbose=true"
if (Test-Path env:APPVEYOR_PULL_REQUEST_NUMBER) {
	$branch = $Env:APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH;
	$target = $Env:APPVEYOR_REPO_BRANCH;
	$commit = $Env:APPVEYOR_PULL_REQUEST_HEAD_COMMIT;
	$pullRequestId = $Env:APPVEYOR_PULL_REQUEST_NUMBER;
	$args += "/d:sonar.scm.revision=$commit";
	$args += "/d:sonar.pullrequest.key=$pullRequestId";
	$args += "/d:sonar.pullrequest.base=$target";
	$args += "/d:sonar.pullrequest.branch=$branch";
} else {
	$branch = $Env:APPVEYOR_REPO_BRANCH;
	$args += "/d:sonar.branch.name=$branch";
	if ($branch -ne "master") {
		$target = "develop";
		if ($branch -eq "develop" -or $branch -like "release/*" -or $branch -like "hotfix/*") {
			$target = "master";
		}		
		$args += "/d:sonar.newCode.referenceBranch=$target";
	}
}

$env:SOURCE_BRANCH="$branch";
$env:TARGET_BRANCH="$target";
$env:SONAR_ARGUMENTS="$analysisArgs";
echo "building version $version from branch $branch targeting $target with analysis arguments of $args";

dotnet tool install --global dotnet-sonarscanner
dotnet sonarscanner begin /k:"$($env:SONAR_PROJECT)" /o:"cortside" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.token="$($env:SONAR_TOKEN)" /v:"$($env:APPVEYOR_BUILD_VERSION)" /d:sonar.cs.opencover.reportsPaths="**/*.opencover.xml" /d:sonar.coverage.exclusions="**/*Test*.cs,**/Migrations/*" /d:sonar.exclusions="**/*Test*.cs,**/Migrations/*,**/*Api.xml" /d:sonar.dotnet.excludeTestProjects=true /d:sonar.scanner.scanAll=false /d:sonar.scm.disabled=true $args
dotnet build src --no-restore --configuration Debug /property:"Version=$($env:APPVEYOR_BUILD_VERSION)"
dotnet test src --no-restore --no-build --collect:"XPlat Code Coverage" --settings ./src/coverlet.runsettings.xml
dotnet sonarscanner end /d:sonar.token="$($env:SONAR_TOKEN)"

dotnet pack src --include-symbols -p:SymbolPackageFormat=snupkg --configuration $env:configuration -o ((get-location).Path + '\artifacts') /property:Version=$env:PACKAGE_VERSION
