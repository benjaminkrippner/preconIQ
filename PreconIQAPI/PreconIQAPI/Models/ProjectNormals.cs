using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PreconIQAPI.Models
{
    /// <summary>
    /// Model class for the ProjectNormals object. This controls the columns for the database table.
    /// </summary>
    public class ProjectNormals
    {
        public int Id { get; set; }
        [StringLength(50)]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public int Month { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? AvgHighTemp { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? AvgLowTemp { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? RecordAvgHighTemp { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? RecordAvgLowTemp { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? AvgRain { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? RecordHighAvgRain { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? RecordLowAvgRain { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? AvgSnow { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? RecordHighAvgSnow { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? RecordLowAvgSnow { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? AvgHighWind { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? AvgLowWind { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? RecordAvgHighWind { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? RecordAvgLowWind { get; set; }
    }
}
