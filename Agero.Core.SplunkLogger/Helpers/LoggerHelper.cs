using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Agero.Core.Checker;
using Agero.Core.Lazy;
using Agero.Core.RestCaller;
using Agero.Core.RestCaller.Extensions;

namespace Agero.Core.SplunkLogger.Helpers
{
    /// <summary>Splunk Logger Helper</summary>
    public static class LoggerHelper
    {
        private static readonly SyncLazy<string> Ec2InstanceId = new SyncLazy<string>(() => GetAwsMetaDataValue("instance-id"));

        private static readonly IRESTCaller RestCaller = new RESTCaller();

        private static string GetAwsMetaDataValue(string key)
        {
            Check.ArgumentIsNullOrWhiteSpace(key, "key");

            try
            {
                var uri = new Uri($"http://169.254.169.254/latest/meta-data/{key}");
                var response = RestCaller.Get(uri, timeout: 100);

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

        /// <summary>Gets environment data of the underlying instance</summary>
        public static object GetSystemData()
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
                    ec2InstanceId = Ec2InstanceId
                };
        }
    }
}
