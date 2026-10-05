#!/bin/bash

# Define variables (no spaces around '=')
projectPath="./Restaurant-API/Restaurant-API.csproj"
devProfile="Api [DEV]"
prodProfile="Api [PROD]"
migrationProfile="Server [MIGRATION]"

echo "Launching API with profile: $devProfile"

# Run the command
dotnet run --project "$projectPath" --launch-profile "$devProfile" --verbosity quiet