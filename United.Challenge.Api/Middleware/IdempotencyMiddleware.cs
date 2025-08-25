using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace United.Challenge.Api.Middleware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public const string HeaderName = "Idempotency-Key";

        public IdempotencyMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Just POST
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                await _next(context);
                return;
            }

            // Check for key 
            if (!context.Request.Headers.TryGetValue(HeaderName, out var keyValues))
            {
                await _next(context);
                return;
            }

            // Check key isNull
            var idempotencyKey = keyValues.ToString().Trim();
            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                await _next(context);
                return;
            }

            // Avoid collisions between lanes
            var namespacedKey = $"{context.Request.Method}:{context.Request.Path}:{idempotencyKey}";

            // There is a response stored in the cache with the same key.
            if (_cache.TryGetValue(namespacedKey, out CachedResponse cached))
            {
                context.Response.StatusCode = cached.StatusCode;
                if (!string.IsNullOrEmpty(cached.ContentType))
                    context.Response.ContentType = cached.ContentType;

                foreach (var kv in cached.Headers)
                {
                    context.Response.Headers[kv.Key] = kv.Value;
                }

                await context.Response.WriteAsync(cached.Body);

                return;  // Not running controller again
            }

            // No previous response .. Execute controller
            var originalBody = context.Response.Body;
            await using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context); // Running controller

            // Capture data
            memStream.Seek(0, SeekOrigin.Begin);
            var bodyText = await new StreamReader(memStream, Encoding.UTF8).ReadToEndAsync();

            var toCache = new CachedResponse
            {
                StatusCode = context.Response.StatusCode,
                ContentType = context.Response.ContentType,
                Body = bodyText
            };


            // Store for 24 hours
            _cache.Set(namespacedKey, toCache, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            });

            // Rewrite the response to the original
            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
        }
    }
}