Param
(
	[Parameter(Mandatory = $true)][string]$version,
	[Parameter(Mandatory = $false)][switch]$WithReturn
)

# TODO: find current version in file if it exists and truncate to the end

$commits = (git --no-pager log --pretty=format:'| %h | <span style="white-space:nowrap;">%ad</span> | <span style="white-space:nowrap;">%aN</span> | %d %s' --date=short master.. --reverse)

$contents = ((Get-Content CHANGELOG.md -Raw) -replace "(?m)^\s*`r`n",'').trim()

"# Release $version" | Out-File CHANGELOG.md -Encoding utf8 # no append so that file is rewritten
"" | Out-File CHANGELOG.md -Encoding utf8 -Append
"|Commit|Date|Author|Message|" | Out-File CHANGELOG.md -Encoding utf8 -Append
"|---|---|---|---|" | Out-File CHANGELOG.md -Encoding utf8 -Append
$commits | Out-File CHANGELOG.md -Encoding utf8 -Append
"****" | Out-File CHANGELOG.md -Encoding utf8 -Append
"" | Out-File CHANGELOG.md -Encoding utf8 -Append

$releaselog = ((Get-Content CHANGELOG.md -Raw) -replace "(?m)^\s*`r`n",'').trim()
$contents | Out-File CHANGELOG.md -Encoding utf8 -Append

if ($WithReturn.IsPresent) {
	return $releaselog
}
