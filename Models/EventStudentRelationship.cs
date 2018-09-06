namespace Models
{
    public class EventStudentRelationship
    {
        public int StudentId { get; set; }
        
        public Student Student { get; set; }

        public int EventId { get; set; }
        
        public Event Event { get; set; }
    }
}