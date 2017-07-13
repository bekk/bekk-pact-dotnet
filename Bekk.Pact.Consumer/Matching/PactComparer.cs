using System;
using Bekk.Pact.Consumer.Contracts;

namespace Bekk.Pact.Consumer.Matching
{
    class PactComparer
    {
        private readonly IPactDefinition template;

        public PactComparer(IPactDefinition template)
        {
            this.template = template ?? throw new ArgumentNullException(nameof(template));
        }
        public bool Matches(IPactRequestDefinition request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (template.HttpVerb != request.HttpVerb) return false;
            if (template.RequestPath != request.RequestPath) return false;
            if (template.Query != request.Query) return false;
            foreach (var header in template.RequestHeaders)
            {
                if (!request.RequestHeaders[header.Key].Equals(header.Value)) return false;
            }
            return true;
        }
    }
}