using CampeonatinhoApp.Context;
using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using CampeonatinhoApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CampeonatinhoApp.Services
{
    public class FootballApiRequestService
    {
        private readonly CampeonatinhoDbContext _dbContext;
        private readonly ILeagueRepository _leagueRepository;
        private readonly ICountryRepository _countryRepository;

        public FootballApiRequestService(
            CampeonatinhoDbContext context,
            ILeagueRepository leagueRepository,
            ICountryRepository countryRepository)
        {
            _dbContext = context;
            _leagueRepository = leagueRepository;
            _countryRepository = countryRepository;
        }


        public IResult GetApiDataLeagues()
        {
            try
            {
                var client = new RestClient("https://v3.football.api-sports.io/leagues");
                var request = new RestRequest().AddHeader("x-rapidapi-key", "01ab0dce91ecea8d9923643ad0d75f4c").AddHeader("x-rapidapi-host", "v3.football.api-sports.io");
                var response = client.Execute(request);

                string jsonLeague = response.Content;

                JObject jsonLeagueParse = JObject.Parse(jsonLeague);
                IList<JToken> results = jsonLeagueParse["response"].Children().ToList();

                foreach (JToken result in results)
                {
                    JToken leagueToken = result["league"];
                    JToken countryToken = result["country"];
                    _countryRepository.GetByName("teste");

                    League l = JsonConvert.DeserializeObject<League>(leagueToken.ToString());
                    _dbContext.Add(l);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Results.BadRequest();
            }

            return Results.Ok();
        }

        public string GetApiDataClubs()
        {
            var client = new RestClient("https://v3.football.api-sports.io/teams?league=39&season=2023");
            var request = new RestRequest().AddHeader("x-rapidapi-key", "01ab0dce91ecea8d9923643ad0d75f4c").AddHeader("x-rapidapi-host", "v3.football.api-sports.io");
            var response = client.Execute(request);

            return response.Content;
        }

        public IResult GetApiDataCountries()
        {
            try
            {
                var client = new RestClient("https://v3.football.api-sports.io/countries");
                var request = new RestRequest().AddHeader("x-rapidapi-key", "01ab0dce91ecea8d9923643ad0d75f4c").AddHeader("x-rapidapi-host", "v3.football.api-sports.io");
                var response = client.Execute(request);

                string jsonCountry = response.Content;

                JObject jsonCountryParse = JObject.Parse(jsonCountry);
                IList<JToken> results = jsonCountryParse["response"].Children().ToList();

                foreach (JToken result in results)
                {
                    Country c = JsonConvert.DeserializeObject<Country>(result.ToString());
                    _dbContext.Add(c);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Results.BadRequest();
            }

            return Results.Ok();
        }
    }
}
