using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity; // Required for IdentityRole

namespace College.Domain.Models
{
    // Inheriting from IdentityRole<string> ensures compatibility with AspNetRoles
    public class Role : IdentityRole
    {
        // The 'Name' property is already provided by IdentityRole.
        // We can add a constructor to easily create the roles you need.
        public Role() : base() { }

        public Role(string roleName) : base(roleName)
        {
            Name = roleName;
        }

        // Navigation property for users assigned to this role
        // Identity handles the many-to-many relationship via AspNetUserRoles
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();
    }
}