using System;

namespace Bekk.Pact.Consumer.Contracts
{
    public interface IPact : IDisposable
    {
        void Verify();
    }
}