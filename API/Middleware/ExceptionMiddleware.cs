using System;
using System.Net;
using System.Threading.Tasks;
using Application.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }   
            catch (Exception ex)
            {
                //log the erro
                _logger.LogError(ex.Message);

                //if and Exceptions was caught in the middleware
                //send a 500 server error response
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                
                //if env is not development do not provide the stack trace
                var responseObject = _env.IsDevelopment()? 
                new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace.ToString()):
                new AppException(context.Response.StatusCode, ex.Message);

                //set some json naming policys
                var namingPolicy = new JsonSerializerOptions{PropertyNamingPolicy= JsonNamingPolicy.CamelCase};
                var responseJsonObject = JsonSerializer.Serialize<AppException>(responseObject,namingPolicy);

                await context.Response.WriteAsync(responseJsonObject);
            }
        }
    }
}