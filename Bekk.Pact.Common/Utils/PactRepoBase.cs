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
using System.IO;

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
            var broker = PublishToBroker(metadata, payload);
            var filesystem = PublishToFilesystem(metadata, payload);
            await Task.WhenAll(new[]{broker, filesystem});
        }

        private async Task PublishToFilesystem(IPactPathMetadata metadata, string payload)
        {
            if(Configuration.PublishPath == null) return;
            var folder = Path.Combine(Configuration.PublishPath, "pacts", metadata.Consumer, metadata.Provider);
            var filename = $"{metadata.Consumer}_{metadata.Provider}_{metadata.Version}.json";
            var filePath = Path.Combine(folder, filename);
            Directory.CreateDirectory(folder);
            using(var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                var encoded = Encoding.UTF8.GetBytes(payload);
                await file.WriteAsync(encoded, 0, encoded.Length);
            }
            Configuration.LogSafe(LogLevel.Info, $"Saved pact to {filePath}.");
        }
        private async Task PublishToBroker(IPactPathMetadata metadata, string payload)
        {
            if(Configuration.BrokerUri == null) return;
            var uri = new Uri(Configuration.BrokerUri, $"/pacts/provider/{metadata.Provider}/consumer/{metadata.Consumer}/version/{metadata.Version}");
            var client = Client;
            HttpResponseMessage result;
            try
            {
                result = await client.PutAsync(uri.ToString(), new StringContent(payload, Encoding.UTF8, "application/json"));
            }
            catch(Exception e)
            {
                var exception = e.InnerException??e;
                throw new PactException($"Error when connecting to broker <{uri}>: {exception.Message}", exception);
            }
            if(!result.IsSuccessStatusCode)
            {
                Configuration.LogSafe(LogLevel.Error, $"Broker replied with {(int)result.StatusCode}: {result.ReasonPhrase}");
                Configuration.LogSafe(LogLevel.Verbose, await result.Content.ReadAsStringAsync());
                throw new PactRequestException("Couldn't put pact to broker.", result);
            }
            Configuration.LogSafe(LogLevel.Info, $"Uploaded pact to {uri}.");
        }

        protected IEnumerable<JObject> FetchPacts()
        {
            var client = Client;
            foreach(var url in FetchUrls(Configuration).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                Configuration.LogSafe(LogLevel.Verbose, $"Fetching pact at {url}");
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