# UA Client Design

The Client API is designed to ease client development by handling the standard tasks which all clients need to do. The tasks include:

-  sending the session keep alive requests.
-  managing the publishing pipeline.
-  keeping track of the status for subscription and monitored items.
-  managing a client-side node cache
-  processing and caching incoming data change and event notifications.
-  saving and restoring the session state including the subscriptions and monitored items.

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

## Accessing an OPC UA Server

There are only a few classes required by an UA client to handle operations with a UA server. In general, an UA client

-  creates one or more Sessions by using the Session class.
-  creates one or more Subscriptions within a Session by using the Subscription class.
-  adding one or more MonitoredItems within a Subscription by using the MonitoredItem class

### Session

The Session class inherits from the SessionClient which means all the UA services are in general accessible as methods on the Session object.

The Session object provides several helper methods including a Session.CreateAsync() method which Creates and Opens the Session. The process required when establishing a session with a Server is as follows:

-  The Client application must choose the EndpointDescription to use. This can be done manually or by getting a list of available EndpointDescriptions by using the Discover.GetEndpointDescriptions() method.
-  The client can also use the Discover.SelectEndpoint() method which choose the best match for the current settings.
-  The Client takes the EndpointDescription and uses it to Create the Session object by using the Session.CreateAsync() method. If Session.CreateAsync() succeeds the client application will be able to call other methods.

Example from the SampleClient (MyUaClient.cs):

```
    // Get the endpoint by connecting to server's discovery endpoint.
    // Try to find the first endopint with security.
    var endpointConfiguration = EndpointConfiguration.Create(configuration_);
    var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

    TraceableSessionFactory sessionFactory = TraceableSessionFactory.Instance;

    // Create the session
	Session session = await Session.CreateAsync(
		configuration_,
		endpoint,
		false,
		false,
		configuration_.ApplicationName,
		SessionLifeTime,
		UserIdentity,
		null,
		ct
	).ConfigureAwait(false);
```

#### Keep Alive

After creating the session, the Session object starts periodically reading the current state from the Server at a rate specified by the KeepAliveInterval (default is 5s). Each time a response is received the state and latest timestamp is reported as a SessionKeepAliveEvent event. If the response does not arrive after 2 KeepAliveIntervals have elapsed a SessionKeepAliveEvent event with an error is raised. The KeepAliveStopped property will be set to true. If communication resumes the normal SessionKeepAliveEvent events will be reported and the KeepAliveStopped property will go back to false.

The client application uses the SessionKeepAliveEvent event and KeepAliveStopped property to detect communication problems with the server. In some cases, these communication problems will be temporary but while they are going on the client application may choose not to invoke any services because they would likely timeout. If the channel does not come back on its own the client application will execute whatever error recovery logic it has.

Client applications need to ensure that the SessionTimeout is not set too low. If a call times out the WCF channel is closed automatically and the client application will need to create a new one. Creating a new channel will take time. The KeepAliveStopped property allows applications to detect failures even if they are using a long SessionTimeout.

After creating the session the client can add a keep alive event handler.

Example from the Simple SampleClient (MyUaClient.cs):

```
	// Assign the created session
	if (session != null && session.Connected)
	{
		Session = session;

		// override keep alive interval
		Session.KeepAliveInterval = KeepAliveInterval;

		// support transfer
		Session.DeleteSubscriptionsOnClose = false;
		Session.TransferSubscriptionsOnReconnect = true;

		// set up keep alive callback.
		Session.SessionKeepAliveEvent += OnSessionKeepAlive;

		// prepare a reconnect handler
		reconnectHandler_ = new SessionReconnectHandler(true, ReconnectPeriodExponentialBackoff);
	}
```

Now the client gets updated with the keep alive events and can easily add a reconnect feature:

