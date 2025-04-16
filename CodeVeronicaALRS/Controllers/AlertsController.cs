using ALRS.Data;
using ALRS.DTO;
using ALRS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ALRS.Controllers
{
    [AllowAnonymous]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AlertsController> _logger;
        private readonly IWebHostEnvironment _env;


        public AlertsController(ApplicationDbContext context, ILogger<AlertsController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        private byte[] GetPlaceholderImageBytes(string type)
        {
            string placeholderFileName = type.Equals("victim", StringComparison.OrdinalIgnoreCase)
                ? "placeholder_victim.jpg"
                : "placeholder_abductor.png";

            var placeholderPath = Path.Combine(_env.ContentRootPath, "Resources", placeholderFileName);

            if (System.IO.File.Exists(placeholderPath))
            {
                return System.IO.File.ReadAllBytes(placeholderPath);
            }
            return new byte[0];
        }


        [Authorize(Roles = "1")]
        [HttpPost("alert/create")]
        public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto dto)
        {
            _logger.LogInformation("Entering {Action} with alert data: {@Dto}", nameof(CreateAlert), dto);

            var datePart = DateTime.ParseExact(dto.CrimeDate.Date,
                                           "dd.MM.yyyy",
                                           CultureInfo.InvariantCulture);

            var timePart = TimeSpan.ParseExact(dto.CrimeDate.Time,
                                           @"hh\:mm",
                                           CultureInfo.InvariantCulture);

            try
            {
                var alert = new Alert
                {
                    AlertStatus = dto.AlertStatus,
                    CrimeDistrict = string.IsNullOrWhiteSpace(dto.CrimeDistrict) ? "Unknown" : dto.CrimeDistrict,
                    CrimeLocation = string.IsNullOrWhiteSpace(dto.CrimeLocation) ? "Unknown" : dto.CrimeLocation,
                    CrimeDate = datePart,
                    CrimeTime = timePart,
                };

                var victim = new Victim
                {
                    VictimName = string.IsNullOrWhiteSpace(dto.Victim.VictimName) ? "Unknown" : dto.Victim.VictimName,
                    VictimAge = dto.Victim.VictimAge > 0 ? dto.Victim.VictimAge : 0,
                    VictimSex = string.IsNullOrWhiteSpace(dto.Victim.VictimSex) ? "Unknown" : dto.Victim.VictimSex,
                    VictimSkinColor = string.IsNullOrWhiteSpace(dto.Victim.VictimSkinColor) ? "Unknown" : dto.Victim.VictimSkinColor,
                    VictimHair = string.IsNullOrWhiteSpace(dto.Victim.VictimHair) ? "Unknown" : dto.Victim.VictimHair,
                    VictimClothing = string.IsNullOrWhiteSpace(dto.Victim.VictimClothing) ? "Unknown" : dto.Victim.VictimClothing,
                    VictimDistinctiveFeatures = string.IsNullOrWhiteSpace(dto.Victim.VictimDistinctiveFeatures) ? "Unknown" : dto.Victim.VictimDistinctiveFeatures,
                    VictimPhoto = string.IsNullOrEmpty(dto.Victim.VictimPhoto)
                        ? GetPlaceholderImageBytes("victim")
                        : Convert.FromBase64String(dto.Victim.VictimPhoto),
                    Alert = alert
                };

                var abductor = new Abductor
                {
                    AbductorName = string.IsNullOrWhiteSpace(dto.Abductor.AbductorName) ? "Unknown" : dto.Abductor.AbductorName,
                    AbductorAge = dto.Abductor.AbductorAge > 0 ? dto.Abductor.AbductorAge : 0,
                    AbductorSex = string.IsNullOrWhiteSpace(dto.Abductor.AbductorSex) ? "Unknown" : dto.Abductor.AbductorSex,
                    AbductorSkinColor = string.IsNullOrWhiteSpace(dto.Abductor.AbductorSkinColor) ? "Unknown" : dto.Abductor.AbductorSkinColor,
                    AbductorHair = string.IsNullOrWhiteSpace(dto.Abductor.AbductorHair) ? "Unknown" : dto.Abductor.AbductorHair,
                    AbductorClothing = string.IsNullOrWhiteSpace(dto.Abductor.AbductorClothing) ? "Unknown" : dto.Abductor.AbductorClothing,
                    AbductorDistinctiveFeatures = string.IsNullOrWhiteSpace(dto.Abductor.AbductorDistinctiveFeatures) ? "Unknown" : dto.Abductor.AbductorDistinctiveFeatures,
                    AbductorVehicle = string.IsNullOrWhiteSpace(dto.Abductor.AbductorVehicle) ? "Unknown" : dto.Abductor.AbductorVehicle,
                    AbductorPhoto = string.IsNullOrEmpty(dto.Abductor.AbductorPhoto)
                        ? GetPlaceholderImageBytes("abductor")
                        : Convert.FromBase64String(dto.Abductor.AbductorPhoto),
                    Alert = alert
                };

                _context.Alert.Add(alert);
                _context.Victim.Add(victim);
                _context.Abductor.Add(abductor);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Alert created successfully: {@Alert}", alert);

                return Ok(new
                {
                    message = "Alert created successfully.",
                    alertId = alert.AlertId
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
        public async Task<IActionResult> GetAlertById(int id)
        {
            _logger.LogInformation("Entering {Action} to retrieve alert with ID {AlertId}", nameof(GetAlertById), id);

           

            try
            {
                var alert = await _context.Alert
                                   .Include(a => a.Victim)
                                   .Include(a => a.Abductor)
                                   .FirstOrDefaultAsync(a => a.AlertId == id);

                if (alert == null)
                {
                    _logger.LogWarning("Alert with ID {AlertId} not found.", id);
                    return NotFound();
                }

                var crimeDateDto = new CrimeDateDto
                {
                    Date = alert.CrimeDate.ToString("dd.MM.yyyy"),
                    Time = alert.CrimeTime.ToString(@"hh\:mm")
                };

                var alertDto = new GetAlertByIdDto
                {
                    AlertStatus = alert.AlertStatus,
                    CrimeDistrict = alert.CrimeDistrict,
                    CrimeLocation = alert.CrimeLocation,
                    CrimeDate = crimeDateDto,

                    Victim = new GetAlertByIdVictimDto
                    {
                        VictimName = alert.Victim.VictimName,
                        VictimAge = alert.Victim.VictimAge,
                        VictimSex = alert.Victim.VictimSex,
                        VictimSkinColor = alert.Victim.VictimSkinColor,
                        VictimHair = alert.Victim.VictimHair,
                        VictimClothing = alert.Victim.VictimClothing,
                        VictimDistinctiveFeatures = alert.Victim.VictimDistinctiveFeatures,
                        VictimPhoto = (alert.Victim.VictimPhoto != null && alert.Victim.VictimPhoto.Length > 0)
                            ? Convert.ToBase64String(alert.Victim.VictimPhoto)
                            : Convert.ToBase64String(GetPlaceholderImageBytes("victim"))
                    },
                    Abductor = new GetAlertByIdAbductorDto
                    {
                        AbductorName = alert.Abductor?.AbductorName,
                        AbductorAge = alert.Abductor?.AbductorAge ?? 0,
                        AbductorSex = alert.Abductor?.AbductorSex,
                        AbductorSkinColor = alert.Abductor?.AbductorSkinColor,
                        AbductorHair = alert.Abductor?.AbductorHair,
                        AbductorClothing = alert.Abductor?.AbductorClothing,
                        AbductorDistinctiveFeatures = alert.Abductor?.AbductorDistinctiveFeatures,
                        AbductorVehicle = alert.Abductor?.AbductorVehicle,
                        AbductorPhoto = (alert.Abductor?.AbductorPhoto != null && alert.Abductor?.AbductorPhoto.Length > 0)
                            ? Convert.ToBase64String(alert.Abductor.AbductorPhoto)
                            : Convert.ToBase64String(GetPlaceholderImageBytes("abductor"))
                    }
                };

                _logger.LogInformation("Alert with ID {AlertId} retrieved successfully.", id);
                return Ok(alertDto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating alert with ID {AlertId}.", id);
                return StatusCode(500, new { message = "An error occurred while updating alerts.", error = ex.Message });
            }
        }

        [Authorize(Roles = "1")]
        [HttpGet("alert/{id}/user-reports")]
        public async Task<IActionResult> GetUserReportsForAlert(int id)
        {
            _logger.LogInformation("Entering {Action} to retrieve user reports for alert with ID {AlertId}", nameof(GetUserReportsForAlert), id);

            try
            {
                var citizenReports = await _context.CitizenReport
                                    .Where(r => r.AlertId == id)
                                    .ToListAsync();

                if (citizenReports == null || !citizenReports.Any())
                {
                    _logger.LogWarning("No citizen reports found for alert with ID {AlertId}.", id);
                    return NotFound();
                }

                var citizenReportDtos = citizenReports.Select(citizenReport => new CitizenReportsDto
                {
                    CitizenReportId = citizenReport.CitizenReportId,
                    CitizenName = citizenReport.CitizenName,
                    CitizenContactPhone = citizenReport.CitizenContactPhone,
                    Location = citizenReport.Location,
                    Date = citizenReport.Date,
                    Description = citizenReport.Description,
                    IsAnonymous = citizenReport.IsAnonymous
                }).ToList();

                _logger.LogInformation("User reports for alert with ID {AlertId} retrieved successfully.", id);
                return Ok(citizenReportDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user reports for alert with ID {AlertId}.", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the user reports.", error = ex.Message });
            }
        }


        [Authorize(Roles = "1")]
        [HttpPut("alert/{id}/update")]
        public async Task<IActionResult> UpdateAlert(int id, [FromBody] UpdateAlertDto dto)
        {
            _logger.LogInformation("Entering {Action} to update alert with ID {AlertId}", nameof(UpdateAlert), id);

            try
            {
                var existingAlert = await _context.Alert
                    .Include(a => a.Victim)
                    .Include(a => a.Abductor)
                    .FirstOrDefaultAsync(a => a.AlertId == id);

                if (existingAlert == null)
                {
                    _logger.LogWarning("Alert with ID {AlertId} not found.", id);
                    return NotFound(new { message = $"Alert with ID {id} not found." });
                }

                _logger.LogInformation("Current alert data: {AlertData}", existingAlert);

                

                existingAlert.AlertStatus = dto.AlertStatus;
                existingAlert.CrimeDistrict = dto.CrimeDistrict;
                existingAlert.CrimeLocation = dto.CrimeLocation;


                if (dto.CrimeDate != null)
                {
                    if (!string.IsNullOrWhiteSpace(dto.CrimeDate.Date))
                    {
                        existingAlert.CrimeDate = DateTime.ParseExact(
                            dto.CrimeDate.Date,
                            "dd.MM.yyyy",
                            CultureInfo.InvariantCulture
                        );
                    }
                    if (!string.IsNullOrWhiteSpace(dto.CrimeDate.Time))
                    {
                        existingAlert.CrimeTime = TimeSpan.ParseExact(
                            dto.CrimeDate.Time,
                            @"hh\:mm",
                            CultureInfo.InvariantCulture
                        );
                    }
                }

                if (existingAlert.Victim != null && dto.Victim != null)
                {
                    existingAlert.Victim.VictimName = dto.Victim.VictimName;
                    existingAlert.Victim.VictimAge = dto.Victim.VictimAge;
                    existingAlert.Victim.VictimSex = dto.Victim.VictimSex;
                    existingAlert.Victim.VictimSkinColor = dto.Victim.VictimSkinColor;
                    existingAlert.Victim.VictimHair = dto.Victim.VictimHair;
                    existingAlert.Victim.VictimClothing = dto.Victim.VictimClothing;
                    existingAlert.Victim.VictimDistinctiveFeatures = dto.Victim.VictimDistinctiveFeatures;
                    existingAlert.Victim.VictimPhoto = string.IsNullOrEmpty(dto.Victim.VictimPhoto)
                            ? GetPlaceholderImageBytes("victim")
                            : Convert.FromBase64String(dto.Victim.VictimPhoto);
                }

                if (existingAlert.Abductor != null && dto.Abductor != null)
                {
                    existingAlert.Abductor.AbductorName = dto.Abductor.AbductorName;
                    existingAlert.Abductor.AbductorAge = dto.Abductor.AbductorAge;
                    existingAlert.Abductor.AbductorSex = dto.Abductor.AbductorSex;
                    existingAlert.Abductor.AbductorSkinColor = dto.Abductor.AbductorSkinColor;
                    existingAlert.Abductor.AbductorHair = dto.Abductor.AbductorHair;
                    existingAlert.Abductor.AbductorClothing = dto.Abductor.AbductorClothing;
                    existingAlert.Abductor.AbductorDistinctiveFeatures = dto.Abductor.AbductorDistinctiveFeatures;
                    existingAlert.Abductor.AbductorVehicle = dto.Abductor.AbductorVehicle;
                    existingAlert.Abductor.AbductorPhoto = string.IsNullOrEmpty(dto.Abductor.AbductorPhoto)
                            ? GetPlaceholderImageBytes("abductor")
                            : Convert.FromBase64String(dto.Abductor.AbductorPhoto);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Alert with ID {AlertId} updated successfully.", id);
                return Ok(new { message = "Alert updated successfully.", alert = existingAlert });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating alert with ID {AlertId}.", id);
                return StatusCode(500, new { message = "An error occurred while updating the alert.", error = ex.Message });
            }
        }

        [Authorize(Roles = "1")]
        [HttpPatch("alerts/{id}/close")]
        public async Task<IActionResult> CloseAlert(int id)
        {
            _logger.LogInformation("Entering {Action} to close alert with ID {AlertId}", nameof(CloseAlert), id);

            try
            {
                var alert = await _context.Alert.FindAsync(id);
                if (alert == null)
                {
                    _logger.LogWarning("Alert with ID {AlertId} not found.", id);
                    return NotFound();
                }

                if (alert.AlertStatus == 1)
                {
                    _logger.LogInformation("Alert with ID {AlertsId} already closed.", id);
                    return Ok("Alert dont need to be closed");
                }

                alert.AlertStatus = 1;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Alert with ID {AlertId} closed successfully.", id);
                return Ok($"Alert with ID {alert.AlertId} successfuly closed, status {alert.AlertStatus}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while closing alert with ID {AlertId}.", id);
                return StatusCode(500, new { message = "An error occurred while closing the alert.", error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("alerts")]
        public async Task<IActionResult> GetAllAlerts()
        {
            _logger.LogInformation("Entering {Action} to get all alerts", nameof(GetAllAlerts));

            try
            {
                var alerts = await _context.Alert
                    .Include(a => a.Victim)
                    .Include(a => a.Abductor)
                    .ToListAsync();

                var result = alerts.Select(a => new GetAlertByIdDto
                {
                    AlertId = a.AlertId,
                    AlertStatus = a.AlertStatus,
                    CrimeDistrict = a.CrimeDistrict,
                    CrimeLocation = a.CrimeLocation,

                    CrimeDate = new CrimeDateDto
                    {
                        Date = a.CrimeDate.ToString("dd.MM.yyyy"),
                        Time = a.CrimeTime.ToString(@"hh\:mm")
                    },

                    Victim = new GetAlertByIdVictimDto
                    {
                        VictimName = a.Victim.VictimName,
                        VictimAge = a.Victim.VictimAge,
                        VictimSex = a.Victim.VictimSex,
                        VictimSkinColor = a.Victim.VictimSkinColor,
                        VictimHair = a.Victim.VictimHair,
                        VictimClothing = a.Victim.VictimClothing,
                        VictimDistinctiveFeatures = a.Victim.VictimDistinctiveFeatures,
                        VictimPhoto = (a.Victim.VictimPhoto != null && a.Victim.VictimPhoto.Length > 0)
                            ? Convert.ToBase64String(a.Victim.VictimPhoto)
                            : Convert.ToBase64String(GetPlaceholderImageBytes("victim"))
                    },
                    Abductor = new GetAlertByIdAbductorDto
                    {
                        AbductorName = a.Abductor.AbductorName,
                        AbductorAge = a.Abductor.AbductorAge,
                        AbductorSex = a.Abductor.AbductorSex,
                        AbductorSkinColor = a.Abductor.AbductorSkinColor,
                        AbductorHair = a.Abductor.AbductorHair,
                        AbductorClothing = a.Abductor.AbductorClothing,
                        AbductorDistinctiveFeatures = a.Abductor.AbductorDistinctiveFeatures,
                        AbductorVehicle = a.Abductor.AbductorVehicle,
                        AbductorPhoto = (a.Abductor.AbductorPhoto != null && a.Abductor.AbductorPhoto.Length > 0)
                            ? Convert.ToBase64String(a.Abductor.AbductorPhoto)
                            : Convert.ToBase64String(GetPlaceholderImageBytes("abductor"))
                    }
                }).ToList();

                _logger.LogInformation("Retrieved {Count} alerts.", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all alerts.");
                return StatusCode(500, new { message = "An error occurred while getting all alerts.", error = ex.Message });
            }
        }
    }
}