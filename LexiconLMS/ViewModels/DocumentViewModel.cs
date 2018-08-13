using LexiconLMS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace LexiconLMS.ViewModels
{
    public interface IDocumentViewModel
    {
        int? CourseId { get; set; }
        int? ModuleId { get; set; }
        int? ActivityId { get; set; }
        string FileName { get; set; }
        string CourseName { get; set; }
        string ModuleName { get; set; }
        string ActivityName { get; set; }
    }

    public class DocumentUpdateViewModel : IDocumentViewModel
    {
        [Required]
        [Display(Name = "Document type")]
        public int SelectedTypeId { get; set; }

        public SelectList Types { get; set; }

        [Display(Name = "Filename")]
        public string FileName { get; set; }

        public string Description { get; set; }

        public DateTime? Deadline { get; set; }

        public Document Document { get; set; }

        public int? CourseId { get; set; }
        public string CourseName { get; set; }
        public int? ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int? ActivityId { get; set; }
        public string ActivityName { get; set; }
    }

    public class DocumentInsertViewModel : DocumentUpdateViewModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }

        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }
    }
}
