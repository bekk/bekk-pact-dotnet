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

namespace Bekk.Pact.Provider
{
    public class PactRepo : PactRepoBase
    {
        private readonly IProviderConfiguration configuration;
        public PactRepo(IProviderConfiguration configuration): base(configuration)
        {
            this.configuration = configuration;
        }
        public IEnumerable<IPact> FetchAll()
        {
            foreach(var parsedPact in FetchPacts())
            {
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
    }
}
