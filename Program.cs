using HotelListing.Api.Core.Middleware;
using HotelListing.API.Configurations;
using HotelListing.API.Data;
using HotelListing.API.Middleware;
using HotelListing.API.Middlewear;
using HotelListing.API.Repository;
using HotelListing.API.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Get Connection String
string? connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");



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

//Add Db Context for entity framework 
builder.Services.AddDbContext<HotelListingDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});



//register autoMapper 
builder.Services.AddAutoMapper(typeof(AutomapperConfig));
builder.Services.AddControllers().AddOData(options =>
{
    options.Select().Filter().OrderBy();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Listing Api ", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the bearer scheme.
                      Enter the 'Bearer' [space] and then you token in the
                      Text Input below ex: 'Bearer abc.123.efg'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id ="Bearer"
                },
                Scheme = "0auth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            }, new List<string>()
        }
    });
});
builder.Services.AddApiVersioning(options =>
{
    //If request doesn't come on with the version number give default
    options.AssumeDefaultVersionWhenUnspecified = true;
    //Default Version
    options.DefaultApiVersion = new ApiVersion(1, 0);
    //Send api version in response
    options.ReportApiVersions = true;
    //The ways the request can come in  
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-version"),
        new MediaTypeApiVersionReader("ver"));

});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
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

builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("Custom Health Check",
    failureStatus: HealthStatus.Degraded,
    tags: new[] { "Custom", "Database" })
    .AddSqlServer(connectionString, tags: new[] { "Database" })
    .AddDbContextCheck<HotelListingDbContext>(tags: new[] { "Database" });

//Adds serilog for logging
builder.Host.UseSerilog((ctx, logger) =>
{
    logger.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddResponseCaching(options =>
{
    //Max cache size
    options.MaximumBodySize = 1024;
    //Store different case indedepntly
    options.UseCaseSensitivePaths = true;
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

app.MapHealthChecks("/healthcheck", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = healthcheck => healthcheck.Tags.Contains("Custom"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] =StatusCodes.Status200OK,
        [HealthStatus.Degraded] =StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] =StatusCodes.Status503ServiceUnavailable

    },
    ResponseWriter = CustomHealthCheck.WriteResponse
});
app.MapHealthChecks("/DatabaseHealthCheck", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = healthcheck => healthcheck.Tags.Contains("Database"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] =StatusCodes.Status200OK,
        [HealthStatus.Degraded] =StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] =StatusCodes.Status503ServiceUnavailable

    },
    ResponseWriter = CustomHealthCheck.WriteResponse
});

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
//Enable the AllowAll Cors option
app.UseCors("AllowAll");

app.UseResponseCaching();

app.UseMiddleware<CachingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
