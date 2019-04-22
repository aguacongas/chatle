$result = 0

$location = Get-Location;

$envar = Get-Childitem env: -Name 

if (-not($envar -contains 'APPVEYOR_PULL_REQUEST_NUMBER'))
{
	if ($isLinux) {
		Get-ChildItem -rec `
		| Where-Object { $_.Name -like "*.IntegrationTest.csproj" `
			-Or $_.Name -like "*.Test.csproj" `
			} `
		| ForEach-Object { 
			Set-Location $_.DirectoryName
			dotnet test
		
			if ($LASTEXITCODE -ne 0) {
				$result = $LASTEXITCODE
			}
		}
	} else {
		Get-ChildItem -rec `
		| Where-Object { $_.Name -like "*.IntegrationTest.csproj" `
			-Or $_.Name -like "*.Test.csproj" `
			} `
		| ForEach-Object { 
			&('dotnet') ('test', $_.FullName, '--logger', "trx;LogFileName=$_.trx", '-c', 'Release', '/p:CollectCoverage=true', '/p:CoverletOutputFormat=cobertura', '/p:Exclude="[*]xunit*?%2c[*]System.Interactive*?%2c[*]MySqlConnector*"')    
			if ($LASTEXITCODE -ne 0) {
				$result = $LASTEXITCODE
			}
		}
	}
} else {
	Get-ChildItem -rec `
	| Where-Object { $_.Name -like "*.Test.csproj" } `
	| ForEach-Object { 
		Set-Location $_.DirectoryName
		&('dotnet') ('test', $_.FullName, '--logger', "trx;LogFileName=$_.trx", '-c', 'Release', '/p:CollectCoverage=true', '/p:CoverletOutputFormat=cobertura')    
	
		if ($LASTEXITCODE -ne 0) {
			$result = $LASTEXITCODE
		}
	}	
}
Set-Location $location;
exit $result
