using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.Domain.Models
{
    public class Course
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // Linked to the Branch entity
        [Required]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        // Updated to plural 'Modules' to fix CS1061 and match the Many-to-Many configuration
        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

        public virtual ICollection<Enrolments> Enrolments { get; set; } = new List<Enrolments>();
    }
}