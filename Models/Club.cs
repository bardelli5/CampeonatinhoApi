namespace CampeonatinhoApp.Models
{
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public int Founded { get; set; }
        public string LogoUrl { get; set; }


        public int LeagueId { get; set; }
        public League League { get; set; }
    }
}
