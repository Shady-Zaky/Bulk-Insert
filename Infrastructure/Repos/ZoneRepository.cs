using BulkInsertAPI.Infrastructure.DBContext;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BulkInsertAPI.Infrastructure.Repos
{
    public class ZoneRepository
    {
        private ApplicationDbContext _dbContext { get; }
        public ZoneRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<List<Zone>> GetZonesByCodes(IEnumerable<string> codes)
        {
            if (codes == null || !codes.Any())
            {
                return null;
            }
            return await _dbContext.Zones
                .Where(w => codes.Contains(w.Code))
                .ToListAsync();
        }
    }
}
