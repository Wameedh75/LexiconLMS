using System;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models
{
    public class Document
    {
        public Document(string fileName, string mimeType) {
            FileName = fileName;
            MimeType = mimeType;
            Timestamp = DateTime.Now;
        }

        public int Id { get; set; }

        [Required]
        public DocumentType Type { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public string FileName { get; }

        [Required]
        public string MimeType { get; }

        public string Description { get; set; }
        public DateTime Timestamp { get; }
        public DateTime? Deadline { get; set; }
        public Course Course { get; set; }
        public Module Module { get; set; }
    }
}
