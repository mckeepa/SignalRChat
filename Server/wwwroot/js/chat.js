"use strict";

document.getElementById("sendButton").disabled = true;

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
        //Disable send button until connection is established
        document.getElementById("sendButton").disabled = false;
        console.log("Connection Id:" + connection.connection.connectionId);
        document.getElementById("connectionId").value = connection.connection.connectionId;
        
        var sessionId =  document.getElementById("sessionId").value;
        var user = document.getElementById("userInput").value;

        


    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(start);

// Start the connection.
start();




connection.on("ReceiveMessage", function (user, sessionId, message, fromConnectionId) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "from: " + fromConnectionId + "/" + user + " Session: " +sessionId + " messsage: " +  msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("ReceiveTextMessage", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "Message: "+  msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});



document.getElementById("registerOriginButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var sessionId =  document.getElementById("sessionId").value;
    
    connection
        .invoke("Register", user, sessionId, "ORIGIN")
        .catch(function (err) {
        return console.error(err.toString());
    })
});

    
    
document.getElementById("registerTargetButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var sessionId =  document.getElementById("sessionId").value;
    
    connection
        .invoke("Register", user, sessionId, "TARGET")
        .catch(function (err) {
        return console.error(err.toString());
    })
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var sessionId =  document.getElementById("sessionId").value;
    var message = document.getElementById("messageInput").value;
    connection
        .invoke("SendMessage", user, sessionId, message)
        .catch(function (err) {
            return console.error(err.toString());
        });
    event.preventDefault();
});