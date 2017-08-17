using System.Collections.Generic;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Contracts
{
    interface IPactDefinition : IPactPathMetadata
    {
        IEnumerable<IPactInteractionDefinition> Interactions { get; }
    }
}