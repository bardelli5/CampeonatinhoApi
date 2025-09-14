using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using CampeonatinhoApp.Models.DTOs;
using CampeonatinhoApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using System.Text.Encodings.Web;
using System.Web;

namespace CampeonatinhoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserProfileDTO> _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSenderService _emailSenderService;

        public UserController(ILogger<UserProfileDTO> logger, IUserRepository userRepository, UserManager<ApplicationUser> userManager, EmailSenderService emailSenderService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
        }

        [HttpGet("all")]
        public ActionResult<IList<UserProfileDTO>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return users == null ? BadRequest("Invalid Request.") : Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(Guid id)
        {
            var users = await _userManager.FindByIdAsync(id.ToString());
            return users == null ? BadRequest("Invalid Request.") : Ok(users);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserDTO model)
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
                FavoriteTeamId = model.FavoriteTeamId, //id do spfc = 43
                EmailConfirmed = false
            };

            var createdUser = await _userManager.CreateAsync(user, model.Password);
            if (createdUser.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = HttpUtility.UrlEncode(code);
                var confirmationLink = Url.Action(
                    "ConfirmEmail",
                    "User",
                    values: new { userId = user.Id, code = code },
                    protocol: Request.Scheme
                    );

                await _emailSenderService.SendEmailAsync(user.Email, "Confirm your Email", $"Please confirm your account by clicking here: <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>link</a>.");

                _logger.LogInformation($"User {user.UserName} registered successfully.");
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            else
            {
                _logger.LogError($"Error registering user {user.UserName}: {string.Join(", ", createdUser.Errors.Select(e => e.Description))}");
                return BadRequest(createdUser.Errors);
            }
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("Invalid Request.");
            }

            var decodedToken = HttpUtility.UrlDecode(code);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                return Ok("Thank you for confirming your email. Your account is now verified!");
            }

            return BadRequest("Could not confirm the email.");
        }

        [HttpPut("edit/{id}")]
        public async Task<ActionResult> EditUser(Guid id, [FromBody] UserProfileDTO model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }
            if (model.Id != id.ToString())
            { return BadRequest("User Id mismatch."); }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return BadRequest("Invalid Request.");
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
                user.FavoriteTeamId = (int) model.FavoriteTeamId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(user);
        }

        [HttpPost("resetpassword")]
        public async Task<ActionResult> ResetPassoword([FromBody] ForgotPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    return BadRequest("Invalid Request.");
                }
                var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user!);
                var resetTokenUrl = Url.Action("ChangePassword", "User", new { UserId = user.Id, token = passwordToken }, protocol: Request.Scheme);
                await _emailSenderService.SendEmailAsync(user.Email!, "Reset your Password", $"Please reset your password by clicking here: <a href='{HtmlEncoder.Default.Encode(resetTokenUrl)}'>link</a>");
            }

            return Ok();
        }

        [HttpPut("changepassword")]
        public async Task<ActionResult> ChangePassword([FromBody] UserChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Invalid Request.");
            }

            var decodedToken = HttpUtility.UrlDecode(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }
            return Ok("Password reseted!");
        }


        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return BadRequest("Invalid Request.");
            }

            await _userManager.DeleteAsync(user);
            _logger.LogInformation($"User {user.UserName} successfully deleted.");
            return NoContent();
        }
    }
}
