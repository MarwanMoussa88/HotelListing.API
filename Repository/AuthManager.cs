using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Models.User;
using HotelListing.API.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.Repository
{
    /*
     * Implementing the Authentication using Identity Core **/
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        //Manager class to manage Identity core tables
        private readonly UserManager<ApiUser> _userManager;
        //To access the configuration file
        private readonly IConfiguration _configuration;
        private ApiUser _user;

        private const string _loginProvider = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";
        //Dependency Injection
        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> CreateRefreshToken()
        {
            //Removes the old authentication token from the database so attackers can't use them
            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);
            //Generate a refresh token 
            var refreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
            //Set the refresh token to the user in the database 
            var result = _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, refreshToken);
            return refreshToken;

        }

        /*
         * Logging in the requested user**/
        public async Task<ApiUserAuthenticationResponse> LoginUser(ApiUserLogin userLogin)
        {
            bool isPasswordValid = false;

            //Get the user by email
            _user = await _userManager.FindByEmailAsync(userLogin.Email);
            //Checks if password is valid
            isPasswordValid = await _userManager.CheckPasswordAsync(_user, userLogin.Password);

            if (_user is null || !isPasswordValid)
            {
                return null;
            }
            //Generate Token
            var token = await GenerateToken();
            //Return the token and UserId
            return new ApiUserAuthenticationResponse { Token = token, UserId = _user.Id, RefreshToken = await CreateRefreshToken() };



        }
        /*
         * Register Requested User
         */
        public async Task<IEnumerable<IdentityError>> RegisterUser(ApiUserRegister userRegister)
        {
            //Map the models
            var user = _mapper.Map<ApiUser>(userRegister);
            //Makes the email as the username
            user.UserName = userRegister.Email;
            //Creates the user
            var result = await _userManager.CreateAsync(user, userRegister.Password);

            if (result.Succeeded)
            {
                //Add Role user
                await _userManager.AddToRoleAsync(user, "User");
            }
            return result.Errors;

        }


        public async Task<ApiUserAuthenticationResponse> VerifyRefreshToken(ApiUserAuthenticationResponse request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByNameAsync(username);

            if (_user == null)
                return null;

            var isRefreshTokenValid = await _userManager.VerifyUserTokenAsync(_user, _loginProvider,
                _refreshToken, request.RefreshToken);

            if (isRefreshTokenValid)
            {
                var token = await GenerateToken();
                return new ApiUserAuthenticationResponse
                {
                    RefreshToken = await CreateRefreshToken(),
                    Token = await GenerateToken(),
                    UserId = _user.Id,
                };
            }
            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }


        /*
         * Generate JWT Bearer Token**/
        private async Task<string> GenerateToken()
        {
            //Get Symmetric Key and turn it into bytes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            //Encrypt the key with Hmac256
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //Get all user roles
            var roles = await _userManager.GetRolesAsync(_user);
            //Take all the roles and turn it into claims and make them into a list
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            //Get all other user claims
            var userClaims = await _userManager.GetClaimsAsync(_user);

            //Create my own claims and combine them with the other claims
            var claims = new List<Claim>
            {
                //Subject of the token
                new Claim(JwtRegisteredClaimNames.Sub,_user.Email),
                //Unique Identifer for the token
                new Claim(JwtRegisteredClaimNames.Jti,new Guid().ToString()),
                //User's Email
                new Claim(JwtRegisteredClaimNames.Email,_user.Email),
                //FirstName
                new Claim(JwtRegisteredClaimNames.Name,_user.FirstName),
                //User Id
                new Claim("UID",_user.Id)

            }.Union(userClaims).Union(roleClaims);

            //Create the security token
            var token = new JwtSecurityToken(
                //Who issued the token
                issuer: _configuration["JwtSettings:Issuer"],
                //Who the token is being issued to
                audience: _configuration["JwtSettings:Audience"],
                //User claims
                claims: claims,
                //Token expire date
                expires: DateTime.Now.AddMinutes(15),
                //Token Key
                signingCredentials: credentials
                );

            //Return the key
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
