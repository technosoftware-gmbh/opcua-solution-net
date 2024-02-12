# UA Server Design

## Overview

We concentrate in this tutorial on the simple *SampleServer*, a console-based application for testing the server specific features. This tutorial will refer to that code while explaining the different steps to take to accomplish the main tasks of an OPC UA server.

![](../images/UaServerDesign.png)

The Server API is designed to ease the server development by handling the standard tasks which all servers need to do and provides APIs that allow users to add their own functionality. These APIs fall into the following categories:

- The first level, the Core Layer (Technosoftware.UaServer namespace), implements all common code necessary for an OPC UA Server and manages communication connection infrastructure like UaGenericServer, UaGenericNodeManager, GenericServerData, MasterNodeManager, ResourceManager, SubscriptionManager, SessionManager, EventManager and RequestManager.
- The second level, the Base Layer (Technosoftware.UaBaseServer namespace) are interfaces and implementations like UaServer, UaBaseServer and UaBaseNodeManager for information integration and mapping of the OPC UA defined services. It includes a standard implementation for the Base Layer and require that the user creates subclasses of the classes defined here. It also defines the interfaces which must be implemented by the third level like IUaServerPlugin, IUaOptionalServerPlugin and IUaReverseConnectServerPlugin which allows a very quick and easy implementation of an OPC UA Server.
- The third level, the implementation of the UA server implements the interfaces like IUaServerPlugin, IUaOptionalServerPlugin and IUaReverseConnectServerPlugin. The user can start implementing a base OPC UA server supporting Data Access and Simple Events and later enhance it through adding subclasses of the interfaces and classes defined in the Base Layer.

The Core Layer classes are used in user applications and tools and should not be changed or subclassed. The Base Layer classes can be subclassed and extended by your application.

**Important:**

Implementations based on these examples allows the creation of one node tree under the root like shown below:


![](../images/SampleAddressSpace.png)

“My Data” is the only tree you can create with this Base OPC UA Server Implementation.

## Tutorial “Simple OPC UA Server”

This chapter describes how to create your first OPC UA server based on the Simulation SampleServer project. It canb be found in /tutorials/SampleCompany/Simple/SampleServer. It shows the basic steps required to adapt one of the examples to your needs. It is highly recommended to follow this tutorial. More detailed topics will be later discussed and base on the sample server created in this chapter.

### Goals of this tutorial

You will learn the following topics:

1. Minimum required changes to start a new OPC UA server development.
2. Create a basic address space with a static base data variable as well as a simulated data variable.
3. Using of OnSimpleWriteValue to be able to handle changes coming from an OPC UA client.
4. Changing values of the simulated variable (OnSimulation).

### Overview

The example OPC UA Servers uses all a common layout and consists at least of the following C\# files:

| **Name**                   | **Description**                                                                                                                                                                                                                                                                                      |
|:---------------------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Directory.Build.props**  | A user-defined file that provides customizations to projects under a directory. In our case just imports the targets.props file. You can find it in the root of the repository.                                                                                                                      |
| **targets.props**          | A user-defined file that defines for which target the server should be build for. Adapt the following parameter to your needs: \<AppTargetFramework\>net6.0\</AppTargetFramework\> Possible values are net462, net472, net48, netcoreapp3.1 or net6.0 You can find it in the root of the repository. |
| **Namespace.cs**           | Constant(s) defining the used namespaces for the application, e.g.   public const string SampleServer = "http://samplecompany.com/SampleServer";                                                                                                                                                     |
| **Program.cs**             | Contains the code to startup the base OPC UA server (UaServer).                                                                                                                                                                                                                                      |
| **UaServerPlugin.cs**      | Contains the implementation of the interfaces IUaServerPlugin, IUaOptionalServerPlugin and optionally IUaReverseConnectServerPlugin This file consists because of historic reasons where a real simple server with just a plugin DLL was possible.                                                   |
| **\*.Config.xml**          | Contains the configuration of the OPC UA Server, e.g. ApplicationName. For the empty server it is named:   Technosoftware.SampleServer.Config.xml                                                                                                                                                    |
| **\*Server.cs**            | Implements the basic OPC UA Server and overrides the UaBaseServer class. For the empty server it is named:   Technosoftware.SampleServer.cs                                                                                                                                                          |
| **\*ServerNodeManager.cs** | Implements the OPC UA NodeManager and overrides the UaBaseNodeManager class. For the empty server it is named:   Technosoftware.SampleServerNodeManager.cs                                                                                                                                           |

