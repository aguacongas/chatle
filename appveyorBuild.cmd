call npm install -g bower
call npm install -g gulp
call dnvm install latest -a x64 -r coreclr -Alias core
call dnvm install latest -a x64 -r clr -Alias default -p
call dnu restore --quiet
if %ERRORLEVEL% NEQ 0 (
  exit /b 0
)
