using InSite.Application.Files.Read;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Serilog;

using Shift.Common.Integration.Google;
using Shift.Constant;
using Shift.Contract.Presentation;
using Shift.Service.Content;
using Shift.Service.Directory;
using Shift.Service.Feedback;
using Shift.Service.Presentation;
using Shift.Service.Workspace;

// Step 1. Load configuration settings (from appsettings.json) before doing anything else.

var configuration = BuildConfiguration();

var settings = LoadSettings<AppSettings>();
var databases = settings.Database;
var database = databases.ConnectionStrings.Shift;
var engine = settings.Engine;
var security = settings.Security;
var shift = settings.Shift;
var telemetry = shift.Api.Telemetry;
var release = settings.Release;
var platform = settings.Platform;

var environment = release.GetEnvironment();
if (environment.IsLocal())
    DebugConfigurationProviders();

// Step 2. Configure logging before we build the application host to ensure we capture all log
// entries, including those generated during the host initialization process. This is critical for
// diagnosing startup issues, monitoring initialization steps, and providing consistent, centralized
// logging throughout the application lifecycle.

Serilog.Log.Logger = ConfigureLogging(shift.Api.Telemetry.Logging.File, shift.Api.Telemetry.Logging.Console);

// Step 3. Build the application host with all services registered in the DI container.

var host = BuildHost(settings, release, telemetry);

// Step 4. Start up the application.

await Startup(host);

// Step 5. Shut down the application

await Shutdown(host);


// -------------------------------------------------------------------------------------------------


IConfigurationRoot BuildConfiguration()
{
    var basePath = AppContext.BaseDirectory;

    var builder = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .SetBasePath(basePath)
        .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile("AppSettings.Local.json", optional: true, reloadOnChange: true);

    return builder.Build();
}

T LoadSettings<T>() where T : new()
{
    var configuration = BuildConfiguration();
    var settings = configuration.Get<T>();
    return settings ?? new T();
}

void DebugConfigurationProviders()
{
    var jsonProviders = configuration.Providers.OfType<JsonConfigurationProvider>();

    foreach (var provider in jsonProviders)
    {
        var absolutePath = Path.Combine(AppContext.BaseDirectory, provider.Source.Path!);

        var exists = File.Exists(absolutePath);

        var descriptor = absolutePath + (exists ? " (exists)" : " (not found)");

        release.ConfigurationProviders.Add(descriptor);
    }
}

Serilog.ILogger ConfigureLogging(string path, bool writeToConsole)
{
    var config = new LoggerConfiguration()
        .MinimumLevel.Debug();

    var file = ProcessHelper.InitializeLogging(path);
    if (!string.IsNullOrEmpty(file))
        config = config.WriteTo.File(file, rollingInterval: RollingInterval.Day);

    if (writeToConsole)
        config = config.WriteTo.Console();

    return config.CreateLogger();
}

WebApplication BuildHost(AppSettings settings, ReleaseSettings release, TelemetrySettings telemetry)
{
    var builder = WebApplication.CreateBuilder();

    var services = builder.Services;

    AddSettings(services);

    AddEntities(services);

    services.AddSingleton<ReactStartupOptions>();

    services.AddSingleton<IShiftIdentityService, IdentityService>();

    services.AddSingleton<IActionService, ActionService>();

    services.AddSingleton<ILabelService, LabelService>();

    services.AddSingleton<ITranslationClient, TranslationClient>();

    services.AddSingleton<INavigationService, NavigationService>();

    services.AddSingleton<IPageService, PageService>();

    services.AddSingleton<ResponseService>();

    services.AddScoped<ITranslator, Translator>();
    services.AddScoped<IGroupLookupService, GroupLookupService>();

    services.AddSingleton<IFileChangeFactory>(x => new FileChangeFactory(x => string.Empty));
    services.AddSingleton<IFileSearchAsync, FileSearch>();
    services.AddSingleton<IFileStoreAsync, FileStore>();
    services.AddSingleton<IFileManagerServiceAsync>(x =>
    {
        var paths = new FilePaths(settings.DataFolderShare, settings.DataFolderEnterprise);
        return new FileManagerService(paths);
    });
    services.AddSingleton<IStorageServiceAsync, StorageService>();

    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddSerilog(dispose: true);
        builder.Services.AddSingleton(Serilog.Log.Logger);
    });

    services.AddMonitoring(settings.Shift.Api.Telemetry.Monitoring, release);

    services.AddSingleton<Shift.Common.IJsonSerializer, Shift.Common.Json.Serializer>();
    services.AddSingleton<IJsonSerializerBase, JsonSerializer2>();

    services.AddControllers(options =>
    {
        options.Conventions.Add(new DisallowAuthorizeAttributeConvention());
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new DefaultNamingStrategy() // Preserve original casing
        };
        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });

    var developerDocsUrl = settings.Application.HelpUrl.TrimEnd('/') + "/developers";

    services.AddDocumentation(release.Brand, developerDocsUrl);

    services.AddQueries(typeof(TestOrganizationQuery).Assembly);

    services.AddSecurity(security);

    services.AddDbContextFactory<TableDbContext>(options =>
            options.UseSqlServer(database)
        );

    services.AddDbContextFactory<ViewDbContext>(options =>
            options.UseSqlServer(database)
        );

    services.AddDbContextFactory<Shift.Service.Variant.CMDS.CmdsDbContext>(options =>
            options.UseSqlServer(database)
        );

    services.AddTransient<Shift.Service.Variant.CMDS.ComplianceSummaryReader>();

    services.AddTransient<IReactService, ReactService>();

    services.AddHttpContextAccessor();

    services.Configure<MvcOptions>(options =>
    {
        options.RespectBrowserAcceptHeader = true;

        options.ReturnHttpNotAcceptable = true;

        // Remove StringOutputFormatter (handles text/plain)

        var stringFormatter = options.OutputFormatters
            .OfType<StringOutputFormatter>()
            .FirstOrDefault();

        if (stringFormatter != null)
        {
            options.OutputFormatters.Remove(stringFormatter);
        }

        // Remove all JSON output formatters except application/json

        var jsonFormatter = options.OutputFormatters
            .OfType<SystemTextJsonOutputFormatter>()
            .FirstOrDefault();

        if (jsonFormatter != null)
        {
            jsonFormatter.SupportedMediaTypes.Clear();
            jsonFormatter.SupportedMediaTypes.Add("application/json");
            jsonFormatter.SupportedMediaTypes.Add("application/problem+json");
        }
    });

    return BuildApplication(builder, release, settings.Shift.Api, telemetry);
}

