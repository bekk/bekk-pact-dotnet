using System.Net.Http;

namespace Bekk.Pact.Common.Contracts
{
    /// <summary>
    /// The result of a pact test.
    /// Use <seealso cref="Success"/> to test
    /// if the test was successful.
    /// </summary>
    public interface ITestResult
    {
        /// <summary>
        /// True if the test was successful.
        /// </summary>
        bool Success { get; }
        /// <summary>
        /// The types of error occured.
        /// </summary>
        ValidationTypes ErrorTypes { get; }
        /// <summary>
        /// The expected response
        /// </summary>
        string ExpectedResponseBody { get; }
        /// <summary>
        /// The actual response.
        /// </summary>
        HttpResponseMessage ActualResponse { get; }
    }
}
