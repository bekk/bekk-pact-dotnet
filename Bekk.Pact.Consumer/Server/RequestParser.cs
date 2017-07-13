using System;
using System.Collections.Generic;
using System.Linq;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Utils;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Rendering;

namespace Bekk.Pact.Consumer.Server
{
    class RequestParser : IPactRequestDefinition
    {
        public RequestParser(string req, Uri baseUri)
        {
            RequestHeaders = new HeaderCollection();
            Parse(req, baseUri);
        }

        private void Parse(string req, Uri baseUri)
        {
            var lines = new Queue<string>(req.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries));
            var startLine = lines.Dequeue().Split(' ').ToArray();
            HttpVerb = startLine[0];
            var uri = new Uri(baseUri, startLine[1]);
            RequestPath = uri.LocalPath;
            Query = uri.Query;
            string hdr;
            while(lines.Count > 0 &&(hdr = lines.Dequeue()) != "\r\n" && hdr != null)
            {
                RequestHeaders.ParseAndAdd(hdr);
            }
        }

        public string RequestPath { get; private set; }
        public string Query { get; private set; }
        public IHeaderCollection RequestHeaders { get; }
        public string HttpVerb { get; private set; }

        public override string ToString()
        {
            return new PactRequestJsonRenderer(this).ToString();
        }
    }
}