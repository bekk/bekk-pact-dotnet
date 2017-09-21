using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Contracts
{
    interface IPactResponseDefinition
    {
        IHeaderCollection ResponseHeaders { get; }
        int? ResponseStatusCode { get; }
        object ResponseBody { get; }
    }
}