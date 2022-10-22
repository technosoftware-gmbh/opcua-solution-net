@echo off
echo Run the local docker container of the Technosoftware.ReferenceServer
echo By default, the certificate store of the ref server is mapped to '.\OPC Foundation\pki'
echo A log file is created at '.\Technosoftware\Logs\Technosoftware.ReferenceServer.log.txt'
echo A shadow configuration file for customization is created in '.\Technosoftware\Technosoftware.ReferenceServer.Config.xml'
docker run -it -p 56241:56241 -h %COMPUTERNAME% -v "%CD%/OPC Foundation:/root/.local/share/Technosoftware" Technosoftware.ReferenceServer:latest -c -s
