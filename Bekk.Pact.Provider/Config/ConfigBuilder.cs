using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Provider.Config
{
    public class Configuration : IConfiguration
    {
        private Configuration()
        {
            comparison = StringComparison.CurrentCultureIgnoreCase;
        }
        public static Configuration With => new Configuration();

        public Configuration BrokerUri(Uri uri)
        {
            brokerUri = uri;
            return this;
        }

        public Configuration BrokerUri(Uri serverUri, string providerName) => BrokerUri(new Uri(serverUri,
            $"/pacts/provider/{providerName}/latest"));

        public Configuration Log(Action<string> log){
            this.log = log;
            return this;
        }

        public Configuration Comparison(StringComparison comparison)
        {
            this.comparison = comparison;
            return this;
        }

        private Uri brokerUri;
        Uri IConfiguration.BrokerUri => brokerUri;

        private Action<string> log;
        Action<string> IConfiguration.Log => log;
        private StringComparison comparison;
        StringComparison IConfiguration.BodyKeyStringComparison => comparison;
    }
}
