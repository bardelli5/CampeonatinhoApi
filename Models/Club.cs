using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CampeonatinhoApp.Models
{
    public class Club
    {
        [Key]
        public int? Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string? Abbreviation { get; set; }

        [JsonProperty(PropertyName = "founded")]
        public int? Founded { get; set; }

        [JsonProperty(PropertyName = "logo")]
        public string? LogoUrl { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int? ApiId { get; set; }

        public int? ApiLeagueId { get; set; }


        public int LeagueId { get; set; }
        public League League { get; set; }
    }
}
