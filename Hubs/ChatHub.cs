using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Microsoft.AspNetCore.WebUtilities;
using Demo;
using System.Collections.Concurrent;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        static List<object> ChatHistory3 = new List<object>();
        static Dictionary<string, string> ChatHistory = new Dictionary<string, string>();
        static ConcurrentDictionary<string, string> ChatHistory2 = new ConcurrentDictionary<string, string>();
        private MyContext dbContext;
        public ChatHub(MyContext context)
        {
            dbContext = context;
        }
        public async override Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Welcome", ChatHistory3);
        }
        public async Task IConnected()
        {
            System.Console.WriteLine("connected!");
        }
        public async Task SendMessage(string user, string message, int userid)
        {

            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(ChatHistory3));
            await Clients.All.SendAsync("ReceiveMessage", user, message, userid);
        }
        public void SaveMessage(string msg, int userid, string user)
        {
            Console.WriteLine($"*****************************{msg}*******************{userid}**************************");
            Message newMessage = new Message()
            {
                Content = msg,
                UserId = userid

            };
            ChatHistory3.Add(new { user = user, message = msg });
            dbContext.Add(newMessage);
            dbContext.SaveChanges();
        }
    }
}