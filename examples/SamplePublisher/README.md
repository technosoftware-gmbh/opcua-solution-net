# OPC UA PubSub .NET - Sample Publisher

## Introduction
This OPC application was created to provide the sample code for creating Publisher applications using the OPC UA PubSub .NET. The Sample Publisher is configured to run in parallel with the [Sample Subscriber](../SampleSubscriber/README.md)

## How to build and run the sample publisher from Visual Studio
1. Open the solution **TutorialSamples.sln** with Visual Studio 2022.
2. Choose the project `SamplePublisher` in the Solution Explorer and set it with a right click as `Startup Project`.
3. Hit `F5` to build and execute the sample.

## How to build and run the sample publisher on Windows, Linux and macOS
This section describes how to run the **SamplePublisher**.

## Start the Publisher
1. Open a command prompt.
2. Navigate to the folder **examples/SampleCompany/SamplePublisher**.
3. To run the Publisher sample execute: 

`dotnet run --project SampleCompany.SamplePublisher.csproj --framework net6.0` 

The Publisher will start and publish network messages that can be consumed by the Sample Subscriber. 

## Command Line Arguments for *SamplePublisher*
 **SamplePublisher** can be executed using the following command line arguments:
 

 -  -h|help - Shows usage information
 -  -m|mqtt_json - Creates a connection using there MQTT with Json encoding Profile. This is the default option.
 -  -u|udp_uadp - Creates a connection using there UDP with UADP encoding Profile. 

To run the Publisher sample using a connection with MQTT with Json encoding execute: 

		dotnet run --project SampleCompany.SamplePublisher.csproj --framework net6.0 

		or 

		dotnet run --project SampleCompany.SamplePublisher.csproj --framework net6.0 -m

To run the Publisher sample using a connection with the UDP with UADP encoding execute: 

		dotnet run --project SampleCompany.SamplePublisher.csproj --framework net6.0 -u