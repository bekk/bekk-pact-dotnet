namespace Bekk.Pact.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void LogSafe(this Contracts.IConfiguration config, string text)
        {
            config?.Log?.Invoke(text);
        }    
    }
}