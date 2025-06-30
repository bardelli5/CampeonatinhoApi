using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserProfileDto> _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ILogger<UserProfileDto> logger, IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [HttpGet("all")]
        public ActionResult<IList<UserProfileDto>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return users == null ? NotFound("Users not founded.") : Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(Guid id)
        {
            var users = await _userManager.FindByIdAsync(id.ToString());
            return users == null ? NotFound("User not found.") : Ok(users);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserDto model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                BirthDate = model.BirthDate,
                Gender = model.Gender,
                ChampionshipsPlayed = 0,
                FavoriteTeamId = model.FavoriteTeamId
                //id do spfc = 43
            };

            var createdUser = await _userManager.CreateAsync(user, model.Password);
            if (createdUser.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} registered successfully.");
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            else
            {
                _logger.LogError($"Error registering user {user.UserName}: {string.Join(", ", createdUser.Errors.Select(e => e.Description))}");
                return BadRequest(createdUser.Errors);
            }
        }

        [HttpPut("edit/{id}")]
        public async Task<ActionResult> EditUser(Guid id, [FromBody] UserProfileDto model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }
            if (model.Id != id.ToString())
            {return BadRequest("User ID mismatch.");}

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!string.IsNullOrWhiteSpace(model.FullName))
                user.FullName = model.FullName;

            if (!string.IsNullOrWhiteSpace(model.Email))
                user.Email = model.Email;

            if (model.BirthDate.HasValue)
                user.BirthDate = model.BirthDate.Value;

            if (!string.IsNullOrWhiteSpace(model.Gender))
                user.Gender = model.Gender;

            if (model.FavoriteTeamId.HasValue)
                user.FavoriteTeamId = (int)model.FavoriteTeamId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(user);
        }


        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword(UserChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Password for {user.UserName} changed.");
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            else
            {
                _logger.LogError($"Error changing password for {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return BadRequest(result.Errors);
            }
        }


        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userManager.DeleteAsync(user);
            _logger.LogInformation($"User {user.UserName} successfully deleted.");
            return NoContent();
        }
    }
}
