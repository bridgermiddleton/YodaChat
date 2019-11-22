"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection.on("Welcome", (data) => {
    console.log(data);
    const list = document.getElementById("messagesList");
    data.forEach(thing => {
        const li = document.createElement("li");
        li.textContent = thing.user + ":  " + thing.message;
        list.appendChild(li);
    })

});
//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message, userid) {
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
        const li = document.createElement("li");
        const encodedMsg = user + ":  " + data.contents.translated;
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
        const messagesContainer = document.getElementById("messagesDiv");
        $('#messagesDiv').animate({
            scrollTop: $('#messagesDiv')[0].scrollHeight
        }, "slow");
        connection.invoke('SaveMessage', data.contents.translated, userid, user).catch(err => console.log("Something went oh horribly so wrong....", err))

    });
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    const userid = document.getElementById("userid").value;
    connection.invoke("SendMessage", user, message, userid).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});