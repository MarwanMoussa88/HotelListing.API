using AutoMapper;
using HotelListing.API.Models.User;
using HotelListing.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        public AccountController(IMapper mapper, IAuthManager authManager)
        {
            _mapper = mapper;
            _authManager = authManager;
        }

        //POST : api/Account/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] ApiUserRegister userDetails)
        {
            var errors = await _authManager.RegisterUser(userDetails);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);

                }
                return BadRequest(ModelState);
            }
            return Ok();

        }
        //POST : api/Account/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] ApiUserLogin userDetails)
        {
            var authResponse = await _authManager.LoginUser(userDetails);

            if (authResponse == null)
                return Unauthorized();

            return Ok(authResponse);

        }

        //POST : api/Account/RefreshToken
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] ApiUserAuthenticationResponse userDetails)
        {
            var authResponse = await _authManager.VerifyRefreshToken(userDetails);

            if (authResponse == null)
                return Unauthorized();

            return Ok(authResponse);

        }
    }
}
