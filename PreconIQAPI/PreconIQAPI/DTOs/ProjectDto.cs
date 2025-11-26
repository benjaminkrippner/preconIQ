namespace PreconIQAPI.DTOs
{
    /// <summary>
    /// DTO class to hold ProjectDto information.
    /// </summary>
    public class ProjectDto
    {
        public int Id { get; set; }
        public string OpportunityId { get; set; }
        public string? OpportunityName { get; set; }
        public string? OpportunityLegalContractProjectName { get; set; }
        public string? OpportunityStageName { get; set; }
        public int? ProjectNumber { get; set; }
        public string? AccountName { get; set; }
        public bool IsWon { get; set; }
        public bool IsClosed { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
