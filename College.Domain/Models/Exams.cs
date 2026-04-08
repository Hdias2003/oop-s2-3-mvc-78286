using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.Domain.Models
{
    public class Exams
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

        public DateTime ExamDate { get; set; }

        [Required]
        public int MaxScore { get; set; }

        [Required]
        public bool ResultsReleased { get; set; }

        // Navigation property for related results
        public virtual ICollection<ExamResults> ExamResults { get; set; } = new List<ExamResults>();
    }
}