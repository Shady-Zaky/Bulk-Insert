using CsvHelper.Configuration.Attributes;

namespace BulkInsertAPI.Models
{
    public class WorkerZoneAssignmentRequest
    {
        [Name("worker_code")]
        public string? WorkerCode { get; set; }

        [Name("zone_code")]
        public string? ZoneCode { get; set; }

        [Name("assignment_date")]
        public string? AssignmentDate { get; set; }
    }

    public class ErrorResponse
    {
        public object? Data { get; set; }
        public Dictionary<string, string>? Error { get; set; }
    }
}
