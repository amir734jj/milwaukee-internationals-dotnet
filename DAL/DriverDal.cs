using System.Linq;
using AutoMapper;
using DAL.Abstracts;
using DAL.Extensions;
using DAL.Interfaces;
using DAL.Utilities;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL
{
    public class DriverDal : BasicCrudDalAbstract<Driver>, IDriverDal
    {
        private readonly EntityDbContext _dbContext;
        
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public DriverDal(EntityDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns IMapper
        /// </summary>
        /// <returns></returns>
        public override IMapper GetMapper() => _mapper;
        
        /// <summary>
        /// Returns database context
        /// </summary>
        /// <returns></returns>
        public override DbContext GetDbContext() => _dbContext;

        /// <summary>
        /// Returns students entity
        /// </summary>
        /// <returns></returns>
        protected override DbSet<Driver> GetDbSet() => _dbContext.Drivers;

        /// <summary>
        /// Update while eager loading the PK/FK
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedInstance"></param>
        /// <returns></returns>
        public override Driver Update(int id, Driver updatedInstance)
        {
            var instance = GetDbSet().Include(x => x.Host).FirstOrDefault(x => x.Id == id);
            
            return base.Update(id, instance, updatedInstance);
        }
    }
}