using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Config
{
    class MergedConfiguration : IConsumerConfiguration
    {
        private readonly IEnumerable<IConsumerConfiguration> _configurations;

        public MergedConfiguration(params IConsumerConfiguration[] configurations)
        {
            _configurations = configurations.Where(c => c!=null).Reverse().ToList();
        }

        private T GetValue<T>(Func<IConsumerConfiguration,T> property) => _configurations.Select(property).FirstOrDefault(v => !object.Equals(v, default(T)));

        public Uri MockServiceBaseUri => GetValue(c => c.MockServiceBaseUri);

        public Uri BrokerUri => GetValue(c => c.BrokerUri);

        public Action<string> Log => GetValue(c => c.Log);
    }

}