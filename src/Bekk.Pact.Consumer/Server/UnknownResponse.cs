using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Utils;
using Bekk.Pact.Consumer.Contracts;
using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Consumer.Server
{
    class UnknownResponse : IPactResponseDefinition
    {
        private readonly object _request;
        private readonly IHeaderCollection _headers;
        private UnknownResponse(IHeaderCollection headers=null)
        {
            _headers = headers??new HeaderCollection().Add("Content-Type", "application/json; charset=utf-8");
            _headers.Add("Warning", "Request was not recognized as one of the registered pacts.");
        }
        public UnknownResponse(IPactRequestDefinition request): this()
        {
            _request = request;
        }
        public  UnknownResponse(params JObject[] contents): this()
        {
            switch(contents.Length)
            {
                case 1: _request = contents[0]; break;
                case 2: _request = new JArray(contents); break;
            }
        }
        public IHeaderCollection ResponseHeaders => _headers;

        public int? ResponseStatusCode => 501;

        public object ResponseBody => _request;

        IJsonable IPactResponseDefinition.ResponseBody => null;
    }
}