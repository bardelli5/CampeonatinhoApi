using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CampeonatinhoApp.Models.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
