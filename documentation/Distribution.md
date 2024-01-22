# Distribution

## NuGet packages

The OPC UA Solution .NET is delivered as a set of [NuGet packages](/nuget/packages). Depending on the targeted .NET Framework you have the following packages available:

### .NET 8.0, .NET 7.0, .NET 6.0 or .NET 4.8

- Technosoftware.UaSolution.UaClient

  The OPC UA Client .NET offers a fast and easy access to the OPC UA Client technology. Develop OPC UA compliant UA Clients with C#.

- Technosoftware.UaSolution.UaServer

  The OPC UA Server .NET offers a fast and easy access to the OPC Unified Architecture (UA) technology. Develop OPC UA&nbsp;compliant Servers with C#.

- Technosoftware.UaSolution.UaPubSub

  The OPC UA PubSub .NET offers a fast and easy access to the OPC Unified Architecture (UA) technology. Develop OPC UA compliant Publisher and Subscriber with C#.

- Technosoftware.UaSolution.UaConfiguration

  The OPC UA Client and Server .NET uses a common set of configuration options which is the content of this package.

- Technosoftware.UaSolution.UaBindings.Https

  If your OPC UA Client or OPC UA Server needs support of HTTPS then you need to reference also this package.

- Technosoftware.UaSolution.UaCore

  Based on the OPC UA .NET Stack from the OPC Foundation.

## Features Included
  
.NET 8.0, .NET 7.0 and .NET 6.0 allows you to develop applications that run on all common platforms available today, including Linux, macOS and Windows 8.1/10
(including embedded/IoT editions) without requiring platform-specific modifications.

Furthermore, cloud applications and services (such as ASP.NET, DNX, Azure Websites, Azure Webjobs, Azure Nano Server and Azure Service Fabric) are also supported.

### OPC UA Solution Core

- Fully ported Core OPC UA Stack.
- X.509 Certificate support for client and server authentication.
- SHA-2 support (up to SHA512) including security profile Basic256Sha256, Aes128Sha256RsaOaep and  Aes256Sha256RsaPss for configurations with high security needs.
- Anonymous, username and X.509 certificate user authentication.
- UA-TCP & HTTPS transports (client and server).
- Reverse Connect for the UA-TCP transport (client and server).
- Folder & OS-level (X509Store) Certificate Stores with *Global Discovery Server* and *Server Push* support.
- Sessions and Subscriptions.
- Improved support for Logging with `ILogger` and `EventSource`. 
- Support for custom certificate stores with refactored `ICertificateStore` and `CertificateStoreType` interface.
