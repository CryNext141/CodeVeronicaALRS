using ALRS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALRS.Controllers
{
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

        //[Authorize(Roles = "0")]
        [HttpGet("users")]
        public async Task<List<Users>> GetAllUsers()
        {
            _logger.LogInformation("Entering {Action} to retrieve all users.", nameof(GetAllUsers));

            var users = await _userManager.Users.ToListAsync();

            _logger.LogInformation("Retrieved {UserCount} users in {Action}.", users.Count, nameof(GetAllUsers));
            return users;
        }

        [Authorize(Roles = "1")]
        [HttpGet("user/{id}/show")]
        public async Task<IActionResult> GetUserById(int id)
        {
            _logger.LogInformation("Entering {Action} to retrieve user with ID: {UserId}.", nameof(GetUserById), id);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (user != null)
            {
                _logger.LogInformation("User with ID {UserId} found in {Action}.", id, nameof(GetUserById));
                return Ok(user);
            }

            _logger.LogWarning("User with ID {UserId} not found in {Action}.", id, nameof(GetUserById));
            return NotFound(new { message = "User not found." });
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
