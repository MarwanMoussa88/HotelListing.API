using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Repository.IRepository;
using AutoMapper;
using HotelListing.API.Models.Hotel;
using HotelListing.API.Models.Country;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelRepository _hotels;
        private readonly IMapper _mapper;

        public HotelsController(IHotelRepository hotels,IMapper mapper)
        {
            _hotels = hotels;
            _mapper = mapper;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels()
        {
            var hotels = await _hotels.GetAll();
            return Ok(_mapper.Map<IEnumerable<GetHotel>>(hotels));
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> GetHotel(int id)
        {
            var hotel = await _hotels.Get(id);


            if (hotel == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GetHotel>(hotel));
        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, UpdateHotel updateHotel)
        {
            var hotel = await _hotels.Get(id);

            if (id != updateHotel.Id)
            {
                return BadRequest();
            }
            if (hotel == null)
            {
                return NotFound(id);
            }

            _mapper.Map(updateHotel, hotel);

            try
            {
                await _hotels.Update(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _hotels.Exists(id))
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

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotel createHotel)
        {
            var hotel=_mapper.Map<Hotel>(createHotel);
            await _hotels.Add(hotel);
            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _hotels.Get(id);
            if (hotel == null)
            {
                return NotFound();
            }

            await _hotels.Delete(hotel);
        

            return NoContent();
        }


    }
}