In this tutorial we use a general naming of files and namespaces:

Company: SampleCompany or for the ready to use servers Technosoftware   
Server Namespace: SampleServer  
Namespace: SampleCompany.SampleServer  
File Names: Starts with SampleCompany.SampleServer

### Start developing your own OPC UA Server

You need the following files to be able to develop your own OPC UA Server:

1. Copy the directoy with the Sample OPC UA Server at /tutorials/SampleCompany/Simple/SampleServer, to your development directory, e.g., /solutions
2. Copy /Directory.Build.props and /targets.props to /solutions
3. Rename the folder to your needs.

In the following chapters you will find some required and recommended changes necessary before you start changing the source code. For a first OPC UA Server we recommend changing only those values and keep everything else untouched. As you proceed further with this tutorial more and more changes and enhancements are done.

#### Renaming files

It is a good idea to rename several files to fit your project. As mentioned in the last chapter we use a general naming of files and namespaces and for this tutorial we defined it as:

Company: SampleCompany   
Server Namespace: SampleServer  
Namespace: SampleCompany.SampleServer  
File Names: Starts with SampleCompany.SampleServer

Of course, you can replace the company name with your company name and the server namespace with a more usable one for your project.

Please change the names of the following files:

1. SampleCompany.SampleServer.Config.xml
2. SampleCompany.SampleServer.cs
3. SampleCompany.SampleServer.csproj
4. SampleCompany.SampleServerNodeManager.cs

In this tutorial we just use the same file names.

#### Changes to project and building options

1. Please open the SampleCompany.SampleServer.csproj file with Visual Studio 2022.
2. Replace SampleCompany.SampleServer in all your project files.
3. Check the project properties Package dialog or edit the SampleCompany.SampleServer.csproj file and adapt the following entries: \<Company\>, \<Product\>, \<Description\> and \<Copyright\>
4. The SampleCompany.SampleServer.csproj file use \$(AppTargetFramework) for the target it should build for. That one is defined in the targets.props file and can be changed to net462, net472, net48, netcoreapp3.1 or net6.0.

#### Changes in NameSpaces.cs

1. Open the Namespaces.cs file with Visual Studio 2022.
2. Rename variable SampleServer for the whole project.
3. Change the value of SampleServer.

#### Changes in SampleCompany.SampleServer.Config.xml

1. Open the SampleCompany.SampleServer.Config.xml file with Visual Studio 2022.
2. The values of the following global parameters changes must at least be changed:
    - \<ApplicationName\>SampleCompany OPC UA Sample Server\</ApplicationName\>
    - \<ApplicationUri\>urn:localhost:SampleCompany:SampleServer\</ApplicationUri\>
    - \<ProductUri\>uri:samplecompany.com:SampleServer\</ProductUri\>
3. In the section \<SecurityConfiguration\>\<ApplicationCertificate\> you need to define the Subject name for the application certificate.
    - \<SubjectName\>CN=SampleCompany OPC UA Sample Server, C=CH, S=Aargau, O=SampleCompany, DC=localhost\</SubjectName\>
4. In the section \<ServerConfiguration\> you need to define the URL the server should be reachable. One entry is for the opc.tcp protocol and one for the https protocol:
    - \<ua:String\>opc.tcp://localhost:62555/SampleServer\</ua:String\>
    - \<ua:String\>opc.https://localhost:62556/SampleServer</ua:String\>

#### Changes of class names

