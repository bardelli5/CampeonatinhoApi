using CampeonatinhoApp.Models;
using CampeonatinhoApp.Repositories;

namespace CampeonatinhoApp.Interfaces
{
    public interface IClubRepository : IGenericRepository<Club>
    {
        Task<Club> GetByName(string name);
        Task<Club> GetByApiLeagueId(int apiLeagueId);
    }
}
