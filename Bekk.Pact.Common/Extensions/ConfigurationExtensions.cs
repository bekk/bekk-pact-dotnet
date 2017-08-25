using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void LogSafe(this Contracts.IConfiguration config, LogLevel level, string text)
        {
            if(config == null || level > config.LogLevel) return;
            config?.Log?.Invoke(text);
        }    
    }
}