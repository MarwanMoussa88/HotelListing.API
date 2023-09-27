using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using HotelListing.API.Models.Hotel;
using HotelListing.API.Models.User;

namespace HotelListing.API.Configurations
{
    public class AutomapperConfig : Profile
    {
        /*
         *autoMapper configuration
         */
        public AutomapperConfig()
        {
            CreateMap<Country, CreateCountry>().ReverseMap();
            CreateMap<Country, GetCountry>().ReverseMap();
            CreateMap<Country, UpdateCountry>().ReverseMap();
            CreateMap<Hotel, GetHotel>().ReverseMap();
            CreateMap<Hotel, CreateHotel>().ReverseMap();
            CreateMap<Hotel, UpdateHotel>().ReverseMap();
            CreateMap<ApiUser, ApiUserRegister>().ReverseMap();
            CreateMap<ApiUser, ApiUserLogin>().ReverseMap();
        }
    }
}
