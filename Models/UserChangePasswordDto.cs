using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CampeonatinhoApp.Models
{
    public class UserChangePasswordDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A senha atual é obrigatória.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        public string NewPassword { get; set; }

        //[Required(ErrorMessage = "A confirmação da nova senha é obrigatória.")]
        //[Compare("NewPassword", ErrorMessage = "A confirmação da nova senha não corresponde à nova senha.")]
        //public string ConfirmNewPassword { get; set; }

        public string Email { get; set; }

    }
}
