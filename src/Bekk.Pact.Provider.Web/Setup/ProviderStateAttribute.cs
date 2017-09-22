using System;

namespace Bekk.Pact.Provider.Web.Setup
{
    /// <summary>
    /// Use this attribute to decorate methods for setting up
    /// for each provider state in classes inheriting from 
    /// <seealso="ProviderStateSetupBase" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ProviderStateAttribute: Attribute
    {
        public ProviderStateAttribute(string state)
        {
            State = state;
        }
        public string State { get; private set; }
    }
}