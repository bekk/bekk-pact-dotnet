using System;
using Bekk.Pact.Consumer.Config;
using Bekk.Pact.Common.Contracts;
using Xunit;

namespace Bekk.Pact.Consumer.Tests.Config
{
    [Collection("Configuration tests")]
    public class EnvironmentVariableConfigTests : IDisposable
    {
        public void Dispose()
        {
            Environment.SetEnvironmentVariable("Bekk:Pact:BrokerPassword", null);
            Environment.SetEnvironmentVariable("Bekk:Pact:LogLevel", null);
            Environment.SetEnvironmentVariable("Bekk:Pact:Consumer:MockServiceBaseUri", null);
            Environment.SetEnvironmentVariable("Bekk:Pact:PublishPath", null);
            Environment.SetEnvironmentVariable("Bekk:Pact:LogFile", null);
            Environment.SetEnvironmentVariable("Bekk:Pact:LogLevel", null);
        }

        [Fact]
        public void FromEnvironmentVariables_ReturnsObjectReadFromEnvironment()
        {
            Environment.SetEnvironmentVariable("Bekk:Pact:BrokerPassword", "test0");
            Environment.SetEnvironmentVariable("Bekk:Pact:LogLevel", "Error");
            Environment.SetEnvironmentVariable("Bekk:Pact:Consumer:MockServiceBaseUri", "http://localhost:12");
            var target = Configuration.FromEnvironmentVartiables();

            Assert.Equal("test0", target.BrokerPassword);
            Assert.Equal(new Uri("http://localhost:12"), target.MockServiceBaseUri);
            Assert.Equal(LogLevel.Error, target.LogLevel);
        }

        [Fact]
        public void With_SettingValuesInCode_EnvironmentVariablesOverrides()
        {
            Environment.SetEnvironmentVariable("Bekk:Pact:PublishPath", "expectedPath");
            Environment.SetEnvironmentVariable("Bekk:Pact:LogFile", "expectedLogFile");
            Environment.SetEnvironmentVariable("Bekk:Pact:LogLevel", "Info");
            Environment.SetEnvironmentVariable("Bekk:Pact:Consumer:MockServiceBaseUri", "http://localhost:42");

            IConsumerConfiguration target = Configuration.With
                .PublishPath("Not expected")
                .LogFile("Not expected")
                .LogLevel(LogLevel.Verbose);

            Assert.Equal("expectedPath", target.PublishPath);
            Assert.Equal("expectedLogFile", target.LogFile);
            Assert.Equal(LogLevel.Info, target.LogLevel);
            Assert.Equal("http://localhost:42/", target.MockServiceBaseUri.ToString());
        }
    }
}