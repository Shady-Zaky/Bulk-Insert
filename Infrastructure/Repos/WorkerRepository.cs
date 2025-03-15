using BulkInsertAPI.Infrastructure.DBContext;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BulkInsertAPI.Infrastructure.Repos
{
    public class WorkerRepository
    {
        private ApplicationDbContext _dbContext { get; }
        public WorkerRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<List<Worker>> GetWokersWithAssigmentsByCodes(IEnumerable<string> codes)
        {
            if (codes == null || !codes.Any())
            {
                return null;
            }
            return await _dbContext.Workers
                .Include(x => x.WorkerZoneAssignments)
                .Where(w => codes.Contains(w.Code)).ToListAsync();
        }
    }
}
