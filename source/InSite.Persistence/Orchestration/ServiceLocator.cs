using System;
using System.Collections.Generic;
using System.Net;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;
using Shift.Common.Timeline.Queries;
using Shift.Common.Timeline.Snapshots;

using InSite.Application;
using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Application.Contacts.Read;
using InSite.Application.Contacts.Write;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Files.Read;
using InSite.Application.Glossaries.Read;
using InSite.Application.Glossaries.Write;
using InSite.Application.Integrations.Prometric;
using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Application.Issues.Read;
using InSite.Application.Issues.Write;
using InSite.Application.Logs.Read;
using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Organizations.Read;
using InSite.Application.Organizations.Write;
using InSite.Application.Payments.Read;
using InSite.Application.Payments.Write;
using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;
using InSite.Application.Records.Read;
using InSite.Application.Records.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Application.Resources.Read;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Application.Standards.Read;
using InSite.Application.Standards.Write;
using InSite.Application.Surveys.Read;
using InSite.Application.Surveys.Write.Handlers;
using InSite.Application.Utility.Read;
using InSite.Domain;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.Persistence.Integration.BCMail;
using InSite.Persistence.Integration.DirectAccess;
using InSite.Persistence.Integration.Prometric;
using InSite.Persistence.Platform.Collections;
using InSite.Persistence.Plugin.CMDS;
using InSite.Persistence.Plugin.RCABC;
using InSite.Persistence.Plugin.SkilledTradesBC;

using Serilog;

using Shift.Common;
using Shift.Common.Integration.Google;
using Shift.Common.Json;
using Shift.Constant;
using Shift.Toolbox;
using Shift.Toolbox.Integration.DirectAccess;

using FilePaths = Shift.Common.FilePaths;
using Maintenance = Shift.Common.Maintenance;
using OldUserSearch = InSite.Persistence.UserSearch;
using ShiftAppSettings = Shift.Common.AppSettings;
using Urls = Shift.Common.Urls;

namespace InSite
{
    public static class ServiceLocator
    {
        // AppSettings

        public static ShiftAppSettings AppSettings { get; private set; }
        public static IPartitionModel Partition { get; private set; }

        public static FilePaths FilePaths { get; private set; }
        public static Maintenance Maintenance { get; private set; }
        public static Urls Urls { get; private set; }

        // Timeline Services

        public static IAggregateSearch AggregateSearch { get; private set; }

        public static ICommandQueue CommandQueue { get; private set; }
        public static ICommandStore CommandStore { get; private set; }
        public static ICommandSearch CommandSearch { get; private set; }

        public static IChangeQueue ChangeQueue { get; private set; }
        public static IChangeStore ChangeStore { get; private set; }
        public static IChangeBuffer ChangeBuffer { get; private set; }

        public static IQueryQueue QueryQueue { get; private set; }

        public static IIdentityService IdentityService { get; set; }
        public static IChangeRepository ChangeRepository { get; private set; }
        public static IChangeRepository SnapshotRepository { get; private set; }

        // Infrastructure Services

        public static ILogger Logger { get; internal set; }
        public static ILogger Log => Logger;

        public static IAlertMailer AlertMailer { get; internal set; }
        public static IJsonSerializer Serializer { get; internal set; }

        public static IPrometricApi PrometricApi { get; internal set; }
        public static SwiftSmsGatewayClient SwiftSmsGatewayClient { get; internal set; }
        public static TranslationClient TranslationClient { get; internal set; }

        public static MailgunServer MailgunServer { get; private set; }
        public static IEmailOutbox EmailOutbox { get; private set; }
        public static FFmpeg FFmpeg { get; private set; }
        public static CountrySearch CountrySearch { get; private set; }
        public static ProvinceSearch ProvinceSearch { get; private set; }

        // Application Services

        public static CoreProcessManager CoreProcessManager { get; set; }

