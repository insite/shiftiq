using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

using Shift.Hub;
using Shift.Hub.Google;
using Shift.Hub.ScormCloud;

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

        services.AddScoped<Shift.Hub.Google.LocationSearch>();
        services.AddScoped<ITranslationService, TranslationService>();

        services.AddScoped<Shift.Hub.Partitions.PartitionStore>();

        services.AddRateLimiter(RateLimiterHelper.ConfigureRateLimiter);
    },
    addControllers: (options) =>
    {
        options.Filters.Add<ControllerHeaderAttribute>();
    },
    configApp: (app, _, _) =>
    {
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

        // SCORM authenticator applies only to /scorm paths so the other integrations
        // (Google, Premailer, ImageMagick) do not require UserName/Password headers.
        app.UseWhen(
            ctx => ctx.Request.Path.StartsWithSegments("/scorm", StringComparison.OrdinalIgnoreCase),
            branch => branch.UseMiddleware<ScormAuthenticator>());
    });

await HubApplication.StartupAsync(app, config, async app =>
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();

        await db.MigrateAsync();
    }
});

await HubApplication.ShutdownAsync(app);
