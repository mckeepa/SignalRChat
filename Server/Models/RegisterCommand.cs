namespace SignalRChatApi.Commands
{
    public class RegisterCommand {

        private string _sessionId;

        public string UserId { get; set; }
        public string SessionId { 
            get {return _sessionId;} 
            set{_sessionId = value.ToUpper().Trim();}
        }
        public string ClientType { get; set; }
        public string ConnectionId { get; set; }
    }
}
