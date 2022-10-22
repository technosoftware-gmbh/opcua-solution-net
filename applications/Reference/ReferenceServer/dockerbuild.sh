#!/bin/bash
echo build a docker container of the console reference server, without https support
buildoptions="--configuration Release -p:NoHttps=true --framework net6.0"
dotnet build $buildoptions Technosoftware.ReferenceServer.csproj
dotnet publish $buildoptions Technosoftware.ReferenceServer.csproj -o ./publish
sudo docker build -f Dockerfile -t referenceserver .
