using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Exceptions;

namespace Bekk.Pact.Common.Config
{
    public abstract class EnvironmentBasedConfigurationBase : IConfiguration
    {
        protected string Prefix { get; } = "Bekk:Pact";
        public Uri BrokerUri => GetUriValue(nameof(BrokerUri));

        public string BrokerUserName => GetValue(nameof(BrokerUserName));

        public string BrokerPassword => GetValue(nameof(BrokerPassword));

        public string PublishPath => GetValue(nameof(PublishPath));

        public Action<string> Log => null;

        public LogLevel? LogLevel 
        {
            get
            {
                var result = GetValue(nameof(LogLevel));
                if(result == null) return null;
                if(Enum.TryParse<LogLevel>(result, out var level)) return level;
                throw new ConfigurationException($"Couldn't parse configurationVariable {Prefix}:{nameof(LogLevel)} value {result} to a valid loglevel.", this);
            }
        }
        public string LogFile => GetValue(nameof(LogFile));
        protected string GetValue(string key)=> Environment.GetEnvironmentVariable(string.Concat(Prefix, ":", key));
        protected Uri GetUriValue(string key)
        {
            var value = GetValue(key);
            if(value == null) return null;
            try
            {
                return new Uri(value);
            }
            catch(Exception e)
            {
                throw new ConfigurationException($"Couldn't parse configurationVariable {Prefix}:{key} value {value} to an Uri.", this, e);
            }
        }

    }
}