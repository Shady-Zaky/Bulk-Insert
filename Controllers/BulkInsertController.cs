using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using BulkInsertAPI.Infrastructure.Services;

namespace BulkInsertAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BulkInsertController : Controller
    {
        private readonly AssigmentService _assigmentService;

        public BulkInsertController(AssigmentService assigmentService)
        {
            _assigmentService = assigmentService;
        }
        
        [HttpPost("upload")]
        public async Task<IActionResult> UploadCsvFile(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or not provided.");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only CSV files are allowed.");
            }

            //TODO: move csv parser to a separate service
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });
            var assigments = csv.GetRecords<WorkerZoneAssignmentRequest>().ToList();
            if (assigments.Count == 0)
            {
                return BadRequest("No records found in the CSV file.");
            }

            if (assigments.Count > 70000)
            {
                return BadRequest(" records exceeds the limit of 50000.");
            }

            var allowedMimeTypes = new List<string> { "text/csv", "application/vnd.ms-excel" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest("Invalid file format. Only CSV files are supported.");
            }

            var errorResponse = await _assigmentService.InsertAssigments(assigments);

            if (errorResponse.Count != 0)
                return BadRequest(errorResponse);

            return Ok(new { Message = "File uploaded and processed successfully." });

        }
    }

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
