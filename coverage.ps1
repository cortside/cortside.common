dotnet tool install --global dotnet-reportgenerator-globaltool
dotnet tool install --global dotnet-coverage

dotnet test src --collect:"XPlat Code Coverage" --settings src/coverlet.runsettings.xml
dotnet-coverage merge *.cobertura.xml --recursive --output coverage.cobertura.xml --output-format cobertura
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

rm coverage.cobertura.xml
