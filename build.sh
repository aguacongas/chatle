#!/usr/bin/env bash
repoFolder="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $repoFolder

koreBuildZip="https://github.com/aspnet/KoreBuild/archive/dev.zip"
if [ ! -z $KOREBUILD_ZIP ]; then
    koreBuildZip=$KOREBUILD_ZIP
fi

buildFolder="$repoFolder/.build"
echo "buildfolder is : $buildFolder"

if test ! -d $buildFolder; then
    echo "Downloading KoreBuild from $koreBuildZip"
    
    tempFolder="/tmp/KoreBuild-$(uuidgen)"    
    mkdir $tempFolder
    
    localZipFile="$tempFolder/korebuild.zip"
    
    retries=6
    until (wget -O $localZipFile $koreBuildZip 2>/dev/null || curl -o $localZipFile --location $koreBuildZip 2>/dev/null)
    do
        echo "Failed to download '$koreBuildZip'"
        if [ "$retries" -le 0 ]; then
            exit 1
        fi
        retries=$((retries - 1))
        echo "Waiting 10 seconds before retrying. Retries left: $retries"
        sleep 10s
    done
    
    unzip -q -d $tempFolder $localZipFile
  
    mkdir $buildFolder
    cp -r $tempFolder/**/build/** $buildFolder
    
    # Cleanup
    if test ! -d $tempFolder; then
        rm -rf $tempFolder  
    fi
fi

#!/usr/bin/env bash

echo "Building $repoFolder"

# Make the path relative to the repo root because Sake/Spark doesn't support full paths
koreBuildFolder=$buildFolder

if test `uname` = Darwin; then
    versionFileName="cli.version.darwin"
else
    versionFileName="cli.version.unix"
fi

version=$(<$versionFileName)

[ -z "$KOREBUILD_DOTNET_CHANNEL" ] && KOREBUILD_DOTNET_CHANNEL=rel-1.0.0
[ -z "$KOREBUILD_DOTNET_VERSION" ] && KOREBUILD_DOTNET_VERSION=$version

if [ ! -z "$KOREBUILD_SKIP_RUNTIME_INSTALL" ]; then
    echo "Skipping runtime installation because KOREBUILD_SKIP_RUNTIME_INSTALL is set"

    # Add .NET installation directory to the path if it isn't yet included.
    # Add to the _end_ in case preferred .NET CLI is not in the default location.
    [[ ":$PATH:" != *":$DOTNET_INSTALL_DIR:"* ]] && export PATH="$PATH:$DOTNET_INSTALL_DIR"
else
    # Need to set this variable because by default the install script
    # requires sudo
    export DOTNET_INSTALL_DIR=~/.dotnet
    export KOREBUILD_FOLDER="$(dirname $koreBuildFolder)"
    chmod +x $koreBuildFolder/dotnet/dotnet-install.sh

    $koreBuildFolder/dotnet/dotnet-install.sh --channel $KOREBUILD_DOTNET_CHANNEL --version $KOREBUILD_DOTNET_VERSION

    # Add .NET installation directory to the path if it isn't yet included.
    [[ ":$PATH:" != *":$DOTNET_INSTALL_DIR:"* ]] && export PATH="$DOTNET_INSTALL_DIR:$PATH"
fi


# workaround for CLI issue: https://github.com/dotnet/cli/issues/2143
DOTNET_PATH=`which dotnet | head -n 1`
ROOT_PATH=`dirname $DOTNET_PATH`
FOUND=`find $ROOT_PATH/shared -name dotnet`
if [ ! -z "$FOUND" ]; then
    echo $FOUND | xargs rm
fi

if [ "$(uname)" == "Darwin" ]; then
    ulimit -n 2048
fi

sakeFolder=$koreBuildFolder/Sake
if [ ! -d $sakeFolder ]; then
    toolsProject="$koreBuildFolder/project.json"
    dotnet restore "$toolsProject" --packages $koreBuildFolder -v Minimal
    # Rename the project after restore because we don't want it to be restore afterwards
    mv "$toolsProject" "$toolsProject.norestore"
fi

netFrameworkFolder=$repoFolder/$koreBuildFolder/NETFrameworkReferenceAssemblies
netFrameworkContentDir=$netFrameworkFolder/4.5.1/content
if [ ! -d $netFrameworkFolder ]; then
    xplatToolsProject="$koreBuildFolder/xplat.project.json"
    dotnet restore "$xplatToolsProject" --packages $koreBuildFolder -v Minimal
    # Rename the project after restore because we don't want it to be restore afterwards
   mv $xplatToolsProject $xplatToolsProject.norestore
fi

export DOTNET_REFERENCE_ASSEMBLIES_PATH=$netFrameworkContentDir

nugetPath="$koreBuildFolder/nuget.exe"
if [ ! -f $nugetPath ]; then
    nugetUrl="https://dist.nuget.org/win-x86-commandline/v3.5.0-beta2/NuGet.exe"
    wget -O $nugetPath $nugetUrl 2>/dev/null || curl -o $nugetPath --location $nugetUrl 2>/dev/null
fi

makeFile="makefile.shade"

export KOREBUILD_FOLDER="$koreBuildFolder"

cd $repoFolder

toolsProject="project.json"
mv "$toolsProject.norestore" "$toolsProject"
dotnet restore "$toolsProject" --packages "packages" -v Minimal
mv "$toolsProject" "$toolsProject.norestore"

mono $sakeFolder/0.2.2/tools/Sake.exe -I $koreBuildFolder/shade -f $makeFile "$@"



