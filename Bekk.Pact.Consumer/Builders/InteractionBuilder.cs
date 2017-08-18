using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Utils;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Rendering;
using Bekk.Pact.Consumer.Server;
using IPact = Bekk.Pact.Consumer.Contracts.IPact;

namespace Bekk.Pact.Consumer.Builders
{
    class InteractionBuilder : IRequestPathBuilder, IRequestBuilder, IResponseBuilder, IPact, IPactInteractionDefinition, IPactDefinition
    {
        private readonly IConsumerConfiguration configuration;
        private readonly List<string> queries = new List<string>();
        private IVerifyAndClosable handler;
        public string State { get; }
        public Version Version { get; }
        public string Description { get; }
        public string Provider { get; }
        public string Consumer { get; }
        public string RequestPath { get; private set; }
        public string Query => queries.Any() ? $"?{string.Join("&", queries)}" : null;
        public IHeaderCollection RequestHeaders { get; } = new HeaderCollection();
        public IHeaderCollection ResponseHeaders { get; } = new HeaderCollection();
        public string HttpVerb { get; private set; } = "GET";
        public int? ResponseStatusCode { get; private set; }
        public object ResponseBody { get; private set; }

        public InteractionBuilder(string state, string consumer, string provider, string description, Version version, IConsumerConfiguration config)
        {
            if(string.IsNullOrWhiteSpace(consumer)) throw new ArgumentException("Please provide a consumer name", nameof(consumer));
            if(string.IsNullOrWhiteSpace(provider)) throw new ArgumentException("Please provide a provider name", nameof(provider));
            this.configuration = config;
            State = state;
            Consumer = consumer;
            Provider = provider;
            Description = description;
            Version = version ?? new Version(1,0);
        }

        IRequestBuilder IRequestPathBuilder.WhenRequesting(string path)
        {
            RequestPath = path;
            return this;
        }

        IRequestBuilder IRequestBuilder.WithQuery(string key, string value)
        {
            queries.Add($"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(value)}");
            return this;
        }

        IRequestBuilder IRequestBuilder.WithVerb(HttpMethod verb) => ((IRequestBuilder) this).WithVerb(verb.ToString());

        IRequestBuilder IRequestBuilder.WithVerb(string verb)
        {
            HttpVerb = verb;
            return this;
        }

        IResponseBuilder IRequestBuilder.ThenRespondsWith(int statusCode)
        {
            ResponseStatusCode = statusCode;
            return this;
        }

        IResponseBuilder IRequestBuilder.ThenRespondsWith(HttpStatusCode statusCode) => ((IRequestBuilder) this).ThenRespondsWith((int) statusCode);

        IResponseBuilder IResponseBuilder.WithHeader(string key, params string[] values)
        {
            ResponseHeaders.Add(key, values);
            return this;
        }

        IResponseBuilder IResponseBuilder.WithBody(object body)
        {
            ResponseBody = body;
            return this;
        }

        async Task<IPact> IResponseBuilder.InPact()
        {
            handler = await Context.RegisterListener(this, configuration);
            return this;
        }

        void IPact.Verify()
        {
            var matches = handler.VerifyAndClose(1);
            if(matches != 1) throw new Exception($"The pact was matched {matches} times. Expected one.");
        }

        private async Task Save()
        {
            using(var repo = new PactRepo(configuration))
            {
                await repo.Put(this);
            }
        }

        async Task IPact.VerifyAndSave()
        {
            var pact = (IPact)this;
            pact.Verify();
            await Save();
        }

        IRequestBuilder IRequestBuilder.WithHeader(string key, params string[] values)
        {
            RequestHeaders.Add(key, values);
            return this;
        }

        IEnumerable<IPactInteractionDefinition> IPactDefinition.Interactions => new []{this};
        public void Dispose()
        {
            handler.VerifyAndClose(1);
        }

        public override string ToString()
        {
            return new PactJsonRenderer(this).ToString();
        }
    }
}