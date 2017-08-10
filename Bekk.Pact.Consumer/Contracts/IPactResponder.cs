namespace Bekk.Pact.Consumer.Contracts
{
    interface IPactResponder
    {
         IPactResponseDefinition Respond(IPactRequestDefinition request);
    }
}