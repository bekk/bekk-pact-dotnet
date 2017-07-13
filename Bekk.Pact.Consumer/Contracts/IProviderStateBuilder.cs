namespace Bekk.Pact.Consumer.Contracts
{
    public interface IProviderStateBuilder
    {
        IProviderStateBuilder WithConsumer(string consumer);
        IRequestBuilder WhenRequesting(string path);
    }
}