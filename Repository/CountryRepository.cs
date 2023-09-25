using HotelListing.API.Data;
using HotelListing.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    /*
     * Implementation for the Country Repository class*
     */
    public class CountryRepository :GenericRepository<Country> , ICountriesRepository
    {
        private readonly HotelListingDbContext _context;

        //Dependency Injection for the Database
        public CountryRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context;
        }

        //To Link Country and hotels tables
        public async Task<Country> GetDetails(int id)
        {
            return await _context.Countries.Include(p => p.Hotels)
                .FirstOrDefaultAsync(p=>p.Id==id);   
        }
    }
}
