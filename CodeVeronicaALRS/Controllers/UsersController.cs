using ALRS.DTO;
using ALRS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALRS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<Users> userManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = "0")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Entering {Action} to retrieve all users.", nameof(GetAllUsers));

            try
            {
                var users = await _userManager.Users.ToListAsync();

                _logger.LogInformation("Retrieved {UserCount} users in {Action}.", users.Count, nameof(GetAllUsers));

                var usersDto = users.Select(user => new UserDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    LoginWithIdentifier = user.LoginWithIdentifier,
                    Email = user.Email,
                    Role = user.Role
                }).ToList();

                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users.");
                return StatusCode(500, new { message = "An error occurred while retrieving users.", error = ex.Message });
            }
        }


        [Authorize(Roles = "0")]
        [HttpGet("user/{id}/show")]
        public async Task<IActionResult> GetUserById(int id)
        {
            _logger.LogInformation("Entering {Action} to retrieve user with ID: {UserId}.", nameof(GetUserById), id);

            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserId == id);

                if (user != null)
                {
                    _logger.LogInformation("User with ID {UserId} found in {Action}.", id, nameof(GetUserById));

                    var userDto = new UserDto
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        LoginWithIdentifier = user.LoginWithIdentifier,
                        Email = user.Email,
                        Role = user.Role
                    };

                    return Ok(userDto);
                }

                _logger.LogWarning("User with ID {UserId} not found in {Action}.", id, nameof(GetUserById));
                return NotFound(new { message = "User not found." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user with ID {UserId}.", id);
                return StatusCode(500, new { message = "An error occurred while updating alerts.", error = ex.Message });
            }
        }

        [Authorize(Roles = "0")]
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Entering {Action} to delete user with ID: {UserId}.", nameof(DeleteUser), id);

            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserId == id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found in {Action}.", id, nameof(DeleteUser));
                    return NotFound(new { message = "User not found." });
                }

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to delete user with ID {UserId} in {Action}. Errors: {@Errors}", id, nameof(DeleteUser), result.Errors);
                    return BadRequest(new { message = "Error deleting user.", errors = result.Errors });
                }

                _logger.LogInformation("User with ID {UserId} deleted successfully in {Action}.", id, nameof(DeleteUser));
                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID {UserId} in {Action}.", id, nameof(DeleteUser));
                return StatusCode(500, new { message = "An error occurred while deleting user.", error = ex.Message });
            }
        }
    }
}