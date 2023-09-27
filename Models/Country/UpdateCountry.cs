using Microsoft.Build.Framework;

namespace HotelListing.API.Models.Country
{
    public class UpdateCountry : BaseCountry
    {
        [Required]
        public int Id { set; get; }
    }
}
