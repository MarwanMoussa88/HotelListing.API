using HotelListing.API.Data;
using HotelListing.API.Repository.IRepository;

namespace HotelListing.API.Repository
{
    public class HotelRepository : GenericRepository<Hotel> , IHotelRepository
    {
        public HotelRepository(HotelListingDbContext context) : base(context)
        {
        }
    }
}
