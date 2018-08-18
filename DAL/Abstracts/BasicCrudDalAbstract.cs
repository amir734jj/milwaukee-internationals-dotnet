using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
        public virtual IEnumerable<T> GetAll() => GetDbSet().OrderBy(x => x.Fullname).ToList();

        /// <summary>
        /// Returns an entity given the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T Get(int id) => GetDbSet().FirstOrDefaultCache(x => x.Id == id);

        /// <summary>
        /// Saves an instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual T Save(T instance)
        {
            GetDbSet().Add(instance);
            GetDbContext().SaveChanges();
            return instance;
        }

        /// <summary>
        /// Deletes enitity given the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T Delete(int id)
        {
            var instance = GetDbSet().FirstOrDefaultCache(x => x.Id == id);

            if (instance != null)
            {
                GetDbSet().Remove(instance);
                GetDbContext().SaveChanges();
                return instance;
            }

            return null;
        }

        /// <summary>
        /// Handles update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public T Update(int id, T instance)
        {
            var entity = GetDbSet().FirstOrDefault(x => x.Id == id);
                
            if (entity != null)
            {
                // Being tracking
                GetDbContext().Update(entity);

                // Update
                GetMapper().Map(instance, entity);
                    
                // Save and dispose
                GetDbContext().SaveChanges();

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
        public T Update(int id, Action<T> modifyAction)
        {            
            var entity = GetDbSet().FirstOrDefault(x => x.Id == id);
                
            if (entity != null)
            {
                // Update
                modifyAction(entity);
                    
                // Save and dispose
                GetDbContext().SaveChanges();

                // Returns the updated entity
                return entity;
            }

            // Not found
            return null;
        }
    }
}