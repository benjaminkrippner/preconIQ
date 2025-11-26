using PreconIQAPI.Models;

namespace PreconIQAPI.Data.DAL.Repositiories
{
    /// <summary>
    /// Interface to control a ProjectMetaData Repository to allow interacting with ProjectMetaData records.
    /// </summary>
    public interface IProjectMetaDataRepository : IGenericRepository<ProjectMetaData>, IDisposable
    {
    }
}
