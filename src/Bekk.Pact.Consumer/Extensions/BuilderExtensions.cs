using System.Collections.Generic;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Rendering;

namespace Bekk.Pact.Consumer.Extensions
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// A shortcut to add header with key <value>Content-Type</value> and value <value>application/json; charset=utf-8</value>
        /// </summary>
        public static IRequestBuilder WithContentTypeJson(this IRequestBuilder builder)
        {
            return builder.WithHeader("Content-Type", "application/json; charset=utf-8");
        }
        /// <summary>
        /// A shortcut to add header with key <value>Content-Type</value> and value <value>application/x-www-form-urlencoded</value>
        /// </summary>
        public static IRequestBuilder WithContentTypeFormUrlEncoded(this IRequestBuilder builder)
        {
            return builder.WithHeader("Content-Type", "application/x-www-form-urlencoded");
        }
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
        /// <summary>
        /// Defines the body as json, setting the content type header and adding the body to the interaction.
        /// </summary>
        public static IRequestBuilder WithJsonBody(this IRequestBuilder builder, object body) => builder.WithContentTypeJson().WithBody(new JsonBody(body));
        /// <summary>
        /// Defines the body as url encoded form data, setting the content type header and adding the body to the interaction.
        /// </summary>
        public static IRequestBuilder WithUrlEncodedFormData(this IRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> values) => 
            builder.WithContentTypeFormUrlEncoded().WithBody(new JsonBody(values));
        /// <summary>
        /// Defines the body as url encoded form data, setting the content type header and adding the body to the interaction.
        /// </summary>
        public static IRequestBuilder WithUrlEncodedFormData(this IRequestBuilder builder, FormData data) => 
            WithUrlEncodedFormData(builder, (Dictionary<string,string>)data);
        /// <summary>
        /// Defines the body as json, setting the content type header and adding the body to the interaction.
        /// </summary>
        public static IResponseBuilder WithJsonBody(this IResponseBuilder builder, object body) => builder.WithContentTypeJson().WithBody(new JsonBody(body));
    }
}