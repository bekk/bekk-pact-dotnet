using System;
using System.Linq;
using Bekk.Pact.Consumer.Contracts;
using Newtonsoft.Json.Linq;

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
            if (CompareAcceptEmptyAsNull(template.RequestPath, request.RequestPath)) return false;
            if (CompareAcceptEmptyAsNull(template.Query, request.Query)) return false;
            foreach (var header in template.RequestHeaders)
            {
                if (!request.RequestHeaders[header.Key].Equals(header.Value)) return false;
            }
            return true;
        }
        public JObject DiffGram(IPactRequestDefinition request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            dynamic diff = new JObject();
            if (template.HttpVerb != request.HttpVerb) diff.Add("HttpVerb", GetDiff(template.HttpVerb, request.HttpVerb));
            if (!CompareAcceptEmptyAsNull(template.RequestPath, request.RequestPath)) diff.Add("Path", GetDiff(template.RequestPath, request.RequestPath));
            if (!CompareAcceptEmptyAsNull(template.Query, request.Query)) diff.Add("Query", GetDiff(template.Query, request.Query));
            var headers = template.RequestHeaders.Where(expected => !request.RequestHeaders[expected.Key].Equals(expected.Value)).ToList();
            if(headers.Any()){
                dynamic headersDiff = new JObject();
                foreach(var header in headers)
                {
                    headersDiff.Add(header.Key, GetDiff(header.Value, request.RequestHeaders[header.Key]));
                }
                diff.Add("headers", headers);
            }
            return diff;
        }

        private JObject GetDiff(string expected, string actual)
        {
            dynamic diff = new JObject();
            diff.Add("expected", expected);
            diff.Add("actual", actual);
            return diff;
        }

        private bool CompareAcceptEmptyAsNull(string left, string right)
        {
            if(string.IsNullOrEmpty(left)) return string.IsNullOrEmpty(right);
            return left.Equals(right);
        }
    }
}