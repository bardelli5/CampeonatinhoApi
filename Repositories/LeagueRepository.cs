using CampeonatinhoApp.Context;
using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Repositories
{
    public class LeagueRepository : GenericRepository<LeagueRepository>, ILeagueRepository
    {
        public LeagueRepository(CampeonatinhoDbContext context) : base(context)
        {
        }
    }
}
