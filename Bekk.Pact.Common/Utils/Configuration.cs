using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Common.Utils
{
    public abstract class Configuration<T> : IConfiguration where T: class
    {
        private Uri _brokerUri;
        private string _publishPath;
        private Action<string> _log;
        private LogLevel _logLevel;
        private string _logFile;
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.BrokerUri"/>
        /// </summary>
        public T BrokerUri(Uri uri)
        {
            _brokerUri = uri;
            return this as T;
        }
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.BrokerUri"/>.
        /// Must be parsable to an absolute uri.
        /// </summary>
        public T BrokerUrl(string url) => BrokerUri(new Uri(url));
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
        public T LogLevel(LogLevel level)
        {
            _logLevel = level;
            return this as T;
        }
        LogLevel IConfiguration.LogLevel => _logLevel;
        public T LogFile(string path)
        {
            _logFile = path;
            return this as T;
        }
        string IConfiguration.LogFile => _logFile;
    }
}