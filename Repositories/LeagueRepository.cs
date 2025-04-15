using CampeonatinhoApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Repositories
{
    public class LeagueRepository : GenericRepository<LeagueRepository>, ILeagueRepository
    {
        public LeagueRepository(DbContext context) : base(context)
        {
        }
    }
}
