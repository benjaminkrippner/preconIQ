using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PreconIQAPI.Models
{
    /// <summary>
    /// Model class for the ProjectMetaData object. This controls the columns for the database table.
    /// </summary>
    public class ProjectMetaData
    {
        public int Id { get; set; }
        [StringLength(50)]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        [StringLength(255)]
        public string? Address { get; set; }
        [StringLength(50)]
        public string? City { get; set; }
        [StringLength(50)]
        public string? State { get; set; }
        [StringLength(50)]
        public string? ZipCode { get; set; }
        [StringLength(50)]
        public string? County { get; set; }
        [StringLength(255)]
        public string? BidNumber { get; set; }
        [StringLength(50)]
        public string? ContractType { get; set; }
        public int? Duration { get; set; }
        public string? MarketSegment { get; set; }
        public string? OpportunityDescription { get; set; }
        [StringLength(50)]
        public string? BidAsCompany { get; set; }
        public bool IsParentOpportunity { get; set; }
        public int? ParentProjectId { get; set; }
        [ForeignKey("ParentProjectId")]
        public Project? ParentProject { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? EstimatedFinalContractRevenue { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? MarginPercent { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? MarginDollars { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Megawatts { get; set; }
        [StringLength(50)]
        public string? RevenueType { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Revenue { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? ProjectStartDate { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? ProjectCompletionDate { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? CloseDate { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? SubstantialCompletionDate { get; set; }
        [Column(TypeName = "decimal(10, 6)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(10, 6)")]
        public decimal? Longitude { get; set; }
    }
}
