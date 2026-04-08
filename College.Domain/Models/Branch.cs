using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace College.Domain.Models
{
    public class Branch
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Eircode { get; set; }

        // Navigation property for the relationship defined in the assessment brief
        public virtual ICollection<Course> Course { get; set; } = new List<Course>();
    }
}