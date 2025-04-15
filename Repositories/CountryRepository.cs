using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Repositories
{
    public class CountryRepository : GenericRepository<CountryRepository>, ICountryRepository
    {
        protected readonly DbContext _context;
        protected readonly DbSet<Country> _dbSet;

        public CountryRepository(DbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Country>();
        }

        public Task<Country> GetByName(string name)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
