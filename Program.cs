using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Get Connection String
string? connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
//Add Db Context for entity framework 
builder.Services.AddDbContext<HotelListingDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
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
    options.AddPolicy("AllowAll",b=>
    b.AllowAnyHeader().
    AllowAnyOrigin().
    AllowAnyHeader());
});


//Adds serilog for logging
builder.Host.UseSerilog((ctx,logger) => { 
    logger.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");


app.UseAuthorization();

app.MapControllers();

app.Run();
