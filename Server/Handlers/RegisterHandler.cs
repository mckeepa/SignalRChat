using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SignalRChatApi.Commands;
using SignalRChatApi.Models;

namespace SignalRChatApi.Handlers
{

    public class RegisterHandler 
{

    private Hub _hub;
    private IList<SignalRChatApi.Models.Session> _sessions;

    public RegisterHandler(Hub hub, IList<SignalRChatApi.Models.Session> sessions)
    {
        _hub =hub;
        _sessions = sessions;
    }
     public Task Handle(RegisterCommand registerCommand)
        {

            WriteLog(registerCommand);

            Session session = GetSessionForConnectionId(registerCommand);

            session = UpdateSessionDetails(registerCommand, session);

            return NotifyClientOfRegistrationUpdate(registerCommand);
        }

        private async Task NotifyClientOfRegistrationUpdate(RegisterCommand registerCommand)
        {
            // Send all message and wait for them to complete
            await _hub.Clients.All.SendAsync(
                "Registered" + registerCommand.ClientType,
                _hub.Context.ConnectionId);
        }

        private Session UpdateSessionDetails(RegisterCommand registerCommand, Session session)
        {
            if (session is null)
            {
                session = new SignalRChatApi.Models.Session()
                {
                    SessionToken = registerCommand.SessionId,
                    UserId = registerCommand.UserId,
                    OriginConnectionId = registerCommand.ConnectionId
                };

                _sessions.Add(session);
            }

            // Store Client as either Origin Or Target
            if (registerCommand.ClientType.Trim().ToUpper() == "ORIGIN")
                session.OriginConnectionId = _hub.Context.ConnectionId;
            else
                session.TagetConnectionId = _hub.Context.ConnectionId;
            return session;
        }

        private Session GetSessionForConnectionId(RegisterCommand registerCommand)
        {
            return _sessions
                .Where(item =>
                    item.SessionToken == registerCommand.SessionId 
                    // && item.UserId == registerCommand.UserId
                    )
                .SingleOrDefault();
        }

        private static void WriteLog(RegisterCommand registerCommand)
        {
            System.Diagnostics.Debug.WriteLine(
                "Register Session: " + registerCommand.UserId +
                " SessionId: " + registerCommand.SessionId +
                " connection Id:" + registerCommand.ConnectionId);
        }
    }
}
