using CampeonatinhoApp.Models;
using CampeonatinhoApp.Repositories;

namespace CampeonatinhoApp.Interfaces
{
    public interface ILeagueRepository : IGenericRepository<League>
    {
        Task<League> GetByName(string name);
    }
}
