using Microsoft.AspNetCore.Identity;

namespace CampeonatinhoApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public int ChampionshipsPlayed { get; set; }
        public string FavoriteTeam { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
