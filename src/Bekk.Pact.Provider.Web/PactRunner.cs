using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Exceptions;
using Bekk.Pact.Provider.Exceptions;
using Bekk.Pact.Provider.Repo;
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
            var results = new List<ITestResult>();
            foreach(var pact in repo.FetchAll(providerName))
            {
                using (var server = new TestServer(new WebHostBuilder().UseStartup<TStartup>(pact, setup)))
                    using (var client = server.CreateClient())
                    {
                        results.Add(await pact.Assert(client));
                    }
            }
            if(results.Any(r=>! r.Success)) throw new AssertionFailedException(results);
        }
    }
}