using System.Reflection;

using Engine.Common;

using Shift.Common;

var config = EngineApplication.LoadConfiguration<EngineSettings>("Engine.Api.Premailer", Assembly.GetExecutingAssembly().GetName().Name!, "Engine");

EngineApplication.ConfigureLogging(config);

var app = EngineApplication.BuildWebApp(args, config);

await EngineApplication.StartupAsync(app, config);

await EngineApplication.ShutdownAsync(app);
