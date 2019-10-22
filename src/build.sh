#!/bin/bash
mkdir -p ./publish

dotnet publish core.sln -r linux-arm -c release
rm -r ./publish/core-linux-arm
mv ./CryptoVote/bin/Release/netcoreapp2.2/linux-arm/publish ./publish/core-linux-arm

dotnet publish core.sln -r win-x64 -c release
rm -r ./publish/core-win-x64
mv ./CryptoVote/bin/Release/netcoreapp2.2/win-x64/publish ./publish/core-win-x64

dotnet publish website.sln -r linux-arm -c release
rm -r ./publish/web-linux-arm
mv ./Web/bin/Release/netcoreapp2.2/linux-arm/publish ./publish/web-linux-arm

dotnet publish website.sln -r win-x64 -c release
rm -r ./publish/web-win-x64
mv ./Web/bin/Release/netcoreapp2.2/win-x64/publish ./publish/web-win-x64