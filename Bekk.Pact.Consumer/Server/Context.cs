using System;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Consumer.Contracts;

namespace Bekk.Pact.Consumer.Server
{
    public class Context : IDisposable
    {
        private static WebServerContainer _servers;
        private static object _lockToken = new object();
        public Context()
        {
            lock (_lockToken)
            {
                if(_servers == null || _servers.IsClosed)
                {
                    _servers = new WebServerContainer(false);
                }
            } 
        }       

        internal static async Task<IVerifyAndClosable> RegisterListener(IPactDefinition pact, IConsumerConfiguration config)
        {
            lock (_lockToken)
            {
                if(_servers == null || _servers.IsClosed)
                {
                    _servers = new WebServerContainer(true);
                }
            }
            return await _servers.RegisterListener(pact, config);
        }
        public void Dispose()
        {
            _servers?.Dispose();
            _servers = null;
        }
    }
}