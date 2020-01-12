using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Interfaces;

namespace DAL.Abstracts
{
    public abstract class BasicCrudDalAbstract<T> : IBasicCrudDal<T> where T : class, IBasicModel
    {   
        /// <summary>
        /// Abstract to get database context
        /// </summary>
        /// <returns></returns>
        protected abstract DbContext GetDbContext();
        
        /// <summary>
        /// Abstract to get entity set
        /// </summary>
        /// <returns></returns>
        protected abstract DbSet<T> GetDbSet();

        /// <summary>
        /// Returns all entities
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await Intercept(GetDbSet()).ToListAsync();
        }

        /// <summary>
        /// Returns an entity given the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Get(int id)
        {
            return await Intercept(GetDbSet()).FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Saves an instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<T> Save(T instance)
        {
            await GetDbSet().AddAsync(instance);

            await GetDbContext().SaveChangesAsync();

            return instance;
        }

        /// <summary>
        /// Deletes entity given the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Delete(int id)
        {
            var entity = await Get(id);

            if (entity != null)
            {
                // Remove from persistence
                GetDbSet().Remove(entity);
                
                // Remove form DbContext
                GetDbContext().Remove(entity);
                
                await GetDbContext().SaveChangesAsync();
                
                return entity;
            }

            return null;
        }

        /// <summary>
        /// Handles update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, T entity)
        {
            if (entity != null)
            {
                // Save and dispose
                await GetDbContext().SaveChangesAsync();

                // Returns the updated entity
                return entity;
            }

            // Not found
            return null;
        }

        /// <summary>
        /// Handles manual update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifyAction"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, Action<T> modifyAction)
        {
            var entity = await Get(id);
                
            if (entity != null)
            {
                // Update
                modifyAction(entity);

                // Save and dispose
                await GetDbContext().SaveChangesAsync();

                // Returns the updated entity
                return entity;
            }

            // Not found
            return null;
        }
        
        /// <summary>
        /// Intercept the IQueryable to include
        /// </summary>
        /// <returns></returns>
        protected abstract IQueryable<T> Intercept<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<T>;
    }
}