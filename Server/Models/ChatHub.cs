using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChatApi
{
    public class ChatHub : Hub
    {
   
        public void Start()
        {
            System.Diagnostics.Debug.WriteLine("Started ");
        }
        public async Task SendTextMessage( string message)
        {
            System.Diagnostics.Debug.WriteLine("Recieved Text Messge: " + message);

            await Clients.All.SendAsync("ReceiveMessage", "user", "sessionId", message);
            await Clients.All.SendAsync("ReceiveTextMessage", message);
        }
        public async Task SendMessage(string user,string sessionId, string message)
        {
            System.Diagnostics.Debug.WriteLine("Recieved  Messge: " + message);
            await Clients.All.SendAsync("ReceiveMessage", user, sessionId, message);
            await Clients.All.SendAsync("ReceiveTextMessage", message);
        }
    }
}