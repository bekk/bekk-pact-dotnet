using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Consumer.Config;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Server;

namespace Bekk.Pact.Consumer.Builders
{
    public class PactBuilder : IPactBuilder, IConsumerBuilder
    {
        private readonly string _description;
        private string _consumer;
        private string _provider;
        private Version _version;
        private IConsumerConfiguration _configuration;

        private PactBuilder(string description, IConsumerConfiguration config)
        {
            this._description = description;
            _configuration = MergeConfigs(Context.Configuration, config);
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
            _configuration = MergeConfigs(_configuration, config);
            return this;
        }
        public IPactBuilder With(Version version)
        {
            this._version = version;
            return this;
        }
        public IPactBuilder WithVersion(string version) => With(Version.Parse(version));

        public IPactBuilder ForConsumer(string name)
        {
            _consumer = name;
            return this;
        }
        public IPactBuilder ForProvider(string name)
        {
            _provider = name;
            return this;
        }

        /// <param name="provider">The provider of the pact</param>
        public IConsumerBuilder Between(string provider)
        {
            this._provider = provider;
            return this;
        }

        public IProviderStateBuilder Given(string state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            return new InteractionBuilder(state, _consumer ?? Context.ConsumerName, _provider, _description, _version ?? Context.Version, _configuration ?? new Configuration());
        }

        public IProviderStateBuilder WithProviderState(string state) => Given(state);
        /// <param name="consumer">The consumer of the pact</param>
        IPactBuilder IConsumerBuilder.And(string consumer)
        {
            this._consumer = consumer;
            return this;
        }
    }
}
