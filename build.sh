#!/bin/bash
mkdir -p ./publish

# dotnet publish ./src/CryptoVote-Core.sln -r win-x64 -c release
# rm -r ./publish/cryptovote-win-x64
# mv ./src/CryptoVote/bin/Release/netcoreapp3.1/win-x64/publish ./publish/cryptovote-win-x64
# zip -r -m -j ./publish/cryptovote-win-x64.zip ./publish/cryptovote-win-x64
# rm -r ./publish/cryptovote-win-x64

# dotnet publish ./src/CryptoVote-Core.sln -r linux-arm -c release
# rm -r ./publish/cryptovote-linux-arm
# mv ./src/CryptoVote/bin/Release/netcoreapp3.1/linux-arm/publish ./publish/cryptovote-linux-arm
# tar -C ./publish/cryptovote-linux-arm -cvf ./publish/cryptovote-linux-arm.tar.gz .
# rm -r ./publish/cryptovote-linux-arm

dotnet publish ./src/CryptoVote-Core.sln -r linux-x64 -c release
rm -r ./publish/cryptovote-linux-x64
mv ./src/CryptoVote/bin/Release/netcoreapp3.1/linux-x64/publish ./publish/cryptovote-linux-x64
tar -C ./publish/cryptovote-linux-x64 -czvf ./publish/cryptovote-linux-x64.tar.gz .
rm -r ./publish/cryptovote-linux-x64

cp ./Dockerfile ./publish
cp ./docker-compose.yml ./publish
