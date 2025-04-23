using CampeonatinhoApp.Models;
using CampeonatinhoApp.Repositories;

namespace CampeonatinhoApp.Interfaces
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<Country> GetByName(string name);
    }
}
