function Set-EnvironmentVariablesFromEnvFile {
    param(
        [string]$envFilePath = ".env"
    )

    # Check if the .env file exists
    if (-Not (Test-Path -Path $envFilePath)) {
        Write-Host ".env file not found at path: $envFilePath"
        return
    }

	# Read the .env file line by line
	$lines = Get-Content -Path $envFilePath

	foreach ($line in $lines) {
		# Skip empty lines and comments
		if (-not $line.Trim() -or $line.StartsWith('#')) {
			continue
		}

		# Split the line into key and value
		$parts = $line -split '=', 2
		if ($parts.Length -eq 2) {
			$key = $parts[0].Trim()
			$value = $parts[1].Trim()

			# Set the environment variable
			[System.Environment]::SetEnvironmentVariable($key, $value, [System.EnvironmentVariableTarget]::Process)
			Write-Host "Set environment variable: $key=$value"
		} else {
			Write-Host "Skipping invalid line: $line"
		}
	}
}

Set-EnvironmentVariablesFromEnvFile ".env"
Set-EnvironmentVariablesFromEnvFile ".env.local"
