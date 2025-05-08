using CampeonatinhoApp.Context;
using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Repositories
{
    public class ClubRepository : GenericRepository<Club>, IClubRepository
    {
        protected readonly DbContext _context;
        protected readonly DbSet<Club> _dbSet;
        public ClubRepository(CampeonatinhoDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Club>();
        }

        public Task<Club> GetByName(string name)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        }

        public Task<Club> GetByApiLeagueId(int apiLeagueId) 
        {
            return _dbSet.FirstOrDefaultAsync(x => x.ApiLeagueId == apiLeagueId);
        }
    }
}
