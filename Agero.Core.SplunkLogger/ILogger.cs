using System.Threading.Tasks;

namespace Agero.Core.SplunkLogger
{
    /// <summary>Splunk Logger</summary>
    public interface ILogger
    {
        /// <summary>Submits log to Splunk</summary>
        /// <param name="type">Log type (Error, Info, etc.)</param>
        /// <param name="message">Log text message</param>
        /// <param name="data">Any object which will be serialized into JSON</param>
        /// <param name="correlationId">Any optional string which can correlate different logs</param>
        /// <returns>Returns flag whether log submitted to Splunk</returns>
        bool Log(string type, string message, object data = null, string correlationId = null);

        /// <summary>Submits log to Splunk</summary>
        /// <param name="type">Log type (Error, Info, etc.)</param>
        /// <param name="message">Log text message</param>
        /// <param name="data">Any object which will be serialized into JSON</param>
        /// <param name="correlationId">Any optional string which can correlate different logs</param>
        /// <returns>Returns flag whether log submitted to Splunk</returns>
        Task<bool> LogAsync(string type, string message, object data = null, string correlationId = null);
    }
}