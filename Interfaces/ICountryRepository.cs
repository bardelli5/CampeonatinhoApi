using CampeonatinhoApp.Models;
using CampeonatinhoApp.Repositories;

namespace CampeonatinhoApp.Interfaces
{
    public interface ICountryRepository : IGenericRepository<CountryRepository>
    {
        Task<Country> GetByName(string name);
    }
}
