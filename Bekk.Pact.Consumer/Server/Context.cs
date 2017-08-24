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
    /// <summary>
    /// Use this to create a context object to share between all pact tests.
    /// It may provide configuration and publishing of pacts when all tests has passed.
    /// Dispose after all tests are run.
    /// </summary>
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
        /// <summary>
        /// Create a new instance of the shared context.
        /// </summary>
        /// <param name="configuration">A configuration that may be overridden by the individual interactions. This may be null.</param>
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
        /// <summary>
        /// Provide a version number to be shared by the interactions. May be overridden.
        /// </summary>
        public Context WithVersion(Version version)
        {
            _version = version;
            return this;
        }
        /// <summary>
        /// Provide a version number from the assembly.
        /// </summary>
        public Context WithVersion(AssemblyName assemblyWithVersion)
        {
            _version = assemblyWithVersion.Version;
            return this;
        }
        /// <summary>
        /// Provide a version number from the assembly of the provided type.
        /// </summary>
        public Context WithVersion(Type typeFromAssembly) => WithVersion(typeFromAssembly.GetTypeInfo().Assembly.GetName());
        /// <summary>
        /// Provide a version number from the assembly of the provided type parameter.
        /// </summary>
        public Context WithVersion<T>() => WithVersion(typeof(T));
        /// <summary>
        /// Provide a valid parsable version number to be shared by the interactions.
        /// </summary>
        public Context WithVersion(string version) => WithVersion(Version.Parse(version));
        /// <summary>
        /// Provide a consumer name to be used by all interactions.
        /// </summary>
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
        /// <summary>
        /// This method will dispose all tcp listeners and publish all pacts if all were succsessful.
        /// </summary>
        public void Dispose()
        {
            _instance = null;
            _servers?.Empty();
            try
            {
                PublishIfSuccessful().Wait();
            }
            catch(Exception e)
            {
                var exception = e.InnerException??e;
                _configuration.LogSafe($"Error occured while publishing: {exception.Message} {exception.StackTrace}");
                throw exception;
            }
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