```
	/// <summary>
	/// Handles a keep alive event from a session and triggers a reconnect if necessary.
	/// </summary>
	private void OnSessionKeepAlive(object sender, SessionKeepAliveEventArgs e)
	{
		try
		{
			var session = (Session)sender;

			// check for events from discarded sessions.
			if (!Session.Equals(session))
			{
				return;
			}

			// start reconnect sequence on communication error.
			if (ServiceResult.IsBad(e.Status))
			{
				if (ReconnectPeriod <= 0)
				{
					Utils.LogWarning("KeepAlive status {0}, but reconnect is disabled.", e.Status);
					return;
				}

				SessionReconnectHandler.ReconnectState state = reconnectHandler_.BeginReconnect(Session, ReconnectPeriod, OnReconnectComplete);
				if (state == SessionReconnectHandler.ReconnectState.Triggered)
				{
					Utils.LogInfo("KeepAlive status {0}, reconnect status {1}, reconnect period {2}ms.", e.Status, state, ReconnectPeriod);
				}
				else
				{
					Utils.LogInfo("KeepAlive status {0}, reconnect status {1}.", e.Status, state);
				}

				// cancel sending a new keep alive request, because reconnect is triggered.
				e.CancelKeepAlive = true;

				return;
			}
		}
		catch (Exception exception)
		{
			Utils.LogError(exception, "Error in OnKeepAlive.");
		}
	}
```

As soon as the session keep alive event handler (OnSessionKeepAlive) detects that a reconnect must be done a reconnect handler is called to begin the reconnect. In the above sample the following lines are doing this:

```
	SessionReconnectHandler.ReconnectState state = reconnectHandler_.BeginReconnect(Session, ReconnectPeriod, OnReconnectComplete);
	if (state == SessionReconnectHandler.ReconnectState.Triggered)
	{
		Utils.LogInfo("KeepAlive status {0}, reconnect status {1}, reconnect period {2}ms.", e.Status, state, ReconnectPeriod);
	}
	else
	{
		Utils.LogInfo("KeepAlive status {0}, reconnect status {1}.", e.Status, state);
	}
```

As soon as the OPC UA stack reconnected to the OPC UA Server the OnReconnectComplete handler is called and can then finish the client-side actions.

The following sample is taken from the Simple SampleClient (MyUaClient.cs) and shows how to implement the OnReconnectComplete handler:

```
	/// <summary>
	/// Called when the reconnect attempt was successful.
	/// </summary>
	private void OnReconnectComplete(object sender, EventArgs e)
	{
		// ignore callbacks from discarded objects.
		if (!ReferenceEquals(sender, reconnectHandler_))
		{
			return;
		}

		lock (lock_)
		{
			// if session recovered, Session property is null
			if (reconnectHandler_.Session != null)
			{
				// ensure only a new instance is disposed
				// after reactivate, the same session instance may be returned
				if (!ReferenceEquals(Session, reconnectHandler_.Session))
				{
					output_.WriteLine("--- RECONNECTED TO NEW SESSION --- {0}", reconnectHandler_.Session.SessionId);
					IUaSession session = Session;
					Session = (Session)reconnectHandler_.Session;
					Utils.SilentDispose(session);
				}
				else
				{
					output_.WriteLine("--- REACTIVATED SESSION --- {0}", reconnectHandler_.Session.SessionId);
				}
			}
			else
			{
				output_.WriteLine("--- RECONNECT KeepAlive recovered ---");
			}
		}
	}
```

Important in the OnServerReconnectComplete handler are the following lines:

```
	// ensure only a new instance is disposed
	// after reactivate, the same session instance may be returned
	if (!ReferenceEquals(Session, reconnectHandler_.Session))
	{
		output_.WriteLine("--- RECONNECTED TO NEW SESSION --- {0}", reconnectHandler_.Session.SessionId);
		IUaSession session = Session;
		Session = (Session)reconnectHandler_.Session;
		Utils.SilentDispose(session);
	}
```

The session used up to now must be replaced with the new session provided by the reconnect handler. The client itself does not need to create a new session, subscriptions or MonitoredItems. That’s all done by the OPC UA stack. So with taking the session provided by the reconnect handler all subriptions and MonitoredItems are then still valid and functional.

#### Cache

The Session object provides a cache that can be used to store Nodes that are accessed frequently. The cache is particularly useful for storing the types defined by the server because the client will often need to check if one type is a subtype of another. The cache can be accessed via the NodeCache property of the Session object. The type hierarchies stored in the cache can be searched using the TypeTree property of the NodeCache or Session object (the both return a reference to the same object).

The NodeCache is populated with the FetchNode() method which will read all of the attributes for the Node and the fetch all of its references. The Find() method on the NodeCache looks for a previously cached version of the Node and calls the FetchNode() method if it does not exist.

