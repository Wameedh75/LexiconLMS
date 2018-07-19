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
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        [Required]
        public Course Course { get; set; }
    }
}