        public static IAchievementSearch AchievementSearch { get; set; }
        public static IAchievementStore AchievementStore { get; set; }
        public static IEventSearch EventSearch { get; set; }
        public static IEventStore EventStore { get; set; }
        public static ILearnerAttemptSummarySearch LearnerAttemptSummarySearch { get; set; }
        public static IAttemptSearch AttemptSearch { get; set; }
        public static IInstructorAttemptStore InstructorAttemptStore { get; set; }
        public static IPerformanceReportSearch PerformanceReportSearch { get; set; }
        public static ITakerReportSearch TakerReportSearch { get; set; }
        public static IBankSearch BankSearch { get; set; }
        public static IBankStore BankStore { get; set; }
        public static ICollectionSearch CollectionSearch { get; set; }
        public static IContactSearch ContactSearch { get; set; }
        public static IContentSearch ContentSearch { get; set; }
        public static IContentStore ContentStore { get; set; }
        public static ICourseObjectSearch CourseObjectSearch { get; set; }
        public static ICourseObjectStore CourseObjectStore { get; set; }
        public static ICourseSearch CourseSearch { get; set; }
        public static ICourseStore CourseStore { get; set; }
        public static ICourseDistributionSearch CourseDistributionSearch { get; set; }
        public static ICourseDistributionStore CourseDistributionStore { get; set; }
        public static IGlossarySearch GlossarySearch { get; set; }
        public static IGlossaryStore GlossaryStore { get; set; }
        public static IInvoiceSearch InvoiceSearch { get; set; }
        public static IInvoiceStore InvoiceStore { get; set; }
        public static ICaseSearch IssueSearch { get; set; }
        public static ICaseStore IssueStore { get; set; }
        public static IMessageSearch MessageSearch { get; set; }
        public static IMessageStore MessageStore { get; set; }
        public static IPaymentSearch PaymentSearch { get; set; }
        public static IPaymentStore PaymentStore { get; set; }
        public static IProgramSearch ProgramSearch { get; set; }
        public static IProgramStore ProgramStore { get; set; }
        public static IProgramService ProgramService { get; set; }
        public static IProgressRestarter ProgressRestarter { get; set; }
        public static IRecordSearch RecordSearch { get; set; }
        public static IPeriodSearch PeriodSearch { get; set; }
        public static IPeriodStore PeriodStore { get; set; }
        public static IRecordStore RecordStore { get; set; }
        public static IRubricSearch RubricSearch { get; set; }
        public static IRubricStore RubricStore { get; set; }
        public static IJournalSearch JournalSearch { get; set; }
        public static IJournalStore JournalStore { get; set; }
        public static IRegistrationSearch RegistrationSearch { get; set; }
        public static IRegistrationStore RegistrationStore { get; set; }
        public static ISnapshotStore SnapshotStore { get; set; }
        public static ISnapshotStrategy SnapshotStrategy { get; set; }
        public static IOldStandardSearch OldStandardSearch { get; set; }
        public static ISurveySearch SurveySearch { get; set; }
        public static ISurveyStore SurveyStore { get; set; }
        public static IPageSearch PageSearch { get; set; }
        public static IPageStore PageStore { get; set; }
        public static ISiteSearch SiteSearch { get; set; }
        public static ISiteStore SiteStore { get; set; }
        public static IOrganizationSearch OrganizationSearch { get; set; }
        public static IOrganizationStore OrganizationStore { get; set; }
        public static IUploadSearch UploadSearch { get; set; }
        public static IGroupSearch GroupSearch { get; set; }
        public static IGroupStore GroupStore { get; set; }
        public static IFileSearch FileSearch { get; set; }
        public static IFileManagerService FileManagerService { get; set; }
        public static IStorageService StorageService { get; set; }
        public static IMembershipSearch MembershipSearch { get; set; }
        public static IMembershipStore MembershipStore { get; set; }
        public static IMembershipReasonSearch MembershipReasonSearch { get; set; }
        public static IMembershipReasonStore MembershipReasonStore { get; set; }
        public static IUserSearch UserSearch { get; set; }
        public static IUserStore UserStore { get; set; }
        public static IPersonSearch PersonSearch { get; set; }
        public static IPersonStore PersonStore { get; set; }
        public static IPersonSecretSearch PersonSecretSearch { get; set; }
        public static IPersonSecretStore PersonSecretStore { get; set; }
        public static IStandardSearch StandardSearch { get; set; }
        public static IStandardStore StandardStore { get; set; }
        public static IStandardTierStore StandardTierStore { get; set; }
        public static IStandardValidationSearch StandardValidationSearch { get; set; }
        public static IStandardValidationStore StandardValidationStore { get; set; }
        public static IQuizSearch QuizSearch { get; set; }
        public static IQuizStore QuizStore { get; set; }
        public static IQuizAttemptSearch QuizAttemptSearch { get; set; }
        public static IQuizAttemptStore QuizAttemptStore { get; set; }

        // Custom Tenant-Specific Services

        public static CmdsProcessor CmdsProcessor { get; set; }
        public static IDirectAccessSearch DirectAccessSearch { get; set; }
        public static IDirectAccessClient DirectAccessServer { get; set; }
        public static IDirectAccessStore DirectAccessStore { get; set; }
        public static IBCMailServer BCMailServer { get; set; }
        public static CustomDistributionSubscriber BCMailCommandReceiver { get; set; }

        // Subscribers

