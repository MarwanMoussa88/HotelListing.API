using HotelListing.API.Data;
using HotelListing.API.Models.Country;

namespace HotelListing.API.Repository.IRepository

/*
 * Applying the generic repository to the Country Model
 * Here goes any functions that are related to only the Country model
 * **/
{
    public interface ICountriesRepository : IGenericRepository<Country>
    {
        Task<GetCountry> GetDetails(int id);
    }
}
