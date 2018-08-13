using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
namespace LexiconLMS
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send(string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.addNewMessageToPage(Context.User.Identity.Name, message);
            Clients.All.hello();
        }
    }
}