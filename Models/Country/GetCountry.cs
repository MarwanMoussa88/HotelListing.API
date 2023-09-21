using HotelListing.API.Models.Hotel;
using Microsoft.Build.Framework;

namespace HotelListing.API.Models.Country
{
    public class GetCountry : BaseCountry
    {
        [Required]
        public int Id { get; set; }

        public List<GetHotel> Hotels { get; set; }
    }
}
