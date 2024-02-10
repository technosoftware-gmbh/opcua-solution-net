# Configuration

## Application Configuration

The solution provides an extensible mechanism for storing the application configuration in an XML file. The class is extensible, so developers can add their own configuration information to it. The table below describes primary elements of the ApplicationConfiguration class.

| **Name**                | **Type**                          | **Description**                                                                                                                                                                                                                                                                                |
|-------------------------|-----------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| ApplicationName         | String                            | A human readable name for the application.                                                                                                                                                                                                                                                     |
| ApplicationUri          | String                            | A globally unique name for the application. This should be a URL with which the machine domain name or IP address as the hostname followed by the vendor/product name followed by an instance identifier. For example: http://machine1/OPC/UASampleServer/4853DB1C-776D-4ADA-9188-00CAA737B780 |
| ProductUri              | String                            | A human readable name for the product.                                                                                                                                                                                                                                                         |
| ApplicationType         | ApplicationType                   | The type of application. Possible values: Server_0, Client_1, ClientAndServer_2 or DiscoveryServer_3                                                                                                                                                                                           |
| SecurityConfiguration   | SecurityConfiguration             | The security configuration for the application. Specifies the application instance certificate, list of trusted peers and trusted certificate authorities.                                                                                                                                     |
| TransportConfigurations | TransportConfiguration Collection | Specifies the Bindings to use for each transport protocol used by the application.                                                                                                                                                                                                             |
| TransportQuotas         | TransportQuotas                   | Specifies the default limits to use when initializing WCF channels and endpoints.                                                                                                                                                                                                              |
| ServerConfiguration     | ServerConfiguration               | Specifies the configuration for Servers                                                                                                                                                                                                                                                        |
| ClientConfiguration     | ClientConfiguration               | Specifies the configuration for Clients                                                                                                                                                                                                                                                        |
| TraceConfiguration      | TraceConfiguration                | Specifies the location of the Trace file. Unexpected exceptions that are silently handled are written to the trace file. Developers can add their own trace output with the Utils.Trace(…) functions.                                                                                          |
| Extensions              | XmlElementCollection              | Allows developers to add additional information to the file.                                                                                                                                                                                                                                   |

The ApplicationConfiguration can be persisted anywhere, but the class provides functions that load/save the configuration as an XML file on disk. The location of the XML file is normally in the same directory as the executable. It can be loaded as shown in the following example:

```
// Load the Application Configuration and use the specified config section
ApplicationConfiguration config = await   
 application.LoadConfigurationAsync("Technosoftware.SampleClient");
```

The Application Configuration file of the SampleClient can be found in the file Technosoftware.SampleClient.Config.xml.

### Extensions

The Application Configuration file of the SampleClient uses the Extensions feature to make the Excel Configuration configurable.

| **Name**          | **Type** | **Description**                                                                                      |
|-------------------|----------|------------------------------------------------------------------------------------------------------|
| ConfigurationFile | String   | The full path including file name of the Excel file used for the configuration of the address space. |

The Extension looks like:

```
  <Extensions>
    <ua:XmlElement>
      <WorkshopClientConfiguration xmlns="http://technosoftware.com/SampleClient">
        <ConfigurationFile>.\SimpleServerConfiguration.xlsx</ConfigurationFile>
      </WorkshopClientConfiguration>
    </ua:XmlElement>
  </Extensions>
```

**Important:**

**This only shows how to use the Extension feature. The Excel based configuration is not implemented at all.**

### Tracing Output

With the TraceConfiguration UA client and server applications can activate trace information. SampleClient creates the following logfile:

**SampleClient:**

```
%LocalApplicationData%/Logs/SampleCompany.SampleClient.log
```

where

**%CommonApplicationData%** typically points to C:\\ProgramData
