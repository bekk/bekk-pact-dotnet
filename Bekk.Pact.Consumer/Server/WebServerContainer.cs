using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Server;

namespace Bekk.Pact.Consumer.Server
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

        private async Task<Listener> GetListener(Uri uri)
        {
            AssureNotDisposed();
            var taskResult = new TaskCompletionSource<Listener>();
            void Create()
            {
                lock(_lockToken)
                {
                    var listener = new Listener();
                    _listeners.Add(uri, listener);
                    listener.Start(uri, ((IPactResponder)this).Respond);
                    taskResult.SetResult(listener);
                }
            };
            if(!_listeners.TryGetValue(uri, out var result))
            {
                System.Console.WriteLine("No listener: create new");
                Create();
            }
            else
            {
                if(result.State > Listener.ListenerState.Parsing)
                {
                    var handler = new EventHandler<EventArgs>(delegate (object o,EventArgs e){System.Console.WriteLine("Create in event handler");  Create();});
                    result.Stopped += handler;  
                    if(result.State == Listener.ListenerState.Stopped && !taskResult.Task.IsCompleted){
                        result.Stopped -= handler;
                        System.Console.WriteLine("Create outside eent handler");
                        Create();
                    }
                }
            }
            return await taskResult.Task;
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

        public async Task<IVerifyAndClosable> RegisterListener(IPactDefinition pact, IConsumerConfiguration config)
        {
            AssureNotDisposed();
            var handler = new PactHandler(pact, config, Unregister);
            _handlers.Add(handler);
            var listener = await GetListener(config.MockServiceBaseUri);
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
            foreach(var listener in _listeners)
            {
                var l = listener.Value;
                var uri = listener.Key;
                l.Stopped += (o,a)=>{_listeners.Remove(uri);};
                l.Dispose();
            }
        }

    }
}