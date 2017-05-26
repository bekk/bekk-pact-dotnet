using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Bekk.Pact.Provider.Model
{
    class Request
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public IDictionary<string, string> Headers { get; set; }

        public HttpRequestMessage BuildMessage()
        {
            var message = new HttpRequestMessage();
            message.Method = new HttpMethod(Method);
            foreach(var header in Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }
            message.RequestUri = new Uri(Path, UriKind.Relative);
            return message;
        }
    }
}