1. Open the SampleCompany.SampleServer.cs file with Visual Studio 2022.
2. Rename the EmptyServer class to SampleServer for the whole project.
3. Open the SampleCompany.SampleServerNodeManager.cs file with Visual Studio 2022.
4. Rename the EmptyServerNodeManager class to SampleServerNodeManager for the whole project.
5. Open the Program.cs file with Visual Studio 2022.
6. Rename the class MySampleServer for the whole project.
7. Rename the Task SampleServer method for the whole project.

#### Program Customization

The Program.cs file contains the startup of the application and the OPC UA Server. The important part related to OPC UA are:

**Using statements:**

```
using Opc.Ua;

using Technosoftware.UaConfiguration;
using Technosoftware.UaServer.Sessions;  
using Technosoftware.UaBaseServer;
```

**Definitions of variables:**

Two variables in the class MySampleServer:

```
private static UaServer uaServer\_ = new UaServer();
private static UaServerPlugin uaServerPlugin\_ = new UaServerPlugin();
```

**Console output in Main:**

Change the console output:

```
Console.WriteLine("SampleCompany {0} OPC UA Sample Server", Utils.IsRunningOnMono() ? "Mono" : ".NET Core");
```

**Starting the server:**

Within the Task SampleServer():

```
// start the server.
await uaServer_.StartAsync(uaServerPlugin_, "SampleCompany.SampleServer",  
 passwordProvider, OnCertificateValidation, null);
```

Please ensure that the second parameter "SampleCompany.SampleServer" fits your configuration file name SampleCompany.SampleServer.Config.xml.

#### UaServerPlugin Customization

The UaServerPlugin class can return a custom UaBaseNodeManager and UaBaseServer as shown in the sample below.

Open the UaServerPlugin.cs file with Visual Studio 2022 and verify the following shown methods:

```
    public class UaServerPlugin : IUaServerPlugin, IUaOptionalServerPlugin, IUaReverseConnectServerPlugin, IDisposable
    {

        #region Optional Server Plugin methods
        public UaBaseServer OnGetServer()
        {
            Utils.Trace(Utils.TraceMasks.Information, "OnGetServer(): Request the instance of the server.");
            return new SampleServer();
        }

        public UaBaseNodeManager OnGetNodeManager(IUaServer opcServer, IUaServerData uaServer, ApplicationConfiguration configuration, params string[] namespaceUris)
        {
            Utils.Trace(Utils.TraceMasks.Information, "OnGetNodeManager(): Request the instance of the node manager.");
            return new SampleServerNodeManager(opcServer, this, uaServer, configuration, namespaceUris);
        }
        #endregion
    }
```
Using a custom UaBaseNodeManager means that the OnCreateAddressSpace method of the UaServerPlugin class is no longer called. Instead, the custom UaBaseNodeManager must override the CreateAddressSpace method as shown in the next chapter.

Another changes/checks are required in the method:

```
    public ServerProperties OnGetServerProperties()
    {
        Utils.Trace(Utils.TraceMasks.Information, "OnGetServerProperties(): Request some standard information of the server}.");
        var properties = new ServerProperties {
            ManufacturerName = "SampleCompany",
            ProductName = "SampleCompany OPC UA Sample Server",
            ProductUri = "http://samplecompany.com/SampleServer/v1.0",
            SoftwareVersion = GetAssemblySoftwareVersion(),
            BuildNumber = GetAssemblyBuildNumber(),
            BuildDate = GetAssemblyTimestamp()
        };
        return properties;
    }
        
    public string[] OnGetNamespaceUris()
    {
        Utils.Trace(Utils.TraceMasks.Information, "OnGetNamespaceUris(): Request the supported namespace Uris.");
        // set one namespace for the type model.
        var namespaceUrls = new string[1];
        namespaceUrls[0] = Model.Namespaces.SampleServer;
        return namespaceUrls;
    }
```

#### SampleCompany.SampleServerNodeManager Customization

