namespace Bekk.Pact.Consumer.Contracts
{
    public interface IConsumerBuilder
    {
        IPactBuilder And(string consumer);
    }
}