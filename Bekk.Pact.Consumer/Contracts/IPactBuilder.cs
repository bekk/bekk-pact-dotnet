using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Contracts
{
    public interface IPactBuilder
    {
        /// <summary>
        /// Call this method to define a provider state and start defining the request.
        /// </summary>
        /// <param name="state">The text recognized in the provider test to set up the test state (data).</param>
        /// <returns>A builder to define the request to the provider.</returns>
        /// <seealso cref="WithProviderState"/>
        IRequestPathBuilder Given(string state);
        /// <summary>
        /// Synonymous to <seealso cref="Given"/>.
        /// </summary>
        IRequestPathBuilder WithProviderState(string state);
        /// <summary>
        /// Provide a configuration with this method. The configuration will override configuration in the <seealso cref="Context"/>.
        /// </summary>
        /// <param name="config">A configuration object. You may use <seealso cref="Bekk.Pact.Consumer.Config.Configuration"/> for this.</param>
        IPactBuilder With(IConsumerConfiguration config);
        /// <summary>
        /// Provide a version for the pact. This will override the version provided in the <seealso cref="Context"/>.
        /// </summary>
        IPactBuilder With(Version version);
        /// <summary>
        /// Provide a version for the pact. This will override the version provided in the <seealso cref="Context"/>.
        /// <param name="version">Provide a valid parsable version (i.e.<c>1.0.0.0</c>)</param>
        /// </summary>
        IPactBuilder WithVersion(string version);
        /// <summary>
        /// The consumer of the pact. (The client calling a service.)
        /// </summary>
        /// <param name="name">The name used to recognize this client.</param>
        /// <seealso cref="Between"/>
        IPactBuilder ForConsumer(string name);
        /// <summary>
        /// The provider of the pact. (The service being called.)
        /// This value can also be set in the <seealso cref="Context"/>.
        /// </summary>
        /// <param name="name">The name used by the service to fetch and recognize pacts.</param>
        /// <seealso cref="IConsumerBuilder.And"/>
        IPactBuilder ForProvider(string name);
                /// <summary>
        /// The provider of the pact. (The service being called.)
        /// </summary>
        /// <param name="provider">The name used by the service to fetch and recognize pacts.</param>
        /// <seealso cref="ForProvider"/>
        IConsumerBuilder Between(string provider);
    }
}