using System;
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
        protected abstract IBasicCrud<T> Repository();

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
            return await Repository().GetAll();
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Get(int id)
        {
            return await Repository().Get(id);
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<T> Save(T instance)
        {
            return await Repository().Save(instance);
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Delete(int id)
        {
            return await Repository().Delete(id);
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedInstance"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, T updatedInstance)
        {
            return await Repository().Update(id, updatedInstance);
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifyAction"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, Action<T> modifyAction)
        {
            return await Repository().Update(id, modifyAction);;
        }
    }
}