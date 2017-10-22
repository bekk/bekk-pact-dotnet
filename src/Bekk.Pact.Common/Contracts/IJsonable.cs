using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Common.Contracts
{
    public interface IJsonable
    {
         JContainer Render();
    }
}