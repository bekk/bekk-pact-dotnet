using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Provider.Config
{
    /// <summary>
    /// Use this class to build a configuration object.
    /// </summary>
    public class Configuration : Bekk.Pact.Common.Utils.Configuration<Configuration>, IProviderConfiguration
    {
        private Configuration()
        {
            comparison = StringComparison.CurrentCultureIgnoreCase;
        }
        public static Configuration With => new Configuration();

        public Configuration Comparison(StringComparison comparison)
        {
            this.comparison = comparison;
            return this;
        }
        private Uri mockServiceUri = new Uri("http://localhost:1234");

        private StringComparison comparison;
        StringComparison IProviderConfiguration.BodyKeyStringComparison => comparison;
    }
}
