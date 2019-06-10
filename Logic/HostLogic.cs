using System;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic
{
    public class HostLogic : BasicCrudLogicAbstract<Host>, IHostLogic
    {
        private readonly IHostDal _hostDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="hostDal"></param>
        public HostLogic(IHostDal hostDal)
        {
            _hostDal = hostDal;
        }

        /// <summary>
        /// Returns instance of student DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudDal<Host> GetBasicCrudDal() => _hostDal;

        public override Task<Host> Save(Host instance)
        {
            // Set the year
            instance.Year = DateTime.Now.Year;
            
            return base.Save(instance);
        }
    }
}