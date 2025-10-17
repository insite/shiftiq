using InSite.Application.Files.Read;
using InSite.Application.Responses.Write;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuestPDF.Infrastructure;

using Shift.Common;
using Shift.Constant;
using Shift.Service;
using Shift.Service.Content;
using Shift.Toolbox;

using TimelineServices = Common.Timeline.Services;

namespace Shift.Test
{
    public class TestFixture : IDisposable
    {
        public AppSettings Settings = AppSettingsHelper.GetAllSettings<AppSettings>();

        public IDbContextFactory<TableDbContext>? TableDbContextFactory { get; private set; }
        public IDbContextFactory<ViewDbContext>? ViewDbContextFactory { get; private set; }

        public static Organization? Organization1;
        public static Organization? Organization2;
        public static Guid SurveyId = Guid.Parse("438a8a4e-bad5-44a8-83a8-2d42a0c45129");
        static TimelineServices.IGuidGenerator? _guidGenerator;

        public IStorageServiceAsync StorageService { get; }
        public IFileManagerServiceAsync FileManagerService { get; }
        public IJsonSerializer Serializer { get; internal set; }
        public IFileStoreAsync FileStore { get; internal set; }

        public TestFixture()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            _guidGenerator = new NewGuidGenerator();

            TimelineServices.ServiceLocator.Instance.Register<TimelineServices.IGuidGenerator>(_guidGenerator);

            SetupContextFactory();

            OrganizationIdentifiers.Initialize(Settings.Application.Organizations);

            Serializer = new Shift.Common.Json.Serializer();

            var fileSearch = new FileSearch(TableDbContextFactory!, ViewDbContextFactory!, Serializer);

            var fileChangeFactory = new FileChangeFactory(x => { return string.Empty; });

            FileStore = new FileStore(TableDbContextFactory!, fileChangeFactory, Serializer);

            var filePaths = new FilePaths(Settings.DataFolderShare, Settings.DataFolderEnterprise);

            FileManagerService = new FileManagerService(filePaths);

            StorageService = new StorageService(fileSearch, FileStore, FileManagerService, Settings);

            Organization1 = new Organization(OrganizationIdentifiers.Global, "global");

            Organization2 = new Organization(OrganizationIdentifiers.InSite, "insite");
        }

        public void Dispose() { }

        public void SendCommand(CreateResponseSession createResponseSession)
        {
            var client = new TimelineClient(Settings.Shift.Api.Hosting.V2, Settings.Security);

            client.QueueCommand(createResponseSession);
        }

        private void SetupContextFactory()
        {
            var services = new ServiceCollection();

            services.AddDbContextFactory<TableDbContext>(options =>
            {
                options.UseSqlServer(Settings.Database.ConnectionStrings.Shift);
            });

            services.AddDbContextFactory<ViewDbContext>(options =>
            {
                options.UseSqlServer(Settings.Database.ConnectionStrings.Shift);
            });

            var serviceProvider = services.BuildServiceProvider();

            TableDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<TableDbContext>>();

            ViewDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ViewDbContext>>();
        }
    }

    public class Organization
    {
        public Guid Id { get; }
        public string Code { get; set; }

        public Organization(Guid id, string code)
        {
            Id = id;
            Code = code;
        }
    }
}
