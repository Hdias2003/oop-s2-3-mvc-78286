using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Required for IdentityUser

namespace College.Domain.Models
{
    public class StaffProfile
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        [ForeignKey("RoleName")]
        public virtual Role Role { get; set; }

        [Required]
        public string IdentityUserID { get; set; }

        // Proper foreign key relationship to the ASP.NET Identity table
        [ForeignKey("IdentityUserID")]
        public virtual IdentityUser User { get; set; }

        [Required]
        public string Name { get; set; }

        // Email and Phone are removed here because they exist in 'User'
        // Access them via: staffProfile.User.Email

        // Navigation property for assigned modules
        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}