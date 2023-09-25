using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace HotelListing.API.Data.Configurations
{
    public class CountriesConfiguration : IEntityTypeConfiguration<Country>
    {
        /*
         * Configurations to seed the Country Table**/
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
               new Country { Id = 1, Name = "Egypt", ShortName = "EGY" },
               new Country { Id = 2, Name = "America", ShortName = "USA" },
               new Country { Id = 3, Name = "England", ShortName = "UK" });
        }
    }
}
