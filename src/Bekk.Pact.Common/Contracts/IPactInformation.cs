namespace Bekk.Pact.Common.Contracts
{
    public interface IPactInformation
    {
         string Description { get; }
         string Consumer { get; }
         string ProviderState { get; }
    }
}