using System.ComponentModel.DataAnnotations;

namespace CampeonatinhoApp.Models
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ClientUri { get; set; }

    }
}
