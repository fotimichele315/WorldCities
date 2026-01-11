using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace WorldCities.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities => Set<City>();
        public DbSet<Country> Countries => Set<Country>();

                                                      
    }
}
