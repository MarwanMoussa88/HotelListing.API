using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using AutoMapper;
using HotelListing.API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace HotelListing.API.Controllers
{
    //Controller Route
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountry>>> GetCountries()
        {

            var countries = await _countries.GetAll();
            var records= _mapper.Map<List<GetCountry>>(countries);

            return Ok(records);
        }

        // GET: api/Countries/5
        //Specify Template 
        [Authorize(Roles = "Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCountry>> GetCountry(int id)
        {
            var country = await _countries.GetDetails(id);
            if (country == null)
            {
                return NotFound();
            }

            var getCountry =_mapper.Map<GetCountry>(country);
            return Ok(getCountry);
        }

        // PUT: api/Countries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountry updateCountry)
        {

            if (id != updateCountry.Id)
            {
                return BadRequest("Invalid Id ");
            }

            var country =await _countries.Get(id);
            _mapper.Map(updateCountry, country);
            
            try
            {
                await _countries.Update(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _countries.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CreateCountry createCountry)
        {
            //auto maps the object sent to the model
            var country = _mapper.Map<Country>(createCountry);
            await _countries.Add(country);
          
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countries.Get(id);
            if (country == null)
            {
                return NotFound("Invalid Id");
            }

            await _countries.Delete(country);

            return NoContent();
        }

        
    }
}
