using System;
using Bekk.Pact.Common.Config;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Consumer.Config
{
    /// <summary>
    /// Use this class to build a configuration object.
    /// Use <see cref="With"/> to create a new instance.
    /// </summary>
    public class Configuration : Bekk.Pact.Common.Config.Configuration<Configuration>, IConsumerConfiguration
    {
        internal Configuration(): this(FromEnvironmentVartiables())
        {
        }
        internal Configuration(IConsumerConfiguration inner) : base(inner)
        {
            if(inner != null)
            {
               mockServiceUri = inner.MockServiceBaseUri;
               this.inner = inner;
            }
            MockServiceBaseUri(new Uri("http://127.0.0.1:1234"));
        }
        /// <summary>
        /// Create a new instance.
        /// </summary>
        public static Configuration With => new Configuration(FromEnvironmentVartiables());

        /// <summary>
        /// Reads environment variables into a configuration object.
        /// </summary>
        public static IConsumerConfiguration FromEnvironmentVartiables() => new EnvironmentBasedConfiguration();
        
        private Uri mockServiceUri;
        private IConsumerConfiguration inner;

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
            mockServiceUri = inner?.MockServiceBaseUri ?? uri;
            return this;
        }

        Uri IConsumerConfiguration.MockServiceBaseUri => mockServiceUri;
    }

}