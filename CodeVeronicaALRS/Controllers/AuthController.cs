using ALRS.Data;
using ALRS.DTO;
using ALRS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ALRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly ApplicationDbContext _context;

        public AuthController(UserManager<Users> userManager, SignInManager<Users> signInManager, IConfiguration configuration, ILogger<AuthController> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            _logger.LogInformation("Entering {Action} to register user with email: {Email}", nameof(Register), model.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in {Action}.", nameof(Register));
                return BadRequest(ModelState);
            }

            if (model.Role < 0 || model.Role > 2)
            {
                _logger.LogWarning("Invalid role provided: {Role} in {Action}.", model.Role, nameof(Register));
                return BadRequest(new { message = "Invalid role provided. Role must be 0, 1, or 2." });
            }

            string roleName = model.Role switch
            {
                0 => "HR Officer",
                1 => "CodeVeronica Alert Coordinator",
                2 => "Admin",
                _ => "Unknown"
            };

            try
            {
                var random = new Random();
                var identifier = "#" + random.Next(10000, 99999).ToString();

                var user = new Users
                {
                    Name = model.Name,
                    UserName = $"{model.Login}{identifier}",
                    Email = model.Email,
                    Role = model.Role,
                    LoginWithIdentifier = $"{model.Login}{identifier}"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("User creation failed for {Email} in {Action}. Errors: {@Errors}", model.Email, nameof(Register), result.Errors);
                    return BadRequest(result.Errors);
                }

                _logger.LogInformation("User {Email} registered successfully in {Action}.", user.Email, nameof(Register));

                return Ok(new
                {
                    message = "User registered successfully.",
                    login = user.LoginWithIdentifier,
                    role = roleName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration in {Action}.", nameof(Register));
                return StatusCode(500, new { message = "An error occurred while registering the user.", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            _logger.LogInformation("Entering {Action} for user login attempt: {Login}", nameof(Login), model.Login);

            try
            {
                var user = await _userManager.FindByNameAsync(model.Login);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: user not found for {Login}.", model.Login);
                    return Unauthorized(new { message = "User not found." });
                }

                if (user.Role != model.Role)
                {
                    _logger.LogWarning("Login failed: role mismatch for {Login}.", model.Login);
                    return Unauthorized(new { message = "Role mismatch." });
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login failed: invalid credentials for {Login}.", model.Login);
                    return Unauthorized(new { message = "Invalid credentials." });
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("Login successful for {Login}. Token generated.", user.LoginWithIdentifier);

                return Ok(new
                {
                    token,
                    login = user.LoginWithIdentifier,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login attempt in {Action}.", nameof(Login));
                return StatusCode(500, new { message = "An error occurred while processing the login.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            _logger.LogInformation("Entering {Action} for token logout. JTI: {Jti}", nameof(Logout), jti);

            try
            {
                if (!string.IsNullOrEmpty(jti))
                {
                    _context.BlacklistedTokens.Add(new BlacklistedToken
                    {
                        TokenId = jti,
                        ExpirationDate = DateTime.UtcNow.AddHours(1)
                    });

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Token with JTI {Jti} blacklisted successfully in {Action}.", jti, nameof(Logout));
                }

                return Ok(new { message = "Token blacklisted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout in {Action}.", nameof(Logout));
                return StatusCode(500, new { message = "An error occurred while logging out.", details = ex.Message });
            }
        }

        private string GenerateJwtToken(Users user)
        {
            try
            {
                _logger.LogInformation("Generating JWT token for {UserName}", user.UserName);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                _logger.LogInformation("JWT token generated successfully for {UserName}.", user.UserName);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the JWT token for {UserName}.", user.UserName);
                throw new InvalidOperationException("Error generating JWT token", ex);
            }
        }

        private async Task<bool> IsTokenBlacklisted(string jti)
        {
            _logger.LogInformation("Checking if token with JTI {Jti} is blacklisted.", jti);
            return await _context.BlacklistedTokens.AnyAsync(t => t.TokenId == jti);
        }
    }
}
