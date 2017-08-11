using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Matching;

namespace Bekk.Pact.Consumer.Server
{
    class PactHandler : IPactResponder, IVerifyAndClosable
    {
        private readonly PactComparer matcher;
        private readonly IConsumerConfiguration config;
        private readonly IPactDefinition pact;
        private readonly Action<bool> unregister;
        private int matches;

        public PactHandler(
            IPactDefinition pact, 
            IConsumerConfiguration config,
            Action<PactHandler, bool> unregister)
        {
            this.matcher = new PactComparer(pact);
            this.pact = pact;
            this.config = config;
            this.unregister = (isValid) => unregister(this, isValid);
        }

        public IPactResponseDefinition Respond(IPactRequestDefinition request)
        {
            if(matcher.Matches(request))
            {
                config.LogSafe($"Request received at {request.RequestPath} matching expectation.");
                matches++;
                return pact;
            }
            return null;
        }
        public int VerifyAndClose(int expectedMatches = 1)
        {
            unregister(matches == expectedMatches);
            return matches;
        }
    }
}