        public static AchievementCommandReceiver AchievementCommandReceiver = null;
        public static AchievementChangeProjector AchievementChangeProjector = null;
        public static EventCommandReceiver EventCommandReceiver = null;
        public static EventChangeProjector EventChangeProjector = null;
        public static AttemptCommandReceiver AttemptCommandSubscriber = null;
        public static AttemptChangeProjector AttemptChangeProjector = null;
        public static AttemptChangeProcessor AttemptChangeProcessor = null;
        public static BankCommandReceiver BankCommandSubscriber = null;
        public static BankChangeProjector BankChangeProjector = null;
        public static CourseObjectCommandReceiver CourseObjectCommandReceiver = null;
        public static CourseCommandReceiver CourseCommandReceiver = null;
        public static CourseChangeProjector CourseChangeSubscriber = null;
        public static CredentialCommandReceiver CredentialCommandReceiver = null;
        public static ContactCommandReceiver ContactCommandSubscriber = null;
        public static ContactChangeProjector ContactChangeSubscriber = null;
        public static GroupCommandReceiver GroupCommandSubscriber = null;
        public static GroupChangeProjector GroupChangeSubscriber = null;
        public static GlossaryCommandReceiver GlossaryCommandSubscriber = null;
        public static GlossaryChangeProjector GlossaryChangeSubscriber = null;
        public static InvoiceChangeProjector InvoiceChangeProjector = null;
        public static InvoiceCommandReceiver InvoiceCommandReceiver = null;
        public static IssueCommandReceiver IssueCommandSubscriber = null;
        public static IssueChangeProjector IssueChangeSubscriber = null;
        public static MembershipCommandReceiver MembershipCommandSubscriber = null;
        public static MembershipChangeProjector MembershipChangeSubscriber = null;
        public static MessageCommandReceiver MessageCommandSubscriber = null;
        public static MessageChangeProjector MessageChangeSubscriber = null;
        public static PaymentChangeProcessor PaymentChangeProcessor = null;
        public static PaymentChangeProjector PaymentChangeProjector = null;
        public static PaymentCommandReceiver PaymentCommandReceiver = null;
        public static PeriodChangeProjector PeriodChangeProjector = null;
        public static PeriodCommandReceiver PeriodCommandReceiver = null;
        public static ProgressCommandReceiver ProgressCommandSubscriber = null;
        public static GradebookCommandReceiver RecordCommandSubscriber = null;
        public static GradebookChangeProjector RecordChangeSubscriber = null;
        public static JournalSetupCommandReceiver JournalSetupCommandSubscriber = null;
        public static JournalCommandReceiver JournalCommandSubscriber = null;
        public static JournalChangeProjector JournalChangeSubscriber = null;
        public static RegistrationCommandReceiver RegistrationCommandSubscriber = null;
        public static RegistrationChangeProjector RegistrationChangeProjector = null;
        public static ResponseCommandReceiver ResponseCommandReceiver = null;
        public static ResponseChangeProcessor ResponseChangeProcessor = null;
        public static ResponseChangeProjector ResponseChangeProjector = null;
        public static RubricCommandReceiver RubricCommandReceiver = null;
        public static RubricChangeProjector RubricChangeProjector = null;
        public static UserCommandReceiver UserCommandSubscriber = null;
        public static UserChangeProjector UserChangeSubscriber = null;
        public static PersonCommandReceiver PersonCommandSubscriber = null;
        public static PersonChangeProjector PersonChangeSubscriber = null;
        public static PersonSecretCommandReceiver PersonSecretCommandSubscriber = null;
        public static PersonSecretChangeProjector PersonSecretChangeSubscriber = null;
        public static StandardCommandReceiver StandardCommandSubscriber = null;
        public static StandardChangeProjector StandardChangeSubscriber = null;
        public static StandardTierProjector StandardTierProjector = null;
        public static StandardValidationCommandReceiver StandardValidationCommandSubscriber = null;
        public static StandardValidationChangeProjector StandardValidationChangeSubscriber = null;

        public static SiteCommandReceiver SiteCommandReceiver = null;
        public static SiteChangeProcessor SiteChangeProcessor = null;
        public static SiteChangeProjector SiteChangeProjector = null;
        public static PageCommandReceiver PageCommandReceiver = null;
        public static PageChangeProcessor PageChangeProcessor = null;
        public static PageChangeProjector PageChangeProjector = null;

        public static SurveyCommandReceiver SurveyCommandReceiver = null;
        public static SurveyChangeProcessor SurveyChangeProcessor = null;
        public static SurveyChangeProjector SurveyChangeProjector = null;
        public static OrganizationCommandReceiver OrganizationCommandReceiver = null;
        public static OrganizationChangeProcessor OrganizationChangeProcessor = null;
        public static OrganizationChangeSubscriber OrganizationChangeSubscriber = null;

        #region Methods

