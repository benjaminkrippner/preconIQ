namespace PreconIQAPI.DTOs
{
    /// <summary>
    /// DTO class to hold ProjectMetaDataDto information.
    /// </summary>
    public class ProjectMetaDataDto
    {
        public int Id { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? County { get; set; }
        public string? BidNumber { get; set; }
        public string? ContractType { get; set; }
        public int? Duration { get; set; }
        public string? MarketSegment { get; set; }
        public string? OpportunityDescription { get; set; }
        public string? BidAsCompany { get; set; }
        public bool IsParentOpportunity { get; set; }
        public int? ParentProjectId { get; set; }
        public decimal? EstimatedFinalContractRevenue { get; set; }
        public decimal? MarginPercent { get; set; }
        public decimal? MarginDollars { get; set; }
        public decimal? Megawatts { get; set; }
        public string? RevenueType { get; set; }
        public decimal? Revenue { get; set; }
        public DateOnly? ProjectStartDate { get; set; }
        public DateOnly? ProjectCompletionDate { get; set; }
        public DateOnly? CloseDate { get; set; }
        public DateOnly? SubstantialCompletionDate { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
