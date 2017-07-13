namespace Bekk.Pact.Consumer.Contracts
{
    interface IPactDefinition : IPactRequestDefinition, IPactResponseDefinition
    {
        string State { get; }
        string Description { get; }
        string Provider { get; }
        string Consumer { get; }
    }
}