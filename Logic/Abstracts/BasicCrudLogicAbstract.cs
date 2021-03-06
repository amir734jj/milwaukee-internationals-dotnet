﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;
using Models.Entities;
using Models.Interfaces;

namespace Logic.Abstracts
{
    public abstract class BasicCrudLogicAbstract<T> : IBasicCrudLogic<T> where T : class, IEntity
    {
        /// <summary>
        /// Returns instance of basic DAL
        /// </summary>
        /// <returns></returns>
        protected abstract IBasicCrud<T> GetBasicCrudDal();

        public async Task<IEnumerable<T>> GetAll(int year)
        {
            var result = (await GetAll()).ToList();

            return result switch
            {
                List<Driver> drivers => drivers.Where(x => x.Year == year).Cast<T>(),
                List<Host> hosts => hosts.Where(x => x.Year == year).Cast<T>(),
                List<Student> students => students.Where(x => x.Year == year).Cast<T>(),
                List<User> users => users.Cast<T>(),
                _ => result
            };
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await GetBasicCrudDal().GetAll();
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Get(int id)
        {
            return await GetBasicCrudDal().Get(id);
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<T> Save(T instance)
        {
            return await GetBasicCrudDal().Save(instance);
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Delete(int id)
        {
            return await GetBasicCrudDal().Delete(id);
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedInstance"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, T updatedInstance)
        {
            return await GetBasicCrudDal().Update(id, updatedInstance);
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifyAction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual async Task<T> Update(int id, Action<T> modifyAction)
        {
            await using var session = GetBasicCrudDal();
            
            var entity = await session.Get(id);

            modifyAction(entity);

            return await session.Update(id, entity);
        }
    }
}