Client applications that wish to use the NodeCache must pre-fetch all the ReferenceType hierarchy supported by the Server by calling FetchTypeTree() method on the Session object.

The Find() method is used during Browse of the address space.

#### Events

The Session object is responsible for sending and processing the Publish requests. Client applications can receive events whenever a new NotificationMessage is received by subscribing to the SessionNotificationEvent event.

-  The SessionPublishErrorEvent event is raised whenever a Publish response reports an error.
-  The SubscriptionsChangedEvent event indicates when a Subscription is added or removed.
-  The SessionClosingEvent event indicates that the Session is about to be closed.

**Important**: The Simple Sample doesn’t show the usage of these features.

#### Multi-Threading

The Session is designed for multi-threaded operation because client application frequently need to make multiple simultaneous calls to the Server. However, this is only guaranteed for calls using the Session class. Client applications should avoid calling services directly which update the Session state, e.g. CreateSession or ActivateSession.

### Browse the address space

The first thing to do is typically to find the server items you wish to read or write. The OPC UA address space is a bit more complex structure than you might expect to, but nevertheless, you can explore it by browsing.

In the solution, the address space is accessed through the Browser class. You can call browse to request nodes from the server. You start from any known node, typically the root folder and follow references between the nodes. In a first step, you create a browser object as follows:

```
	/// <summary>
	/// Browse Server nodes
	/// </summary>
	public void Browse(IUaSession session)
	{
		if (session == null || session.Connected == false)
		{
			output_.WriteLine("Session not connected!");
			return;
		}

		try
		{
			// Create a Browser object
			var browser = new Browser(session) {
				// Set browse parameters
				BrowseDirection = BrowseDirection.Forward,
				NodeClassMask = (int)NodeClass.Object | (int)NodeClass.Variable,
				ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
				IncludeSubtypes = true
			};

			NodeId nodeToBrowse = ObjectIds.Server;

			// Call Browse service
			output_.WriteLine("Browsing {0} node...", nodeToBrowse);
			ReferenceDescriptionCollection browseResults = browser.Browse(nodeToBrowse);

			// Display the results
			output_.WriteLine("Browse returned {0} results:", browseResults.Count);

			foreach (ReferenceDescription result in browseResults)
			{
				output_.WriteLine("     DisplayName = {0}, NodeClass = {1}", result.DisplayName.Text, result.NodeClass);
			}
		}
		catch (Exception ex)
		{
			// Log Error
			output_.WriteLine($"Browse Error : {ex.Message}.");
		}
	}
```

The ObjectIds.Server node represents the root folder of the server node, so starting from the root folder can be done with the following call:

```
	// Call Browse service
	output_.WriteLine("Browsing {0} node...", nodeToBrowse);
	ReferenceDescriptionCollection browseResults = browser.Browse(nodeToBrowse);

```

### Read Value(s)

Once you have a node selected, you can read the attributes of the node. There are actually several alternative read-calls that you can make in the Session class, e.g.: 

```
	/// <summary>
	/// Read a list of nodes from Server
	/// </summary>
	public void ReadNodes(IUaSession session)
	{
		if (session == null || session.Connected == false)
		{
			output_.WriteLine("Session not connected!");
			return;
		}

		try
		{
			#region Read a node by calling the Read Service

			// build a list of nodes to be read
			var nodesToRead = new ReadValueIdCollection()
			{
				// Value of ServerStatus
				new ReadValueId() { NodeId = Variables.Server_ServerStatus, AttributeId = Attributes.Value },
				// BrowseName of ServerStatus_StartTime
				new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.BrowseName },
				// Value of ServerStatus_StartTime
				new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.Value }
			};

			// Read the node attributes
			output_.WriteLine("Reading nodes...");

			// Call Read Service
			_ = session.Read(
				null,
				0,
				TimestampsToReturn.Both,
				nodesToRead,
				out DataValueCollection resultsValues,
				out DiagnosticInfoCollection diagnosticInfos);

			// Validate the results
			validateResponse_(resultsValues, nodesToRead);

			// Display the results.
			foreach (DataValue result in resultsValues)
			{
				output_.WriteLine("Read Value = {0} , StatusCode = {1}", result.Value, result.StatusCode);
			}
			#endregion

			#region Read the Value attribute of a node by calling the Session.ReadValue method
			// Read Server NamespaceArray
			output_.WriteLine("Reading Value of NamespaceArray node...");
			DataValue namespaceArray = session.ReadValue(Variables.Server_NamespaceArray);
			// Display the result
			output_.WriteLine($"NamespaceArray Value = {namespaceArray}");
			#endregion
		}
		catch (Exception ex)
		{
			// Log Error
			output_.WriteLine($"Read Nodes Error : {ex.Message}.");
		}
	}
```

