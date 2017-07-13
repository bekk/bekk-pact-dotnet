using System;
using System.Threading.Tasks;

namespace Bekk.Pact.Consumer.Contracts
{
    public interface IPact : IDisposable
    {
        void Verify();
        Task VerifyAndSave();
    }
}