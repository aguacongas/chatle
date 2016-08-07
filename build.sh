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
    if test ! -d $tempFolder; then
        rm -rf $tempFolder  
    fi
fi

nugetPath="$koreBuildFolder/nuget.exe"
if [ ! -f $nugetPath ]; then
    nugetUrl="https://dist.nuget.org/win-x86-commandline/v3.5.0-beta2/NuGet.exe"
    wget -O $nugetPath $nugetUrl 2>/dev/null || curl -o $nugetPath --location $nugetUrl 2>/dev/null
fi

nuget="mono $buildFolder/nuget.exe"

$nuget install GitVersion.CommandLine -ExcludeVersion -Source https://www.nuget.org/api/v2/ -Out packages

cliversion="1.0.0-preview3-003206"
clifile="$buildFolder/cli.version"
if [ -f "$clifile" ]; then
    rm $clifile
fi

echo $cliversion >> $clifile
cp $clifile "$buildFolder/cli.version.darwin"
cp $clifile "$buildFolder/cli.version.unix"


$buildFile -r $repoFolder "$@"