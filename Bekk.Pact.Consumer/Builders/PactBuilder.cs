using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Consumer.Config;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Server;

namespace Bekk.Pact.Consumer.Builders
{
    public class PactBuilder : IPactBuilder, IConsumerBuilder
    {
        private readonly string description;
        private string consumer;
        private string provider;
        private Version version;
        private IConsumerConfiguration configuration;

        public PactBuilder(string description, IConsumerConfiguration config)
        {
            this.description = description;
            configuration = MergeConfigs(Context.Configuration, config);
        }

        private IConsumerConfiguration MergeConfigs(IConsumerConfiguration left, IConsumerConfiguration right)
        {
            if(left == null)
            {
                return right;
            }
            else
            {
                return right == null ? left : new MergedConfiguration(left, right);
            }
        }

        public static IPactBuilder Build(string description) => new PactBuilder(description,null);

        public IPactBuilder With(IConsumerConfiguration config)
        {
            configuration = MergeConfigs(configuration, config);
            return this;
        }
        public IPactBuilder With(Version version)
        {
            this.version = version;
            return this;
        }
        public IPactBuilder WithVersion(string version) => With(Version.Parse(version));

        /// <param name="provider">The provider of the pact</param>
        public IConsumerBuilder Between(string provider)
        {
            this.provider = provider;
            return this;
        }

        public IProviderStateBuilder Given(string state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            return new InteractionBuilder(state, consumer, provider, description, version ?? Context.Version, configuration ?? new Configuration());
        }

        public IProviderStateBuilder WithProviderState(string state) => Given(state);
        /// <param name="consumer">The consumer of the pact</param>
        IPactBuilder IConsumerBuilder.And(string consumer)
        {
            this.consumer = consumer;
            return this;
        }
    }
}
