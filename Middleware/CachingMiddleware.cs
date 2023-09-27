using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace HotelListing.API.Middleware
{
    public class CachingMiddleware
    {
        private readonly RequestDelegate _next;

        public CachingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            {
                //Public cache
                Public = true,
                //Every 10 seconds keep cache alive
                MaxAge = TimeSpan.FromSeconds(10),
            };
            context.Response.Headers[HeaderNames.Vary] = new string[] { "Accept-encoding" };
            await _next(context);
        }
    }
}
