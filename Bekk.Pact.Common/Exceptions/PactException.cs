using System;

namespace Bekk.Pact.Common.Exceptions
{
    public class PactException: Exception
    {
        public PactException(string message):base(message)
        {
        }
    }
}