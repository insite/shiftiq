using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Contract.Presentation;
using Shift.Service;
using Shift.Service.Content;
using Shift.Service.Metadata;
using Shift.Service.Presentation;
using Shift.Service.Security;
using Shift.Service.Workspace;

namespace Shift.Test
{
    public class PlatformFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }

        public AppSettings Settings { get; private set; } = null!;

        public IDbContextFactory<TableDbContext>? TableDbContextFactory { get; private set; }

        public PlatformFixture()
        {
            var services = new ServiceCollection();

            AddSettings(services);

            AddSerializers(services);

            AddQueries(services);

            AddDatabases(services);

            AddEntities(services);

            AddAuthentication(services);

            services.AddSingleton<IActionService, ActionService>();
            services.AddSingleton<ILabelService, LabelService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<PartitionFieldService, PartitionFieldService>();
            services.AddSingleton<INavigationService, TestNavigationService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        private void AddAuthentication(ServiceCollection services)
        {
            var testIdentityService = new TestIdentityService();
            services.AddSingleton<IShiftIdentityService>(testIdentityService);
        }

        void AddSettings(IServiceCollection services)
        {
            Settings = AppSettingsHelper.GetAllSettings<AppSettings>();

            OrganizationIdentifiers.Initialize(Settings.Application.Organizations);

            services.AddSingleton<AppSettings>(Settings);
            services.AddSingleton<Platform>(Settings.Platform);
            services.AddSingleton<ReleaseSettings>(Settings.Release);
            services.AddSingleton<SecuritySettings>(Settings.Security);

            services.AddSingleton<ReactStartupOptions>();
        }

        void AddSerializers(IServiceCollection services)
        {
            services.AddSingleton<Shift.Common.IJsonSerializer, Shift.Common.Json.Serializer>();
            services.AddSingleton<IJsonSerializerBase, JsonSerializer2>();
        }

        void AddQueries(IServiceCollection services)
        {
            services.AddQueries(typeof(TestOrganizationQuery).Assembly);
        }

        void AddDatabases(IServiceCollection services)
        {
            services.AddDbContextFactory<TableDbContext>(options =>
                   options.UseSqlServer(Settings.Database.ConnectionStrings.Shift)
               );
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
        }

        public void Dispose()
        {
            if (ServiceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
