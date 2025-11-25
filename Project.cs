using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace COMP2139_ICE.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string? Status { get; set; }

        public List<ProjectTask>? Tasks { get; set; }
    }
}