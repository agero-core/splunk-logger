# splunk-logger

Splunk Logger is a library for logging to Splunk using HTTP collector. It automatically collects environment information and adds it to log.

## Set up:

Create the json file **logger-settings.json** with the below contents.

```json
{
  "SplunkCollectorUrl": "<Your Splunk Collector Url>",
  "AuthenticationToken": "<Your Splunk Access Token>"
}
```

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