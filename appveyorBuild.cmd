set PATH=%USERPROFILE%/.k/bin;C:\Program Files (x86)\MSBuild\14.0\Bin;%PATH%
set KRE_HOME=%USERPROFILE%/.k
call kvm upgrade
call git submodule update --init --recursive
cd %APPVEYOR_BUILD_FOLDER%\EntityFramework
call git checkout beta3
call git submodule update --init --recursive
call kpm restore %APPVEYOR_BUILD_FOLDER%
