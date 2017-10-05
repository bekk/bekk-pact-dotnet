using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Config
{
    public class EnvironmentBasedConfiguration : Bekk.Pact.Common.Config.EnvironmentBasedConfigurationBase, IConsumerConfiguration
    {
        public Uri MockServiceBaseUri => GetUriValue("Consumer", nameof(MockServiceBaseUri));
    }
}