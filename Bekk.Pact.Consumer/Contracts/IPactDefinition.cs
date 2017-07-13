using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Contracts
{
    interface IPactDefinition : IPactRequestDefinition, IPactResponseDefinition, IPactPathMetadata
    {
        string State { get; }
        string Description { get; }
    }
}