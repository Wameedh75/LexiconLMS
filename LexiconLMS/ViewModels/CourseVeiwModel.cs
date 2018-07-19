using LexiconLMS.Models;
using System;
using System.Collections.Generic;

namespace LexiconLMS.ViewModels
{
    public class CourseVeiwModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public virtual ICollection<ApplicationUser> CourseStudents { get; set; }
        public virtual ICollection<Module> CourseModules { get; set; }
    }
}