using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using Microsoft.Extensions.Logging;

namespace Bekk.Pact.Provider.Web.Config
{
    public class LogWrapper : ILoggerProvider, ILogger
    {
        private IProviderConfiguration configuration;

        public LogWrapper(IProviderConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return this;
        }
        
        public void Dispose()
        {

        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            switch (configuration.LogLevel){
                case Common.Contracts.LogLevel.Error: return logLevel > Microsoft.Extensions.Logging.LogLevel.Information;
                case Bekk.Pact.Common.Contracts.LogLevel.Info: return logLevel >=  Microsoft.Extensions.Logging.LogLevel.Information;
                default: return true;
            }
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            configuration.LogSafe(Convert(logLevel), formatter(state, exception));
        }

        private Bekk.Pact.Common.Contracts.LogLevel Convert(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            if(logLevel < Microsoft.Extensions.Logging.LogLevel.Information) return Bekk.Pact.Common.Contracts.LogLevel.Verbose;
            if(logLevel == Microsoft.Extensions.Logging.LogLevel.Information) return Bekk.Pact.Common.Contracts.LogLevel.Info;
            return Bekk.Pact.Common.Contracts.LogLevel.Error;
        }
    }
}