In general, you should avoid calling the read methods for individual items. If you need to read several items at the same time, you should consider using mySessionTechnosoftwareSampleServer .ReadValues() [6.5.4]. It is a bit more complicated to use, but it will only make a single call to the server to read any number of values. Or if you want to monitor variables that are changing in the server, you had better use the Subscription, as described in chapter [0].

### Write Value(s)

Like reading, you can also write values to the server. For example:

```
	/// <summary>
	/// Write a list of nodes to the Server.
	/// </summary>
	public void WriteNodes(IUaSession session)
	{
		if (session == null || session.Connected == false)
		{
			output_.WriteLine("Session not connected!");
			return;
		}

		try
		{
			// Write the configured nodes
			var nodesToWrite = new WriteValueCollection();

			// Int32 Node - Objects\CTT\Scalar\Scalar_Static\Int32
			var intWriteVal = new WriteValue {
				NodeId = new NodeId("ns=2;s=Scalar_Static_Int32"),
				AttributeId = Attributes.Value,
				Value = new DataValue {
					Value = 100
				}
			};
			nodesToWrite.Add(intWriteVal);

			// Float Node - Objects\CTT\Scalar\Scalar_Static\Float
			var floatWriteVal = new WriteValue {
				NodeId = new NodeId("ns=2;s=Scalar_Static_Float"),
				AttributeId = Attributes.Value,
				Value = new DataValue {
					Value = (float)100.5
				}
			};
			nodesToWrite.Add(floatWriteVal);

			// String Node - Objects\CTT\Scalar\Scalar_Static\String
			var stringWriteVal = new WriteValue {
				NodeId = new NodeId("ns=2;s=Scalar_Static_String"),
				AttributeId = Attributes.Value,
				Value = new DataValue {
					Value = "String Test"
				}
			};
			nodesToWrite.Add(stringWriteVal);

			// Write the node attributes
			output_.WriteLine("Writing nodes...");

			// Call Write Service
			_ = session.Write(null,
							nodesToWrite,
							out StatusCodeCollection results,
							out DiagnosticInfoCollection diagnosticInfos);

			// Validate the response
			validateResponse_(results, nodesToWrite);

			// Display the results.
			output_.WriteLine("Write Results :");

			foreach (StatusCode writeResult in results)
			{
				output_.WriteLine("     {0}", writeResult);
			}
		}
		catch (Exception ex)
		{
			// Log Error
			output_.WriteLine($"Write Nodes Error : {ex.Message}.");
		}
	}
```

As a response, you get a status code – indicating if the write was successful or not.

If the operation fails, e.g. because of a connection loss, you will get an exception. For service call errors, such that the server could not handle the service request at all, you can expect ServiceResultException.

### Calling Methods

OPC UA also defines a mechanism to call methods in the server objects. To find out if an object defines methods, you can call ReadNode() of the session and use as parameter the NodeId you want to call a method from:

```
    private readonly NodeId methodsNodeId_ = new NodeId("ns=2;s=Methods");
    private readonly NodeId callHelloMethodNodeId_ = new NodeId("ns=2;s=Methods_Hello");

    INode node = mySessionSampleServer_.ReadNode(callHelloMethodNodeId_);

    MethodNode methodNode = node as MethodNode;

    if (methodNode != null)
    {
        // Node supports methods
    }
```

OPC UA Methods have a variable list of Input and Output Arguments. To make this example simple we have choosen a method with one input and one output argument. To be able to call a method you need to know the node of the method, in our example callHelloMethodNodeId\_ but also the parent node, in our example methodsNodeId_. Calling the method then done by

