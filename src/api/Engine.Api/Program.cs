using System.Reflection;

using Engine.Api.Internal;
using Engine.Api.Location;
using Engine.Api.Metadata;
using Engine.Api.Orchestration;
using Engine.Api.Translation;
using Engine.Common;

using Microsoft.EntityFrameworkCore;

var config = EngineApplication.LoadConfiguration<EngineSettings>("Engine.Api.Google", Assembly.GetExecutingAssembly().GetName().Name!, "Engine");

EngineApplication.ConfigureLogging(config);

var app = EngineApplication.BuildWebApp(
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
        services.AddScoped<DatabaseUpgrader>();

        services.AddScoped<LocationSearch>();
        services.AddScoped<ITranslationService, TranslationService>();

        services.AddRateLimiter(RateLimiterHelper.ConfigureRateLimiter);

    },
    addControllers: (options) =>
    {
        options.Filters.Add<ControllerHeaderAttribute>();
    },
    configApp: (app, _, _) =>
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseRateLimiter();
    });

await EngineApplication.StartupAsync(app, config, async app =>
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DatabaseUpgrader>();

        await db.UpgradeAsync();
    }
});

await EngineApplication.ShutdownAsync(app);
