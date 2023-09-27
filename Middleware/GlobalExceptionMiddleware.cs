using HotelListing.API.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace HotelListing.API.Middlewear
{
    public class GlobalExceptionMiddleware
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            this.next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in{context.Request.Path}");
                await HandleExceptionAsync(ex, context);
            }
        }

        private Task HandleExceptionAsync(Exception ex, HttpContext context)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode code = HttpStatusCode.InternalServerError;
            ErrorDetails errorDetails = new ErrorDetails
            {
                ErrorMessage = ex.Message,
                ErrorType = "Failure"
            };

            if (ex.GetType() == typeof(NotFoundException))
            {
                errorDetails.ErrorType = "Not Found";
                code = HttpStatusCode.NotFound;
            }

            string response = JsonConvert.SerializeObject(errorDetails);
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(response);
        }
    }

}

public class ErrorDetails
{
    public string ErrorMessage { get; set; }
    public string ErrorType { get; set; }
}
