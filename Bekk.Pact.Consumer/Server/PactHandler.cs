using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Matching;
using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Consumer.Server
{
    class PactHandler : IPactResponder, IVerifyAndClosable
    {
        private readonly PactComparer matcher;
        private readonly IConsumerConfiguration config;
        private readonly IPactInteractionDefinition pact;
        private readonly Action unregister;
        private int matches;

        public PactHandler(
            IPactInteractionDefinition pact, 
            IConsumerConfiguration config,
            Action<PactHandler> unregister)
        {
            this.matcher = new PactComparer(pact);
            this.pact = pact;
            this.config = config;
            this.unregister = () => unregister(this);
        }

        public JObject DiffGram(IPactRequestDefinition request) => matcher.DiffGram(request);

        public IPactResponseDefinition Respond(IPactRequestDefinition request)
        {
            if(matcher.Matches(request))
            {
                config.LogSafe(LogLevel.Info, $"Request received at {request.RequestPath} matching expectation.");
                matches++;
                return pact;
            }
            return null;
        }
        public int VerifyAndClose(int expectedMatches = 1)
        {
            unregister();
            return matches;
        }
    }
}