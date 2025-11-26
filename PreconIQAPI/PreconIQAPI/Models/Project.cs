using System.ComponentModel.DataAnnotations;

namespace PreconIQAPI.Models
{
    /// <summary>
    /// Model class for the Project object. This controls the columns for the database table.
    /// </summary>
    public class Project
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string OpportunityId { get; set; }
        [StringLength(255)]
        public string? OpportunityName { get; set; }
        [StringLength(255)]
        public string? OpportunityLegalContractProjectName { get; set; }
        [StringLength(255)]
        public string? OpportunityStageName { get; set; }
        public int? ProjectNumber { get; set; }
        [StringLength(255)]
        public string? AccountName { get; set; }
        public bool IsWon { get; set; }
        public bool IsClosed { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
