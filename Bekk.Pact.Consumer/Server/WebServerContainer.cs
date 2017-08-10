using System;
using System.Collections.Generic;
using System.Linq;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Server;

namespace Bekk.Pact.Consumer.ServiceContext
{
    class WebServerContainer : IPactResponder, IDisposable
    {
        private readonly bool _implicitCreation;
        private object _lockToken = new object();
        private IDictionary<Uri, Listener> _listeners = new Dictionary<Uri, Listener>();
        private IList<PactHandler> _handlers = new List<PactHandler>();
        public WebServerContainer(bool implicitCreation)
        {
            _implicitCreation = implicitCreation;
        }
         
        private void AssureNotDisposed() 
        {
            if(IsClosed) throw new InvalidOperationException("Context is closed.");
        }

        private Listener GetListener(Uri uri)
        {
            AssureNotDisposed();
            var result = _listeners[uri];
            if(result == null)
            {
                lock(_lockToken)
                {
                    result = new Listener();
                    _listeners.Add(uri, result);
                    result.Start(uri, ((IPactResponder)this).Respond);
                }
            }
            return result;
        }
        IPactResponseDefinition IPactResponder.Respond(IPactRequestDefinition request)
        {
            if(!_handlers.Any()) throw new InvalidOperationException("Request received, but no pacts are registered");
            var result = _handlers
                .Select(h => h.Respond(request))
                .Where(r => r!= null)
                .FirstOrDefault();
            return result;
        }
        public bool IsClosed { get; private set; }

        public IVerifyAndClosable RegisterListener(IPactDefinition pact, IConsumerConfiguration config)
        {
            AssureNotDisposed();
            var listener = GetListener(config.MockServiceBaseUri);
            var handler = new PactHandler(pact, config, Unregister);
            _handlers.Add(handler);
            return handler;
        }

        private void Unregister(PactHandler handler, bool isValid)
        {
            _handlers.Remove(handler);
            if(!_handlers.Any() && _implicitCreation) Dispose();
        }

        public void Dispose()
        {
            IsClosed = true;
            foreach(var listener in _listeners.Values)
            {
                listener.Dispose();
            }
            _listeners.Clear();
        }

    }
}