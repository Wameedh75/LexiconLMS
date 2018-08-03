using LexiconLMS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace LexiconLMS.ViewModels
{
    public class DocumentViewModel
    {
        [Required]
        [Display(Name = "Document type")]
        public int SelectedTypeId { get; set; }

        public SelectList Types { get; set; }

        [Required]
        public HttpPostedFileBase File { get; set; }

        public string Description { get; set; }
        public DateTime? Deadline { get; set; }

        public Course Course { get; set; }
        public Module Module { get; set; }
        public Activity Activity { get; set; }
    }
}
