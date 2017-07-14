using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Matching;
using Bekk.Pact.Common.Extensions;

namespace Bekk.Pact.Consumer.Server
{
    class WebServer : IDisposable
    {
        private readonly IPactDefinition pact;
        private Listener listener;
        private PactComparer matcher;
        private readonly IConsumerConfiguration configuration;
        private int matches = 0;

        private WebServer(IPactDefinition pact, IConsumerConfiguration config)
        {
            this.pact = pact ?? throw new ArgumentNullException(nameof(pact));
            configuration = config;
            SetUp(pact, config);
        }

        public int Matches => matches;

        public static WebServer Listen(IPactDefinition pact, IConsumerConfiguration config) => new WebServer(pact, config);

        private void SetUp(IPactDefinition pact, IConsumerConfiguration config)
        {
            matcher = new PactComparer(pact);
            listener = new Listener();
            listener.Start(config.MockServiceBaseUri, Respond);
        }

        private IPactResponseDefinition Respond(IPactRequestDefinition request)
        {
            if (matcher.Matches(request))
            {
                configuration.LogSafe($"Request received at {request.RequestPath} matching expectation.");
                matches++;
                return pact;
            }
            else
            {
                configuration.LogSafe($"Unrecognized request received at {request.RequestPath}. ");
                configuration.LogSafe(matcher.DiffGram(request).ToString());
            }
            return null;
        }

        public void Dispose()
        {
            listener?.Dispose();
        }
    }
}