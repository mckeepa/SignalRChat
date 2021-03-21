using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_WInFormReciver
{
    public class SignalRConnection
    {
        private string _url;
        private string _hubName;


        public SignalRConnection(string url, string hubName) {
            _url = url;
            _hubName = hubName;
        }

        public IHubProxy ConnectToSignalRServer()
        {
            //bool connected = false;
            try
            {
                var Connection = new HubConnection(_url);
                IHubProxy Hub = Connection.CreateHubProxy(_hubName);
                //Connection.Start().Wait();
                //var myHub = hubConnection.CreateHubProxy("ChatHub");
                Connection.Start(new CustomHttpClient((sender, certificate, chain, sslPolicyErrors) =>
                {
                    //put some validation logic here if you want to.
                    return true;
                })).Wait();

                //See @Oran Dennison's comment on @KingOfHypocrites's answer
                if (Connection.State == ConnectionState.Connected)
                {
                    //connected = true;
                    Connection.Closed += Connection_Closed;
                }
                return Hub; // connected;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return null; // connected;
        }

        public void Connection_Closed()
        {   // A global variable being set in "Form_closing" event 
            // of Form, check if form not closed explicitly to prevent a possible deadlock.
            //if (!IsFormClosed)
            //{
                // specify a retry duration
                TimeSpan retryDuration = TimeSpan.FromSeconds(30);
                DateTime retryTill = DateTime.UtcNow.Add(retryDuration);

            while (DateTime.UtcNow < retryTill)
            {
                IHubProxy hubProxy = ConnectToSignalRServer();
                if (hubProxy != null)
                {
                    Console.WriteLine("Connection Open");
                    return;
                }
            }
            Console.WriteLine("Connection closed");

            //}
        }
    }
}
