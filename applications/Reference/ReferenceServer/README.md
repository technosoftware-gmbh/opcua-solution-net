# OPC UA Solution .NET Reference Server

This document is referring to the CTT version 1.04.9.401.

## Introduction
This OPC UA Server is designed to be easily usable with the the [OPC UA Compliance Test Tool](https://opcfoundation.org/developer-tools/certification-test-tools/opc-ua-compliance-test-tool-uactt/) and it uses an address-space that matches the design of the UACTT and the requirements for OPC UA compliance testing.. 

It supports both the opc.tcp and https transports. There is a .NET based console version of the server which runs on any OS supporting [.NET Standard](https://docs.microsoft.com/en-us/dotnet/articles/standard) or .NET 6.0 and later.

## How to build and run the Windows OPC UA Reference Server with UACTT
1. Open the solution **OpcUaNetReference.sln** with Visual Studio 2022. The solution can be found in the **/** folder
2. Choose the project `Technosoftware.ReferenceServer` in the Solution Explorer and set it with a right click as `Startup Project`.
3. Hit `F5` to build and execute the sample.

## How to build and run the console Technosoftware OPC UA Reference Server on Windows, Linux and MacOS.
This section describes how to run the **Technosoftware.ReferenceServer**.

Please follow instructions in this [article](https://aka.ms/dotnetcoregs) to setup the dotnet command line environment for your platform. 

## Start the server 
1. Open a command prompt.
2. Navigate to the folder **examples/Reference/ReferenceServer**.
3. To run the server sample type `dotnet run --project Technosoftware.ReferenceServer.csproj`. The server is now running and waiting for the connection of the UACTT. 

## UACTT test certificates
The reference server always rejects new client certificates and requires that the UACTT certificates are in appropriate folders. 
- The console server certificates are stored in **%LocalApplicationData%/OPC Foundation/pki**.
    - **%LocalApplicationData%** maps to a hidden location in the user home folder and depends on the target platform.

### Certificate stores
Under **pki**, the following stores contain certificates under **certs**, CRLs under **crl** or private keys under **private**.
- **own** contains the reference server public certificate and private key.
- **rejected** contains the rejected client certificates. To trust a client certificate, copy the rejected certificate to the **trusted/certs** folder.
- **trusted** contains *trusted* client and CAs certificates and CRLs.
- **issuer** contains CAs certificates and CRLs needed for validation.
- **trustedUser** contains *trusted* user and user CA certificates and CRLs.
- **issuerUser** contains user CA certificates and CRLs needed for validation of the certificate chains.

### Placing the UACTT certificates
CTT creates a **PKI** folder in a new project. There is a subfolder **copyToServer** which contains the application and user identity certificates.
Copy the **ApplicationInstance_PKI** certificates for testing with the UACTT to the following stores:
- **trusted/certs**: trusted certificates
- **trusted/crl**: revocation lists of trusted certificates
- **issuer/certs**: issuer certificates
- **issuer/crl**: revocation lists of issuer certificates

Copy the **X509UserIdentity_PKI** certificates for testing with the UACTT to the following stores:
- **trustedUser/certs**: trusted user certificates
- **trustedUser/crl**: revocation lists of trusted user certificates
- **issuerUser/certs**: user issuer certificates
- **issuerUser/crl**: revocation lists of user issuer certificates

## UACTT Testing
Download and install the [OPC UA Compliance Test Tool](https://opcfoundation.org/developer-tools/certification-test-tools/opc-ua-compliance-test-tool-uactt/). 

Note: Access to the UACTT is granted to OPC Foundation Corporate Members.

### UACTT sample configuration
A sample configuration for the UACTT Version [1.04.9.401](https://opcfoundation.org/developer-tools/certification-test-tools/opc-ua-compliance-test-tool-uactt/) can be found in [ReferenceServer.ctt.xml](../Compliance/ReferenceServer/ReferenceServer.ctt.xml). The reference server was tested against the **Standard UA Server** profile, the **Method Server Facet** and the **DataAccess Server Facet**, in addition to all security related profiles.

### Known missing Tests and Issues ###

Depending on the setup in the provided project configuration, unsupported profiles should be skipped. At the time of writing (1.4.9.401) the following tests, profiles or facets are not yet covered or not configured due to issues:
- no support for events, historian and Alarms & Conditions. 
- Decimal type causes crash of CTT (needs investigation)
- no Image / Enumeration scalar test cases
- Arrays may not include Variant (needs investigation)
- no ArrayItemType implemented
- multiple hierarchical references test node missing
- deprecated security profiles support for SHA1/1k RSA keys implemented not according to certification. SHA1/1k keys can only be turned on/off for all profiles, not just for the deprecated profiles.
- Monitored Items with array are failing a few test cases.
 
It is recommended to run the server as retail build with disabled logging, to avoid side effects due to timing artifacts when log entries are written to a disk drive. 

#### Finding the Address Space Configuration Code
- Project: Technosoftware.Servers
- File: ReferenceServerNodeManager.cs
- Method: CreateAddressSpace


