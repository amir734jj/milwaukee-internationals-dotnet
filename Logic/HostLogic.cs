using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Profiles;
using DAL.Utilities;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Constants;
using Models.Entities;

namespace Logic
{
    public class HostLogic : BasicCrudLogicAbstract<Host>, IHostLogic
    {
        private readonly EntityDbContext _dbContext;
        private readonly GlobalConfigs _globalConfigs;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="globalConfigs"></param>
        public HostLogic(EntityDbContext dbContext, GlobalConfigs globalConfigs)
        {
            _dbContext = dbContext;
            _globalConfigs = globalConfigs;
        }

        public override Task<Host> Save(Host instance)
        {
            // Set the year
            instance.Year = DateTime.UtcNow.Year;
            
            return base.Save(instance);
        }
        
        protected override EntityDbContext GetDbContext()
        {
            return _dbContext;
        }

        protected override IEntityProfile<Host> Profile()
        {
            return new HostProfile();
        }

        public override async Task<IEnumerable<Host>> GetAll()
        {
            return (await base.GetAll()).Where(x => x.Year == _globalConfigs.YearValue);
        }
    }
}