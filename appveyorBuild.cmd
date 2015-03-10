set PATH=%USERPROFILE%/.k/bin;%PATH%
set KRE_HOME=%USERPROFILE%/.k
kvm upgrade
git submodule update --init --recursive
set PATH=C:\Program Files (x86)\MSBuild\14.0\Bin;%PATH%
kpm restore %APPVEYOR_BUILD_FOLDER%
if %ERRORLEVEL% NEQ 0 (
   exit /b 0
)