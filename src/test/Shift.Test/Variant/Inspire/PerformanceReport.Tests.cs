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
using Shift.Toolbox.Reporting.PerformanceReport;
using Shift.Toolbox.Reporting.PerformanceReport.Models;

using TimelineServices = Shift.Common.Timeline.Services;

namespace Shift.Test.Variant.Inspire
{
    [Collection(TestFixtures.Default)]
    public class PerformanceReport
    {
        static readonly ReportConfig _report1 = new ReportConfig
        {
            Language = "en",
            EmergentScore = 0.5m,
            ConsistentScore = 0.7m,
            RequiredRole = "HCA",
            RoleWeights = new[]
            {
                new ItemWeight { Name = "HCA", Weight = 1m },
            },
            AssessmentTypeWeights = new[]
            {
                new ItemWeight { Name = "CBA", Weight = 0.6m },
                new ItemWeight { Name = "SLA", Weight = 0.4m },
            }
        };

        static readonly ReportConfig _report2 = new ReportConfig
        {
            Language = "en",
            EmergentScore = 0.43m,
            ConsistentScore = 0.59m,
            RequiredRole = "LPN",
            RoleWeights = new[]
            {
                new ItemWeight { Name = "HCA", Weight = 0.5m },
                new ItemWeight { Name = "LPN", Weight = 1m },
            },
            AssessmentTypeWeights = new[]
            {
                new ItemWeight { Name = "CBA", Weight = 0.6m },
                new ItemWeight { Name = "SLA", Weight = 0.4m },
            }
        };

        static readonly ReportConfig _report3 = new ReportConfig
        {
            Language = "en",
            EmergentScore = 0.43m,
            ConsistentScore = 0.64m,
            RequiredRole = "RN",
            RoleWeights = new[]
            {
                new ItemWeight { Name = "RN", Weight = 1m },
                new ItemWeight { Name = "LPN", Weight = 0.5m },
                new ItemWeight { Name = "HCA", Weight = 0.25m },
            },
            AssessmentTypeWeights = new[]
            {
                new ItemWeight { Name = "CBA", Weight = 0.6m },
                new ItemWeight { Name = "SLA", Weight = 0.4m },
            }
        };

        static readonly Guid _areaId1 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb8d");
        static readonly Guid _areaId2 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb8e");
        static readonly Guid _areaId3 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb81");
        static readonly Guid _areaId4 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb82");
        static readonly Guid _areaId5 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb83");
        static readonly Guid _areaId6 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb84");
        static readonly Guid _areaId7 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb85");
        static readonly Guid _areaId8 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb86");
        static readonly Guid _areaId9 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb87");
        static readonly Guid _areaId10 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb88");
        static readonly Guid _areaId11 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb18");
        static readonly Guid _areaId12 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb28");
        static readonly Guid _areaId13 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb38");
        static readonly Guid _areaId14 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb48");
        static readonly Guid _areaId15 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb58");
        static readonly Guid _areaId16 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb68");
        static readonly Guid _areaId17 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb78");
        static readonly Guid _areaId18 = Guid.Parse("e451e181-ab0a-4153-8af4-c2f74597cb88");

