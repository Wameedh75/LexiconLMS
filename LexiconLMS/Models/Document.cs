using System;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models
{
    public class Document
    {
        public Document() {
            Timestamp = DateTime.Now;
        }

        public int Id { get; set; }

        [Required]
        public DocumentType Type { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string MimeType { get; set; }

        public string Description { get; set; }
        public DateTime Timestamp { get; }
        public DateTime? Deadline { get; set; }
        public Course Course { get; set; }
        public Module Module { get; set; }
    }
}
