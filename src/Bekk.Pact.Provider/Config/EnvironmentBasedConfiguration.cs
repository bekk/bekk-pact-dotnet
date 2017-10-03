using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Exceptions;

namespace Bekk.Pact.Provider.Config
{
    public class EnvironmentBasedConfiguration : Bekk.Pact.Common.Config.EnvironmentBasedConfigurationBase, IProviderConfiguration
    {
        public StringComparison? BodyKeyStringComparison 
        {
            get
            {
                var result = GetValue("Consumer:" + nameof(BodyKeyStringComparison));
                if(result == null) return null;
                if(Enum.TryParse<StringComparison>(result, out var comparison)) return comparison;
                throw new ConfigurationException($"Couldn't parse configurationVariable {Prefix}:Consumer:{nameof(BodyKeyStringComparison)} value {result} to a valid string comparison.", this);
            }
        }
    }
}