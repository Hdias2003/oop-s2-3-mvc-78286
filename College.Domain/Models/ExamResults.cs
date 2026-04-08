using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.Domain.Models
{
    public class ExamResults
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [ForeignKey("StudentProfileId")]
        public virtual StudentProfile StudentProfile { get; set; }

        [Required]
        public int ExamsId { get; set; }

        [ForeignKey("ExamsId")]
        public virtual Exams Exams { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        public string Status { get; set; }

        // MaxScore is accessed via the Exams navigation property 
        // to maintain database integrity and normalization.
    }
}
