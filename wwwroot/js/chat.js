"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    console.log(message);
    const encodedMessage = encodeURIComponent(message);
    fetch(`https://yodish.p.rapidapi.com/yoda.json?text=${encodedMessage}`, {
        "method": "POST",
        "headers": {
            "x-rapidapi-host": "yodish.p.rapidapi.com",
            "x-rapidapi-key": "e9077e6ee9mshe29568a25c117e8p111788jsn2ad418d390bc",
            "content-type": "application/x-www-form-urlencoded"
        },
        "body": {}
    }).then(response => response.json()).then(data => {
        console.log(data.contents.translated);
        var li = document.createElement("li");
        var encodedMsg = user + " says " + data.contents.translated;
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);

    });
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});