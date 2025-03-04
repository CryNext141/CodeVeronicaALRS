using ALRS.Data;
using ALRS.DTO;
using ALRS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ALRS.Controllers
{
    [AllowAnonymous]
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AlertsController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;


        public AlertsController(ApplicationDbContext context, ILogger<AlertsController> logger, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _env = env;
            _configuration = configuration;
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

            try
            {
                var alert = new Alert
                {
                    AlertStatus = dto.AlertStatus,
                    CrimeLocation = string.IsNullOrWhiteSpace(dto.CrimeLocation) ? "Unknown" : dto.CrimeLocation,
                    CrimeDate = string.IsNullOrWhiteSpace(dto.CrimeDate) ? "Unknown" : dto.CrimeDate,
                };

                var victim = new Victim
                {
                    VictimName = string.IsNullOrWhiteSpace(dto.Victim.VictimName) ? "Unknown" : dto.Victim.VictimName,
                    VictimAge = dto.Victim.VictimAge > 0 ? dto.Victim.VictimAge : 0,
                    VictimSex = string.IsNullOrWhiteSpace(dto.Victim.VictimSex) ? "Unknown" : dto.Victim.VictimSex,
                    VictimHair = string.IsNullOrWhiteSpace(dto.Victim.VictimHair) ? "Unknown" : dto.Victim.VictimHair,
                    VictimClothing = string.IsNullOrWhiteSpace(dto.Victim.VictimClothing) ? "Unknown" : dto.Victim.VictimClothing,
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
                    AbductorHair = string.IsNullOrWhiteSpace(dto.Abductor.AbductorHair) ? "Unknown" : dto.Abductor.AbductorHair,
                    AbductorClothing = string.IsNullOrWhiteSpace(dto.Abductor.AbductorClothing) ? "Unknown" : dto.Abductor.AbductorClothing,
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


                var alertDto = new GetAlertById
                {
                    AlertStatus = alert.AlertStatus,
                    CrimeLocation = alert.CrimeLocation,
                    CrimeDate = alert.CrimeDate,

                    Victim = new GetAlertByIdVictimDto
                    {
                        VictimName = alert.Victim.VictimName,
                        VictimAge = alert.Victim.VictimAge,
                        VictimSex = alert.Victim.VictimSex,
                        VictimHair = alert.Victim.VictimHair,
                        VictimClothing = alert.Victim.VictimClothing
                    },
                    Abductor = new GetAlertByIdAbductorDto
                    {
                        AbductorName = alert.Abductor?.AbductorName,
                        AbductorAge = alert.Abductor?.AbductorAge ?? 0,
                        AbductorSex = alert.Abductor?.AbductorSex,
                        AbductorHair = alert.Abductor?.AbductorHair,
                        AbductorClothing = alert.Abductor?.AbductorClothing,
                        AbductorVehicle = alert.Abductor?.AbductorVehicle
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
                existingAlert.CrimeLocation = dto.CrimeLocation;
                existingAlert.CrimeDate = dto.CrimeDate;

                if (existingAlert.Victim != null && dto.Victim != null)
                {
                    existingAlert.Victim.VictimName = dto.Victim.VictimName;
                    existingAlert.Victim.VictimAge = dto.Victim.VictimAge;
                    existingAlert.Victim.VictimSex = dto.Victim.VictimSex;
                    existingAlert.Victim.VictimHair = dto.Victim.VictimHair;
                    existingAlert.Victim.VictimClothing = dto.Victim.VictimClothing;
                }

                if (existingAlert.Abductor != null && dto.Abductor != null)
                {
                    existingAlert.Abductor.AbductorName = dto.Abductor.AbductorName;
                    existingAlert.Abductor.AbductorAge = dto.Abductor.AbductorAge;
                    existingAlert.Abductor.AbductorSex = dto.Abductor.AbductorSex;
                    existingAlert.Abductor.AbductorHair = dto.Abductor.AbductorHair;
                    existingAlert.Abductor.AbductorClothing = dto.Abductor.AbductorClothing;
                    existingAlert.Abductor.AbductorVehicle = dto.Abductor.AbductorVehicle;
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

                var result = alerts.Select(a => new GetAlertById
                {
                    AlertId = a.AlertId,
                    AlertStatus = a.AlertStatus,
                    CrimeLocation = a.CrimeLocation,
                    CrimeDate = a.CrimeDate,
                    Victim = new GetAlertByIdVictimDto
                    {
                        VictimName = a.Victim.VictimName,
                        VictimAge = a.Victim.VictimAge,
                        VictimSex = a.Victim.VictimSex,
                        VictimHair = a.Victim.VictimHair,
                        VictimClothing = a.Victim.VictimClothing,
                        VictimPhoto = (a.Victim.VictimPhoto != null && a.Victim.VictimPhoto.Length > 0)
                            ? Convert.ToBase64String(a.Victim.VictimPhoto)
                            : Convert.ToBase64String(GetPlaceholderImageBytes("victim"))
                    },
                    Abductor = new GetAlertByIdAbductorDto
                    {
                        AbductorName = a.Abductor.AbductorName,
                        AbductorAge = a.Abductor.AbductorAge,
                        AbductorSex = a.Abductor.AbductorSex,
                        AbductorHair = a.Abductor.AbductorHair,
                        AbductorClothing = a.Abductor.AbductorClothing,
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

        [AllowAnonymous]
        [HttpPost("alert/{id}/send")]
        public async Task<IActionResult> SendAlertNotification(int id)
        {
            _logger.LogInformation("Starting to send alert {AlertId}", id);

            try
            {
                var alert = await _context.Alert
                    .Include(a => a.Victim)
                    .Include(a => a.Abductor)
                    .FirstOrDefaultAsync(a => a.AlertId == id);

                if (alert == null)
                {
                    _logger.LogWarning("Alert {AlertId} not found", id);
                    return NotFound(new { message = "Alert not found." });
                }

                var subscriptions = await _context.Subscriptions
                    .Where(s => s.IsActive)
                    .ToListAsync();

                if (!subscriptions.Any())
                {
                    _logger.LogInformation("No active subscriptions to send alert {AlertId}", id);
                    return Ok(new { message = "No active subscriptions." });
                }

                var botToken = _configuration["BotToken"];
                if (string.IsNullOrEmpty(botToken))
                {
                    _logger.LogError("Bot token is missing in configuration.");
                    return StatusCode(500, new { message = "Internal server error." });
                }

                var botClient = new TelegramBotClient(botToken);

                foreach (var subscription in subscriptions)
                {
                    try
                    {
                        byte[] photoBytes = alert.Victim?.VictimPhoto ?? GetPlaceholderImageBytes("victim");
                        using var photoStream = new MemoryStream(photoBytes);

                        var caption = FormatAlertMessage(alert);

                        await botClient.SendPhotoAsync(
                            chatId: subscription.ChatId,
                            photo: Telegram.Bot.Types.InputFile.FromStream(photoStream, "alert_photo.jpg"),
                            caption: caption,
                            parseMode: ParseMode.Html);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send alert {AlertId} to chat {ChatId}", id, subscription.ChatId);
                    }
                }

                return Ok(new { message = $"Alert sent to {subscriptions.Count} subscribers." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending alert {AlertId}", id);
                return StatusCode(500, new { message = "Error sending alert." });
            }
        }

        
        private string FormatAlertMessage(Alert alert)
        {
            return $"<b>🚨 НОВЕ ВИКРАДЕННЯ</b>\n" +
                   $"📍 Місце: {alert.CrimeLocation}\n" +
                   $"🕒 Час: {alert.CrimeDate}\n" +
                   $"👶 Ім'я: {alert.Victim.VictimName}\n" +
                   $"🔢 Вік: {alert.Victim.VictimAge}\n" +
                   $"🚻 Стать: {alert.Victim.VictimSex}\n" +
                   $"🎨 Колір волосся: {alert.Victim.VictimHair}\n" +
                   $"👕 Одяг: {alert.Victim.VictimClothing}\n" +
                   $"\n" +
                   $"Якщо Вам відома якась інформація повідомляйте за номером:\n" +
                   $"000-000-00";
        }

        [HttpPost("subscribe/{chatId}")]
        public async Task<IActionResult> Subscribe(long chatId)
        {
            _logger.LogInformation("Attempting to subscribe chat ID {ChatId}", chatId);

            try
            {
                var existingSubscription = await _context.Subscriptions
                    .FirstOrDefaultAsync(s => s.ChatId == chatId);

                if (existingSubscription != null)
                {
                    if (existingSubscription.IsActive)
                    {
                        _logger.LogInformation("Chat ID {ChatId} is already subscribed.", chatId);
                        return Ok(new { message = "You are already subscribed." });
                    }
                    else
                    {
                        existingSubscription.IsActive = true;
                        _logger.LogInformation("Chat ID {ChatId} resubscribed.", chatId);
                    }
                }
                else
                {
                    var subscription = new Subscription
                    {
                        ChatId = chatId,
                        IsActive = true
                    };
                    _context.Subscriptions.Add(subscription);
                    _logger.LogInformation("Chat ID {ChatId} successfully subscribed.", chatId);
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "You have been successfully subscribed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing chat ID {ChatId}", chatId);
                return StatusCode(500, new { message = "An error occurred while subscribing.", error = ex.Message });
            }
        }

        [HttpPost("unsubscribe/{chatId}")]
        public async Task<IActionResult> Unsubscribe(long chatId)
        {
            _logger.LogInformation("Attempting to unsubscribe chat ID {ChatId}", chatId);

            try
            {
                var subscription = await _context.Subscriptions
                    .FirstOrDefaultAsync(s => s.ChatId == chatId);

                if (subscription == null)
                {
                    _logger.LogInformation("Chat ID {ChatId} is not subscribed.", chatId);
                    return Ok(new { message = "You are not subscribed." });
                }

                subscription.IsActive = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Chat ID {ChatId} successfully unsubscribed.", chatId);
                return Ok(new { message = "You have been successfully unsubscribed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing chat ID {ChatId}", chatId);
                return StatusCode(500, new { message = "An error occurred while unsubscribing.", error = ex.Message });
            }
        }

        [HttpGet("subscriptionStatus/{chatId}")]
        public async Task<IActionResult> GetSubscriptionStatus(long chatId)
        {
            var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.ChatId == chatId);
            return Ok(new SubscriptionStatusDto { IsSubscribed = subscription?.IsActive == true });
        }
    }
}