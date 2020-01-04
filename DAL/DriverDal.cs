using System.Linq;
using System.Threading.Tasks;
using DAL.Abstracts;
using DAL.Interfaces;
using DAL.Utilities;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL
{
    public class DriverDal : BasicCrudDalAbstract<Driver>, IDriverDal
    {
        private readonly EntityDbContext _dbContext;
        
        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        public DriverDal(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns database context
        /// </summary>
        /// <returns></returns>
        protected override DbContext GetDbContext()
        {
            return _dbContext;
        }

        /// <summary>
        /// Returns students entity
        /// </summary>
        /// <returns></returns>
        protected override DbSet<Driver> GetDbSet()
        {
            return _dbContext.Drivers;
        }
        
        public override async Task<Driver> Update(int id, Driver dto)
        {
            var entity = await Get(id);

            entity.Fullname = dto.Fullname;
            entity.Role = dto.Role;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.Capacity = dto.Capacity;
            entity.Navigator = dto.Navigator;
            entity.DisplayId = dto.DisplayId;
            entity.RequireNavigator = dto.RequireNavigator;
            entity.HaveChildSeat = dto.HaveChildSeat;
            
            return await base.Update(id, entity);
        }

        protected override IQueryable<Driver> Intercept<TQueryable>(TQueryable queryable)
        {
            return queryable
                .Include(x => x.Host)
                .Include(x => x.Host.Drivers)
                .Include(x => x.Students)
                .OrderBy(x => x.Fullname);
        }
    }
}