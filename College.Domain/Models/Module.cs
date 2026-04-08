using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.Domain.Models
{
    public class Module
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        // Many-to-Many: A Module can belong to multiple Courses
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

        // Many-to-Many: A Module can be taught by multiple Staff members
        public virtual ICollection<StaffProfile> StaffProfiles { get; set; } = new List<StaffProfile>();

        // Navigation properties for related academic items
        public virtual ICollection<Assignment> Assignment { get; set; } = new List<Assignment>();
        public virtual ICollection<Exams> Exams { get; set; } = new List<Exams>();
        public virtual ICollection<Attendance> Attendance { get; set; } = new List<Attendance>();
    }
}