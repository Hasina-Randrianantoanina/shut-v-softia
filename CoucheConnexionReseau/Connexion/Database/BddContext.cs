using Connexion.Entities;
using Microsoft.EntityFrameworkCore;

namespace Connexion.Database
{
    public class BddContext : DbContext
    {
        public DbSet<Station> Stations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=stations_test;Username=postgres;Password=rrrrr;");
        }
    }
}
