using Microsoft.Build.Framework;

namespace HotelListing.API.Models.Country
{
    public abstract class BaseCountry
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortName { get; set; }
    }
}
