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
    class WebServerContainer : IPactResponder
    {
        private readonly bool _implicitCreation;
        private object _lockToken = new object();
        private IDictionary<Uri, Listener> _listeners = new Dictionary<Uri, Listener>();
        private IList<PactHandler> _handlers = new List<PactHandler>();
        public WebServerContainer(bool implicitCreation)
        {
            _implicitCreation = implicitCreation;
        }

        private bool ListenerIsActive(Listener listener) => listener!=null && listener.State < Listener.ListenerState.Cancelled;

        private async Task<Listener> GetListener(Uri uri)
        {
            var taskResult = new TaskCompletionSource<Listener>();
            void Create()
            {
                lock(_lockToken)
                {
                    if(!(_listeners.TryGetValue(uri, out var listener) && ListenerIsActive(listener)))
                    {
                        if(listener != null) _listeners.Remove(uri);
                        listener = new Listener();
                        _listeners.Add(uri, listener);
                        listener.Start(uri, ((IPactResponder)this).Respond);
                    }
                    taskResult.SetResult(listener);
                }
            };
            if(!_listeners.TryGetValue(uri, out var result))
            {
                Create();
            }
            else
            {
                if(ListenerIsActive(result))
                {
                    taskResult.SetResult(result);
                }
                else
                {
                    var handler = new EventHandler<EventArgs>(delegate (object o,EventArgs e){Create();});
                    result.Stopped += handler;  
                    if(result.State == Listener.ListenerState.Stopped && !taskResult.Task.IsCompleted){
                        result.Stopped -= handler;
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

        public async Task<IVerifyAndClosable> RegisterListener(IPactInteractionDefinition pact, IConsumerConfiguration config)
        {
            var handler = new PactHandler(pact, config, Unregister);
            _handlers.Add(handler);
            var listener = await GetListener(config.MockServiceBaseUri);
            return handler;
        }

        private void Unregister(PactHandler handler)
        {
            _handlers.Remove(handler);
            if(!_handlers.Any() && _implicitCreation) Empty();
        }

        public void Empty()
        {
            var removable = _listeners.ToList();
            foreach(var listener in removable)
            {
                var l = listener.Value;
                var uri = listener.Key;
                l.Stopped += (o,a)=>{_listeners.Remove(uri);};
                l.Dispose();
            }
        }

    }
}