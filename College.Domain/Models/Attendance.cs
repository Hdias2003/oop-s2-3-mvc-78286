using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.Domain.Models
{
    public class Attendance
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [ForeignKey("StudentProfileId")]
        public virtual StudentProfile StudentProfile { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        // Note: Total calculations (ClassesTaken, ClassesMissed, ClassesTotal) 
        // should now be performed using LINQ queries in your Service/Logic layer 
        // to ensure 100% mathematical accuracy based on these session records.
    }
}