using System.Net.Http;

namespace Bekk.Pact.Common.Contracts
{
    public interface ITestResult
    {
        bool Success { get; }
        ValidationTypes ErrorTypes { get; }
        string ExpectedResponseBody { get; }
        HttpResponseMessage ActualResponse { get; }
    }
}
