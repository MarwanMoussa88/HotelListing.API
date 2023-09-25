using HotelListing.API.Configurations;
using HotelListing.API.Data;
using HotelListing.API.Repository;
using HotelListing.API.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Get Connection String
string? connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");

//Add Db Context for entity framework 
builder.Services.AddDbContext<HotelListingDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

//Injecting Repository Pattern to our app
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//Injecting Countries Repository 
builder.Services.AddScoped<ICountriesRepository, CountryRepository>();
//Injecting Hotel Repository
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
//Injecting User Registration class
builder.Services.AddScoped<IAuthManager, AuthManager>();
//Adding Identity Core with Role Based and linking it with Entity Framework
builder.Services.AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ApiUser>>("HotelListingApi")
    .AddEntityFrameworkStores<HotelListingDbContext>()
    .AddDefaultTokenProviders();

//JWT Authentication
builder.Services.AddAuthentication(options =>
{
    /*
     * Specify What type of scheme the app will use by default
     * **/
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //Bearer 
    /*
     * The scheme that will be used to challenge users when they try to access a resource when not authenticated 
     * **/
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    
    
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        
        ValidateIssuerSigningKey = true, //Validate issuer signing key
        ValidateIssuer = true, // Validate issuer of the token
        ValidateAudience = true, // Validate the audience 
        ValidateLifetime = true, //Validate the lifetime 
        ClockSkew = TimeSpan.Zero, // make sure time in present
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"], //Validate the issuer of  the token
        ValidAudience = builder.Configuration["JwtSettings:Audience"], // Specifies the audience
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
        (builder.Configuration["JwtSettings:Key"])) // Key 
        



    };

});

//register autoMapper 
builder.Services.AddAutoMapper(typeof(AutomapperConfig));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


/*
 * Cors (cross origin resource sharing)
 * it allows which origins are allowed to access its resources
 * by deafault browsers implement (sop) same origin policy to prevent xss attacks 
 **/

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b =>
    b.AllowAnyHeader().
    AllowAnyOrigin().
    AllowAnyHeader());
});


//Adds serilog for logging
builder.Host.UseSerilog((ctx, logger) =>
{
    logger.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Check All Requests in Serilog
app.UseSerilogRequestLogging();


app.UseHttpsRedirection();
//Enable the AllowAll Cors option
app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
