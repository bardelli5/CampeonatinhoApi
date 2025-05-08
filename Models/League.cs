using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CampeonatinhoApp.Models
{
    public class League
    {
        [Key]
        public int Id { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "logo")]
        public string ImageUrl { get; set; }
        public int CountryId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int ApiId { get; set; }


        public Country Country { get; set; }
        public ICollection<Club> Clubs { get; set; } = new List<Club>();
    }
}
