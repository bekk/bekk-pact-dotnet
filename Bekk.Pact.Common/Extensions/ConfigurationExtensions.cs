using System;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void LogSafe(this Contracts.IConfiguration config, LogLevel level, string text)
        {
            if(config == null || level > config.LogLevel || text == null) return;
            config.Log(text);
            LogToFile(config.LogFile, level, text);
        }
        public static void LogSafe(this Contracts.IConfiguration config, LogLevel level, Func<string> logMsg)
        {
            if(config == null || level > config.LogLevel || logMsg == null) return;
            var text = logMsg.Invoke();
            config.Log(text);
            LogToFile(config.LogFile, level, text);
        }
        private static void LogToFile(string path, LogLevel level, string text)
        {
            if(string.IsNullOrWhiteSpace(path)) return;
            var txt = $"{DateTime.Now:o} {level}: {text}{Environment.NewLine}";
            System.IO.File.AppendAllText(path, txt);
        }
    }
}