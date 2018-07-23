using System;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models
{
    public class Module
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Display(Name = "Start date")]
        [UIHint("ShortDate")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        [UIHint("ShortDate")]
        public DateTime EndDate { get; set; }

        public int CourseId  { get; set; }

        //navigation prop
        public virtual Course Course { get; set; }
    }
}