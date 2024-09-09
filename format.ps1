# install/update latest version
dotnet tool update -g dotnet-format --version "8.*" --add-source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet8/nuget/v3/index.json

# Format code to match editorconfig settings
#dotnet format --verbosity normal .\src

# per readme and because of dotnet cli issues, executing via command directly is best for now
dotnet-format --verbosity normal .\src
