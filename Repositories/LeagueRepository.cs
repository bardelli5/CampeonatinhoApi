using CampeonatinhoApp.Context;
using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Repositories
{
    public class LeagueRepository : GenericRepository<League>, ILeagueRepository
    {
        protected readonly DbContext _context;
        protected readonly DbSet<League> _dbSet;
        public LeagueRepository(CampeonatinhoDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<League>();
        }

        public Task<League> GetByName(string name)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
