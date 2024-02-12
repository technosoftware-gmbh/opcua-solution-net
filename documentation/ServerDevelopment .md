# Develop OPC UA Servers with C\# for of .NET

## OPC UA Server .NET

The OPC UA Server NET offers a fast and easy access to the OPC Unified Architecture (UA) technology. Develop OPC UA compliant Servers with C# targeting .NET 8.0, .NET 7.0 or .NET 6.0. For backward compatibility we also provide .NET 4.8 support.

.NET 8.0, .NET 7.0 or .NET 6.0 allows you develop applications that run on all common platforms available today, including Linux, macOS and Windows 8.1/10/11 (including embedded/IoT editions) without requiring platform-specific modifications.

The developer can concentrate on his application and servers can be developed fast and easily without the need to spend a lot of time learning how to implement the OPC Unified Architecture specification. The server API is easy to use and many OPC specific functions are handled by the framework.

The included OPC Foundation Model Compiler can be used to create the necessary C# classes of Information Model’s specified in XML and CSV based files. At the moment the XML files must be edited by a text editor. 

Documentation of the Model Compiler can be found [here](https://github.com/OPCFoundation/UA-ModelCompiler).

**Document Control**

| **Version** | **Date**    | **Comment**                          |
|-------------|-------------|--------------------------------------|
| 3.1         | 28-APR-2023 | Initial version based on version 3.1 |
| 3.3         | 02-FEB-2024 | Updated to new sample client         |

**Purpose and audience of document**

Microsoft’s .NET is an application development environment that supports multiple languages and provides a large set of standard programming APIs. This document defines an Application Programming Interface (API) for OPC UA Client development based on the .NET programming model.

This document gives a short overview of the functionality of the OPC UA .NET solutions family. The goal of this document is to give an introduction and can be used as base for your own implementations.

**Referenced OPC Documents**

| **Documents**                                                                                                                                                                                                                             |
|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Online versions of OPC UA specifications and information models. The OPC UA Online Reference is available at:  <https://reference.opcfoundation.org>                                                                                      |
| OPC Unified Architecture Textbook, written by Wolfgang Mahnke, Stefan-Helmut Leitner and Matthias Damm:  <http://www.amazon.com/OPC-Unified-Architecture-Wolfgang-Mahnke/dp/3540688986/ref=sr_1_1?ie=UTF8&s=books&qid=1209506074&sr=8-1>  |


## [Supported OPC UA Profiles](./UaServer/SupportedProfiles.md)

## [Sample Application](./UaServer/SampleApplication.md)

## [Configuration](./UaServer/Configuration.md)

## [Certificate Management and Validation](./UaServer/CertificateManagement.md)

## [Client Design](./UaServer/ServerDesign.md)