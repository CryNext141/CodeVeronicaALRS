using ALRS.Data;
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

        [HttpPost("user-submissions/victim")]
        public async Task<IActionResult> CreateChildInfo([FromBody] UserSubmittedAlert childDto)
        {
            _logger.LogInformation("Entering {Action} to create a new victim alert.", nameof(CreateChildInfo));

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in {Action}.", nameof(CreateChildInfo));
                    return BadRequest(ModelState);
                }

                var childAlert = new UserSubmittedAlert
                {
                    CrimeLocation = childDto.CrimeLocation,
                    CrimeDate = childDto.CrimeDate,
                    VictimLook = childDto.VictimLook
                };

                _context.UserSubmittedAlerts.Add(childAlert);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Victim alert created successfully in {Action}.", nameof(CreateChildInfo));

                return Ok(childAlert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in {Action} while creating a victim alert.", nameof(CreateChildInfo));
                return StatusCode(500, new { message = "An error occurred while sending information.", error = ex.Message });
            }
        }

        [HttpPost("user-submissions/kidnapper")]
        public async Task<IActionResult> CreateKidnapperInfo([FromBody] KidnapperDto kidnapperDto)
        {
            _logger.LogInformation("Entering {Action} to create a new kidnapper alert.", nameof(CreateKidnapperInfo));

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in {Action}.", nameof(CreateKidnapperInfo));
                    return BadRequest(ModelState);
                }

                var kidnapperDetails = new KidnapperDetails
                {
                    KidnapperName = string.IsNullOrWhiteSpace(kidnapperDto.KidnapperName) ? "Unknown" : kidnapperDto.KidnapperName,
                    KidnapperAge = kidnapperDto.KidnapperAge == 0 ? (int)0 : kidnapperDto.KidnapperAge,
                    KidnapperSex = string.IsNullOrWhiteSpace(kidnapperDto.KidnapperSex) ? "Unknown" : kidnapperDto.KidnapperSex,
                    KidnapperLook = string.IsNullOrWhiteSpace(kidnapperDto.KidnapperLook) ? "Unknown" : kidnapperDto.KidnapperLook,
                    KidnapperVehicle = string.IsNullOrWhiteSpace(kidnapperDto.KidnapperVehicle) ? "Unknown" : kidnapperDto.KidnapperVehicle
                };

                _context.KidnapperDetails.Add(kidnapperDetails);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Kidnapper alert created successfully in {Action}.", nameof(CreateKidnapperInfo));

                return Ok(kidnapperDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in {Action} while creating a kidnapper alert.", nameof(CreateKidnapperInfo));
                return StatusCode(500, new { message = "An error occurred while sending information.", error = ex.Message });
            }
        }
    }
}