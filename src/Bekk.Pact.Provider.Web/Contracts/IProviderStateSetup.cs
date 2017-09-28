using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Bekk.Pact.Provider.Web.Contracts
{
    /// <summary>
    /// Implementations are responsible for setting up the environment before
    /// testing each claim.
    /// Implemantations may inherit from <seealso cref="Bekk.Pact.Provider.Web.Setup.ProviderStateSetupBase" />.
    /// </summary>
    public interface IProviderStateSetup
    {
        /// <summary>
        /// This method is called before testing each interaction.
        /// It should return claims to be included in the user's 
        /// claims identity (as bearer token).
        /// </summary>
        /// <param name="providerState">The provider state, as defined in the pact interaction.</param>
        IEnumerable<Claim> GetClaims(string providerState);
        /// <summary>
        /// This method is called before testing each interaction.
        /// It should setup the service collection (i.e. mock) with services.
        /// </summary>
        /// <param name="providerState">The provider state, as defined in the pact interaction.</param>
        Action<IServiceCollection> ConfigureServices(string providerState);
    }
}