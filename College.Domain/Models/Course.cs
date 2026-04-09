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
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)] // Ensures the HTML view shows a date picker
        [Column(TypeName = "date")] // Maps precisely to SQL 'date' instead of 'datetime2'
        public DateOnly StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateOnly EndDate { get; set; }

        // Linked to the Branch entity
        [Required]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

        public virtual ICollection<Enrolments> Enrolments { get; set; } = new List<Enrolments>();
    }
}