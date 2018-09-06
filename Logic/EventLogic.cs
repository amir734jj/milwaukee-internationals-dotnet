using DAL.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models;

namespace Logic
{
    public class EventLogic : BasicCrudLogicAbstract<Event>, IEventLogic
    {
        private readonly IEventDal _eventDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="eventDal"></param>
        public EventLogic(IEventDal eventDal)
        {
            _eventDal = eventDal;
        }

        /// <summary>
        /// Returns instance of student DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudDal<Event> GetBasicCrudDal() => _eventDal;
    }
}