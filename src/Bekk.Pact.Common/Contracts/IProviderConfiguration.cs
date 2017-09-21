using System;

namespace Bekk.Pact.Common.Contracts
{
    public interface IProviderConfiguration : IConfiguration
    {
        StringComparison BodyKeyStringComparison { get; }
    }
}
