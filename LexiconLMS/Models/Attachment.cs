using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebGrease.Activities;

namespace LexiconLMS.Models
{
    public class Attachment
    {
        //[Key, ForeignKey("Message")]
        public string MessageId { get; set; }

        //public string Id { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public FileTypes FileType { get; set; }
        public byte[] Content { get; set; }

        //Navigation Property
        //[Required]
        public virtual Message Message { get; set; }

    }
}