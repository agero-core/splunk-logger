# Splunk Logger

[![NuGet Version](http://img.shields.io/nuget/v/Agero.Core.SplunkLogger.svg?style=flat)](https://www.nuget.org/packages/Agero.Core.SplunkLogger/) 
[![NuGet Downloads](http://img.shields.io/nuget/dt/Agero.Core.SplunkLogger.svg?style=flat)](https://www.nuget.org/packages/Agero.Core.SplunkLogger/)

Splunk Logger is a .NET library for logging to Splunk using HTTP collector. It automatically collects environment information and adds it to log.

## Usage:
Create instance:
```csharp
ILogger logger = 
    new Logger(
        collectorUri: new Uri("<Your Splunk Collector Url>"), 
        authorizationToken: "<Your Splunk Access Token>", 
        applicationName: "TestName", 
        applicationVersion: "1.2.3.4", 
        timeout: 3000);
```
Create log:
```csharp
bool result = 
    logger.Log(
        type: "TestInfo", 
        message: "Test message", 
        data: new { test1 = "test1", test2 = "test2" },
        correlationId: "1234567");	
```
or
```csharp
bool result = 
    await logger.LogAsync(
        type: "TestInfo", 
        message: "Test message", 
        data: new { test1 = "test1", test2 = "test2" },
        correlationId: "1234567");	
```

This creates the following log in Splunk:

```json
{  
   "type":"TestInfo",
   "name":"TestName",
   "version":"1.2.3.4",
   "message":"Test message",
   "system":{  
      "userName":"DomainName\\user",
      "userDomainName":"DomainName",
      "operatingSystem":"Unix 18.2.0.0",
      "is64BitOperatingSystem":true,
      "processorCount":4,
      "clrVersion":"4.0.30319.42000",
      "is64BitProcess":false,
      "machineName":"MACHINE-NAME",
      "localTime":"2017-08-16T14:43:07.7658829-04:00",
      "utcTime":"2017-08-16T18:43:07.7668831+00:00",
      "hostName":"MACHINE-NAME",
      "ipAddresses":[  
         "123.123.1.0"
      ]
   },
   "app":{  
      "test1":"test1",
      "test2":"test2"
   },
   "correlationId": "1234567"
}
```

## Running Tests:

Create the json file **logger-settings.json** with the below configuration under the project **Agero.Core.SplunkLogger.Tests**.

```json
{
  "splunkCollectorUrl": "<Your Splunk Collector Url>",
  "authenticationToken": "<Your Splunk Access Token>"
}
```