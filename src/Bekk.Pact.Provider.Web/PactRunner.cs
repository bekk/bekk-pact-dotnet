using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Exceptions;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Provider.Exceptions;
using Bekk.Pact.Provider.Repo;
using Bekk.Pact.Provider.Web.Contracts;
using Bekk.Pact.Provider.Web.Events;
using Bekk.Pact.Provider.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Bekk.Pact.Provider.Web
{
    /// <summary>
    /// Fetches, parses and runs pacts for a provider.
    /// </summary>
    /// <typeparam name="TStartup">The class to use to startup the web server</typeparam>
    public class PactRunner<TStartup> where TStartup : class
    {
        private readonly IProviderConfiguration configuration;
        private readonly IProviderStateSetup setup;
        public PactRunner(IProviderConfiguration configuration, IProviderStateSetup setup)
        {
            this.setup = setup;
            this.configuration = configuration;
        }
        /// <summary>
        /// Event is raised after each interaction test.
        /// </summary>
        public event EventHandler<PactResultEventArgs> Asserted;
        /// <summary>
        /// Event is raised after each failing interaction test.
        /// </summary>
        public event EventHandler<PactResultEventArgs> AssertionFailed;
        /// <summary>
        /// Fetches all pacts and asserts them.
        /// Throws exception after all pacts are run if any assertion fails.
        /// Use <seealso cref="Run" /> to avoid the exception.
        /// </summary>
        /// <param name="providerName">The name of the provider, as stated in the pacts.</param>
        /// <exception cref="AssertionFailedException">Thrown when one or more pact assertions fails</exception>
        public async Task Assert(string providerName)
        {
            var results = await Run(providerName);
            if(results.Any(r=>! r.Success)) throw new AssertionFailedException(results);
        }
        /// <summary>
        /// Fetches all pacts and asserts them.
        /// </summary>
        /// <param name="providerName">The name of the provider, as stated in the pacts.</param>
        /// <returns>Testresults. Use <seealso cref="ITestResult.Success" /> to check the results of each pact.</returns>
        public async Task<IEnumerable<ITestResult>> Run(string providerName)
        {
            var repo = new PactRepo(configuration);
            var results = new List<ITestResult>();
            foreach(var pact in repo.FetchAll(providerName))
            {
                using (var server = new TestServer(new WebHostBuilder().UseStartup<TStartup>(pact, setup)))
                using (var client = server.CreateClient())
                {
                    var result = await pact.Assert(client);
                    var args = new PactResultEventArgs(pact, result);
                    Asserted?.Invoke(this, args);
                    if(!result.Success)
                    {
                         AssertionFailed?.Invoke(this, args);
                         pact.Configuration.LogSafe(LogLevel.Error, $"Validation failed:{Environment.NewLine}{result}{Environment.NewLine}");
                    }
                    results.Add(result);
                }
            }
            return results;
        }
    }
}