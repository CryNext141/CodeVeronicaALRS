using ALRS.Data;
using ALRS.DTO;
using ALRS.Models;
using CodeVeronicaALRS.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
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
        private readonly IEventBus _eventBus;



        public AlertsController(ApplicationDbContext context, ILogger<AlertsController> logger, IWebHostEnvironment env, IEventBus eventBus)
        {
            _context = context;
            _logger = logger;
            _env = env;
            _eventBus = eventBus;
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
                    AlertStatusId = dto.AlertStatusId,
                    CrimeDistrict = string.IsNullOrWhiteSpace(dto.CrimeDistrict) ? "Unknown" : dto.CrimeDistrict,
                    CrimeLocation = string.IsNullOrWhiteSpace(dto.CrimeLocation) ? "Unknown" : dto.CrimeLocation,
                    CrimeDate = datePart,
                    CrimeTime = timePart,
                };

                var victim = new Victim
                {
                    VictimName = string.IsNullOrWhiteSpace(dto.Victim.VictimName) ? "Unknown" : dto.Victim.VictimName,
                    VictimAge = dto.Victim.VictimAge > 0 ? dto.Victim.VictimAge : 0,
                    GenderId = dto.Victim.GenderId,
                    SkinColorId = dto.Victim.SkinColorId,
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
                    GenderId = dto.Abductor.GenderId,
                    SkinColorId = dto.Abductor.SkinColorId,
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

                var evt = new AlertCreatedEvent(
                    alert.AlertId,
                    alert.CrimeDistrict,
                    $"New alert in {alert.CrimeDistrict}"
                );
                _eventBus.Publish(evt, "alert.created");

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
        public async Task<ActionResult<IEnumerable<GetAlertByIdDto>>> GetAlertById(int id)
        {
            _logger.LogInformation("Entering {Action} to retrieve alert ID {AlertId}",
                                   nameof(GetAlertById), id);

            try
            {
                var alert = await _context.Alert
                    .Include(a => a.AlertStatus)
                    .Include(a => a.Victim)
                        .ThenInclude(v => v.Gender)
                    .Include(a => a.Victim)
                        .ThenInclude(v => v.SkinColor)

                    .Include(a => a.Abductor)
                        .ThenInclude(ab => ab.Gender)
                    .Include(a => a.Abductor)
                        .ThenInclude(ab => ab.SkinColor)

                    .FirstOrDefaultAsync(a => a.AlertId == id);

                var dtoList = new List<GetAlertByIdDto>();

                if (alert != null)
                {
                    _logger.LogInformation("Alert with ID {AlertId} found.", id);
                    var dto = new GetAlertByIdDto
                    {
                        AlertId = alert.AlertId,
                        AlertStatusId = alert.AlertStatusId,
                        AlertStatus = alert.AlertStatus?.DisplayName,

                        CrimeDistrict = alert.CrimeDistrict,
                        CrimeLocation = alert.CrimeLocation,
                        CrimeDate = new CrimeDateDto
                        {
                            Date = alert.CrimeDate.ToString("dd.MM.yyyy"),
                            Time = alert.CrimeTime.ToString(@"hh\:mm")
                        },

                        Victim = alert.Victim == null ? null : new GetAlertByIdVictimDto
                        {
                            VictimName = alert.Victim.VictimName,
                            VictimAge = alert.Victim.VictimAge,
                            VictimGender = alert.Victim.Gender.DisplayName,
                            VictimSkinColor = alert.Victim.SkinColor.Name,
                            VictimHair = alert.Victim.VictimHair,
                            VictimClothing = alert.Victim.VictimClothing,
                            VictimDistinctiveFeatures = alert.Victim.VictimDistinctiveFeatures,
                            VictimPhoto = (alert.Victim.VictimPhoto?.Length > 0)
                                                     ? Convert.ToBase64String(alert.Victim.VictimPhoto)
                                                     : Convert.ToBase64String(GetPlaceholderImageBytes("victim"))
                        },

                        Abductor = alert.Abductor == null ? null : new GetAlertByIdAbductorDto
                        {
                            AbductorName = alert.Abductor.AbductorName,
                            AbductorAge = alert.Abductor.AbductorAge,
                            AbductorGender = alert.Abductor.Gender.DisplayName,
                            AbductorSkinColor = alert.Abductor.SkinColor.Name,
                            AbductorHair = alert.Abductor.AbductorHair,
                            AbductorClothing = alert.Abductor.AbductorClothing,
                            AbductorDistinctiveFeatures = alert.Abductor.AbductorDistinctiveFeatures,
                            AbductorVehicle = alert.Abductor.AbductorVehicle,
                            AbductorPhoto = (alert.Abductor.AbductorPhoto?.Length > 0)
                                                     ? Convert.ToBase64String(alert.Abductor.AbductorPhoto)
                                                     : Convert.ToBase64String(GetPlaceholderImageBytes("abductor"))
                        }
                    };
                    dtoList.Add(dto);
                }
                else
                {
                    _logger.LogWarning("Alert with ID {AlertId} not found. Returning empty list.", id);
                }


                _logger.LogInformation("Returning {Count} alert(s) for ID {AlertId}.", dtoList.Count, id);
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving alert ID {AlertId}.", id);
                return StatusCode(500, new { message = "An internal error occurred.", error = ex.Message });
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



                var citizenReportDto = citizenReports.Select(citizenReport => new CitizenReportsDto
                {
                    CitizenName = citizenReport.CitizenName,
                    CitizenContactPhone = citizenReport.CitizenContactPhone,
                    Location = citizenReport.Location,
                    ReportDate = new ReportDateDto
                    {
                        Date = citizenReport.ReportDate.ToString("dd.MM.yyyy"),
                        Time = citizenReport.ReportTime.ToString(@"hh\:mm")
                    },
                    Description = citizenReport.Description,
                    IsAnonymous = citizenReport.IsAnonymous
                }).ToList();

                _logger.LogInformation("User reports for alert with ID {AlertId} retrieved successfully.", id);
                return Ok(citizenReportDto);
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
            _logger.LogInformation("Entering {Action} to update alert with ID {AlertId}",
                                    nameof(UpdateAlert), id);

            try
            {
                var existingAlert = await _context.Alert
                    .Include(a => a.AlertStatus)
                    .Include(a => a.Victim)
                        .ThenInclude(v => v.Gender)
                    .Include(a => a.Victim)
                        .ThenInclude(v => v.SkinColor)
                    .Include(a => a.Abductor)
                        .ThenInclude(ab => ab.Gender)
                    .Include(a => a.Abductor)
                        .ThenInclude(ab => ab.SkinColor)
                    .FirstOrDefaultAsync(a => a.AlertId == id);

                if (existingAlert == null)
                {
                    _logger.LogWarning("Alert with ID {AlertId} not found.", id);
                    return NotFound(new { message = $"Alert with ID {id} not found." });
                }

                existingAlert.AlertStatusId = dto.AlertStatusId;
                existingAlert.CrimeDistrict = dto.CrimeDistrict;
                existingAlert.CrimeLocation = dto.CrimeLocation;

                if (dto.CrimeDate != null)
                {
                    if (!string.IsNullOrWhiteSpace(dto.CrimeDate.Date))
                    {
                        existingAlert.CrimeDate = DateTime.ParseExact(
                            dto.CrimeDate.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    }
                    if (!string.IsNullOrWhiteSpace(dto.CrimeDate.Time))
                    {
                        existingAlert.CrimeTime = TimeSpan.ParseExact(
                            dto.CrimeDate.Time, @"hh\:mm", CultureInfo.InvariantCulture);
                    }
                }

                if (existingAlert.Victim != null && dto.Victim != null)
                {
                    existingAlert.Victim.VictimName = dto.Victim.VictimName;
                    existingAlert.Victim.VictimAge = dto.Victim.VictimAge;
                    existingAlert.Victim.GenderId = dto.Victim.GenderId;
                    existingAlert.Victim.SkinColorId = dto.Victim.SkinColorId;
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
                    existingAlert.Abductor.GenderId = dto.Abductor.GenderId;
                    existingAlert.Abductor.SkinColorId = dto.Abductor.SkinColorId;
                    existingAlert.Abductor.AbductorHair = dto.Abductor.AbductorHair;
                    existingAlert.Abductor.AbductorClothing = dto.Abductor.AbductorClothing;
                    existingAlert.Abductor.AbductorDistinctiveFeatures = dto.Abductor.AbductorDistinctiveFeatures;
                    existingAlert.Abductor.AbductorVehicle = dto.Abductor.AbductorVehicle;
                    existingAlert.Abductor.AbductorPhoto = string.IsNullOrEmpty(dto.Abductor.AbductorPhoto)
                        ? GetPlaceholderImageBytes("abductor")
                        : Convert.FromBase64String(dto.Abductor.AbductorPhoto);
                }

                await _context.SaveChangesAsync();

                await _context.Entry(existingAlert)
                    .Reference(a => a.AlertStatus)
                    .LoadAsync();

                var responseDto = new GetAlertByIdDto
                {
                    AlertId = existingAlert.AlertId,
                    AlertStatusId = existingAlert.AlertStatusId,
                    AlertStatus = existingAlert.AlertStatus?.DisplayName,
                    CrimeDistrict = existingAlert.CrimeDistrict,
                    CrimeLocation = existingAlert.CrimeLocation,
                    CrimeDate = new CrimeDateDto
                    {
                        Date = existingAlert.CrimeDate.ToString("dd.MM.yyyy"),
                        Time = existingAlert.CrimeTime.ToString(@"hh\:mm")
                    },
                    Victim = existingAlert.Victim == null ? null : new GetAlertByIdVictimDto
                    {
                        VictimName = existingAlert.Victim.VictimName,
                        VictimAge = existingAlert.Victim.VictimAge,
                        VictimGender = existingAlert.Victim.Gender.DisplayName,
                        VictimSkinColor = existingAlert.Victim.SkinColor.Name,
                        VictimHair = existingAlert.Victim.VictimHair,
                        VictimClothing = existingAlert.Victim.VictimClothing,
                        VictimDistinctiveFeatures = existingAlert.Victim.VictimDistinctiveFeatures,
                        VictimPhoto = (existingAlert.Victim.VictimPhoto?.Length > 0)
                                                       ? Convert.ToBase64String(existingAlert.Victim.VictimPhoto)
                                                       : Convert.ToBase64String(GetPlaceholderImageBytes("victim"))
                    },
                    Abductor = existingAlert.Abductor == null ? null : new GetAlertByIdAbductorDto
                    {
                        AbductorName = existingAlert.Abductor.AbductorName,
                        AbductorAge = existingAlert.Abductor.AbductorAge,
                        AbductorGender = existingAlert.Abductor.Gender.DisplayName,
                        AbductorSkinColor = existingAlert.Abductor.SkinColor.Name,
                        AbductorHair = existingAlert.Abductor.AbductorHair,
                        AbductorClothing = existingAlert.Abductor.AbductorClothing,
                        AbductorDistinctiveFeatures = existingAlert.Abductor.AbductorDistinctiveFeatures,
                        AbductorVehicle = existingAlert.Abductor.AbductorVehicle,
                        AbductorPhoto = (existingAlert.Abductor.AbductorPhoto?.Length > 0)
                                                       ? Convert.ToBase64String(existingAlert.Abductor.AbductorPhoto)
                                                       : Convert.ToBase64String(GetPlaceholderImageBytes("abductor"))
                    }
                };

                _logger.LogInformation("Alert with ID {AlertId} updated successfully.", id);
                return Ok(new { message = "Alert updated successfully.", alert = responseDto });
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

                if (alert.AlertStatusId == 2)
                {
                    _logger.LogInformation("Alert with ID {AlertsId} already closed.", id);
                    return Ok("Alert dont need to be closed");
                }

                alert.AlertStatusId = 2;
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
                                    .Include(a => a.AlertStatus)
                                    .Include(a => a.Victim)
                                    .ThenInclude(v => v.Gender)
                                    .Include(a => a.Victim)
                                    .ThenInclude(v => v.SkinColor)
                                    .Include(a => a.Victim)
                                    .Include(a => a.Abductor)
                                    .ThenInclude(ab => ab.Gender)
                                    .Include(a => a.Abductor)
                                    .ThenInclude(ab => ab.SkinColor)
                                    .Include(a => a.Abductor)
                                    .ToListAsync();

                var result = alerts.Select(a => new GetAlertByIdDto
                {
                    AlertId = a.AlertId,
                    AlertStatusId = a.AlertStatusId,
                    AlertStatus = a.AlertStatus.DisplayName,
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
                        VictimGender = a.Victim.Gender.DisplayName,
                        VictimSkinColor = a.Victim.SkinColor.Name,
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
                        AbductorGender = a.Abductor.Gender.DisplayName,
                        AbductorSkinColor = a.Abductor.SkinColor.Name,
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

        [HttpPost("alert/{id}/archive")]
        public async Task<IActionResult> ArchiveAlert(int id)
        {
            _logger.LogInformation("Entering {Action} to archive alert with ID {AlertId}", nameof(ArchiveAlert), id);

            try
            {
                var alert = await _context.Alert
                    .Include(a => a.Victim)
                    .Include(a => a.Abductor)
                    .FirstOrDefaultAsync(a => a.AlertId == id);

                if (alert == null)
                {
                    _logger.LogWarning("Alert with ID {AlertId} not found.", id);
                    return NotFound(new { message = $"Alert {id} not found." });
                }

                _logger.LogInformation("Found alert {AlertId}. Preparing to archive.", id);

                var archive = new AlertArchive
                {
                    AlertId = alert.AlertId,
                    AlertStatusId = alert.AlertStatusId,
                    CrimeDistrict = alert.CrimeDistrict,
                    CrimeLocation = alert.CrimeLocation,
                    CrimeDate = alert.CrimeDate,
                    CrimeTime = alert.CrimeTime,

                    VictimName = alert.Victim?.VictimName,
                    VictimAge = alert.Victim?.VictimAge,
                    VictimGenderId = alert.Victim?.GenderId,
                    VictimSkinColorId = alert.Victim?.SkinColorId,
                    VictimHair = alert.Victim?.VictimHair,
                    VictimClothing = alert.Victim?.VictimClothing,
                    VictimDistinctiveFeatures = alert.Victim?.VictimDistinctiveFeatures,
                    VictimPhoto = alert.Victim?.VictimPhoto,

                    AbductorName = alert.Abductor?.AbductorName,
                    AbductorAge = alert.Abductor?.AbductorAge,
                    AbductorGenderId = alert.Abductor?.GenderId,
                    AbductorSkinColorId = alert.Abductor?.SkinColorId,
                    AbductorHair = alert.Abductor?.AbductorHair,
                    AbductorClothing = alert.Abductor?.AbductorClothing,
                    AbductorDistinctiveFeatures = alert.Abductor?.AbductorDistinctiveFeatures,
                    AbductorVehicle = alert.Abductor?.AbductorVehicle,
                    AbductorPhoto = alert.Abductor?.AbductorPhoto
                };

                _context.AlertArchive.Add(archive);
                _context.Alert.Remove(alert);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Alert with ID {AlertId} archived successfully.", id);
                return Ok(new { message = $"Alert {id} archived." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while archiving alert with ID {AlertId}.", id);
                return StatusCode(500, new
                {
                    message = "An error occurred while archiving the alert.",
                    error = ex.Message
                });
            }
        }


        [HttpPost("alerts/archive/inactive")]
        public async Task<IActionResult> ArchiveInactiveAlerts()
        {
            _logger.LogInformation("Entering {Action} to archive inactive alerts", nameof(ArchiveInactiveAlerts));

            try
            {
                var toArchive = await _context.Alert
                    .Where(a => a.AlertStatusId == 2 || a.AlertStatusId == 3)
                    .Include(a => a.Victim)
                    .Include(a => a.Abductor)
                    .ToListAsync();

                if (!toArchive.Any())
                {
                    _logger.LogWarning("No inactive alerts found to archive.");
                    return Ok(new { message = "No inactive alerts to archive." });
                }

                _logger.LogInformation("Found {Count} inactive alerts to archive.", toArchive.Count);

                var archives = toArchive.Select(alert => new AlertArchive
                {
                    AlertId = alert.AlertId,
                    AlertStatusId = alert.AlertStatusId,
                    CrimeDistrict = alert.CrimeDistrict,
                    CrimeLocation = alert.CrimeLocation,
                    CrimeDate = alert.CrimeDate,
                    CrimeTime = alert.CrimeTime,

                    VictimName = alert.Victim?.VictimName,
                    VictimAge = alert.Victim?.VictimAge,
                    VictimGenderId = alert.Victim?.GenderId,
                    VictimSkinColorId = alert.Victim?.SkinColorId,
                    VictimHair = alert.Victim?.VictimHair,
                    VictimClothing = alert.Victim?.VictimClothing,
                    VictimDistinctiveFeatures = alert.Victim?.VictimDistinctiveFeatures,
                    VictimPhoto = alert.Victim?.VictimPhoto,

                    AbductorName = alert.Abductor?.AbductorName,
                    AbductorAge = alert.Abductor?.AbductorAge,
                    AbductorGenderId = alert.Abductor?.GenderId,
                    AbductorSkinColorId = alert.Abductor?.SkinColorId,
                    AbductorHair = alert.Abductor?.AbductorHair,
                    AbductorClothing = alert.Abductor?.AbductorClothing,
                    AbductorDistinctiveFeatures = alert.Abductor?.AbductorDistinctiveFeatures,
                    AbductorVehicle = alert.Abductor?.AbductorVehicle,
                    AbductorPhoto = alert.Abductor?.AbductorPhoto
                }).ToList();

                _context.AlertArchive.AddRange(archives);
                _context.Alert.RemoveRange(toArchive);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Archived {Count} alerts successfully.", archives.Count);
                return Ok(new { message = $"{archives.Count} alerts archived." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while archiving inactive alerts.");
                return StatusCode(500, new
                {
                    message = "An error occurred while archiving inactive alerts.",
                    error = ex.Message
                });
            }
        }


        [HttpGet("alerts/archive")]
        public async Task<IActionResult> GetArchivedAlerts()
        {
            _logger.LogInformation("Entering {Action} to get archived alerts", nameof(GetArchivedAlerts));

            try
            {
                var query = from a in _context.AlertArchive.AsNoTracking()
                            join vg in _context.Genders on a.VictimGenderId equals vg.GenderId into vgJ
                            from vg in vgJ.DefaultIfEmpty()
                            join vs in _context.SkinColors on a.VictimSkinColorId equals vs.SkinColorId into vsJ
                            from vs in vsJ.DefaultIfEmpty()
                            join ag in _context.Genders on a.AbductorGenderId equals ag.GenderId into agJ
                            from ag in agJ.DefaultIfEmpty()
                            join asg in _context.SkinColors on a.AbductorSkinColorId equals asg.SkinColorId into asgJ
                            from asg in asgJ.DefaultIfEmpty()
                            select new AlertArchiveDto
                            {
                                AlertId = a.AlertId,
                                AlertStatusId = a.AlertStatusId,
                                CrimeDistrict = a.CrimeDistrict,
                                CrimeLocation = a.CrimeLocation,
                                CrimeDate = new CrimeDateDto
                                {
                                    Date = a.CrimeDate.ToString("dd.MM.yyyy"),
                                    Time = a.CrimeTime.ToString(@"hh\:mm")
                                },
                                Victim = new VictimArchiveDto
                                {
                                    VictimName = a.VictimName,
                                    VictimAge = a.VictimAge,
                                    VictimGender = vg != null ? vg.DisplayName : null,
                                    VictimSkinColor = vs != null ? vs.Name : null,
                                    VictimHair = a.VictimHair,
                                    VictimClothing = a.VictimClothing,
                                    VictimDistinctiveFeatures = a.VictimDistinctiveFeatures,
                                    VictimPhoto = a.VictimPhoto != null && a.VictimPhoto.Length > 0
                                                  ? Convert.ToBase64String(a.VictimPhoto)
                                                  : null
                                },
                                Abductor = new AbductorArchiveDto
                                {
                                    AbductorName = a.AbductorName,
                                    AbductorAge = a.AbductorAge,
                                    AbductorGender = ag != null ? ag.DisplayName : null,
                                    AbductorSkinColor = asg != null ? asg.Name : null,
                                    AbductorHair = a.AbductorHair,
                                    AbductorClothing = a.AbductorClothing,
                                    AbductorDistinctiveFeatures = a.AbductorDistinctiveFeatures,
                                    AbductorVehicle = a.AbductorVehicle,
                                    AbductorPhoto = a.AbductorPhoto != null && a.AbductorPhoto.Length > 0
                                                    ? Convert.ToBase64String(a.AbductorPhoto)
                                                    : null
                                }
                            };

                var result = await query.ToListAsync();

                _logger.LogInformation("Retrieved {Count} archived alerts.", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting archived alerts.");
                return StatusCode(500, new
                {
                    message = "An error occurred while getting archived alerts.",
                    error = ex.Message
                });
            }
        }
    }
}