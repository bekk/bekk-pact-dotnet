using System;
using System.Linq;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Exceptions;

namespace Bekk.Pact.Common.Config
{
    public abstract class EnvironmentBasedConfigurationBase : IConfiguration
    {
        private string[] separators = new[]{":", "__"};
        protected string Prefix { get; } = "Bekk:Pact";
        public Uri BrokerUri => GetUriValue(null, nameof(BrokerUri));

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
                throw new ConfigurationException($"Couldn't parse configurationVariable {Prefix}:{nameof(LogLevel)} value {result} to a valid loglevel. Use either of {ListOfEnums<LogLevel>()}.", this);
            }
        }
        public string LogFile => GetValue(nameof(LogFile));
        protected string GetValue(string prefix, string key)=> separators.Select(sep => $"{(prefix == null ? Prefix : string.Concat(Prefix, sep, prefix)).Replace(":", sep)}{sep}{key}").Select(Environment.GetEnvironmentVariable).FirstOrDefault(v => v != null);
        private string GetValue(string key) => GetValue(null, key);
        protected Uri GetUriValue(string prefix, string key)
        {
            var value = GetValue(prefix, key);
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
        protected string ListOfEnums<T>() => string.Join(", ", Enum.GetValues(typeof(T)).Cast<object>().Select(e => e.ToString()));
    }
}