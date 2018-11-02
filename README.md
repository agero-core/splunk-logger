# splunk-logger

Splunk Logger is a library for logging to Splunk using HTTP collector. It automatically collects environment information and adds it to log.

## Usage:

```csharp
ILogger logger = 
    new Logger
    (
        collectorUri: new Uri("<Your Splunk Collector Url>"), 
        authorizationToken: "<Your Splunk Access Token>", 
        applicationName: "TestName", 
        applicationVersion: "1.2.3.4", 
        timeout: 3000
    );
	
bool result = 
    logger.Log
    (
        type: "TestInfo", 
        message: "Test message", 
        data: new { test1 = "test1", test2 = "test2" },
        correlationId: "1234567"
    );	
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
      "operatingSystem":"Microsoft Windows NT 6.1.7601 Service Pack 1",
      "is64BitOperatingSystem":true,
      "processorCount":4,
      "clrVersion":"4.0.30319.42000",
      "is64BitProcess":false,
      "machineName":"MACHINE-NAME",
      "localTime":"2017-08-16T14:43:07.7658829-04:00",
      "utcTime":"2017-08-16T18:43:07.7668831+00:00",
      "hostName":"MACHINE-NAME",
      "ipAddresses":[  
         "123.123.1.0",
         "10.0.0.1"
      ],
      "ec2InstanceId": "i-1234567"
   },
   "app":{  
      "test1":"test1",
      "test2":"test2"
   },
   "correlationId": "1234567"
}
```


## Running Test Cases

### Set up:

Create the json file **logger-settings.json** with the below configuration.


```json
{
  "SplunkCollectorUrl": "<Your Splunk Collector Url>",
  "AuthenticationToken": "<Your Splunk Access Token>"
}
```