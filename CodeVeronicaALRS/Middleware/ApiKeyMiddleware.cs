using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CodeVeronicaALRS.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyMiddleware> _logger;
        private const string ApiKeyHeaderName = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedApiKey))
            {
                _logger.LogWarning("API Key is missing from the request.");

                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing.");
                return;
            }

            var apiKeyFromEnvi = Environment.GetEnvironmentVariable("API_KEY");

            if (string.IsNullOrEmpty(apiKeyFromEnvi))
            {
                _logger.LogError("API key not found in environment variables.");
                context.Response.StatusCode = 500; 
                await context.Response.WriteAsync("Internal server error. API Key not configured.");
                return;
            }

            if (!apiKeyFromEnvi.Equals(providedApiKey))
            {
                _logger.LogWarning("Unauthorized client. Invalid API Key: {ProvidedApiKey}", providedApiKey);

                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            _logger.LogInformation("Valid API Key provided. Request from: {RemoteIpAddress}",
                context.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP"); 

            await _next(context); 
        }
    }
}
