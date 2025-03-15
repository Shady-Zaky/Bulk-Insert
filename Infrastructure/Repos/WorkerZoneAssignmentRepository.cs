using BulkInsertAPI.Infrastructure.DBContext;
using Domain.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace BulkInsertAPI.Infrastructure.Repos
{
    public class WorkerZoneAssignmentRepository
    {
        private ApplicationDbContext _dbContext { get; }
        public WorkerZoneAssignmentRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<bool> BulkInsertAssigment(List<WorkerZoneAssignment> validAssignments)
        {
            if (validAssignments == null || !validAssignments.Any())
            {
                return false;
            }
            
            await _dbContext.BulkInsertAsync(validAssignments);
            return true;
        }
    }
}
