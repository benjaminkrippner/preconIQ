using System.Linq.Expressions;

namespace PreconIQAPI.Data.DAL.Repositiories
{
    /// <summary>
    /// Interface to allow a Generic Repository for common DB methods to perform on an entity.
    /// </summary>
    /// <typeparam name="TEntity">The Model (entity) the repository will operate over.</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> Get(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Expression<Func<TEntity, object>>? includeProperties = null);
        public IEnumerable<TEntity> GetWithIncludes(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            IEnumerable<Expression<Func<TEntity, object>>>? includeProperties = null);
        public TEntity GetByID(object id);
        public void Insert(TEntity entity);
        public void Insert(IEnumerable<TEntity> entities);
        public void Delete(object id);
        public void Delete(TEntity entityToDelete);
        public void Update(TEntity entityToUpdate);
    }
}
