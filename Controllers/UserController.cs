using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using CampeonatinhoApp.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampeonatinhoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IList<ApplicationUser>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? BadRequest("Invalid Request.") : Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var result = await _userService.RegisterAsync(model, (userId, code) =>
            {
                return Url.Action(
                    "ConfirmEmail",
                    "User",
                    new { userId = userId, code = code },
                    protocol: Request.Scheme
                )!;
            });

            if (result.Succeeded)
            {
                var user = await _userService.GetUserByIdAsync(model.UserName); 
                return Ok("User registered successfully. Please check your email to confirm.");
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var result = await _userService.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return Ok("Thank you for confirming your email. Your account is now verified!");
            }

            return BadRequest("Could not confirm the email.");
        }

        [HttpPut("edit/{id}")]
        public async Task<ActionResult> EditUser(string id, [FromBody] UserProfileDTO model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var result = await _userService.UpdateUserAsync(id, model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User updated successfully.");
        }

        [HttpPost("forgotPassword")]
        public async Task<ActionResult> ResetPassword([FromBody] ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var result = await _userService.ForgotPasswordAsync(model.Email, (userId, token) =>
            {
                return Url.Action("ChangePassword", "User", new { UserId = userId, token = token }, protocol: Request.Scheme)!;
            });

            if (result.Succeeded)
            {
                return Ok("Email to reset password sent.");
            }

            return BadRequest("Invalid Request.");
        }

        [HttpPut("changePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] UserChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var result = await _userService.ChangePasswordAsync(model);
            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            return Ok("Password reset.");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var (succeeded, token, errorMessage) = await _userService.LoginAsync(model);
            if (succeeded)
            {
                return Ok(new { Token = token });
            }

            if (errorMessage == "The user does not have a verified account. Please verify it.")
            {
                return Forbid(errorMessage);
            }

            return Unauthorized(errorMessage);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest("Invalid Request.");
        }
    }
}