        private static void InitializeServerCertificateValidation()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = (eventSender, cert, chain, errors) => true;
        }

        public static void InitializeAppSettings(AppSettings settings, StartupRequirements requirements, bool disableCertificateValidation = true, Func<IPartitionModel> partitionFactory = null)
        {
            if (disableCertificateValidation)
                InitializeServerCertificateValidation();

            var app = settings.Application;
            var security = settings.Security;

            AppSettings = settings;

            OrganizationIdentifiers.Initialize(settings.Application.Organizations);

            TranslationClient = new TranslationClient(AppSettings.Engine);

            var swiftSmsGatewayUri = new Uri(AppSettings.Integration.SwiftSmsGateway.BaseUrl);

            var swiftSmsGatewaySerializer = new JsonSerializer2();

            var swiftSmsGatewayFactory = new SwiftSmsGatewayClientFactory(swiftSmsGatewayUri);

            SwiftSmsGatewayClient = new SwiftSmsGatewayClient(swiftSmsGatewayFactory, swiftSmsGatewaySerializer);

            FilePaths = new FilePaths(settings.DataFolderShare, settings.DataFolderEnterprise);
            Maintenance = new Maintenance(FilePaths);
            Urls = new Urls(settings.Environment, security.Domain, app.HelpUrl);

            DbSettings.Init(AppSettings.Database.ConnectionStrings.Shift, security.Domain, AppSettings.Database.IsReadOnly);

            Partition = partitionFactory == null
                ? PartitionSearch.GetPartitionModel(requirements.RequirePartitionSettings)
                : partitionFactory();

            Persistence.MessageSearch.Init(AppSettings.Application.UseStrictModeForEmailEnabled);

            CurrentPartition.IsE03 = Partition.IsE03();

            CurrentPartition.OrganizationId = Partition.Identifier;

            AchievementTypes.RenameModuleToLearningModule = Partition.IsE03();
            MessageLinkExtractor.Initialize(StringHelper.Split(Partition.WhitelistDomains));

            UuidFactory.NamespaceId = UuidFactory.CreateV5ForDns(settings.Security.Domain);
        }

        public static void InitializeLogger(ILogger logger)
        {
            Logger = Serilog.Log.Logger = logger;
        }

        public static void InitializeTimeline(IIdentityService identityService)
        {
            var serializer = new Serializer();

            TimelineRegistry.Initialize(serializer);

            Serializer = new Serializer();

            // Register the organization and user identification service.

            IdentityService = identityService;

            // Register the implementations for storing and sending commands.

            var timeline = AppSettings.Timeline;
            var connectionString = AppSettings.Database.ConnectionStrings.Shift;

            CommandStore = new CommandStore(Serializer);
            CommandSearch = new CommandSearch(Serializer);
            CommandQueue = new CommandQueue(CommandStore, timeline.Debug);

            // Register the implementations for storing and publishing events.

            var aggregateFolder = System.IO.Path.Combine(AppSettings.DataFolderEnterprise, "Aggregates");
            ChangeStore = new ChangeStore(Serializer, connectionString, aggregateFolder);
            ChangeBuffer = new ChangeBuffer(Serializer, connectionString);
            ChangeRepository = new ChangeRepository(ChangeStore);
            SnapshotStore = new SnapshotStore(connectionString, aggregateFolder);
            SnapshotStrategy = new SnapshotStrategy(timeline.SnapshotInterval);
            SnapshotRepository = new SnapshotRepository(ChangeStore, ChangeRepository, SnapshotStore, SnapshotStrategy);
            ChangeQueue = new ChangeQueue();
            AggregateSearch = new AggregateSearch(SnapshotRepository);

            // Register the implementations for processing queries.

            QueryQueue = new QueryQueue();
        }

