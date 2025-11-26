namespace PreconIQAPI.DTOs
{
    /// <summary>
    /// DTO class to hold ProjectNormalsDto information.
    /// </summary>
    public class ProjectNormalsDto
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public decimal? AvgHighTemp { get; set; }
        public decimal? AvgLowTemp { get; set; }
        public decimal? RecordAvgHighTemp { get; set; }
        public decimal? RecordAvgLowTemp { get; set; }
        public decimal? AvgRain { get; set; }
        public decimal? RecordHighAvgRain { get; set; }
        public decimal? RecordLowAvgRain { get; set; }
        public decimal? AvgSnow { get; set; }
        public decimal? RecordHighAvgSnow { get; set; }
        public decimal? RecordLowAvgSnow { get; set; }
        public decimal? AvgHighWind { get; set; }
        public decimal? AvgLowWind { get; set; }
        public decimal? RecordAvgHighWind { get; set; }
        public decimal? RecordAvgLowWind { get; set; }
    }
}
