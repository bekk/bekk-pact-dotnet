using System;
using System.Reflection;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Consumer.Contracts;

namespace Bekk.Pact.Consumer.Server
{
    public class Context : IDisposable
    {
        private static WebServerContainer _servers;
        private static Context _instance;
        private IConsumerConfiguration _configuration;
        private Version _version;
        private static object _lockToken = new object();
        public Context(IConsumerConfiguration configuration)
        {
            if(_instance != null) throw new InvalidOperationException("Dispose the old context before creating a new.");
            _instance = this;
            _configuration = configuration;
            lock (_lockToken)
            {
                if(_servers == null)
                {
                    _servers = new WebServerContainer(false);
                }
            }
        }

        public Context WithVersion(Version version)
        {
            _version = version;
            return this;
        }
        public Context WithVersion(AssemblyName assemblyWithVersion)
        {
            _version = assemblyWithVersion.Version;
            return this;
        }
        public Context WithVersion(Type typeFromAssembly)
        {
            return WithVersion(typeFromAssembly.GetTypeInfo().Assembly.GetName());
        }
        public Context WithVersion<T>()
        {
            return WithVersion(typeof(T));
        }        

        internal static IConsumerConfiguration Configuration => _instance?._configuration;
        internal static Version Version => _instance?._version;
        internal static async Task<IVerifyAndClosable> RegisterListener(IPactDefinition pact, IConsumerConfiguration config)
        {
            var servers = _servers;
            if(servers == null)
            {
                lock (_lockToken)
                {
                    if(_servers == null)
                    {
                        _servers = new WebServerContainer(true);
                    }
                    servers = _servers;
                }
            }
            return await servers.RegisterListener(pact, config);
        }
        public void Dispose()
        {
            _instance = null;
            _servers?.Empty();
        }

        public override string ToString() => $"Context {_version} {_configuration?.MockServiceBaseUri}";
    }
}