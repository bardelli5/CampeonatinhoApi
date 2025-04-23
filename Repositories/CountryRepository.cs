using CampeonatinhoApp.Context;
using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Repositories
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        public CountryRepository(CampeonatinhoDbContext context) : base(context)
        {
        }

        public Task<Country> GetByName(string name)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
