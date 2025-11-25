using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP2139_ICE.Models
{
    public class ProjectTask
    {
        [Key]
        public int ProjectTaskId { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        // FK to Project
        public int ProjectId { get; set; }

        // Navigation property → this MUST match Project.cs
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }
    }
}