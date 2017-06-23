using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Common.Extensions
{
    public static class HeaderExtensions
    {
        public static string ContentType(this IHeaderCollection headers) 
        {
            if(headers == null) return null;
            return headers["content-type"];
        }
    }
}