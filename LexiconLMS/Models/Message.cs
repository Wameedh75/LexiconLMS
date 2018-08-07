using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LexiconLMS.Models
{
    public class Message
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string Content { get; set; }

        public string UserId { get; set; }
        public string ChatId { get; set; }

        //Navigation Properties
        public virtual ApplicationUser User { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual Attachment Attachment { get; set; }
    }
}