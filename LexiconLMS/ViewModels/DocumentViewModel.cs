using LexiconLMS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace LexiconLMS.ViewModels
{
    public class DocumentViewModel
    {
        [Required]
        public DocumentType Type { get; set; }

        [Required]
        public HttpPostedFileBase File { get; set; }

        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
