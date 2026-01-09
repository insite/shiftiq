using InSite.Persistence;
using InSite.Persistence.Integration;
using InSite.Persistence.Plugin.CMDS;
using InSite.Web.Security;

using Serilog;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.Web.Injection
{
    internal static class DependencyInjector
    {
        internal static void Startup(StartupRequirements requirements)
        {
            Shift.Common.Humanizer.Initialize(new HumanizerSingleton());

            var commander = new Commander();
            var identity = new IdentityService();
            var logger = new ApiRequestLogger(AppSentry.SentryError);
            var appSettings = AppSettingsFactory.Create();

            ServiceLocator.InitializeAppSettings(appSettings, requirements);
            ServiceLocator.InitializeLogger(ConfigureLogger());
            ServiceLocator.InitializeTimeline(identity);
            ServiceLocator.InitializeInfrastructure(identity, logger, AppSentry.SentryError);
            ServiceLocator.InitializeApplication(AppSentry.SentryError);
            ServiceLocator.InitializeCustomization(logger);

            ServiceLocator.InitializeProcessManagers(commander, AppSentry.SentryWarning, AppSentry.SentryError, logger);

            OrganizationStore.Initialize(commander);
            MembershipStore.Initialize(commander);
            TPersonFieldStore.Initialize(commander);
            UserStore.Initialize(commander);
            ProgramService.Initialize(commander);
            PersonStore.Initialize(commander);
            UserConnectionStore.Initialize(commander);
            Course1Store.Initialize(commander);
            Course2Store.Initialize(commander, ServiceLocator.ContentSearch);
            StandardValidationStore.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            StandardValidationChangeStore.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            UserCompetencyRepository.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            CompetencyRepository.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            StandardStore.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            TAchievementStandardStore.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            StandardClassificationStore.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            StandardConnectionStore.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            StandardContainmentStore.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            TDepartmentStandardStore.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            ContactRepository2.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            VCmdsCompetencyOrganizationRepository.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
            StandardOrganizationStore.Initialize(ServiceLocator.SendCommand, ServiceLocator.SendCommands);
        }

        private static ILogger ConfigureLogger()
        {
            string file = ServiceLocator.FilePaths.CreateLogFilePath("InSite.UI", "Log.txt");

            var level = ServiceLocator.AppSettings.Shift.Api?.Telemetry?.Logging?.Level ?? "Warning";

            var config = new LoggerConfiguration();

            switch (level)
            {
                case "Debug":
                    config = config.MinimumLevel.Debug(); break;
                case "Error":
                    config = config.MinimumLevel.Error(); break;
                case "Fatal":
                    config = config.MinimumLevel.Error(); break;
                case "Information":
                    config = config.MinimumLevel.Information(); break;
                case "Warning":
                    config = config.MinimumLevel.Warning(); break;
                case "Verbose":
                    config = config.MinimumLevel.Verbose(); break;
            }

            return config
                .WriteTo.File(file, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}