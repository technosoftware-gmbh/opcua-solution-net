# Sample Application

The OPC UA Client .NET contains a sample client application, you can find it them in the /tutorials/SampleCompany/Simulation/SampleClient folder.

## Required NuGet packages

The SDK is divided into several DLL’s as shown in the picture below:

![](../images/OPCUANETArchitecture.png)

The DLLs are delivered as local NuGet Packages. The OPC UA Client .NET uses the following packages:

| **Name**                                       | **Description**                                                                                    |
|:-----------------------------------------------|:---------------------------------------------------------------------------------------------------|
| **Technosoftware.UaSolution.UaCore**           | The OPC UA Core Class Library.                                                                     |
| **Technosoftware.UaSolution.UaBindings.Https** | The OPC UA Https Binding Library.                                                                  |
| **Technosoftware.UaSolution.UaConfiguration**  | Contains configuration related classes like, e.g. ApplicationInstance.                             |
| **Technosoftware.UaSolution.UaClient**         | The OPC UA Client Class library containing the classes and methods usable for server development.  |

## Directory Structure

We provide an online help for the current version: [OPC UA Solution NET Online Help](https://technosoftware.com/help/OPCUaSolutionNet/33/) which also contains updated information about the directory structure.

## OPC UA Client Solution for .NET

The main OPC UA Solution can be found in the root of the repository and is named.

-   Tutorials.sln

The solution contains a sample client, as well as a sample server used by this client.

## Prerequisites

Once the dotnet command is available, navigate to the root folder in your local copy of the repository / and execute the following command:

dotnet restore /p:Configuration=Debug /p:Platform="Any CPU" Tutorials.sln

This command restores the tree of dependencies.

## Start the server

1.  Open a command prompt.
2.  Navigate to the folder tutorials/SampleCompany/Simulation/SampleServer.
3.  To run the server sample type  
       
    dotnet run --no-restore --framework net8.0 --project SampleCompany.SampleServer.csproj --autoaccept
    -   The server is now running and waiting for connections.
    -   The --autoaccept flag allows to auto accept unknown certificates and should only be used to simplify testing.

## Start the client

1.  Open a command prompt.
2.  Navigate to the folder tutorials/SampleCompany/Simulation/SampleClient.
3.  To run the client sample type   
      
    dotnet run --no-restore --framework net8.0 --project SampleCompany.SampleClient.csproj --autoaccept
    -   The client connects to the OPC UA console sample server running on the same host.
    -   The --autoaccept flag allows to auto accept unknown certificates and should only be used to simplify testing.
4.  If not using the --autoaccept auto accept option, on first connection, or after certificates were renewed, the server may have refused the client certificate. Check the server and client folder %LocalApplicationData%/OPC Foundation/pki/rejected for rejected certificates. To approve a certificate copy it to the %LocalApplicationData%/OPC Foundation/pki/trusted.

## Check the output

If everything was done correctly the client should show the following lines:

```
OPC UA Console Sample Client
Connecting to... opc.tcp://localhost:62555/SampleServer
New Session Created with SessionName = SampleCompany OPC UA Sample Client
Connected! Ctrl-C to quit.
Reading nodes...
Read Value = {28.01.2024 07:20:20 \| 28.01.2024 07:21:33 \| Running \| Opc.Ua.BuildInfo \| 0 \| } , StatusCode = Good
Read Value = StartTime , StatusCode = Good
Read Value = 28.01.2024 07:20:20 , StatusCode = Good
Reading Value of NamespaceArray node...
NamespaceArray Value = {http://opcfoundation.org/UA/\|urn:technosoftware:SampleCompany:SampleServer\|http://samplecompany.com/SampleServer/NodeManagers/Simulation\|http://opcfoundation.org/UA/Diagnostics}
Writing nodes...
Write Results :
Good
Good
Good
Browsing i=2253 node...
Browse returned 19 results:
You can abort the running application with Ctrl-C.
```