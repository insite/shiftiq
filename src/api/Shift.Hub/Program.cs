using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

using Shift.Common.Integration;
using Shift.Hub;
using Shift.Hub.Google;
using Shift.Hub.Partitions;

var config = HubApplication.LoadConfiguration<EngineSettings>(
    "Shift.Hub",
    Assembly.GetExecutingAssembly().GetName().Name!,
    "Engine");

HubApplication.ConfigureLogging(config);

var app = HubApplication.BuildWebApp(
    args, config,
    configServices: (services, _, shiftSettings) =>
    {
        var settings = (EngineSettings)shiftSettings;

        services.AddSingleton(settings.Integration);
        services.AddSingleton(settings.Integration.Google);
        services.AddSingleton(settings.Security);
        services.AddSingleton(settings.Security.Token);

        services.AddDbContext<EngineDbContext>(options =>
        {
            options.UseSqlServer(settings.Database.ConnectionString);
        });

        services.AddSingleton<IClientLockService, ClientLockService>();

        services.AddScoped<ISqlDatabase, SqlDatabase>();
        services.AddScoped<DatabaseMigrator>();

        services.AddScoped<LocationSearch>();
        services.AddScoped<ITranslationService, TranslationService>();

        services.AddScoped<PartitionRepository>();
        services.AddScoped<PartitionService>();

        services.AddRateLimiter(RateLimiterHelper.ConfigureRateLimiter);
    },
    addControllers: (options) =>
    {
        options.Filters.Add<ControllerHeaderAttribute>();
    },
    configApp: (app, _, settings) =>
    {
        // The Shift.Hub contract - partition registration and the Google, Premailer and
        // ImageMagick integrations - takes X-Hub-Api-Key. Clients send Engine:Api:ApiKey, and the
        // hub checks against its own value of the same key.
        //
        // The rollout is soft. A wrong key is refused, but a call with no header at all is served
        // as before and reported to Sentry as a warning: it comes from an InSite partition that
        // has not been redeployed with the key, and the warning is the reminder to do it. A hub
        // with no key of its own does the same for every call. Both warnings are throttled to one
        // per caller per hour, because a busy partition makes hundreds of these calls (Premailer
        // runs once per email) and one line is enough to act on.
        var hubApiKey = settings.Api?.ApiKey;

        var monitor = app.Services.GetRequiredService<IMonitor>();

        if (string.IsNullOrWhiteSpace(hubApiKey))
            monitor.Warning(
                "Engine:Api:ApiKey is not configured, so Shift.Hub serves its contract routes without checking {Header}. Configure it before the header becomes required.",
                HubApiKey.HeaderName);

        var lastWarned = new System.Collections.Concurrent.ConcurrentDictionary<string, DateTimeOffset>();

        bool ShouldWarn(string caller)
        {
            var now = DateTimeOffset.UtcNow;
            var previous = lastWarned.GetOrAdd(caller, DateTimeOffset.MinValue);

            return now - previous >= TimeSpan.FromHours(1)
                && lastWarned.TryUpdate(caller, now, previous);
        }

        app.Use(async (context, next) =>
        {
            var path = context.Request.Path.Value;
            var check = HubApiKey.Evaluate(path, context.Request.Headers[HubApiKey.HeaderName].ToString(), hubApiKey);
            var caller = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (check == HubApiKey.Check.Missing && ShouldWarn("missing|" + caller))
                monitor.Warning(
                    "Shift.Hub served {Method} {Path} from {Caller} without the {Header} header. Redeploy that InSite partition with Engine:Api:ApiKey; the header will become required.",
                    context.Request.Method, path, caller, HubApiKey.HeaderName);

            if (check == HubApiKey.Check.Unconfigured && ShouldWarn("unconfigured|" + caller))
                monitor.Warning(
                    "Shift.Hub served {Method} {Path} from {Caller} without checking {Header}, because Engine:Api:ApiKey is not configured.",
                    context.Request.Method, path, caller, HubApiKey.HeaderName);

            if (check == HubApiKey.Check.Invalid)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(new
                {
                    type = "https://httpstatuses.io/401",
                    title = "Hub API key not recognized.",
                    status = 401,
                    detail = $"The {HubApiKey.HeaderName} header does not match this environment's Engine:Api:ApiKey.",
                });
                return;
            }

            await next();
        });

        var deadEndProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.ContentRootPath, "DeadEnd"));
        app.UseDefaultFiles(new DefaultFilesOptions
        {
            RequestPath = "/deadend",
            FileProvider = deadEndProvider
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            RequestPath = "/deadend",
            FileProvider = deadEndProvider
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseRateLimiter();
    });

await HubApplication.StartupAsync(app, config, async app =>
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
        await db.MigrateAsync();

        var partitionStore = scope.ServiceProvider.GetRequiredService<Shift.Hub.Partitions.PartitionRepository>();
        await partitionStore.EnsureSchemaAsync();
    }
});

await HubApplication.ShutdownAsync(app);