```
	/// <summary>
	/// Call UA method
	/// </summary>
	public void CallMethod(IUaSession session)
	{
		if (session == null || session.Connected == false)
		{
			output_.WriteLine("Session not connected!");
			return;
		}

		try
		{
			// Define the UA Method to call
			// Parent node - Objects\My Data\Methods
			// Method node - Objects\My Data\Methods\Hello
			var objectId = new NodeId("ns=2;s=Methods");
			var methodId = new NodeId("ns=2;s=Methods_Hello");

			// Define the method parameters
			// Input argument requires a Float and an UInt32 value
			var inputArguments = "from Call Method";
			IList<object> outputArguments = null;

			// Invoke Call service
			output_.WriteLine("Calling UA method for node {0} ...", methodId);
			outputArguments = session.Call(objectId, methodId, inputArguments);

			// Display results
			output_.WriteLine("Method call returned {0} output argument(s):", outputArguments.Count);

			foreach (var outputArgument in outputArguments)
			{
				output_.WriteLine("     OutputValue = {0}", outputArgument.ToString());
			}
		}
		catch (Exception ex)
		{
			output_.WriteLine("Method call error: {0}", ex.Message);
		}
	}
```


### Create a MonitoredItem

The MonitoredItem class stores the client-side state for a MonitoredItem belonging to a Subscription on a Server. It maintains two sets of properties:

1.  The values requested when the MonitoredItem is/was created
2.  The current values based on the revised values returned by the Server.

The requested properties are what is saved when then MonitoredItem is serialized.

The requested properties are saved when then MonitoredItem is serialized. Please keep in mind that the server may change (revise) some values requested by the client. The revised properties are returned in the Status property, which is of type MonitoredItemStatus.

The NodeId for the MonitoredItem can be specified as an absolute NodeId or as a starting NodeId followed by RelativePath string which conforms to the syntax defined in the OPC Unified Architecture Specification Part 4. The RelativePath class included in the Stack can parse these strings and produce the structures required by the UA services.

Changes to any of the properties which affect the state of the MonitoredItem on the Server are not applied immediately. Instead the ParametersModified flag is set and the changes will only be applied when the ApplyChanges method on the Subscription is called. Note that changes to parameters which can only be specified when the MonitoredItem was created are ignored if the MonitoredItem has already been created. Client applications that wish to change these parameters must delete the monitored item and then re-create it.

The current values for monitoring parameters are stored in the Status property. Client application must use the Status. Error property to check an error occurs while creating or modifying the item. MonitoredItems that specify a RelativePath string may have encountered an error parsing or translating the RelativePath. When such an error occurs the Error property is set and the MonitoredItem is not created.

The MonitoredItem maintains a local queue for data changes or events received from the Server. This means the client application does not need to explicitly process NotificationMessages and can simply read the latest value from the MonitoredItem whenever it is required. The length of the local queue is controlled by the CacheQueueSize property.

The MonitoredItem provides a MonitoredItemNotification event which can be used by the client application to receive events whenever a new notification is received from the Server. It is always called after it is added to the cache.

The MonitoredItem is designed for multi-threaded operation because the Publish requests may arrive on any thread. However, data which is accessed while updating the cache is protected with a separate synchronization lock from data that is used while updating the MonitoredItem parameters. This means notifications can continue to arrive while other threads update the MonitoredItem parameters.

Client applications must be careful when update MonitoredItem parameters while another thread has called ApplyChanges on the Subscription because it could lead to situation where the state of the MonitoredItem on the Server does not match the state of the MonitoredItem on the client.

The Advanced Sample client uses the following code to create a MonitoredItem:

```
	// Create MonitoredItems for data changes (Sample Server)
	var intMonitoredItem = new MonitoredItem(subscription.DefaultItem) {
		// Int32 Node - Objects\CTT\Scalar\Simulation\Int32
		StartNodeId = new NodeId("ns=2;s=Scalar_Simulation_Int32"),
		AttributeId = Attributes.Value,
		DisplayName = "Int32 Variable",
		SamplingInterval = 1000,
		QueueSize = 10,
		DiscardOldest = true
	};
```

### Create a Subscription

The Subscription class stores the client-side state for a Subscription with a Server. It maintains two sets of properties:

-  the values requested when the Subscription is/was created and
-  the current values based on the revised values returned by the Server.

The Subscription object is designed for batch operations. This means the subscription parameters and the MonitoredItem can be updated several times but the changes to the Subscription on the Server do not happen until the ApplyChanges() method is called. After the changes are complete the SubscriptionStatusChangedEvent event is reported with a bit mask indicating what was updated.

