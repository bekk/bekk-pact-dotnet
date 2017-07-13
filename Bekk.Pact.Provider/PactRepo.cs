using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using Bekk.Pact.Provider.Model;

namespace Bekk.Pact.Provider
{
    public class PactRepo : IDisposable
    {
        private HttpClient _client;
        private HttpClient Client => _client ?? (_client = new HttpClient());

        public void Dispose()
        {
            _client?.Dispose();
            _client = null;
        }

        public IEnumerable<IPact> FetchAll(IProviderConfiguration configuration)
        {
            var client = Client;
            foreach(var url in FetchUrls(configuration).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                configuration.LogSafe($"Fetching pact at {url}");
                var pactSpecResponse = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
                pactSpecResponse.EnsureSuccessStatusCode();
                var parsedPact = JObject.Parse(pactSpecResponse.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
                var consumer = parsedPact.SelectToken("consumer.name").ToString();
                configuration.LogSafe($"Parsing pact for {consumer}");
                foreach(var interaction in parsedPact["interactions"].Children().Select(i => i.ToObject<Interaction>()))
                {
                    interaction.Consumer = consumer;
                    interaction.Created = parsedPact["createdAt"].ToObject<DateTime>();
                    yield return new InteractionPact(interaction, configuration);
                }
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
    }
}
