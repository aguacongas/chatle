$ErrorActionPreference = "Stop"

function exec($cmd) {
    $cmdName = [IO.Path]::GetFileName($cmd)
    Write-Host -ForegroundColor Cyan "> $cmdName $args"
    & $cmd @args
}

cd $PSScriptRoot

$dotnetLocalInstallFolder = $env:DOTNET_INSTALL_DIR
if (!$dotnetLocalInstallFolder)
{
    $dotnetLocalInstallFolder = "$env:LOCALAPPDATA\Microsoft\dotnet\"
}

$newPath = "$dotnetLocalInstallFolder;$env:PATH"
if (!($env:Path.Split(';') -icontains $dotnetLocalInstallFolder))
{
    Write-Host "Adding $dotnetLocalInstallFolder to PATH"
    $env:Path = "$newPath"
}

exec dotnet publish .\src\chatle --configuration Release -o ..\..\artifacts\chatle

7z a .\artifacts\chatle.zip .\artifacts\chatle\ > null