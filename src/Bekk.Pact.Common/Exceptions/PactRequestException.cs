using System.Net.Http;

namespace Bekk.Pact.Common.Exceptions
{
    public class PactRequestException : PactException
    {
        public PactRequestException(string message, HttpResponseMessage response)
         :base(message)
        {
            Response = response;
        }
        public HttpResponseMessage Response { get; }

    }
}