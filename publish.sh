#! /usr/bin/env nix-shell
#! nix-shell -i bash -p dotnet-sdk_7 zip gnutar
# shellcheck shell=bash
set -eu

rm -r .publish/ archives/ || true
mkdir archives/

for rid in "win-x64" "win-arm64" "linux-x64" "linux-arm64" "osx-x64" "osx-arm64"; do
    echo Publishing $rid version...
    if dotnet publish --runtime "$rid" --configuration Release --output ".publish/$rid/" --self-contained src/CLI/LuaPack.CLI.csproj; then
        zip -r "archives/$rid.zip" ".publish/$rid/"
        tar czf "archives/$rid.tar.gz" ".publish/$rid/"
    fi
done