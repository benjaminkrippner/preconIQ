using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PreconIQAPI.Data.DAL.Repositiories
{
    /// <summary>
    /// Class to allow a Generic Repository for common DB methods to perform on an entity.
    /// </summary>
    /// <typeparam name="TEntity">The Model (entity) the repository will operate over.</typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly PreconDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Constructor to create a new instance.
        /// </summary>
        /// <param name="dbContext"></param>
        public GenericRepository(PreconDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        /// <summary>
        /// Method to generically get records for an entity based on the optional parameters.
        /// </summary>
        /// <param name="filter">Expression<Func<TEntity, bool>> - lambda to filter data by.</param>
        /// <param name="orderBy">Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> - lambda to order data by.</param>
        /// <param name="includeProperty">Expression<Func<TEntity, object>> - single lambda expression to Include in the results.</param>
        /// <returns>IEnumerable<TEntity> - List of entities retrieved for the query.</returns>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, object>>? includeProperty = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperty is not null)
            {
                query = query.Include(includeProperty);
            }

            return orderBy != null ? orderBy(query).ToList() : (IEnumerable<TEntity>)query.ToList();
        }

        /// <summary>
        /// Method to generically get records for an entity based on the optional parameters.
        /// </summary>
        /// <param name="filter">Expression<Func<TEntity, bool>> - lambda to filter data by.</param>
        /// <param name="orderBy">Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> - lambda to order data by.</param>
        /// <param name="includeProperties">IEnumerable<Expression<Func<TEntity, object>>> - list of lambda expressions to Include in the results.</param>
        /// <returns>IEnumerable<TEntity> - List of entities retrieved for the query.</returns>
        public virtual IEnumerable<TEntity> GetWithIncludes(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            IEnumerable<Expression<Func<TEntity, object>>>? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties is not null)
            {
                foreach (Expression<Func<TEntity, object>> expression in includeProperties)
                {
                    query = query.Include(expression);
                }
            }

            return orderBy != null ? orderBy(query).ToList() : (IEnumerable<TEntity>)query.ToList();
        }

        /// <summary>
        /// Method to get an entity for a specific ID.
        /// </summary>
        /// <param name="id">object - Id to get the record for.</param>
        /// <returns>TEntity - entity that was queried.</returns>
        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// Method to insert a single entity.
        /// </summary>
        /// <param name="entity">TEntity - entity to be inserted.</param>
        public virtual void Insert(TEntity entity)
        {
            _ = _dbSet.Add(entity);
        }

        /// <summary>
        /// Method to insert a collection of entities.
        /// </summary>
        /// <param name="entities">IEnumerable<TEntity> - collection of entities to be inserted.</param>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        /// <summary>
        /// Method to delete an entity for a specific ID.
        /// </summary>
        /// <param name="id">object - Id to delete the record for.</param>
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Method to delete an entity for a specific object.
        /// </summary>
        /// <param name="entityToDelete">TEntity - entity to be deleted.</param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _ = _dbSet.Attach(entityToDelete);
            }

            _ = _dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Method to udpate a specific object.
        /// </summary>
        /// <param name="entityToUpdate">TEntity - entity to be updated.</param>
        public virtual void Update(TEntity entityToUpdate)
        {
            _ = _dbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
