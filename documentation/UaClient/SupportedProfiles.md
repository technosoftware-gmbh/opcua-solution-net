# Supported OPC UA Profiles dor Client Development

The following table shows the different OPC UA profiles and if they are supported by the OPC UA Client .NET:For general information about the OPC UA Solution .NET Development we highly recommend to read the following document:

## Core Characteristics

| Profile  | Description | Supported |
|:---|:---|:---|
| Core 2017 Client Facet | This Facet defines the core functionality required for any Client. This Facet includes the core functions for Security and Session handling. This Facet supersedes the Core Client Facet.  | Yes |
| Sessionless Client Facet | Defines the use of Sessionless Service invocation in a Client. | No |
| Reverse Connect Client Facet | This Facet defines support of reverse connectivity in a Client. Usually, a connection is opened by the Client before starting the UA-specific handshake. This will fail, however, when Servers are behind firewalls. In the reverse connectivity scenario, the Client accepts a connection request and a ReverseHello message from a Server and establishes a Secure Channel using this connection.  | Yes |
| Base Client Behaviour Facet | This Facet indicates that the Client supports behaviour that Clients shall follow for best use by operators and administrators. They include allowing configuration of an endpoint for a server without using the discovery service set; Support for manual security setting configuration and behaviour regarding security issues; support for Automatic reconnection to a disconnected server. These behaviours can only be tested in a test lab. They are best practice guidelines. | Yes |
| Discovery Client Facet | This Facet defines the ability to discover Servers and their Endpoints. | Yes |
| Subnet Discovery Client Facet |  | No  |
| Global Discovery Client Facet |  |  |
| Global Certificate Management Client Facet |  |  |
| Access Token Request Client Facet |  |  |
| KeyCredential Service Client Facet |  |  |
| AddressSpace Lookup Client Facet |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
