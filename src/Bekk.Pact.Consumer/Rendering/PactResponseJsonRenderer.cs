using Bekk.Pact.Consumer.Contracts;
using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Consumer.Rendering
{
    class PactResponseJsonRenderer : PactBaseJsonRenderer
    {
        private readonly IPactResponseDefinition _pact;

        public PactResponseJsonRenderer(IPactResponseDefinition pact)
        {
            _pact = pact;
        }

        public override JObject Render()
        {
            dynamic json = new JObject();
            json.status = _pact.ResponseStatusCode;
            json.headers = RenderHeaders(_pact.ResponseHeaders);
            json.body = new BodyRenderer(_pact).Render();
            return json;
        }
    }
}