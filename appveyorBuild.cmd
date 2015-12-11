call npm install -g grunt-cli
call dnvm upgrade
call dnu restore --quiet
if %ERRORLEVEL% NEQ 0 (
  exit /b 0
)
