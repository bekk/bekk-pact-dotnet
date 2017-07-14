using System;

namespace Bekk.Pact.Common.Contracts
{
    public interface IPactPathMetadata
    {
        string Provider { get; }
        string Consumer { get; }
        Version Version { get; }
    }
}