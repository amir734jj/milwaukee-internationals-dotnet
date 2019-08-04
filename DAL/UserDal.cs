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
    public class UserDal : BasicCrudDalAbstract<User>, IUserDal
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAssignmentUtility _mapper;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        public UserDal(EntityDbContext dbContext, IAssignmentUtility mapper)
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
        protected override DbSet<User> GetDbSet()
        {
            return _dbContext.Users;
        }

        /// <summary>
        /// Returns IAssignmentUtility
        /// </summary>
        /// <returns></returns>
        protected override IAssignmentUtility Mapper()
        {
            return _mapper;
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await GetDbSet()
                .OrderBy(x => x.Fullname)
                .ToListAsync();
        }
    }
}