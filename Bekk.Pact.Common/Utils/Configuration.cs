using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Common.Utils
{
    public abstract class Configuration<T> : IConfiguration where T: class
    {
        private Uri _brokerUri;
        private string _publishPath;
        private Action<string> _log;
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.BrokerUri"/>
        /// </summary>
        public T BrokerUri(Uri uri)
        {
            _brokerUri = uri;
            return this as T;
        }
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.BrokerUri"/>
        /// Must be parseable to an absoulte uri.
        /// </summary>
        public T BrokerUri(Uri serverUri, string providerName) => BrokerUri(new Uri(serverUri,
            $"/pacts/provider/{providerName}/latest"));
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.Log"/>.
        /// </summary>
        public T Log(Action<string> log){
            this._log = log;
            return this as T;
        }
        Action<string> IConfiguration.Log => _log;
        Uri IConfiguration.BrokerUri => _brokerUri;
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.PublishPath"/>.
        /// </summary>
        public T PublishPath(string path)
        {
            _publishPath = path;
            return this as T;
        }
        string IConfiguration.PublishPath => _publishPath;
    }
}