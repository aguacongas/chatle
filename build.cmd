@echo off
cd %~dp0

SETLOCAL
SET NUGET_VERSION=latest
SET CACHED_NUGET=%LocalAppData%\NuGet\nuget.%NUGET_VERSION%.exe
SET BUILDCMD_KOREBUILD_VERSION=
SET BUILDCMD_DNX_VERSION=1.0.0-rc1-update1
SET BUILDCMD_DNX_RUNTIME=clr
SET BUILDCMD_DNX_ARCHITECTURE=x64
SET DNX_FEED=https://www.nuget.org/api/v2


IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/%NUGET_VERSION%/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST .nuget\nuget.exe goto restore
md .nuget
copy %CACHED_NUGET% .nuget\nuget.exe > nul

:restore
IF EXIST packages\Sake goto getdnx
IF "%BUILDCMD_KOREBUILD_VERSION%"=="" (
    .nuget\nuget.exe install KoreBuild -ExcludeVersion -o packages -nocache -pre
) ELSE (
    .nuget\nuget.exe install KoreBuild -version %BUILDCMD_KOREBUILD_VERSION% -ExcludeVersion -o packages -nocache -pre
)
.nuget\NuGet.exe install Sake -ExcludeVersion -Source https://www.nuget.org/api/v2/ -Out packages
.nuget\NuGet.exe install OpenCover -ExcludeVersion -Source https://www.nuget.org/api/v2/ -Out packages
.nuget\NuGet.exe install coveralls.io -ExcludeVersion -Source https://www.nuget.org/api/v2/  -Out packages
.nuget\NuGet.exe install ReportGenerator -ExcludeVersion -Source https://www.nuget.org/api/v2/ -Out packages

:getdnx
    CALL packages\KoreBuild\build\dnvm install %BUILDCMD_DNX_VERSION% -a %BUILDCMD_DNX_ARCHITECTURE% -r coreclr -Alias core
    CALL packages\KoreBuild\build\dnvm install %BUILDCMD_DNX_VERSION% -a %BUILDCMD_DNX_ARCHITECTURE% -r clr -Alias default -p

packages\Sake\tools\Sake.exe -I packages\KoreBuild\build -f makefile.shade %*