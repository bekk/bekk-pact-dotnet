namespace Bekk.Pact.Common.Contracts
{
    public interface IPactPathMetadata
    {
        string Provider { get; }
        string Consumer { get; }
        string Version { get; }
    }
}