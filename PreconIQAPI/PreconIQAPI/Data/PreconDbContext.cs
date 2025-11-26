using PreconIQAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace PreconIQAPI.Data
{
    /// <summary>
    /// DBContext class to provide a database context for the Precon database.
    /// </summary>
    public class PreconDbContext : DbContext
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor to create a new PreconDbContext with the default options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="config"></param>
        public PreconDbContext(DbContextOptions<PreconDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        /// <summary>
        /// Parameterless constructor needed for mocking in unit tests.
        /// </summary>
        public PreconDbContext() { }

        // DbSet properties must be virtual to allow for mocking in unit tests.
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectMetaData> ProjectMetaData { get; set; }
        public virtual DbSet<ProjectWindBODData> ProjectWindBODData { get; set; }
        public virtual DbSet<ProjectNormals> ProjectNormals { get; set; }
        public virtual DbSet<ProjectWindData> ProjectWindData { get; set; }

        /// <summary>
        /// Method to specify the database tables that needed to be created.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use the configured schema so we aren't utilizing dbo.
            _ = modelBuilder.HasDefaultSchema(_config.GetValue<string>("BLAAppsPreconDatabaseSchema"));

            _ = modelBuilder.Entity<Project>().ToTable("Project");
            _ = modelBuilder.Entity<ProjectMetaData>().ToTable("ProjectMetaData");
            _ = modelBuilder.Entity<ProjectWindBODData>().ToTable("ProjectWindBODData");
            _ = modelBuilder.Entity<ProjectNormals>().ToTable("ProjectNormals");
            _ = modelBuilder.Entity<ProjectWindData>().ToTable("ProjectWindData");
        }
    }
}
