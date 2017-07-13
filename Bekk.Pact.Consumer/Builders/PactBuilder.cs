﻿using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Consumer.Config;
using Bekk.Pact.Consumer.Contracts;

namespace Bekk.Pact.Consumer.Builders
{
    public class PactBuilder : IPactBuilder, IConsumerBuilder
    {
        private readonly string description;
        private string consumer;
        private string provider;
        private IConsumerConfiguration configuration;

        public PactBuilder(string description, IConsumerConfiguration config)
        {
            this.description = description;
            configuration = config ?? new Configuration();
        }

        public static IPactBuilder Build(string description) => new PactBuilder(description,null);

        public IPactBuilder With(IConsumerConfiguration config)
        {
            configuration = config;
            return this;
        }

        public IConsumerBuilder Between(string provider)
        {
            this.provider = provider;
            return this;
        }

        public IProviderStateBuilder Given(string state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            return new InteractionBuilder(state, provider, consumer, description, configuration);
        }

        public IProviderStateBuilder WithProviderState(string state) => Given(state);

        IPactBuilder IConsumerBuilder.And(string consumer)
        {
            this.consumer = consumer;
            return this;
        }
    }
}