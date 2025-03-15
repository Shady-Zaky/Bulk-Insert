namespace Domain.Entities
{
    public class Zone
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public virtual ICollection<WorkerZoneAssignment>? WorkerZoneAssignments { get; set; }
    }
}
