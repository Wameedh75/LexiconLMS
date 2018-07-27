using LexiconLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.ViewModels
{
    public class CourseVeiwModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        [UIHint("ShortDate")]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [UIHint("ShortDate")]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public virtual ICollection<ApplicationUser> CourseStudents { get; set; }
        public virtual ICollection<Module> CourseModules { get; set; }
    }
}