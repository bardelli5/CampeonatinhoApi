using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using CampeonatinhoApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Sprache;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace CampeonatinhoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserProfileDto> _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSenderService _emailSenderService;

        public UserController(ILogger<UserProfileDto> logger, IUserRepository userRepository, UserManager<ApplicationUser> userManager, EmailSenderService emailSenderService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
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

                await _emailSenderService.SendEmailAsync(user.Email, "Confirme o seu email", $"Por favor, confirme sua conta em: <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicando aqui</a>.");

                _logger.LogInformation($"User {user.UserName} registered successfully.");
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            else
            {
                _logger.LogError($"Error registering user {user.UserName}: {string.Join(", ", createdUser.Errors.Select(e => e.Description))}");
                return BadRequest(createdUser.Errors);
            }
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var decodedToken = HttpUtility.UrlDecode(code);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                return Ok("Obrigado por confirmar seu e-mail. Sua conta agora está verificada!");
            }

            return BadRequest("Não foi possível confirmar seu e-mail.");
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

        //[HttpPut("reset-password/{id}")]
        //public async Task<ActionResult> ResetPassoword(Guid id, [FromBody] UserChangePasswordDto model, string token)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return NotFound("There is no user with that email.");
        //    }

        //    var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        //    //if (!ModelState.IsValid)
        //    //{ return BadRequest(ModelState); }
        //    //if (model.Id != id)
        //    //{ return BadRequest("User ID mismatch."); }

        //    //var user = await _userManager.FindByIdAsync(model.Id.ToString());
        //    //if (user == null)
        //    //{
        //    //    return NotFound("User not found.");
        //    //}

        //    //var result = await _userManager.ResetPasswordAsync(user, model.CurrentPassword, model.NewPassword);
        //    //if (result.Succeeded)
        //    //{
        //    //    _logger.LogInformation($"Password for {user.UserName} changed.");
        //    //    return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        //    //}
        //    //else
        //    //{
        //    //    _logger.LogError($"Error changing password for {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        //    //    return BadRequest(result.Errors);
        //    //}
        //}

        [HttpPut("change-password/{id}")]
        public async Task<ActionResult> ChangePassword(Guid id, [FromBody] UserChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }
            if (model.Id != id)
            { return BadRequest("User ID mismatch."); }

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

        public void funcaoFodase(int numero)
        {
            numero = 22;
            Console.WriteLine("numero da beca: " + numero);
        }
    }
}