In normal operation, the important settings for the Subscription are the PublishingEnabled and PublishingInterval. The following example shows how the WorkshopClientConsole creates a subscription:

```
	// Define Subscription parameters
	var subscription = new Subscription(session.DefaultSubscription) {
		DisplayName = "Console ReferenceClient Subscription",
		PublishingEnabled = true,
		PublishingInterval = 1000,
		LifetimeCount = 0,
		MinLifetimeInterval = minLifeTime,
	};
```

The settings KeepAliveCount, LifetimeCount, MaxNotificationsPerPublish and the Priority of the Subscription can also be omitted to use the default values.

The **KeepAliveCount** defines how many times the PublishingInterval needs to expire without having notifications available before the server sends an empty message to the client indicating that the server is still alive but no notifications are available.

The **LifetimeCount** defines how many times the PublishingInterval expires without having a connection to the client to deliver data. If the server is not able to deliver notification messages after this time, it deletes the Subsction to clear the resources. The LifetimeCount must be at minimum three times the KeepAliveCount. Both values are negotiated between the client and the server.

The **MaxNotificationsPerPublish** is used to limit the size of the notification message sent from the server to the client. The number of notifications is set by the client but the server can send fewer notifications in one message if his limit is smaller than the client-side limit. If not all available notifications can be sent with one notification message, another notification message is sent.

The **Priority** setting defines the priority of the Subscription relative to the other Subscriptions created by the Client. This allows the server to handle Subscriptions with higher priorities first in high-load scenarios.

The Subscription class provides several helper methods including a Constructor with default values for several. The process required when using a subscription is as follows:

1.  The Subscription object must be created. This can be done by using the default constructor and using one or more of the properties available.
2.  Items (MonitoredItem) must be added to the subscription.
3.  The subscription must be added to the session.
4.  The subscription must be created for the session.
5.  The subscription changes must be applied, because of the above-mentioned batch functionality.

When a Subscription is created, it must start sending Publish requests. It starts off the process by telling the Session object to send one request. Additional Publish requests can be send by calling the Republish() method. Applications can use additional Publish requests to compensate for high network latencies because once the pipeline is filled the Server will be able to return a steady stream of notifications.

Once the Subscription has primed the pump the Session object keeps it going by sending a new Publish whenever it receives a successful response. If an error occurs the Session raises a SessionPublishErrorEvent event and does not send another Publish.

If everything is working properly the Session save the message in cache at least once per keep alive interval. If a NotificationMessage does not arrive it means there are network problems, bugs in the Server or high priority Subscriptions are taking precedence. The keep alive timer is designed to detect these problems and to automatically send additional Publish requests. When the keepalive timer expires, it checks the time elapsed since the last notification message. If publishing appears to have stopped the PublishingStopped property will be true and the Subscription will raise a PublishStatusChangedEvent event and send another Publish request. Client applications must assume that any cache data values are out of date when they receive the PublishStatusChangedEvent event (e.g. the StatusCode should be set to UncertainLastKnownValue). However, client applications do not need to do anything else since the interruption may be temporary. It is up to the client application to decide when to give up on a Session and to try again with a new Session.

The Subscription object checks for missing sequence numbers when it receives a NotificationMessage. If there is a gap it starts a timer that will call Republish() in 1s if the gap still exists. This delay is necessary because the multi-threaded stack on the client side may process responses out of order even if they are received in order.

The Subscription maintains a cache of messages received. The size of this cache is controlled by the MaxMessageCount property. When a new message is received, the Subscription adds it to the cache and removes any extras. It then extracts the notifications and pushes them to the MonitoredItem identified by the ClientHandle in the notification.

The Subscription is designed for multi-threaded operation because the Publish requests may arrive on any thread. However, data which is accessed while processing an incoming message is protected with a separate synchronization lock from data that is used while updating the Subscription parameters. This means notifications can continue to arrive while network operations to update the Subscription state on the server are in progress. However, no more than one operation to update the Subscription state can proceed at one time. Closing the Session will interrupt any outstanding operations. Any synchronization locks held by the subscription are released before any events are raised.

### Subscribe to data changes

In order to monitor data changes, you have to subscribe to the MonitoredItemNotificationEvent as shown below:

```
    intMonitoredItem.MonitoredItemNotificationEvent += OnMonitoredDataItemNotification;
```

You also must add the MonitoredItem to the subscription

