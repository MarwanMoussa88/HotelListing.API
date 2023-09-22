using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using HotelListing.API.Models.Hotel;

namespace HotelListing.API.Configurations
{
    public class AutomapperConfig:Profile
    {
        //automapper configuration
        public AutomapperConfig()
        {
            CreateMap<Country, CreateCountry>().ReverseMap();
            CreateMap<Country, GetCountry>().ReverseMap();
            CreateMap<Country, UpdateCountry>().ReverseMap();
            CreateMap<Hotel, GetHotel>().ReverseMap();
            CreateMap<Hotel, CreateHotel>().ReverseMap();
            CreateMap<Hotel, UpdateHotel>().ReverseMap();
        }
    }
}
