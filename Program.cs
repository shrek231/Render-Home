using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading;
using Websocket.Client;

namespace Render {
    class Program {
        static void Main(string[] args) {
            bool running = true;
            var exitEvent = new ManualResetEvent(false);
            var url = new Uri("wss://echo.websocket.org");

            using (var client = new WebsocketClient(url)) {
                client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                client.ReconnectionHappened.Subscribe(info =>
                    Console.WriteLine($"Reconnection happened, type: {info.Type}"));

                client.MessageReceived.Subscribe(msg => {
                    Console.WriteLine($"Message received: {msg}");
                });
                client.Start();
                while (running) {
                    string send = Console.ReadLine();
                    client.Send(send);
                    client.DisconnectionHappened.Subscribe(died => Console.WriteLine("disconnected for reason "+died));
                }
                exitEvent.WaitOne();
            }
        }
    }
}