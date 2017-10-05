using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Provider.Config
{
    /// <summary>
    /// Use this class to build a configuration object.
    /// </summary>
    public class Configuration : Bekk.Pact.Common.Config.Configuration<Configuration>, IProviderConfiguration
    {
        private Configuration(IProviderConfiguration inner) : base(inner)
        {
            this.inner = inner;
            comparison = StringComparison.CurrentCultureIgnoreCase;
        }
        public static Configuration With => new Configuration(FromEnvironmentVartiables());
        public static IProviderConfiguration FromEnvironmentVartiables() => new EnvironmentBasedConfiguration();
        public Configuration Comparison(StringComparison comparison)
        {
            this.comparison = comparison;
            return this;
        }
        private Uri mockServiceUri = new Uri("http://localhost:1234");
        private IProviderConfiguration inner;
        private StringComparison comparison;
        StringComparison? IProviderConfiguration.BodyKeyStringComparison => inner?.BodyKeyStringComparison ?? comparison;
    }
}
