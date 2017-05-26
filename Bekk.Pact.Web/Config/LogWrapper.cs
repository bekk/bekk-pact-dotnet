using System;
using Microsoft.Extensions.Logging;

namespace Bekk.Pact.Web.Config
{
    public class LogWrapper : ILoggerProvider, ILogger
    {
        private readonly Action<string> output;

        public LogWrapper(Action<string> output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
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

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            output(formatter(state, exception));
        }
    }
}