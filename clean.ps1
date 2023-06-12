[cmdletBinding()]
param(
	[switch]$quiet
)

function Remove-EmptyFolders {
    <#
    .SYNOPSIS
        Remove empty folders recursively from a root directory.
        The root directory itself is not removed.

        Author: Joakim Borger Svendsen, Svendsen Tech, Copyright 2022.
        MIT License.
    .EXAMPLE
        . .\Remove-EmptyFolders.ps1
        Remove-EmptyFolders -Path E:\FileShareFolder
    .EXAMPLE
        Remove-EmptyFolders -Path \\server\share\data

        NB. You might have to change $ChildDirectory.FullName to
        $ChildDirectory.ProviderPath in the code for this to work.
        Untested with UNC paths as of 2022-01-28.
    
    #>
    [CmdletBinding()]
    Param(
        [String] $Path
    )
    Begin {
        [Int32] $Script:Counter = 0
        if (++$Counter -eq 1) {
            $RootPath = $Path
            Write-Verbose -Message "Saved root path as '$RootPath'."
        }
        # Avoid overflow. Overly cautious? ~2.15 million directories...
        if ($Counter -eq [Int32]::MaxValue) {
            $Counter = 1
        }
    }
    Process {
        # List directories.
        foreach ($ChildDirectory in Get-ChildItem -LiteralPath $Path -Force |
            Where-Object {$_.PSIsContainer}) {
            # Use .ProviderPath on Windows instead of .FullName,
            # in order to support UNC paths (untested).
            # Process each child directory recursively.
            Remove-EmptyFolders -Path $ChildDirectory.FullName
        }
        $CurrentChildren = Get-ChildItem -LiteralPath $Path -Force
        # If it's empty, the condition below evaluates to true. Get-ChildItem 
        # returns $null for empty folders.
        if ($null -eq $CurrentChildren) {
            # Do not delete the root folder itself.
            if ($Path -ne $RootPath) {
                Write-Verbose -Message "Removing empty folder '$Path'."
                Remove-Item -LiteralPath $Path -Force
            }
        }
    }
}

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
	Get-ChildItem .\ -include packages,bin,obj,node_modules -Recurse | Where-Object {$_.FullName -NotMatch "BuildScripts"} | %{ 
		Write-Host "Removing $($_.fullname)"; 
		remove-item $_.fullname -Force -Recurse 
	}

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
	
	#delete dist folder in directories that have package.json
	gci -Recurse package.json | %{ 
		$dir = "$($_.DirectoryName)\dist"; 
		If (Test-Path $dir ){ 
			Write-Output "Removing $dir"; 
			rm $dir -force -recurse  
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

# remove emtpy directories
Remove-EmptyFolders -Path . -Verbose 
