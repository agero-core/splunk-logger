using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Agero.Core.Checker;
using Agero.Core.Lazy;
using Agero.Core.RestCaller;
using Agero.Core.RestCaller.Exceptions;
using Agero.Core.RestCaller.Extensions;
using Agero.Core.SplunkLogger.Models;
using Newtonsoft.Json;

namespace Agero.Core.SplunkLogger
{
    /// <summary>Splunk Logger</summary>
    public class Logger : ILogger
    {
        private readonly IReadOnlyDictionary<string, string> _headers;

        private readonly SyncLazy<string> _ec2InstanceId;

        /// <summary>Constructor</summary>
        /// <param name="collectorUri">Splunk HTTP collector URL</param>
        /// <param name="authorizationToken">Splunk authorization token</param>
        /// <param name="applicationName">Unique application name</param>
        /// <param name="applicationVersion">Application version</param>
        /// <param name="timeout">Splunk HTTP collector timeout (milliseconds)</param>
        public Logger(Uri collectorUri, string authorizationToken, string applicationName, string applicationVersion, int timeout = 10000)
        {
            Check.ArgumentIsNull(collectorUri, "collectorUri");
            Check.ArgumentIsNullOrWhiteSpace(authorizationToken, "authorizationToken");
            Check.ArgumentIsNullOrWhiteSpace(applicationName, "applicationName");
            Check.ArgumentIsNullOrWhiteSpace(applicationVersion, "applicationVersion");
            Check.Argument(timeout > 0, "timeout > 0");

            CollectorUri = collectorUri;
            AuthorizationToken = authorizationToken;
            ApplicationName = applicationName;
            ApplicationVersion = applicationVersion;
            Timeout = timeout;

            _headers =
                new Dictionary<string, string>
                {
                    {"Authorization", $"Splunk {authorizationToken}"}
                };

            _ec2InstanceId = new SyncLazy<string>(() => GetAwsMetaDataValue("instance-id"));
        }

        /// <summary>Splunk HTTP collector URL</summary>
        public Uri CollectorUri { get; }

        /// <summary>Splunk authorization token</summary>
        public string AuthorizationToken { get; }

        /// <summary>Unique application name</summary>
        public string ApplicationName { get; }

        /// <summary>Application version</summary>
        public string ApplicationVersion { get; }

        /// <summary>Splunk HTTP collector timeout (milliseconds)</summary>
        public int Timeout { get; }

        private string GetAwsMetaDataValue(string key)
        {
            Check.ArgumentIsNullOrWhiteSpace(key, "key");

            try
            {
                var uri = new Uri($"http://169.254.169.254/latest/meta-data/{key}");
                var response = _restCaller.Get(uri, timeout: 100);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                    return null;

                return response.Text;
            }
            catch
            {
                return null;
            }
        }

        private static string[] GetIpAddresses()
        {
            var hostName = Dns.GetHostName();
            if (string.IsNullOrWhiteSpace(hostName))
                return new string[] { };

            var hostEntry = Dns.GetHostEntry(hostName);

            return
                hostEntry.AddressList
                    .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                    .Select(a => a.ToString())
                    .ToArray();
        }

        private object GetSystemData()
        {
            return
                new
                {
                    userName = Environment.UserName,
                    userDomainName = Environment.UserDomainName,
                    operatingSystem = Environment.OSVersion.VersionString,
                    is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                    processorCount = Environment.ProcessorCount,
                    clrVersion = Environment.Version.ToString(),
                    is64BitProcess = Environment.Is64BitProcess,
                    machineName = Environment.MachineName,
                    localTime = DateTimeOffset.Now,
                    utcTime = DateTimeOffset.UtcNow,
                    hostName = Dns.GetHostName(),
                    ipAddresses = GetIpAddresses(),
                    ec2InstanceId = _ec2InstanceId.Value
                };
        }

        private readonly IRESTCaller _restCaller = new RESTCaller();

        private string CreateBody(string type, string message, object data, string correlationId)
        {
            Check.ArgumentIsNullOrWhiteSpace(type, "type");
            Check.ArgumentIsNullOrWhiteSpace(message, "message");

            var body = new LogBody(type, ApplicationName, ApplicationVersion, message, GetSystemData(), data, correlationId);

            return JsonConvert.SerializeObject(body);
        }
        
        private const string REQUEST_TEMPLATE = @"{{ ""event"": {0} }}";

        /// <summary>Submits log to Splunk</summary>
        /// <param name="type">Log type (Error, Info, etc.)</param>
        /// <param name="message">Log text message</param>
        /// <param name="data">Any object which serialized into JSON</param>
        /// <param name="correlationId">Any optional string which can correlate different logs</param>
        /// <returns>Returns flag whether log submitted to Splunk</returns>
        public bool Log(string type, string message, object data = null, string correlationId = null)
        {
            Check.ArgumentIsNullOrWhiteSpace(type, "type");
            Check.ArgumentIsNullOrWhiteSpace(message, "message");

            var body = CreateBody(type, message, data, correlationId);
            var splunkBody = string.Format(REQUEST_TEMPLATE, body);

            try
            {
                var response = _restCaller.Post(CollectorUri, body: splunkBody, headers: _headers, timeout: Timeout, compressBody: true);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                    return true;

                LogTrace(response, body);
                
                return false;
            }
            catch (WebException ex)
            {
                LogTrace(ex.ToString(), body);
                
                return false;
            }
            catch (RESTCallerException ex) when (ex.InnerException is WebException)
            {
                LogTrace(ex.ToString(), body);
                
                return false;
            }
        }

        /// <summary>Submits log to Splunk</summary>
        /// <param name="type">Log type (Error, Info, etc.)</param>
        /// <param name="message">Log text message</param>
        /// <param name="data">Any object which serialized into JSON</param>
        /// <param name="correlationId">Any optional string which can correlate different logs</param>
        /// <returns>Returns flag whether log submitted to Splunk</returns>
        public async Task<bool> LogAsync(string type, string message, object data = null, string correlationId = null)
        {
            Check.ArgumentIsNullOrWhiteSpace(type, "type");
            Check.ArgumentIsNullOrWhiteSpace(message, "message");

            var body = CreateBody(type, message, data, correlationId);
            var splunkBody = string.Format(REQUEST_TEMPLATE, body);

            try
            {
                var response = await _restCaller.PostAsync(CollectorUri, body: splunkBody, headers: _headers, timeout: Timeout, compressBody: true);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                    return true;

                LogTrace(response, body);
                
                return false;
            }
            catch (WebException ex)
            {
                LogTrace(ex.ToString(), body);
                
                return false;
            }
            catch (RESTCallerException ex) when (ex.InnerException is WebException)
            {
                LogTrace(ex.ToString(), body);

                return false;
            }
        }
        
        private void LogTrace(object data, string body)
        {
            Check.ArgumentIsNull(data, "data");
            Check.ArgumentIsNullOrWhiteSpace(body, "body");

            try
            {
                var splunkFailedLog =
                    new
                    {
                        message = "Request to Splunk failed.",
                        url = CollectorUri,
                        applicationName = ApplicationName,
                        applicationVersion = ApplicationVersion,
                        system = GetSystemData(),
                        data = data
                    };
                Trace.WriteLine(JsonConvert.SerializeObject(splunkFailedLog), "ERROR");

                Trace.WriteLine(body, "WARNING");
            }
            catch
            {
                // ignored
            }
        }
    }
}