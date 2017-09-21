﻿using System;
using System.Linq;
using Bekk.Pact.Consumer.Contracts;
using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Consumer.Rendering
{
    class PactJsonRenderer:PactBaseJsonRenderer
    {
        private readonly IPactDefinition pact;

        public PactJsonRenderer(IPactDefinition pact)
        {
            if (pact == null) throw new ArgumentNullException(nameof(pact));
            this.pact = pact;
        }

        private JObject RenderInteraction(IPactInteractionDefinition interaction)
        {
            dynamic json = new JObject();
            if (interaction.Description != null) json.description = interaction.Description;
            if (interaction.State != null) json.provider_state = interaction.State;
            json.request = new PactRequestJsonRenderer(interaction).Render();
            json.response = new PactResponseJsonRenderer(interaction).Render();
            return json;
        }

        private JObject RenderProviderConsumer(string name)
        {
            dynamic json = new JObject();
            json.Add("name", name);
            return json;
        }

        public override JObject Render()
        {
            dynamic json = new JObject();
            json.Add("provider", RenderProviderConsumer(pact.Provider));
            json.Add("consumer", RenderProviderConsumer(pact.Consumer));
            json.interactions = new JArray(pact.Interactions.Select(RenderInteraction));
            json.Add("metadata", new JObject(new JProperty("pactSpecificationVersion", "1.0.0")));
            return json;
        }
    }
}