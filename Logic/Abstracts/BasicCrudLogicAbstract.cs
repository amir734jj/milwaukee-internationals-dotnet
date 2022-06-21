using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Utilities;
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
        protected abstract EntityDbContext GetDbContext();

        protected abstract IEntityProfile<T> Profile();

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
            return await Profile().Include(GetDbContext().Set<T>().AsNoTracking()).ToListAsync();
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Get(int id)
        {
            return await Profile().Include(GetDbContext().Set<T>().AsNoTracking()).FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<T> Save(T instance)
        {
            var dbSet = GetDbContext().Set<T>();

            dbSet.Add(instance);

            await GetDbContext().SaveChangesAsync();

            return instance;
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Delete(int id)
        {
            var dbSet = GetDbContext().Set<T>();

            var entity = await dbSet.FirstAsync(x => x.Id == id);

            dbSet.Remove(entity);

            await GetDbContext().SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedInstance"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, T updatedInstance)
        {
            var dbSet = GetDbContext().Set<T>();

            var entity = await dbSet.FirstAsync(x => x.Id == id);

            Profile().Update(entity, updatedInstance);

            await GetDbContext().SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifyAction"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, Action<T> modifyAction)
        {
            try
            {
                var dbSet = GetDbContext().Set<T>();

                var entity = await dbSet.FirstAsync(x => x.Id == id);

                modifyAction(entity);

                await GetDbContext().SaveChangesAsync();

                return entity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}