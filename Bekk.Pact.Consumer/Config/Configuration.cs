using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Config
{
    /// <summary>
    /// Use this class to build a configuration object.
    /// Use <see cref="With"/> to create a new instance.
    /// </summary>
    public class Configuration : Bekk.Pact.Common.Utils.Configuration<Configuration>, IConsumerConfiguration
    {
        internal Configuration()
        {
            Log(Console.WriteLine);
            MockServiceBaseUri(new Uri("http://127.0.0.1:1234"));
        }
        /// <summary>
        /// Create a new instance.
        /// </summary>
        public static Configuration With => new Configuration();
        
        private Uri mockServiceUri;
        /// <summary>
        /// Sets the value of <see cref="IConsumerConfiguration.MockServiceBaseUri"/>
        /// Must be parseable to an uri (absolute).
        /// </summary>
        public Configuration MockServiceBaseUri(string url) => MockServiceBaseUri(new Uri(url));
        /// <summary>
        /// Sets the value of <see cref="IConsumerConfiguration.MockServiceBaseUri"/>
        /// </summary>
        public Configuration MockServiceBaseUri(Uri uri)
        {
            mockServiceUri = uri;
            return this;
        }

        Uri IConsumerConfiguration.MockServiceBaseUri => mockServiceUri;
    }

}