using PreconIQAPI.Models;

namespace PreconIQAPI.Data.DAL.Repositiories
{
    /// <summary>
    /// Class to control a ProjectMetaData Repository to allow interacting with ProjectMetaData records.
    /// </summary>
    public class ProjectMetaDataRepository : GenericRepository<ProjectMetaData>, IProjectMetaDataRepository
    {
        private readonly PreconDbContext _dbContext;
        private bool _disposed = false;

        /// <summary>
        /// Constructor to create a new instance.
        /// </summary>
        /// <param name="dbContext"></param>
        public ProjectMetaDataRepository(PreconDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Method to dispose the DB Context and free the resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Method to dispose the DB Context and free the resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
