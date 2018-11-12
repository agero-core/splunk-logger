using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Agero.Core.Lazy;

namespace Agero.Core.SplunkLogger.Helpers
{
    /// <summary>Splunk Logger Helper</summary>
    public static class LoggerHelper
    {
        private static readonly SyncLazy<IReadOnlyCollection<string>> _ipAddresses = 
            new SyncLazy<IReadOnlyCollection<string>>(() =>
            {
                var hostName = Dns.GetHostName();
                if (string.IsNullOrWhiteSpace(hostName))
                    return Array.Empty<string>();

                var hostEntry = Dns.GetHostEntry(hostName);

                return
                    hostEntry.AddressList
                        .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                        .Select(a => a.ToString())
                        .ToArray();
            });

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
                    ipAddresses = _ipAddresses.Value
                };
        }
    }
}