        static readonly UserScore[] _scores1 = new[]
        {
            // Exam #1 HCA
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 1m },
            // Exam #1 LPN
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            // Exam #1 RN
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "RN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "RN" }, MaxScore = 1, Score = 0.5m },
            // Exam #2 HCA
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 0m },
            // Exam #3 LPN
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            // Exam #4 LPN
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 1m },
            // Exam #5 RN
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            // Exam #6 RN
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 0.8m },
        };

        static readonly UserScore[] _scores2 = new[]
        {
            // Exam #1 HCA
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 1m },
            // Exam #1 LPN
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            // Exam #1 RN
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "RN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId1, AssessmentType = "CBA", Roles = new []{ "RN" }, MaxScore = 1, Score = 0.5m },
            // Exam #2 HCA
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "HCA" }, MaxScore = 1, Score = 0m },
            // Exam #3 LPN
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            // Exam #4 LPN
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "LPN" }, MaxScore = 1, Score = 1m },
            // Exam #5 RN
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 0m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            // Exam #6 RN
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 1m },
            new UserScore { AreaId  = _areaId2, AssessmentType = "SLA", Roles = new []{ "RN" }, MaxScore = 1, Score = 0.8m },
        };

        [Fact]
        public void Reports_CreateReport1_Success()
        {
            var areas = new ReportCreator(_report1).CreateAreaScores(_scores1).AreaScores;

            Assert.Single(areas);

            var area = areas.FirstOrDefault(a => a.AreaId == _areaId1);
            Assert.NotNull(area);
            Assert.Equal(0.67m, Math.Round(area.GetWeightedScore(), 2));
        }

        [Fact]
        public void Reports_CreateReport2_Success()
        {
            var areas = new ReportCreator(_report2).CreateAreaScores(_scores1).AreaScores;

            Assert.Single(areas);

            var area = areas.FirstOrDefault(a => a.AreaId == _areaId1);
            Assert.NotNull(area);
            Assert.Equal(0.48m, Math.Round(area.GetWeightedScore(), 2));
        }

        [Fact]
        public void Reports_CreateReport3_Success()
        {
            var areas = new ReportCreator(_report3).CreateAreaScores(_scores1).AreaScores;

            Assert.Single(areas);

            var area = areas.FirstOrDefault(a => a.AreaId == _areaId1);
            Assert.NotNull(area);
            Assert.Equal(0.56m, Math.Round(area.GetWeightedScore(), 2));
        }

        [Fact]
        public void Reports_CreateReport3_TwoAreas_Success()
        {
            var areas = new ReportCreator(_report3).CreateAreaScores(_scores2).AreaScores;

            Assert.Equal(2, areas.Length);

            var area1 = areas.FirstOrDefault(a => a.AreaId == _areaId1);
            Assert.NotNull(area1);
            Assert.Equal(0.29m, Math.Round(area1.GetWeightedScore(), 2));

            var area2 = areas.FirstOrDefault(a => a.AreaId == _areaId2);
            Assert.NotNull(area2);
            Assert.Equal(0.28m, Math.Round(area2.GetWeightedScore(), 2));
        }

        [Fact]
        public void Reports_CreatePdf_Success()
        {
            var areas = new Area[]
            {
                new Area { Id = _areaId1, Name = "Abuse Awareness/Knowledge", Description = "Awareness of real or potential physical or mental harm caused by someone in a position of power or trust." },
                new Area { Id = _areaId2, Name = "Accountability, Responsibility and Ethical Behaviour", Description = "Performs the care provider role in a reflective, responsible, accountable and professional manner. Recognizes and responds to own self development, learning and health enhancement needs." },
                new Area { Id = _areaId3, Name = "Client Mobility", Description = "The ability to facilitate safe client ambulation and positioning." },
                new Area { Id = _areaId4, Name = "Collaborative Practice", Description = "Interacts with other members of the health care team in ways that contribute to effective working relationships and the achievement of goals." },
                new Area { Id = _areaId5, Name = "Common Health Challenges", Description = "Ability to apply knowledge of medical terminology, aging and common health challenges such as diabetes and multiple sclerosis." },
                new Area { Id = _areaId6, Name = "Critical Thinking and Problem Solving", Description = "An ability to use an informed approach to providing care and assistance (observing, gathering data and taking appropriate action)." },
                new Area { Id = _areaId7, Name = "Dementia and Cognitive Health Challenges", Description = "Knowledge of dementia and cognitive health challenges and the ability to apply appropriate strategies when caring for clients with dementia and cognitive health challenges." },
                new Area { Id = _areaId8, Name = "Infection Control", Description = "Practices that prevent the spread of infection. These include, but are not limited to, hand hygiene, and body substance fluid precautions." },
                new Area { Id = _areaId9, Name = "Mental Health Challenges", Description = "Knowledge of mental health challenges and the ability to apply appropriate strategies when caring for clients with mental health challenges." },
                new Area { Id = _areaId10, Name = "Person-Centred Practice", Description = "Person-centred care and assistance recognizes and respects the uniqueness of each individual client and their family. It provides for individuals to exercise control and autonomy over their own lives to the fullest extent possible." },
                new Area { Id = _areaId11, Name = "Planning, Time Management, and Organization", Description = "Planning and provision of timely and organized personal care and assistance." },
                new Area { Id = _areaId12, Name = "Providing Care", Description = "Provide personal care and assistance to clients according to the established plan of care." },
                new Area { Id = _areaId13, Name = "Reporting and Recording", Description = "Required written or verbal information that describes a client’s status, care, and services provided to that client." },
                new Area { Id = _areaId14, Name = "Safe Medication Delivery", Description = "Ability to perform within parameters of practice in relation to medication assistance and administration." },
                new Area { Id = _areaId15, Name = "Safety", Description = "Safety and protection of self and others within a variety of work environments." },
                new Area { Id = _areaId16, Name = "Therapeutic Communication", Description = "Interactions with others (client, family, or healthcare team members) that aims to enhance the client’s comfort, safety, trust or health and well-being." },
                new Area { Id = _areaId17, Name = "Violence Prevention", Description = "Recognizes and responds appropriately to actual or potential situations involving violence." },
                new Area { Id = _areaId18, Name = "Communication thérapeutique", Description = "The test description." },
            };

            var areaScores = new AreaScore[]
            {
                new AreaScore { AreaId = _areaId1, AssessmentTypeScores = CreateWeightedScore(0.7m) },
                new AreaScore { AreaId = _areaId2, AssessmentTypeScores = CreateWeightedScore(0.6m) },
                new AreaScore { AreaId = _areaId3, AssessmentTypeScores = CreateWeightedScore(0.3m) },
                new AreaScore { AreaId = _areaId4, AssessmentTypeScores = CreateWeightedScore(0.8m) },
                new AreaScore { AreaId = _areaId5, AssessmentTypeScores = CreateWeightedScore(0.5m) },
                new AreaScore { AreaId = _areaId6, AssessmentTypeScores = CreateWeightedScore(0.7m) },
                new AreaScore { AreaId = _areaId7, AssessmentTypeScores = CreateWeightedScore(0.2m) },
                new AreaScore { AreaId = _areaId8, AssessmentTypeScores = CreateWeightedScore(0.6m) },
                new AreaScore { AreaId = _areaId9, AssessmentTypeScores = CreateWeightedScore(0.85m) },
                new AreaScore { AreaId = _areaId10, AssessmentTypeScores = CreateWeightedScore(0.3m) },
                new AreaScore { AreaId = _areaId11, AssessmentTypeScores = CreateWeightedScore(0.5m) },
                new AreaScore { AreaId = _areaId12, AssessmentTypeScores = CreateWeightedScore(0.5m) },
                new AreaScore { AreaId = _areaId13, AssessmentTypeScores = CreateWeightedScore(0.6m) },
                new AreaScore { AreaId = _areaId14, AssessmentTypeScores = CreateWeightedScore(0.4m) },
                new AreaScore { AreaId = _areaId15, AssessmentTypeScores = CreateWeightedScore(0.1m) },
                new AreaScore { AreaId = _areaId16, AssessmentTypeScores = CreateWeightedScore(1.0m) },
                new AreaScore { AreaId = _areaId17, AssessmentTypeScores = CreateWeightedScore(0.45m) },
            };


            var areaScoresResult = new AreaScoreResult
            {
                AreaScores = areaScores,
                AssessmentTypeDates = new AssessmentTypeDate[]
                {
                    new AssessmentTypeDate { AssessmentType = "CBA", Date = new DateTime(2023, 10, 1) },
                    new AssessmentTypeDate { AssessmentType = "SLA", Date = new DateTime(2023, 10, 10) },
                }
            };

            var userReport = new UserReport
            {
                ReportType = UserReportType.Report1,
                FullName = "Aleksey Terzi",
                PersonCode = "0101010",
                ReportIssued = DateTime.Now,
                Areas = areas,
                AreaScores = areaScoresResult
            };

            new ReportCreator(_report1).CreatePdf(userReport);

            AssessmentTypeScore[] CreateWeightedScore(decimal score)
                => new[] { new AssessmentTypeScore { Weight = 1, RoleScores = new[] { new RoleScore { WeightedScore = score, WeightedMaxScore = 1 } } } };
        }
    }

    [CollectionDefinition(Default)]
    public class TestFixtures : ICollectionFixture<TestFixture>
    {
        public const string Default = "Default";
    }

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

            _guidGenerator = new UuidFactory();

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

            Organization2 = new Organization(OrganizationIdentifiers.Test, "insite");
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
