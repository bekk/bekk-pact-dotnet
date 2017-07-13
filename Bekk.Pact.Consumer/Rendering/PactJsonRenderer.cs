using System;
using Bekk.Pact.Consumer.Contracts;
using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Consumer.Rendering
{
    class PactJsonRenderer:PactBaseJsonRenderer
    {
        private readonly IPactDefinition pact;
        private readonly PactBaseJsonRenderer requestRenderer;

        public PactJsonRenderer(IPactDefinition pact)
        {
            if (pact == null) throw new ArgumentNullException(nameof(pact));
            this.pact = pact;
            requestRenderer = new PactRequestJsonRenderer(pact);
        }
        
        public JObject RenderResponse(IPactResponseDefinition response)
        {
            dynamic json = new JObject();
            json.status = response.ResponseStatusCode;
            json.headers = RenderHeaders(response.ResponseHeaders);
            json.body = new BodyRenderer(response).Render();
            return json;
        }

        private JObject RenderInteraction()
        {
            dynamic json = new JObject();
            if (pact.Description != null) json.description = pact.Description;
            if (pact.State != null) json.provider_state = pact.State;
            json.request = requestRenderer.Render();
            json.response = RenderResponse(pact);
            return json;
        }

        public override JObject Render()
        {
            dynamic json = new JObject();
            json.Add("provider", pact.Provider);
            json.Add("consumer", pact.Consumer);
            json.interactions = new JArray(RenderInteraction());
            json.Add("metadata", new JObject(new JProperty("pactSpecificationVersion", "1.0.0")));
            json.createdAt = DateTime.Now;
            return json;
        }
    }
}