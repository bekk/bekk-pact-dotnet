using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using Bekk.Pact.Provider.Model;
using Bekk.Pact.Common.Utils;
using Bekk.Pact.Common.Exceptions;

namespace Bekk.Pact.Provider
{
    public class PactRepo : PactRepoBase
    {
        private readonly IProviderConfiguration configuration;
        public PactRepo(IProviderConfiguration configuration): base(configuration)
        {
            this.configuration = configuration;
        }
        public IEnumerable<IPact> FetchAll(string providerName)
        {
            var baseUri = configuration?.BrokerUri;
            if(baseUri == null) 
            {
                configuration.LogSafe(LogLevel.Error, "Broker uri is not configured.");
                throw new InvalidOperationException("Broker uri is missing");
            }
            var uri = new Uri(baseUri, $"/pacts/provider/{providerName}/latest");
            foreach(var parsedPact in FetchPacts(uri))
            {
                var consumer = parsedPact.SelectToken("consumer.name").ToString();
                configuration.LogSafe(LogLevel.Verbose, $"Parsing pact for {consumer}");
                foreach(var interaction in parsedPact["interactions"].Children().Select(i => i.ToObject<Interaction>()))
                {
                    interaction.Consumer = consumer;
                    interaction.Created = parsedPact["createdAt"].ToObject<DateTime>();
                    yield return new InteractionPact(interaction, configuration);
                }
            }
        }

        private IEnumerable<JObject> FetchPacts(Uri brokerUri)
        {
            var client = Client;
            foreach(var url in FetchUrls(brokerUri).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                Configuration.LogSafe(LogLevel.Verbose, $"Fetching pact at {url}");
                var brokerResponse = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
                brokerResponse.EnsureSuccessStatusCode();
                var parsedPact = JObject.Parse(brokerResponse.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
                yield return parsedPact;
            }
        }

        private async Task<IEnumerable<Uri>> FetchUrls(Uri uri)
        {
            try
            {
                var client = Client;
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return JObject.Parse(
                    response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                    .SelectToken("_links.pacts").Children().Select(t => t["href"].ToObject<Uri>());
            }
            catch(HttpRequestException e)
            {
                throw new PactBrokerException($"An error occured during request to the pact broker on {uri}. ({e.Message})", e);
            }
            catch(Exception e)
            {
                Configuration.LogSafe(LogLevel.Error, $"An error occured during request to the pact broker on {uri}. ({e.Message})");
                throw;
            }
        }
    }
}
