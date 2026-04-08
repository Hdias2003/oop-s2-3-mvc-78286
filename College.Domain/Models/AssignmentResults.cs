using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.Domain.Models
{
    public class AssignmentResults
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [ForeignKey("AssignmentId")]
        public virtual Assignment Assignment { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [ForeignKey("StudentProfileId")]
        public virtual StudentProfile StudentProfile { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Result { get; set; }

        // Logic Note: MaxScore is accessed via the virtual Assignment property.
        // This ensures that if an Assignment's MaxScore changes, the results
        // stay logically linked to the new value automatically.
    }
}