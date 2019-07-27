using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using DAL.Extensions;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Interfaces;

namespace DAL.Abstracts
{
    public abstract class BasicCrudDalAbstract<T> : IBasicCrudDal<T> where T : class, IBasicModel, IPerson
    {
        /// <summary>
        /// Abstract to get IMapper
        /// </summary>
        /// <returns></returns>
        protected abstract IMapper GetMapper();
        
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
        /// Returns all enities
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAll() => await GetDbSet().OrderBy(x => x.Fullname).ToListAsync();

        /// <summary>
        /// Returns an entity given the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Get(int id) => await GetDbSet().FirstOrDefaultCacheAsync(x => x.Id == id);

        /// <summary>
        /// Saves an instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<T> Save(T instance)
        {
            GetDbSet().Persist(GetMapper()).InsertOrUpdate(instance);
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
            var entity = GetDbSet().FirstOrDefault(x => x.Id == id);

            if (entity != null)
            {
                // Remove from persistence
                GetDbSet().Persist(GetMapper()).Remove(entity);
                
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
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, T instance)
        {
            if (instance != null)
            {
                // Update
                GetDbSet().Persist(GetMapper()).InsertOrUpdate(instance);
                    
                // Save and dispose
                await GetDbContext().SaveChangesAsync();

                // Returns the updated entity
                return instance;
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
        /// <exception cref="NotImplementedException"></exception>
        public virtual async Task<T> Update(int id, Action<T> modifyAction)
        {            
            var entity = GetDbSet().FirstOrDefaultCache(x => x.Id == id);
                
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
    }
}