using System.Net.Http;
using System.Threading.Tasks;

namespace Bekk.Pact.Common.Contracts
{
    public interface IPact
    {
        string ProviderState { get; }
        Task<ITestResult> Assert(HttpClient client);
        IProviderConfiguration Configuration { get; }
    }
}
