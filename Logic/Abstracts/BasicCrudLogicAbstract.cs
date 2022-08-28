﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DAL.Interfaces;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;
using Models.Entities;
using Models.Interfaces;

namespace Logic.Abstracts
{
    public abstract class BasicCrudLogicAbstract<T> : IBasicCrudLogic<T> where T : class, IEntity
    {
        protected abstract IBasicCrud<T> Repository();
        
        protected abstract IApiEventService ApiEventService();

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
        /// <param name="sortBy"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAll(string sortBy = null, bool? descending = null)
        {
            var result = await Repository().GetAll();

            await ApiEventService().RecordEvent($"Queried all {typeof(T).Name}");

            if (string.IsNullOrEmpty(sortBy))
            {
                return result;
            }

            var property = typeof(T).GetProperties()
                .Where(x => x.CanRead && x.CanWrite)
                .FirstOrDefault(x => x.Name.Equals(sortBy, StringComparison.OrdinalIgnoreCase));

            if (property == null)
            {
                return result;
            }

            var propertyAccessFunc = ToLambda(property).Compile();

            if (!descending.HasValue || !descending.Value)
            {
                return result.OrderBy(propertyAccessFunc).ToList();
            }

            return result.OrderByDescending(propertyAccessFunc).ToList();
        }
        
        private static Expression<Func<T, object>> ToLambda(PropertyInfo propertyInfo)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyInfo);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);            
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Get(int id)
        {
            var result =  await Repository().Get(id);
            
            await ApiEventService().RecordEvent($"Queried ID: {id} of {typeof(T).Name}");

            return result;
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<T> Save(T instance)
        {
            var result = await Repository().Save(instance);
         
            await ApiEventService().RecordEvent($"Save new {typeof(T).Name} => {instance}");

            return result;
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Delete(int id)
        {
            var result =await Repository().Delete(id);
            
            await ApiEventService().RecordEvent($"Delete {typeof(T).Name} with ID: {id}");

            return result;
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedInstance"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, T updatedInstance)
        {
            var result = await Repository().Update(id, updatedInstance);
            
            await ApiEventService().RecordEvent($"Update {typeof(T).Name} with ID: {id} => {updatedInstance}");

            return result;
        }

        /// <summary>
        /// Call forwarding
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifyAction"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(int id, Action<T> modifyAction)
        {
            var result = await Repository().Update(id, modifyAction);;
            
            await ApiEventService().RecordEvent($"Updated {typeof(T).Name} with ID: {id} => {result}");

            return result;
        }
    }
}