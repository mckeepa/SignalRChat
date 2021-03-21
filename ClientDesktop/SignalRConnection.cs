using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

public class SignalRConnection
{
      public static HubConnection ConnectToMessageHub(string url, string hubName)
        {
            var connection = new HubConnectionBuilder()
                            // .WithUrl("http://localhost:5000/ChatHub", (opts) =>
                            .WithUrl(url + hubName, (opts) =>
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
                Console.WriteLine("Connection closed. Error: " + error.Message);
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };


           

         

            return connection;
        }
    
}