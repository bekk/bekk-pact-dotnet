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
        }
        public static void LogSafe(this Contracts.IConfiguration config, LogLevel level, Func<string> logMsg)
        {
            if(config == null || level > config.LogLevel || logMsg == null) return;
            config.Log(logMsg.Invoke());
        }
    }
}