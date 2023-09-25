using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace HotelListing.API.Data.Configurations
{
    public class HotelsConfiguration : IEntityTypeConfiguration<Hotel>
    {


        /*
         * Configurations to seed the Hotel Table**/
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel { Id = 1, Address = "Corniche St", Name = "Sunrie Makadi", Rating = 4.5, CountryId = 1 },
                new Hotel { Id = 2, Address = "Mansheya", Name = "Winsdoor Palace", Rating = 4, CountryId = 1 },
                new Hotel { Id = 3, Address = "Montaza Palace", Name = "Helanan Palestine", Rating = 5, CountryId = 1 });
        }
    }
}
