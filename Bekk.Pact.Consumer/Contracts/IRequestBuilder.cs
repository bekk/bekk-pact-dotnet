using System.Net.Http;

namespace Bekk.Pact.Consumer.Contracts
{
    public interface IRequestBuilder
    {
        IRequestBuilder WithQuery(string key, string value);
        IRequestBuilder WithVerb(HttpMethod verb);
        IRequestBuilder WithVerb(string verb);
        IRequestBuilder WithHeader(string key, params string[] values);
        IRequestBuilder FromProvider(string provider);
        IResponseBuilder ThenResponds();
    }
}