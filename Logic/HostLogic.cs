using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models;
using static Logic.Utilities.DisplayIdUtility;

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
        public override IBasicCrudDal<Host> GetBasicCrudDal() => _hostDal;
        
        /// <summary>
        /// Make sure display ID is not null or empty
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override Host Save(Host instance)
        {
            instance.DisplayId = GenerateDisplayId(instance);
            
            return base.Save(instance);
        }
    }
}