using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LexiconLMS.Models
{
    public class Chat
    {
        public string ChatId { get; set; }
        public DateTime Date { get; set; }

        //Navigation Properties
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

    }
}