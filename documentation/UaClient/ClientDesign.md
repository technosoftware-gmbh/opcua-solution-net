# UA Client Design

The Client API is designed to ease client development by handling the standard tasks which all clients need to do. The tasks include:

-   sending the session keep alive requests.
-   managing the publishing pipeline.
-   keeping track of the status for subscription and monitored items.
-   managing a client-side node cache
-   processing and caching incoming data change and event notifications.
-   saving and restoring the session state including the subscriptions and monitored items.

The figure below shows the main classes a client developer can use:

![](../images/UaClientDesign.png)

**ApplicationInstance**

The ApplicationInstance class is the main instance used to use services provided by an OPC UA server. It is in the Technosoftware.UaConfiguration.dll

**Discover**

For UA Server discovering you can use the methods of the Discover class.

**Session**

A Session represents a connection with a single Server. It maintains a list of Subscriptions in addition to a NodeCache.

**Subscription**

A Subscription represents a Subscription with a Server. A Subscription is owned by a Session and can have one or more MonitoredItems.

**MonitoredItem**

MonitoredItem's are used to monitor data or events produced by individual nodes in the Server address space.

## License Validation

The license validation should be done in the very beginning by calling the following with the provided license data:

```
    #region License validation
    var licenseData =
            @"";
    var licensed = Technosoftware.UaClient.LicenseHandler.Validate(licenseData);
    if (!licensed)
    {
        await output.WriteLineAsync("WARNING: No valid license applied.").ConfigureAwait(false);
    }
    #endregion
```

## Client Startup

The ApplicationInstance class is the main instance used to use services provided by an OPC UA server. The implementation of an OPC UA client application starts with the creation of an ApplicationInstance object. These are the lines in the Program.cs that creates the ApplicationInstance:

```
    // The application name and config file names
    var applicationName = "SampleCompany.SampleClient";
    var configSectionName = "SampleCompany.SampleClient";

    // Define the UA Client application
    ApplicationInstance.MessageDlg = new ApplicationMessageDlg(output);
    var passwordProvider = new CertificatePasswordProvider(password);
    var application = new ApplicationInstance {
        ApplicationName = applicationName,
        ApplicationType = ApplicationType.Client,
        ConfigSectionName = configSectionName,
        CertificatePasswordProvider = passwordProvider
    };
```

An OPC UA Client application can be configured via an application configuration file. This is handled by calling the LoadApplicationConfigurationAsync () method. This loads the ApplicationConfiguration from the configuration file configSectionName.Config.xml:

```
    // load the application configuration.
    ApplicationConfiguration config = await application.LoadApplicationConfigurationAsync(silent: false).ConfigureAwait(false);
```

All clients must have an application certificate, which is used for the validation of trusted clients. It can be renewed if needed and checked with the following code:

```
    // delete old certificate
    if (renewCertificate)
    {
        await application.DeleteApplicationInstanceCertificateAsync(quitCts.Token).ConfigureAwait(false);
    }

    // check the application certificate.
    var haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(false, minimumKeySize: 0).ConfigureAwait(false);
    if (!haveAppCertificate)
    {
        throw new ErrorExitException("Application instance certificate invalid!", ExitCode.ErrorCertificate);
    }
```

## Server Connection

To be able to connect to an OPC UA Server a server Url is required:

| **URI**                                                   | **Server**                                   |
|:----------------------------------------------------------|:---------------------------------------------|
| opc.tcp://\<hostname\>:62555/SampleServer                 | Tutorial OPC UA Sample Server                |
| opc.tcp://\<hostname\>:52520/OPCUA/SampleConsoleServer    | Prosys OPC UA Java SDK Sample Console Server |
| opc.tcp://\<hostname\>:4841                               | Unified Automation Demo Server               |
| opc.tcp://\<hostname\>:62541/Quickstarts/DataAccessServer | OPC Foundation QuickStart Data Access Server |

where \<hostname\> is the host name of the computer in which the server is running.[^1]

[^1]: Note that ’localhost’ may also work. The servers define a list of endpoints that they are listening to. The client can only connect to the server using an Url that matches one of these endpoints. But the solution will convert it to the actual hostname, if the server does not define ’localhost’ in its endpoints.

    Also IP number can only be used, if the server also defines the respective endpoint using the IP number.

    For Windows hostname resolution, see <http://technet.microsoft.com/en-us/library/bb727005.aspx>. If you are using the client in Linux, you cannot use NetBIOS computer names to access Windows servers. In general, it is best to use TCP/IP DNS names from all clients. Alternatively, you can always use the IP address of the computer, if you make sure that the server also initializes an endpoint using the IP address, in addition to the hostname.

Instead of using the complete URI like this, you can alternatively define the connection in parts using the properties Protocol2, Host, Port and ServerName. These make up the Url as follows:

\<Protocol\>[^2]://\<Host\>:\<Port\>\<ServerName\>

[^2]: Note that not all servers support all different protocols, e.g. the OPC Foundation Java stack only supports the binary (opc.tcp) protocol at the moment.

The SampleClient uses as default the following Uri:

```
    // connect Url?
    Uri serverUrl = !string.IsNullOrEmpty(extraArg) ? new Uri(extraArg) : new Uri("opc.tcp://localhost:62555/SampleServer");
```

## Discover Servers

For UA Server discovering you can use the GetUaServers() methods. To be able to find an UA server, all UA Servers running on a machine should register with the UA Local Discovery Server using the Stack API.

If a UA Server running on a machine is registered with the UA Local Discovery Server a client can discover it using the following code:

```
    // Discover all local UA servers
    List<string> servers = Discover.GetUaServers(application.Configuration);

    Console.WriteLine("Found local OPC UA Servers:");
    foreach (var server in servers)
    {
        Console.WriteLine(String.Format("{0}", server));
    }
```

Remote servers can be discovered by specifying a Uri object like shown below:

```
    // Discover all remote UA servers
    Uri discoveryUri = new Uri("opc.tcp://technosoftware:4840/");
    servers = Discover.GetUaServers(application.Configuration, discoveryUri);

    Console.WriteLine("Found remote OPC UA Servers:");
    foreach (var server in servers)
    {
        Console.WriteLine(String.Format("{0}", server));
    }

```