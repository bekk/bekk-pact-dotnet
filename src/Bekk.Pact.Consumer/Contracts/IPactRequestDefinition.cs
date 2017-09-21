using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Contracts
{
    interface IPactRequestDefinition
    {
        string RequestPath { get; }
        string Query { get; }
        IHeaderCollection RequestHeaders { get; }
        string HttpVerb { get; }
    }
}