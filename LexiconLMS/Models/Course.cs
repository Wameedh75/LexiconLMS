using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Start date")]
        [UIHint("ShortDate")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        [UIHint("ShortDate")]
        public DateTime EndDate { get; set; }

        [Required]
        public string Description { get; set; }

        //public  ICollection<ApplicationUser> Students { get; set; }

        public virtual ICollection<ApplicationUser> CourseStudents { get; set; }
        public virtual ICollection<Module> CourseModules { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }

    }
}