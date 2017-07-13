using System;

namespace Bekk.Pact.Common.Contracts
{
    public interface IConsumerConfiguration
    {
        /// <summary>
        /// Base <c>Uri</c> for tcplistener.
        /// Defaults to http://127.0.0.1:1234
        /// </summary>
        Uri MockServiceBaseUri { get; }
    }
}
