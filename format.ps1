[cmdletbinding()]
param(
	[Parameter(Mandatory = $false)][switch]$FixEncoding
)

function Convert-Encoding {
[cmdletBinding()]
Param(
	[string[]]$filePaths
)

	foreach ($file in $filePaths) {
		Write-Output "Convert encoding to UTF-8 for $file"
		$MyFile = Get-Content $file
		[System.IO.File]::WriteAllLines($file, $MyFile)
	}
}

if ($FixEncoding.IsPresent) {
	./clean.ps1

	Convert-Encoding -filePaths ((gci *.ps1 -Recurse) | % { $_.FullName })
	Convert-Encoding -filePaths ((gci *.cs -Recurse) | % { $_.FullName })
	Convert-Encoding -filePaths ((gci *.csproj -Recurse) | % { $_.FullName })
	Convert-Encoding -filePaths ((gci *.json -Recurse) | % { $_.FullName })
	Convert-Encoding -filePaths ((gci *.sln -Recurse) | % { $_.FullName })
	Convert-Encoding -filePaths ((gci *.sql -Recurse) | % { $_.FullName })
}

# install/update latest version
dotnet tool update -g dotnet-format --version "8.*" --add-source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet8/nuget/v3/index.json

# Format code to match editorconfig settings
#dotnet format --verbosity normal .\src

# per readme and because of dotnet cli issues, executing via command directly is best for now
dotnet-format --verbosity normal .\src


git status
