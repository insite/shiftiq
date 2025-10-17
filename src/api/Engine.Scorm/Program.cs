using System.Reflection;

using Engine.Common;

using Shift.Common;

var config = EngineApplication.LoadConfiguration<EngineSettings>("Engine.Api.Scorm", Assembly.GetExecutingAssembly().GetName().Name!, "Engine");

EngineApplication.ConfigureLogging(config);

var app = EngineApplication.BuildWebApp(args, config, configApp: (app, _, _) =>
{
    app.UseMiddleware<ScormAuthenticator>();
});

await EngineApplication.StartupAsync(app, config);

await EngineApplication.ShutdownAsync(app);
