using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Matching;

namespace Bekk.Pact.Consumer.Server
{
    public class Context : IDisposable
    {
        private static WebServerContainer _servers;
        private static Context _instance;
        private IConsumerConfiguration _configuration;
        private string _consumerName;
        private Version _version;
        private readonly IList<IPactInteractionDefinition> _successful = new List<IPactInteractionDefinition>();
        private readonly IList<IPactInteractionDefinition> _failures = new List<IPactInteractionDefinition>();
        private static object _lockToken = new object();
        public Context(IConsumerConfiguration configuration)
        {
            if (_instance != null) throw new InvalidOperationException("Dispose the old context before creating a new.");
            _instance = this;
            _configuration = configuration;
            lock (_lockToken)
            {
                if (_servers == null)
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
        public Context WithVersion(Type typeFromAssembly) => WithVersion(typeFromAssembly.GetTypeInfo().Assembly.GetName());
        public Context WithVersion<T>() => WithVersion(typeof(T));
        public Context WithVersion(string version) => WithVersion(Version.Parse(version));
        public Context ForConsumer(string consumerName)
        {
            _consumerName = consumerName;
            return this;
        }
        public IEnumerable<string> Successes => _successful.Select(p => p.ToString());
        public IEnumerable<string> Failures => _failures.Select(p => p.ToString());
        internal static string ConsumerName => _instance?._consumerName;
        internal static IConsumerConfiguration Configuration => _instance?._configuration;
        internal static Version Version => _instance?._version;
        internal static async Task<IVerifyAndClosable> RegisterListener(IPactInteractionDefinition pact, IConsumerConfiguration config)
        {
            var servers = _servers;
            if (servers == null)
            {
                lock (_lockToken)
                {
                    if (_servers == null)
                    {
                        _servers = new WebServerContainer(true);
                    }
                    servers = _servers;
                }
            }
            return new HandlerWrapper(await servers.RegisterListener(pact, config), success => _instance?.ClosePact(pact, success) );
        }

        private void ClosePact(IPactInteractionDefinition pact, bool success)
        {
            if (success) _successful.Add(pact);
            else _failures.Add(pact);
        }

        private async Task PublishIfSuccessful()
        {
            if(_configuration == null) System.Console.WriteLine("No configuration in context. Publishing is not possible.");
            if(_failures.Any())
            {
                _configuration.LogSafe($"There are {_failures.Count} failing pacts. Publishing is omitted.");
            }
            var repo = new PactRepo(_configuration);
            foreach(var pact in new PactGrouper(_successful))
            {
                await repo.Put(pact);
            }
        }

        public void Dispose()
        {
            _instance = null;
            _servers?.Empty();
            PublishIfSuccessful().Wait();
        }
        public override string ToString() => $"Context {_version} {_configuration?.MockServiceBaseUri}";
        private class HandlerWrapper : IVerifyAndClosable
        {
            private readonly IVerifyAndClosable _inner;
            private readonly Action<bool> _close;
            private bool _closed;

            public HandlerWrapper(IVerifyAndClosable inner, Action<bool> close)
            {
                _close = close;
                _inner = inner;
            }

            int IVerifyAndClosable.VerifyAndClose(int expectedMatches)
            {
                var result = _inner.VerifyAndClose(expectedMatches);
                if(!_closed)
                {
                    _close(result == expectedMatches);
                    _closed = true;
                }
                return result;
            }
        }
    }
}