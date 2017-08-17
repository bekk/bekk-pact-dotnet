using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Contracts
{
    public interface IPactBuilder
    {
        IProviderStateBuilder Given(string state);
        IConsumerBuilder Between(string provider);
        IPactBuilder With(IConsumerConfiguration config);
        IPactBuilder With(Version version);
        IPactBuilder WithVersion(string version);
        IPactBuilder ForConsumer(string consumerName);
        IPactBuilder ForProvider(string providerName);
    }
}