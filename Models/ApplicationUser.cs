using CampeonatinhoApp.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CampeonatinhoApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }

        [EnumDataType(typeof(GenderType))]
        public GenderType Gender { get; set; }
        public int ChampionshipsPlayed { get; set; }
        public int FavoriteTeamId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
