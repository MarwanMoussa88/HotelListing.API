using HotelListing.API.Data;

namespace HotelListing.API.Repository.IRepository
{
    /*
     * Applying the generic repository to the Hotel Model
     * Here goes any functions that are related to only the hotel model
     * **/
    public interface IHotelRepository : IGenericRepository<Hotel>
    {

    }
}
