export DOTNET_INSTALL_DIR="$PWD/.dotnetcli"
./dotnet-install.sh -Channel Current -Version 3.0.100-preview4-011223 --install-dir $DOTNET_INSTALL_DIR
export PATH="$DOTNET_INSTALL_DIR:$PATH"