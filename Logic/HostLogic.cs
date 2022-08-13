using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Constants;
using Models.Entities;

namespace Logic
{
    public class HostLogic : BasicCrudLogicAbstract<Host>, IHostLogic
    {
        private readonly IBasicCrud<Host> _dal;
        private readonly GlobalConfigs _globalConfigs;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="globalConfigs"></param>
        public HostLogic(IEfRepository repository, GlobalConfigs globalConfigs)
        {
            _dal = repository.For<Host>();
            _globalConfigs = globalConfigs;
        }

        public override Task<Host> Save(Host instance)
        {
            // Set the year
            instance.Year = DateTime.UtcNow.Year;
            
            return base.Save(instance);
        }

        protected override IBasicCrud<Host> Repository()
        {
            return _dal;
        }

        public override async Task<IEnumerable<Host>> GetAll(string sortBy = null, bool? descending = null)
        {
            return (await base.GetAll(sortBy, descending)).Where(x => x.Year == _globalConfigs.YearValue);
        }
    }
}