using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Utils;
using Bekk.Pact.Consumer.Contracts;

namespace Bekk.Pact.Consumer.Server
{
    class UnknownResponse : IPactResponseDefinition
    {
        private readonly IPactRequestDefinition _request;
        private readonly IHeaderCollection _headers;

        public UnknownResponse(IPactRequestDefinition request)
        {
            _request = request;
            _headers = new HeaderCollection().Add("Content-Type", "application/json; charset=utf-8");
        }
        public IHeaderCollection ResponseHeaders => _headers;

        public int? ResponseStatusCode => 501;

        public object ResponseBody => _request;
    }
}