using System;

namespace Bekk.Pact.Common.Contracts
{
    public interface IConfiguration
    {
        Uri BrokerUri { get; }
        Action<string> Log { get; }
        StringComparison BodyKeyStringComparison { get; }
    }
}
