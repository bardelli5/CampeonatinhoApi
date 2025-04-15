using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CampeonatinhoApp.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Abbreviation { get; set; }

        [JsonProperty(PropertyName = "flag")]
        public string FlagUrl { get; set; }

        public ICollection<League> Leagues { get; set; } = new List<League>();
    }
}
