using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Bekk.Pact.Common.Contracts;
using System.Linq;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Common.Exceptions;
using System.Text;

namespace Bekk.Pact.Common.Utils
{
    public abstract class PactRepoBase : IDisposable
    {
        protected PactRepoBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private HttpClient _client;
        private HttpClient Client => _client ?? (_client = new HttpClient());
        protected IConfiguration Configuration { get; }
        
        protected async Task PutPacts(IPactPathMetadata metadata, string payload)
        {
            var uri = new Uri(Configuration.BrokerUri, $"/pacts/provider/{metadata.Provider}/consumer/{metadata.Consumer}/version/{metadata.Version}");
            Configuration.LogSafe($"Uploading pact to {uri}");
            var client = Client;
            var result = await client.PostAsync(uri.ToString(), new StringContent(payload, Encoding.UTF8, "application/json"));
            if(!result.IsSuccessStatusCode)
            {
                Configuration.LogSafe($"Broker replied with {result.StatusCode}: {result.ReasonPhrase}");
                Configuration.LogSafe(await result.Content.ReadAsStringAsync());
                throw new PactRequestException("Couldn't put pact to broker.", result);
            }
            Configuration.LogSafe($"Upload to {uri} complete.");
        }

        protected IEnumerable<JObject> FetchPacts()
        {
            var client = Client;
            foreach(var url in FetchUrls(Configuration).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                Configuration.LogSafe($"Fetching pact at {url}");
                var pactSpecResponse = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
                pactSpecResponse.EnsureSuccessStatusCode();
                var parsedPact = JObject.Parse(pactSpecResponse.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
                yield return parsedPact;
            }
        }

        private async Task<IEnumerable<Uri>> FetchUrls(IConfiguration configuration)
        {
            var client = Client;
            var response = await client.GetAsync(configuration.BrokerUri);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(
                response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                .SelectToken("_links.pacts").Children().Select(t => t["href"].ToObject<Uri>());
        }
        public void Dispose()
        {
            _client?.Dispose();
            _client = null;
        }
    }
}