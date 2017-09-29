using System;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Exceptions;

namespace Bekk.Pact.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void LogSafe(this Contracts.IConfiguration config, LogLevel level, string text)
        {
            if(config == null || level > config.LogLevel || text == null) return;
            config.Log?.Invoke(text);
            LogToFile(config, level, text);
        }
        public static void LogSafe(this Contracts.IConfiguration config, LogLevel level, Func<string> logMsg)
        {
            if(config == null || level > config.LogLevel || logMsg == null) return;
            var text = logMsg.Invoke();
            config.Log?.Invoke(text);
            LogToFile(config, level, text);
        }
        private static void LogToFile(Contracts.IConfiguration config, LogLevel level, string text)
        {
            var path = config.LogFile;
            if(string.IsNullOrWhiteSpace(path)) return;
            var txt = $"{DateTime.Now:o} {level}: {text}{Environment.NewLine}";
            try
            {
                System.IO.File.AppendAllText(path, txt);
            }
            catch(Exception e)
            {
                throw new ConfigurationException($"Couldn't write to log file at {path} ({e.Message}).", config);
            }
        }
    }
}