        public static void InitializeApplication(Action<Exception> error)
        {
            // Register query store and search services.

            ContactSearch = new ContactSearch();

            ContentSearch = TContentSearch.Instance;
            ContentStore = new TContentStore();

            OrganizationSearch = new OrganizationSearch();
            OrganizationStore = new OrganizationStore(Serializer);

            BankSearch = new BankSearch(AggregateSearch);

            AchievementSearch = new AchievementSearch();
            AchievementStore = new AchievementStore();

            EventSearch = new EventSearch(SnapshotRepository);
            EventStore = new EventStore(Serializer, BankSearch);

            AttemptSearch = new AttemptSearch();
            InstructorAttemptStore = new InstructorAttemptStore();
            LearnerAttemptSummarySearch = new LearnerAttemptSummarySearch();
            TakerReportSearch = new TakerReportSearch();

            PerformanceReportSearch = new PerformanceReportSearch();

            CourseObjectStore = new Course1Store();
            CourseObjectSearch = new Course1Search();

            CourseStore = new QCourseStore(ContentStore);
            CourseSearch = new QCourseSearch();

            CourseDistributionStore = new CourseDistributionStore();
            CourseDistributionSearch = new CourseDistributionSearch();

            InvoiceSearch = new InvoiceSearch();
            InvoiceStore = new InvoiceStore();

            IssueSearch = new CaseSearch();
            IssueStore = new CaseStore();

            MessageSearch = Persistence.MessageSearch.Instance;
            MessageStore = new MessageStore(AppSettings.Security.Domain);

            PaymentSearch = new PaymentSearch();
            PaymentStore = new PaymentStore();

            PeriodSearch = new PeriodSearch();
            PeriodStore = new PeriodStore();

            GlossarySearch = new GlossarySearch();
            GlossaryStore = new GlossaryStore();

            RegistrationSearch = new RegistrationSearch();
            RegistrationStore = new RegistrationStore(Serializer);

            BankStore = new BankStore(LearnerAttemptSummarySearch);

            OldStandardSearch = new OldStandardSearch();

            PageSearch = new PageSearch(SnapshotRepository, ContentSearch);
            PageStore = new PageStore(error, IdentityService, OrganizationSearch);

            SiteSearch = new Persistence.SiteSearch();
            SiteStore = new SiteStore();

            SurveySearch = new SurveySearch(AggregateSearch);
            SurveyStore = new SurveyStore();

            UploadSearch = new UploadSearch2();

            ProgramSearch = new ProgramSearch2(ContentSearch);
            ProgramStore = new ProgramStore1();
            ProgramService = new ProgramService(ContactSearch, AchievementSearch, ProgramSearch, ProgramStore);

            CollectionSearch = new CollectionSearch();

            RecordSearch = new RecordSearch(AggregateSearch);
            RecordStore = new RecordStore();

            RubricSearch = new QRubricSearch();
            RubricStore = new QRubricStore(ContentStore);

            JournalSearch = new JournalSearch();
            JournalStore = new JournalStore(ContactSearch);

            GroupSearch = new QGroupSearch();
            GroupStore = new QGroupStore(ContactSearch);

            MembershipSearch = new QMembershipSearch();
            MembershipStore = new QMembershipStore();

            MembershipReasonSearch = new QMembershipReasonSearch();
            MembershipReasonStore = new QMembershipReasonStore();

            UserSearch = new QUserSearch();
            UserStore = new QUserStore();

            PersonSearch = new QPersonSearch();
            PersonStore = new QPersonStore();

            PersonSecretSearch = new QPersonSecretSearch();
            PersonSecretStore = new QPersonSecretStore();

            StandardSearch = new QStandardSearch();
            StandardStore = new QStandardStore();
            StandardTierStore = new QStandardTierStore();

            StandardValidationSearch = new QStandardValidationSearch();
            StandardValidationStore = new QStandardValidationStore();

            FileSearch = new TFileSearch(Serializer);

            QuizSearch = new TQuizSearch();
            QuizStore = new TQuizStore();
            QuizAttemptSearch = new TQuizAttemptSearch();
            QuizAttemptStore = new TQuizAttemptStore();

            string userNameResolver(Guid id) => OldUserSearch.Bind(id, x => x.FullName);
            var fileStore = new TFileStore(new FileChangeFactory(userNameResolver), Serializer);

            FileManagerService = new FileManagerService(FilePaths);
            StorageService = new StorageService(FileSearch, fileStore, FileManagerService, AppSettings.Application.TempFileExpirationInMinutes);

            // Register subscribers for application commands and domain events.

            AchievementCommandReceiver = new AchievementCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            AchievementChangeProjector = new AchievementChangeProjector(ChangeQueue, ChangeStore, AggregateSearch, AchievementStore);

            AttemptCommandSubscriber = new AttemptCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            AttemptChangeProjector = new AttemptChangeProjector(ChangeQueue, ChangeStore, InstructorAttemptStore, BankSearch);

            EventCommandReceiver = new EventCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            EventChangeProjector = new EventChangeProjector(ChangeQueue, ChangeStore, EventStore);

            BankCommandSubscriber = new BankCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            BankChangeProjector = new BankChangeProjector(ChangeQueue, ChangeStore, BankStore);

            CourseObjectCommandReceiver = new CourseObjectCommandReceiver(CommandQueue, ChangeQueue);

            CourseCommandReceiver = new CourseCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, CourseStore);
            CourseChangeSubscriber = new CourseChangeProjector(ChangeQueue, CourseStore);

