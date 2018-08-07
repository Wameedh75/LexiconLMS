using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LexiconLMS.ViewModels
{
    public class LandingPageViewModel
    {
        public ChatsListViewModel ChatsList { get; set; }
        public MessagesListViewModel MessagesList { get; set; }
    }
}