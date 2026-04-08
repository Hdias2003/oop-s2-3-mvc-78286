using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Required for IdentityUser link

namespace College.Domain.Models
{
    public class StudentProfile
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; } = "Student";

        [ForeignKey("RoleName")]
        public virtual Role Role { get; set; }

        [Required]
        public string IdentityUserID { get; set; }

        // Establishing the formal relationship to the AspNetUsers table
        [ForeignKey("IdentityUserID")]
        public virtual IdentityUser User { get; set; }

        [Required]
        public string Name { get; set; }

        // Email and Phone are removed here because they exist in 'User' (AspNetUsers)
        // Access them via: studentProfile.User.Email

        // Navigation properties
        public virtual ICollection<Enrolments> Enrolments { get; set; } = new List<Enrolments>();
        public virtual ICollection<Attendance> Attendance { get; set; } = new List<Attendance>();
        public virtual ICollection<AssignmentResults> AssignmentResults { get; set; } = new List<AssignmentResults>();
        public virtual ICollection<ExamResults> ExamResults { get; set; } = new List<ExamResults>();
    }
}