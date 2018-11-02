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