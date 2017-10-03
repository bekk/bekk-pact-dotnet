using System;

namespace Bekk.Pact.Common.Contracts
{
    /// <summary>
    /// Configurations used to run the provider tests
    /// </summary>
    public interface IProviderConfiguration : IConfiguration
    {
        /// <summary>
        /// The comparison type used when matching property names in the message body.
        /// </summary>
        /// <remarks>Default in the configuration builder is <seeAlso cref="StringComparison.CurrentCultureIgnoreCase"/>.</remarks>
        StringComparison? BodyKeyStringComparison { get; }
    }
}
