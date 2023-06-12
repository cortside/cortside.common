[cmdletBinding()]
param(
	[switch]$update=[switch]::Present,
	[Parameter(Mandatory=$false)][switch]$NoVersionLock
)

dotnet tool update --global dotnet-outdated-tool
dotnet tool update --global dotnet-tools-outdated

dotnet-tools-outdated

if ((Test-Path env:BUILD_SERVER) -And ($env:BUILD_SERVER -eq "TeamCity")) {
	gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "TeamCity.VSTest.TestAdapter")){ echo "add TeamCity.VSTest.TestAdapter to $_.Fullname"; dotnet add $_.FullName package TeamCity.VSTest.TestAdapter }}
}

gci *Test*.csproj -Recurse | % { if (select-string -inputobject $_ -Pattern "coverlet.msbuild") { echo "remove coverlet.msbuild to $_.Fullname"; dotnet remove $_.FullName package coverlet.msbuild } }
gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "coverlet.collector")){ echo "add coverlet.collector to $_.Fullname"; dotnet add $_.FullName package coverlet.collector }}
gci *.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "Microsoft.VisualStudio.Threading.Analyzers")){ echo "add Microsoft.VisualStudio.Threading.Analyzers to $_.Fullname"; dotnet add $_.FullName package Microsoft.VisualStudio.Threading.Analyzers }}
gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "Microsoft.NET.Test.Sdk")){ echo "add Microsoft.NET.Test.Sdk to $_.Fullname"; dotnet add $_.FullName package Microsoft.NET.Test.Sdk }}

if ($NoVersionLock.IsPresent) {
	dotnet outdated ./src --pre-release Never --upgrade --exclude restsharp
} else {
	dotnet outdated ./src --version-lock Major --pre-release Never --upgrade
}
