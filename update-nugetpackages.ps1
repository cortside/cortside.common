[cmdletBinding()]
param(
	[switch]$update=[switch]::Present,
	[Parameter(Mandatory=$false)][switch]$NoVersionLock,
	[switch]$cortside
)

function Remove-NuGetPackageFromProjects {
    param (
        [string]$RootPath = ".",
        [string]$filter = "*.csproj",
        [string]$PackageName
    )

    Get-ChildItem -Filter $filter -Recurse | ForEach-Object {
        $file = $_
        if (Select-String -Path $file.FullName -Pattern $PackageName) {
            Write-Host "Removing '$PackageName' from $($file.FullName)"
            dotnet remove $file.FullName package $PackageName
        }
    }
}

function Add-NuGetPackageToProjects {
    param (
        [string]$RootPath = ".",
        [string]$filter = "*.csproj",
        [string]$PackageName
    )

    Get-ChildItem -Filter $filter -Recurse | ForEach-Object {
        $file = $_
		if (-not (Select-String -Path $file.FullName -Pattern $PackageName)) {
            Write-Host "Adding '$PackageName' to $($file.FullName)"
            dotnet add $file.FullName package $PackageName
        }
    }
}

# install tool for updating outdated packages
dotnet tool update --global dotnet-outdated-tool

# remove older analyzers from projects
Remove-NuGetPackageFromProjects -PackageName "AsyncAnalyzers"
Remove-NuGetPackageFromProjects -PackageName "Lindhart.Analyser.MissingAwaitWarning"

# analyzers for all projects
Add-NuGetPackageToProjects -PackageName "Microsoft.VisualStudio.Threading.Analyzers"
Add-NuGetPackageToProjects -PackageName "SonarAnalyzer.CSharp"
Add-NuGetPackageToProjects -PackageName "Roslynator.Analyzers"
Add-NuGetPackageToProjects -PackageName "ReferenceTrimmer"


# remove older packages for test projects
Remove-NuGetPackageFromProjects -filter "*Test*.csproj" -PackageName "coverlet.msbuild"
Remove-NuGetPackageFromProjects -filter "*Test*.csproj" -PackageName "xunit.runner.console"

# packages for test projects
Add-NuGetPackageToProjects -filter "*Test*.csproj" -PackageName "coverlet.collector"
Add-NuGetPackageToProjects -filter "*Test*.csproj" -PackageName "xunit"
Add-NuGetPackageToProjects -filter "*Test*.csproj" -PackageName "xunit.runner.visualstudio"
Add-NuGetPackageToProjects -filter "*Test*.csproj" -PackageName "Microsoft.NET.Test.Sdk"

if ((Test-Path env:BUILD_SERVER) -And ($env:BUILD_SERVER -eq "TeamCity")) {
    Add-NuGetPackageToProjects -filter "*Test*.csproj" -PackageName "TeamCity.VSTest.TestAdapter"
}

if ($NoVersionLock.IsPresent) {
	dotnet outdated ./src --pre-release Never --upgrade
} else {
	dotnet outdated ./src --version-lock Major --pre-release Never --upgrade
}

if ($cortside.IsPresent) {
	dotnet outdated ./src --include Cortside --upgrade
}
