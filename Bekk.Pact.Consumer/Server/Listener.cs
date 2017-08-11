using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Rendering;

namespace Bekk.Pact.Consumer.Server
{
    class Listener : IDisposable
    {
        private TcpListener listener;
        private Func<IPactRequestDefinition, IPactResponseDefinition> callback;
        private CancellationTokenSource cancellation;

        public void Start(Uri uri, Func<IPactRequestDefinition, IPactResponseDefinition> callback)
        {
            var address = uri.HostNameType == UriHostNameType.IPv4 ? 
                IPAddress.Parse(uri.Host) : 
                Dns.GetHostAddressesAsync(uri.Host).Result[0];
            var port = uri.Port;
            listener = new TcpListener(address, port);
            this.callback = callback;
            Listen(uri).ConfigureAwait(false);
        }

        private async Task Listen(Uri baseUri)
        {
            if (listener == null) throw new InvalidOperationException();
            try
            {
                State = ListenerState.Started;
                listener.Start();
                while (!(cancellation?.IsCancellationRequested).GetValueOrDefault())
                {
                    State = ListenerState.Listening;
                    cancellation = new CancellationTokenSource();
                    using (var client = await Task.Run(() => listener.AcceptTcpClientAsync(), cancellation.Token))
                    {
                        State = ListenerState.Parsing;
                        if (client == null || State == ListenerState.Cancelled) continue;
                        var stream = client.GetStream();
                        byte[] readBuffer = new byte[1024];
                        var request = new StringBuilder();
                        do
                        {
                            var numberOfBytesRead = stream.Read(readBuffer, 0, readBuffer.Length);
                            request.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, numberOfBytesRead));
                        } while (stream.DataAvailable);

                        var pact = new RequestParser(request.ToString(), baseUri);
                        var response = callback(pact);
                        using (var responder = new Responder(stream))
                        {
                            responder.Respond(response);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                System.Console.WriteLine("Ups");
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine(e.StackTrace);
                throw;
            }
            finally
            {
                listener.Stop();
                State = ListenerState.Stopped;
                Stopped?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler<EventArgs> Stopped;

        public void Dispose()
        {
            cancellation?.Cancel();
            listener.Stop();
            State = ListenerState.Cancelled;
        }
        public ListenerState State { get; private set; }
        public enum ListenerState
        {
            Created,
            Started,
            Listening,
            Parsing,
            Cancelled,
            Stopped
        }
    }
}