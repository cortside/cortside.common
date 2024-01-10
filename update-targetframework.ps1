using namespace System.Collections.Generic # for List<T> 

$dir = "." # points to "repository root"
$targetVersion = "net6.0"

$projFiles = Get-ChildItem $dir -Recurse -Filter *.csproj

$projsWithVersion = [List[object]]::new()

foreach($file in $projFiles)
{
    $content = [xml](Get-Content $file.FullName)
    $versionNodes = $content.GetElementsByTagName("TargetFramework");
        
    switch($versionNodes.Count)
    {
        0 {
            Write-Host "The project has no framework version: $file.FullName"
            break;
        }
        1 {
            $version = $versionNodes[0].InnerText;

            $projsWithVersion.Add([PsCustomObject]@{
                File = $file;
                XmlContent = $content;
                VersionNode = $versionNodes[0];
                VersionRaw = $version;
                Version = $version;
            })
            break;
        }
        default {
            Write-Host "The project has multiple elements of TargetFramework: $file.FullName"
            break;
        }
    }
}

function Print-Version-Statistics([List[object]] $projsWithVersion)
{
    $numberOfProjectsByVersion = @{}

    foreach($proj in $projsWithVersion)
    {
        if($numberOfProjectsByVersion.ContainsKey($proj.Version))
        {
            $numberOfProjectsByVersion[$proj.Version] = $numberOfProjectsByVersion[$proj.Version] + 1
        }
        else
        {
            $numberOfProjectsByVersion[$proj.Version] = 1
        }
    }
    
    Write-Host "`nCurrent version distribution:"
    $numberOfProjectsByVersion
}

Print-Version-Statistics($projsWithVersion)

foreach($proj in $projsWithVersion)
{    
    if($targetVersion -ne $proj.Version)
    {
        $proj.VersionNode.set_InnerXML("$targetVersion")
        $proj.XmlContent.Save($proj.File.FullName);
		Write-Host "Updated $($proj.File.FullName) to $targetVersion"	
    }
}

