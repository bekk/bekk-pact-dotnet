using System;
using Bekk.Pact.Common.Exceptions;

namespace Bekk.Pact.Provider.Web.Setup
{
    public class SetupException : PactException
    {
        public SetupException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}