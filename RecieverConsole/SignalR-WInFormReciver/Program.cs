using System;
using Microsoft.AspNet.SignalR.Client;

namespace SignalR_WInFormReciver
{
    class Program
    {
        static void Main(string[] args)
        {

            //Set connection
            //var connection = new HubConnection("https://localhost:5001/");
            var connection = new HubConnection("https://localhost:5001/ChatHub/");
            //Make proxy to hub based on hub name on server
            //var myHub = connection.CreateHubProxy("ChatHub");
            //Start connection
            var startTask = connection.Start(new CustomHttpClient((sender, certificate, chain, sslPolicyErrors) =>
            {
                //put some validation logic here if you want to.
                return true;
            }));


            //connection.Start()
            startTask.ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Connected");
                }

            }).Wait();

            //myHub.Invoke<string>("SendTextMessage", "HELLO World ").ContinueWith(task => {
            //    if (task.IsFaulted)
            //    {
            //        Console.WriteLine("There was an error calling send: {0}",
            //                          task.Exception.GetBaseException());
            //    }
            //    else
            //    {
            //        Console.WriteLine(task.Result);
            //    }
            //});

            //myHub.On<string>("ReceiveTextMessage", param => {
            //    Console.WriteLine(param);
            //});

            //myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();


            Console.Read();
            connection.Stop();
        }


        ////var signalRConnection = new SignalRConnection("https://localhost:5001", "chatHub");
        ////var hub =  signalRConnection.ConnectToSignalRServer();

        //Console.WriteLine("Started..." + System.DateTime.Now.ToShortTimeString());


        //var hubConnection = new HubConnection("https://localhost:5001");
        //var hub = hubConnection.CreateHubProxy("ChatHub");
        //hubConnection.Start(new CustomHttpClient((sender, certificate, chain, sslPolicyErrors) =>
        //{
        //    //put some validation logic here if you want to.
        //    return true;
        //})).Wait();




        ////myHub.On<MessageCommand>("ReceiveMessage", message => Console.WriteLine("User: {0}, Session: {1}, Message: {2} ", message.User, message.SessionId, message.Message));
        //hub.On<string, string, string>("ReceiveMessage", (user, session, message) =>
        //{
        //    Console.WriteLine("User: {0}, Session: {1}, Message: {3} ", user, session, message);
        //});

        //hub.On<string>("ReceiveTextMessage", (message) =>
        //{
        //    Console.WriteLine("Message: {3} ", message);
        //});

        //Console.WriteLine("Type 'Exit' to exit the console application. ");
        //var command = Console.ReadLine();



        //while (command != "exit")
        //{
        //    //myHub
        //    Console.WriteLine("Type 'Exit' to exit the console application. ");
        //    command = Console.ReadLine();
        //    //hubConnection.Received();
        //    //myHub.Invoke <MessageCommand> ("SendMessage", new MessageCommand() {
        //    //    Message = "Message",
        //    //    SessionId ="aaa",
        //    //    User ="userid"  });

        //    //hubConnection.Send("aaa");
        //    //hubConnection.EnsureReconnecting((reconnect )=> { Console.WriteLine("Reconnect " + reconnect); }); 


        //    hub.Invoke<string>("SendTextMessage", "Message");

        //    hub.Invoke("SendMessage", "user 111", "session {1}", "message {0}", command, DateTime.Now.ToShortTimeString());
        //};

    //}
    }
}
