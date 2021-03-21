using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace ClientDesktop
{
    class Program
    {
        static void Main(string[] args)
        {
        
            var sessionId = "715c5034-e97c-48b1-b461-2c411e398972";

            if (args.Length > 0) sessionId = args[0];
            
            // Create the connection.
            HubConnection connection = SignalRConnection.ConnectToMessageHub("http://localhost:5000/" , "ChatHub");

            try
            {
                connection.StartAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                    }
                    else
                    {
                        Console.WriteLine("Connected");
                        connection.InvokeAsync("Register","test1", sessionId, "TARGET").Wait();
            
                    }
                }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            connection.Reconnected += connectionId =>
            {
                Console.WriteLine("Reconnected. connectionId:" + connectionId);
                connection.InvokeAsync("Register","test1", sessionId, "TARGET").Wait();
            
                // Notify users the connection was reestablished.
                // Start dequeuing messages queued while reconnecting if any.

                return Task.CompletedTask;
            };
            
            connection.Reconnecting += error =>
            {
                Console.WriteLine("Re-connecting: " + connection.State + " " + HubConnectionState.Reconnecting);

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.
                return Task.CompletedTask;
            };



            connection.On<string,string>("Registered", ( clientType, connectionId) =>
            {
                Console.WriteLine("### Registered ###");
                Console.WriteLine("Registered:" + clientType );
                Console.WriteLine("Connect Id" + connectionId);
                Console.WriteLine();
            });

            connection.On<string>("ReceiveTextMessage", param =>
            {
                Console.WriteLine("### ReceiveMessage ###");
                Console.WriteLine("Received Text Message:" + param);
                Console.WriteLine();
            });

            connection.On<string, string, string, string>("ReceiveMessage", (user, sessiontoken, message, fromConnectionID) =>
            {
                Console.WriteLine("### ReceiveMessage ###");
                Console.WriteLine("Received From: " + fromConnectionID + "\\" + user );
                Console.WriteLine("SessionToken:" + sessiontoken);
                Console.WriteLine("Message: " + message);
                Console.WriteLine();
            });

            var command = "";
            Console.WriteLine("Enter message or type 'exit' to close console.");
            
            // connection.InvokeAsync("Register","test1", sessionId, "TARGET").Wait();
                  
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Connection ID:" + connection.ConnectionId);
                Console.WriteLine("Session ID:" + sessionId);
                Console.WriteLine();
                command = Console.ReadLine();
                var commandArgs = command.Trim().ToUpper().Split(" ");
                
                switch (commandArgs[0])
                {
                    case "EXIT":
                        connection.StopAsync().Wait();
                        break;

                    case "SESSIONID:":
                        sessionId = commandArgs[1];
                        connection.InvokeAsync("Register","test1", sessionId, "TARGET").Wait();
                        Console.WriteLine("Connection ID:" + connection.ConnectionId);
                        Console.WriteLine("Session ID:" + sessionId);
                        break;

                    case "ALL:":
                        connection.InvokeAsync("SendTextMessage", command).Wait();
                        break;

                    default:
                        try
                        {
                            connection.InvokeAsync("SendMessage", "test1", sessionId, command).Wait();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }

                        break;
                }
            }
        }
      
    }
}
