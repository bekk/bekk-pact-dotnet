using System;
using System.Diagnostics;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Provider.Web.Events
{
    [DebuggerDisplay("{Pact} {Result}")]
    public class PactResultEventArgs : EventArgs
    {
        public IPact Pact { get; }
        public ITestResult Result { get; }
        public PactResultEventArgs(IPact pact, ITestResult result)
        {
            this.Result = result;
            this.Pact = pact;

        }
    }
}