#!/usr/bin/env bash
repoFolder="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $repoFolder

koreBuildZip="https://github.com/aspnet/KoreBuild/archive/dev.zip"
if [ ! -z $KOREBUILD_ZIP ]; then
    koreBuildZip=$KOREBUILD_ZIP
fi

buildFolder=".build"
buildFile="$buildFolder/KoreBuild.sh"

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

    chmod +x $buildFile

    # Cleanup
    if test -d $tempFolder; then
        rm -rf $tempFolder
    fi
fi

#!/usr/bin/env bash
set -o pipefail

# Colors
GREEN="\033[1;32m"
CYAN="\033[0;36m"
RESET="\033[0m"
RED="\033[0;31m"


# functions

__exec() {
    local cmd=$1
    shift

    local cmdname=$(basename $cmd)
    echo -e "${CYAN}> $cmdname $@${RESET}"

    if [ -z "${TRAVIS}" ]; then
        $cmd "$@"
    else
        # Work around https://github.com/Microsoft/msbuild/issues/1792
        $cmd "$@" | tee /dev/null
    fi

    local exitCode=$?
    if [ $exitCode -ne 0 ]; then
        echo -e "${RED}'$cmdname $@' failed with exit code $exitCode${RESET}" 1>&2
        exit $exitCode
    fi
}

msbuild_args=""
while [[ $# > 0 ]]; do
    case $1 in
        -r)
            shift
            repoFolder=$1
            ;;
        *)
            msbuild_args+="\"$1\"\n"
            ;;
    esac
    shift
done
if [ ! -e "$repoFolder" ]; then
    printf "Usage: $filename -r [repoFolder] [ [msbuild-args] ]\n\n"
    echo "       -r [repo]       The repository to build"
    echo "       [msbuild-args]  A space separated list of arguments to pass to MSBuild"
    exit 1
fi

scriptRoot="$repoFolder/.build"

if [ "$(uname)" == "Darwin" ]; then
    ulimit -n 2048

    # Check that OS is 10.12 or newer
    osx_version="$(sw_vers | grep ProductVersion | awk '{print $2}')"
    minor_version="$(echo $osx_version | awk -F '.' '{print $2}')"
    if [ $minor_version -lt 12 ]; then
        echo -e "${RED}.NET Core 2.0 requires OSX 10.12 or newer. Current version is $osx_version.${RESET}"
        exit 1
    fi
fi

versionFile="$scriptRoot/cli.version"
version=$(<$versionFile)
sharedRuntimeVersionFile="$scriptRoot/shared-runtime.version"
sharedRuntimeVersion=$(<$sharedRuntimeVersionFile)

[ -z "$KOREBUILD_DOTNET_CHANNEL" ] && KOREBUILD_DOTNET_CHANNEL="preview"
[ -z "$KOREBUILD_DOTNET_VERSION" ] && KOREBUILD_DOTNET_VERSION=$version
[ -z "$KOREBUILD_DOTNET_SHARED_RUNTIME_CHANNEL" ] && KOREBUILD_DOTNET_SHARED_RUNTIME_CHANNEL="master"
[ -z "$KOREBUILD_DOTNET_SHARED_RUNTIME_VERSION" ] && KOREBUILD_DOTNET_SHARED_RUNTIME_VERSION=$sharedRuntimeVersion

install_shared_runtime() {
    eval $invocation

    local version=$1
    local channel=$2

    local sharedRuntimePath="$DOTNET_INSTALL_DIR/shared/Microsoft.NETCore.App/$version"
    if [ ! -d "$sharedRuntimePath" ]; then
        $scriptRoot/dotnet/dotnet-install.sh \
            --shared-runtime \
            --channel $channel \
            --version $version

        if [ $? -ne 0 ]; then
            exit 1
        fi
    fi
}

if [ ! -z "$KOREBUILD_SKIP_RUNTIME_INSTALL" ]; then
    echo "Skipping runtime installation because KOREBUILD_SKIP_RUNTIME_INSTALL is set"

    # Add .NET installation directory to the path if it isn't yet included.
    # Add to the _end_ in case preferred .NET CLI is not in the default location.
    [[ ":$PATH:" != *":$DOTNET_INSTALL_DIR:"* ]] && export PATH="$PATH:$DOTNET_INSTALL_DIR"
else
    # Need to set this variable because by default the install script
    # requires sudo
    [ -z "$DOTNET_INSTALL_DIR" ] && DOTNET_INSTALL_DIR=~/.dotnet
    export DOTNET_INSTALL_DIR=$DOTNET_INSTALL_DIR
    chmod +x $scriptRoot/dotnet/dotnet-install.sh

    $scriptRoot/dotnet/dotnet-install.sh \
        --channel $KOREBUILD_DOTNET_CHANNEL \
        --version $KOREBUILD_DOTNET_VERSION

    if [ $? -ne 0 ]; then
        exit 1
    fi

    # Add .NET installation directory to the path if it isn't yet included.
    [[ ":$PATH:" != *":$DOTNET_INSTALL_DIR:"* ]] && export PATH="$DOTNET_INSTALL_DIR:$PATH"
fi

# Temporarily install 1.1.1 to prevent build breaks for repos not yet converted
install_shared_runtime "1.1.1" "release/1.1.0"

if [ "$sharedRuntimeVersion" != "" ]; then
    install_shared_runtime $KOREBUILD_DOTNET_SHARED_RUNTIME_VERSION $KOREBUILD_DOTNET_SHARED_RUNTIME_CHANNEL
fi

# workaround for CLI issue: https://github.com/dotnet/cli/issues/2143
DOTNET_PATH=`which dotnet | head -n 1`
ROOT_PATH=`dirname $DOTNET_PATH`
FOUND=`find $ROOT_PATH/shared -name dotnet`
if [ ! -z "$FOUND" ]; then
    echo $FOUND | xargs rm
fi

netfxversion='4.6.1'
if [ "$NUGET_PACKAGES" == "" ]; then
    NUGET_PACKAGES="$HOME/.nuget/packages"
fi
export ReferenceAssemblyRoot=$NUGET_PACKAGES/netframeworkreferenceassemblies/$netfxversion/content

nugetPath="$scriptRoot/nuget.exe"
if [ ! -f $nugetPath ]; then
    nugetUrl="https://dist.nuget.org/win-x86-commandline/v4.0.0-rc4/NuGet.exe"
    wget -O $nugetPath $nugetUrl 2>/dev/null || curl -o $nugetPath --location $nugetUrl 2>/dev/null
fi

makeFileProj="build/build.csproj"

__exec dotnet restore "$makeFileProj"
__exec dotnet build "$makeFileProj" -f netcoreapp1.1
$buildFile -r $repoFolder "$@"