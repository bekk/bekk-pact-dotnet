using System;
using System.IO;
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
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.PublishPath"/>.
        /// </summary>
        /// <param name="path">A path appended to the temp path.</param>
        public T PublishPathInTemp(string path = null) => PublishPath(path == null ? Path.GetTempPath() : Path.Combine(Path.GetTempPath(), path));
        string IConfiguration.PublishPath => _publishPath;
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.LogLevel"/>.
        /// </summary>
        public T LogLevel(LogLevel level)
        {
            _logLevel = level;
            return this as T;
        }
        LogLevel IConfiguration.LogLevel => _logLevel;
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.LogFile"/>.
        /// </summary>
        public T LogFile(string path)
        {
            _logFile = path;
            return this as T;
        }
        /// <summary>
        /// Sets the value of <see cref="IConfiguration.LogFile"/>.
        /// </summary>
        /// <param name="filename">The filename of the logfile in the temp folder.</param>
        public T LogFileInTemp(string filename) => LogFile(Path.Combine(Path.GetTempPath(), filename));
        string IConfiguration.LogFile => _logFile;
    }
}