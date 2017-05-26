namespace Bekk.Pact.Common.Contracts
{
    public interface ITestResult
    {
        bool Success { get; }
        ValidationTypes ErrorTypes { get; }
    }
}
