using Microsoft.Extensions.Caching.Memory;
using SAWA.API.Healper;
using System.Net;
using System.Text.Json;

namespace SAWA.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(30);
        private const int _requestLimit = 80;

        public ExceptionMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                ApplySecurity(context);

                if (!IsRequestAllowed(context))
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "application/json";

                    var response = ResponseAPI<string>.Error("Too many requests, please try again later", context.Response.StatusCode);
                    var jsonResponse = JsonSerializer.Serialize(response);

                    await context.Response.WriteAsync(jsonResponse);
                    return;
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ResponseAPI<string>.Error(ex.Message, context.Response.StatusCode);
                var jsonResponse = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(jsonResponse);
            }
        }

        private bool IsRequestAllowed(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var cacheKey = $"Rate:{ip}";
            var now = DateTime.UtcNow;

            var cacheEntry = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _rateLimitWindow;
                return (Timestamp: now, Count: 1);
            });

            var (timestamp, count) = cacheEntry;

            if (now - timestamp < _rateLimitWindow)
            {
                if (count >= _requestLimit)
                {
                    return false;
                }

                _memoryCache.Set(cacheKey, (timestamp, count + 1), _rateLimitWindow);
            }
            else
            {
                _memoryCache.Set(cacheKey, (now, 1), _rateLimitWindow);
            }

            return true;
        }


        private void ApplySecurity(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

            context.Response.Headers["X-Frame-Options"] = "DENY";

        }

    }
}
