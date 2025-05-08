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
        private readonly IClubRepository _clubRepository;

        public FootballApiRequestService(
            CampeonatinhoDbContext context,
            ILeagueRepository leagueRepository,
            ICountryRepository countryRepository,
            IClubRepository clubRepository)
        {
            _dbContext = context;
            _leagueRepository = leagueRepository;
            _countryRepository = countryRepository;
            _clubRepository = clubRepository;
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
                    JToken countryToken = result["country"];
                    JToken leagueToken = result["league"];

                    Country c = JsonConvert.DeserializeObject<Country>(countryToken.ToString());
                    League l = JsonConvert.DeserializeObject<League>(leagueToken.ToString());

                    var buscaCountry = _countryRepository.SearchAsync(x => x.Name == c.Name).GetAwaiter().GetResult();
                    l.CountryId = buscaCountry.FirstOrDefault().Id;

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

        public IResult GetApiDataClubs()
        {
            try
            {
                var buscaLeague = _leagueRepository.GetAllAsync().GetAwaiter().GetResult().ToList();

                foreach (var liga in buscaLeague)
                {
                    var buscaClub = _clubRepository.GetByApiLeagueId(liga.ApiId).GetAwaiter().GetResult();
                    if (buscaClub == null)
                    {
                        var client = new RestClient($"https://v3.football.api-sports.io/teams?league={liga.ApiId}&season=2023");
                        var request = new RestRequest().AddHeader("x-rapidapi-key", "01ab0dce91ecea8d9923643ad0d75f4c").AddHeader("x-rapidapi-host", "v3.football.api-sports.io");
                        var response = client.Execute(request);

                        string jsonLeague = response.Content;
                        JObject jsonLeagueParse = JObject.Parse(jsonLeague);
                        IList<JToken> results = jsonLeagueParse["response"].Children().ToList();

                        if (results.Count() > 0)
                        {
                            foreach (JToken result in results)
                            {
                                JToken clubToken = result["team"];
                                Club cl = JsonConvert.DeserializeObject<Club>(clubToken.ToString());

                                cl.LeagueId = liga.Id;
                                cl.ApiLeagueId = liga.ApiId;
                                _dbContext.Add(cl);
                                _dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Results.BadRequest();
            }

            return Results.Ok();
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
                    if (c.Abbreviation == null && c.FlagUrl == null)
                    {
                        c.Abbreviation = "WORLD";
                        c.FlagUrl = "https://w7.pngwing.com/pngs/560/279/png-transparent-world-map-globe-map-projection-world-map-miscellaneous-globe-logo-thumbnail.png";
                    }
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
