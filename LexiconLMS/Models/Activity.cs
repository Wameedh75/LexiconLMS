using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Starttime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public ActivityType Type { get; set; }

        
        public int ModuleId  { get; set; }

        //navigation prop
        public virtual Module Module { get; set; }
    }
}