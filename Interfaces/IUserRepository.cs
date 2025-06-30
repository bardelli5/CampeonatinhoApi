using CampeonatinhoApp.Models;
using CampeonatinhoApp.Repositories;

namespace CampeonatinhoApp.Interfaces
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByUserName(string userName);
        Task<ApplicationUser> GetByName(string name);
        Task<ApplicationUser> GetByEmail(string email);
    }
}