            CredentialCommandReceiver = new CredentialCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);

            ContactCommandSubscriber = new ContactCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, MessageSearch);
            ContactChangeSubscriber = new ContactChangeProjector(ChangeQueue);

            GroupCommandSubscriber = new GroupCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, GroupSearch);
            GroupChangeSubscriber = new GroupChangeProjector(ChangeQueue, ChangeStore, GroupStore);

            GlossaryCommandSubscriber = new GlossaryCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            GlossaryChangeSubscriber = new GlossaryChangeProjector(ChangeQueue, GlossaryStore);

            RecordCommandSubscriber = new GradebookCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, EventSearch, RecordSearch);
            RecordChangeSubscriber = new GradebookChangeProjector(ChangeQueue, ChangeStore, RecordStore);

            ProgressCommandSubscriber = new ProgressCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, RecordSearch);

            JournalSetupCommandSubscriber = new JournalSetupCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, JournalSearch);
            JournalCommandSubscriber = new JournalCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            JournalChangeSubscriber = new JournalChangeProjector(ChangeQueue, JournalStore, ContentStore);

            InvoiceCommandReceiver = new InvoiceCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            InvoiceChangeProjector = new InvoiceChangeProjector(ChangeQueue, InvoiceStore);

            IssueCommandSubscriber = new IssueCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            IssueChangeSubscriber = new IssueChangeProjector(ChangeQueue, ChangeStore, IssueStore);

            MembershipCommandSubscriber = new MembershipCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            MembershipChangeSubscriber = new MembershipChangeProjector(ChangeQueue, MembershipStore, MembershipReasonStore);

            MessageCommandSubscriber = new MessageCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            MessageChangeSubscriber = new MessageChangeProjector(ChangeQueue, ChangeStore, MessageStore, ContentStore);

            PaymentChangeProjector = new PaymentChangeProjector(ChangeQueue, PaymentStore);
            PaymentCommandReceiver = new PaymentCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);

            PeriodChangeProjector = new PeriodChangeProjector(ChangeQueue, PeriodStore);
            PeriodCommandReceiver = new PeriodCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, RecordSearch);

            RegistrationCommandSubscriber = new RegistrationCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, RegistrationSearch, BankSearch);
            RegistrationChangeProjector = new RegistrationChangeProjector(ChangeQueue, RegistrationStore, RegistrationSearch);

            ResponseCommandReceiver = new ResponseCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            ResponseChangeProjector = new ResponseChangeProjector(ChangeQueue, SurveyStore);

            RubricCommandReceiver = new RubricCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, RubricStore);
            RubricChangeProjector = new RubricChangeProjector(ChangeQueue, RubricStore);

            UserCommandSubscriber = new UserCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, UserSearch, PersonSearch);
            UserChangeSubscriber = new UserChangeProjector(ChangeQueue, UserStore);

            PersonCommandSubscriber = new PersonCommandReceiver(
                CommandQueue,
                ChangeQueue,
                SnapshotRepository,
                UserSearch,
                PersonSearch,
                organizationId => InSite.Persistence.OrganizationSearch.GetPersonFullNamePolicy(organizationId)
            );
            PersonChangeSubscriber = new PersonChangeProjector(ChangeQueue, PersonStore);

            PersonSecretCommandSubscriber = new PersonSecretCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            PersonSecretChangeSubscriber = new PersonSecretChangeProjector(ChangeQueue, PersonSecretStore);

            StandardCommandSubscriber = new StandardCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, StandardSearch);
            StandardChangeSubscriber = new StandardChangeProjector(ChangeQueue, StandardStore, StandardTierStore, ContentStore);

            StandardValidationCommandSubscriber = new StandardValidationCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, StandardValidationSearch);
            StandardValidationChangeSubscriber = new StandardValidationChangeProjector(ChangeQueue, StandardValidationStore);

            SiteCommandReceiver = new SiteCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, PageSearch);
            SiteChangeProjector = new SiteChangeProjector(ChangeQueue, AggregateSearch, SiteStore, ContentStore);

            PageCommandReceiver = new PageCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            PageChangeProjector = new PageChangeProjector(ChangeQueue, AggregateSearch, PageStore, ContentStore);

            SurveyCommandReceiver = new SurveyCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository);
            SurveyChangeProjector = new SurveyChangeProjector(ChangeQueue, SurveyStore);

            OrganizationCommandReceiver = new OrganizationCommandReceiver(CommandQueue, ChangeQueue, SnapshotRepository, OrganizationStore);
            OrganizationChangeSubscriber = new OrganizationChangeSubscriber(ChangeQueue, OrganizationStore);

            InitializeCustomProjectManagers();
        }

        public static void InitializeCustomProjectManagers()
        {
            _ = new RcabcRegistrationChangeProjector(ChangeQueue);
        }

        public static void InitializeCustomization(IApiRequestLogger apiRequestLogger)
        {
            DirectAccessSearch = new DirectAccessSearch();
            DirectAccessServer = new DirectAccessClient(AppSettings.Environment.Name, AppSettings.Variant.SkilledTradesBC, apiRequestLogger, Serializer);
            DirectAccessStore = new DirectAccessStore(DirectAccessSearch, UserSearch, PersonSearch);
            BCMailServer = new BCMailServer(AppSettings.Environment.Name, IdentityService, apiRequestLogger);
            BCMailCommandReceiver = new CustomDistributionSubscriber(CommandQueue, ChangeQueue, SnapshotRepository, BCMailServer, EventSearch);
        }

        public static void InitializeProcessManagers(ICommander commander, Action<string, string> warning, Action<Exception> error, IApiRequestLogger apiRequestLogger)
        {
            // Optional projectors.

            StandardTierProjector = new StandardTierProjector(ChangeQueue, StandardTierStore);

            // Core process managers

            CoreProcessManager = new CoreProcessManager(ChangeQueue, EmailOutbox, RegistrationSearch, ContentSearch, ContactSearch);
            _ = new AchievementChangeProcessor(commander, ChangeQueue, AlertMailer, AchievementSearch, CourseObjectSearch, AttemptSearch, JournalSearch, ProgramSearch, ProgramStore, ProgramService, MessageSearch, ContentSearch, ContactSearch, Partition.IsE03());
            _ = new EventChangeProcessor(commander, ChangeQueue, AlertMailer, ContactSearch, EventSearch, RegistrationSearch, RecordSearch);
            AttemptChangeProcessor = new AttemptChangeProcessor(commander, ChangeQueue, AlertMailer, AttemptSearch, BankSearch, ContactSearch, CourseObjectSearch, RecordSearch, OrganizationSearch);
            _ = new BankChangeProcessor(commander, ChangeQueue, BankSearch, PageSearch, RecordSearch, UploadSearch, AppSettings.Security.Domain);
            ProgressRestarter = new ProgressRestarter(SendCommand, RecordSearch);
            _ = new CourseObjectChangeProcessor(commander, ChangeQueue, ProgressRestarter);
            _ = new IssueChangeProcessor(ChangeQueue, AlertMailer, IssueSearch);
            _ = new MembershipChangeProcessor(commander, ChangeQueue, ContactSearch, GroupSearch, MembershipSearch, InvoiceSearch, AlertMailer, Urls, orgId => Sequence.Increment(orgId, SequenceType.Invoice));
            PaymentChangeProcessor = new PaymentChangeProcessor(commander, ChangeQueue, InvoiceSearch, PaymentSearch, apiRequestLogger);
            _ = new RegistrationChangeProcessor(commander, ChangeQueue, OrganizationSearch, RegistrationSearch, PrometricApi);
            ResponseChangeProcessor = new ResponseChangeProcessor(Urls, commander, ChangeQueue, AlertMailer, ContactSearch, ContentSearch, CourseObjectSearch, GroupSearch, IssueSearch, MessageSearch, RecordSearch, SurveySearch, ProgramSearch, ProgramStore, ProgramService, AchievementSearch, PersonSearch, OrganizationSearch);
            SurveyChangeProcessor = new SurveyChangeProcessor(commander, ChangeQueue, SurveySearch, GroupSearch);
            SiteChangeProcessor = new SiteChangeProcessor(commander, ChangeQueue, SiteSearch);
            PageChangeProcessor = new PageChangeProcessor(commander, ChangeQueue, PageSearch);
            OrganizationChangeProcessor = new OrganizationChangeProcessor(commander, ChangeQueue, apiRequestLogger);
            _ = new GradebookChangeProcessor(commander, ChangeQueue, AlertMailer, Urls, AchievementSearch, BankSearch, RecordSearch, CourseObjectSearch, CourseObjectStore, MessageSearch, ContactSearch, ContentSearch, ProgramSearch, ProgramStore, ProgramService, AggregateSearch);
            _ = new JournalChangeProcessor(Urls, commander, ChangeQueue, JournalSearch, ContactSearch, AlertMailer, OrganizationSearch);
            _ = new GroupChangeProcessor(commander, ChangeQueue, AlertMailer);
            _ = new RubricChangeProcessor(commander, ChangeQueue, BankSearch);

            // Custom process managers

            CmdsProcessor = new CmdsProcessor(ChangeQueue, EmailOutbox, ContactSearch);
            _ = new EventProcessor(Urls, commander, ChangeQueue, CommandSearch, SnapshotRepository, EventSearch, BankSearch, AttemptSearch, ContactSearch, RegistrationSearch, OldStandardSearch, GroupSearch, DirectAccessServer, warning, error, AlertMailer, FilePaths, AppSettings.Security.Domain);
            _ = new RegistrationProcessor(commander, ChangeQueue, CommandSearch, EventSearch, BankSearch, ContactSearch, RegistrationSearch, RegistrationStore, GroupSearch, DirectAccessServer, DirectAccessStore, warning, error, AlertMailer, FilePaths, AppSettings.Security.Domain);
            _ = new RcabcGradebookChangeProcessor(commander, ChangeQueue, SnapshotRepository, RegistrationSearch, RecordSearch);
            _ = new RcabcRegistrationChangeProcessor(commander, ChangeQueue, SnapshotRepository, EventSearch, RecordSearch);
            _ = new SurveyResponseChangeProcessor(commander, ChangeQueue, SurveySearch);
        }

        public static void InitializeInfrastructure(IIdentityService identityService, IApiRequestLogger apiRequestLogger, Action<string> error)
        {
            var domain = AppSettings.Security.Domain;

            MailgunServer = new MailgunServer(
                AppSettings.Integration.Mailgun,
                AppSettings.Application.EmailOutboxDisabled,
                AppSettings.Application.EmailOutboxFiltered,
                StringHelper.Split(Partition.WhitelistDomains),
                StringHelper.Split(Partition.WhitelistEmails),
                AppSettings.Application.AlertsToForceSendList
            );
            EmailOutbox = new EmailOutbox(MailgunServer, AppSettings.Environment.Name);
            FFmpeg = new FFmpeg(AppSettings.Application.FFmpegFolderPath);
            CountrySearch = new CountrySearch(AppSettings.Engine.Api.Google.BaseUrl);
            ProvinceSearch = new ProvinceSearch(AppSettings.Engine.Api.Google.BaseUrl, CountrySearch);

            AlertMailer = new AlertMailer(AppSettings.Environment.Name, EmailOutbox, error);

            PrometricApi = new PrometricApi(
                AppSettings.Environment.Name,
                AppSettings.Integration.Prometric,
                identityService.GetCurrentOrganization,
                identityService.GetCurrentUser,
                apiRequestLogger);

            HtmlBuilder.Initialize(AppSettings.Engine.Api.Premailer);
            ImageHelper.Initialize(AppSettings.Engine.Api.ImageMagick);
        }

        public static void BookmarkCommand(ICommand command, DateTimeOffset expired)
        {
            SetOrigin(command);

            CommandQueue.Bookmark(command, expired);
        }

        public static void SendCommand(ICommand command)
        {
            SetOrigin(command);

            if (AppSettings.Timeline.IsServer)
                ExecuteCommand(command);
            else
                QueueCommand(command);
        }

        public static void QueueCommand(ICommand command)
        {
            var api = AppSettings.Shift.Api.Hosting.V1;

            var security = AppSettings.Security;

            var client = new TimelineClient(api, security);

            client.QueueCommand(command);
        }

        /// <remarks>
        /// Invokes CommandQueue.Send to execute the command. It is critically important that the
        /// method CommandQueue.Send can be invoked in one (and only one) place within the entire
        /// code base. If it is invoked in multiple places, then concurrency errors can occur. One
        /// reason for this is we cannot easily synchronize the execution of multiple commands for
        /// the same aggregate, originating in different application contexts. Additional software
        /// infrastructure would be required for this (e.g., RabbitMQ).
        /// 
        /// This method (ExecuteCommand) is invoked here the InSite.Persistence.ServiceLocator
        /// only if command-processing is disabled in the InSite API. Otherwise, it is invoked by
        /// the CommandController in the InSite API.
        /// </remarks>
        public static void ExecuteCommand(ICommand command)
        {
            CommandQueue.Send(command);
        }

        public static void SendCommands(IEnumerable<ICommand> commands)
        {
            foreach (var command in commands)
                SendCommand(command);
        }

        public static void SendCommands(IEnumerable<ICommand> commands, Action<int, ICommand> callback)
        {
            var number = 0;

            foreach (var command in commands)
            {
                SendCommand(command);
                callback(++number, command);
            }
        }

        public static void ScheduleCommand(ICommand command, DateTimeOffset at)
        {
            SetOrigin(command);

            CommandQueue.Schedule(command, at);
        }

        private static void SetOrigin(ICommand command)
        {
            if (command.OriginOrganization == Guid.Empty)
                command.OriginOrganization = IdentityService.GetCurrentOrganization();

            if (command.OriginUser == Guid.Empty)
            {
                command.OriginUser = IdentityService.GetCurrentUser();

                if (command.OriginUser == Guid.Empty)
                    command.OriginUser = UserIdentifiers.Someone;
            }
        }

        public static void EmitChanges(IEnumerable<AggregateImport> bundles)
        {
            ChangeStore.Save(bundles);
        }

        #endregion
    }
}
