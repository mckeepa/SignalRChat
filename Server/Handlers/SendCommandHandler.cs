using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SignalRChatApi.Commands;

namespace SignalRChatApi.Handlers
{

    public class SendCommandHandler 
{

    private Hub _hub;
    private IList<SignalRChatApi.Models.Session> _sessions;

    public SendCommandHandler(Hub hub, IList<SignalRChatApi.Models.Session> sessions)
    {
        _hub =hub;
        _sessions = sessions;
    }
     public async Task Handle(SendCommandCommand sendCommandCommand)
    {

            System.Diagnostics.Debug.WriteLine("Recieved  Messge: " +  sendCommandCommand.Message);

            var sessionsForConnectionId = _sessions
            .Where(session =>session.SessionToken  == sendCommandCommand.SessionId); 
                // session.SessionToken  == sendCommandCommand.ConnectionId ||
                // session.TagetConnectionId  == sendCommandCommand.ConnectionId
                 
            var session =  sessionsForConnectionId.FirstOrDefault();

            if (session is not null)
            {

                var tasks = new List<Task>();
                try
                {
                    AddConnection(session.OriginConnectionId, sendCommandCommand, session, tasks);
                    AddConnection(session.TagetConnectionId, sendCommandCommand, session, tasks);

                    await Task.WhenAll(tasks);
                }

                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error sending message: " + ex.Message);
                }

            }

            // System.Diagnostics.Debug.WriteLine(
            //     "Register Session: " + sendCommandCommand.UserId + 
            //     " SessionId: " + sendCommandCommand.SessionId + 
            //     " connection Id:" + sendCommandCommand.ConnectionId) ;

            // var session = _sessions
            //     .Where(item => 
            //         item.SessionToken ==sendCommandCommand.SessionId && 
            //         item.UserId == sendCommandCommand.UserId)
            //     .SingleOrDefault();

            // if (session is null)  
            // {
            //     session = new SignalRChatApi.Models.Session(){
            //         SessionToken = sendCommandCommand.SessionId, 
            //         UserId = sendCommandCommand.UserId,
            //         OriginConnectionId = sendCommandCommand.ConnectionId};

            //     _sessions.Add(session);
            // }

            // if ( sendCommandCommand.ClientType.Trim().ToUpper() == "ORIGIN")
            //     session.OriginConnectionId = _hub.Context.ConnectionId;
            // else
            //     session.TagetConnectionId =  _hub.Context.ConnectionId;

            // // await Clients.All.SendAsync("ReceiveMessage", "user", "sessionId", message);
            // await _hub.Clients.All.SendAsync(
            //     "Registered" + sendCommandCommand.ClientType,  
            //     _hub.Context.ConnectionId);
        }

        private void AddConnection(string connectionId, SendCommandCommand sendCommandCommand, Models.Session session, List<Task> tasks)
        {
            if (connectionId is not null)
                AddTask(connectionId, sendCommandCommand, session, tasks);
        }

        private void AddTask(string connectionId, SendCommandCommand sendCommandCommand, Models.Session session, List<Task> tasks)
        {
            tasks.Add(
                _hub.Clients
                    .Client(connectionId)
                    .SendAsync(
                        "ReceiveMessage",
                        sendCommandCommand.UserId,
                        sendCommandCommand.SessionId,
                        sendCommandCommand.Message, 
                        sendCommandCommand.ConnectionId));
        }
    }
}