```
    subscription.AddItem(intMonitoredItem);
```

If you are finished with adding MonitoredItems to the subscription you have to add the subscription to the session:

```
    _ = session.AddSubscription(subscription);
```

Now you can finish creating the subscription and apply the changes to the session by using the following code:

```
    // Create the subscription on Server side
    subscription.Create();
	
    // Create the monitored items on Server side
    subscription.ApplyChanges();
```

The specified event callback OnMonitoredItemNotification looks like:

```
	private void OnMonitoredDataItemNotification(object sender, MonitoredItemNotificationEventArgs e)
	{
		var monitoredItem = sender as MonitoredItem;
		try
		{
			// Log MonitoredItem Notification event
			var notification = e.NotificationValue as MonitoredItemNotification;
			output_.WriteLine("Notification: {0} \"{1}\" and Value = {2}.", notification.Message.SequenceNumber, monitoredItem.ResolvedNodeId, notification.Value);
		}
		catch (Exception ex)
		{
			output_.WriteLine("OnMonitoredItemNotification error: {0}", ex.Message);
		}
	}
```

### Subscribe to events

In addition to subscribing to data changes in the server variables, you may also listen to events from event notifiers. You can use the same subscriptions, but additionally, you must also define the event filter, which defines the events that you are interested in and the event fields you wish to monitor. To make handling of the filters a bit easier the WorkshopClientConsole uses a utility class FilterDefinition. The following code creates a filter:

```
    // the filter to use.
    filterDefinition = new FilterDefinition();
```

The default constructor subscribes to all events coming from the RootFolder of the Server object (ObjectIds.Server) with a Severity of EventSeverity.Min and all events of type ObjectTypeIds.BaseEventType.

The FilterDefinition class also has a helper method to create the select clause:

```
    // must specify the fields that the client is interested in.
    filterDefinition.SelectClauses = filterDefinition.ConstructSelectClauses(
                        mySessionSampleServer_,
                        ObjectIds.Server,
                        ObjectTypeIds.BaseEventType
                        );
```

The code above creates a select claus which includes all fields of the BaseEventType. Another helper method of the FilterDefinition class creates the MonitoredItem:

```
    // create a monitored item based on the current filter settings.
    MonitoredItem monitoredEventItem =
                        filterDefinition.CreateMonitoredItem(mySessionSampleServer_);
```

Now we can subscribe to the event changes with:

```
    // set up callback for notifications.
    monitoredEventItem.MonitoredItemNotificationEvent += OnMonitoredEventItemNotification;

```

See the WorkshopClientConsole for the code of the callback OnMonitoredEventItemNotification(). After creating the MonitoredItem it must be added to the subscription and the changes must be applied:

```
    mySubscription.AddItem(monitoredEventItem);
    mySubscription.ApplyChanges();
    mySubscription.ConditionRefresh();
```

### History Access

The UA Servers may also provide history information for the nodes. You can read the Historizing attribute of a Variable node to see whether history is supported. For this example we use the Historical Access Sample Server with the Endpoint Uri opc.tcp://\<localhost\>:55551/TechnosoftwareHistoricalAccessServer.

#### Check if a Node supports historizing

You can get information about a node by reading the Attribute Attributes.AccessLevel and check whether the node supports HistoricalAccess. The code we use for this is shown below:

```
    ReadValueId nodeToRead = new ReadValueId();
    nodeToRead.NodeId = dynamicHistoricalAccessNodeId_;
    nodeToRead.AttributeId = Attributes.AccessLevel;
    nodesToRead.Add(nodeToRead);

    // Get Information about the node object
    mySessionHistoricalAccessServer_.Read(
                        null,
                        0,
                        TimestampsToReturn.Neither,
                        nodesToRead,
                        out values,
                        out diagnosticInfos);

    ClientBase.ValidateResponse(values, nodesToRead);
    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

    for (int ii = 0; ii < nodesToRead.Count; ii++)
    {
        byte accessLevel = values[ii].GetValue<byte>(0);

        // Check if node supports HistoricalAccess
        if ((accessLevel & AccessLevels.HistoryRead) != 0)
        {
            // Node supports HistoricalAccess
        }
    }
```

#### Reading History

To actually read history data you use the HistoryRead() method of the Session. The example below reads a complete history for a single node (specified by nodeId):

