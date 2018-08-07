using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Security.Principal;

namespace LexiconLMS.Models
{
    public class Document
    {
        public Document() {
            Timestamp = DateTime.Now;
        }

        public int Id { get; set; }

        [Required]
        [Column("TypeId")]
        public int TypeId { get; set; }

        [ForeignKey("TypeId")]
        public virtual DocumentType Type { get; set; }

        [Required]
        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "File name")]
        public string FileName { get; set; }

        public string Url => Path.Combine("/Documents/", FileName);
        public string FullPath { get; set; }

        [Required]
        [Display(Name = "MIME type")]
        public string MimeType { get; set; }

        public string Description { get; set; }
        public DateTime Timestamp { get; }
        public DateTime? Deadline { get; set; }


        [Column("CourseId")]
        public int? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [Column("ModuleId")]
        public int? ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        [Column("ActivityId")]
        public int? ActivityId { get; set; }
        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        public bool CanEdit(IPrincipal user) => Type.CanCreateByUser(user);
        public bool CanView(IPrincipal user) => Type.CanViewByUser(user);
    }
}
