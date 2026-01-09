using System.Threading.RateLimiting;

namespace Engine.Api.Internal
{
    public static class RateLimiterHelper
    {
        public static void ConfigureRateLimiter(Microsoft.AspNetCore.RateLimiting.RateLimiterOptions o)
        {
            o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(CreatePartition);

            o.OnRejected = async (context, token) =>
            {
                string tryAfter;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
                    tryAfter = $"Please try again after {retryAfter.TotalMinutes} minute(s).";
                }
                else
                    tryAfter = "Please try again later.";

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync($"Too many requests. {tryAfter}", cancellationToken: token);
            };
        }

        private static RateLimitPartition<string> CreatePartition(HttpContext httpContext)
        {
            var userName = httpContext.User.Identity?.Name;
            var throttling = httpContext.RequestServices.GetRequiredService<ThrottlingSettings>();

            if (!string.IsNullOrEmpty(userName))
            {
                var isRoot = httpContext.User.IsInRole("Root");

                return !isRoot && throttling.Authenticated != null && throttling.Authenticated.IsEnabled
                    ? RateLimitPartition.GetTokenBucketLimiter(userName, _ => CreateOptions(throttling.Authenticated))
                    : RateLimitPartition.GetNoLimiter(userName);
            }

            return throttling.Anonymous != null && throttling.Anonymous.IsEnabled
                ? RateLimitPartition.GetTokenBucketLimiter("*", _ => CreateOptions(throttling.Anonymous))
                : RateLimitPartition.GetNoLimiter("*");
        }

        private static TokenBucketRateLimiterOptions CreateOptions(ThrottlingSettings.Settings settings)
        {
            return new TokenBucketRateLimiterOptions
            {
                TokenLimit = settings.TokenLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = settings.QueueLimit,
                ReplenishmentPeriod = TimeSpan.FromSeconds(settings.ReplenishmentPeriodInSeconds),
                TokensPerPeriod = settings.TokensPerPeriod,
                AutoReplenishment = true
            };
        }
    }
}