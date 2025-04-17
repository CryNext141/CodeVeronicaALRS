using ALRS.Data;
using ALRS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


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
        var originalActionName = context.ActionDescriptor.DisplayName;
        string methodName = originalActionName;
        if (!string.IsNullOrEmpty(originalActionName))
        {
            int lastDot = originalActionName.LastIndexOf('.');
            if (lastDot >= 0)
            {
                int spaceIndex = originalActionName.IndexOf(' ', lastDot);
                if (spaceIndex >= 0)
                    methodName = originalActionName.Substring(lastDot + 1, spaceIndex - lastDot - 1);
                else
                    methodName = originalActionName.Substring(lastDot + 1);
            }
        }

        var executedContext = await next();

        string description;
        string actionShort;

        switch (methodName)
        {
            case "CreateAlert":
                {
                    int? alertId = null;
                    if (executedContext.Result is ObjectResult objResult && objResult.Value != null)
                    {
                        try
                        {
                            dynamic resultValue = objResult.Value;
                            alertId = resultValue.alertId;
                        }
                        catch { }
                    }
                    description = alertId.HasValue
                        ? $"Created new alert with id {alertId.Value}"
                        : "Created new alert";
                    actionShort = "Create Alert";
                }
                break;
            case "GetAlertById":
                {
                    int? alertId = null;
                    if (context.ActionArguments.TryGetValue("id", out var idVal))
                    {
                        try { alertId = Convert.ToInt32(idVal); } catch { }
                    }
                    description = alertId.HasValue
                        ? $"Retrieved alert details with id {alertId.Value}"
                        : "Retrieved alert details";
                    actionShort = "Get Alert By Id";
                }
                break;
            case "GetUserReportsForAlert":
                {
                    int? alertId = null;
                    if (context.ActionArguments.TryGetValue("alertId", out var alertIdVal))
                    {
                        try { alertId = Convert.ToInt32(alertIdVal); } catch { }
                    }
                    description = alertId.HasValue
                        ? $"Retrieved user reports for alert with id {alertId.Value}"
                        : "Retrieved user reports for alert";
                    actionShort = "Get User Reports For Alert";
                }
                break;
            case "UpdateAlert":
                {
                    int? alertId = null;
                    if (context.ActionArguments.TryGetValue("id", out var idVal))
                    {
                        try { alertId = Convert.ToInt32(idVal); } catch { }
                    }
                    description = alertId.HasValue
                        ? $"Updated alert with id {alertId.Value}"
                        : "Updated alert";
                    actionShort = "Update Alert";
                }
                break;
            case "CloseAlert":
                {
                    int? alertId = null;
                    if (context.ActionArguments.TryGetValue("id", out var idVal))
                    {
                        try { alertId = Convert.ToInt32(idVal); } catch { }
                    }
                    description = alertId.HasValue
                        ? $"Closed alert with id {alertId.Value}"
                        : "Closed alert";
                    actionShort = "Close Alert";
                }
                break;
            case "GetAllAlerts":
                description = "Retrieved list of alerts";
                actionShort = "Get All Alerts";
                break;
            case "Register":
                description = "Registered new user";
                actionShort = "Register";
                break;
            case "Login":
                description = "User logged in";
                actionShort = "Login";
                break;
            case "Logout":
                description = "User logged out";
                actionShort = "Logout";
                break;
            case "CreateCitizenReport":
                {
                    int? reportId = null;
                    if (executedContext.Result is ObjectResult objectResult && objectResult.Value != null)
                    {
                        try
                        {
                            dynamic resultValue = objectResult.Value;
                            reportId = resultValue.reportId;
                        }
                        catch { }
                    }
                    description = reportId.HasValue
                        ? $"Created citizen report with id {reportId.Value}"
                        : "Created citizen report";
                    actionShort = "Create Citizen Report";
                }
                break;
            case "GetAllUsers":
                description = "Retrieved list of users";
                actionShort = "Get All Users";
                break;
            case "GetUserById":
                {
                    int? userId = null;
                    if (context.ActionArguments.TryGetValue("id", out var idVal))
                    {
                        try { userId = Convert.ToInt32(idVal); } catch { }
                    }
                    description = userId.HasValue
                        ? $"Retrieved user details with id {userId.Value}"
                        : "Retrieved user details";
                    actionShort = "Get User By Id";
                }
                break;
            case "DeleteUser":
                {
                    int? userId = null;
                    if (context.ActionArguments.TryGetValue("id", out var idVal))
                    {
                        try { userId = Convert.ToInt32(idVal); } catch { }
                    }
                    description = userId.HasValue
                        ? $"Deleted user with id {userId.Value}"
                        : "Deleted user";
                    actionShort = "Delete User";
                }
                break;
            default:
                description = $"Performed action: {methodName}";
                actionShort = methodName;
                break;
        }

        var auditEntry = new AuditLog
        {
            ActionName = description,
            Action = actionShort,
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
            _logger.LogError(ex, "Error occurred while saving the audit log.");
        }
    }
}
