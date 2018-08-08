using LexiconLMS.Models;
using LexiconLMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LexiconLMS.Controllers
{
    public class ChatController : Controller
    {
        public ActionResult Chat()
        {
            return View("SendMessage");
        }

        public ActionResult Messages(string chatId)
        {

            if (chatId!=null)
            {
                using (var db = ApplicationDbContext.Create())
                {
                    var MessageList = db.Messages.Where(m => m.ChatId == chatId).ToList();
                    return Json(new { success = true, message = MessageList });
                    //return PartialView("_MessagesList", MessageList);
                }
            }
            return PartialView("_MessagesList");
        }

        public ActionResult ChatsList()
        {
            var fakechat = new ChatsListViewModel()
            {
                //Chats = new List<Chat>() { new Chat() { ChatName = "First Chat", ChatId = "xxx" } }
            };
            return PartialView("_ChatsList", fakechat);
        }
        public ActionResult MessagesList(string chatId)
        {
            if (chatId != null)
            {
                //TODO return real chat list
                return PartialView("_MessagesList");
            }
            var fakemessages = new MessagesListViewModel()
            {
                Messages = new List<Message>()
                {
                    //new Message() { Content = "lets test",User=new ApplicationUser() { FirstName="john" , LastName= "Smith"} }
                //,   //new Message() { Content = "second messagw test",User=new ApplicationUser() { FirstName="Dimitris" , LastName= "Bjorlingh"} }

                }
            };
            return PartialView("_MessagesList", fakemessages);
        }
    }
}