Open the SampleCompany.SampleServerNodeManager.cs file with Visual Studio 2022 and do the following additions and changes:

Add using directives:

```
using System.Threading;
```

Add private variables under:

```
    #region Private Fields 
    private Timer simulationTimer_;
    private ushort simulationInterval_ = 1000;
    private bool simulationEnabled_ = true;
    private List<BaseDataVariableState> dynamicNodes_;
    private int cycleId_;
```

Add the following event handlers:

```
    #region Event Handlers
    private ServiceResult OnWriteInterval(ISystemContext context, NodeState node, ref object value)
    {
        try
        {
            simulationInterval_ = (ushort)value;

            if (simulationEnabled_)
            {
                _ = simulationTimer_.Change(100, simulationInterval_);
            }

            return ServiceResult.Good;
        }
        catch (Exception e)
        {
            Utils.Trace(e, "Error writing Interval variable.");
            return ServiceResult.Create(e, StatusCodes.Bad, "Error writing Interval variable.");
        }
    }

    private ServiceResult OnWriteEnabled(ISystemContext context, NodeState node, ref object value)
    {
        try
        {
            simulationEnabled_ = (bool)value;

            _ = simulationTimer_.Change(100, simulationEnabled_ ? simulationInterval_ : 0);

            return ServiceResult.Good;
        }
        catch (Exception e)
        {
            Utils.Trace(e, "Error writing Enabled variable.");
            return ServiceResult.Create(e, StatusCodes.Bad, "Error writing Enabled variable.");
        }
    }
```

These event handlers are called if an OPC UA client changes the value of the interval variable (OnWriteInterval) or the enabled variable (OnWriteEnabled). Within the address space creation these event handlers are assigned with

```
    intervalVariable.OnSimpleWriteValue = OnWriteInterval;
    intervalVariable.OnSimpleWriteValue = OnWriteInterval;
```

Add the following helper methods:

```
    #region Helper Methods
    private BaseDataVariableState CreateDynamicVariable(NodeState parent, string path, string name, string description, NodeId dataType, int valueRank, byte accessLevel, object initialValue)
    {
        BaseDataVariableState variable = CreateBaseDataVariableState(parent, path, name, description, dataType, valueRank, accessLevel, initialValue);
        dynamicNodes_.Add(variable);
        return variable;
    }

    private BaseDataVariableState CreateDynamicVariable(NodeState parent, string path, string name, string description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue)
    {
        BaseDataVariableState variable = CreateBaseDataVariableState(parent, path, name, description, dataType, valueRank, accessLevel, initialValue);
        dynamicNodes_.Add(variable);
        return variable;
    }

    private void DoSimulation(object state)
    {
        try
        {
            lock (Lock)
            {
                foreach (BaseDataVariableState variable in dynamicNodes_)
                {
                    _ = opcServer_.WriteBaseVariable(variable, GetNewValue(variable), StatusCodes.Good, DateTime.UtcNow);
                }
                OnRaiseSystemEvents();
            }
        }
        catch (Exception e)
        {
           Utils.Trace(e, "Unexpected error doing simulation.");
        }
    }

    private void OnRaiseSystemEvents()
    {
        try
        {
           for (var ii = 1; ii < 3; ii++)
           {
                // construct translation object with default text.
                var info = new TranslationInfo(
                    "SystemCycleStarted",
                    "en-US",
                    "The system cycle '{0}' has started.",
                    ++cycleId_);

                // construct the event.
                var e = new SystemCycleStartedEventState(null);

                e.Initialize(
                    SystemContext,
                    null,
                    (EventSeverity)ii,
                    new LocalizedText(info));

                _ = e.SetChildValue(SystemContext, Opc.Ua.BrowseNames.SourceName, "System", false);
                _ = e.SetChildValue(SystemContext, Opc.Ua.BrowseNames.SourceNode, ObjectIds.Server, false);
                _ = e.SetChildValue(SystemContext, new QualifiedName(Model.BrowseNames.CycleId, NamespaceIndex),
                                    cycleId_.ToString(), false);

                var step = new CycleStepDataType { Name = "Step 1", Duration = 1000 };

                _ = e.SetChildValue(SystemContext,
                                    new QualifiedName(Model.BrowseNames.CurrentStep, NamespaceIndex), step, false);
                _ = e.SetChildValue(SystemContext, new QualifiedName(Model.BrowseNames.Steps, NamespaceIndex),
                                    new[] { step, step }, false);

                ServerData.ReportEvent(e);
            }
        }
        catch (Exception e)
        {
            Utils.Trace(e, "Unexpected error in OnRaiseSystemEvents");
        }
    }
    #endregion
```

