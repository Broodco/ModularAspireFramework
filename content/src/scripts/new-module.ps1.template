# src/scripts/new-module.ps1
param(
    [Parameter(Mandatory=$true)]
    [string]$ModuleName
)

dotnet run --project ./src/ModularAspire.ModuleGenerator/ModularAspire.ModuleGenerator.csproj -- --name $ModuleName