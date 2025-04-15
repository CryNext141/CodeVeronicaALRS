using ALRS.Data;
using ALRS.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

public class AuditActionFilter : IAsyncActionFilter
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuditActionFilter> _logger;

    public AuditActionFilter(ApplicationDbContext context, ILogger<AuditActionFilter> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var requestData = JsonSerializer.Serialize(context.ActionArguments);

        var executedContext = await next();

        var auditEntry = new AuditLog
        {
            ActionName = actionName,
            RequestData = requestData,
            ResponseStatusCode = context.HttpContext.Response.StatusCode,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            _context.AuditLogs.Add(auditEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during saving the audit.");
        }
    }
}
