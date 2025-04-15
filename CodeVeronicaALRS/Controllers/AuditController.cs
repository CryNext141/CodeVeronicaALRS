using ALRS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeVeronicaALRS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetAuditHistory(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _context.AuditLogs.AsQueryable();
            if (from.HasValue)
            {
                query = query.Where(audit => audit.Timestamp >= from.Value);
            }
            if (to.HasValue)
            {
                query = query.Where(audit => audit.Timestamp <= to.Value);
            }

            var totalRecords = await query.CountAsync();

            var auditHistory = await query
                .OrderByDescending(audit => audit.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = auditHistory.Select(a => new
            {
                auditLogId = a.AuditLogId,
                actionName = a.ActionName,
                action = a.Action,
                responseStatusCode = a.ResponseStatusCode,
                date = a.Timestamp.ToString("yyyy-MM-dd"),
                time = a.Timestamp.ToString("HH:mm:ss")
            });

            return Ok(new
            {
                totalRecords,
                pageNumber,
                pageSize,
                data = result
            });
        }
    }
}
