using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Exceptions;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Provider.Config;
using Bekk.Pact.Provider.Contracts;
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
    /// Fetches, parses and verifies pacts for a provider.
    /// </summary>
    /// <typeparam name="TStartup">The class to use to startup the web server</typeparam>
    public class PactRunner<TStartup> where TStartup : class
    {
        private readonly IProviderConfiguration configuration;
        private readonly IProviderStateSetup setup;
        /// <summary>
        /// Creates a new wrapper object to setup a test server and fetch all pacts. Configuration is read solely from environment variables.
        /// </summary>
        /// <param name="setup">A setup object responsible for mocking in front of each verification.</param>
        public PactRunner(IProviderStateSetup setup) : this(null, setup)
        {
        }
        /// <summary>
        /// Creates a new wrapper object to setup a test server and fetch all pacts.
        /// </summary>
        /// <param name="configuration"> A configuration object</param>
        /// <param name="setup">A setup object responsible for mocking in front of each verification.</param>
        public PactRunner(IProviderConfiguration configuration, IProviderStateSetup setup)
        {
            this.setup = setup;
            this.configuration = configuration ?? new EnvironmentBasedConfiguration();
        }
        /// <summary>
        /// Event is raised after each pact verification.
        /// </summary>
        public event EventHandler<PactResultEventArgs> Verified;
        /// <summary>
        /// Event is raised after each failing pact verifion.
        /// </summary>
        public event EventHandler<PactResultEventArgs> VerificationFailed;
        /// <summary>
        /// Fetches all pacts and asserts them.
        /// Throws exception after all pacts are run if any assertion fails.
        /// Use <seealso cref="Verify" /> to avoid the exception.
        /// </summary>
        /// <param name="providerName">The name of the provider, as stated in the pacts.</param>
        /// <exception cref="AssertionFailedException">Thrown when one or more pact assertions fails</exception>
        public async Task Assert(string providerName)
        {
            var results = await Verify(providerName);
            if(results.Any(r=>! r.Success)) throw new AssertionFailedException(results);
        }
        /// <summary>
        /// Fetches all pacts and verifies them. Returns all test results.
        /// </summary>
        /// <param name="providerName">The name of the provider, as stated in the pacts.</param>
        /// <returns>Testresults. Use <seealso cref="ITestResult.Success" /> to check the results of each pact.</returns>
        public async Task<IEnumerable<ITestResult>> Verify(string providerName)
        {
            var repo = new PactRepo(configuration);
            var results = new List<ITestResult>();
            if(!DoNotGenerateOneDummyTestResult) results.Add(new DummyTestResult());
            foreach(var pact in repo.FetchAll(providerName))
            {
                using (var server = new TestServer(new WebHostBuilder().UseStartup<TStartup>(pact, setup)))
                using (var client = server.CreateClient())
                {
                    var result = await pact.Verify(client);
                    var args = new PactResultEventArgs(pact, result);
                    Verified?.Invoke(this, args);
                    if(!result.Success)
                    {
                         VerificationFailed?.Invoke(this, args);
                         pact.Configuration.LogSafe(LogLevel.Error, $"Assertion failed:{Environment.NewLine}{result}{Environment.NewLine}");
                    }
                    results.Add(result);
                }
            }
            return results;
        }
        /// <summary>
        /// Set this to false to omit one dummy successful result.
        /// </summary>
        /// <returns></returns>
        public bool DoNotGenerateOneDummyTestResult { private get; set; } = false;
    }
}