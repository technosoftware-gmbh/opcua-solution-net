# OPC UA Server SDK .NET Standard Reference Server

## Introduction
This OPC UA Server is designed to be easily usable with the the [OPC UA Compliance Test Tool](https://opcfoundation.org/developer-tools/certification-test-tools/opc-ua-compliance-test-tool-uactt/) and it uses an address-space that matches the design of the UACTT and the requirements for OPC UA compliance testing.. 

It supports both the opc.tcp and https transports. There is a .NET Core 3.1 based console version of the server which runs on any OS supporting [.NET Standard 2.1](https://docs.microsoft.com/en-us/dotnet/articles/standard).

## How to build and run the Windows OPC UA Reference Server with UACTT
1. Open the solution **OpcUaNetReference.sln** with Visual Studio 2019. The solution can be found in the **/** folder
2. Choose the project `Technosoftware.ReferenceServer` in the Solution Explorer and set it with a right click as `Startup Project`.
3. Hit `F5` to build and execute the sample.

## How to build and run the console Technosoftware OPC UA Reference Server on Windows, Linux and MacOS.
This section describes how to run the **Technosoftware.ReferenceServer**.

Please follow instructions in this [article](https://aka.ms/dotnetcoregs) to setup the dotnet command line environment for your platform. 

## Start the server 
1. Open a command prompt.
2. Navigate to the folder **examples/Reference/ReferenceServer**.
3. To run the server sample type `dotnet run --framework netcoreapp3.1 --project Technosoftware.ReferenceServer.csproj`. The server is now running and waiting for the connection of the UACTT. 

## UACTT test certificates
The reference server always rejects new client certificates and requires that the UACTT certificates are in appropriate folders. 
- The console server certificates are stored in **%LocalApplicationData%/OPC Foundation/CertificateStores**.
    - **%LocalApplicationData%** maps to a hidden location in the user home folder and depends on the target platform.

### Certificate stores
Under **pki**, the following stores contain certificates under **certs**, CRLs under **crl** or private keys under **private**.
- **own** contains the reference server public certificate and private key.
- **rejected** contains the rejected client certificates. To trust a client certificate, copy the rejected certificate to the **trusted/certs** folder.
- **trusted** contains *trusted* client and CAs certificates and CRLs.
- **issuer** contains CAs certificates and CRLs needed for validation.

Not yet tested are:
- **trustedUser** contains *trusted* user x509 certificates and CRLs.
- **issuerUser** contains CAs certificates and CRLs needed for validation of the user x509 certificates.

## UACTT project
For testing we used the version 1.4.9.396 of the OPC UA Compliance Test Tool and we deliver the cimplete project settings in the folder **\examples\Reference\Compliance\ReferenceServer**.

### Placing the UACTT certificates
Using Windows Explorer (or equivalent file browser) open your project directory and go to the **PKI->copyToServer->ApplicationInstance_PKI** folder, and copy all the included files into the appropriate folders in the application instance certificate PKI structure on your server. For the Reference server, as explained above, this is **%LocalApplicationData%\OPC Foundation\pki**

Not yet tested are:
- Copy the **PKI->copyToServer->X509UserIdentity_PKI->issuers** to **issuerUser** 
- Copy the **PKI->copyToServer->X509UserIdentity_PKI->trusted** to **trustedUser** 

## UACTT Testing
Download and install the [OPC UA Compliance Test Tool](https://opcfoundation.org/developer-tools/certification-test-tools/ua-compliance-test-tool-uactt/). 

For testing we used the version 1.4.9.396 and we recommend the use of the same version as a starting point.

Note: Access to the UACTT is granted to OPC Foundation Corporate Members.

### UACTT sample configuration
A sample configuration for the UACTT Version [1.4.9.396](https://opcfoundation.org/developer-tools/certification-test-tools/ua-compliance-test-tool-uactt/) can be found in [ReferenceServer.ctt.xml](**/examples/Reference/Compliance/UA 1.04/ReferenceServer/ReferenceServer.ctt.xml**). The reference server is tested against the **Standard UA Server** profile, the **Method Server Facet** and the **DataAccess Server Facet**. It is recommended to run the server as retail build with disabled logging, to avoid side effects due to timing artifacts when log entries are written to a disk drive. 

#### Finding the Address Space Configuration Code
- Project: OpcUaNetReference.sln
- File: ReferenceServerNodeManager.cs
- Method: CreateAddressSpace


