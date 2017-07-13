using Bekk.Pact.Consumer.Contracts;
using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Consumer.Rendering
{
    class PactRequestJsonRenderer : PactBaseJsonRenderer
    {
        private readonly IPactRequestDefinition pact;

        public PactRequestJsonRenderer(IPactRequestDefinition pact)
        {
            this.pact = pact;
        }
        public override JObject Render()
        {
            dynamic json = new JObject();
            json.method = pact.HttpVerb;
            json.path = pact.RequestPath + pact.Query;
            json.headers = RenderHeaders(pact.RequestHeaders);
            return json;
        }
    }
}