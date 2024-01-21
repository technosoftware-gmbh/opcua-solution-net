# Installation and Administration of .NET based OPC UA Applications

## Installation .NET

Please follow instructions in this [article](https://dotnet.microsoft.com/en-us/learn/dotnet/hello-world-tutorial/intro) to setup the dotnet command line environment for your platform.
As of today, .NET 8.0, .NET 7.0 or .NET 6.0 is required. The article describes the installation of .NET 8.0.101 for
Windows, Linux and macOS. This version also works with the OPC UA Client and Server Solutions we
provide in this GitHub repositories.

Please follow at least the sections:

* Intro
* Download and Install

to install the .NET SDK. You find the different .NET versions also [here](https://dotnet.microsoft.com/en-us/download/dotnet).

## Environments used for testing

This GitHub repository is automatically built with the following environments:

- Linux Ubuntu 22.04.03 LTS
  - .NET 8.0.1, .NET .NET 7.0.405 and .NET 6.0.418
  - MSBuild 17.8.3
- Mac OS X 12.7.2
  - .NET 8.0.1, .NET .NET 7.0.405 and .NET 6.0.418
  - MSBuild 17.8.3
* Microsoft Windows Server 2022 10.0.20348
  - .NET 8.0.1, .NET .NET 7.0.405 and .NET 6.0.418
  - MSBuild 17.8.3

##  Directory Structure
The repository contains the following basic directory layout:

- bin/
  - modelcompiler/

    Executable of OPC Foundation Model compiler
- doc

  Source used for building the online documentation
- documentation/

  - Installation.md

    Installation and Administration of .NET based OPC UA Applications
  
    Additional documentation like:
    - OPC_UA_Solution_NET_Introduction.pdf

      Introduction in Developing OPC UA Clients and OPC UA Servers with C# / VB.NET
    - OPC_UA_Client_Development_with_NET.pdf
  
      Tutorial for Developing OPC UA Clients with C# for .NET.
    - OPC_UA_Server_Development_with_NET.pdf
  
      Tutorial for Developing OPC UA Servers with C# for .NET.
    - OPC_UA_PubSub_Development_with_NET.pdf
    
      Tutorial for Developing OPC UA Servers with C# for .NET.
- examples/
  
  Sample applications using nuget packages for versions up to 2.3.3
- licenses/
  
  Licenses applying
- nuget/

  nuget specification files used to build the nuget packages
  - packages
  
    Local nuget package repository
- reference/

  Reference applications using the source of the solution. Also used for the unit tests
- schema/
  
  XSD files like the UAModelDesign.xsd used for the Model Designer.
- src/

  Source of all solution libraries (client, server, pubsub)
- Test/

  Source of the unit tests
- Workshop/

  OPC UA Workshop content as PDF

##  DLLs used by applications

The solution consists of the following main components:

- Technosoftware.UaCore.dll
- Technosoftware.UaBindings.Https.dll
- Technosoftware.UaConfiguration.dll
  
These DLLs are used by all applications using the solution. In addition, one or several of the following DLL’s might be required:

- Technosoftware.UaClient.dll
  
  Client Applications require this DLL.
- Technosoftware.UaServer.dll
  
  Server Applications require this DLL.
- Technosoftware.UaPubSub.dll
  
  PubSub Applications require this DLL.
  
Depending on which features server applications uses you also need to use one of the following DLLs:

- Technosoftware.UaBaseServer.dll
  
  Server Applications using the original features from V2.x require this DLL.
- Technosoftware.UaStandardServer.dll
  
  Advanced server Applications with more than one node manager require this DLL. Used from the reference applications.

These DLLs are delivered via NuGet Packages available [here](/nuget/packages):

- Technosoftware.UaSolution.UaCore
- Technosoftware.UaSolution.UaConfiguration
- Technosoftware.UaSolution.UaClient
- Technosoftware.UaSolution.UaServer
- Technosoftware.UaSolution.UaPubSub
- Technosoftware.UaSolution.UaBaseServer
- Technosoftware.UaSolution.UaStandardServer

## OPC UA Local Discovery Server
  
The Local Discovery Server (LDS) is a DiscoveryServer that maintains a list of all UA Servers and Gateways available on the host/PC that it runs on and is the UA equivalent to the OPC Classic OPCENUM interface.
An LDS is a service that runs in the background. UA Servers will periodically connect to the LDS and Register themselves as being available. This periodic activity means that the list of available UA servers is always current and means that a client can immediately connect to any of them (security permissions pending).
The OPC UA Local Discovery Server is an installation from the OPC Foundation and delivered as installation executable and as merge module. You can download it [here](https://opcfoundation.org/developer-tools/samples-and-tools-unified-architecture/local-discovery-server-lds/).

## Test your installation with .NET 8.0
The main OPC UA Solution can be found in the root of the repository and is named.

- TutorialSamples.sln

The solution contains several sample clients, as well as several sample server examples used by these
clients.

###  Prerequisites

Once the `dotnet` command is available, navigate to the root folder in your local copy of the repository and execute `dotnet restore 'TutorialSamples.sln'`.

This command restores the tree of dependencies.

### Start the server

1. Open a command prompt.
2. Navigate to the folder **examples/Simulation/SampleServer**.
3. To run the server sample type

   `dotnet run --no-restore --framework net8.0 --project SampleCompany.SampleServer.csproj --autoaccept`

   - The server is now running and waiting for connections.
   - The `--autoaccept` flag allows to auto accept unknown certificates and should only be used to simplify testing.

### Start the client
1. Open a command prompt.
2. Navigate to the folder **examples/Simulation/SampleClient**.
3. To run the client sample type
   `dotnet run --no-restore --framework net8.0 --project SampleCompany.SampleClient.csproj --autoaccept`
   
   - The client connects to the OPC UA console sample server running on the same host.
   - The `--autoaccept` flag allows to auto accept unknown certificates and should only be used to simplify testing.

4. If not using the `--autoaccept` auto accept option, on first connection, or after certificates were renewed, the server may have refused the client certificate. Check the server and client folder %LocalApplicationData%/OPC Foundation/pki/rejected for rejected certificates. To approve a certificate copy it to the %LocalApplicationData%/OPC Foundation/pki/trusted.

### Check the output
If everything was done correctly the client should show the following lines:

```
SampleCompany .NET Core OPC UA Sample Client
Connecting...
Accepted Certificate: CN=SampleCompany OPC UA Sample Server, C=CH, S=Aargau, O=SampleCompany, DC=macbook-pro
Accepted Certificate: CN=SampleCompany OPC UA Sample Server, C=CH, S=Aargau, O=SampleCompany, DC=macbook-pro
Browse address space.
Reading nodes...
   Read Value = {21.01.2024 10:52:32 | 21.01.2024 10:52:38 | Running | Opc.Ua.BuildInfo | 0 | } , StatusCode = Good
   Read Value = StartTime , StatusCode = Good
   Read Value = 21.01.2024 10:52:32 , StatusCode = Good
Read a single value from node ns=2;s=Scalar_Simulation_Number.
   Node ns=2;s=Scalar_Simulation_Number Value = 54490 StatusCode = Good.
Read multiple values from different nodes.
   Node ns=2;s=Scalar_Simulation_Number Value = 54490 StatusCode = Good.
   Node ns=2;s=Scalar_Static_Integer Value = 33 StatusCode = Good.
   Node ns=2;s=Scalar_Static_Double Value = 14438842530529280 StatusCode = Good.
Read multiple values asynchronous.
Running...Press Ctrl-C to exit...
Status of Read of Node ns=2;s=Scalar_Simulation_Number is: 54490
Status of Read of Node ns=2;s=Scalar_Static_Integer is: 33
Status of Read of Node ns=2;s=Scalar_Static_Double is: 14438842530529280
--- SIMULATE RECONNECT --- 
Accepted Certificate: CN=SampleCompany OPC UA Sample Server, C=CH, S=Aargau, O=SampleCompany, DC=macbook-pro
--- RECONNECTED ---
```