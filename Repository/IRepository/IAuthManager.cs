using HotelListing.API.Data;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Repository.IRepository
{
    /*
     * Defining the Contract where the Authentication will happen**/
    public interface IAuthManager
    {
        //Create User
        Task<IEnumerable<IdentityError>> RegisterUser(ApiUserRegister user);
        //Login User
        Task<ApiUserAuthenticationResponse> LoginUser(ApiUserLogin user);

        Task<string> CreateRefreshToken();

        Task<ApiUserAuthenticationResponse> VerifyRefreshToken(ApiUserAuthenticationResponse request);



    }
}
