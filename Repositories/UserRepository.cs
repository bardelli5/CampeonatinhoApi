using CampeonatinhoApp.Context;
using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        protected readonly DbContext _context;
        protected readonly DbSet<ApplicationUser> _dbSet;

        public UserRepository(CampeonatinhoDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<ApplicationUser>();
        }
        public Task<ApplicationUser> GetByUserName(string username) 
        {
            return _dbSet.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public Task<ApplicationUser> GetByName(string name)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.FullName == name);
        }

        public Task<ApplicationUser> GetByEmail(string email)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