```
    HistoryReadResultCollection results = null;

    // do it the hard way (may take a long time with some servers).
    ReadRawModifiedDetails details = new ReadRawModifiedDetails();
    details.StartTime = DateTime.UtcNow.AddDays(-1);
    details.EndTime = DateTime.MinValue;
    details.NumValuesPerNode = 10;
    details.IsReadModified = false;
    details.ReturnBounds = false;

    HistoryReadValueId nodeToReadHistory = new HistoryReadValueId();
    nodeToReadHistory.NodeId = dynamicHistoricalAccessNodeId_;

    HistoryReadValueIdCollection nodesToReadHistory = new HistoryReadValueIdCollection();
    nodesToReadHistory.Add(nodeToReadHistory);

    // Read the historical data
    mySessionHistoricalAccessServer_.HistoryRead(
        null,
        new ExtensionObject(details),
        TimestampsToReturn.Both,
        false,
        nodesToReadHistory,
        out results,
        out diagnosticInfos);

    ClientBase.ValidateResponse(results, nodesToRead);
    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

    if (StatusCode.IsBad(results[0].StatusCode))
    {
        throw new ServiceResultException(results[0].StatusCode);
    }

    // Get the historical data
    HistoryData historyData = ExtensionObject.ToEncodeable(results[0].HistoryData) as HistoryData;
```

What you need to be aware of is that there are several “methods” that the historyRead supports, depending on which HistoryReadDetails you use. For example, in the above example we used ReadRawModifiedDetails, to read a raw history (the same structure is used to read Modified history as well, therefore the name).

## UserIdentity and UserIdentityTokens

The solution provides the UserIdentity class which converts UA user identity tokens to and from the SecurityTokens used by WCF. The solution currently supports UserNameSecurityToken, X509SecurityToken, SamlSecurityToken and any other subtype of SecurityToken which is supported by the WCF WSSecurityTokenSerializer class. The UA specification requires that UserIdentityTokens be encrypted or signed before they are sent to the Server. UserIdentityToken class provides several methods that implement these features.

**Important**: This feature is not supported in the WorkshopClientConsole.

## Reverse Connect Handling

To be able to use the Reverse Connect Feature the following code must be added to your client:

### Program.cs

The configuration of the client Url’s to be used for reverse connect can be done after starting the client. For example like this:

```
  private async Task<Session> ConsoleSampleClient()
  {
      ApplicationInstance application = new ApplicationInstance 
                                              { ApplicationType = ApplicationType.Client };
 
      #region Create an Application Configuration
      Console.WriteLine(" 1 - Create an Application Configuration.");
      ExitCode = ExitCode.ErrorCreateApplication;
 
      // Load the Application Configuration and use the specified config section 
      // "Technosoftware.SampleClient"
      ApplicationConfiguration config = await application.LoadConfigurationAsync(
                                                 "Technosoftware.SampleClient");
 
      Uri reverseConnectUrl = new Uri("opc.tcp://localhost:65300");

      reverseConnectManager_ = null;
      if (reverseConnectUrl != null)
      {
          // start the reverse connection manager
          reverseConnectManager_ = new ReverseConnectManager();
          reverseConnectManager_.AddEndpoint(reverseConnectUrl);
          reverseConnectManager_.StartService(config);
      }
      #endregion
```

The above code configures the reverse connection. Session creation is also a bit different to a normal session. Below the code for a normal creation of a session:

```
  // create worker session
  if (reverseConnectManager_ == null)
  {
      session_ = await CreateSessionAsync(config, selectedEndpoint,
                                                  userIdentity).ConfigureAwait(false);
  }
```

And below the code for a creation of a session with reverse connect:

```
  else
  {
      Console.WriteLine("   Waiting for reverse connection.");
      // Define the cancellation token.
      var source = new CancellationTokenSource(60000);
      var token = source.Token;
      try
      {
          ITransportWaitingConnection connection =
                                         await reverseConnectManager_.WaitForConnection(
                                                 new Uri(endpointUrl_), null, token);
          if (connection == null)
          {
              throw new ServiceResultException(StatusCodes.BadTimeout,
                  "Waiting for a reverse connection timed out.");
          }
 
          session_ = await CreateSessionAsync(config, connection, selectedEndpoint,
                                              userIdentity).ConfigureAwait(false);
      }
      finally
      {
          source.Dispose();
      }
  }
```