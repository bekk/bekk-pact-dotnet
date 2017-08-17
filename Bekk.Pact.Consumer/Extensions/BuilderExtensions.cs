using Bekk.Pact.Consumer.Contracts;

namespace Bekk.Pact.Consumer.Extensions
{
    public static class BuilderExtensions
    {
        public static IResponseBuilder WithContentTypeJson(this IResponseBuilder builder)
        {
            return builder.WithHeader("Content-Type", "application/json; charset=utf-8");
        }
    }
}