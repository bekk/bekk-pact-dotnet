using System.Linq;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Exceptions;
using Bekk.Pact.Provider.Web.Contracts;
using Bekk.Pact.Provider.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Bekk.Pact.Provider.Web
{
    public class PactRunner<TStartup> where TStartup : class
    {
        private readonly IProviderConfiguration configuration;
        private readonly IProviderStateSetup setup;
        public PactRunner(IProviderConfiguration configuration, IProviderStateSetup setup)
        {
            this.setup = setup;
            this.configuration = configuration;
        }
        public async Task Assert(string providerName)
        {
            var repo = new PactRepo(configuration);
            var asserts = repo.FetchAll(providerName)
                .Select(pact => {
                    using (var server = new TestServer(new WebHostBuilder().UseStartup<TStartup>(pact, setup)))
                    using (var client = server.CreateClient())
                    {
                        return pact.Assert(client);
                    }
                });
            var results = await Task.WhenAll(asserts);
            if(results.Any(r=>! r.Success)) throw new PactException("Fail!!!");
        }
    }
}