using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using SignalRChatApi.Models;
using System.Collections.Generic;
using System.Linq;
using SignalRChatApi.Handlers;
using SignalRChatApi.Commands;

namespace SignalRChatApi
{
    public class ChatHub : Hub
    {
        internal static IList<SignalRChatApi.Models.Session> _sessions;
   
        static ChatHub()
        {
            _sessions = new List<SignalRChatApi.Models.Session>(); 
        }
        
        public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;
            System.Diagnostics.Debug.WriteLine("Connected ID: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task Register(string userId, string sesssionId, string clientType)
        {      
            var registerHandler = new RegisterHandler(this, _sessions );
            await registerHandler.Handle(new RegisterCommand(){
                UserId = userId,
                SessionId = sesssionId,
                ClientType = clientType,
                ConnectionId = Context.ConnectionId} 
            );
        }
       
        public async Task SendTextMessage( string message)
        {
            System.Diagnostics.Debug.WriteLine("Recieved Text Messge: " + message);

            await Clients.All.SendAsync("ReceiveTextMessage", message);
        }
        public async Task SendMessage(string user,string sessionId, string message)
        {

            var sendCommandCommand = new SendCommandHandler(this, _sessions);
            
            await sendCommandCommand.Handle(new SendCommandCommand(){
                UserId = user,
                ConnectionId = this.Context.ConnectionId,
                Message =message,
                SessionId =sessionId
            });
        }
    }
}