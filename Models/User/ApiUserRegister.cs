using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.User
{

    /*
     * User Model that will be mapped to the orignal user 
     **/
    public class ApiUserRegister :ApiUserDetailsBase
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

    }
}