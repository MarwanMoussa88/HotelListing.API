using HotelListing.API.Data;

namespace HotelListing.API.Repository.IRepository

    /*
     * Applying the generic repository to the Country Model
     * Here goes any functions that are related to only the Country model
     * **/
{
    public interface ICountriesRepository : IGenericRepository<Country>
    {
        Task<Country> GetDetails(int id); 
    }
}
