using System.Net.Http;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Provider.Contracts
{
    /// <summary>
    /// The result of a pact test.
    /// Use <seealso cref="Success"/> to test
    /// if the test was successful.
    /// </summary>
    public interface ITestResult : IPactInformation
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
        /// The consumer defining the pact.
        /// </summary>
        string Consumer { get; }
        /// <summary>
        /// The description of the pact
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The provider satte property of the pact
        /// </summary>
        string ProviderState { get; }
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
