using ALRS.Data;
using ALRS.DTO;
using ALRS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ALRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitizenreportsControler : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CitizenreportsControler> _logger;

        public CitizenreportsControler(ApplicationDbContext context, ILogger<CitizenreportsControler> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("citizen-report")]
        public async Task<IActionResult> CreateCitizenReport(int alertId, [FromBody] CitizenReportsDto citizenReportsDto)
        {
            _logger.LogInformation("Entering {Action} to send citizen report with information.", nameof(CreateCitizenReport));

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in {Action}.", nameof(CreateCitizenReport));
                    return BadRequest(ModelState);
                }

                var alert = await _context.Alert.FindAsync(alertId);
                if (alert == null)
                {
                    _logger.LogWarning("Alert with ID {alertId} not found.", alertId);
                    return NotFound(new { message = $"Alert with ID {alertId} not found." });
                }

                var childCitizenReport = new CitizenReport
                {
                    CitizenName = string.IsNullOrWhiteSpace(citizenReportsDto.CitizenName) ? "Unknown" : citizenReportsDto.CitizenName,
                    CitizenContactPhone = string.IsNullOrWhiteSpace(citizenReportsDto.CitizenContactPhone) ? "Unknown" : citizenReportsDto.CitizenContactPhone,
                    Location = citizenReportsDto.Location,
                    Date = citizenReportsDto.Date,
                    Description = citizenReportsDto.Description,
                    IsAnonymous = citizenReportsDto.IsAnonymous,
                    AlertId = alertId
                };

                _context.Add(childCitizenReport);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Citizen report has been created {Action}", nameof(CreateCitizenReport));
                return Ok(childCitizenReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in {Action} while creating an citizen report.", nameof(CreateCitizenReport));
                return StatusCode(500, new { message = "An error occurred while sending information.", error = ex.Message });
            }
        }
    }
}