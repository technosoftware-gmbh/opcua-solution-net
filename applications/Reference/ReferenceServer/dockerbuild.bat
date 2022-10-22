REM build a docker container of the console reference server
set buildoptions=--configuration Release -p:NoHttps=true --framework net6.0
dotnet build %buildoptions% Technosoftware.ReferenceServer.csproj
dotnet publish %buildoptions% Technosoftware.ReferenceServer.csproj -o ./publish
docker build -f Dockerfile -t referenceserver .
