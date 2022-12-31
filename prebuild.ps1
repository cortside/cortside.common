$fail = $false

#$env:RESTRICTED_WORDS="comma,separated,list"
if ((Test-Path 'env:RESTRICTED_WORDS')) {
	$words =$($env:RESTRICTED_WORDS).Split(",")
	$words |%{
		& grep -R -i "$_" * |% { $_; $fail = $true }
	}
}

$textFiles = git grep -Il .
git ls-files | where { $textFiles -notcontains $_ } | % { $_; $fail = $true }

if ($fail) {
	throw "Found restricted words or unexpected binary files"
}

echo "prebuild check succeeded"
