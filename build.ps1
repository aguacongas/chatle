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
