using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using HotelListing.API.Models.QueryParameters;
using HotelListing.API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Controllers
{
    //Controller Route
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CountriesController : ControllerBase
    {
        private readonly ICountriesRepository _countries;
        private readonly IMapper _mapper;

        public CountriesController(ICountriesRepository countries, IMapper mapper)
        {
            _countries = countries;
            _mapper = mapper;

        }

        // GET: api/Countries
        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountry>>> GetCountries()
        {
            return Ok(await _countries.GetAll<GetCountry>());
        }

        // GET: api/Countries/5
        //Specify Template 
        [Authorize(Roles = "Administrator")]
        [HttpGet("{id}")]

        public async Task<ActionResult<GetCountry>> GetCountry(int id)
        {
            return Ok(await _countries.Get<GetCountry>(id));
        }

        // PUT: api/Countries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountry updateCountry)
        {

            await _countries.Update(id, updateCountry);
            return NoContent();
        }

        // POST: api/Countries
        [HttpPost]
        public async Task<ActionResult<GetCountry>> PostCountry(CreateCountry createCountry)
        {
            var country = await _countries.Add<CreateCountry, GetCountry>(createCountry);

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _countries.Delete(id);

            return NoContent();
        }


        [HttpGet("GetAllPaged")]
        public async Task<ActionResult<IEnumerable<GetCountry>>> GetPagedCountries([FromQuery] QueryParameters parameters)
        {

            var pagedCountries = await _countries.GetAll<GetCountry>(parameters);
            return Ok(pagedCountries);
        }


    }
}
