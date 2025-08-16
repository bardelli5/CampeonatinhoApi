using Microsoft.AspNetCore.Identity;

namespace CampeonatinhoApp.Models
{
    public class UserProfileDTO
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public int? ChampionshipsPlayed { get; set; }
        public int? FavoriteTeamId { get; set; }
        public string? FavoriteTeamName { get; set; }
        public string? FavoriteTeamLogo { get; set; }
    }
}
