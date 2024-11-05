using ALRS.Data;
using ALRS.DTO;
using ALRS.Models;
using Microsoft.AspNetCore.Mvc;


namespace ALRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAlertController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserAlertController> _logger;

        public UserAlertController(ApplicationDbContext context, ILogger<UserAlertController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("user-submissions/alert-with-kidnapper")]
        public async Task<IActionResult> CreateUserSubmittedAlertWithKidnapper([FromBody] CombinedUserAlertDto combinedDto)
        {
            _logger.LogInformation("Entering {Action} to create a new alert with kidnapper info.", nameof(CreateUserSubmittedAlertWithKidnapper));

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in {Action}.", nameof(CreateUserSubmittedAlertWithKidnapper));
                    return BadRequest(ModelState);
                }

                // Перевіряємо, чи існує запис в Alerts з наданим AlertsId
                var alert = await _context.Alerts.FindAsync(combinedDto.AlertsId);
                if (alert == null)
                {
                    _logger.LogWarning("Alert with ID {Id} not found.", combinedDto.AlertsId);
                    return NotFound(new { message = $"Alert with ID {combinedDto.AlertsId} not found." });
                }

                var childAlert = new UserSubmittedAlert
                {
                    CrimeLocation = combinedDto.UserAlert.CrimeLocation,
                    CrimeDate = combinedDto.UserAlert.CrimeDate,
                    VictimLook = combinedDto.UserAlert.VictimLook,
                    AlertsId = combinedDto.AlertsId, // Використовуємо передане AlertsId
                    KidnapperDetails = new KidnapperDetails
                    {
                        KidnapperName = string.IsNullOrWhiteSpace(combinedDto.Kidnapper.KidnapperName) ? "Unknown" : combinedDto.Kidnapper.KidnapperName,
                        KidnapperAge = combinedDto.Kidnapper.KidnapperAge == 0 ? 0 : combinedDto.Kidnapper.KidnapperAge,
                        KidnapperSex = string.IsNullOrWhiteSpace(combinedDto.Kidnapper.KidnapperSex) ? "Unknown" : combinedDto.Kidnapper.KidnapperSex,
                        KidnapperLook = string.IsNullOrWhiteSpace(combinedDto.Kidnapper.KidnapperLook) ? "Unknown" : combinedDto.Kidnapper.KidnapperLook,
                        KidnapperVehicle = string.IsNullOrWhiteSpace(combinedDto.Kidnapper.KidnapperVehicle) ? "Unknown" : combinedDto.Kidnapper.KidnapperVehicle
                    }
                };

                _context.UserSubmittedAlerts.Add(childAlert);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Alert with kidnapper info created successfully in {Action}.", nameof(CreateUserSubmittedAlertWithKidnapper));

                return Ok(childAlert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in {Action} while creating an alert with kidnapper info.", nameof(CreateUserSubmittedAlertWithKidnapper));
                return StatusCode(500, new { message = "An error occurred while sending information.", error = ex.Message });
            }
        }


    }
}