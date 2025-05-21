using System;
using CampeonatinhoApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Context
{
    public class CampeonatinhoDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public CampeonatinhoDbContext(DbContextOptions<CampeonatinhoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Leagues)
                .WithOne(l => l.Country)
                .HasForeignKey(l => l.CountryId);

            modelBuilder.Entity<League>()
                .HasMany(l => l.Clubs)
                .WithOne(c => c.League)
                .HasForeignKey(c => c.LeagueId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
