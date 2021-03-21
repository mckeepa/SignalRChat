using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientDesktop
{
    class Program
    {
        static void Main(string[] args)
        {

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/ChatHub", (opts) =>
                    {
                        opts.HttpMessageHandlerFactory = (message) =>
                        {
                            if (message is HttpClientHandler clientHandler)
                                // bypass SSL certificate
                                clientHandler.ServerCertificateCustomValidationCallback +=
                                    (sender, certificate, chain, sslPolicyErrors) => { return true; };
                            return message;
                        };
                    })
                .WithAutomaticReconnect()
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0,5) * 1000);
                await connection.StartAsync();
            };

            connection.Reconnecting += error =>
            {
                Console.WriteLine("Re-connecting: " + connection.State + " " + HubConnectionState.Reconnecting);

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.
                return Task.CompletedTask;
            };

            try
            {
                connection.StartAsync().ContinueWith(task => {
                    if (task.IsFaulted) {
                        Console.WriteLine("There was an error opening the connection:{0}",
                                        task.Exception.GetBaseException());
                    } else {
                        Console.WriteLine("Connected");
                    }
                }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            connection.On<string>("ReceiveTextMessage", param => {
                Console.WriteLine("Received Text Message:" + param);
            });

            connection.On<string, string, string>("ReceiveMessage", (user, sessiontoken, message) => {
                Console.WriteLine("Received  Message User:" + user + " SessionToken:" + sessiontoken +" Message: " + message );
            });

            var command = "";
            Console.WriteLine("Enter message or type 'exit' to close console.");
            while (command.Trim().ToUpper() != "EXIT")
            {

                command = Console.ReadLine();
                try
                {
                    connection.InvokeAsync("SendTextMessage",command).Wait();
                }
                catch (Exception ex)
                {                
                    Console.WriteLine("Error: " + ex.Message);                
                }
            }

            connection.StopAsync().Wait();


          
            //  //Set connection
            // // var connection = new HubConnection("https://localhost:5001/");
            // // //Make proxy to hub based on hub name on server
            // // var myHub = connection.CreateHubProxy("chatHub");
            // //Start connection

            // connection.Start().ContinueWith(task => {
            //     if (task.IsFaulted) {
            //         Console.WriteLine("There was an error opening the connection:{0}",
            //                           task.Exception.GetBaseException());
            //     } else {
            //         Console.WriteLine("Connected");
            //     }

            // }).Wait();

            // myHub.Invoke<string>("Send", "HELLO World ").ContinueWith(task => {
            // connection.InvokeAsync<string>("SendTextMessage", "HELLO World ").ContinueWith(task => {
            //      if (task.IsFaulted) {
            //          Console.WriteLine("There was an error calling send: {0}",
            //                            task.Exception.GetBaseException());
            //      } else {
            //          Console.WriteLine(task.Result);
            //      }
            // });

            // myHub.On<string>("addMessage", param => {
            //     Console.WriteLine(param);
            // });

            // myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();


            // Console.Read();
            // connection.Stop();
        }
    }
}
