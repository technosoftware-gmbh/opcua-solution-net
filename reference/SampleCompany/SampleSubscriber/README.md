# OPC UA PubSub .NET - Sample Subscriber

## Introduction
This OPC application was created to provide the sample code for creating Subscriber applications using the OPC UA PubSub .NET. The Sample Subscriber is configured to run in parallel with the [Sample Subscriber](../SamplePublisher/README.md)

## How to build and run the sample publisher from Visual Studio
1. Open the solution **Tutorials.sln** with Visual Studio 2022.
2. Choose the project `SampleSubscriber` in the Solution Explorer and set it with a right click as `Startup Project`.
3. Hit `F5` to build and execute the sample.

## How to build and run the sample subscriber on Windows, Linux and macOS
This section describes how to run the **SampleSubscriber**.

## Start the Subscriber
1. Open a command prompt.
2. Navigate to the folder **examples/SampleCompany/SampleSubscriber**.
3. To run the Subscriber sample type 

`dotnet run --project SampleCompany.SampleSubscriber.csproj --framework net6.0.` 

The Subscriber will start and listen for network messages sent by the Reference Publisher. 

## Command Line Arguments for *SampleSubscriber*
 **SampleSubscriber** can be executed using the following command line arguments:

 -  -h|help - Shows usage information
 -  -m|mqtt_json - Creates a connection using there MQTT with Json encoding Profile. This is the default option.
 -  -u|udp_uadp - Creates a connection using there UDP with UADP encoding Profile. 

To run the Subscriber sample using a connection with MQTT with Json encoding execute: 

		dotnet run --project SampleCompany.SampleSubscriber.csproj --framework net6.0 

		or 

		dotnet run --project SampleCompany.SampleSubscriber.csproj --framework net6.0 -m

To run the Subscriber sample using a connection with the UDP with UADP encoding execute: 

		dotnet run --project SampleCompany.SampleSubscriber.csproj --framework net6.0 -u
