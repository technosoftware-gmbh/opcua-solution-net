REM collect a trace using the EventSource provider OPC-UA-Core and Technosoftware.UAServer
dotnet tool install --global dotnet-trace
dotnet-trace collect --name Technosoftware.ReferenceServer --providers OPC-UA-Core,Technosoftware.UAServer
