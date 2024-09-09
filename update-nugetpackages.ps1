[cmdletBinding()]
param(
	[switch]$update=[switch]::Present,
	[Parameter(Mandatory=$false)][switch]$NoVersionLock,
	[switch]$cortside
)

dotnet tool update --global dotnet-outdated-tool
dotnet tool update --global dotnet-tools-outdated

# remove older analyzers from projects
gci *.csproj -Recurse | %{ if (select-string -inputobject $_ -Pattern "AsyncAnalyzers") { echo "remove AsyncAnalyzers from $_.Fullname"; dotnet remove $_.FullName package AsyncAnalyzers } }
gci *.csproj -Recurse | %{ if (select-string -inputobject $_ -Pattern "Lindhart.Analyser.MissingAwaitWarning") { echo "remove Lindhart.Analyser.MissingAwaitWarning from $_.Fullname"; dotnet remove $_.FullName package Lindhart.Analyser.MissingAwaitWarning } }

# analyzers for all projects
gci *.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "Microsoft.VisualStudio.Threading.Analyzers")){ echo "add Microsoft.VisualStudio.Threading.Analyzers to $_.Fullname"; dotnet add $_.FullName package Microsoft.VisualStudio.Threading.Analyzers }}

# remove older packages for test projects
gci *Test*.csproj -Recurse | %{ if (select-string -inputobject $_ -Pattern "coverlet.msbuild") { echo "remove coverlet.msbuild from $_.Fullname"; dotnet remove $_.FullName package coverlet.msbuild } }
gci *Test*.csproj -Recurse | %{ if (select-string -inputobject $_ -Pattern "xunit.runner.console") { echo "remove coverlet.msbuild from $_.Fullname"; dotnet remove $_.FullName package xunit.runner.console } }

# packages for test projects
gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "coverlet.collector")){ echo "add coverlet.collector to $_.Fullname"; dotnet add $_.FullName package coverlet.collector }}
gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern """xunit""")){ echo "add xunit to $_.Fullname"; dotnet add $_.FullName package xunit }}
gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "xunit.runner.visualstudio")){ echo "add xunit.runner.visualstudio to $_.Fullname"; dotnet add $_.FullName package xunit.runner.visualstudio }}
gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "Microsoft.NET.Test.Sdk")){ echo "add Microsoft.NET.Test.Sdk to $_.Fullname"; dotnet add $_.FullName package Microsoft.NET.Test.Sdk }}

if ((Test-Path env:BUILD_SERVER) -And ($env:BUILD_SERVER -eq "TeamCity")) {
	gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "TeamCity.VSTest.TestAdapter")){ echo "add TeamCity.VSTest.TestAdapter to $_.Fullname"; dotnet add $_.FullName package TeamCity.VSTest.TestAdapter }}
}

if ($NoVersionLock.IsPresent) {
	dotnet outdated ./src --pre-release Never --upgrade --exclude restsharp
} else {
	dotnet outdated ./src --version-lock Major --pre-release Never --upgrade
}

if ($cortside.IsPresent) {
	dotnet outdated ./src --include Cortside --upgrade
}