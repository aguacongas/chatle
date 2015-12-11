call dnvm upgrade
call dnu restore
if %ERRORLEVEL% NEQ 0 (
  exit /b 0
)
