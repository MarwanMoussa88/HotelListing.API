using HotelListing.API.Data;
using HotelListing.API.Repository.IRepository;

namespace HotelListing.API.Repository
{
    /*
     * Implementation for Hotel Repository Class from Generic Repository
     * **/
    public class HotelRepository : GenericRepository<Hotel> , IHotelRepository
    {
        //Dependency Injection for Generic Class
        public HotelRepository(HotelListingDbContext context) : base(context)
        {
        }
    }
}
