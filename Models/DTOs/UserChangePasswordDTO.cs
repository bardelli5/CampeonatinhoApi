using System.ComponentModel.DataAnnotations;

namespace CampeonatinhoApp.Models.DTOs
{
    public class UserChangePasswordDTO
    {
        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "A confirmação da nova senha não corresponde à nova senha.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

    }
}
