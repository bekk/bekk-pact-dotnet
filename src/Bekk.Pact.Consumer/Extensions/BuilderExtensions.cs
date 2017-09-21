using Bekk.Pact.Consumer.Contracts;

namespace Bekk.Pact.Consumer.Extensions
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// A shortcut to add header with key <value>Content-Type</value> and value <value>application/json; charset=utf-8</value>
        /// </summary>
        public static IResponseBuilder WithContentTypeJson(this IResponseBuilder builder)
        {
            return builder.WithHeader("Content-Type", "application/json; charset=utf-8");
        }
        /// <summary>
        /// Provide a header that is required in the request.
        /// </summary>
        public static IRequestBuilder WithHeader(this IRequestBuilder builder, string key, int value) => builder.WithHeader(key, value.ToString());
        /// <summary>
        /// Provide a query to add to the url.
        /// </summary>
        public static IRequestBuilder WithQuery(this IRequestBuilder builder, string key, int value) => builder.WithQuery(key, value.ToString());
    }
}