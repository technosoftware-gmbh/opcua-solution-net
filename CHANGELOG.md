-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.1.8

# Changes
- Updated to OPC UA Solution .NET 3.1.8

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.1.5

# Changes
- Support use of opc.https endpoint url for client and server.
- Support custom cert store with flat directory structure

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.1.3

# Changes
- Removed unused System.Security.Cryptography.Pkcs

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.1.1

# Changes
- Includes all changes and fixes from the [OPC UA 1.04 Maintenance Update 1.4.371.91](https://github.com/OPCFoundation/UA-.NETStandard/releases/tag/1.4.371.91).
- Added PubSub samples

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.1.0

### Breaking Changes
- Removed support of .NET 4.6.2 and .NET 4.7.2
- SendCertificateChain default is now true

### Fixed issues
- Fixed issue in macOS, when cert chains are used the X509Certificate2 constructor throws exception
- Do not dispose the stream if the BinaryEncoder leaveopen flag is set in the constructor + ported tests from JSON
- Improve hashcode calculation for some built in types
- Fixes to support structures with allowsubtypes
- Close socket if a client stops processing responses.

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.0.6

### Changes

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.0.5

### Changes
- Updated to 3.0.5 of the UaCore

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.0.4

### Fixed Issues
- Server: Eliminate duplicate event monitored items.

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.0.3

### Changes
- Updated to 3.0.3 of the UaCore

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 3.0.2

### Breaking Changes
- License
  - A new license ist needed. Old licenses from 2.3 or earlier will no longer work with 3.0 and above.
  - Product puchases from 2021 and 2022 can get a new license free of charge. Either through their online account or by sending us an Email.
  - All others can order an OPC UA Support subscription incl. Update [here](https://technosoftware.com/product/opc-support-subscription-update/). Be aware that you need the original invoice as proof of your license.
- removed support of .NET Core 3.1 because of end of life (see [here](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core))

### Changes
- Includes all changes and fixes from the [OPC UA 1.04 Maintenance Update 1.4.371.50](https://github.com/OPCFoundation/UA-.NETStandard/releases/tag/1.4.371.50).

### Now Included
- Source Code of UaConfiguration, UaClient, UaServer, UaBaseServer and UaStandardServer. License mechanism is in UaCore, so you still need a valid license!

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.6.4

### Changes
- Includes all changes and fixes from the [OPC UA 1.04 Maintenance Update 1.4.371.50](https://github.com/OPCFoundation/UA-.NETStandard/releases/tag/1.4.371.50).

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.6.3

### Fixed issues
- License handling optimized

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.6.2

# Changes
Includes all changes and fixes from the [OPC UA 1.04 Maintenance Update 1.4.371.41](https://github.com/OPCFoundation/UA-.NETStandard/releases/tag/1.4.371.41).

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.6.1

### Enhancements
- Support of .NET 7.0

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.6.0

### Breaking Changes
- Client: Introduced IUaSession

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.5.5

### Changes
- Updated OPC UA Core to 1.4.371
- Updated model compiler

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.5.3

### Changes
- Only load optional bindings assemblies if needed.
- Server: Provided UaBaseServer as nuget package.
- Server: Fix audit event that may throw exception when server is halted.


-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.5.2

### Changes
- Stack: Updated until 24-OCT-2022
- Server: Moved several classes to Technosoftware.UaStandardServer.csproj available at src\Technosoftware\UaStandardServer. 

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.5.0

### Breaking Changes
- Server: Moved several classes to Technosoftware.UaBaseServer.csproj available at src\Technosoftware\UaBaseServer. You have to add that project and NameSpace.

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.4.5

### Fixed issues
- Server: Fixed an issue with duplicate child nodes (Using the same Node ID)

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.4.4

### Breaking Changes
- Server: Renamed INodeIdFactory.New to INodeIdFactory.Create to fix CA1716: Identifiers should not match keywords.

### Fixed issues
- Improved application certificate handling

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.4.3

### Fixed issues
- HTTPS support is working again for the NuGet packages

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.4.2

### Changes
- Client:
  - Performance updates for NodeCache

### Breaking Changes
- License
  - A new license ist needed. Old licenses from 2.3 or earlier will no longer work with 2.4 and above.
  - Product puchases from 2021 and 2022 can get a new license free of charge. Either through their online account or by sending us an Email.
  - All others can order an OPC UA Support subscription incl. Update [here](https://technosoftware.com/product/opc-support-subscription-update/). Be aware that you need the original invoice as proof of your license.
- Integrated own version of the OPC Foundation OPC UA Core Stack:
  - Technosoftware.UaCore replaces Opc.Ua.Core
  - Technosoftware.UaBindings.Https replaces Opc.Ua.Bindings.Https
- Configuration: All interfaces which can be used by servers or clients starts now with "IUa".
- Client:
  - Renamed INodeCache to IUaNodeCache
- Server:
  - Splitted IUaNodeManager into IUaBaseNodeManager and IUaNodeManager
  - Renamed MonitoredNode to UaMonitoredNode
  
### Fixed issues
- Refactor the ReceiveEvents permission type validation to be taken into account also when ConditionRefresh method is called
- Handle processing matrixes with array dimensions that overflow
- Added methods for reporting Add/Delete nodes audit events and added fixes
- Allow diagnostic nodes to be accessed only by determined users  
- Fixed ReportAuditCreateSessionEvent and added ReportAuditUpdateMethodEvent
- Fixes on AuditEvents

### Known issues
- HTTPS support is not working yet

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.14

### Changes
- Updated to OPC UA Core 1.4.370.1

### Fixed issues
- Server: Improved ResendData functionality
- Server: Fixed Server JWT Token Support

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.13

### Breaking Changes
- removed support of .NET 5.0 because of end of life (see [here](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core))

### Changes
- Updated to OPC UA Core 1.4.369.30

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.12

### Breaking Changes
- Changed ReverseConnectState to UaReverseConnectState.
- Changed ReverseConnectProperty to UaReverseConnectProperty.

### Changes
- Server: Added ResendData functionality

### Fixed issues
- Client: Fixed monitored item role permissions.
- Server: Defer MethodId permissions validation to the point where the actual MethodState that will be executed is identified
- Server: Avoid deadlock on processing events.

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.11

### Changes
- Updated to OPC UA Core 1.4.368.58
- Enhanced HistoricalAccess example

### Fixed issues
- Client: Improved reconnect handler and subscription transfer
- Server: Check operation limits on Variant arrays and TranslateBrowsePathsToNodeIds
- Server: Strict rules to compare X500 distinguished names

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.10

### Changes
- Added new StartAsync methods to UaServer using an ApplicationConfiguration object instead of the section name

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.9

### Changes
- Updated to OPC UA Core 1.4.368.33. 
- Added UaStandardServer class as new way of implementing OPC UA Servers. Documentation will be done for next release.

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.8

### Changes
- Updated to OPC UA Core 1.4.367.100

### Fixed issues
- Server: Fixed missing TimeFlowsBackward checks and invalid status code on HistoryRead

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.7

### Changes
- Included .NET 6.0 in the nuget packages

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.6

### Changes
- Updated to OPC UA Core 1.4.367.95

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.5

### Changes
- Updated to OPC UA Core 1.4.367.75

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.4

### Changes
- Updated to OPC UA Core 1.4.367.42

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.3

### Changes
- Updated to OPC UA Core 1.4.367.41

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.3.2

### Changes
- All versions now use the official OPC UA Core Stack.
- We now use the following official version 1.4.366.38 NuGet packages:
  - OPCFoundation.NetStandard.Opc.Ua.Core 
  - OPCFoundation.NetStandard.Opc.Ua.Security.Certificates
  - OPCFoundation.NetStandard.Opc.Ua.Bindings.Https
- Updated ModelCompiler to 1.01.335.1
  - Compliler can now generate properly typed code for Variables and DataType fields with abstract DataTypes. Prior releases produced code with an ExtensionObject or Variant as the type name. Passing the -useAllowSubtypes flag will enable this feature.
- Source Code of OPC UA core is removed from the source code version. 
- Enhanced performance 
- Modelcompiler updates, errata 1.04.9 nodeset
- Add certificate password provider interface to support password protected pfx files.
- Tested with OPC UA Local Discovery Server V1.04.402.461
- Tested with OPC UA Compliance Test Tool V1.4.9.398

### Breaking Changes
- Added OnApplicationConfigurationLoaded(ApplicationInstance application, ApplicationConfiguration configuration) to IUaServerPlugin. 
  It is called after OnStartup(). You have to add it to the UaServerPlugin.cs
- Removed UaApplicationInstance. You can use ApplicationInstance instead.

### Fixed issues
- Fixed parameter type of method "PropertyState CreatePropertyState(NodeState parent, ...)". It is now NodeState and was before BaseObjectState.

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.2.0

### Changes
- .NET 4.6.2, .NET 4.7.2 and .NET 4.8 are now delivered with it's own NuGet packages:
  - Technosoftware.Net4.UaConfiguration
  - Technosoftware.Net4.UaClient
  - Technosoftware.Net4.UaServer
- Updated OPC UA Core to 1.4.365.48
  
-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.1.2

### Changes
- Use of original OPC UA Foundation Core (Only .NET 5.0 and .NET Standard 2.1 build uses a modified Core)

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.1.1

### Enhancement
- Introduced new overridable method AddEncodeableTypes in the UaBaseServer to allow adding encodeable types from different assemblies.

### Changes
- OPC Foundation Core is now only modified regarding .NET 5.0. 
- As soon as .NET 5.0 is officially supported we move to the original NuGet packages of the OPC UA Core.

### Fixed issues
- fix issue with JSON encoder, field names were not escaped
- add some basic tests for ECDsa cert generation and validate yet missing ECC cert factory API extensions with test cases
- improve how the cert validator returns suppressible errors
- not a breaking change: the cert validator callback is now called for every suppressible error, not only once, for backward compatibility with existing applications
- applications which implement to handle all suppressible errors in a single callback can set the 'AcceptAll' flag instead of 'Accept' to accept all suppressible errors once.
- in the client library the domain check can also be handled in the validator callback.
- fix warnings and cross platform line feed usage
- Compliance fixes for certificate validation.
- Use monotonic timer for HiResClock and fix loss of monitored items when system time is changed
- Method calls validation for Executable and UserExecutable.
- Private key is not stored to the Windows certificate store when using .NET Standard 2.1
- Fix build and LGTM warnings
- The AcceptAll flag can be used to accept all suppressible errors in one call back
- fix decoding of ecdsa signature
- Set TypeId for structured types at decode.
- Fix issue setting empty comments
- Fix Alarm ShelvingState: Cannot call Unshelve Method when Alarm state is TimedShelved.
- Fix AlarmConditionState should have easier mechanism to update UnshelveTime.
- Merge model compiler fixes with latest code.
- Client: Validate server domains in Certificate validator

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.1.0

### Changes
- This edition supports .NET Standard 2.1, .NET Core 3.1 and .NET 5.0
- For backward compatibility we also provide .NET 4.8, .NET 4.7.2 and .NET 4.6.2 support.

### New implementation of many X509 related functions based on new System.Formats.Asn1 library

- Based on the new System.Formats.Asn1 library released with .NET Core 5 many ASN.1 encoding and decoding operations have been reimplemented to reduce the dependency on an external crypto library called bouncy castle.
- The X509 code located in Security/Certificates was refactored and reimplemented to seperate the X509 ASN.1 encoder/decoder functions from Core into a new assembly called Opc.Ua.Security.Certificates with source code under MIT license.
- New CertificateBuilder and CrlBuilder class APIs allow for simplified cert and crl creation and are prepared for future ECC support.
- For applications which use the .NET Standard 2.1 version of the core library (e.g. .NET Core 3.1 applications) the built in CertificateRequest class is used to create certificates and the dependency on the bouncy castle library is completely removed.

### Breaking change for some Utils functions

- many X509 helper functions which were located in Utils found a new home in the X509Utils class.

### Note
- .NET 4.8, .NET 4.7.2 and .NET 4.6.2 versions still use BouncyCastle for Cert creation

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.0.8

### Changes
- Use of Opc.Ua.Core V1.4.364.40

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.0.7

### Fixed issues
- Fix for OPC UA Bundle .NET Standard license handling

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.0.6

### Changes
- .NET 5.0 requires a new license and is available for the OPC UA Bundle .NET

### Fixed issues
- Fix in client part for Prosys OPC UA simulation server

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.0.5

### Changes
- DLL's are now code signed

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.0.4

### Changes
- Model Compiler build with .NET 4.6.2
- Binary version supports .NET 4.6.2 and .NET Stanbdard 2.1 and is based on official OPC Foundation Opc.Ua.Core
- Source Code version supports .NET 4.6.2, .NET 4.7.2, .NET 4.8, .NET Standard 2.1 and .NET 5.0 and uses
  a modified Opc.Ua.Core version

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.0.3

### Changes
- Use of Opc.Ua.Core V1.4.364.31-preview
- Reduce core dependencies, move https binding to optional assembly
- Removed .NET Core 3.0 support because it is out of support and will not receive security updates in the future. 

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.0.2

### Changes
- Use of OPC.Ua.Core V1.4.303.107

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.0.1

### Changes
- Use of OPC UA ModelCompiler from 26-SEP-2020

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 2.0.0

### Important Information
Goal of this 2.0 version is the alignment with the original Opc.Ua.Core stack to get easier compliance with
the current OPC UA specifications. Because of this a lot of changes had to be done in 2.0 and some 
functionality was removed.

### Updating from 1.3 or 1.4 to 2.0
It is recommended to update to the latest 1.4 version first. That way you get more hints which 
changes must be applied. Obsolete methods no longer supported in 2.0 are tagged with an obsolete attribute in the latest version of 1.4 and are removed in 2.0.
	
### Breaking Changes
- Renamed INodeIdFactory.CreateNodeId to INodeIdFactory.New
- Removed obsolete code

### Changes
- Integrated version 1.4.363.107 of the official OPC UA Core Class Library

### Removed Features
- Service Handling (StartAsService)
- InstallConfig handling
- ProcessCommandLine() handling

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.13
	
### Changes
- Added some comments for easier transition to 2.0

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.12
	
### Changes
- Updated BouncyCastle.Crypto.dll to 1.8.8
- Removed several obsolete methods from Technosoftware.UaConfiguration

### Fixed Issues
- Improved reconnect handling for client part
- Fixed user identity handling in server session

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.11
	
### Changes
- Updated stack to the official version from the OPC Foundation (1.4.363.107)

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.10
	
### Changes
- Moved the stack to it's own repository

### Fixed issues
- Integrated v1.4.10 of https://github.com/technosoftware-gmbh/opc-ua-core-net-standard

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.0731
	
### Fixed Issues
- Stack: Index range fixes
  - Fixed NumericRange ApplyRange method to work also for Matrix objects.
  - Fixed NumericRange.UpdateRange to work with multidimensional arrays. Also fixed it to handle Matrix type as destination. Fixed WriteValue validation to also accept Matrix type as value when IndexRange contains SubRanges.
  - Fixed WriteValue to validate NumericRange correctly by also accepting string value.
  - Fixed typo in NumericRangeTests. Implemented tests for WriteValue Validate method.
  - Implemented test case for NumericRange.UpdateRange when using Matrix as destination. Fixed comments.
  - Fixed WriteValue.Validate to accept String and ByteString arrays when IndexRange with SubRanges is used. String and ByteString arrays have special handling in this case.
  - Added null check.
- Stack: Moved some traces to Debug
- Stack: Fix argTypes null pointer condition
- Client: Session validation in Client Subscription

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.0707
	
###	Fixed Issues
- Fix for self signed certs

###	Breaking Changes
- CheckApplicationInstanceCertificateAsync() has now 3 parameters. Sample Client applications now use
  - bool haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(false, CertificateFactory.DefaultKeySize, CertificateFactory.DefaultLifeTime);
  instead of
  - bool haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(false, 0);

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.0705
	
###	Fixed Issues
- Changed method CreateMethodState()
  - Parameter parent was wrongly defined as BaseObjectState and not NodeState.

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.0627

###	Enhancements
- Support for OPC UA 1.04 reverse connection
  - Restrictions:
    - Client and Server support added for opc.tcp only
    - Tested with SimpleClient and SimpleServer on Windows
	- Tested with Unfied Automation UaExpert 1.5.1 with SimpleServer on Windows
	- Tested with SimpleClient and Unified Automation UaCppServer on Windows
	
###	Fixed Issues
- Compliance and stability fixes
- Server Compliance tested with OPC UA Compliance Test Tool 1.04.9.396	

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.0612

###	Enhancements
- New sample clients and servers
  - SimpleServer and SimpleClient uses basic functionality (coded).
  - ModelDesignServer use a ModelDesign created with th OPC Foundation ModelCompiler. 
  - ModelDesignClient browses the address space of the ModelDesignServer and read a single value
- Travis CI and AppVeyor used to build and test the sample applications on different platforms.

###	Fixed Issues
- Integrated OPC UA Core .NET V1.4.0612
- Support of enum array in BinaryEncoder improved.

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.0604

- Integrated OPC UA Core .NET from https://github.com/technosoftware-gmbh/opc-ua-core-net-standard

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.4.0522

###	Breaking Changes
- Updated minimal required .NET version to 4.6.2 so that also .NET framework applications can use latest version of Kestrel
  - Updated Kestrel to 2.2.0 (for Opc.Ua.Core)
  - Removed Kestrel 1.1.3 code from Stack/Opc.Ua.Core/Stack/Https/HttpsListener.cs 
  - add conditions to Stack/Opc.Ua.Core/Stack/Https/HttpsTransportChannel.cs 
    - [HttpClientHandler.ServerCertificateCustomValidationCallback](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.servercertificatecustomvalidationcallback?view=netcore-3.1&viewFallbackFrom=netframework-4.6.2) should not be available in .NET 4.6.X

#### Server 
- Refactored IUaServer and UaBaseNodemanager. Please be aware of several methods changed.
- ReferenceServer is now a console based .NET Core 3.1 application and used the new UaBaseNodeManager methods.
- Tested with UACTT 1.3.341.395

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.3.0503

###	Fixed Issues

#### Server 
- Avoid null reference in GenericServerData.ReportEvent()

#### Client
- Compliance fix. DeleteSubscription after Publish response with unknown SubscriptionId.
- RolePermission is an optional attribute, it can be ignored if access is denied.

#### Stack
- add encode/decode tests for all IEncodeables in Opc.Ua.Core.
- *Breaking change*: ExtensionObject now returns the proper TypeId on decode for known encodeables.
- *Breaking change*: SessionlessMessage uses the same TypeId as SessionlessInvokeRequestType which can cause problems in type factory, demoted the structure as not being IEncodeable as it is only used as a helper class in encoders/decoders.
- Fix ReadNode for Nodes with RolePermission returning BadUserAccessDenied
  - ReadNode was throwing exception in this case, making it impossible to browse the namespace
  - Fix: RolePermission is an optional attribute, it can be ignored if access is denied.

-------------------------------------------------------------------------------------------------------------
## OPC UA Solution .NET - 1.3.0419

###	Important

#### Evaluation Edition and Binary Edition

- Technical Support is available via https://github.com/technosoftware-gmbh/opc-ua-bundle-net-standard/issues

#### Source Edition

- The Source Edition is now available on GitHub. You get access to the private repository after purchase at https://technosoftware.com/product/opc-ua-bundle-net-standard/
- Technical Support is available via https://github.com/technosoftware-gmbh/opc-ua-bundle-net-standard-src/issues

###	Redistributables

- Redistributables of the OPC UA Local Discovery Server are available via https://opcfoundation.org/developer-tools/samples-and-tools-unified-architecture/local-discovery-server-lds/
- We used the version 1.03.401 for our tests

-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.3.0413 (Release Date 13-APR-2020)

###	Enhancements
- .NET 4.6.1 version now supported in the Source Edition

###	Breaking Changes
- .NET Standard 2.0 and .NET Core 2.0 now only supported in the Source Edition

## OPC UA Server .NET - 1.3.0413 (Release Date 13-APR-2020)

###	Enhancements
- .NET 4.6.1 version now supported in the Source Edition

###	Breaking Changes
- .NET Standard 2.0 and .NET Core 2.0 now only supported in the Source Edition

## OPC UA Core .NET - 1.3.0413 (Release Date 13-APR-2020)

###	Enhancements
- .NET 4.6.1 version now supported in the Source Edition

###	Breaking Changes
- .NET Standard 2.0 and .NET Core 2.0 now only supported in the Source Edition


-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.3.0411 (Release Date 11-APR-2020)

###	Issues
- Fix typo and grammar in Application Instance Certificate exception

## OPC UA Server .NET - 1.3.0411 (Release Date 11-APR-2020)

###	Issues
- Fix for writing RolePermissions and AccessRestrictions attributes in Server.

## OPC UA Core .NET - 1.3.0411 (Release Date 11-APR-2020)

###	Issues
- Allow also UInt32 type when writing AccessRestrictions attribute.

-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.3.0409 (Release Date 03-APR-2020)

###	Issues
- Fixed source distribution issues

## OPC UA Server .NET - 1.3.0403 (Release Date 03-APR-2020)

###	Issues
- Fixed source distribution issues
- CTT compliance fixes for Audit events.

## OPC UA Core .NET - 1.3.0403 (Release Date 03-APR-2020)

###	Enhancements
- Update generated code and UA-nodeset files for 1.04.5 Errata

###	Issues
- Fixed source distribution issues
- Fixes duplicate entries in GetEndpoint response.
- fix variant constructor for enum
- fix behaviour change for guid, can cause cast error
- Fix for CertificateUpdate

-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.3.0307 (Release Date 07-MAR-2020)

###	Enhancements
- Updated samples to .NET 4.8 and added .NET Core 3.1 to samples

###	Issues
- Handle exception when server returns null terminated dictionary bytestring

## OPC UA Server .NET - 1.3.0307 (Release Date 07-MAR-2020)

###	Enhancements
- Updated samples to .NET 4.8 and added .NET Core 3.1 to samples
- Updated profile documentation

###	Issues
- Compliance fixes for latest CTT

## OPC UA Core .NET - 1.3.0307 (Release Date 07-MAR-2020)

###	Issues
- Compliance fixes for latest CTT
- Stabilize TCP receive
- Fix for non reversible encoding of Unions and structures

-------------------------------------------------------------------------------------------------------------
## OPC UA Core .NET - 1.3.0216 (Release Date 22-FEB-2020)

###	Issues
- fix for non reversible encoding of Unions and structures
- handle exception when server returns null terminated dictionary bytestring
- Stabilize TCP receive
- stabilize binary decoder

-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.3.0216 (Release Date 16-FEB-2020)

###	Issues
- Validate for spec compliant subscription defaults
- CreateSubscription default constructor used very tight setting for keepalive/minlifetime

## OPC UA Core .NET - 1.3.0216 (Release Date 16-FEB-2020)

###	Enhancement
- Added unit tests (included in source code version)
 
###	Breaking Changes
- breaking change for JSON encoding of NodeId. Encoding was not according to spec., see Part6, 5.4.2.10 NodeId)
- JSON encoding of the NodeId was not quite right
- JsonDecoder parsing was dependent on current culture

###	Issues
- Implement validation for duplicate nonces
- Add Server nonce validation for duplicate nonces.
- Add configuration flag to bypass validation exceptions
- Add ITransportChannel.CurrentToken interface for nonce validation.
- Fixed DataGenerator for GUID

-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.3.0131 (Release Date 31-JAN-2020)

###	Enhancement
- Updated model to OPC UA 1.0.4 Errata

## OPC UA Server .NET - 1.3.0131 (Release Date 31-JAN-2020)

###	Enhancement
- Updated model compiler to latest version
- Updated model compiler to NET 4.8
- Updated model to OPC UA 1.0.4 Errata

## OPC UA Core .NET - 1.3.0131 (Release Date 31-JAN-2020)

###	Enhancement
- Updated model to OPC UA 1.0.4 Errata


-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.3.0118 (Release Date 18-JAN-2020)

###	Enhancement
- Use nameof() where applicable

## OPC UA Server .NET - 1.3.0118 (Release Date 18-JAN-2020)

###	Enhancement
- Use nameof() where applicable

## OPC UA Core .NET - 1.3.0118 (Release Date 18-JAN-2020)

###	Enhancement
- Use nameof() where applicable
- Rethrow original exception
  Preserves original stack trace of the exception, easier to diagnose
- Remove variable from the catch if not logged (compiler warning).
- Performance enhancements

###	Issues
- Prevent deep recursion and excessive thread creation in TcpMessageSocket
- Fix for handling of AccessLevel and AccessLevelEx attributes.

-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.3.0106 (Release Date 06-JAN-2020)

###	Breaking Changes
- New license required. License for version 1.2 will not work
- Class CoreClientUtils merged into class Discover
- Methods in class Discover removed which used the Specification class

###	Enhancement
- .NET 4.8 support added
- .NET Standard 2.1 support added
- .NET Core 3.0 support added

## OPC UA Server .NET - 1.3.0106 (Release Date 06-JAN-2020)

###	Breaking Changes
- New license required. License for version 1.2 will not work

###	Enhancement
- .NET 4.8 support added
- .NET Standard 2.1 support added
- .NET Core 3.0 support added

## OPC UA Core .NET - 1.3.0106 (Release Date 06-JAN-2020)

###	Breaking Changes
- New license required. License for version 1.2 will not work
- Class Specification removed. OPC Classic specifications are no longer supported since 1.2 and therefore not needed.

###	Enhancement
- .NET 4.8 support added
- .NET Standard 2.1 support added
- .NET Core 3.0 support added

-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.2.1130 (Release Date 30-NOV-2019)

###	Enhancement
- Description and RolePermissions are now initialized

## OPC UA Server .NET - 1.2.1130 (Release Date 30-NOV-2019)

###	Enhancement
- Description and RolePermissions are now initialized

## OPC UA Core .NET - 1.2.1130 (Release Date 30-NOV-2019)

###	Enhancement
- Description and RolePermissions are now initialized
- Description for OPC UA standard nodes are now loaded also.

-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.2.1125 (Release Date 25-NOV-2019)

###	Enhancement
- Less logging for Info messages. Moved them to OperationDetail

## OPC UA Server .NET - 1.2.1125 (Release Date 25-NOV-2019)

###	Enhancement
- Less logging for Info messages. Moved them to OperationDetail

## OPC UA Core .NET - 1.2.1125 (Release Date 25-NOV-2019)

###	Enhancement
- Less logging for Info messages. Moved them to OperationDetail

-------------------------------------------------------------------------------------------------------------
## OPC UA Client .NET - 1.2.1124 (Release Date 24-NOV-2019)

###	Issues
- Add server nonce validation in Client session.

## OPC UA Server .NET - 1.2.1124 (Release Date 24-NOV-2019)

###	Issues
- Added user authentication handling for UaMonitoredItem.

## OPC UA Core .NET - 1.2.1124 (Release Date 24-NOV-2019)

###	Enhancement
- Preparation for Complex Types

###	Issues
- Avoid deep recursion in TcpMessageSocket.ReadNextBlock()
- Certificate Validation fix.
- Remove BadCertificateUriInvalid from list of certificate validation errors that may be suppressed.
- Add server nonce validation in Client session.
- Length validation in user token decryption.
- Stability in BinaryDecoder improved.
- Fix in DiscoveryClient.Create()

-------------------------------------------------------------------------------------------------------------

## OPC UA Server .NET - 1.2.1101 (Release Date 01-NOV-2019)

###	Breaking Changes
- Changed license handling. You will get some errors using this version now. Please see the new license certificate. The new method to use is 
        void OnGetLicenseInformation(out Opc.Ua.LicenseHandler.LicenseEdition productEdition, out string serialNumber);

###	Enhancement
- Added CreateBaseDataVariable(NodeState parent, string path, string name, ExpandedNodeId expandedNodeId, int valueRank, byte accessLevel, object initialValue) method to IUaServer

## OPC UA Client .NET - 1.2.1101 (Release Date 01-NOV-2019)

###	Breaking Changes
- Changed license handling. You will get some errors using this version now. Please see the new license certificate. The new method to use is 
        public static bool Validate(Opc.Ua.LicenseHandler.LicenseEdition productEdition, string serialNumber)

###	Issues
- Add check for some configuration fields and return some additional errors.

## OPC UA Core .NET - 1.2.1101 (Release Date 01-NOV-2019)

###	Issues
- ExpandedNodeId.CompareTo contract violation
- fix schema validator and dictionary loader (only binary)
- fix schema validator and dictionary loader (only binary scheme)
- include some .bsd files for validation

-------------------------------------------------------------------------------------------------------------

## OPC UA Server .NET - 1.2.1013 (Release Date 13-OCT-2019)

###	Breaking Changes
- Several synchronous methods marked as obsolete in 1.1 are now giving an error while building with 1.2. Use the asynchronous versions mentioned in the error instead.
- Updated .NET 4.6.1 version to .NET 4.6.2
  
###	Enhancement
- Add IsNodeAccessibleForUser() and IsReferenceAccessibleForUser() to UaBaseNodeManager().
  This allows to hide nodes and references for specific users. 
  The ServerForms and ServerConsole sample shows the usage if these new methods.

###	Issues
- Added several XSD schema's to the schema folder. The Workshop server sample ModelDesign.xml files references the UAModelDesign.xsd. 
  Visual Studio can be used to change the Design files while giving context sensitive and popups. 

## OPC UA Client .NET - 1.2.1013 (Release Date 13-OCT-2019)

###	Breaking Changes
- Several synchronous methods marked as obsolete in 1.1 are now giving an error while building with 1.2. Use the asynchronous versions mentioned in the error instead.
- Updated .NET 4.6.1 version to .NET 4.6.2
- OPC UA Client Gateway is no longer maintained and source code is no longer delivered.

###	Issues
- The Session Method "ReadValue" delivers old values. 
- Using IUaServerData.Status can be made thread safe by using Status.Lock
- Client keep-alive fix - Outstanding publish requests should not stop keep alive reads.

## OPC UA Core .NET - 1.2.1013 (Release Date 13-OCT-2019)

###	Enhancement
- Synchronize with newest UA Nodeset

###	Issues
- TraceEventHandler event does not publish exception information
- Possible handle leak in stress test for TCP communication 
- Interoperability fixes for HTTPS
- Improvement to enable UANodeSet to import/export AccessLevelEx as uint32

-------------------------------------------------------------------------------------------------------------

## OPC UA Server .NET - 1.1.815 (Release Date 15-AUG-2019)

###	Changes
Version numbering scheme changed. Version numbers are used with Major.Minor.Build, where
- Major: is incremented when we make incompatible API changes
- Minor: is incremented when we add functionality in a backwards-compatible manner
- Build: is incremented when we make backwards-compatible bug fixes and is the date (mmyy) the build was triggered

###	Updates / Fixes
- Enhanced documentation and added example usage on Linux, macOS
- CertificateValidation event not fired for BadCertificateChainIncomplete. 
- Cleaned up source and added build related property files (Source Distribution only)

## OPC UA Client .NET - 1.1.824 (Release Date 24-AUG-2019)

###	Updates / Fixes
- Enhanced documentation and added example usage on Linux, macOS
- CertificateValidation event not fired for BadCertificateChainIncomplete. 
- Cleaned up source and added build related property files (Source Distribution only)

-------------------------------------------------------------------------------------------------------------

## OPC UA Server .NET - 1.1.0 (Release Date 22-JUN-2019)

###	Highlights
- Enhanced documentation regarding certificate and configuration tool handling

###	Updates / Fixes
- Removed WindowsCertificateStore support
- Fixed several screens of the Configuration Tool
- Fix for abandoned socket connection in UaSCUaBinaryTransportChannel.Reconnect()
- Fix to avoid timer interval higher than Int32.MaxValue in ClientSubscription.StartKeepAliveTimer()
- Updated Local Discovery Server to 1.03.401.438

## OPC UA Client .NET - 1.1.0 (Release Date 22-JUN-2019)

###	Highlights
- Enhanced documentation regarding certificate and configuration tool handling
- Fixed several screens of the Configuration Tool

###	Updates / Fixes
- Removed WindowsCertificateStore support
- Fix for abandoned socket connection in UaSCUaBinaryTransportChannel.Reconnect()
- Updated Local Discovery Server to 1.03.401.438

-------------------------------------------------------------------------------------------------------------

## OPC UA Server .NET - 1.0.9 (Release Date 31-MAY-2019)

###	Updates / Fixes
- Compliance fixes in CertificateValidator.
  Added setting in SecurityConfiguration to require revocation lists for all CAs
- Updated documentation

## OPC UA Client .NET - 1.0.9 (Release Date 31-MAY-2019)

###	Updates / Fixes
- Updated documentation

-------------------------------------------------------------------------------------------------------------

## OPC UA Server .NET - 1.0.8 (Release Date 22-MAY-2019)

###	Highlights
- Enhanced documentation
- .NET 4.6.1 sample applications and installer added
- Visual Studio 2013, Visual Studio 2015 added
- New examples added compiling for .NET 4.6.1, .NET 4.7.2 and .NET Core 2.0 with one solution
- Source Code Distribution now available

###	Updates / Fixes
- Cleanup of namespaces (Technosoftware.UaServer.Base no longer exists) and code
- GenericServer class is now in namespace Technosoftware.UaServer.Server
- Session related classes are now in namespace Technosoftware.UaServer.Sessions
- Renamed IAggregateCalculator to IUaAggregateCalculator
- Renamed CertificateUpdateCallbackHandler to CertificateUpdateEvent
- Renamed FirewallUpdateCallbackHandler to FirewallUpdateEvent
- Several session related events of IUaSessionManager changed in naming to fit general concept:
     - SessionCreated to SessionCreatedEvent
	 - SessionActivated to SessionActivatedEvent
	 - SessionClosing to SessionClosingEvent
	 - ImpersonateUser to ImpersonateUserEvent
	 - ValidateSessionLessRequest to ValidateSessionLessRequestEvent

## OPC UA Client .NET - 1.0.8 (Release Date 22-MAY-2019)

###	Highlights
- Enhanced documentation
- .NET 4.6.1 sample applications and installer added
- Visual Studio 2013, Visual Studio 2015 added
- New examples added compiling for .NET 4.6.1, .NET 4.7.2 and .NET Core 2.0 with one solution
- Source Code Distribution now available

###	Updates / Fixes
- Cleanup of code
- Renamed CertificateUpdateCallbackHandler to CertificateUpdateEvent
- Renamed FirewallUpdateCallbackHandler to FirewallUpdateEvent

-------------------------------------------------------------------------------------------------------------

## OPC UA Server .NET - 1.0.7 (Release Date 06-MAY-2019)

###	Highlights
- First version of OPC UA Server .NET. Compatible to API of OPC UA Server SDK .NET
- Supports .NET Standard 2.0. New WorkshopServerConsole is now available for 
     - .NET 4.6.1, .NET 4.7.2, .NET Core 2.0 (\examples\Workshop\ServerConsole)
	 - .NET 4.7.2 (\examples\Workshop\ServerForms)
  Both samples has the same functionality but the first one is a console application and the second one a Wiondows Form based application.

###	Updates / Fixes
- Update 1.04 NodeSets and generated code
- CTT Compliance fixes
- CertificateValidator update

## OPC UA Client .NET - 1.0.7 (Release Date 06-MAY-2019)

###	Highlights
- Supports .NET Standard 2.0. New WorkshopServerConsole is now available for 
     - .NET 4.6.1, .NET 4.7.2, .NET Core 2.0 (\examples\Workshop\ClientConsole)
	 - .NET 4.7.2 (\examples\Workshop\ClientForms)

###	Updates / Fixes
- Update 1.04 NodeSets and generated code

-------------------------------------------------------------------------------------------------------------

## OPC UA Server .NET - 1.0.6 (Release Date 05-APR-2019)

###	Highlights
- First version of OPC UA Server .NET. Compatible to API of OPC UA Server SDK .NET
- Supports .NET 4.6.1 and .NET 4.7.2. 

###	Known Issaues
- .NET Standard 2.0 not yet supported

## OPC UA Client .NET - 1.0.6 (Release Date 05-APR-2019)

###	Highlights
- Added more compatibility to OPC UA Client SDK .NET (InstallConfig, Services and command line parameters supported)
- Supports .NET 4.6.1, .NET 4.7.2. and .NET Standard 2.0

-------------------------------------------------------------------------------------------------------------

## OPC UA Client .NET - 1.0.5 (Release Date 16-FEB-2019)

###	Highlights
- OPC UA Configuration Client is now able to handle also clients based on this SDK (.NET 4.6.1 only)

-------------------------------------------------------------------------------------------------------------

## OPC UA Client .NET - 1.0.4 (Release Date 05-JAN-2019)

###	Highlights
- Support for AES security policies
- 1.04 Specification NodeSets and generated code
- Custom configuration settings for user certificate stores

###	Updates / Fixes
- Security updates
- Certificate stores renamed according to specification recommendation

-------------------------------------------------------------------------------------------------------------

## OPC UA Client .NET - 1.0.3 (Release Date 08-DEC-2018)
- Added Professional license for .NET 4.6.1 usage only

-------------------------------------------------------------------------------------------------------------

## OPC UA Client .NET - 1.0.1 (Release Date 10-NOV-2018)
- Updated brochures

-------------------------------------------------------------------------------------------------------------

## OPC UA Client .NET - 1.0.0 (Release Date 13-OCT-2018)

###	Highlights
- .NET Standard 2.0 version tested on macOS 10.14

## OPC UA Stack .NET Standard - 1.0.0 (Release Date 13-OCT-2018)
- .NET Standard 2.0 version tested on macOS 10.14

-------------------------------------------------------------------------------------------------------------

## OPC UA Client .NET - 0.2.0 (Release Date 03-OCT-2018)

###	Highlights
- Licensing mechanism added
- Certificate stores renamed according to Part 12 specification recommendation.

## OPC UA Stack .NET Standard - 0.2.0 (Release Date 03-OCT-2018)
- Updated stack with OPC Foundation Stack changes until 13-SEP-2018

-------------------------------------------------------------------------------------------------------------

## OPC UA Client .NET - 0.1.0 (Release Date 17-SEP-2018)
- Initial beta version

## OPC UA Stack .NET Standard - 0.1.0 (Release Date 17-SEP-2018)
- Initial beta version
