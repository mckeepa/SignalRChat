using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChatApi.Models{

    public class Session 
    {
        public string OriginConnectionId { get; set; }
        public string TagetConnectionId { get; set; }
        public string SessionToken {get; set;}
        public string UserId {get; set;}    
    }

}