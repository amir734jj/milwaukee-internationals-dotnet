using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Abstracts;
using DAL.Interfaces;
using DAL.Utilities;
using EntityUpdater.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL
{
    public class DriverDal : BasicCrudDalAbstract<Driver>, IDriverDal
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAssignmentUtility _mapper;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public DriverDal(EntityDbContext dbContext, IAssignmentUtility mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
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

        /// <summary>
        /// Returns IAssignmentUtility
        /// </summary>
        /// <returns></returns>
        protected override IAssignmentUtility Mapper()
        {
            return _mapper;
        }

        /// <summary>
        /// Override to include related entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<Driver> Get(int id)
        {
            return await GetDbSet().Include(x => x.Host).FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Override to include related entity
        /// </summary>
        /// <returns></returns>
        public override async Task<IEnumerable<Driver>> GetAll()
        {
            return await GetDbSet()
                .Include(x => x.Host)
                .Include(x => x.Host.Drivers)
                .Include(x => x.Students)
                .OrderBy(x => x.Fullname)
                .ToListAsync();
        }
    }
}