The DoSimulation method loops through all simulated values (in this tutorial just one) and uses WriteBaseVariable of the base OPC UA server implementation (opcServer_) to write a new value to it.

Replace the CreateAddressSpace with the one shown below:

```
    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        lock (Lock)
        {
            dynamicNodes_ = new List<BaseDataVariableState>();

            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out IList<IReference> references))
            {
                externalReferences[ObjectIds.ObjectsFolder] = References = new List<IReference>();
            }
            else
            {
                References = references;
            }

            LoadPredefinedNodes(SystemContext, externalReferences);

            // Create the root folder for all nodes of this server
            FolderState root = CreateFolderState(null, "My Data", new LocalizedText("en", "My Data"),
                new LocalizedText("en", "Root folder of the Sample Server. All nodes must be placed under this root."));

            try
            {
                #region Scalar_Static
                ResetRandomGenerator(1);
                FolderState scalarFolder = CreateFolderState(root, "Scalar", "Scalar", null);
                BaseDataVariableState scalarInstructions = CreateBaseDataVariableState(scalarFolder, "Scalar_Instructions", "Scalar_Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                scalarInstructions.Value = "A library of Variables of different data-types.";
                FolderState staticFolder = CreateFolderState(scalarFolder, "Scalar_Static", "Scalar_Static", null);
                const string scalarStatic = "Scalar_Static_";
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Double", "Double", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Float", "Float", null, DataTypeIds.Float, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Number", "Number", null, DataTypeIds.Number, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateBaseDataVariableState(staticFolder, scalarStatic + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);

                BaseDataVariableState decimalVariable = CreateBaseDataVariableState(staticFolder, scalarStatic + "Decimal", "Decimal", null, DataTypeIds.DecimalDataType, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                // Set an arbitrary precision decimal value.
                var largeInteger = BigInteger.Parse("1234567890123546789012345678901234567890123456789012345");
                var decimalValue = new DecimalDataType {
                    Scale = 100,
                    Value = largeInteger.ToByteArray()
                };
                decimalVariable.Value = decimalValue;
                #endregion

                #region Scalar_Simulation
                ResetRandomGenerator(6);
                FolderState simulationFolder = CreateFolderState(scalarFolder, "Scalar_Simulation", "Simulation", null);
                const string scalarSimulation = "Scalar_Simulation_";
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Double", "Double", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Float", "Float", null, DataTypeIds.Float, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Number", "Number", null, DataTypeIds.Number, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);

                BaseDataVariableState intervalVariable = CreateBaseDataVariableState(simulationFolder, scalarSimulation + "Interval", "Interval", null, DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                intervalVariable.Value = simulationInterval_;
                intervalVariable.OnSimpleWriteValue = OnWriteInterval;

                BaseDataVariableState enabledVariable = CreateBaseDataVariableState(simulationFolder, scalarSimulation + "Enabled", "Enabled", null, DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                enabledVariable.Value = simulationEnabled_;
                enabledVariable.OnSimpleWriteValue = OnWriteEnabled;
                #endregion

                #region Methods
                FolderState methodsFolder = CreateFolderState(root, "Methods", "Methods", null);
                const string methods = "Methods_";

                BaseDataVariableState methodsInstructions = CreateBaseDataVariableState(methodsFolder, methods + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                methodsInstructions.Value = "Contains methods with varying parameter definitions.";

                #region Hello Method
                MethodState helloMethod = CreateMethodState(methodsFolder, methods + "Hello", "Hello", OnHelloCall);
                // set input arguments
                Argument inputArgument1 = CreateArgument("String value", "String value", BuiltInType.String, ValueRanks.Scalar);
                _ = AddInputArguments(helloMethod, new[] { inputArgument1 });

                // set output arguments
                Argument outputArgument1 = CreateArgument("Hello Result", "Hello Result", BuiltInType.String, ValueRanks.Scalar);
                _ = AddOutputArguments(helloMethod, new[] { outputArgument1 });
                #endregion

                #endregion

                #region Access Rights Handling
                FolderState folderAccessRights = CreateFolderState(root, "AccessRights", "AccessRights", null);
                const string accessRights = "AccessRights_";
                BaseDataVariableState accessRightsInstructions = CreateBaseDataVariableState(folderAccessRights, accessRights + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                accessRightsInstructions.Value = "This folder will be accessible to all authenticated users who enter, but contents therein will be secured.";


                #region Access Rights Operator Handling
                // sub-folder for "AccessOperator"
                FolderState folderAccessRightsAccessOperator = CreateFolderState(folderAccessRights, "AccessRights_AccessOperator", "AccessOperator", null);
                const string accessRightsAccessOperator = "AccessRights_AccessOperator_";

                BaseDataVariableState arOperatorRW = CreateBaseDataVariableState(folderAccessRightsAccessOperator, accessRightsAccessOperator + "OperatorUsable", "OperatorUsable", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                arOperatorRW.AccessLevel = AccessLevels.CurrentReadOrWrite;
                arOperatorRW.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                arOperatorRW.OnReadUserAccessLevel = OnReadOperatorUserAccessLevel;
                arOperatorRW.OnSimpleWriteValue = OnWriteOperatorValue;
                arOperatorRW.OnReadValue = OnReadOperatorValue;
                dynamicNodes_.Add(arOperatorRW);
                #endregion

                #region Access Rights Administrator Handling
                // sub-folder for "AccessAdministrator"
                FolderState folderAccessRightsAccessAdministrator = CreateFolderState(folderAccessRights, "AccessRights_AccessAdministrator", "AccessAdministrator", null);
                const string accessRightsAccessAdministrator = "AccessRights_AccessAdministrator_";

                BaseDataVariableState arAdministratorRW = CreateBaseDataVariableState(folderAccessRightsAccessAdministrator, accessRightsAccessAdministrator + "AdministratorOnly", "AdministratorOnly", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                arAdministratorRW.AccessLevel = AccessLevels.CurrentReadOrWrite;
                arAdministratorRW.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                arAdministratorRW.OnReadUserAccessLevel = OnReadAdministratorUserAccessLevel;
                arAdministratorRW.OnSimpleWriteValue = OnWriteAdministratorValue;
                arAdministratorRW.OnReadValue = OnReadAdministratorValue;
                dynamicNodes_.Add(arAdministratorRW);
                #endregion
                #endregion
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Error creating the address space.");
            }
            // Add all nodes under root to the server
            AddPredefinedNode(SystemContext, root);
            simulationTimer_ = new Timer(DoSimulation, null, 1000, 1000);
        }
    }
```

### Testing your OPC UA server

You should now be able to build and start your first OPC UA server. Using the Unified Automation UaExpert you can use to connect to the OPC UA server and should see the following address space:

![](../images/SimpleServerAddressSpace.png)

You can drag&drop the variables “Double”, “Enabled” and “Interval” to the Data Access View and see the value of the “Double” variable changing. By changing the “Interval” the update interval of the “Double” should chang and with setting “Enabled” to false no more changes should happen to the “Double” variable.

### SampleServer project

For your convenience, the resulting sample server is also available in the distribution at

/tutorials/SampleCompany/Simple/SampleServer