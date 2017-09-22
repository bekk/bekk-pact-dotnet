using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Bekk.Pact.Provider.Web.Config
{
    public interface IProviderStateSetup
    {
         IEnumerable<Claim> GetClaims(string providerState);
         Action<IServiceCollection> ConfigureServices(string providerState);
    }
}