WebApplication BuildApplication(WebApplicationBuilder builder, ReleaseSettings release, ShiftSettingsApi api, TelemetrySettings telemetry)
{
    var environment = release.GetEnvironment();

    var isLocal = environment.IsLocal();

    var host = builder.Build();

    if (telemetry.Monitoring.Enabled)
        host.UseMiddleware<ExceptionHandlingMiddleware>();
    else
        host.UseDeveloperExceptionPage();

    host.UseDocumentation(release);

    host.UseHttpsRedirection();

    host.UseRouting();

    host.UseCors(o =>
    {
        o.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .WithExposedHeaders("X-Query-Pagination", "Content-Disposition")
            .WithOrigins(api.Origins);
    });

    host.UseAuthentication();

    host.UseAuthorization();

    host.MapControllers();

    return host;
}

void AddSettings(IServiceCollection services)
{
    services.AddSingleton(settings);
    services.AddSingleton(database);
    services.AddSingleton(databases);
    services.AddSingleton(engine);
    services.AddSingleton(release);
    services.AddSingleton(security);
    services.AddSingleton(security.Token);
    services.AddSingleton(settings.Shift);
    services.AddSingleton(settings.Shift.Api.Telemetry.Logging);
    services.AddSingleton(settings.Shift.Api.Telemetry.Monitoring);
    services.AddSingleton(platform);

    UuidFactory.NamespaceId = UuidFactory.CreateV5ForDns(security.Domain);

    OrganizationIdentifiers.Initialize(settings.Application.Organizations);
}

void AddEntities(IServiceCollection services)
{
    var assembly = typeof(TableDbContext).Assembly;

    var classTypes = assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && (
            typeof(IEntityAdapter).IsAssignableFrom(t) ||
            typeof(IEntityService).IsAssignableFrom(t) ||
            typeof(IEntityReader).IsAssignableFrom(t) ||
            typeof(IEntityWriter).IsAssignableFrom(t)
            )
        )
        .ToList();

    foreach (var classType in classTypes)
    {
        services.AddTransient(classType);
    }

    var interfaceTypes = assembly.GetTypes()
        .Where(t => t.IsInterface && (
            typeof(IEntityReader).IsAssignableFrom(t) ||
            typeof(IEntityWriter).IsAssignableFrom(t)
            )
        )
        .ToList();

    foreach (var interfaceType in interfaceTypes)
    {
        // Assume implementation is named without the 'I' prefix

        var implementationName = interfaceType.Name.Substring(1);

        var implementationType = assembly.GetType($"{interfaceType.Namespace}.{implementationName}");

        if (implementationType != null)
        {
            services.AddTransient(interfaceType, implementationType);
        }
    }

    services.AddSingleton<IPersonValidator, PersonValidator>();

    services.AddSingleton<PartitionFieldService>();
}

async Task Startup(WebApplication host)
{
    var log = host.Services.GetRequiredService<ILog>();
    log.Information("Shift.Api starting up.");

    var labelService = host.Services.GetRequiredService<ILabelService>();
    await labelService.Refresh();

    var actionService = host.Services.GetRequiredService<IActionService>();
    await actionService.RefreshAsync();

    await host.RunAsync();
}

async Task Shutdown(WebApplication host)
{
    try
    {
        var monitor = host.Services.GetRequiredService<IMonitor>();

        await monitor.FlushAsync();
    }
    catch (ObjectDisposedException)
    {
        // The async code in Sentry is not perfect, so it is important to try to manually flush the
        // queue. If this exception occurs, then IServiceProvider is already disposed, and the queue
        // is already flushed, therefore we can ignore the exception.
    }
}