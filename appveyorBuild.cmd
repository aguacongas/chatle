call npm install -g grunt-cli
call dnvm install latest -a x64 -default -p 
call dnu restore --quiet
if %ERRORLEVEL% NEQ 0 (
  exit /b 0
)
