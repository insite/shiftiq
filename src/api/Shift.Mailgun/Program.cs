using Microsoft.Extensions.Caching.Memory;

using Serilog;

using Shift.Common;
using Shift.Mailgun;

// Step 1. Load configuration settings

var settings = LoadSettings<AppSettings>("AppSettings");
var release = settings.Release;

// Step 2. Configure logging.

Serilog.Log.Logger = ConfigureLogging(settings.Shift.Mailgun.Telemetry.Logging);

// Step 3. Build the application host

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(settings);
builder.Services.AddMemoryCache();

builder.Services.AddLogging(lb =>
{
    lb.ClearProviders();
    lb.AddSerilog(dispose: true);
});

var app = builder.Build();
var programLogger = app.Services.GetRequiredService<ILogger<Program>>();
var queueHelper = new QueueHelper(settings);

// Step 4. Initialize RabbitMQ publisher

var publisherLogger = app.Services.GetRequiredService<ILogger<RabbitMqPublisher>>();
await using var publisher = await RabbitMqPublisher.CreateAsync(settings, publisherLogger, queueHelper);

// Step 5. Map endpoints

var payloadLogger = app.Services.GetRequiredService<ILogger<PayloadValidator>>();
var validator = new PayloadValidator(payloadLogger, app.Services.GetRequiredService<IMemoryCache>(), settings, queueHelper);

app.MapPost("/api/handle-webhook", async (HttpRequest request) =>
{
    var mailgun = settings.Integration.Mailgun;

    string json;
    using (var reader = new StreamReader(request.Body))
        json = await reader.ReadToEndAsync();

    var error = validator.ParseAndValidate(json, out var environmentDomain, out var queueName);
    if (error != null)
        return error;

    await publisher.PublishJsonAsync(queueName!, json);

    programLogger.LogInformation("Published webhook to {QueueName} for {EnvironmentDomain}", queueName, environmentDomain);

    return Results.Ok();
});

app.Lifetime.ApplicationStarted.Register(() =>
{
    foreach (var url in app.Urls)
        programLogger.LogInformation("Shift.Mailgun is listening on {Url}", url);
});

// Step 6. Run.

await app.RunAsync();


// -------------------------------------------------------------------------------------------------


T LoadSettings<T>(string filename) where T : new()
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile(filename + ".json", optional: false, reloadOnChange: true)
        .AddJsonFile(filename + ".local.json", optional: true, reloadOnChange: true)
        .Build();

    return configuration.Get<T>() ?? new T();
}

Serilog.ILogger ConfigureLogging(LoggingSettings logging)
{
    var config = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning);

    var file = ProcessHelper.InitializeLogging(logging.File);
    if (!string.IsNullOrEmpty(file))
        config = config.WriteTo.File(file, rollingInterval: RollingInterval.Day);

    if (logging.Console)
        config = config.WriteTo.Console();

    return config.CreateLogger();
}