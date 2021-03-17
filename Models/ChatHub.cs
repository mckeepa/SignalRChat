using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChatApi
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user,string sessionId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, sessionId, message);
        }
    }
}