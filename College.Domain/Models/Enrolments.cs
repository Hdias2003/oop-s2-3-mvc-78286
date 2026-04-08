using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.Domain.Models
{
    public class Enrolments
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [ForeignKey("StudentProfileId")]
        public virtual StudentProfile StudentProfile { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; }
        // Expected values: "Active", "Graduated", "Withdrawn", "Deferred"
    }
}