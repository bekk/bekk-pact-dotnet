using System;
using System.Runtime.CompilerServices;
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
            if(string.IsNullOrWhiteSpace(description))throw new ArgumentException("Please provide a description", nameof(description));
            this._description = description;
            _configuration = MergedConfiguration.MergeConfigs(Context.Configuration, config);
        }

        /// <summary>
        /// Call this method to create a pact interaction instance.
        /// </summary>
        /// <remarks>
        /// This should be complemented with an instance of <seealso cref="Context"/>, shared across all tests/pacts.
        /// <list type="bullet">
        /// <listheader><description>Please provide the builder with:</description></listheader>
        /// <item>Consumer: Either with <see cref="ForConsumer"/>, <see cref="IConsumerBuilder.And"/> or <seealso cref="Context.ForConsumer"/>.</item>
        /// <item>Provider: Either with <see cref="ForProvider"/> or <see cref="Between"/>.</item>
        /// <item>Version (optional): Either with <see cref="WithVersion"/>, <see cref="With(Version)"/> or in <seealso cref="Context"/>.</item>
        /// <item>Configuration: Either with <see cref="With(IConsumerConfiguration)"/> or in <seealso cref="Context"/>.</item> 
        /// </list>
        /// Provide provider state by calling <see cref="WithProviderState"/> or <see cref="Given"/>.
        /// </remarks>
        /// <param name="description">
        /// The description of the interaction. 
        /// If <c>null</c>, the value of <paramref name="methodName"/> (the name of the calling method) is used.
        /// </param>
        /// <param name="methodName">This value is set automatically by compiler services.</param>
        /// <returns>A pact builder instance.</returns>
        public static IPactBuilder Build(string description = null, [CallerMemberName]string methodName = "") => new PactBuilder(description ?? methodName, null);
        /// <summary>
        /// Provide a configuration with this method. The configuration will override configuration in the <seealso cref="Context"/>.
        /// </summary>
        /// <param name="config">A configuration object. You may use <seealso cref="Bekk.Pact.Consumer.Config.Configuration"/> for this.</param>
        public IPactBuilder With(IConsumerConfiguration config)
        {
            _configuration = MergedConfiguration.MergeConfigs(_configuration, config);
            return this;
        }
        /// <summary>
        /// Provide a version for the pact. This will override the version provided in the <seealso cref="Context"/>.
        /// </summary>
        public IPactBuilder With(Version version)
        {
            this._version = version;
            return this;
        }
        /// <summary>
        /// Provide a version for the pact. This will override the version provided in the <seealso cref="Context"/>.
        /// <param name="version">Provide a valid parsable version (i.e.<c>1.0.0.0</c>)</param>
        /// </summary>
        public IPactBuilder WithVersion(string version) => With(Version.Parse(version));
        /// <summary>
        /// The consumer of the pact. (The client calling a service.)
        /// </summary>
        /// <param name="name">The name used to recognize this client.</param>
        /// <seealso cref="Between"/>
        public IPactBuilder ForConsumer(string name)
        {
            _consumer = name;
            return this;
        }
        /// <summary>
        /// The provider of the pact. (The service being called.)
        /// This value can also be set in the <seealso cref="Context"/>.
        /// </summary>
        /// <param name="name">The name used by the service to fetch and recognize pacts.</param>
        /// <seealso cref="IConsumerBuilder.And"/>
        public IPactBuilder ForProvider(string name)
        {
            _provider = name;
            return this;
        }

        /// <summary>
        /// The provider of the pact. (The service being called.)
        /// </summary>
        /// <param name="provider">The name used by the service to fetch and recognize pacts.</param>
        /// <seealso cref="ForProvider"/>
        public IConsumerBuilder Between(string provider)
        {
            this._provider = provider;
            return this;
        }
        /// <summary>
        /// Call this method to define a provider state and start defining the request.
        /// </summary>
        /// <param name="state">The text recognized in the provider test to set up the test state (data).</param>
        /// <returns>A builder to define the request to the provider.</returns>
        /// <seealso cref="WithProviderState"/>
        public IRequestPathBuilder Given(string state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            return new InteractionBuilder(state, _consumer ?? Context.ConsumerName, _provider, _description, _version ?? Context.Version, _configuration ?? new Configuration());
        }
        /// <summary>
        /// Call this method to define a provider state and start defining the request.
        /// </summary>
        /// <param name="state">The text recognized in the provider test to set up the test state (data).</param>
        /// <returns>A builder to define the request to the provider.</returns>
        /// <seealso cref="Given"/>
        public IRequestPathBuilder WithProviderState(string state) => Given(state);
        /// <param name="consumer">The consumer of the pact</param>
        IPactBuilder IConsumerBuilder.And(string consumer)
        {
            this._consumer = consumer;
            return this;
        }
    }
}
