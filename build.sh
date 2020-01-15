#!/bin/bash
mkdir -p ./publish

#dotnet publish ./src/All.sln -r win-x64 -c release
#rm -r ./publish/crypto-vote-win-x64
#mv ./src/crypto-vote/bin/Release/netcoreapp3.1/win-x64/publish ./publish/crypto-vote-win-x64
#zip -r -m -j ./publish/crypto-vote-win-x64.zip ./publish/crypto-vote-win-x64
#rm -r ./publish/crypto-vote-win-x64

#dotnet publish ./src/All.sln -r linux-arm -c release
#rm -r ./publish/crypto-vote-linux-arm
#mv ./src/crypto-vote/bin/Release/netcoreapp3.1/linux-arm/publish ./publish/crypto-vote-linux-arm
#tar -C ./publish/crypto-vote-linux-arm -cvf ./publish/crypto-vote-linux-arm.tar.gz .
#rm -r ./publish/crypto-vote-linux-arm

dotnet publish ./src/All.sln -r linux-x64 -c release
rm -r ./publish/crypto-vote-linux-x64
mv ./src/crypto-vote/bin/Release/netcoreapp3.1/linux-x64/publish ./publish/crypto-vote-linux-x64
tar -C ./publish/crypto-vote-linux-x64 -czvf ./publish/crypto-vote-linux-x64.tar.gz .
rm -r ./publish/crypto-vote-linux-x64


cp ./Dockerfile ./publish
cp ./docker-compose.yml ./publish
