using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Data;
using HotelListing.API.Exceptions;
using HotelListing.API.Models.Country;
using HotelListing.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    /*
     * Implementation for the Country Repository class*
     */
    public class CountryRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;


        //Dependency Injection for the Database
        public CountryRepository(HotelListingDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        //To Link Country and hotels tables
        public async Task<GetCountry> GetDetails(int id)
        {
            var country = await _context.Countries.Include(p => p.Hotels)
                .ProjectTo<GetCountry>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetDetails), id);
            }
            return country;
        }
    }
}
