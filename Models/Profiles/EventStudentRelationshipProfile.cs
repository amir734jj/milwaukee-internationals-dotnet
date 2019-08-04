using EntityUpdater.Abstracts;
using Models.Entities;

namespace Models.Profiles
{
    public class EventStudentRelationshipProfile : AssignmentProfile<EventStudentRelationship>
    {
        public EventStudentRelationshipProfile()
        {
            Map(x => x.Event)
                .Then(x => x.EventId)
                .Then(x => x.Student)
                .Then(x => x.StudentId);
        }
    }
}