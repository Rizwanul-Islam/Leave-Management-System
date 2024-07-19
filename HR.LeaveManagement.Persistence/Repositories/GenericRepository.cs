using HR.LeaveManagement.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Persistence.Repositories
{
    /// <summary>
    /// Generic repository for CRUD operations.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly LeaveManagementDbContext _dbContext;

        public GenericRepository(LeaveManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a new entity to the context.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        /// <returns>The added entity.</returns>
        public async Task<T> Add(T entity)
        {
            await _dbContext.AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Deletes an entity from the context.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        public async Task Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Checks if an entity with the given id exists.
        /// </summary>
        /// <param name="id">Id of the entity to check.</param>
        /// <returns>True if entity exists, otherwise false.</returns>
        public async Task<bool> Exists(int id)
        {
            var entity = await Get(id);
            return entity != null;
        }

        /// <summary>
        /// Retrieves an entity by its id.
        /// </summary>
        /// <param name="id">Id of the entity to retrieve.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        public async Task<T> Get(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Retrieves all entities.
        /// </summary>
        /// <returns>A read-only list of entities.</returns>
        public async Task<IReadOnlyList<T>> GetAll()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        public async Task Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
