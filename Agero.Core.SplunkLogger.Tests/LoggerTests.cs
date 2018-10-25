using System;
using System.IO;
using System.Threading.Tasks;
using Agero.Core.RestCaller.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Agero.Core.SplunkLogger.Tests
{
    [TestClass]
    public class LoggerTests
    {
        private readonly LoggerTestsSetup _splunkCollectorInfo = JsonConvert.DeserializeObject<LoggerTestsSetup>(File.ReadAllText(@"../../../logger-settings.json"));

        [TestMethod]
        public async Task Log_Should_Return_True_When_Log_To_Splunk_Successful()
        {
            // Arrange
            var logger = new Logger(new Uri(_splunkCollectorInfo.SplunkCollectorUrl), _splunkCollectorInfo.AuthenticationToken, "TestName", "TestVersion", 3000);

            // Act
            var result = await logger.LogAsync("TestInfo", "Test message", new { test1 = "test1", test2 = "test2" });

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Log_Should_Return_False_When_Collector_Url_Does_Not_Exist()
        {
            // Arrange
            var logger = new Logger(new Uri("http://wrong-url/"), _splunkCollectorInfo.AuthenticationToken, "TestName", "TestVersion", 1000);

            // Act
            var result = await logger.LogAsync("TestInfo", "Test message", new { test1 = "test1", test2 = "test2" });

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Log_Should_Return_False_When_Splunk_Returned_Error()
        {
            // Arrange
            var logger = new Logger(new Uri(_splunkCollectorInfo.SplunkCollectorUrl).Add("test"), _splunkCollectorInfo.AuthenticationToken, "TestName", "TestVersion", 3000);

            // Act
            var result = await logger.LogAsync("TestInfo", "Test message", new { test1 = "test1", test2 = "test2" });

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Log_With_CorrelationId_Should_Return_True_When_Log_To_Splunk_Successful()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var logger = new Logger(new Uri(_splunkCollectorInfo.SplunkCollectorUrl), _splunkCollectorInfo.AuthenticationToken, "TestName", "TestVersion", 3000);

            // Act
            var result = await logger.LogAsync("TestInfo", "Test message", new { test1 = "test1", test2 = "test2" }, correlationId: guid.ToString());

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Log_With_CorrelationId_Should_Return_False_When_Collector_Url_Does_Not_Exist()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var logger = new Logger(new Uri("http://wrong-url/"), _splunkCollectorInfo.AuthenticationToken, "TestName", "TestVersion", 1000);

            // Act
            var result = await logger.LogAsync("TestInfo", "Test message", new { test1 = "test1", test2 = "test2" }, correlationId: correlationId.ToString());

            // Assert
            Assert.IsFalse(result);
        }
    }
}
