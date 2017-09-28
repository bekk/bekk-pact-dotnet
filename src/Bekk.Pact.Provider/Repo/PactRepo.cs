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
using System.IO;
using Newtonsoft.Json;

namespace Bekk.Pact.Provider.Repo
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
            if(Configuration.BrokerUri == null && string.IsNullOrWhiteSpace(Configuration.PublishPath))
            {
                throw new InvalidOperationException("Broker uri and publish path is missing in configuration. Please provide one of them.");
            }
            return FetchAndParseAllFromBroker(configuration.BrokerUri, providerName)
                .Union(FetchAndParseNewestFromFileSystem(configuration.PublishPath, providerName));
        }

        private IEnumerable<IPact> FetchAndParseNewestFromFileSystem(string path, string providerName)
        {
            if(string.IsNullOrWhiteSpace(path)) return Enumerable.Empty<IPact>();            
            var result = new List<IPact>();
            foreach(var file in Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories))
            {
                Configuration.LogSafe(LogLevel.Verbose, $"Parsing pact file {file}");
                try
                {
                    using(var stream = File.OpenText(file))
                    using(var reader = new JsonTextReader(stream))
                    {
                        result.AddRange(ReadInteractionFromJson((JObject)JToken.ReadFrom(reader), providerName));
                    }
                    Configuration.LogSafe(LogLevel.Verbose, $"Parsing pact file {file} successful.");
                }
                catch(Exception e)
                {
                    Configuration.LogSafe(LogLevel.Error, $"Error when parsing pact file {file}");
                    Configuration.LogSafe(LogLevel.Error, $"Inner exception: {e.Message}");
                }
            }
            return result;
        }

        private IEnumerable<IPact> FetchAndParseAllFromBroker(Uri uri, string providerName)
        {
            if(uri == null) return Enumerable.Empty<IPact>();
            var brokerUri = new Uri(uri, $"/pacts/provider/{providerName}/latest");
            return FetchPacts(brokerUri).SelectMany(json => ReadInteractionFromJson(json, providerName));
        }

        private IEnumerable<IPact> ReadInteractionFromJson(JObject parsedPact, string providerName)
        {
            var provider = parsedPact.SelectToken("provider.name").ToString();
            if(provider != null && provider != providerName)
            {
                Configuration.LogSafe(LogLevel.Verbose, $"Skipping interactions for provider {provider}");
                yield break;
            };
            var consumer = parsedPact.SelectToken("consumer.name").ToString();
            configuration.LogSafe(LogLevel.Verbose, $"Parsing pact for {consumer}");
            foreach(var interaction in parsedPact["interactions"].Children().Select(i => i.ToObject<Interaction>()))
            {
                interaction.Consumer = consumer;
                interaction.Created = parsedPact["createdAt"].ToObject<DateTime>();
                yield return new InteractionPact(interaction, configuration);
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
