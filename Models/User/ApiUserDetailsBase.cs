using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.User
{
    public abstract class ApiUserDetailsBase
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage = "Password is limited to {2} to {1} characters"),
            MinLength(6)]
        public string Password { get; set; }
    }
}
