using System;

namespace Bekk.Pact.Provider.Web.Setup
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ProviderStateAttribute: Attribute
    {
        public ProviderStateAttribute(string state)
        {
            State = state;
        }
        public string State { get; private set; }
    }
}