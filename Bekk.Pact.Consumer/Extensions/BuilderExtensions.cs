using Bekk.Pact.Consumer.Contracts;

namespace Bekk.Pact.Consumer.Extensions
{
    public static class BuilderExtensions
    {
        public static IRequestBuilder WithContentTypeJson(this IRequestBuilder builder)
        {
            return builder.WithHeader("Content-Type", "application/json; charset=utf-8");
        }
    }
}