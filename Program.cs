using System;
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
                client.MessageReceived.Subscribe(msg =>
                    Console.WriteLine($"Message received: {msg}"));
                client.DisconnectionHappened.Subscribe(died => {
                    Console.WriteLine("disconnected for reason " + died);
                    try {
                        Console.WriteLine("Trying to reconnect...");
                        client.Reconnect(); 
                    }
                    catch (Exception e) {
                        Console.WriteLine(e);
                        throw;
                    }
                });
                client.Start();
                while (running) {
                    string send = Console.ReadLine();
                    client.Send(send);
                }
                exitEvent.WaitOne();
            }
        }
    }
}
