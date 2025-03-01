using System;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Context
{
    public class CampeonatinhoDbContext : DbContext
    {
        public CampeonatinhoDbContext(DbContextOptions<CampeonatinhoDbContext> options) : base(options) { }


    }
}
