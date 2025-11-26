namespace PreconIQAPI.DTOs
{
    /// <summary>
    /// DTO class to hold ProjectWindDataDto information.
    /// </summary>
    public class ProjectWindDataDto
    {
        public int Id { get; set; }
        public DateTime ValidTime { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double U10 { get; set; }
        public double V10 { get; set; }
        public int Number { get; set; }
        public string? Expver { get; set; }
    }
}
