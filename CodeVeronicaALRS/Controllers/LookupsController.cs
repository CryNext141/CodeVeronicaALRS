using ALRS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeVeronicaALRS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LookupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LookupsController> _logger;

        public LookupsController(
            ApplicationDbContext context,
            ILogger<LookupsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("genders")]
        public async Task<IActionResult> GetGenders()
        {
            _logger.LogInformation("Entering {Action} to retrieve genders", nameof(GetGenders));

            try
            {
                var genders = await _context.Genders
                    .Select(g => new { g.GenderId, g.DisplayName })
                    .ToListAsync();

                if (genders == null || !genders.Any())
                {
                    _logger.LogWarning("No genders found.");
                    return NotFound(new { message = "No genders available." });
                }

                _logger.LogInformation("Retrieved {Count} genders successfully.", genders.Count);
                return Ok(genders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving genders.");
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving genders.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("skin-colors")]
        public async Task<IActionResult> GetSkinColors()
        {
            _logger.LogInformation("Entering {Action} to retrieve skin colors", nameof(GetSkinColors));

            try
            {
                var skinColors = await _context.SkinColors
                    .Select(s => new { s.SkinColorId, s.Name })
                    .ToListAsync();

                if (skinColors == null || !skinColors.Any())
                {
                    _logger.LogWarning("No skin colors found.");
                    return NotFound(new { message = "No skin colors available." });
                }

                _logger.LogInformation("Retrieved {Count} skin colors successfully.", skinColors.Count);
                return Ok(skinColors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving skin colors.");
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving skin colors.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("alert-statuses")]
        public async Task<IActionResult> GetAlertStatuses()
        {
            _logger.LogInformation("Entering {Action} to retrieve alert statuses", nameof(GetAlertStatuses));

            try
            {
                var list = await _context.AlertStatus
                    .Select(s => new {
                        s.AlertStatusId,
                        s.Code,
                        s.DisplayName
                    })
                    .ToListAsync();

                if (list == null || !list.Any())
                {
                    _logger.LogWarning("No alert statuses found.");
                    return NotFound(new { message = "No alert statuses available." });
                }

                _logger.LogInformation("Retrieved {Count} alert statuses successfully.", list.Count);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving alert statuses.");
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving alert statuses.",
                    error = ex.Message
                });
            }
        }
    }
}
