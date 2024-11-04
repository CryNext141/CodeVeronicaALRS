using ALRS.Data;
using ALRS.DTO;
using ALRS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALRS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AlertsController> _logger;

        public AlertsController(ApplicationDbContext context, ILogger<AlertsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "1")]
        [HttpPost("alert/create")]
        public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto dto)
        {
            _logger.LogInformation("Entering {Action} with alert data: {@Dto}", nameof(CreateAlert), dto);

            try
            {
                var alert = new Alerts
                {
                    VictimName = dto.VictimName,
                    VictimAge = dto.VictimAge,
                    CrimeLocation = dto.CrimeLocation,
                    CrimeDate = dto.CrimeDate,
                    CrimeStatus = dto.CrimeStatus
                };

                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();

                var kidnapperDetails = new KidnapperDetailsAlerts
                {
                    KidnapperName = string.IsNullOrWhiteSpace(dto.KidnapperName) ? "Unknown" : dto.KidnapperName,
                    KidnapperAge = dto.KidnapperAge == 0 ? (int)0 : dto.KidnapperAge,
                    KidnapperSex = string.IsNullOrWhiteSpace(dto.KidnapperSex) ? "Unknown" : dto.KidnapperSex,
                    KidnapperLook = string.IsNullOrWhiteSpace(dto.KidnapperLook) ? "Unknown look" : dto.KidnapperLook,
                    KidnapperVehicle = string.IsNullOrWhiteSpace(dto.KidnapperVehicle) ? "Unknown vehicle" : dto.KidnapperVehicle,
                    AlertsId = alert.Id,
                    Alerts = alert
                };

                _context.KidnapperDetailsAlerts.Add(kidnapperDetails);
                await _context.SaveChangesAsync();


                _logger.LogInformation("Alert created successfully: {@Alert}", alert);

                return Ok(new
                {
                    message = "Alert created successfully.",
                    alert
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an alert.");
                return StatusCode(500, new { message = "An error occurred while creating alerts.", error = ex.Message });
            }
        }


        [Authorize(Roles = "1")]
        [HttpGet("alert/{id}")]
        public async Task<IActionResult> GetAlert(int id)
        {
            _logger.LogInformation("Entering {Action} to retrieve alert with ID {AlertId}", nameof(GetAlert), id);


            try
            {
                var alert = await _context.Alerts
                                   .Include(a => a.KidnapperDetailsAlerts)
                                   .FirstOrDefaultAsync(a => a.Id == id);

                if (alert == null)
                {
                    _logger.LogWarning("Alert with ID {AlertId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Alert with ID {AlertId} retrieved successfully.", id);
                return Ok(alert);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating alert with ID {AlertId}.", id);
                return StatusCode(500, new { message = "An error occurred while updating alerts.", error = ex.Message });
            }
        }

        [Authorize(Roles = "1")]
        [HttpPut("alert/{id}/update")]
        public async Task<IActionResult> UpdateAlert(int id, [FromBody] Alerts alert)
        {
            _logger.LogInformation("Entering {Action} to update alert with ID {AlertId}", nameof(UpdateAlert), id);

            try
            {
                var existingAlert = await _context.Alerts.FindAsync(id);
                if (existingAlert == null)
                {
                    _logger.LogWarning("Alert with ID {AlertId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Current alert data: {AlertData}", existingAlert);

                existingAlert.VictimName = alert.VictimName;
                existingAlert.VictimAge = alert.VictimAge;
                existingAlert.CrimeLocation = alert.CrimeLocation;
                existingAlert.CrimeDate = alert.CrimeDate;
                existingAlert.CrimeStatus = alert.CrimeStatus;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Alert with ID {AlertId} updated successfully.", id);
                return Ok(existingAlert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating alert with ID {AlertId}.", id);
                return StatusCode(500, new { message = "An error occurred while updating alerts.", error = ex.Message });
            }
        }


        [Authorize(Roles = "1")]
        [HttpPatch("alerts/{id}/close")]
        public async Task<IActionResult> CloseAlert(int id)
        {
            _logger.LogInformation("Entering {Action} to close alert with ID {AlertId}", nameof(CloseAlert), id);

            try
            {
                var alert = await _context.Alerts.FindAsync(id);
                if (alert == null)
                {
                    _logger.LogWarning("Alert with ID {AlertId} not found.", id);
                    return NotFound();
                }

                if (alert.CrimeStatus == 0)
                {
                    _logger.LogInformation("Alert with ID {AlertsId} already closed.", id);
                    return Ok("Alert dont need to be closed");
                }

                alert.CrimeStatus = 0;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Alert with ID {AlertId} closed successfully.", id);
                return Ok($"Alert with ID {alert.Id} successfuly closed, status {alert.CrimeStatus}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while closing alert with ID {AlertId}.", id);
                return StatusCode(500, new { message = "An error occurred while closing the alert.", error = ex.Message });
            }
        }

        [Authorize(Roles = "1")]
        [HttpDelete("alerts/all")]
        public async Task<IActionResult> DeleteAllAlerts()
        {
            _logger.LogInformation("Entering {Action} to delete all alerts", nameof(DeleteAllAlerts));

            try
            {
                var allAlerts = _context.Alerts.ToList();

                if (!allAlerts.Any())
                {
                    _logger.LogWarning("No alerts found to delete.");
                    return Ok(new { message = "No alerts to delete." });
                }

                _context.Alerts.RemoveRange(allAlerts);
                await _context.SaveChangesAsync();

                _logger.LogInformation("All alerts have been deleted successfully.");
                return Ok(new { message = "All alerts have been deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting all alerts.");
                return StatusCode(500, new { message = "An error occurred while deleting alerts.", error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("alerts")]
        public async Task<IActionResult> GetAllAlerts()
        {
            _logger.LogInformation("Entering {Action} to get all alerts", nameof(GetAllAlerts));

            try
            {
                var alerts = await _context.Alerts
                    .Include(alert => alert.KidnapperDetailsAlerts)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} alerts with kidnapper details.", alerts.Count);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all alerts.");
                return StatusCode(500, new { message = "An error occurred while getting all alerts.", error = ex.Message });
            }
        }
    }
}