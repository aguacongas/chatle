call dnvm upgrade
call dnu restore %APPVEYOR_BUILD_FOLDER%
if %ERRORLEVEL% NEQ 0 (
  exit /b 0
)
