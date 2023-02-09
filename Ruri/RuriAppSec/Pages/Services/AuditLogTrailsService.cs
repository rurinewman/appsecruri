using RuriAppSec.Model;

namespace RuriAppSec.Pages.Services
{
    public class AuditLogTrailsService
    {
        private readonly AuthDbContext _dbContext;

        public AuditLogTrailsService(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Track(string id, string details)
        {
            var auditLog = new AuditLogTrails
            {
                userID = id,
                Details = details,
                Date = DateTime.UtcNow
            };
            _dbContext.AuditLogTrails.Add(auditLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}

