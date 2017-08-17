using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Config
{
    public class Configuration : Bekk.Pact.Common.Utils.Configuration<Configuration>, IConsumerConfiguration
    {
        public Configuration()
        {
            Log(Console.WriteLine);
            MockServiceBaseUri(new Uri("http://127.0.0.1:1234"));
        }
        public static Configuration With => new Configuration();
        
        private Uri mockServiceUri;

        public Configuration MockServiceBaseUri(string url) => MockServiceBaseUri(new Uri(url));
        public Configuration MockServiceBaseUri(Uri uri)
        {
            mockServiceUri = uri;
            return this;
        }

        Uri IConsumerConfiguration.MockServiceBaseUri => mockServiceUri;
    }

}