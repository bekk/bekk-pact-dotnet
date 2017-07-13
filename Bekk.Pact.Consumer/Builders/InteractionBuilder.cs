using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Utils;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Rendering;
using Bekk.Pact.Consumer.Server;
using IPact = Bekk.Pact.Consumer.Contracts.IPact;

namespace Bekk.Pact.Consumer.Builders
{
    class InteractionBuilder : IProviderStateBuilder, IRequestBuilder, IResponseBuilder, IPact, IPactDefinition
    {
        private readonly IConsumerConfiguration configuration;
        private readonly List<string> queries = new List<string>();
        private WebServer server;
        public string State { get; }
        public string Description { get; }
        public string Provider { get; private set; }
        public string Consumer { get; private set; }
        public string RequestPath { get; private set; }
        public string Query => queries.Any() ? $"?{string.Join("&", queries)}" : null;
        public IHeaderCollection RequestHeaders { get; } = new HeaderCollection();
        public IHeaderCollection ResponseHeaders { get; } = new HeaderCollection();
        public string HttpVerb { get; private set; } = "GET";
        public int? ResponseStatusCode { get; private set; }
        public object ResponseBody { get; private set; }

        public InteractionBuilder(string state, string consumer, string provider, string description, IConsumerConfiguration config)
        {
            this.configuration = config;
            State = state;
            Consumer = consumer;
            Provider = provider;
            Description = description;
        }

        IProviderStateBuilder IProviderStateBuilder.WithConsumer(string consumer)
        {
            Consumer = consumer;
            return this;
        }

        IRequestBuilder IProviderStateBuilder.WhenRequesting(string path)
        {
            RequestPath = path;
            return this;
        }

        IRequestBuilder IRequestBuilder.FromProvider(string provider)
        {
            Provider = provider;
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

        IResponseBuilder IResponseBuilder.WithStatus(HttpStatusCode statusCode) => ((IResponseBuilder) this).WithStatus((int) statusCode);

        IResponseBuilder IResponseBuilder.WithStatus(int statusCode)
        {
            ResponseStatusCode = statusCode;
            return this;
        }

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

        IPact IResponseBuilder.InPact()
        {
            server = WebServer.Listen(this, configuration);
            return this;
        }

        void IPact.Verify()
        {
            if(server.Matches < 1) throw new Exception("The pact has not been matched.");
        }

        IRequestBuilder IRequestBuilder.WithHeader(string key, params string[] values)
        {
            RequestHeaders.Add(key, values);
            return this;
        }

        IResponseBuilder IRequestBuilder.ThenResponds()
        {
            return this;
        }

        public void Dispose()
        {
            server?.Dispose();
        }

        public override string ToString()
        {
            return new PactJsonRenderer(this).ToString();
        }
    }
}