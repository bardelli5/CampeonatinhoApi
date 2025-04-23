using CampeonatinhoApp.Models;
using CampeonatinhoApp.Repositories;

namespace CampeonatinhoApp.Interfaces
{
    public interface IFootballApiRequestService
    {
        public IResult GetApiDataLeagues();
        public IResult GetApiDataClubs();
        public IResult GetApiDataCountries();

    }
}
