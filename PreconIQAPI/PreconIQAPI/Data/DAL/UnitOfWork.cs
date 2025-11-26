using PreconIQAPI.Data.DAL.Repositiories;

namespace PreconIQAPI.Data.DAL
{
    /// <summary>
    /// Class to control repositories for each entity type and share a DB Context.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PreconDbContext _dbContext;
        private bool _disposed = false;

        private IProjectRepository _projectRepository;
        private IProjectMetaDataRepository _projectMetaDataRepository;
        private IProjectNormalsRepository _projectNormalsRepository;
        private IProjectWindDataRepository _projectWindDataRepository;
        private IProjectWindBODDataRepository _projectWindBODDataRepository;

        /// <summary>
        /// Constructor to create a new instance.
        /// </summary>
        /// <param name="dbContext">RbaDbContext</param>
        public UnitOfWork(PreconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get the IProjectRepository to interact with Project records.
        /// </summary>
        public IProjectRepository ProjectRepository
        {
            get
            {
                _projectRepository ??= new ProjectRepository(_dbContext);
                return _projectRepository;
            }
        }

        /// <summary>
        /// Get the IProjectMetaDataRepository to interact with ProjectMetaData records.
        /// </summary>
        public IProjectMetaDataRepository ProjectMetaDataRepository
        {
            get
            {
                _projectMetaDataRepository ??= new ProjectMetaDataRepository(_dbContext);
                return _projectMetaDataRepository;
            }
        }

        /// <summary>
        /// Get the IProjectNormalsRepository to interact with ProjectNormals records.
        /// </summary>
        public IProjectNormalsRepository ProjectNormalsRepository
        {
            get
            {
                _projectNormalsRepository ??= new ProjectNormalsRepository(_dbContext);
                return _projectNormalsRepository;
            }
        }

        /// <summary>
        /// Get the IProjectWindDataRepository to interact with ProjectWindData records.
        /// </summary>
        public IProjectWindDataRepository ProjectWindDataRepository
        {
            get
            {
                _projectWindDataRepository ??= new ProjectWindDataRepository(_dbContext);
                return _projectWindDataRepository;
            }
        }

        /// <summary>
        /// Get the IProjectWindBODDataRepository to interact with ProjectWindBODData records.
        /// </summary>
        public IProjectWindBODDataRepository ProjectWindBODDataRepository
        {
            get
            {
                _projectWindBODDataRepository ??= new ProjectWindBODDataRepository(_dbContext);
                return _projectWindBODDataRepository;
            }
        }

        /// <summary>
        /// Method to save all changes currently pending for the DB Context.
        /// </summary>
        public void Save()
        {
            _ = _dbContext.SaveChanges();
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
