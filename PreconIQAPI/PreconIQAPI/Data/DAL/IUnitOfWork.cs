using PreconIQAPI.Data.DAL.Repositiories;

namespace PreconIQAPI.Data.DAL
{
    /// <summary>
    /// Interface for IUnitOfWork to control repositories for each entity type and share a DB Context.
    /// </summary>
    public interface IUnitOfWork
    {
        IProjectRepository ProjectRepository { get; }
        IProjectMetaDataRepository ProjectMetaDataRepository { get; }
        IProjectNormalsRepository ProjectNormalsRepository { get; }
        IProjectWindDataRepository ProjectWindDataRepository { get; }
        IProjectWindBODDataRepository ProjectWindBODDataRepository { get; }
        public void Save();
    }
}
