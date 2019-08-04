
using EntityUpdater.Abstracts;
using Models.Entities;

namespace Models.Profiles
{
    public class EventProfile : AssignmentProfile<Event>
    {
        public EventProfile()
        {
            Map(x => x.Name)
                .Then(x => x.Address)
                .Then(x => x.Description)
                .Then(x => x.DateTime);
        }
    }
}