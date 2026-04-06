using CampeonatinhoApp.Models;
using CampeonatinhoApp.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace CampeonatinhoApp.Interfaces
{
    public interface IUserService
    {
        Task<IList<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<IdentityResult> RegisterAsync(RegisterUserDTO model, Func<string, string, string> urlGenerator);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string code);
        Task<IdentityResult> UpdateUserAsync(string id, UserProfileDTO model);
        Task<IdentityResult> ForgotPasswordAsync(string email, Func<string, string, string> urlGenerator);
        Task<IdentityResult> ChangePasswordAsync(UserChangePasswordDTO model);
        Task<(bool Succeeded, string? Token, string? ErrorMessage)> LoginAsync(LoginDTO model);
        Task<IdentityResult> DeleteUserAsync(string id);
    }
}
