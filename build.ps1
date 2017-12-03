gci -Path src -rec `
| ? { $_.Name -like "*.csproj" `
     } `
| % { 
    dotnet msbuild $_.FullName -t:Build -p:Configuration=Release -p:Version=$env:GitVersion_NuGetVersion -p:OutputPath=..\..\artifacts\build -p:GeneratePackageOnBuild=true
    if ($LASTEXITCODE -ne 0) {
            throw "build failed" + $d.FullName
    }
  }

gci -rec `
| ? { $_.Name -like "*.IntegrationTests.csproj" `
       -Or $_.Name -like "*.IntegrationTest.csproj" `
       -Or $_.Name -like "*.Test.csproj" `
       -Or $_.Name -like "*.Tests.csproj" `
     } `
| % { 
    dotnet test $_.FullName -l trx`;logfilename=Test_Results.trx 
    if ($LASTEXITCODE -ne 0) {
            throw "test failed" + $d.FullName
    }
  }
