$ErrorActionPreference = "Stop"

function exec($cmd) {
    $cmdName = [IO.Path]::GetFileName($cmd)
    Write-Host -ForegroundColor Cyan "> $cmdName $args"
    & $cmd @args
}

dotnet msbuild ./src/chatle -t:Publish -p:Configuration=Release -p:OutputPath=..\..\artifacts\chatle -p:Version=$env:GitVersion_NuGetVersion -p:FileVersion=$env:GitVersion_AssemblySemVer -p:FileVersion=$env:GitVersion_AssemblySemVer

7z a .\artifacts\chatle.zip .\artifacts\chatle\ > null

dotnet build .\src\dotnet-encrypt -c Release

gci -Path src -rec `
| ? { $_.Name -like "*.csproj" -and $_.Name -ne "chatle.angular.csproj" ` -and $_.Name -ne "DotZLib.csproj"
     } `
| % { 
    dotnet msbuild $_.FullName -t:Build -p:Configuration=Release -p:OutputPath=..\..\artifacts\build -p:GeneratePackageOnBuild=true -p:Version=$env:GitVersion_NuGetVersion -p:FileVersion=$env:GitVersion_AssemblySemVer -p:FileVersion=$env:GitVersion_AssemblySemVer
    if ($LASTEXITCODE -ne 0) {
            throw "build failed" + $d.FullName
    }
  }
