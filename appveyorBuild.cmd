call npm install -g grunt-cli
call dnvm install latest -a x64 -r clr -Alias clr
call dnvm install latest -a x64 -r coreclr -Alias core -p
call dnvm list
call dnu restore --quiet
if %ERRORLEVEL% NEQ 0 (
  exit /b 0
)
