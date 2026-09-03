using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Serialization;

using Serilog;

using Shift.Common;

namespace Shift.Hub
{
    public static class HubApplication
    {
        #region Load configuration

        private class HubConfig : IHubConfiguration
        {
            public required string AppName { get; set; }
            public required string AssemblyName { get; set; }
            public required IConfigurationRoot Configuration { get; set; }
            public required EngineSettings Settings { get; set; }
        }

        public static IHubConfiguration LoadConfiguration<T>(string appName, string assemblyName, string sectionName) where T : Shift.Common.EngineSettings
        {
            var config = BuildConfiguration();
            var settings = GetSettings<T>(config, sectionName);

            return new HubConfig
            {
                AppName = appName,
                AssemblyName = assemblyName,
                Configuration = config,
                Settings = settings
            };
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .Build();
        }

        private static T GetSettings<T>(IConfigurationRoot configuration, string name)
        {
            var section = configuration.GetRequiredSection(name);
            var settings = section.Get<T>()!;
            return settings;
        }

        #endregion

        #region Configure logging

        public static void ConfigureLogging(IHubConfiguration config)
        {
            var settings = config.Settings
                ?? throw new ArgumentNullException(nameof(IHubConfiguration.Settings));
            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Debug();

            var file = ProcessHelper.InitializeLogging(settings.Telemetry.Logging.File);
            if (!string.IsNullOrEmpty(file))
                logConfig = logConfig.WriteTo.File(file, rollingInterval: RollingInterval.Day);

            if (settings.Telemetry.Logging.Console)
                logConfig = logConfig.WriteTo.Console();

            Serilog.Log.Logger = logConfig.CreateLogger();
        }

        #endregion

        #region Build app host

        public static WebApplication BuildWebApp(
            string[] args,
            IHubConfiguration config,
            Action<IServiceCollection, IConfigurationRoot, EngineSettings>? configServices = null,
            Action<MvcOptions>? addControllers = null,
            Action<WebApplication, IConfigurationRoot, EngineSettings>? configApp = null)
        {
            var settings = config.Settings
                ?? throw new ArgumentNullException(nameof(IHubConfiguration.Settings));
            var configuration = config.Configuration
                ?? throw new ArgumentNullException(nameof(IHubConfiguration.Configuration));

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();
            builder.Configuration.AddConfiguration(configuration);

            var services = builder.Services;

            services.AddSingleton(settings);
            services.AddSingleton(settings.Integration.Google);
            services.AddSingleton(settings.Integration.Google.Translation);
            services.AddSingleton(settings.Release);
            services.AddSingleton(settings.Telemetry);
            services.AddSingleton(settings.Telemetry.Logging);
            services.AddSingleton(settings.Telemetry.Monitoring);

            if (settings.Telemetry.Throttling != null)
                services.AddSingleton(settings.Telemetry.Throttling);

            services.AddControllers(addControllers)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new DefaultNamingStrategy()
                    };
                    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                });

            services.AddDocumentation(config);

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(dispose: true);
                builder.Services.AddSingleton(Serilog.Log.Logger);
            });

            services.AddMonitoring(settings.Telemetry.Monitoring, settings.Release);

            configServices?.Invoke(services, configuration, settings);

            var app = builder.Build();

            app.UseDocumentation(config);

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            configApp?.Invoke(app, configuration, settings);

            return app;
        }

        #endregion

        #region Start up

        public static async Task StartupAsync(WebApplication app, IHubConfiguration config, Func<WebApplication, Task>? beforeRun = null)
        {
            var log = app.Services.GetRequiredService<ILog>();

            log.Information(config.AppName + " starting up.");

            if (beforeRun != null)
                await beforeRun(app);

            await app.RunAsync();
        }

        #endregion

        #region Shut down

        public static async Task ShutdownAsync(WebApplication app)
        {
            try
            {
                var monitor = app.Services.GetRequiredService<IMonitor>();

                await monitor.FlushAsync();
            }
            catch (ObjectDisposedException)
            {
                // IServiceProvider already disposed; Sentry queue already flushed.
            }
        }

        #endregion
    }
}
