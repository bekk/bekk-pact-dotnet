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
        /// The user name for the broker service
        /// </summary>
        string BrokerUserName { get; }
        /// <summary>
        /// The password for the broker service
        /// </summary>
        string BrokerPassword { get; }
        /// <summary>
        /// The file path to store published pacts
        /// </summary>
        string PublishPath { get; }
        /// <summary>
        /// A delegate for outputting log informastion.
        /// </summary>
        Action<string> Log { get; }
        /// <summary>
        /// A filter for throttling the log output
        /// </summary>
        LogLevel LogLevel { get; }
        /// <summary>
        /// Location of a log file to append log messages to
        /// </summary>
        string LogFile { get; }
    }
}
