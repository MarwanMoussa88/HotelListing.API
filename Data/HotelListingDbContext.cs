using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data
{

    //Entity Framework Core DB Context
    public class HotelListingDbContext : DbContext
    {
        public HotelListingDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "Egypt", ShortName = "EGY" },
                new Country { Id = 2, Name = "America", ShortName = "USA" },
                new Country { Id = 3, Name = "England", ShortName = "UK" });
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { Id = 1, Address="Corniche St",Name="Sunrie Makadi",Rating=4.5,CountryId=1},
                new Hotel { Id = 2, Address = "Mansheya", Name = "Winsdoor Palace", Rating =4 ,CountryId=1},
                new Hotel { Id = 3, Address = "Montaza Palace", Name = "Helanan Palestine", Rating = 5 ,CountryId=1});   
        }
    }
}
