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
                listener.Start();
                System.Console.WriteLine("Listener started.");
                while (!(cancellation?.IsCancellationRequested).GetValueOrDefault())
                {
                    cancellation = new CancellationTokenSource();
                    using (var client = await Task.Run(() => listener.AcceptTcpClientAsync(), cancellation.Token))
                    {
                        if (client == null) continue;
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
            finally
            {
                listener.Stop();
                System.Console.WriteLine("Listener stopped");
            }
        }

        public void Dispose()
        {
            cancellation?.Cancel();
            System.Console.WriteLine("Listener cancelled");
        }

    }
}