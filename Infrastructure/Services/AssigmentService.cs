using BulkInsertAPI.Controllers;
using CsvHelper.Configuration;
using CsvHelper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BulkInsertAPI.Infrastructure.Repos;
using System.Threading.Tasks;

namespace BulkInsertAPI.Infrastructure.Services
{
    public class AssigmentService
    {
        private readonly WorkerRepository _workerRepository;
        private readonly ZoneRepository _zoneRepository;
        private readonly WorkerZoneAssignmentRepository  _workerZoneAssignmentRepository;

        public AssigmentService(WorkerRepository workerRepository, ZoneRepository zoneRepository, WorkerZoneAssignmentRepository workerZoneAssignmentRepository)
        {
            _workerRepository = workerRepository;
            _zoneRepository = zoneRepository;
            _workerZoneAssignmentRepository = workerZoneAssignmentRepository;
        }
        public async Task<List<ErrorResponse>> InsertAssigments(List<WorkerZoneAssignmentRequest> assigments)
        {
            var validAssignments = new List<WorkerZoneAssignment>();
            var errorResponse = new List<ErrorResponse>();
            var seen = new HashSet<string>();

            var workers = await _workerRepository.GetWokersWithAssigmentsByCodes(assigments.Select(x => x.WorkerCode));
            var zones = await _zoneRepository.GetZonesByCodes(assigments.Select(x => x.ZoneCode));

            //TODO: move basic csv file validation to entity class
            foreach (var assigment in assigments)
            {
                var errors = new Dictionary<string, string>();
                var zone = zones.FirstOrDefault(z => z.Code == assigment.ZoneCode);
                var worker = workers.FirstOrDefault(w => w.Code == assigment.WorkerCode);

                if (assigment.WorkerCode?.Length > 10)
                {
                    errors["worker_code"] = "Worker code exceeds 10 characters";
                }

                if (assigment.ZoneCode?.Length > 10)
                {
                    errors["zone_code"] = "exceeds 10 characters.";
                }

                var hasValidDate = DateOnly.TryParseExact(assigment.AssignmentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate);

                if (!hasValidDate)
                {
                    errors["assigment_date"] = "invalid date format";
                }

                if (zone == null)
                {
                    errors["zone_code"] = " Zone does not exist";
                }

                var seenKey = $"{assigment.WorkerCode}-{assigment.ZoneCode}-{assigment.AssignmentDate}";
                if (!seen.Add(seenKey))
                {
                    errors["rowError"] = "Duplicate row in file";
                }

                if (worker == null)
                {
                    errors["worker_code"] = "Worker does not exist";
                }

                if (hasValidDate)
                {
                    var existAssigment = worker?.WorkerZoneAssignments!.FirstOrDefault(a => a.AssignmentDate == parsedDate);
                    if (worker?.WorkerZoneAssignments?.Count != 0 && existAssigment != null)
                    {
                        errors["rowError"] = "Worker assigned to multiple zones on the same date";
                    }

                    if (worker?.WorkerZoneAssignments?.Count != 0 && existAssigment != null && existAssigment.ZoneId == zone?.Id)
                    {
                        errors["rowError"] = "Assignment already exists in worker_zone_assignment table";
                    }
                }
                    

                if (errors.Count != 0)
                {
                    errorResponse.Add(new ErrorResponse
                    {
                        Data = assigment,
                        Error = errors
                    });
                }
                else
                {
                    validAssignments.Add(new WorkerZoneAssignment
                    {
                        WorkerId = worker.Id,
                        ZoneId = zone.Id,
                        AssignmentDate = parsedDate
                    });
                }
            }
            
        await _workerZoneAssignmentRepository.BulkInsertAssigment(validAssignments);
        return errorResponse;
        }
    }
}
