using LexiconLMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LexiconLMS.ViewModels
{
    public class MessagesListViewModel
    {
        public IEnumerable<Message> Messages { get; set; }
    }
}