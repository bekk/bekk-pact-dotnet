using System;

namespace Bekk.Pact.Common.Contracts
{
    public interface IConfiguration
    {
        /// <summary>
        /// The address to the pact broker service. This is used to publish the pacts.
        /// </summary>
        Uri BrokerUri { get; }
        /// <summary>
        /// The file path to store published pacts
        /// </summary>
        string PublishPath { get; }
        /// <summary>
        /// A delegate for outputting log informastion.
        /// Default is output to the console.
        /// </summary>
        Action<string> Log { get; }
    }
}
