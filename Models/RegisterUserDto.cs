﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CampeonatinhoApp.Models
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Gender { get; set; }

        [Required(ErrorMessage = "O time favorito é obrigatório.")]
        public int FavoriteTeamId { get; set; }
    }
}
