using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Common.Exceptions
{
    [Serializable]
    public class ConfigurationException : PactException
    {
        public IConfiguration Configuration { get; set; }
        public ConfigurationException(string message, IConfiguration configuration) : base(message)
        {
            Configuration = configuration;
        }

        public ConfigurationException(string message, IConfiguration configuration, Exception innerException) : base(message, innerException)
        {
            Configuration = configuration;
        }
    }
}