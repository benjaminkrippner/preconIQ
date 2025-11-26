using PreconIQAPI.Models;

namespace PreconIQAPI.Data.DAL.Repositiories
{
    /// <summary>
    /// Interface to control a Project Repository to allow interacting with Project records.
    /// </summary>
    public interface IProjectRepository : IGenericRepository<Project>, IDisposable
    {
    }
}
