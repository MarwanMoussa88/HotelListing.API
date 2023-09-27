using HotelListing.API.Data.Configurations;
using HotelListing.API.Models.Country;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data
{

    /*
     * Entity Framework Core DB Context*/
    public class HotelListingDbContext : IdentityDbContext<ApiUser>
    {
        public HotelListingDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new HotelsConfiguration());
            modelBuilder.ApplyConfiguration(new CountriesConfiguration());

        }

        public DbSet<HotelListing.API.Models.Country.GetCountry> GetCountry { get; set; }
    }
}
