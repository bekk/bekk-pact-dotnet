using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Provider.Web.Config;
using Bekk.Pact.Provider.Web.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Bekk.Pact.Provider.Web.Extensions
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseStartup(this IWebHostBuilder hostBuilder, 
            IPact pact, 
            IEnumerable<Claim> claims = null,
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder> configure = null,
            Type startupType=null)
        {
            if (pact == null) throw new ArgumentNullException(nameof(pact));
            if (pact.Configuration != null)
            {
                hostBuilder.ConfigureLogging(fac =>
                {
                    var log = new LogWrapper(pact.Configuration);
                    fac.AddProvider(log);
                });
            }
            var startup = new Startup(startupType)
            {
                ConfigureCallback = configure,
                ConfigureServicesCallback = configureServices
            };
            foreach (var claim in claims??Enumerable.Empty<Claim>())
            {
                startup.AddClaim(claim);
            }
            hostBuilder.ConfigureServices(sc => startup.ConfigureServices(sc));
            hostBuilder.Configure(startup.Configure);
            return hostBuilder;
        }
        public static IWebHostBuilder UseStartup<T>(this IWebHostBuilder hostBuilder, 
            IPact pact,
            IEnumerable<Claim> claims = null,
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder> configure = null) where T : class
        {
            return UseStartup(hostBuilder, pact, claims, configureServices, configure, typeof(T));
        }

        public static IWebHostBuilder UseStartup<T>(this IWebHostBuilder hostBuilder, 
            IPact pact,
            IProviderStateSetup providerStateSetup) where T : class
            {
                return UseStartup<T>(
                    hostBuilder,
                    pact,
                    providerStateSetup.GetClaims(pact.ProviderState),
                    providerStateSetup.ConfigureServices(pact.ProviderState),
                    null);
            }

    }
}