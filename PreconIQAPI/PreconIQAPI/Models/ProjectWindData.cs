using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PreconIQAPI.Models
{
    /// <summary>
    /// Represents wind data associated with a project.
    /// </summary>
    public class ProjectWindData
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;

        public DateTime ValidTime { get; set; }

        [Column(TypeName = "decimal(10, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(10, 6)")]
        public decimal Longitude { get; set; }

        public double U10 { get; set; }

        public double V10 { get; set; }

        public int Number { get; set; }

        [StringLength(50)]
        public string? Expver { get; set; }
    }
}
