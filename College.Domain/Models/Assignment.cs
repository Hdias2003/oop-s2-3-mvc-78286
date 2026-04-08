using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.Domain.Models
{
    public class Assignment
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int MaxScore { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DueDate { get; set; }

        [Required]
        public bool Visibility { get; set; }
    }
}