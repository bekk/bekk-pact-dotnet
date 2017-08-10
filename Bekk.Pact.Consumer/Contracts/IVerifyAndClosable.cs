namespace Bekk.Pact.Consumer.Contracts
{
    public interface IVerifyAndClosable
    {
         int VerifyAndClose(int expectedMatches = 1);
    }
}