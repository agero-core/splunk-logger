using System.Runtime.Serialization;
using Agero.Core.Checker;

namespace Agero.Core.SplunkLogger.Models
{
    [DataContract]
    internal class LogBody
    {
        public LogBody(string type, string name, string version, string message, object systemData, object applicationData = null, string correlationId = null)
        {
            Check.ArgumentIsNullOrWhiteSpace(type, "type");
            Check.ArgumentIsNullOrWhiteSpace(name, "name");
            Check.ArgumentIsNullOrWhiteSpace(version, "version");
            Check.ArgumentIsNullOrWhiteSpace(message, "message");
            Check.ArgumentIsNull(systemData, "systemData");

            Type = type;
            Name = name;
            Version = version;
            Message = message;
            SystemData = systemData;
            ApplicationData = applicationData;
            CorrelationId = correlationId;
        }

        [DataMember(Name = "type")]
        public string Type { get; }

        [DataMember(Name = "name")]
        public string Name { get; }

        [DataMember(Name = "version")]
        public string Version { get; }

        [DataMember(Name = "message")]
        public string Message { get; }

        [DataMember(Name = "system")]
        public object SystemData { get; }

        [DataMember(Name = "app")]
        public object ApplicationData { get; }

        [DataMember(Name = "correlationId")]
        public string CorrelationId { get; }
    }
}