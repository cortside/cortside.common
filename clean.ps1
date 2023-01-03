[cmdletBinding()]
param(
	[switch]$quiet
)

Function remove {
    param([string]$item)
    If (Test-Path $item){
		if (-not $quiet.IsPresent) {
			Write-Output "Removing $item"
		}
        Remove-Item $item -Force -Recurse
    }
}

Function Invoke-Cleanup {
	if (-not $quiet.IsPresent) {
		Write-Output "---------------------"
		Write-Output "Invoke-Cleanup"
		Write-Output "---------------------"
	}

	# clean package, bin and obj folders
	Get-ChildItem .\ -include packages,bin,obj,node_modules -Recurse | Where-Object {$_.FullName -NotMatch "BuildScripts"} | foreach ($_) { Write-Host "Removing $($_.fullname)"; remove-item $_.fullname -Force -Recurse }

	#Find nunit files
	Get-ChildItem -include *.nunit -Recurse |
	ForEach-Object{
		if (-not $quiet.IsPresent) {
			Write-Host $_
		}

		$results = "$($_.BaseName).xml"
		If (Test-Path $results){
				if (-not $quiet.IsPresent) {
					Write-Host "Removing $results"
				}
				Remove-Item $results
		}
	}

	remove "TestResults"
	remove "OpenCover"
	remove "Publish"
	remove "TestBin"
}

# stop extraneous processes
dotnet build-server shutdown

# cleanup all nuget resources
#dotnet nuget locals --clear all

# remove all bin/obj folders
Invoke-Cleanup
