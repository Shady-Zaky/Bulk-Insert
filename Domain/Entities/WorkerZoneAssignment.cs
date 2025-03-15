namespace Domain.Entities
{
    public class WorkerZoneAssignment
    {
        public int Id { get; set; }
        public int WorkerId { get; set; }
        public int ZoneId { get; set; }
        public DateOnly AssignmentDate { get; set; }

        public virtual Worker? Worker { get; set; }
        public virtual Zone? Zone { get; set; }
    }
}
