using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using CampeonatinhoApp.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using System.Web;

namespace CampeonatinhoApp.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSenderService _emailSenderService;
        private readonly ITokenRepository _tokenRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            EmailSenderService emailSenderService,
            ITokenRepository tokenRepository,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _emailSenderService = emailSenderService;
            _tokenRepository = tokenRepository;
            _logger = logger;
        }

        public async Task<IList<ApplicationUser>> GetAllUsersAsync()
        {
            return await Task.FromResult(_userManager.Users.ToList());
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IdentityResult> RegisterAsync(RegisterUserDTO model, Func<string, string, string> urlGenerator)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                BirthDate = model.BirthDate,
                Gender = model.Gender,
                ChampionshipsPlayed = 0,
                FavoriteTeamId = model.FavoriteTeamId,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (model.Roles != null && model.Roles.Any())
                {
                    await _userManager.AddToRolesAsync(user, model.Roles);
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = urlGenerator(user.Id, code);

                await _emailSenderService.SendEmailAsync(user.Email!, "Confirm your Email", $"Please confirm your account by clicking here: <a href='{HtmlEncoder.Default.Encode(confirmationLink!)}'>link</a>.");

                _logger.LogInformation($"User {user.UserName} registered successfully.");
            }
            else
            {
                _logger.LogError($"Error registering user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return result;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Email confirmed for user {user.Email}.");
            }
            else
            {
                _logger.LogError($"Could not confirm the email for user {user.Email}.");
            }

            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(string id, UserProfileDTO model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            if (user.Id != id)
            { 
                return IdentityResult.Failed(new IdentityError { Description = "User Id mismatch." });
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
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} updated successfully.");
            }

            return result;
        }

        public async Task<IdentityResult> ForgotPasswordAsync(string email, Func<string, string, string> urlGenerator)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found or email not confirmed." });
            }

            var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetTokenUrl = urlGenerator(user.Id, passwordToken);

            await _emailSenderService.SendEmailAsync(user.Email!, "Reset your Password", $"Please reset your password by clicking here: <a href='{HtmlEncoder.Default.Encode(resetTokenUrl!)}'>link</a>");

            _logger.LogInformation($"Email to reset password sent to {user.Email}.");
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ChangePasswordAsync(UserChangePasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            var decodedToken = HttpUtility.UrlDecode(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Password reset for user {user.Email}.");
            }

            return result;
        }

        public async Task<(bool Succeeded, string? Token, string? ErrorMessage)> LoginAsync(LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    _logger.LogInformation($"User {model.Username} does not have a verified account.");
                    return (false, null, "The user does not have a verified account. Please verify it.");
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (passwordValid)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var jwtToken = _tokenRepository.CreateJwtToken(user, roles.ToList());
                    _logger.LogInformation($"User {model.Username} logged in successfully.");
                    return (true, jwtToken, null);
                }
            }

            _logger.LogInformation($"Invalid username or password for {model.Username}.");
            return (false, null, "Invalid username or password.");
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} successfully deleted.");
            }

            return result;
        }
    }
}
