CREATE SCHEMA [accounts]
GO
CREATE SCHEMA [achievements]
GO
CREATE SCHEMA [assessment]
GO
CREATE SCHEMA [assessments]
GO
CREATE SCHEMA [assets]
GO
CREATE SCHEMA [backups]
GO
CREATE SCHEMA [banks]
GO
CREATE SCHEMA [billing]
GO
CREATE SCHEMA [communications]
GO
CREATE SCHEMA [contacts]
GO
CREATE SCHEMA [contents]
GO
CREATE SCHEMA [courses]
GO
CREATE SCHEMA [custom_cmds]
GO
CREATE SCHEMA [custom_ita]
GO
CREATE SCHEMA [custom_ncsha]
GO
CREATE SCHEMA [database]
GO
CREATE SCHEMA [databases]
GO
CREATE SCHEMA [events]
GO
CREATE SCHEMA [identities]
GO
CREATE SCHEMA [integration]
GO
CREATE SCHEMA [invoices]
GO
CREATE SCHEMA [issues]
GO
CREATE SCHEMA [learning]
GO
CREATE SCHEMA [locations]
GO
CREATE SCHEMA [logs]
GO
CREATE SCHEMA [messages]
GO
CREATE SCHEMA [metadata]
GO
CREATE SCHEMA [payments]
GO
CREATE SCHEMA [plugin]
GO
CREATE SCHEMA [record]
GO
CREATE SCHEMA [records]
GO
CREATE SCHEMA [registrations]
GO
CREATE SCHEMA [reports]
GO
CREATE SCHEMA [resources]
GO
CREATE SCHEMA [security]
GO
CREATE SCHEMA [settings]
GO
CREATE SCHEMA [setup]
GO
CREATE SCHEMA [sites]
GO
CREATE SCHEMA [standard]
GO
CREATE SCHEMA [standards]
GO
CREATE SCHEMA [surveys]
GO
CREATE SCHEMA [utilities]
GO
CREATE SCHEMA [workflow]
GO
CREATE TYPE [dbo].[IdentifierList] AS TABLE(
	[IdentifierItem] [uniqueidentifier] NOT NULL
)
GO
CREATE TYPE [dbo].[IntegerList] AS TABLE(
	[IntegerItem] [int] NOT NULL
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [contents].[GetBodyHtmlEn]
(
    @json NVARCHAR(MAX)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    IF @json IS NULL
       OR ISJSON(@json) = 0
        RETURN @json;

    return json_value(@json, N'$.BodyHtml.en');
end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [contents].[GetBodyTextEn]
(
    @json nvarchar(max)
)
returns nvarchar(max)
as
begin
    if @json is null
       or isjson(@json) = 0
        return @json;

    return json_value(@json, N'$.BodyText.en');
end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [contents].[GetContentHtmlEn]( @id UNIQUEIDENTIFIER, @label VARCHAR(100) ) RETURNS NVARCHAR(MAX)
AS
    BEGIN
        RETURN (SELECT TOP 1 ContentHtml FROM contents.TContent WHERE ContentLanguage = 'en' AND ContentLabel = @label AND ContainerIdentifier = @id)
    END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [contents].[GetContentText]( @id uniqueidentifier, @label varchar(100), @language varchar(2)) returns nvarchar(max)
as
    begin
        return (select top 1 ContentText from contents.TContent where ContentLanguage = @language and ContentLabel = @label and ContainerIdentifier = @id)
    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [contents].[GetContentTextEn]( @id UNIQUEIDENTIFIER, @label VARCHAR(100) ) RETURNS NVARCHAR(MAX)
AS
    BEGIN
        RETURN (SELECT TOP 1 ContentText FROM contents.TContent WHERE ContentLanguage = 'en' AND ContentLabel = @label AND ContainerIdentifier = @id)
    END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [contents].[GetDescriptionEn]
(
    @json nvarchar(max)
)
returns nvarchar(max)
as
begin
    if @json is null
       or isjson(@json) = 0
        return @json;

    return json_value(@json, N'$.Description.en');
end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [contents].[GetSummaryEn]
(
    @json nvarchar(max)
)
returns nvarchar(max)
as
begin
    if @json is null
       or isjson(@json) = 0
        return @json;

    return json_value(@json, N'$.Summary.en');
end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [contents].[GetTitleEn]
(
    @json nvarchar(max)
)
returns nvarchar(max)
as
begin
    if @json is null
       or isjson(@json) = 0
        return @json;

    return json_value(@json, N'$.Title.en');
end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [contents].[IsContentContains](
    @id UNIQUEIDENTIFIER
   ,@label VARCHAR(100)
   ,@language VARCHAR(2)
   ,@keyword NVARCHAR(MAX)
) RETURNS BIT
AS
BEGIN
    DECLARE @SearchPattern nvarchar(max) = N'%' + @keyword + N'%';
    RETURN CASE WHEN EXISTS(SELECT TOP 1 1 FROM contents.TContent WHERE ContainerIdentifier = @id AND (ContentText LIKE @SearchPattern OR ContentHtml LIKE @SearchPattern) AND (@label IS NULL OR ContentLabel = @label) AND (@language IS NULL OR ContentLanguage = @language)) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [contents].[IsContentTranslated](
    @id UNIQUEIDENTIFIER
   ,@label VARCHAR(100)
) RETURNS BIT
AS
BEGIN
    RETURN CASE WHEN 1 < (SELECT COUNT(*) FROM contents.TContent WHERE ContainerIdentifier = @id AND (@label IS NULL OR ContentLabel = @label)) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [contents].[Translate]
(
    @Json nvarchar(max)
  , @Language nvarchar(max)
)
returns nvarchar(max)
as
begin
    if @Json is null
       or isjson(@Json) = 0
        return @Json;

    if @Language is null
       or @Language = 'en'
        return json_value(@Json, '$.en');

    declare @Result nvarchar(max) =
            (
                select top 1 Value from openjson(@Json)where [Key] = @Language
            );

    return case
               when @Result is not null then
                   @Result
               else
                   json_value(@Json, '$.en')
           end;
end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [custom_cmds].[GetUserCompanies]
(
    @UserIdentifier UNIQUEIDENTIFIER
)
RETURNS NVARCHAR(1024)
AS
BEGIN
    DECLARE
        @Result NVARCHAR(1024)
      , @Name   NVARCHAR(256);

    DECLARE person_cursor CURSOR FOR
        SELECT     DISTINCT
                    Organization.CompanyTitle AS CompanyName
        FROM
                    identities.Department AS g
        INNER JOIN contacts.Membership    AS m
                    ON m.GroupIdentifier = g.DepartmentIdentifier

        INNER JOIN accounts.QOrganization AS Organization
                    ON Organization.OrganizationIdentifier = g.OrganizationIdentifier
        WHERE
                    m.UserIdentifier = @UserIdentifier
        ORDER BY
                    CompanyName;

    OPEN person_cursor;

    FETCH NEXT FROM person_cursor
    INTO
        @Name;

    WHILE @@fetch_status = 0
        BEGIN
            IF @Result IS NULL
                SET @Result = @Name;
            ELSE
                SET @Result = @Result + N', ' + @Name;

            FETCH NEXT FROM person_cursor
            INTO
                @Name;
        END;

    CLOSE person_cursor;
    DEALLOCATE person_cursor;

    RETURN @Result;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [custom_cmds].[GetUserCompetencyDateExpired]
(
    @UserIdentifier               uniqueidentifier
  , @CompetencyStandardIdentifier uniqueidentifier
  , @DateCompleted         datetime
  , @DateExpired           datetime
)
returns datetime
    begin
        declare @LifetimeInMonths int =
                (
                    select
                        min(ValidForCount * (case when ValidForUnit = 'Years' then 12 else 1 end))
as                      Months
                    from
                        custom_cmds.DepartmentProfileCompetency
                        inner join
                        contacts.Membership on Membership.GroupIdentifier = DepartmentProfileCompetency.DepartmentIdentifier
                                 and Membership.UserIdentifier = @UserIdentifier
                        inner join
                        custom_cmds.UserCompetency on UserCompetency.CompetencyStandardIdentifier = DepartmentProfileCompetency.CompetencyStandardIdentifier
                                           and UserCompetency.UserIdentifier = @UserIdentifier
                        inner join
                        custom_cmds.UserProfile on UserProfile.DepartmentIdentifier = DepartmentProfileCompetency.DepartmentIdentifier
                                        and UserProfile.UserIdentifier = @UserIdentifier
                    where
                        ValidForCount is not null
                      and ValidForUnit is not null
                      and DepartmentProfileCompetency.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
                );

        if @DateExpired is not null return @DateExpired;

        return dateadd(month, @LifetimeInMonths, @DateCompleted);
    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [custom_cmds].[GetUserDepartments]
(
    @UserIdentifier   UNIQUEIDENTIFIER
  , @OrganizationIdentifier UNIQUEIDENTIFIER
)
returns nvarchar(1024)
as
    begin
        declare
            @Result       varchar(1024)
          , @Name         varchar(256)
          , @RoleType     varchar(100);

        declare person_cursor cursor for
            select
                DepartmentName
              , m.MembershipType
            from
                identities.Department as department
                inner join
                contacts.Membership as m on m.GroupIdentifier = department.DepartmentIdentifier
            where
                m.UserIdentifier              = @UserIdentifier
              and department.OrganizationIdentifier = @OrganizationIdentifier
            order by
                DepartmentName;

        open person_cursor;

        fetch next from person_cursor
        into
            @Name
          , @RoleType;

        while @@fetch_status = 0
            begin
                if @Result is null
                    set @Result = @Name + N' (' + case when @RoleType = 'Company' then 'C' when @RoleType = 'Department' then 'D' else 'A' end + N')';
                else
                    set @Result = @Result + N', ' + @Name + N' (' + case when @RoleType = 'Company' then 'C' when @RoleType = 'Department' then 'D' else 'A' end + N')';

                fetch next from person_cursor
                into
                    @Name
                  , @RoleType;
            end;

        close person_cursor;
        deallocate person_cursor;

        return @Result;
    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [database].[GetRandomHook]()
RETURNS CHAR(9)
AS
  BEGIN

    DECLARE @code CHAR(8);
    DECLARE @x VARCHAR(2048);

    -- Exclude vowels (to ensure the code never generates expletives). Also exclude look-alike characters (0,o,1,l) to improve readability.
    SET @x = 'BCDFGHJKMNPQRSTVWXYZ23456789';

    SELECT @code = ( SELECT TOP (8)
                            SUBSTRING(@x, 1 + number, 1) AS [text()]
                     FROM master..spt_values
                     WHERE number < DATALENGTH(@x)
                           AND type = 'P'
                     ORDER BY (SELECT NewIdentifier FROM [database].GetNewIdentifier) --instead of using newid()
                     FOR XML PATH(''));

    RETURN LEFT(@code,4) + '-' + RIGHT(@code,4);

  END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [settings].[ConvertToBit](@FieldValue NVARCHAR(MAX))
RETURNS BIT
BEGIN

	IF @FieldValue IN ('1','Y','Yes','True','T')
		RETURN 1;

	RETURN 0;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [settings].[IsBoolean](@FieldValue NVARCHAR(MAX))
RETURNS BIT
BEGIN

	IF @FieldValue IN ('0','1','Y','N','Yes','No','True','False','T','F')
		RETURN 1;

	RETURN 0;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [settings].[TruncateColumnValue](@TableName VARCHAR(100), @ColumnName VARCHAR(100), @ColumnValue VARCHAR(MAX)) RETURNS VARCHAR(MAX)
AS
BEGIN

	IF @ColumnValue IS NULL
		RETURN @ColumnValue;

	DECLARE @maxlen INT = (SELECT MAX(MaximumLength) FROM databases.VTableColumn WHERE TableName = @TableName AND ColumnName = @ColumnName);
	IF @maxlen > 0
		SET @ColumnValue = SUBSTRING(@ColumnValue,1,@maxlen);

	RETURN @ColumnValue;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [sites].[GetQPagePath]
(
    @PageIdentifier UNIQUEIDENTIFIER,
    @IncludeHost INT
)
RETURNS VARCHAR(MAX)
AS
BEGIN
    DECLARE @Path VARCHAR(MAX);

    WITH CTE
    AS (SELECT ParentPageIdentifier,
               CAST(PageSlug AS VARCHAR(MAX)) AS Path,
               '/' + CAST(PageIdentifier AS VARCHAR(MAX)) + '/' AS IdPath,
               0 AS Depth
        FROM sites.QPage
        WHERE QPage.PageIdentifier = @PageIdentifier
        UNION ALL
        SELECT p.ParentPageIdentifier,
               CAST(p.PageSlug + '/' + c.Path AS VARCHAR(MAX)),
               c.IdPath + CAST(p.PageIdentifier AS VARCHAR(MAX)) + '/',
               c.Depth + 1
        FROM sites.QPage AS p
            INNER JOIN CTE AS c
                ON c.ParentPageIdentifier = p.PageIdentifier
        WHERE c.IdPath NOT LIKE '%/' + CAST(p.PageIdentifier AS VARCHAR(MAX)) + '/%')
    SELECT TOP (1)
           @Path = (CASE
                        WHEN @IncludeHost = 1 THEN
                            Site.SiteDomain
                        ELSE
                            ''
                    END
                   ) + (CASE
                            WHEN Site.SiteIsPortal = 1 THEN
                                '/portals'
                            ELSE
                                ''
                        END
                       ) + '/' + CTE.Path
    FROM CTE
        OUTER APPLY
    (
        SELECT S.SiteDomain,
               CASE
                   WHEN LEN(S.SiteDomain) - LEN(REPLACE(S.SiteDomain, '.', '')) = 1 THEN
                       0
                   ELSE
                       1
               END AS SiteIsPortal
        FROM sites.QSite AS S
            INNER JOIN sites.QPage
                ON QPage.SiteIdentifier = S.SiteIdentifier
        WHERE QPage.PageIdentifier = @PageIdentifier
    ) AS Site
    ORDER BY CTE.Depth DESC;

    RETURN @Path;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [standards].[CalculateExpectedStandardDepth] (
    @ParentIdentifier UNIQUEIDENTIFIER
   ,@UpdateIdentifier UNIQUEIDENTIFIER
) RETURNS INT
AS
BEGIN
    DECLARE @DepthUpward INT;
    DECLARE @DepthDownward INT;

    IF @ParentIdentifier IS NOT NULL
      BEGIN
        WITH CTE_Upward AS (
            SELECT
                StandardIdentifier
               ,ParentStandardIdentifier
               ,CAST(1 AS INT) AS Depth
            FROM
                standards.[Standard]
            WHERE
                StandardIdentifier = @ParentIdentifier

            UNION ALL

            SELECT
                p.StandardIdentifier
               ,p.ParentStandardIdentifier
               ,c.Depth + 1
            FROM
                standards.[Standard] AS p
                INNER JOIN CTE_Upward AS c ON c.ParentStandardIdentifier = p.StandardIdentifier
        )
        SELECT TOP 1 @DepthUpward = Depth FROM CTE_Upward WHERE ParentStandardIdentifier IS NULL;
      END;

    IF @UpdateIdentifier IS NOT NULL
      BEGIN
        WITH CTE_Downward AS (
            SELECT
                StandardIdentifier
               ,ParentStandardIdentifier
               ,CAST(1 AS INT) AS Depth
            FROM
                standards.[Standard]
            WHERE
                StandardIdentifier = @UpdateIdentifier

            UNION ALL

            SELECT
                c.StandardIdentifier
               ,c.ParentStandardIdentifier
               ,p.Depth + 1
            FROM
                CTE_Downward AS P
                INNER JOIN standards.[Standard] AS c ON c.ParentStandardIdentifier = p.StandardIdentifier
        )
        SELECT @DepthDownward = MAX(Depth) FROM CTE_Downward;
      END;

    RETURN ISNULL(@DepthUpward,0) + ISNULL(@DepthDownward,0);
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [standards].[GetStandardRootKey] (
    @StandardIdentifier UNIQUEIDENTIFIER
) RETURNS UNIQUEIDENTIFIER
AS
BEGIN
    DECLARE @RootID UNIQUEIDENTIFIER;

    WITH CTE AS (
        SELECT
            StandardIdentifier
           ,ParentStandardIdentifier
        FROM
            standards.Standard
        WHERE
            StandardIdentifier = @StandardIdentifier

        UNION ALL

        SELECT
            p.StandardIdentifier
           ,p.ParentStandardIdentifier
        FROM
            standards.Standard AS p
            INNER JOIN CTE AS c ON c.ParentStandardIdentifier = p.StandardIdentifier
    )
    SELECT TOP 1 @RootID = StandardIdentifier FROM CTE WHERE ParentStandardIdentifier IS NULL

    RETURN @RootID;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [registrations].[QRegistration](
	[EventIdentifier] [uniqueidentifier] NOT NULL,
	[EventPotentialConflicts] [varchar](max) NULL,
	[ApprovalProcess] [varchar](max) NULL,
	[ApprovalReason] [varchar](max) NULL,
	[ApprovalStatus] [varchar](30) NULL,
	[AttemptIdentifier] [uniqueidentifier] NULL,
	[AttendanceStatus] [varchar](20) NULL,
	[BillingCustomer] [varchar](128) NULL,
	[CandidateIdentifier] [uniqueidentifier] NOT NULL,
	[CustomerIdentifier] [uniqueidentifier] NULL,
	[DistributionExpected] [datetimeoffset](7) NULL,
	[EligibilityProcess] [varchar](max) NULL,
	[EligibilityStatus] [varchar](30) NULL,
	[EmployerIdentifier] [uniqueidentifier] NULL,
	[ExamFormIdentifier] [uniqueidentifier] NULL,
	[ExamTimeLimit] [int] NULL,
	[Grade] [varchar](30) NULL,
	[GradeAssigned] [datetimeoffset](7) NULL,
	[GradePublished] [datetimeoffset](7) NULL,
	[GradeReleased] [datetimeoffset](7) NULL,
	[GradeWithheld] [datetimeoffset](7) NULL,
	[GradeWithheldReason] [varchar](200) NULL,
	[GradingProcess] [varchar](128) NULL,
	[GradingStatus] [varchar](100) NULL,
	[LastChangeTime] [datetimeoffset](7) NULL,
	[LastChangeType] [varchar](100) NULL,
	[LastChangeUser] [varchar](100) NULL,
	[MaterialsIncludeDiagramBook] [bit] NULL,
	[MaterialsPackagedForDistribution] [varchar](max) NULL,
	[MaterialsPermittedToCandidates] [varchar](max) NULL,
	[RegistrationComment] [varchar](max) NULL,
	[RegistrationFee] [decimal](11, 2) NULL,
	[RegistrationIdentifier] [uniqueidentifier] NOT NULL,
	[RegistrationPassword] [varchar](14) NOT NULL,
	[RegistrationRequestedOn] [datetimeoffset](7) NULL,
	[RegistrationSequence] [int] NULL,
	[RegistrationSource] [varchar](max) NULL,
	[SchoolIdentifier] [uniqueidentifier] NULL,
	[Score] [decimal](5, 4) NULL,
	[SeatIdentifier] [uniqueidentifier] NULL,
	[SynchronizationProcess] [varchar](max) NULL,
	[SynchronizationStatus] [varchar](40) NULL,
	[WorkBasedHoursToDate] [int] NULL,
	[IncludeInT2202] [bit] NOT NULL,
	[PaymentIdentifier] [uniqueidentifier] NULL,
	[CandidateType] [varchar](100) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[RegistrationRequestedBy] [uniqueidentifier] NULL,
	[AttendanceTaken] [datetimeoffset](7) NULL,
	[EligibilityUpdated] [datetimeoffset](7) NULL,
	[BillingCode] [varchar](100) NULL,
 CONSTRAINT [PK_QRegistration] PRIMARY KEY CLUSTERED 
(
	[RegistrationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [events].[QEvent](
	[EventBillingType] [varchar](30) NULL,
	[EventClassCode] [varchar](30) NULL,
	[ExamDurationInMinutes] [int] NULL,
	[EventFormat] [varchar](10) NULL,
	[EventIdentifier] [uniqueidentifier] NOT NULL,
	[EventNumber] [int] NOT NULL,
	[EventSchedulingStatus] [varchar](30) NULL,
	[EventTitle] [varchar](400) NULL,
	[EventType] [varchar](40) NULL,
	[DistributionCode] [varchar](100) NULL,
	[DistributionErrors] [varchar](max) NULL,
	[DistributionExpected] [datetimeoffset](7) NULL,
	[DistributionProcess] [varchar](40) NULL,
	[DistributionOrdered] [datetimeoffset](7) NULL,
	[DistributionShipped] [datetimeoffset](7) NULL,
	[DistributionStatus] [varchar](100) NULL,
	[DistributionTracked] [datetimeoffset](7) NULL,
	[ExamStarted] [datetimeoffset](7) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[VenueLocationIdentifier] [uniqueidentifier] NULL,
	[VenueRoom] [varchar](200) NULL,
	[PublicationErrors] [varchar](max) NULL,
	[VenueCoordinatorIdentifier] [uniqueidentifier] NULL,
	[EventDescription] [varchar](max) NULL,
	[LastChangeTime] [datetimeoffset](7) NULL,
	[LastChangeType] [varchar](100) NULL,
	[LastChangeUser] [varchar](100) NULL,
	[EventScheduledStart] [datetimeoffset](7) NOT NULL,
	[EventScheduledEnd] [datetimeoffset](7) NULL,
	[DurationQuantity] [int] NULL,
	[DurationUnit] [varchar](10) NULL,
	[EventSource] [varchar](max) NULL,
	[CreditHours] [decimal](5, 2) NULL,
	[CapacityMinimum] [int] NULL,
	[CapacityMaximum] [int] NULL,
	[ExamType] [varchar](40) NULL,
	[AchievementIdentifier] [uniqueidentifier] NULL,
	[RegistrationDeadline] [datetimeoffset](7) NULL,
	[EventSummary] [varchar](max) NULL,
	[EventPublicationStatus] [varchar](50) NULL,
	[Content] [nvarchar](max) NULL,
	[WaitlistEnabled] [bit] NOT NULL,
	[RegistrationStart] [datetimeoffset](7) NULL,
	[EventRequisitionStatus] [varchar](50) NULL,
	[InvigilatorMinimum] [int] NULL,
	[ExamMaterialReturnShipmentCode] [varchar](30) NULL,
	[ExamMaterialReturnShipmentReceived] [datetimeoffset](7) NULL,
	[ExamMaterialReturnShipmentCondition] [varchar](10) NULL,
	[IntegrationWithholdGrades] [bit] NOT NULL,
	[IntegrationWithholdDistribution] [bit] NOT NULL,
	[VenueOfficeIdentifier] [uniqueidentifier] NULL,
	[AppointmentType] [varchar](40) NULL,
	[RegistrationLocked] [datetimeoffset](7) NULL,
	[AllowRegistrationWithLink] [bit] NULL,
	[LearnerRegistrationGroupIdentifier] [uniqueidentifier] NULL,
	[PersonCodeIsRequired] [bit] NOT NULL,
	[AllowMultipleRegistrations] [bit] NOT NULL,
	[EventCalendarColor] [varchar](7) NULL,
	[RegistrationFields] [varchar](max) NULL,
	[MandatorySurveyFormIdentifier] [uniqueidentifier] NULL,
	[BillingCodeEnabled] [bit] NOT NULL,
	[WhenEventReminderRequestedNotifyLearnerMessageIdentifier] [uniqueidentifier] NULL,
	[WhenEventReminderRequestedNotifyInstructorMessageIdentifier] [uniqueidentifier] NULL,
	[ReminderMessageSent] [datetimeoffset](7) NULL,
	[SendReminderBeforeDays] [int] NULL,
 CONSTRAINT [PK_QEvent] PRIMARY KEY CLUSTERED 
(
	[EventIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [identities].[QUser](
	[DefaultPassword] [varchar](100) NULL,
	[DefaultPasswordExpired] [datetimeoffset](7) NULL,
	[Email] [varchar](254) NOT NULL,
	[EmailVerified] [varchar](254) NULL,
	[FirstName] [varchar](40) NOT NULL,
	[Honorific] [varchar](32) NULL,
	[ImageUrl] [varchar](500) NULL,
	[Initials] [varchar](32) NULL,
	[AccessGrantedToCmds] [bit] NOT NULL,
	[LastName] [varchar](40) NOT NULL,
	[MiddleName] [varchar](38) NULL,
	[FullName] [varchar](120) NOT NULL,
	[SoundexFirstName] [varchar](4) NULL,
	[SoundexLastName] [varchar](4) NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[TimeZone] [varchar](40) NOT NULL,
	[UserLicenseAccepted] [datetimeoffset](7) NULL,
	[UserPasswordChanged] [datetimeoffset](7) NULL,
	[UserPasswordExpired] [datetimeoffset](7) NOT NULL,
	[UserPasswordHash] [varchar](70) NOT NULL,
	[UtcArchived] [datetimeoffset](7) NULL,
	[UtcUnarchived] [datetimeoffset](7) NULL,
	[MultiFactorAuthentication] [bit] NOT NULL,
	[MultiFactorAuthenticationCode] [varchar](6) NULL,
	[EmailAlternate] [varchar](254) NULL,
	[PhoneMobile] [varchar](32) NULL,
	[AccountCloaked] [datetimeoffset](7) NULL,
	[PrimaryLoginMethod] [int] NULL,
	[SecondaryLoginMethod] [int] NULL,
	[OAuthProviderUserId] [varchar](max) NULL,
	[LoginOrganizationCode] [varchar](30) NULL,
	[OldUserPasswordHash] [varchar](70) NULL,
	[UserPasswordChangeRequested] [int] NULL,
 CONSTRAINT [PK_QUser] PRIMARY KEY CLUSTERED 
(
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create view [identities].[User]
as
select
    DefaultPassword
    ,DefaultPasswordExpired
    ,Email
    ,EmailVerified
    ,FirstName
    ,Honorific
    ,ImageUrl
    ,Initials
    ,AccessGrantedToCmds
    ,LastName
    ,MiddleName
    ,FullName
    ,SoundexFirstName
    ,SoundexLastName
    ,UserIdentifier
    ,TimeZone
    ,UserLicenseAccepted
    ,UserPasswordChanged
    ,UserPasswordExpired
    ,UserPasswordHash
    ,UtcArchived
    ,UtcUnarchived
    ,MultiFactorAuthentication
    ,MultiFactorAuthenticationCode
    ,EmailAlternate
    ,PhoneMobile
    ,AccountCloaked
    ,PrimaryLoginMethod
    ,SecondaryLoginMethod
    ,OAuthProviderUserId
    ,LoginOrganizationCode
    ,OldUserPasswordHash
    ,UserPasswordChangeRequested
from
    identities.QUser
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QPerson](
	[EmployeeUnion] [varchar](32) NULL,
	[IsAdministrator] [bit] NOT NULL,
	[SocialInsuranceNumber] [varchar](32) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[PersonCode] [varchar](20) NULL,
	[IsLearner] [bit] NOT NULL,
	[Birthdate] [date] NULL,
	[Citizenship] [varchar](100) NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CustomKey] [int] NULL,
	[EducationLevel] [varchar](80) NULL,
	[Gender] [varchar](20) NULL,
	[ImmigrationApplicant] [varchar](20) NULL,
	[ImmigrationCategory] [varchar](120) NULL,
	[ImmigrationDestination] [varchar](100) NULL,
	[ImmigrationLandingDate] [date] NULL,
	[ImmigrationNumber] [varchar](64) NULL,
	[JobTitle] [varchar](256) NULL,
	[Language] [varchar](2) NULL,
	[MemberEndDate] [date] NULL,
	[MemberStartDate] [date] NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[Phone] [varchar](30) NULL,
	[PhoneFax] [varchar](32) NULL,
	[PhoneHome] [varchar](32) NULL,
	[PhoneOther] [varchar](32) NULL,
	[PhoneWork] [varchar](32) NULL,
	[Referrer] [varchar](100) NULL,
	[ReferrerOther] [varchar](200) NULL,
	[Region] [varchar](50) NULL,
	[CredentialingCountry] [varchar](100) NULL,
	[ShippingPreference] [varchar](20) NULL,
	[TradeworkerNumber] [varchar](20) NULL,
	[WebSiteUrl] [varchar](500) NULL,
	[WelcomeEmailsSentToUser] [int] NULL,
	[EmergencyContactName] [varchar](100) NULL,
	[EmergencyContactPhone] [varchar](32) NULL,
	[EmergencyContactRelationship] [varchar](50) NULL,
	[FirstLanguage] [varchar](20) NULL,
	[EmployerGroupIdentifier] [uniqueidentifier] NULL,
	[JobsApproved] [datetimeoffset](7) NULL,
	[JobsApprovedBy] [varchar](128) NULL,
	[ImmigrationDisability] [varchar](20) NULL,
	[PersonIdentifier] [uniqueidentifier] NOT NULL,
	[AccessRevoked] [datetimeoffset](7) NULL,
	[AccessRevokedBy] [varchar](254) NULL,
	[UserAccessGranted] [datetimeoffset](7) NULL,
	[UserAccessGrantedBy] [varchar](128) NULL,
	[UserApproveReason] [varchar](200) NULL,
	[ConsentConsultation] [varchar](30) NULL,
	[CandidateCompletionProfilePercent] [int] NULL,
	[CandidateCompletionResumePercent] [int] NULL,
	[CandidateIsActivelySeeking] [bit] NULL,
	[CandidateIsWillingToRelocate] [bit] NULL,
	[CandidateLinkedInUrl] [varchar](500) NULL,
	[CandidateOccupationList] [varchar](max) NULL,
	[IndustryItemIdentifier] [uniqueidentifier] NULL,
	[OccupationStandardIdentifier] [uniqueidentifier] NULL,
	[AccountReviewQueued] [datetimeoffset](7) NULL,
	[AccountReviewCompleted] [datetimeoffset](7) NULL,
	[ConsentToShare] [varchar](30) NULL,
	[EmailEnabled] [bit] NOT NULL,
	[EmailAlternateEnabled] [bit] NOT NULL,
	[MembershipStatusItemIdentifier] [uniqueidentifier] NULL,
	[FullName] [varchar](120) NULL,
	[LastAuthenticated] [datetimeoffset](7) NULL,
	[IsOperator] [bit] NOT NULL,
	[BillingAddressIdentifier] [uniqueidentifier] NULL,
	[HomeAddressIdentifier] [uniqueidentifier] NULL,
	[ShippingAddressIdentifier] [uniqueidentifier] NULL,
	[WorkAddressIdentifier] [uniqueidentifier] NULL,
	[MarketingEmailEnabled] [bit] NOT NULL,
	[IsArchived] [bit] NOT NULL,
	[WhenArchived] [datetimeoffset](7) NULL,
	[WhenUnarchived] [datetimeoffset](7) NULL,
	[EmployeeType] [varchar](16) NULL,
	[IsDeveloper] [bit] NOT NULL,
	[JobDivision] [varchar](100) NULL,
	[PersonType] [varchar](20) NULL,
	[SinModified] [datetimeoffset](7) NULL,
	[AgeGroup] [varchar](20) NULL,
 CONSTRAINT [PK_QPerson] PRIMARY KEY CLUSTERED 
(
	[PersonIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_QPerson] UNIQUE NONCLUSTERED 
(
	[UserIdentifier] ASC,
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [contacts].[Person] AS
    SELECT AccessRevoked,
           AccessRevokedBy,
           AccountReviewCompleted,
           AccountReviewQueued,
           AgeGroup,
           BillingAddressIdentifier,
           Birthdate,
           CandidateCompletionProfilePercent,
           CandidateCompletionResumePercent,
           CandidateIsActivelySeeking,
           CandidateIsWillingToRelocate,
           CandidateLinkedInUrl,
           CandidateOccupationList,
           Citizenship,
           ConsentConsultation,
           ConsentToShare,
           Created,
           CreatedBy,
           CredentialingCountry,
           CustomKey,
           EducationLevel,
           EmailAlternateEnabled,
           EmailEnabled,
           EmergencyContactName,
           EmergencyContactPhone,
           EmergencyContactRelationship,
           EmployeeType,
           EmployeeUnion,
           EmployerGroupIdentifier,
           FirstLanguage,
           FullName,
           Gender,
           HomeAddressIdentifier,
           ImmigrationApplicant,
           ImmigrationCategory,
           ImmigrationDestination,
           ImmigrationDisability,
           ImmigrationLandingDate,
           ImmigrationNumber,
           IndustryItemIdentifier,
           IsAdministrator,
           IsArchived,
           IsLearner,
           IsOperator,
           JobsApproved,
           JobsApprovedBy,
           JobDivision,
           JobTitle,
           Language,
           LastAuthenticated,
           MarketingEmailEnabled,
           MemberEndDate,
           MembershipStatusItemIdentifier,
           MemberStartDate,
           Modified,
           ModifiedBy,
           OccupationStandardIdentifier,
           OrganizationIdentifier,
           PersonCode,
           PersonIdentifier,
           PersonType,
           Phone,
           PhoneFax,
           PhoneHome,
           PhoneOther,
           PhoneWork,
           Referrer,
           ReferrerOther,
           Region,
           ShippingAddressIdentifier,
           ShippingPreference,
           SocialInsuranceNumber,
           TradeworkerNumber,
           UserAccessGranted,
           UserAccessGrantedBy,
           UserApproveReason,
           UserIdentifier,
           WebSiteUrl,
           WelcomeEmailsSentToUser,
           WhenArchived,
           WhenUnarchived,
           WorkAddressIdentifier
    FROM contacts.QPerson;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [banks].[QBankForm](
	[BankIdentifier] [uniqueidentifier] NOT NULL,
	[FieldCount] [int] NOT NULL,
	[FormIdentifier] [uniqueidentifier] NOT NULL,
	[FormAsset] [int] NOT NULL,
	[FormName] [varchar](200) NOT NULL,
	[FormPublicationStatus] [varchar](50) NULL,
	[FormTitle] [varchar](200) NULL,
	[FormType] [varchar](20) NULL,
	[FormVersion] [varchar](20) NULL,
	[SpecQuestionLimit] [int] NOT NULL,
	[SectionCount] [int] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[SpecIdentifier] [uniqueidentifier] NOT NULL,
	[AttemptStartedCount] [int] NOT NULL,
	[AttemptPassedCount] [int] NOT NULL,
	[FormCode] [varchar](40) NULL,
	[FormTimeLimit] [int] NULL,
	[FormPassingScore] [decimal](3, 2) NULL,
	[BankLevelType] [varchar](20) NULL,
	[FormSource] [varchar](80) NULL,
	[FormAssetVersion] [int] NOT NULL,
	[FormFirstPublished] [datetimeoffset](7) NULL,
	[FormOrigin] [varchar](100) NULL,
	[FormHook] [varchar](100) NULL,
	[FormSummary] [nvarchar](max) NULL,
	[FormIntroduction] [nvarchar](max) NULL,
	[FormMaterialsForParticipation] [nvarchar](max) NULL,
	[FormMaterialsForDistribution] [nvarchar](max) NULL,
	[FormInstructionsForOnline] [nvarchar](max) NULL,
	[FormInstructionsForPaper] [nvarchar](max) NULL,
	[FormHasDiagrams] [bit] NOT NULL,
	[FormHasReferenceMaterials] [varchar](21) NULL,
	[FormAttemptLimit] [int] NOT NULL,
	[AttemptSubmittedCount] [int] NOT NULL,
	[AttemptGradedCount] [int] NOT NULL,
	[FormThirdPartyAssessmentIsEnabled] [bit] NOT NULL,
	[FormClassificationInstrument] [varchar](100) NULL,
	[GradebookIdentifier] [uniqueidentifier] NULL,
	[WhenAttemptStartedNotifyAdminMessageIdentifier] [uniqueidentifier] NULL,
	[WhenAttemptCompletedNotifyAdminMessageIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QBankForm] PRIMARY KEY CLUSTERED 
(
	[FormIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [registrations].[VAttendance]
AS
SELECT  e.EventFormat,
		e.EventIdentifier,
		e.EventNumber,
		e.EventScheduledStart,

		e.OrganizationIdentifier,

		f.FormCode AS AssessmentFormCode,
		f.FormIdentifier AS AssessmentFormIdentifier,
		f.FormName AS AssessmentFormName,
		f.FormTitle AS AssessmentFormTitle,

		r.AttendanceStatus,
		r.LastChangeTime,
		r.LastChangeType,
		r.RegistrationIdentifier,

		p.PersonCode AS LearnerCode,
		u.Email AS LearnerEmail,
		u.FullName AS LearnerName,
		u.UserIdentifier AS LearnerIdentifier

FROM registrations.QRegistration AS r
    INNER JOIN events.QEvent AS e ON e.EventIdentifier = r.EventIdentifier
    INNER JOIN identities.[User] AS u ON r.CandidateIdentifier = u.UserIdentifier
    INNER JOIN contacts.Person AS p ON p.UserIdentifier = u.UserIdentifier AND p.OrganizationIdentifier = e.OrganizationIdentifier
    INNER JOIN banks.QBankForm AS f ON f.FormIdentifier = r.ExamFormIdentifier

WHERE r.AttendanceStatus IS NOT NULL;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [registrations].[QRegistrationTimer](
	[RegistrationIdentifier] [uniqueidentifier] NOT NULL,
	[TimerDescription] [varchar](200) NULL,
	[TimerStatus] [varchar](20) NOT NULL,
	[TriggerCommand] [uniqueidentifier] NOT NULL,
	[TriggerTime] [datetimeoffset](7) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QRegistrationTimer] PRIMARY KEY CLUSTERED 
(
	[TriggerCommand] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [registrations].[XRegistrationTimer]
AS
SELECT
    R.EventIdentifier
  , T.RegistrationIdentifier
  , T.TimerDescription
  , T.TimerStatus
  , T.TriggerCommand
  , T.TriggerTime
FROM
    registrations.QRegistrationTimer AS T
    LEFT JOIN
    registrations.QRegistration      AS R ON R.RegistrationIdentifier = T.RegistrationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QGroup](
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ParentGroupIdentifier] [uniqueidentifier] NULL,
	[SurveyFormIdentifier] [uniqueidentifier] NULL,
	[MessageToUserWhenMembershipStarted] [uniqueidentifier] NULL,
	[MessageToAdminWhenMembershipStarted] [uniqueidentifier] NULL,
	[MessageToAdminWhenEventVenueChanged] [uniqueidentifier] NULL,
	[AddNewUsersAutomatically] [bit] NOT NULL,
	[AllowSelfSubscription] [bit] NOT NULL,
	[GroupCapacity] [int] NULL,
	[GroupCategory] [varchar](120) NULL,
	[GroupCode] [varchar](32) NULL,
	[GroupCreated] [datetimeoffset](7) NOT NULL,
	[GroupType] [varchar](32) NOT NULL,
	[GroupDescription] [varchar](max) NULL,
	[GroupFax] [varchar](32) NULL,
	[GroupImage] [varchar](100) NULL,
	[GroupIndustry] [varchar](100) NULL,
	[GroupIndustryComment] [varchar](100) NULL,
	[GroupLabel] [varchar](100) NULL,
	[GroupName] [varchar](90) NOT NULL,
	[GroupOffice] [varchar](800) NULL,
	[GroupPhone] [varchar](32) NULL,
	[GroupRegion] [varchar](30) NULL,
	[GroupSize] [varchar](100) NULL,
	[GroupWebSiteUrl] [varchar](500) NULL,
	[ShippingPreference] [varchar](20) NULL,
	[SurveyNecessity] [varchar](50) NULL,
	[LastChangeTime] [datetimeoffset](7) NOT NULL,
	[LastChangeType] [varchar](100) NOT NULL,
	[LastChangeUser] [varchar](100) NOT NULL,
	[GroupEmail] [varchar](254) NULL,
	[SocialMediaUrls] [varchar](max) NULL,
	[GroupExpiry] [datetimeoffset](7) NULL,
	[LifetimeUnit] [varchar](6) NULL,
	[LifetimeQuantity] [int] NULL,
	[MessageToAdminWhenMembershipEnded] [uniqueidentifier] NULL,
	[MessageToUserWhenMembershipEnded] [uniqueidentifier] NULL,
	[MembershipProductIdentifier] [uniqueidentifier] NULL,
	[AllowJoinGroupUsingLink] [bit] NOT NULL,
	[GroupStatusItemIdentifier] [uniqueidentifier] NULL,
	[OnlyOperatorCanAddUser] [bit] NOT NULL,
 CONSTRAINT [PK_QGroup] PRIMARY KEY CLUSTERED 
(
	[GroupIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QGroupAddress](
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[AddressType] [varchar](50) NOT NULL,
	[City] [varchar](128) NULL,
	[Country] [varchar](32) NULL,
	[Description] [varchar](128) NULL,
	[PostalCode] [varchar](20) NULL,
	[Province] [varchar](64) NULL,
	[Street1] [varchar](200) NULL,
	[Street2] [varchar](200) NULL,
	[Latitude] [decimal](6, 3) NULL,
	[Longitude] [decimal](6, 3) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[AddressIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QGroupAddress] PRIMARY KEY CLUSTERED 
(
	[AddressIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [utilities].[TCollectionItem](
	[ItemColor] [varchar](7) NULL,
	[ItemDescription] [varchar](800) NULL,
	[ItemFolder] [varchar](50) NULL,
	[ItemHours] [decimal](5, 2) NULL,
	[ItemIcon] [varchar](32) NULL,
	[ItemIsDisabled] [bit] NOT NULL,
	[ItemNumber] [int] IDENTITY(1,1) NOT NULL,
	[ItemName] [varchar](200) NOT NULL,
	[ItemNameTranslation] [varchar](max) NULL,
	[ItemSequence] [int] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[ItemIdentifier] [uniqueidentifier] NOT NULL,
	[GroupIdentifier] [uniqueidentifier] NULL,
	[CollectionIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TCollectionItem] PRIMARY KEY CLUSTERED 
(
	[ItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QPersonAddress](
	[AddressIdentifier] [uniqueidentifier] NOT NULL,
	[City] [varchar](128) NULL,
	[Country] [varchar](32) NULL,
	[Description] [varchar](128) NULL,
	[PostalCode] [varchar](20) NULL,
	[Province] [varchar](64) NULL,
	[Street1] [varchar](200) NULL,
	[Street2] [varchar](200) NULL,
 CONSTRAINT [PK_QPersonAddress] PRIMARY KEY CLUSTERED 
(
	[AddressIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create view [locations].[Address]
as
select
    City
    ,Country
    ,Description
    ,PostalCode
    ,Province
    ,Street1
    ,Street2
    ,AddressIdentifier
from
    contacts.QPersonAddress
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [reports].[Employee]
AS
    SELECT
        Employee.UserIdentifier            AS EmployeeUserIdentifier
      , Employee.FullName                  AS EmployeeFullName
      , Employee.FirstName                 AS EmployeeFirstName
      , Employee.LastName                  AS EmployeeLastName
      , Employee.Email                     AS EmployeeEmail
      , P.JobTitle                         AS EmployeeJobTitle
      , MembershipStatus.ItemName          AS EmployeeProcessStep
      , P.Gender                           AS EmployeeGender
      , MembershipStatus.ItemName          AS EmployeeContactLabel
      , CASE
            WHEN MembershipStatus.ItemName = 'Active Member'
                THEN
                'AM'
            WHEN MembershipStatus.ItemName = 'Apply for AM'
                THEN
                'AP'
            WHEN MembershipStatus.ItemName = 'Non-member'
                THEN
                'NM'
            ELSE
                MembershipStatus.ItemName
        END                                AS EmployeeContactType
      , Employer.OrganizationIdentifier          AS EmployerOrganizationIdentifier
      , Employer.GroupIdentifier           AS EmployerGroupIdentifier
      , Employer.GroupName                 AS EmployerGroupName
      , Employer.GroupCode                 AS EmployerGroupNumber
      , Employer.GroupCategory             AS EmployerContactLabel
      , EmployerDistrict.GroupIdentifier   AS EmployerDistrictIdentifier
      , EmployerDistrict.GroupName         AS EmployerDistrictName
      , EmployerDistrict.GroupRegion       AS EmployerDistrictRegion
      , EmployerShippingAddress.Street1    AS EmployerShippingAddressStreet1
      , EmployerShippingAddress.Street2    AS EmployerShippingAddressStreet2
      , EmployerShippingAddress.City       AS EmployerShippingAddressCity
      , EmployerShippingAddress.Province   AS EmployerShippingAddressProvince
      , EmployerShippingAddress.Country    AS EmployerShippingAddressCountry
      , EmployerShippingAddress.PostalCode AS EmployerShippingAddressPostalCode
      , Employer.GroupPhone                AS EmployerPhone
      , Employer.GroupFax                  AS EmployerPhoneFax
      , Employee.Honorific                 AS EmployeeHonorific
      , P.Phone                            AS EmployeePhone
      , P.PhoneHome                        AS EmployeePhoneHome
      , Employee.PhoneMobile               AS EmployeePhoneMobile
      , P.MemberStartDate                  AS EmployeeMemberStartDate
      , P.MemberEndDate                    AS EmployeeMemberEndDate
      , EmployeeShippingAddress.Street1    AS EmployeeShippingAddressStreet1
      , EmployeeShippingAddress.City       AS EmployeeShippingAddressCity
      , EmployeeShippingAddress.Province   AS EmployeeShippingAddressProvince
      , EmployeeShippingAddress.PostalCode AS EmployeeShippingAddressPostalCode
      , EmployeeShippingAddress.Country    AS EmployeeShippingAddressCountry
      , P.ShippingPreference               AS EmployeeShippingPreference
      , P.PersonCode                       AS EmployeeAccountNumber
      , EmployeeHomeAddress.Street1        AS EmployeeHomeAddressStreet1
      , EmployeeHomeAddress.City           AS EmployeeHomeAddressCity
      , EmployeeHomeAddress.Province       AS EmployeeHomeAddressProvince
      , EmployeeHomeAddress.PostalCode     AS EmployeeHomeAddressPostalCode
      , EmployeeHomeAddress.Country        AS EmployeeHomeAddressCountry
      , P.OrganizationIdentifier                 AS EmployeeOrganizationIdentifier
    FROM
        identities.[User]               AS Employee
    INNER JOIN contacts.Person          AS P
               ON P.UserIdentifier = Employee.UserIdentifier

    LEFT JOIN locations.Address         AS EmployeeShippingAddress
              ON EmployeeShippingAddress.AddressIdentifier = P.ShippingAddressIdentifier

    LEFT JOIN locations.Address         AS EmployeeHomeAddress
              ON EmployeeHomeAddress.AddressIdentifier = P.HomeAddressIdentifier

    LEFT JOIN contacts.QGroup          AS Employer
              ON P.EmployerGroupIdentifier = Employer.GroupIdentifier

    LEFT JOIN contacts.QGroup          AS EmployerDistrict
              ON Employer.ParentGroupIdentifier = EmployerDistrict.GroupIdentifier

    LEFT JOIN contacts.QGroupAddress         AS EmployerShippingAddress
              ON EmployerShippingAddress.GroupIdentifier = Employer.GroupIdentifier AND EmployerShippingAddress.AddressType = 'Shipping'

    LEFT JOIN utilities.TCollectionItem AS MembershipStatus
              ON P.MembershipStatusItemIdentifier = MembershipStatus.ItemIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contents].[TContent](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ContainerIdentifier] [uniqueidentifier] NOT NULL,
	[ContentLabel] [varchar](100) NOT NULL,
	[ContentLanguage] [varchar](2) NOT NULL,
	[ContentSnip] [nvarchar](100) NOT NULL,
	[ContentText] [nvarchar](max) NULL,
	[ContentHtml] [nvarchar](max) NULL,
	[ContainerType] [varchar](100) NULL,
	[ContentSequence] [int] NULL,
	[ContentIdentifier] [uniqueidentifier] NOT NULL,
	[ReferenceFiles] [varchar](max) NULL,
	[ReferenceCount] [int] NULL,
 CONSTRAINT [PK_TContent] PRIMARY KEY CLUSTERED 
(
	[ContentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [surveys].[QSurveyQuestion](
	[SurveyQuestionIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyFormIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyQuestionType] [varchar](20) NOT NULL,
	[SurveyQuestionCode] [varchar](4) NULL,
	[SurveyQuestionIndicator] [varchar](10) NULL,
	[SurveyQuestionSequence] [int] NOT NULL,
	[SurveyQuestionIsRequired] [bit] NOT NULL,
	[SurveyQuestionListEnableBranch] [bit] NOT NULL,
	[SurveyQuestionListEnableOtherText] [bit] NOT NULL,
	[SurveyQuestionListEnableRandomization] [bit] NOT NULL,
	[SurveyQuestionNumberEnableStatistics] [bit] NOT NULL,
	[SurveyQuestionTextCharacterLimit] [int] NULL,
	[SurveyQuestionTextLineCount] [int] NULL,
	[SurveyQuestionIsNested] [bit] NOT NULL,
	[SurveyQuestionSource] [varchar](100) NULL,
	[SurveyQuestionAttribute] [varchar](100) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[SurveyQuestionListEnableGroupMembership] [bit] NOT NULL,
 CONSTRAINT [PK_QSurveyQuestion] PRIMARY KEY CLUSTERED 
(
	[SurveyQuestionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [surveys].[QSurveyOptionList](
	[SurveyOptionListIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyOptionListSequence] [int] NOT NULL,
	[SurveyQuestionIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QSurveyOptionList] PRIMARY KEY CLUSTERED 
(
	[SurveyOptionListIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [surveys].[QSurveyOptionItem](
	[SurveyOptionListIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyOptionItemIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyOptionItemSequence] [int] NOT NULL,
	[BranchToQuestionIdentifier] [uniqueidentifier] NULL,
	[SurveyOptionItemCategory] [varchar](120) NULL,
	[SurveyOptionItemPoints] [decimal](7, 2) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QSurveyOptionItem] PRIMARY KEY CLUSTERED 
(
	[SurveyOptionItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [surveys].[QResponseOption](
	[ResponseSessionIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyOptionIdentifier] [uniqueidentifier] NOT NULL,
	[ResponseOptionIsSelected] [bit] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[SurveyQuestionIdentifier] [uniqueidentifier] NOT NULL,
	[OptionSequence] [int] NOT NULL,
 CONSTRAINT [PK_QResponseOption] PRIMARY KEY CLUSTERED 
(
	[ResponseSessionIdentifier] ASC,
	[SurveyOptionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [surveys].[VResponseFirstSelection]
AS
SELECT Q.SurveyFormIdentifier      AS SurveyIdentifier,
       Q.SurveyQuestionSequence    AS QuestionSequence,
       Q.SurveyQuestionType        AS QuestionType,
       O.ResponseSessionIdentifier AS ResponseIdentifier,
       C.ContentText               AS AnswerText
FROM surveys.QSurveyQuestion             AS Q
    INNER JOIN surveys.QSurveyOptionList AS L
        ON L.SurveyQuestionIdentifier = Q.SurveyQuestionIdentifier
    INNER JOIN surveys.QSurveyOptionItem AS I
        ON I.SurveyOptionListIdentifier = L.SurveyOptionListIdentifier
    INNER JOIN contents.TContent         AS C
        ON C.ContainerIdentifier = I.SurveyOptionItemIdentifier
           AND C.ContentLabel = 'Title'
           AND C.ContentLanguage = 'en'
    INNER JOIN surveys.QResponseOption   AS O
        ON O.SurveyOptionIdentifier = I.SurveyOptionItemIdentifier
           AND O.ResponseOptionIsSelected = 1
WHERE Q.SurveyQuestionSequence =
(
    SELECT MIN(QuestionWithSelection.SurveyQuestionSequence)
    FROM surveys.QSurveyQuestion AS QuestionWithSelection
    WHERE QuestionWithSelection.SurveyQuestionType IN ( 'RadioList', 'Selection' )
          AND QuestionWithSelection.SurveyFormIdentifier = Q.SurveyFormIdentifier
);
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [reports].[VGroup]
AS
    SELECT
        G.GroupCode
      , G.GroupCapacity
      , G.GroupCategory
      , G.GroupType
      , G.GroupCreated
      , G.GroupDescription
      , G.LastChangeTime           AS Modified
      , G.LastChangeUser           AS GroupModifiedBy
      , G.GroupName
      , G.GroupOffice
      , G.GroupPhone
      , G.GroupFax
      , G.GroupRegion
      , G.ShippingPreference       AS GroupShippingPreference
      , G.GroupIdentifier
      , G.GroupWebSiteUrl          AS GroupUrl
      , GS.ItemName                AS GroupStatus
      , GS.ItemIdentifier          AS GroupStatusItemIdentifier
      , G.GroupLabel
      , G.AddNewUsersAutomatically AS GroupAllowAutoSubscription
      , G.OrganizationIdentifier
    FROM
        contacts.QGroup        AS G
        left join utilities.TCollectionItem AS GS ON GS.ItemIdentifier = G.GroupStatusItemIdentifier
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [surveys].[QResponseAnswer](
	[ResponseSessionIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyQuestionIdentifier] [uniqueidentifier] NOT NULL,
	[RespondentUserIdentifier] [uniqueidentifier] NOT NULL,
	[ResponseAnswerText] [nvarchar](max) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QResponseAnswer] PRIMARY KEY CLUSTERED 
(
	[ResponseSessionIdentifier] ASC,
	[SurveyQuestionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [surveys].[VResponseFirstComment]
AS
SELECT Q.SurveyFormIdentifier      AS SurveyIdentifier,
       Q.SurveyQuestionSequence    AS QuestionSequence,
       Q.SurveyQuestionType        AS QuestionType,
       A.ResponseSessionIdentifier AS ResponseIdentifier,
       A.ResponseAnswerText        AS AnswerText
FROM surveys.QSurveyQuestion           AS Q
    INNER JOIN surveys.QResponseAnswer AS A
        ON A.SurveyQuestionIdentifier = Q.SurveyQuestionIdentifier
WHERE Q.SurveyQuestionSequence =
(
    SELECT MIN(QuestionWithSelection.SurveyQuestionSequence)
    FROM surveys.QSurveyQuestion AS QuestionWithSelection
    WHERE QuestionWithSelection.SurveyQuestionType IN ( 'Comment' )
          AND QuestionWithSelection.SurveyFormIdentifier = Q.SurveyFormIdentifier
          AND A.ResponseAnswerText IS NOT NULL
          AND LEN(A.ResponseAnswerText) > 0
);
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QMembership](
	[MembershipIdentifier] [uniqueidentifier] NOT NULL,
	[MembershipEffective] [datetimeoffset](7) NOT NULL,
	[MembershipFunction] [varchar](20) NULL,
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[MembershipExpiry] [datetimeoffset](7) NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QMembership] PRIMARY KEY CLUSTERED 
(
	[MembershipIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [contacts].[Membership]
as
select QMembership.MembershipEffective as Assigned
     , QMembership.MembershipFunction  as MembershipType
     , QMembership.GroupIdentifier
     , QMembership.UserIdentifier
     , QMembership.OrganizationIdentifier
     , QMembership.MembershipIdentifier
	 , QMembership.MembershipExpiry
     , QMembership.Modified
     , QMembership.ModifiedBy
from contacts.QMembership
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [achievements].[QAchievement](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[AchievementIdentifier] [uniqueidentifier] NOT NULL,
	[AchievementLabel] [varchar](50) NOT NULL,
	[AchievementTitle] [varchar](200) NOT NULL,
	[AchievementDescription] [varchar](1200) NULL,
	[AchievementIsEnabled] [bit] NOT NULL,
	[ExpirationType] [varchar](8) NULL,
	[ExpirationFixedDate] [datetimeoffset](7) NULL,
	[ExpirationLifetimeQuantity] [int] NULL,
	[ExpirationLifetimeUnit] [varchar](6) NULL,
	[CertificateLayoutCode] [varchar](100) NULL,
	[AchievementType] [varchar](100) NULL,
	[AchievementReportingDisabled] [bit] NOT NULL,
	[BadgeImageUrl] [varchar](500) NULL,
	[HasBadgeImage] [bit] NULL,
	[AchievementAllowSelfDeclared] [bit] NOT NULL,
 CONSTRAINT [PK_QAchievement] PRIMARY KEY CLUSTERED 
(
	[AchievementIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [achievements].[QCredential](
	[AchievementIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[CredentialGranted] [datetimeoffset](7) NULL,
	[CredentialRevoked] [datetimeoffset](7) NULL,
	[CredentialExpired] [datetimeoffset](7) NULL,
	[ExpirationType] [varchar](8) NULL,
	[ExpirationFixedDate] [datetimeoffset](7) NULL,
	[ExpirationLifetimeQuantity] [int] NULL,
	[ExpirationLifetimeUnit] [varchar](6) NULL,
	[CredentialAssigned] [datetimeoffset](7) NULL,
	[CredentialStatus] [varchar](10) NULL,
	[CredentialReminderType] [varchar](20) NULL,
	[AuthorityName] [varchar](50) NULL,
	[AuthorityLocation] [varchar](50) NULL,
	[AuthorityReference] [varchar](40) NULL,
	[CredentialDescription] [varchar](500) NULL,
	[CredentialHours] [decimal](5, 2) NULL,
	[CredentialExpirationExpected] [datetimeoffset](7) NULL,
	[CredentialExpirationReminderRequested0] [datetimeoffset](7) NULL,
	[CredentialExpirationReminderRequested1] [datetimeoffset](7) NULL,
	[CredentialExpirationReminderRequested2] [datetimeoffset](7) NULL,
	[CredentialExpirationReminderRequested3] [datetimeoffset](7) NULL,
	[CredentialExpirationReminderDelivered0] [datetimeoffset](7) NULL,
	[CredentialExpirationReminderDelivered1] [datetimeoffset](7) NULL,
	[CredentialExpirationReminderDelivered2] [datetimeoffset](7) NULL,
	[CredentialExpirationReminderDelivered3] [datetimeoffset](7) NULL,
	[CredentialIdentifier] [uniqueidentifier] NOT NULL,
	[CredentialNecessity] [varchar](50) NULL,
	[CredentialPriority] [varchar](50) NULL,
	[AuthorityIdentifier] [uniqueidentifier] NULL,
	[AuthorityType] [varchar](20) NULL,
	[CredentialRevokedReason] [varchar](200) NULL,
	[CredentialGrantedDescription] [varchar](200) NULL,
	[CredentialGrantedScore] [decimal](5, 4) NULL,
	[CredentialRevokedScore] [decimal](5, 4) NULL,
	[TransactionHash] [varchar](100) NULL,
	[PublisherAddress] [varchar](100) NULL,
	[PublicationStatus] [int] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[EmployerGroupIdentifier] [uniqueidentifier] NULL,
	[EmployerGroupStatus] [varchar](100) NULL,
 CONSTRAINT [PK_QCredential] PRIMARY KEY CLUSTERED 
(
	[CredentialIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [reports].[VPersonAchievement]
AS
    SELECT
        A.AchievementIdentifier
      , A.AchievementLabel
      , A.AchievementTitle
      , A.AchievementDescription
      , A.AchievementIsEnabled
      , A.ExpirationType             AS AchievementExpirationType
      , A.ExpirationFixedDate        AS AchievementExpirationFixedDate
      , A.ExpirationLifetimeQuantity AS AchievementExpirationLifetimeQuantity
      , A.ExpirationLifetimeUnit     AS AchievementExpirationLifetimeUnit
      , A.CertificateLayoutCode
      , C.CredentialGranted
      , C.CredentialRevoked
      , C.CredentialExpired
      , C.ExpirationType             AS CredentialExpirationType
      , C.ExpirationFixedDate        AS CredentialExpirationFixedDate
      , C.ExpirationLifetimeQuantity AS CredentialExpirationLifetimeQuantity
      , C.ExpirationLifetimeUnit     AS CredentialExpirationLifetimeUnit
      , C.CredentialAssigned
      , C.CredentialStatus
      , C.CredentialReminderType
      , C.AuthorityName              AS CredentialAuthorityName
      , C.AuthorityLocation          AS CredentialAuthorityLocation
      , C.AuthorityReference         AS CredentialAuthorityReference
      , C.CredentialDescription
      , C.CredentialHours
      , C.CredentialExpirationExpected
      , C.CredentialExpirationReminderRequested0
      , C.CredentialExpirationReminderRequested1
      , C.CredentialExpirationReminderRequested2
      , C.CredentialExpirationReminderRequested3
      , C.CredentialExpirationReminderDelivered0
      , C.CredentialExpirationReminderDelivered1
      , C.CredentialExpirationReminderDelivered2
      , C.CredentialExpirationReminderDelivered3
      , C.CredentialIdentifier
      , C.CredentialNecessity
      , C.CredentialPriority
      , C.AuthorityType              AS CredentialAuthorityType
      , C.CredentialRevokedReason
      , C.UserIdentifier
    FROM
        achievements.QCredential         AS C
    INNER JOIN achievements.QAchievement AS A
               ON A.AchievementIdentifier = C.AchievementIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QPeriod](
	[PeriodIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[PeriodName] [varchar](50) NOT NULL,
	[PeriodStart] [datetimeoffset](7) NOT NULL,
	[PeriodEnd] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_QPeriod] PRIMARY KEY CLUSTERED 
(
	[PeriodIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [surveys].[QSurveyForm](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyFormIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyFormStatus] [varchar](20) NOT NULL,
	[SurveyFormLanguage] [varchar](2) NULL,
	[SurveyFormLanguageTranslations] [varchar](20) NULL,
	[SurveyFormName] [varchar](256) NOT NULL,
	[UserFeedback] [varchar](8) NOT NULL,
	[RequireUserAuthentication] [bit] NOT NULL,
	[RequireUserIdentification] [bit] NOT NULL,
	[AssetNumber] [int] NOT NULL,
	[ResponseLimitPerUser] [int] NULL,
	[SurveyFormOpened] [datetimeoffset](7) NULL,
	[SurveyFormClosed] [datetimeoffset](7) NULL,
	[SurveyMessageInvitation] [uniqueidentifier] NULL,
	[SurveyMessageResponseStarted] [uniqueidentifier] NULL,
	[SurveyMessageResponseCompleted] [uniqueidentifier] NULL,
	[SurveyMessageResponseConfirmed] [uniqueidentifier] NULL,
	[SurveyFormLocked] [datetimeoffset](7) NULL,
	[SurveyFormDurationMinutes] [int] NULL,
	[SurveyFormHook] [varchar](100) NULL,
	[PageCount] [int] NOT NULL,
	[QuestionCount] [int] NOT NULL,
	[BranchCount] [int] NOT NULL,
	[ConditionCount] [int] NOT NULL,
	[EnableUserConfidentiality] [bit] NOT NULL,
	[DisplaySummaryChart] [bit] NOT NULL,
	[LastChangeTime] [datetimeoffset](7) NOT NULL,
	[LastChangeType] [varchar](100) NOT NULL,
	[LastChangeUser] [uniqueidentifier] NOT NULL,
	[HasWorkflowConfiguration] [bit] NOT NULL,
	[SurveyFormTitle] [varchar](256) NULL,
 CONSTRAINT [PK_QSurveyForm] PRIMARY KEY CLUSTERED 
(
	[SurveyFormIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [surveys].[QResponseSession](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyFormIdentifier] [uniqueidentifier] NOT NULL,
	[ResponseSessionIdentifier] [uniqueidentifier] NOT NULL,
	[ResponseSessionStatus] [varchar](20) NOT NULL,
	[RespondentUserIdentifier] [uniqueidentifier] NOT NULL,
	[RespondentLanguage] [varchar](2) NULL,
	[ResponseIsLocked] [bit] NOT NULL,
	[ResponseSessionCreated] [datetimeoffset](7) NULL,
	[ResponseSessionStarted] [datetimeoffset](7) NULL,
	[ResponseSessionCompleted] [datetimeoffset](7) NULL,
	[GroupIdentifier] [uniqueidentifier] NULL,
	[PeriodIdentifier] [uniqueidentifier] NULL,
	[LastChangeTime] [datetimeoffset](7) NOT NULL,
	[LastChangeType] [varchar](100) NOT NULL,
	[LastChangeUser] [uniqueidentifier] NOT NULL,
	[LastAnsweredQuestionIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QResponseSession] PRIMARY KEY CLUSTERED 
(
	[ResponseSessionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [surveys].[VResponse]
as
select R.GroupIdentifier
     , R.LastAnsweredQuestionIdentifier
     , R.LastChangeTime
     , R.LastChangeType
     , R.LastChangeUser
     , R.OrganizationIdentifier
     , R.PeriodIdentifier
     , R.RespondentLanguage
     , R.RespondentUserIdentifier
     , R.ResponseIsLocked
     , R.ResponseSessionCompleted
     , R.ResponseSessionCreated
     , R.ResponseSessionIdentifier
     , R.ResponseSessionStarted
     , R.ResponseSessionStatus
     , R.SurveyFormIdentifier
     , S.EnableUserConfidentiality as SurveyIsConfidential
     , S.SurveyFormName            as SurveyName
     , S.AssetNumber               as SurveyNumber
     , U.FullName                  as RespondentName
     , U.FirstName                 as RespondentNameFirst
     , U.LastName                  as RespondentNameLast
     , U.Email                     as RespondentEmail
     , G.GroupName
     , P.PeriodName
     , FirstComment.AnswerText     as FirstCommentText
     , FirstSelection.AnswerText   as FirstSelectionText
from surveys.QResponseSession                 as R
    inner join surveys.QSurveyForm            as S
        on S.SurveyFormIdentifier = R.SurveyFormIdentifier
    left join identities.QUser                as U
        on R.RespondentUserIdentifier = U.UserIdentifier
    left join contacts.QGroup                 as G
        on G.GroupIdentifier = R.GroupIdentifier
    left join records.QPeriod                 as P
        on P.PeriodIdentifier = R.PeriodIdentifier
    left join surveys.VResponseFirstComment   as FirstComment
        on FirstComment.ResponseIdentifier = R.ResponseSessionIdentifier
    left join surveys.VResponseFirstSelection as FirstSelection
        on FirstSelection.ResponseIdentifier = R.ResponseSessionIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[QAttempt](
	[AttemptGraded] [datetimeoffset](7) NULL,
	[AttemptDuration] [decimal](12, 3) NULL,
	[AttemptGrade] [varchar](4) NULL,
	[AttemptIdentifier] [uniqueidentifier] NOT NULL,
	[AttemptImported] [datetimeoffset](7) NULL,
	[AttemptIsPassing] [bit] NOT NULL,
	[AttemptNumber] [int] NOT NULL,
	[AttemptPinged] [datetimeoffset](7) NULL,
	[AttemptPoints] [decimal](7, 2) NULL,
	[AttemptScore] [decimal](9, 8) NULL,
	[AttemptStarted] [datetimeoffset](7) NULL,
	[AttemptStatus] [varchar](50) NOT NULL,
	[AttemptTag] [varchar](100) NULL,
	[AttemptTimeLimit] [int] NULL,
	[AssessorUserIdentifier] [uniqueidentifier] NOT NULL,
	[FormIdentifier] [uniqueidentifier] NOT NULL,
	[FormPoints] [decimal](7, 2) NULL,
	[RegistrationIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserAgent] [varchar](512) NULL,
	[AttemptSubmitted] [datetimeoffset](7) NULL,
	[LearnerUserIdentifier] [uniqueidentifier] NOT NULL,
	[GradingAssessorUserIdentifier] [uniqueidentifier] NULL,
	[SectionsAsTabsEnabled] [bit] NOT NULL,
	[TabNavigationEnabled] [bit] NOT NULL,
	[FormSectionsCount] [int] NULL,
	[ActiveSectionIndex] [int] NULL,
	[SingleQuestionPerTabEnabled] [bit] NOT NULL,
	[ActiveQuestionIndex] [int] NULL,
	[TabTimeLimit] [varchar](8) NULL,
	[AttemptPingInterval] [int] NULL,
	[AttemptLanguage] [varchar](2) NULL,
 CONSTRAINT [PK_QAttempt] PRIMARY KEY CLUSTERED 
(
	[AttemptIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[TLearnerAttemptSummary](
	[FormIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerUserIdentifier] [uniqueidentifier] NOT NULL,
	[AttemptLastStartedIdentifier] [uniqueidentifier] NULL,
	[AttemptLastFailedIdentifier] [uniqueidentifier] NULL,
	[AttemptLastPassedIdentifier] [uniqueidentifier] NULL,
	[AttemptStartedCount] [int] NOT NULL,
	[AttemptPassedCount] [int] NOT NULL,
	[AttemptFailedCount] [int] NOT NULL,
	[AttemptVoidedCount] [int] NOT NULL,
	[AttemptImportedCount] [int] NOT NULL,
	[AttemptTotalCount] [int] NOT NULL,
	[AttemptLastStarted] [datetimeoffset](7) NULL,
	[AttemptLastPassed] [datetimeoffset](7) NULL,
	[AttemptLastFailed] [datetimeoffset](7) NULL,
	[AttemptLastSubmittedIdentifier] [uniqueidentifier] NULL,
	[AttemptLastGradedIdentifier] [uniqueidentifier] NULL,
	[AttemptSubmittedCount] [int] NOT NULL,
	[AttemptGradedCount] [int] NOT NULL,
	[AttemptLastSubmitted] [datetimeoffset](7) NULL,
	[AttemptLastGraded] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_TLearnerAttemptSummary] PRIMARY KEY CLUSTERED 
(
	[FormIdentifier] ASC,
	[LearnerUserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [reports].[VPersonAttempt]
as
select A.AttemptGraded
     , A.AttemptDuration
     , A.AttemptGrade
     , A.AttemptIdentifier
     , A.AttemptImported
     , A.AttemptIsPassing
     , A.AttemptNumber
     , A.AttemptPinged
     , A.AttemptPoints
     , A.AttemptScore
     , A.AttemptStarted
     , A.AttemptStatus
     , A.AttemptTag
     , A.AttemptTimeLimit
     , A.LearnerUserIdentifier as UserIdentifier
     , A.FormIdentifier
     , A.FormPoints
     , S.AttemptSubmittedCount
     , S.AttemptGradedCount
     , S.AttemptPassedCount
     , S.AttemptStartedCount
     , F.BankLevelType
     , F.FormAsset
     , F.FormAssetVersion
     , F.FormCode
     , F.FormFirstPublished
     , F.FormHook
     , F.FormName
     , F.FormOrigin
     , F.FormPassingScore
     , F.FormPublicationStatus
     , F.FormSource
     , F.FormTimeLimit
     , F.FormTitle
     , F.FormType
     , F.FormVersion
     , F.SectionCount
     , F.SpecIdentifier
     , F.SpecQuestionLimit
     , F.OrganizationIdentifier
from assessments.QAttempt                          as A
     inner join assessments.TLearnerAttemptSummary as S on S.FormIdentifier = A.FormIdentifier
                                                           and S.LearnerUserIdentifier = A.LearnerUserIdentifier
     inner join banks.QBankForm                    as F on F.FormIdentifier = A.FormIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [courses].[QCourse](
	[CourseIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[FrameworkStandardIdentifier] [uniqueidentifier] NULL,
	[GradebookIdentifier] [uniqueidentifier] NULL,
	[CatalogIdentifier] [uniqueidentifier] NULL,
	[CompletionActivityIdentifier] [uniqueidentifier] NULL,
	[CompletedToLearnerMessageIdentifier] [uniqueidentifier] NULL,
	[CompletedToAdministratorMessageIdentifier] [uniqueidentifier] NULL,
	[StalledToLearnerMessageIdentifier] [uniqueidentifier] NULL,
	[StalledToAdministratorMessageIdentifier] [uniqueidentifier] NULL,
	[CourseAsset] [int] NOT NULL,
	[CourseName] [varchar](200) NOT NULL,
	[CourseHook] [varchar](100) NULL,
	[CourseCode] [varchar](30) NULL,
	[CourseImage] [varchar](200) NULL,
	[CoursePlatform] [varchar](100) NULL,
	[SourceIdentifier] [uniqueidentifier] NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[CourseIsHidden] [bit] NOT NULL,
	[CourseLabel] [varchar](20) NULL,
	[CourseProgram] [varchar](50) NULL,
	[CourseLevel] [varchar](50) NULL,
	[CourseUnit] [varchar](50) NULL,
	[CourseDescription] [varchar](max) NULL,
	[CourseStyle] [varchar](max) NULL,
	[IsMultipleUnitsEnabled] [bit] NOT NULL,
	[CourseSequence] [int] NULL,
	[CourseIcon] [varchar](30) NULL,
	[CourseSlug] [varchar](100) NULL,
	[SendMessageStalledAfterDays] [int] NULL,
	[SendMessageStalledMaxCount] [int] NULL,
	[IsProgressReportEnabled] [bit] NOT NULL,
	[OutlineWidth] [int] NULL,
	[AllowDiscussion] [bit] NOT NULL,
	[CourseFlagColor] [varchar](10) NULL,
	[CourseFlagText] [varchar](50) NULL,
	[Closed] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_QCourse] PRIMARY KEY CLUSTERED 
(
	[CourseIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [accounts].[QOrganization](
	[AccountClosed] [datetimeoffset](7) NULL,
	[AccountOpened] [datetimeoffset](7) NULL,
	[AccountStatus] [varchar](6) NOT NULL,
	[CompanyDomain] [varchar](50) NULL,
	[CompanyName] [varchar](50) NULL,
	[CompanySize] [varchar](10) NULL,
	[CompanySummary] [varchar](900) NULL,
	[CompanyTitle] [varchar](100) NULL,
	[CompanyWebSiteUrl] [varchar](500) NULL,
	[CompetencyAutoExpirationMode] [varchar](8) NOT NULL,
	[CompetencyAutoExpirationMonth] [int] NULL,
	[CompetencyAutoExpirationDay] [int] NULL,
	[StandardContentLabels] [varchar](200) NULL,
	[OrganizationCode] [varchar](30) NULL,
	[OrganizationLogoUrl] [varchar](500) NULL,
	[TimeZone] [varchar](32) NULL,
	[AdministratorUserIdentifier] [uniqueidentifier] NULL,
	[GlossaryIdentifier] [uniqueidentifier] NULL,
	[ParentOrganizationIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationData] [varchar](7700) NOT NULL,
	[PersonFullNamePolicy] [varchar](50) NULL,
 CONSTRAINT [PK_QOrganization] PRIMARY KEY CLUSTERED 
(
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [learning].[TCourseCategory](
	[CourseIdentifier] [uniqueidentifier] NOT NULL,
	[ItemIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[CategorySequence] [int] NULL,
 CONSTRAINT [PK_TCourseCategory] PRIMARY KEY NONCLUSTERED 
(
	[CourseIdentifier] ASC,
	[ItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [learning].[TCatalog](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[CatalogIdentifier] [uniqueidentifier] NOT NULL,
	[CatalogName] [varchar](100) NOT NULL,
	[IsHidden] [bit] NOT NULL,
 CONSTRAINT [PK_TCatalog] PRIMARY KEY CLUSTERED 
(
	[CatalogIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [courses].[VCatalogCourse]
    AS
        SELECT T.CatalogIdentifier,
               T.CatalogName,
               T.IsHidden                      AS CatalogIsHidden,
               I.ItemName                      AS CourseCategory,
               C.Created                       AS CourseCreated,
               C.CourseFlagColor,
               C.CourseFlagText,
               C.CourseIdentifier,
               C.CourseIsHidden,
               C.CourseImage,
               C.CourseLabel,
               C.Modified                      AS CourseModified,
               C.CourseName,
               C.CourseSlug,
               ISNULL(C.CourseLabel, 'Course') AS CourseType,
               O.OrganizationCode,
               O.OrganizationIdentifier,
               O.CompanyName                   AS OrganizationName
        FROM courses.QCourse AS C
                 INNER JOIN
             learning.TCatalog AS T
             ON C.CatalogIdentifier = T.CatalogIdentifier
                 INNER JOIN
             accounts.QOrganization AS O
             ON O.OrganizationIdentifier = C.OrganizationIdentifier
                 LEFT JOIN
             learning.TCourseCategory AS G
             ON G.CourseIdentifier = C.CourseIdentifier
                 LEFT JOIN
             utilities.TCollectionItem AS I
             ON G.ItemIdentifier = I.ItemIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [reports].[VPersonEventRegistration]
AS
    SELECT
        R.EventIdentifier                                         AS RegistrationEventIdentifier
      , R.EventPotentialConflicts                                 AS RegistrationEventPotentialConflicts
      , R.ApprovalProcess                                         AS RegistrationApprovalProcess
      , R.ApprovalReason                                          AS RegistrationApprovalReason
      , R.ApprovalStatus                                          AS RegistrationApprovalStatus
      , R.AttemptIdentifier                                       AS RegistrationAttemptIdentifier
      , R.AttendanceStatus                                        AS RegistrationAttendanceStatus
      , R.BillingCustomer                                         AS RegistrationBillingCustomer
      , CASE
            WHEN P.Birthdate IS NOT NULL
                 AND E.EventScheduledStart IS NOT NULL
                THEN
      ((CONVERT(INT, CONVERT(CHAR(8), E.EventScheduledStart, 112)) - CONVERT(CHAR(8), P.Birthdate, 112)) / 10000)
            ELSE
                NULL
        END                                                       AS RegistrationCandidateAge
      , R.CandidateIdentifier                                     AS RegistrationCandidateIdentifier
      , R.CandidateType                                           AS RegistrationCandidateType
      , R.CustomerIdentifier                                      AS RegistrationCustomerIdentifier
      , R.DistributionExpected                                    AS RegistrationDistributionExpected
      , R.EligibilityProcess                                      AS RegistrationEligibilityProcess
      , R.EligibilityStatus                                       AS RegistrationEligibilityStatus
      , R.EmployerIdentifier                                      AS RegistrationEmployerIdentifier
      , R.ExamFormIdentifier                                      AS RegistrationExamFormIdentifier
      , R.ExamTimeLimit                                           AS RegistrationExamTimeLimit
      , R.Grade                                                   AS RegistrationGrade
      , R.GradeAssigned                                           AS RegistrationGradeAssigned
      , R.GradePublished                                          AS RegistrationGradePublished
      , R.GradeReleased                                           AS RegistrationGradeReleased
      , R.GradeWithheld                                           AS RegistrationGradeWithheld
      , R.GradeWithheldReason                                     AS RegistrationGradeWithheldReason
      , R.GradingProcess                                          AS RegistrationGradingProcess
      , R.GradingStatus                                           AS RegistrationGradingStatus
      , R.IncludeInT2202                                          AS RegistrationIncludeInT2202
      , R.LastChangeTime                                          AS RegistrationLastChangeTime
      , R.LastChangeType                                          AS RegistrationLastChangeType
      , R.LastChangeUser                                          AS RegistrationLastChangeUser
      , R.MaterialsIncludeDiagramBook                             AS RegistrationMaterialsIncludeDiagramBook
      , R.MaterialsPackagedForDistribution                        AS RegistrationMaterialsPackagedForDistribution
      , R.MaterialsPermittedToCandidates                          AS RegistrationMaterialsPermittedToCandidates
      , R.PaymentIdentifier                                       AS RegistrationPaymentIdentifier
      , R.RegistrationComment                                     AS RegistrationComment
      , R.RegistrationFee                                         AS RegistrationFee
      , R.RegistrationIdentifier                                  AS RegistrationIdentifier
      , R.RegistrationPassword                                    AS RegistrationPassword
      , R.RegistrationRequestedOn                                 AS RegistrationRequestedOn
      , R.RegistrationSequence                                    AS RegistrationSequence
      , R.RegistrationSource                                      AS RegistrationSource
      , R.SchoolIdentifier                                        AS RegistrationSchoolIdentifier
      , R.Score                                                   AS RegistrationScore
      , R.SeatIdentifier                                          AS RegistrationSeatIdentifier
      , R.SynchronizationProcess                                  AS RegistrationSynchronizationProcess
      , R.SynchronizationStatus                                   AS RegistrationSynchronizationStatus
      , R.WorkBasedHoursToDate                                    AS RegistrationWorkBasedHoursToDate
      , G.GroupIdentifier                                         AS EmployerGroupIdentifier
      , G.GroupName                                               AS EmployerGroupName
      , GS.ItemName                                               AS EmployerGroupStatus
      , GS.ItemIdentifier                                         AS EmployerGroupStatusItemIdentifier
      , E.AchievementIdentifier                                   AS EventAchievementIdentifier
      , E.EventBillingType                                        AS EventBillingType
      , E.EventClassCode                                          AS EventClassCode
      , E.EventDescription                                        AS EventDescription
      , E.EventFormat                                             AS EventFormat
      , E.EventIdentifier                                         AS EventIdentifier
      , E.EventNumber                                             AS EventNumber
      , E.EventPublicationStatus                                  AS EventPublicationStatus
      , E.EventRequisitionStatus                                  AS EventRequisitionStatus
      , E.EventScheduledEnd                                       AS EventScheduledEnd
      , E.EventScheduledStart                                     AS EventScheduledStart
      , DATEDIFF(DAY, SYSDATETIMEOFFSET(), E.EventScheduledStart) AS EventScheduledStartAgeInDays
      , E.EventSchedulingStatus                                   AS EventSchedulingStatus
      , E.EventSource                                             AS EventSource
      , E.EventSummary                                            AS EventSummary
      , E.EventTitle                                              AS EventTitle
      , E.EventType                                               AS EventType
      , E.CapacityMaximum                                         AS EventCapacityMaximum
      , E.CapacityMinimum                                         AS EventCapacityMinimum
      , E.Content                                                 AS EventContent
      , E.CreditHours                                             AS EventCreditHours
      , E.DistributionCode                                        AS EventDistributionCode
      , E.DistributionErrors                                      AS EventDistributionErrors
      , E.DistributionExpected                                    AS EventDistributionExpected
      , E.DistributionOrdered                                     AS EventDistributionOrdered
      , E.DistributionProcess                                     AS EventDistributionProcess
      , E.DistributionShipped                                     AS EventDistributionShipped
      , E.DistributionStatus                                      AS EventDistributionStatus
      , E.DistributionTracked                                     AS EventDistributionTracked
      , E.DurationQuantity                                        AS EventDurationQuantity
      , E.DurationUnit                                            AS EventDurationUnit
      , E.ExamDurationInMinutes                                   AS EventExamDurationInMinutes
      , E.ExamMaterialReturnShipmentCode                          AS EventExamMaterialReturnShipmentCode
      , E.ExamMaterialReturnShipmentCondition                     AS EventExamMaterialReturnShipmentCondition
      , E.ExamMaterialReturnShipmentReceived                      AS EventExamMaterialReturnShipmentReceived
      , E.ExamStarted                                             AS EventExamStarted
      , E.ExamType                                                AS EventExamType
      , E.InvigilatorMinimum                                      AS EventInvigilatorMinimum
      , E.LastChangeTime                                          AS EventLastChangeTime
      , E.LastChangeType                                          AS EventLastChangeType
      , E.LastChangeUser                                          AS EventLastChangeUser
      , E.PublicationErrors                                       AS EventPublicationErrors
      , E.RegistrationDeadline                                    AS EventRegistrationDeadline
      , E.RegistrationStart                                       AS EventRegistrationStart
      , E.OrganizationIdentifier                                  AS EventOrganizationIdentifier
      , E.VenueCoordinatorIdentifier                              AS EventVenueCoordinatorIdentifier
      , E.VenueLocationIdentifier                                 AS EventVenueLocationIdentifier
      , E.VenueRoom                                               AS EventVenueRoom
      , E.WaitlistEnabled                                         AS EventWaitlistEnabled
      , A.AchievementLabel
      , A.AchievementTitle
      , C.CredentialGranted
      , C.CredentialStatus
    FROM
        registrations.QRegistration     AS R
    LEFT JOIN events.QEvent             AS E
              ON E.EventIdentifier = R.EventIdentifier

    LEFT JOIN contacts.QGroup          AS G
              ON R.EmployerIdentifier = G.GroupIdentifier

    LEFT JOIN contacts.Person         AS P
              ON P.UserIdentifier = R.CandidateIdentifier
                AND P.OrganizationIdentifier = R.OrganizationIdentifier

    LEFT JOIN achievements.QAchievement AS A
              ON A.AchievementIdentifier = E.AchievementIdentifier

    LEFT JOIN achievements.QCredential  AS C
              ON C.AchievementIdentifier = E.AchievementIdentifier
                 AND C.UserIdentifier = P.UserIdentifier
    LEFT JOIN utilities.TCollectionItem AS GS
              ON GS.ItemIdentifier = G.GroupStatusItemIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QGradebook](
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[EventIdentifier] [uniqueidentifier] NULL,
	[AchievementIdentifier] [uniqueidentifier] NULL,
	[FrameworkIdentifier] [uniqueidentifier] NULL,
	[GradebookCreated] [datetimeoffset](7) NOT NULL,
	[GradebookTitle] [varchar](100) NOT NULL,
	[GradebookType] [varchar](20) NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[Reference] [varchar](100) NULL,
	[PeriodIdentifier] [uniqueidentifier] NULL,
	[LastChangeTime] [datetimeoffset](7) NOT NULL,
	[LastChangeType] [varchar](100) NOT NULL,
	[LastChangeUser] [varchar](100) NOT NULL,
 CONSTRAINT [PK_QGradebook] PRIMARY KEY CLUSTERED 
(
	[GradebookIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QGradeItem](
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[GradeItemIdentifier] [uniqueidentifier] NOT NULL,
	[ParentGradeItemIdentifier] [uniqueidentifier] NULL,
	[GradeItemSequence] [int] NOT NULL,
	[GradeItemCode] [varchar](100) NULL,
	[GradeItemName] [varchar](400) NOT NULL,
	[AchievementIdentifier] [uniqueidentifier] NULL,
	[GradeItemFormat] [varchar](20) NOT NULL,
	[GradeItemType] [varchar](20) NOT NULL,
	[GradeItemWeighting] [varchar](20) NOT NULL,
	[GradeItemIsReported] [bit] NOT NULL,
	[GradeItemShortName] [varchar](50) NULL,
	[GradeItemReference] [varchar](100) NULL,
	[GradeItemMaxPoints] [decimal](28, 4) NULL,
	[GradeItemHook] [varchar](100) NULL,
	[GradeItemPassPercent] [decimal](5, 4) NULL,
	[AchievementWhenChange] [varchar](20) NULL,
	[AchievementWhenGrade] [varchar](20) NULL,
	[AchievementThenCommand] [varchar](20) NULL,
	[AchievementElseCommand] [varchar](20) NULL,
	[AchievementFixedDate] [datetimeoffset](7) NULL,
	[ProgressCompletedMessageIdentifier] [uniqueidentifier] NULL,
	[StatementWhenVerb] [varchar](100) NULL,
	[StatementWhenObject] [varchar](100) NULL,
	[StatementThenCommand] [varchar](20) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QGradeItem] PRIMARY KEY CLUSTERED 
(
	[GradeItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QProgress](
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[GradeItemIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[ProgressPercent] [decimal](5, 4) NULL,
	[ProgressText] [varchar](100) NULL,
	[ProgressComment] [varchar](1200) NULL,
	[ProgressNumber] [decimal](28, 4) NULL,
	[ProgressGraded] [datetimeoffset](7) NULL,
	[ProgressPoints] [decimal](28, 4) NULL,
	[ProgressMaxPoints] [decimal](28, 4) NULL,
	[ProgressStarted] [datetimeoffset](7) NULL,
	[ProgressCompleted] [datetimeoffset](7) NULL,
	[ProgressIsPublished] [bit] NOT NULL,
	[ProgressIsCompleted] [bit] NOT NULL,
	[ProgressIsLocked] [bit] NOT NULL,
	[ProgressIsDisabled] [bit] NOT NULL,
	[ProgressPassOrFail] [varchar](4) NULL,
	[ProgressStatus] [varchar](20) NULL,
	[ProgressIdentifier] [uniqueidentifier] NOT NULL,
	[ProgressRestartCount] [int] NULL,
	[ProgressAdded] [datetimeoffset](7) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[ProgressIsIgnored] [bit] NOT NULL,
	[ProgressElapsedSeconds] [int] NULL,
 CONSTRAINT [PK_QProgress] PRIMARY KEY CLUSTERED 
(
	[ProgressIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [reports].[VPersonGradebookScore]
AS
    SELECT
        S.GradebookIdentifier
      , S.UserIdentifier      AS UserIdentifier
      , S.ProgressPercent     AS ScorePercent
      , S.ProgressText        AS ScoreText
      , S.ProgressComment     AS ScoreComment
      , S.ProgressIsPublished AS ScoreIsReleased
      , S.ProgressNumber      AS ScoreNumber
      , S.ProgressGraded      AS ScoreGraded
      , S.ProgressPoints      AS ScorePoint
      , S.ProgressMaxPoints   AS ScoreMaxPoint
      , I.GradeItemIdentifier AS ItemKey
      , I.GradeItemSequence   AS ItemSequence
      , I.GradeItemCode       AS ItemCode
      , I.GradeItemName       AS ItemName
      , I.GradeItemFormat     AS ItemFormat
      , I.GradeItemType       AS ItemType
      , I.GradeItemWeighting  AS ItemWeighting
      , I.GradeItemIsReported AS ItemReporting
      , I.GradeItemShortName  AS ItemNameShort
      , I.GradeItemReference  AS ItemReference
      , I.GradeItemMaxPoints  AS ItemMaxPoint
      , I.GradeItemHook       AS ItemHook
      , G.GradebookCreated
      , G.GradebookTitle
      , G.GradebookType
      , G.IsLocked            AS GradebookIsLocked
      , G.Reference           AS GradebookReference
    FROM
        records.QProgress         AS S
    INNER JOIN records.QGradeItem AS I
               ON I.GradeItemIdentifier = S.GradeItemIdentifier

    INNER JOIN records.QGradebook AS G
               ON G.GradebookIdentifier = I.GradebookIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [issues].[QIssueFileRequirement](
	[IssueIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[RequestedFileCategory] [varchar](120) NOT NULL,
	[RequestedTime] [datetimeoffset](7) NOT NULL,
	[RequestedUserIdentifier] [uniqueidentifier] NOT NULL,
	[RequestedFrom] [varchar](50) NOT NULL,
	[RequestedFileSubcategory] [varchar](120) NULL,
	[RequestedFileDescription] [varchar](2400) NULL,
	[RequestedFileStatus] [varchar](20) NULL,
 CONSTRAINT [PK_QIssueFileRequirement] PRIMARY KEY CLUSTERED 
(
	[IssueIdentifier] ASC,
	[RequestedFileCategory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [issues].[VIssueFileRequirement]
as
select r.IssueIdentifier
     , r.OrganizationIdentifier
     , r.RequestedFileCategory
     , r.RequestedFileSubcategory
     , r.RequestedTime
     , r.RequestedUserIdentifier
     , r.RequestedFrom
     , r.RequestedFileDescription
     , r.RequestedFileStatus
     , u.FullName as RequestedUserName
from issues.QIssueFileRequirement as r
     inner join identities.[User] as u on u.UserIdentifier = r.RequestedUserIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [reports].[VPersonGroup]
as
select G.GroupCode
     , G.GroupCapacity
     , G.GroupCategory
     , G.GroupType
     , G.GroupCreated             as GroupCreated
     , G.GroupDescription
     , G.LastChangeTime           as Modified
     , G.GroupName
     , G.GroupOffice
     , G.GroupPhone
     , G.GroupFax
     , G.GroupRegion
     , G.ShippingPreference       as GroupShippingPreference
     , G.GroupIdentifier
     , G.GroupWebSiteUrl          as GroupUrl
     , GS.ItemName                as GroupStatus
     , GS.ItemIdentifier          as GroupStatusItemIdentifier
     , G.GroupLabel
     , G.AddNewUsersAutomatically as GroupAllowAutoSubscription
     , M.Assigned                 as SubscriptionJoined
     , M.MembershipType           as SubscriptionType
     , M.UserIdentifier
from contacts.QGroup                  as G
     inner join contacts.Membership   as M on M.GroupIdentifier = G.GroupIdentifier
     left join contacts.QGroupAddress as BillingAddress on BillingAddress.GroupIdentifier = G.GroupIdentifier
                                                           and BillingAddress.AddressType = 'Billing'
     left join contacts.QGroupAddress as PhysicalAddress on PhysicalAddress.GroupIdentifier = G.GroupIdentifier
                                                            and BillingAddress.AddressType = 'Physical'
     left join contacts.QGroupAddress as ShippingAddress on ShippingAddress.GroupIdentifier = G.GroupIdentifier
                                                            and BillingAddress.AddressType = 'Shipping'
     left join utilities.TCollectionItem as GS on GS.ItemIdentifier = G.GroupStatusItemIdentifier
;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [reports].[TReport](
	[ReportIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[ReportTitle] [varchar](200) NOT NULL,
	[ReportData] [varchar](max) NULL,
	[ReportDescription] [varchar](300) NULL,
	[ReportType] [varchar](20) NOT NULL,
	[Created] [datetimeoffset](7) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[Modified] [datetimeoffset](7) NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ActionIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_TReport] PRIMARY KEY CLUSTERED 
(
	[ReportIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [reports].[VReport]
    AS
        SELECT R.ReportIdentifier
             , R.OrganizationIdentifier
             , R.UserIdentifier
             , R.ReportTitle
             , R.ReportData
             , R.ReportDescription
             , R.ReportType
             , R.Created
             , R.CreatedBy
             , R.Modified
             , R.ModifiedBy
             , UC.FullName AS CreatedByFullName
             , UM.FullName AS ModifiedByFullName
        FROM reports.TReport AS R WITH (NOLOCK)
                 LEFT JOIN identities.[User] AS UC WITH (NOLOCK)
                           ON UC.UserIdentifier = R.CreatedBy
                 LEFT JOIN identities.[User] AS UM WITH (NOLOCK)
                           ON UM.UserIdentifier = R.ModifiedBy;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [metadata].[VTable]
AS
SELECT 
  SCHEMA_NAME(SYSTBL.schema_id) AS SchemaName,
  SYSTBL.name AS TableName,
  SYSTBL.max_column_id_used AS ColumnCount,
  CAST(CASE SINDX_1.index_id
           WHEN 1 THEN 1
           ELSE 0
       END AS BIT) AS HasClusteredIndex,
  COALESCE((
      SELECT SUM(rows)
      FROM sys.partitions AS SPART
      WHERE object_id = SYSTBL.object_id
            AND index_id < 2
    ), 0) AS [RowCount],
  SYSTBL.create_date AS CreatedWhen,
  SYSTBL.modify_date AS ModifiedWhen
FROM sys.tables AS SYSTBL
INNER JOIN sys.indexes AS SINDX_1
  ON SINDX_1.object_id = SYSTBL.object_id AND SINDX_1.index_id < 2;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [metadata].[VTableColumn]
AS
SELECT 
  T.SchemaName,
  T.TableName,
  COLUMN_NAME AS ColumnName,
  DATA_TYPE AS DataType,
  CHARACTER_MAXIMUM_LENGTH AS MaximumLength,
  CAST(CASE
           WHEN IS_NULLABLE = 'YES' THEN 0
           WHEN IS_NULLABLE = 'NO' THEN 1
           ELSE NULL
       END AS BIT) AS IsRequired,
  CAST(CASE
           WHEN COLUMNPROPERTY(OBJECT_ID(T.SchemaName + '.' + T.TableName), COLUMN_NAME, 'IsIdentity') = 1 THEN 1
           ELSE 0
       END AS BIT) AS IsIdentity,
  C.ORDINAL_POSITION AS OrdinalPosition,
  C.COLUMN_DEFAULT AS DefaultValue
FROM INFORMATION_SCHEMA.COLUMNS AS C
INNER JOIN metadata.VTable AS T
  ON C.TABLE_NAME = T.TableName AND C.TABLE_SCHEMA = T.SchemaName;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandard](
	[AssetNumber] [int] NOT NULL,
	[AuthorDate] [datetimeoffset](7) NULL,
	[AuthorName] [varchar](100) NULL,
	[BankIdentifier] [uniqueidentifier] NULL,
	[BankSetIdentifier] [uniqueidentifier] NULL,
	[CalculationArgument] [int] NULL,
	[CanvasIdentifier] [varchar](100) NULL,
	[CategoryItemIdentifier] [uniqueidentifier] NULL,
	[CertificationHoursPercentCore] [decimal](5, 2) NULL,
	[CertificationHoursPercentNonCore] [decimal](5, 2) NULL,
	[Code] [varchar](256) NULL,
	[CompetencyScoreCalculationMethod] [varchar](50) NULL,
	[CompetencyScoreSummarizationMethod] [varchar](50) NULL,
	[ContentDescription] [nvarchar](max) NULL,
	[ContentName] [varchar](512) NULL,
	[ContentSettings] [varchar](100) NULL,
	[ContentSummary] [nvarchar](max) NULL,
	[ContentTitle] [varchar](1500) NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreditHours] [decimal](5, 2) NULL,
	[CreditIdentifier] [varchar](128) NULL,
	[DatePosted] [datetimeoffset](7) NULL,
	[DepartmentGroupIdentifier] [uniqueidentifier] NULL,
	[DocumentType] [varchar](40) NULL,
	[Icon] [varchar](32) NULL,
	[IndustryItemIdentifier] [uniqueidentifier] NULL,
	[IsCertificateEnabled] [bit] NOT NULL,
	[IsFeedbackEnabled] [bit] NOT NULL,
	[IsHidden] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[IsPractical] [bit] NOT NULL,
	[IsPublished] [bit] NOT NULL,
	[IsTemplate] [bit] NOT NULL,
	[IsTheory] [bit] NOT NULL,
	[Language] [nvarchar](8) NULL,
	[LevelCode] [varchar](1) NULL,
	[LevelType] [varchar](20) NULL,
	[MajorVersion] [varchar](8) NULL,
	[MasteryPoints] [decimal](5, 2) NULL,
	[MinorVersion] [varchar](8) NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ParentStandardIdentifier] [uniqueidentifier] NULL,
	[PointsPossible] [decimal](5, 2) NULL,
	[Sequence] [int] NOT NULL,
	[SourceDescriptor] [varchar](3400) NULL,
	[StandardAlias] [varchar](512) NULL,
	[StandardIdentifier] [uniqueidentifier] NOT NULL,
	[StandardLabel] [varchar](64) NULL,
	[StandardPrivacyScope] [varchar](10) NULL,
	[StandardStatus] [varchar](32) NULL,
	[StandardStatusLastUpdateTime] [datetimeoffset](7) NULL,
	[StandardStatusLastUpdateUser] [uniqueidentifier] NULL,
	[StandardTier] [varchar](30) NULL,
	[StandardType] [varchar](64) NOT NULL,
	[Tags] [varchar](100) NULL,
	[UtcPublished] [datetimeoffset](7) NULL,
	[StandardHook] [varchar](9) NULL,
	[PassingScore] [decimal](9, 8) NULL,
 CONSTRAINT [PK_QStandard] PRIMARY KEY CLUSTERED 
(
	[StandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [standards].[Standard]
AS
SELECT
    StandardLabel
   ,StandardType
   ,StandardTier
   ,AuthorDate
   ,AuthorName
   ,CalculationArgument
   ,CompetencyScoreSummarizationMethod
   ,CertificationHoursPercentCore
   ,CertificationHoursPercentNonCore
   ,Code
   ,ContentName
   ,ContentTitle
   ,CreatedBy
   ,Created
   ,CreditHours
   ,CreditIdentifier
   ,DatePosted
   ,Icon
   ,IsCertificateEnabled
   ,IsFeedbackEnabled
   ,IsHidden
   ,IsLocked
   ,IsPractical
   ,IsPublished
   ,IsTheory
   ,[Language]
   ,LevelCode
   ,LevelType
   ,MajorVersion
   ,MasteryPoints
   ,MinorVersion
   ,ModifiedBy
   ,Modified
   ,AssetNumber
   ,[Sequence]
   ,SourceDescriptor
   ,Tags
   ,StandardIdentifier
   ,UtcPublished
   ,DocumentType
   ,PointsPossible
   ,PassingScore
   ,CompetencyScoreCalculationMethod
   ,ContentDescription
   ,ContentSummary
   ,ContentSettings
   ,ParentStandardIdentifier
   ,OrganizationIdentifier
   ,StandardPrivacyScope
   ,IsTemplate
   ,CanvasIdentifier
   ,CategoryItemIdentifier
   ,StandardAlias
   ,IndustryItemIdentifier
   ,DepartmentGroupIdentifier
   ,BankIdentifier
   ,BankSetIdentifier
   ,StandardStatus
   ,StandardStatusLastUpdateUser
   ,StandardStatusLastUpdateTime
   ,StandardHook
FROM
    [standard].QStandard
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandardValidationLog](
	[LogIdentifier] [uniqueidentifier] NOT NULL,
	[StandardIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[AuthorUserIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[LogComment] [varchar](max) NULL,
	[LogPosted] [datetimeoffset](7) NOT NULL,
	[LogStatus] [varchar](50) NULL,
	[StandardValidationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QStandardValidationLog] PRIMARY KEY CLUSTERED 
(
	[LogIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [standards].[StandardValidationChange]
AS
SELECT
    LogComment AS ChangeComment
   ,LogIdentifier AS ChangeIdentifier
   ,LogPosted AS ChangePosted
   ,LogStatus AS ChangeStatus
   ,StandardIdentifier
   ,AuthorUserIdentifier
   ,UserIdentifier
   ,OrganizationIdentifier
FROM
    [standard].QStandardValidationLog
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [standards].[CompetencyValidationSummary]
AS
    SELECT
        ChangeIdentifier
      , ChangeStatus
      , ChangeComment
      , Competency.StandardIdentifier
      , Competency.Code                                AS CompetencyCode
      , CompetencyContent.ContentText                  AS CompetencyTitle
      , [User].UserIdentifier
      , [User].FirstName + ' ' + [User].LastName       AS UserName
      , AuthorUserIdentifier
      , Validator.FirstName + ' ' + Validator.LastName AS ValidatorName
      , ChangePosted
    FROM
        standards.StandardValidationChange
    INNER JOIN standards.Standard AS Competency
               ON Competency.StandardIdentifier = StandardValidationChange.StandardIdentifier

    INNER JOIN identities.[User]
               ON [User].UserIdentifier = StandardValidationChange.UserIdentifier

    LEFT JOIN identities.[User]   AS Validator
              ON Validator.UserIdentifier = StandardValidationChange.AuthorUserIdentifier

    LEFT JOIN contents.TContent   AS CompetencyContent
              ON CompetencyContent.ContainerIdentifier = Competency.StandardIdentifier
                 AND CompetencyContent.ContentLabel = 'Title'
                 AND CompetencyContent.ContentLanguage = 'en';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [standards].[Occupation]
as
select StandardIdentifier                            as OccupationKey
  , StandardIdentifier                        as OccupationIdentifier
  , Standard.OrganizationIdentifier
  , StandardTitle.ContentText                 as JobTitle
  , StandardSummary.ContentText               as Purpose
  , StandardStatements.ContentText            as Statements
from
    standards.Standard
    left join contents.TContent as StandardTitle on StandardTitle.ContainerIdentifier = Standard.StandardIdentifier
                                                        and StandardTitle.ContentLabel = 'Title'
                                                        and StandardTitle.ContentLanguage = 'en'
    left join contents.TContent as StandardSummary on StandardSummary.ContainerIdentifier = Standard.StandardIdentifier
                                                        and StandardSummary.ContentLabel = 'Summary'
                                                        and StandardSummary.ContentLanguage = 'en'
    left join contents.TContent as StandardStatements on StandardStatements.ContainerIdentifier = Standard.StandardIdentifier
                                                        and StandardStatements.ContentLabel = 'Statements'
                                                        and StandardStatements.ContentLanguage = 'en'
where StandardType = 'Occupation';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandardContainment](
	[ParentStandardIdentifier] [uniqueidentifier] NOT NULL,
	[ChildStandardIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ChildSequence] [int] NOT NULL,
	[CreditHours] [decimal](5, 2) NULL,
	[CreditType] [varchar](10) NULL,
 CONSTRAINT [PK_QStandardContainment] PRIMARY KEY CLUSTERED 
(
	[ParentStandardIdentifier] ASC,
	[ChildStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [standards].[StandardContainment]
AS
SELECT
    ChildSequence
   ,CreditHours
   ,CreditType
   ,ChildStandardIdentifier
   ,ParentStandardIdentifier
   ,OrganizationIdentifier
FROM
    [standard].QStandardContainment
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [standards].[StandardContainmentSummary]
as
    select
        parent.OrganizationIdentifier                               as ParentOrganizationIdentifier
      , parent.StandardType                            as ParentStandardType
      , parent.AssetNumber                             as ParentAssetNumber
      , parent.ContentTitle                            as ParentTitle
      , isnull(parent.ContentName, parent.AssetNumber) as ParentName
      , parent.Sequence                              as ParentSequence
      , parent.StandardIdentifier                      as ParentStandardIdentifier
      , parent.Icon                                    as ParentIcon
      , child.OrganizationIdentifier                         as ChildOrganizationIdentifier
      , child.StandardType                             as ChildStandardType
      , child.AssetNumber                              as ChildAssetNumber
      , child.ContentTitle                             as ChildTitle
      , isnull(child.ContentName, child.AssetNumber)   as ChildName
      , edge.ChildSequence                           as ChildSequence
      , child.StandardIdentifier                       as ChildStandardIdentifier
      , child.Icon                                     as ChildIcon
      , cast(0 as bit)                                 as ParentIsPrimaryContainer
    from
        standards.StandardContainment as edge
        inner join
        standards.Standard          as parent on parent.StandardIdentifier = edge.ParentStandardIdentifier
        inner join
        standards.Standard          as child on child.StandardIdentifier   = edge.ChildStandardIdentifier

    union

    select
        parent.OrganizationIdentifier
      , parent.StandardType
      , parent.AssetNumber
      , parent.ContentTitle
      , isnull(parent.ContentName, parent.AssetNumber)
      , parent.Sequence
      , parent.StandardIdentifier
      , parent.Icon
      , child.OrganizationIdentifier
      , child.StandardType
      , child.AssetNumber
      , child.ContentTitle
      , isnull(child.ContentName, child.AssetNumber)
      , child.Sequence
      , child.StandardIdentifier
      , child.Icon
      , cast(1 as bit)
    from
        standards.Standard as parent
        inner join
        standards.Standard as child on child.ParentStandardIdentifier = parent.StandardIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [assessments].[XAttemptOrganization]
as
    select
        A.AttemptIdentifier
      , P.OrganizationIdentifier as CandidateOrganizationIdentifier
      , F.OrganizationIdentifier as FormOrganizationIdentifier
    from
        assessments.QAttempt as A
        left join (
            identities.[User] as U

            inner join contacts.Person as P
               on U.UserIdentifier = P.UserIdentifier
              and P.IsLearner = 1
        )
            on U.UserIdentifier = A.AssessorUserIdentifier

        left join banks.QBankForm as F
            on A.FormIdentifier = F.FormIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [standards].[StandardHierarchy]
AS
    WITH CTE AS (
                SELECT
                    ParentStandardIdentifier
                  , StandardIdentifier
                  , StandardType
                  , StandardLabel
                  , Code
                  , ContentName             AS Name
                  , AssetNumber
                  , Sequence
                  , SourceDescriptor
                  , OrganizationIdentifier
                  , ContentTitle            AS Title
                  , IsHidden
                  , IsPractical
                  , IsPublished
                  , IsTheory
                FROM
                    standards.Standard WITH (NOLOCK))
       , Hierarchy AS (SELECT
                           StandardIdentifier                                                       AS RootStandardIdentifier
                         , ParentStandardIdentifier
                         , StandardIdentifier
                         , StandardType
                         , StandardLabel
                         , ContentName                                                              AS Name
                         , AssetNumber
                         , OrganizationIdentifier
                         , ContentTitle                                                             AS Title
                         , Sequence
                         , SourceDescriptor
                         , 0                                                                        AS Depth
                         , CAST(ISNULL(Code, '') AS NVARCHAR(MAX))                                  AS PathCode
                         , CAST(StandardIdentifier AS VARCHAR(MAX))                                 AS PathKey
                         , CAST('/' + ISNULL(ContentName, StandardIdentifier) + '' AS VARCHAR(MAX)) AS PathName
                         , CAST(AssetNumber AS NVARCHAR(MAX))                                       AS PathAsset
                         , RIGHT('00' + CAST(Sequence AS NVARCHAR(MAX)), 3)                         AS PathSequence
                         , IsHidden
                         , IsPractical
                         , IsPublished
                         , IsTheory
                       FROM
                           standards.Standard WITH (NOLOCK)
                       WHERE
                           ParentStandardIdentifier IS NULL
                       UNION ALL
                       SELECT
                           Parent.RootStandardIdentifier
                         , Child.ParentStandardIdentifier
                         , Child.StandardIdentifier
                         , Child.StandardType
                         , Child.StandardLabel
                         , Child.Name
                         , Child.AssetNumber
                         , Child.OrganizationIdentifier
                         , Child.Title
                         , Child.Sequence
                         , Child.SourceDescriptor
                         , Parent.Depth + 1
                         , Parent.PathCode + (CASE
                                                  WHEN Parent.PathCode <> ''
                                                       AND Child.Code IS NOT NULL
                                                      THEN
                                                      '.'
                                                  ELSE
                                                      ''
                                              END
                                             ) + ISNULL(Child.Code, '')
                         , Parent.PathKey + '/' + CAST(Child.StandardIdentifier AS VARCHAR(MAX))
                         , Parent.PathName + '/' + ISNULL(Child.Name, Child.StandardIdentifier)
                         , Parent.PathAsset + '/' + CAST(Child.AssetNumber AS NVARCHAR(MAX))
                         , Parent.PathSequence + '/' + RIGHT('00' + CAST(Child.Sequence AS NVARCHAR(MAX)), 3)
                         , Child.IsHidden
                         , Child.IsPractical
                         , Child.IsPublished
                         , Child.IsTheory
                       FROM
                           CTE              AS Child WITH (NOLOCK)
                       INNER JOIN Hierarchy AS Parent
                                  ON Child.ParentStandardIdentifier = Parent.StandardIdentifier)
    SELECT
        *
    FROM
        Hierarchy;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [standards].[VCompetency]
AS
    SELECT
        C.StandardIdentifier                  AS CompetencyIdentifier
      , ISNULL(C.StandardLabel, 'Competency') AS CompetencyLabel
      , C.Code                                AS CompetencyCode
      , C.ContentTitle                        AS CompetencyTitle
      , C.AssetNumber                         AS CompetencyAsset
      , C.Sequence                            AS CompetencySequence
      , (
            SELECT
                COUNT(*)
            FROM
                standards.Standard AS X
            WHERE
                X.ParentStandardIdentifier = C.StandardIdentifier
        )                                     AS CompetencySize
      , A.StandardIdentifier                  AS AreaIdentifier
      , ISNULL(A.StandardLabel, 'GAC')        AS AreaLabel
      , A.Code                                AS AreaCode
      , A.ContentTitle                        AS AreaTitle
      , A.AssetNumber                         AS AreaAsset
      , A.Sequence                            AS AreaSequence
      , A.StandardType                        AS AreaStandardType
      , (
            SELECT
                COUNT(*)
            FROM
                standards.Standard AS X
            WHERE
                X.ParentStandardIdentifier = A.StandardIdentifier
        )                                     AS AreaSize
      , AParent.StandardIdentifier            AS FrameworkIdentifier
    FROM
        standards.Standard       AS C
    LEFT JOIN standards.Standard AS A
              ON C.ParentStandardIdentifier = A.StandardIdentifier

    LEFT JOIN standards.Standard AS AParent
              ON A.ParentStandardIdentifier = AParent.StandardIdentifier
    WHERE
        C.StandardType = 'Competency';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [standards].[VFramework]
AS
SELECT F.StandardIdentifier AS FrameworkIdentifier,
       ISNULL(F.StandardLabel, 'Framework') AS FrameworkLabel,
       F.Code AS FrameworkCode,
       F.ContentTitle AS FrameworkTitle,
       F.AssetNumber AS FrameworkAsset,
       F.StandardIdentifier AS FrameworkKey,
       (
           SELECT COUNT(*)
           FROM standards.Standard AS X
           WHERE X.ParentStandardIdentifier = F.StandardIdentifier
       ) AS FrameworkSize,
       O.StandardIdentifier AS OccupationIdentifier,
       ISNULL(O.StandardLabel, 'Occupation') AS OccupationLabel,
       O.Code AS OccupationCode,
       O.ContentTitle AS OccupationTitle,
       O.AssetNumber AS OccupationAsset,
       O.StandardIdentifier AS OccupationKey,
       (
           SELECT COUNT(*)
           FROM standards.Standard AS X
           WHERE X.ParentStandardIdentifier = O.StandardIdentifier
       ) AS OccupationSize
FROM standards.Standard AS F
    LEFT JOIN standards.Standard AS O
        ON F.ParentStandardIdentifier = O.StandardIdentifier
WHERE F.StandardType = 'Framework';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [standards].[VStandard]
AS
SELECT S.AssetNumber AS StandardAsset,
       S.Code AS StandardCode,
       S.StandardIdentifier,
       ISNULL(S.StandardLabel, S.StandardType) AS StandardLabel,
       S.ContentTitle AS StandardTitle,
       S.StandardType,
       Parent.StandardIdentifier AS ParentStandardIdentifier,
       S.CompetencyScoreSummarizationMethod,
       S.CompetencyScoreCalculationMethod,
       S.CalculationArgument,
       S.MasteryPoints,
       S.PointsPossible
FROM standards.Standard AS S
    LEFT JOIN standards.Standard AS Parent
        ON S.ParentStandardIdentifier = Parent.StandardIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [accounts].[VOrganization]
    AS
        SELECT AccountClosed
             , AccountOpened
             , AccountStatus
             , AdministratorUserIdentifier
             , CompanyDomain
             , CompanyName
             , CompanySize
             , CompanySummary
             , CompanyTitle
             , CompanyWebSiteUrl
             , CompetencyAutoExpirationDay
             , CompetencyAutoExpirationMode
             , CompetencyAutoExpirationMonth
             , PersonFullNamePolicy
             , GlossaryIdentifier
             , OrganizationCode
             , OrganizationData
             , OrganizationIdentifier
             , OrganizationLogoUrl
             , ParentOrganizationIdentifier
             , StandardContentLabels
             , TimeZone
        FROM accounts.QOrganization;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[PersonSummary]
AS
    SELECT
        U.UserIdentifier
      , T.OrganizationIdentifier
      , T.OrganizationCode
      , T.CompanyName
    FROM
        accounts.QOrganization        AS T
    INNER JOIN contacts.Person   AS P
               ON P.OrganizationIdentifier = T.OrganizationIdentifier

    INNER JOIN identities.[User] AS U
               ON U.UserIdentifier = P.UserIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [reports].[VPerson]
as
select U.UserIdentifier                       as UserIdentifier
     , P.PersonCode                           as PersonCode
     , U.Email                                as Email
     , U.EmailAlternate                       as EmailAlternate
     , U.FullName                             as FullName
     , U.FirstName                            as FirstName
     , U.MiddleName                           as MiddleName
     , U.LastName                             as LastName
     , P.Phone                                as Phone
     , U.PhoneMobile                          as PhoneMobile
     , P.PhoneOther                           as PhoneOther
     , P.LastAuthenticated                    as PersonLastAuthenticated
     , P.Created                              as PersonCreatedDate
     , Created.FullName                       as PersonCreatedBy
     , P.Modified                             as PersonModifiedDate
     , Modified.FullName                      as PersonModifiedBy
     , P.UserAccessGranted                    as PersonAccessGrantedDate
     , P.UserAccessGrantedBy                  as PersonAccessGrantedBy
     , P.Birthdate                            as PersonBirthdate
     , P.Gender                               as PersonGender
     , P.[Language]                           as PersonLanguage
     , case P.FirstLanguage
           when 'English' then
               'No'
           else
               'Yes'
       end                                    as PersonEnglishLanguageLearner
     , P.JobTitle                             as PersonJobTitle
     , P.JobsApproved                         as PersonJobsApproved
     , JobsApproved.FullName                  as PersonJobsApprovedBy
     , P.ConsentToShare                       as PersonConsentToShare
     , EmployerGroup.GroupName                as EmployerGroupName
     , EmployerGroup.GroupLabel               as EmployerGroupTag
     , EmployerGroup.GroupCode                as EmployerGroupCode
     , EmployerGroup.GroupCategory            as EmployerGroupCategory
     , EmployerGroupStatusItem.ItemName       as EmployerGroupStatus
     , EmployerGroupStatusItem.ItemIdentifier as EmployerGroupStatusItemIdentifier
     , EmployerGroup.GroupDescription         as EmployerGroupDescription
     , EmployerGroup.GroupIdentifier          as EmployerGroupIdentifier
     , P.EmergencyContactName                 as PersonEmergencyContactName
     , P.EmergencyContactPhone                as PersonEmergencyContactPhone
     , P.EmergencyContactRelationship         as PersonEmergencyContactRelationship
     , P.CredentialingCountry                 as PersonCredentialingCountry
     , MembershipStatusItem.ItemName          as PersonMembershipStatus
     , P.MemberStartDate                      as PersonMemberStartDate
     , P.MemberEndDate                        as PersonMemberEndDate
     , P.Region                               as PersonRegion
     , HomeAddress.Street1                    as HomeAddressStreet1
     , HomeAddress.Street2                    as HomeAddressStreet2
     , HomeAddress.City                       as HomeAddressCity
     , HomeAddress.Province                   as HomeAddressProvince
     , HomeAddress.PostalCode                 as HomeAddressPostalCode
     , HomeAddress.Country                    as HomeAddressCountry
     , WorkAddress.Street1                    as WorkAddressStreet1
     , WorkAddress.Street2                    as WorkAddressStreet2
     , WorkAddress.City                       as WorkAddressCity
     , WorkAddress.Province                   as WorkAddressProvince
     , WorkAddress.PostalCode                 as WorkAddressPostalCode
     , WorkAddress.Country                    as WorkAddressCountry
     , ShippingAddress.Street1                as ShippingAddressStreet1
     , ShippingAddress.Street2                as ShippingAddressStreet2
     , ShippingAddress.City                   as ShippingAddressCity
     , ShippingAddress.Province               as ShippingAddressProvince
     , ShippingAddress.PostalCode             as ShippingAddressPostalCode
     , ShippingAddress.Country                as ShippingAddressCountry
     , BillingAddress.Street1                 as BillingAddressStreet1
     , BillingAddress.Street2                 as BillingAddressStreet2
     , BillingAddress.City                    as BillingAddressCity
     , BillingAddress.Province                as BillingAddressProvince
     , BillingAddress.PostalCode              as BillingAddressPostalCode
     , BillingAddress.Country                 as BillingAddressCountry
     , U.Honorific                            as Honorific
     , U.ImageUrl                             as ImageURL
     , U.TimeZone                             as UserTimeZone
     , P.PhoneHome                            as PersonPhoneHome
     , P.PhoneWork                            as PersonPhoneWork
     , P.OrganizationIdentifier               as OrganizationIdentifier
from contacts.QPerson                    as P
     inner join identities.[User]        as U on U.UserIdentifier = P.UserIdentifier
     left join locations.[Address]       as HomeAddress on HomeAddress.AddressIdentifier = P.HomeAddressIdentifier
     left join locations.[Address]       as WorkAddress on WorkAddress.AddressIdentifier = P.WorkAddressIdentifier
     left join locations.[Address]       as ShippingAddress on ShippingAddress.AddressIdentifier = P.ShippingAddressIdentifier
     left join locations.[Address]       as BillingAddress on BillingAddress.AddressIdentifier = P.BillingAddressIdentifier
     left join identities.[User]         as Created on P.CreatedBy = Created.UserIdentifier
     left join identities.[User]         as Modified on P.ModifiedBy = Modified.UserIdentifier
     left join identities.[User]         as JobsApproved on P.JobsApprovedBy = JobsApproved.Email
     left join contacts.QGroup           as EmployerGroup on P.EmployerGroupIdentifier = EmployerGroup.GroupIdentifier
     left join utilities.TCollectionItem as MembershipStatusItem on P.MembershipStatusItemIdentifier = MembershipStatusItem.ItemIdentifier
     left join utilities.TCollectionItem as EmployerGroupStatusItem on EmployerGroupStatusItem.ItemIdentifier = EmployerGroup.GroupStatusItemIdentifier
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [learning].[TProgramCategory](
	[ProgramIdentifier] [uniqueidentifier] NOT NULL,
	[ItemIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_TProgramCategory] PRIMARY KEY NONCLUSTERED 
(
	[ProgramIdentifier] ASC,
	[ItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[TProgram](
	[GroupIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ProgramIdentifier] [uniqueidentifier] NOT NULL,
	[ProgramCode] [varchar](100) NULL,
	[ProgramDescription] [varchar](500) NULL,
	[ProgramName] [varchar](500) NOT NULL,
	[ProgramTag] [varchar](40) NULL,
	[NotificationStalledTriggerDay] [int] NULL,
	[NotificationStalledReminderLimit] [int] NULL,
	[NotificationStalledAdministratorMessageIdentifier] [uniqueidentifier] NULL,
	[NotificationStalledLearnerMessageIdentifier] [uniqueidentifier] NULL,
	[NotificationCompletedAdministratorMessageIdentifier] [uniqueidentifier] NULL,
	[NotificationCompletedLearnerMessageIdentifier] [uniqueidentifier] NULL,
	[ProgramSlug] [varchar](100) NULL,
	[ProgramImage] [varchar](200) NULL,
	[ProgramIcon] [varchar](30) NULL,
	[CompletionTaskIdentifier] [uniqueidentifier] NULL,
	[AchievementIdentifier] [uniqueidentifier] NULL,
	[AchievementWhenChange] [varchar](20) NULL,
	[AchievementWhenGrade] [varchar](20) NULL,
	[AchievementThenCommand] [varchar](20) NULL,
	[AchievementElseCommand] [varchar](20) NULL,
	[AchievementFixedDate] [datetimeoffset](7) NULL,
	[ProgramType] [varchar](30) NULL,
	[CatalogIdentifier] [uniqueidentifier] NULL,
	[IsHidden] [bit] NOT NULL,
	[CatalogSequence] [int] NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TProgram] PRIMARY KEY CLUSTERED 
(
	[ProgramIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [records].[VCatalogProgram]
as
select
    t.CatalogIdentifier
    ,t.CatalogName
    ,t.IsHidden as CatalogIsHidden
    ,i.ItemIdentifier as ProgramCategoryItemIdentifier
    ,i.ItemName as ProgramCategory
    ,program.Created as ProgramCreated
    ,program.ProgramIdentifier
    ,program.ProgramImage
    ,program.Modified as ProgramModified
    ,program.ProgramName
    ,program.ProgramSlug
    ,program.ProgramDescription
    ,o.OrganizationCode
    ,o.OrganizationIdentifier
    ,o.CompanyName as OrganizationName
from
    records.TProgram as program
    inner join learning.TCatalog as t on program.CatalogIdentifier = t.CatalogIdentifier
    inner join accounts.QOrganization as o on o.OrganizationIdentifier = program.OrganizationIdentifier
    inner join learning.TProgramCategory as g on g.ProgramIdentifier = program.ProgramIdentifier
    inner join utilities.TCollectionItem as i on g.ItemIdentifier = i.ItemIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [accounts].[QOrganizationHierarchy]
as
with Hierarchy
as (
   select OrganizationIdentifier
        , ParentOrganizationIdentifier
        , cast(OrganizationCode as nvarchar(max)) as PathCode
        , 0                                       as PathDepth
        , CompanyName                             as OrganizationName
        , AccountClosed
   from accounts.QOrganization
   where ParentOrganizationIdentifier is null OR ParentOrganizationIdentifier = '00000000-0000-0000-0000-000000000000'
   union all
   select Child.OrganizationIdentifier
        , Child.ParentOrganizationIdentifier
        , cast(Parent.PathCode + '/' + Child.OrganizationCode as nvarchar(max))
        , Parent.PathDepth + 1
        , Child.CompanyName
        , Child.AccountClosed
   from accounts.QOrganization as Child
        inner join Hierarchy   as Parent on Child.ParentOrganizationIdentifier = Parent.OrganizationIdentifier)
select *
     , substring('######', 1, PathDepth + 1) as PathIndent
from Hierarchy;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [workflow].[TCaseStatus](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[CaseType] [varchar](50) NOT NULL,
	[StatusIdentifier] [uniqueidentifier] NOT NULL,
	[StatusName] [varchar](50) NOT NULL,
	[StatusSequence] [int] NOT NULL,
	[StatusCategory] [varchar](120) NOT NULL,
	[ReportCategory] [varchar](10) NULL,
	[StatusDescription] [varchar](200) NULL,
 CONSTRAINT [PK_TCaseStatus] PRIMARY KEY CLUSTERED 
(
	[StatusIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [issues].[TIssueStatus]
AS
SELECT 
    OrganizationIdentifier
   ,CaseType AS IssueType
   ,StatusIdentifier
   ,StatusName
   ,StatusSequence
   ,StatusCategory
   ,ReportCategory
   ,StatusDescription
FROM
    workflow.TCaseStatus;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [accounts].[TUserSessionCache](
	[SessionStarted] [datetimeoffset](7) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[CacheIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TUserSessionCache] PRIMARY KEY CLUSTERED 
(
	[CacheIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [accounts].[TUserSessionCacheSummary]
as
select max(SessionStarted) as SessionStarted
     , S.OrganizationIdentifier
     , S.UserIdentifier
     , T.OrganizationCode
     , T.CompanyName
     , T.CompanyTitle
from accounts.TUserSessionCache        as S
     inner join accounts.QOrganization as T on T.OrganizationIdentifier = S.OrganizationIdentifier
group by S.OrganizationIdentifier
       , S.UserIdentifier
       , T.OrganizationCode
       , T.CompanyName
       , T.CompanyTitle;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sites].[QSite](
	[SiteIdentifier] [uniqueidentifier] NOT NULL,
	[SiteDomain] [varchar](256) NOT NULL,
	[SiteTitle] [nvarchar](128) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[LastChangeTime] [datetimeoffset](7) NULL,
	[LastChangeType] [varchar](100) NULL,
	[LastChangeUser] [varchar](100) NULL,
 CONSTRAINT [PK_QSite] PRIMARY KEY CLUSTERED 
(
	[SiteIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sites].[QPage](
	[PageIdentifier] [uniqueidentifier] NOT NULL,
	[PageType] [varchar](64) NOT NULL,
	[Title] [nvarchar](128) NOT NULL,
	[Sequence] [int] NOT NULL,
	[AuthorDate] [datetimeoffset](7) NULL,
	[AuthorName] [varchar](100) NULL,
	[ContentControl] [varchar](100) NULL,
	[NavigateUrl] [varchar](500) NULL,
	[IsHidden] [bit] NOT NULL,
	[ContentLabels] [varchar](200) NULL,
	[PageIcon] [varchar](30) NULL,
	[Hook] [varchar](100) NULL,
	[IsNewTab] [bit] NOT NULL,
	[PageSlug] [varchar](100) NULL,
	[SiteIdentifier] [uniqueidentifier] NULL,
	[ParentPageIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[LastChangeTime] [datetimeoffset](7) NULL,
	[LastChangeType] [varchar](100) NULL,
	[LastChangeUser] [varchar](100) NULL,
	[IsAccessDenied] [bit] NOT NULL,
	[ObjectType] [varchar](100) NULL,
	[ObjectIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QPage] PRIMARY KEY CLUSTERED 
(
	[PageIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [sites].[VWebPageHierarchy]
AS
WITH Hierarchy
AS (
   SELECT SiteIdentifier AS WebSiteIdentifier,
          PageIdentifier AS WebPageIdentifier,
          ParentPageIdentifier AS ParentWebPageIdentifier,
          OrganizationIdentifier,
          CAST(PageIdentifier AS VARCHAR(MAX)) AS PathIdentifier,
          0 AS PathDepth,
          Title AS WebPageTitle,
          PageType AS WebPageType,
          PageIdentifier AS RootWebPageIdentifier,
          CAST(FORMAT(Sequence, '00') AS VARCHAR(MAX)) AS PathSequence,
          PageSlug AS PageSlug,
          CAST(PageSlug AS VARCHAR(MAX)) AS PathUrl
   FROM sites.QPage WITH (NOLOCK)
   WHERE ParentPageIdentifier IS NULL
   UNION ALL
   SELECT Child.SiteIdentifier AS WebSiteIdentifier,
          Child.PageIdentifier AS WebPageIdentifier,
          Child.ParentPageIdentifier AS ParentWebPageIdentifier,
          Child.OrganizationIdentifier,
          CAST(Hierarchy.PathIdentifier + ',' + CAST(Child.PageIdentifier AS VARCHAR(MAX)) AS VARCHAR(MAX)),
          Hierarchy.PathDepth + 1,
          Child.Title,
          Child.PageType AS WebPageType,
          Hierarchy.RootWebPageIdentifier,
          CAST(Hierarchy.PathSequence + ' | ' + CAST(FORMAT(Child.Sequence, '00') AS VARCHAR(MAX)) AS VARCHAR(MAX)),
          Child.PageSlug,
          CAST(Hierarchy.PathUrl AS VARCHAR(MAX)) + '/' + CAST(Child.PageSlug AS VARCHAR(MAX))
   FROM Hierarchy
       INNER JOIN sites.QPage AS Child WITH (NOLOCK)
           ON Child.ParentPageIdentifier = Hierarchy.WebPageIdentifier)
SELECT Hierarchy.PathDepth,
       SUBSTRING('######', 1, PathDepth + 1) AS PathIndent,
       Hierarchy.PathIdentifier,
       Hierarchy.PathUrl,
       Hierarchy.PathSequence,
       Hierarchy.OrganizationIdentifier,
       Hierarchy.WebPageIdentifier,
       Hierarchy.PageSlug,
       Hierarchy.WebPageTitle,
       Hierarchy.WebPageType,
       S.SiteDomain AS WebSiteDomain,
       S.SiteIdentifier AS WebSiteIdentifier,
		CAST(
			CASE
				WHEN LEN(S.SiteDomain) - LEN(REPLACE(S.SiteDomain, '.', '')) = 1 THEN
					0
				ELSE
					1
			END AS BIT
		) AS SiteIsPortal,
       Hierarchy.ParentWebPageIdentifier,
       Hierarchy.RootWebPageIdentifier
FROM Hierarchy
    INNER JOIN sites.QSite AS S
        ON S.SiteIdentifier = Hierarchy.WebSiteIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [sites].[VSitemap] 
AS 
SELECT h.OrganizationIdentifier,
       h.WebSiteDomain AS SiteDomain,
       h.WebSiteIdentifier AS SiteIdentifier,
       h.ParentWebPageIdentifier AS FolderIdentifier,
       h.WebPageIdentifier AS PageIdentifier,
       h.WebPageType AS PageType,
       h.WebPageTitle AS PageTitle,
       SPACE(h.PathDepth * 4) + h.WebPageTitle AS PageTitleIndented,
	   h.PageSlug,
	   SPACE(h.PathDepth * 4) + h.PageSlug AS PageSlugIndented,
       h.PathUrl,
	   h.PathSequence,
	   p.IsHidden AS PageIsHidden

FROM sites.VWebPageHierarchy AS h
INNER JOIN sites.QPage AS p ON p.PageIdentifier = h.WebPageIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [messages].[QMessage](
	[ContentHtml] [nvarchar](max) NULL,
	[ContentText] [nvarchar](max) NULL,
	[MessageAttachments] [varchar](max) NULL,
	[MessageIdentifier] [uniqueidentifier] NOT NULL,
	[MessageName] [varchar](256) NOT NULL,
	[MessageTitle] [nvarchar](256) NULL,
	[MessageType] [varchar](64) NOT NULL,
	[SenderIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyFormIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[IsDisabled] [bit] NOT NULL,
	[LastChangeTime] [datetimeoffset](7) NOT NULL,
	[LastChangeType] [varchar](100) NOT NULL,
	[LastChangeUser] [uniqueidentifier] NOT NULL,
	[AutoBccSubscribers] [bit] NOT NULL,
 CONSTRAINT [PK_QMessage] PRIMARY KEY CLUSTERED 
(
	[MessageIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [messages].[QSubscriberUser](
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[MessageIdentifier] [uniqueidentifier] NOT NULL,
	[Subscribed] [datetimeoffset](7) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QSubscriberUser] PRIMARY KEY CLUSTERED 
(
	[UserIdentifier] ASC,
	[MessageIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [messages].[XSubscriberUser]
AS
SELECT SU.MessageIdentifier
     , SU.UserIdentifier
     , SU.Subscribed

     , U.FirstName                                                                                                                 AS UserFirstName
     , U.LastName                                                                                                                  AS UserLastName
     , U.FullName                                                                                                                  AS UserFullName

     , U.Email                                                                                                                     AS UserEmail
     , CAST(( SELECT COUNT(*) FROM contacts.Person AS P WHERE P.UserIdentifier = U.UserIdentifier AND P.EmailEnabled = 1 ) AS BIT) AS UserEmailEnabled
     , CAST(0 AS BIT)                                                                                                              AS UserMarketingEmailEnabled
     , NULL                                                                                                                        AS UserEmailAlternate
     , CAST(0 AS BIT)                                                                                                              AS UserEmailAlternateEnabled

     , 'en'                                                                                                                        AS PersonLanguage
     , CAST(NULL AS VARCHAR(20))                                                                                                   AS PersonCode

     , M.MessageName
     , M.OrganizationIdentifier                                                                                                    AS MessageOrganizationIdentifier
     , M.MessageTitle

FROM messages.QSubscriberUser     AS SU
     INNER JOIN identities.[User] AS U ON U.UserIdentifier = SU.UserIdentifier
     INNER JOIN messages.QMessage AS M ON M.MessageIdentifier = SU.MessageIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [reports].[VGroupPerson]
as
select P.FullName AS UserFullName
     , P.FirstName AS UserFirstName
     , P.LastName AS UserLastName
     , P.Email AS UserEmail
     , M.Assigned       as SubscriptionJoined
     , M.MembershipType as SubscriptionType
     , M.UserIdentifier
     , M.GroupIdentifier
from reports.VPerson                as P
     inner join contacts.Membership as M on M.UserIdentifier = P.UserIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [messages].[XSubscriberPerson]
AS
SELECT Subscriber.MessageIdentifier
     , Subscriber.UserIdentifier
     , Subscriber.Subscribed

     , U.FirstName                                          AS UserFirstName
     , U.LastName                                           AS UserLastName
     , U.FullName                                           AS UserFullName

     , U.Email                                              AS UserEmail
     , ISNULL(Person.EmailEnabled, CAST(0 AS BIT))          AS UserEmailEnabled
     , ISNULL(Person.MarketingEmailEnabled, CAST(0 AS BIT)) AS UserMarketingEmailEnabled
     , U.EmailAlternate                                     AS UserEmailAlternate
     , ISNULL(Person.EmailAlternateEnabled, CAST(0 AS BIT)) AS UserEmailAlternateEnabled

     , Person.[Language]                                    AS PersonLanguage
     , Person.PersonCode

     , M.MessageName
     , M.OrganizationIdentifier                             AS MessageOrganizationIdentifier
     , M.MessageTitle

FROM [messages].QSubscriberUser     AS Subscriber
     INNER JOIN identities.[User]   AS U ON U.UserIdentifier = Subscriber.UserIdentifier
     INNER JOIN [messages].QMessage AS M ON M.MessageIdentifier = Subscriber.MessageIdentifier
     LEFT JOIN contacts.Person      AS Person ON Person.UserIdentifier = U.UserIdentifier
                                                 AND Person.OrganizationIdentifier = M.OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QJournalSetup](
	[JournalSetupIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[FrameworkStandardIdentifier] [uniqueidentifier] NULL,
	[JournalSetupName] [varchar](200) NOT NULL,
	[JournalSetupCreated] [datetimeoffset](7) NOT NULL,
	[LastChangeTime] [datetimeoffset](7) NULL,
	[LastChangeType] [varchar](100) NULL,
	[LastChangeUser] [varchar](100) NULL,
	[EventIdentifier] [uniqueidentifier] NULL,
	[AchievementIdentifier] [uniqueidentifier] NULL,
	[IsValidationRequired] [bit] NOT NULL,
	[ValidatorMessageIdentifier] [uniqueidentifier] NULL,
	[LearnerMessageIdentifier] [uniqueidentifier] NULL,
	[LearnerAddedMessageIdentifier] [uniqueidentifier] NULL,
	[JournalSetupLocked] [datetimeoffset](7) NULL,
	[AllowLogbookDownload] [bit] NULL,
 CONSTRAINT [PK_QJournalSetup] PRIMARY KEY CLUSTERED 
(
	[JournalSetupIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [accounts].[TSender](
	[SenderIdentifier] [uniqueidentifier] NOT NULL,
	[SenderType] [varchar](50) NOT NULL,
	[SenderNickname] [varchar](100) NOT NULL,
	[SenderName] [varchar](100) NOT NULL,
	[SystemMailbox] [varchar](254) NOT NULL,
	[SenderEmail] [varchar](254) NOT NULL,
	[CompanyAddress] [varchar](100) NOT NULL,
	[CompanyCity] [varchar](50) NOT NULL,
	[CompanyPostalCode] [varchar](20) NULL,
	[CompanyCountry] [varchar](50) NOT NULL,
	[SenderEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_TSender] PRIMARY KEY CLUSTERED 
(
	[SenderIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [courses].[TCourse]
as
select
    CourseIdentifier
    ,CourseAsset
    ,CourseName
    ,CourseHook
    ,CourseCode
    ,CourseImage
    ,CoursePlatform
    ,GradebookIdentifier
    ,SourceIdentifier
    ,OrganizationIdentifier
    ,CreatedBy
    ,Created
    ,ModifiedBy
    ,Modified
    ,CourseIsHidden
    ,CourseLabel
    ,CourseProgram
    ,CourseLevel
    ,CourseUnit
    ,CourseDescription
    ,CourseStyle
    ,FrameworkStandardIdentifier as FrameworkIdentifier
    ,IsMultipleUnitsEnabled
    ,CourseSequence
    ,CourseIcon
    ,CourseSlug
    ,StalledToLearnerMessageIdentifier as CourseMessageStalledToLearner
    ,StalledToAdministratorMessageIdentifier as CourseMessageStalledToAdministrator
    ,CompletedToLearnerMessageIdentifier as CourseMessageCompletedToLearner
    ,CompletedToAdministratorMessageIdentifier as CourseMessageCompletedToAdministrator
    ,SendMessageStalledAfterDays
    ,SendMessageStalledMaxCount
    ,CompletionActivityIdentifier
    ,IsProgressReportEnabled
    ,OutlineWidth
    ,AllowDiscussion
    ,CatalogIdentifier
    ,CourseFlagColor
    ,CourseFlagText
	,Closed
from
    courses.QCourse
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [messages].[NotificationSender] 
as
with cte as (

      select O.OrganizationCode, G.MessageToAdminWhenEventVenueChanged as MessageIdentifier, 'MessageToAdminWhenEventVenueChanged' as MessageName, M.MessageType, 'contacts.QGroup' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from contacts.QGroup as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.MessageToAdminWhenEventVenueChanged inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.MessageToAdminWhenMembershipEnded as MessageIdentifier, 'MessageToAdminWhenMembershipEnded' as MessageName, M.MessageType, 'contacts.QGroup' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from contacts.QGroup as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.MessageToAdminWhenMembershipEnded inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.MessageToAdminWhenMembershipStarted as MessageIdentifier, 'MessageToAdminWhenMembershipStarted' as MessageName, M.MessageType, 'contacts.QGroup' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from contacts.QGroup as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.MessageToAdminWhenMembershipStarted inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.MessageToUserWhenMembershipEnded as MessageIdentifier, 'MessageToUserWhenMembershipEnded' as MessageName, M.MessageType, 'contacts.QGroup' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from contacts.QGroup as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.MessageToUserWhenMembershipEnded inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.MessageToUserWhenMembershipStarted as MessageIdentifier, 'MessageToUserWhenMembershipStarted' as MessageName, M.MessageType, 'contacts.QGroup' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from contacts.QGroup as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.MessageToUserWhenMembershipStarted inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.CourseMessageCompletedToAdministrator as MessageIdentifier, 'CourseMessageCompletedToAdministrator' as MessageName, M.MessageType, 'courses.TCourse' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from courses.TCourse as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.CourseMessageCompletedToAdministrator inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.CourseMessageCompletedToLearner as MessageIdentifier, 'CourseMessageCompletedToLearner' as MessageName, M.MessageType, 'courses.TCourse' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from courses.TCourse as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.CourseMessageCompletedToLearner inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.CourseMessageStalledToAdministrator as MessageIdentifier, 'CourseMessageStalledToAdministrator' as MessageName, M.MessageType, 'courses.TCourse' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from courses.TCourse as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.CourseMessageStalledToAdministrator inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.CourseMessageStalledToLearner as MessageIdentifier, 'CourseMessageStalledToLearner' as MessageName, M.MessageType, 'courses.TCourse' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from courses.TCourse as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.CourseMessageStalledToLearner inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.ProgressCompletedMessageIdentifier as MessageIdentifier, 'ProgressCompletedMessageIdentifier' as MessageName, M.MessageType, 'records.QGradeItem' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from records.QGradeItem as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.ProgressCompletedMessageIdentifier inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.LearnerAddedMessageIdentifier as MessageIdentifier, 'LearnerAddedMessageIdentifier' as MessageName, M.MessageType, 'records.QJournalSetup' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from records.QJournalSetup as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.LearnerAddedMessageIdentifier inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.LearnerMessageIdentifier as MessageIdentifier, 'LearnerMessageIdentifier' as MessageName, M.MessageType, 'records.QJournalSetup' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from records.QJournalSetup as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.LearnerMessageIdentifier inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.ValidatorMessageIdentifier as MessageIdentifier, 'ValidatorMessageIdentifier' as MessageName, M.MessageType, 'records.QJournalSetup' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from records.QJournalSetup as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.ValidatorMessageIdentifier inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.NotificationCompletedAdministratorMessageIdentifier as MessageIdentifier, 'NotificationCompletedAdministratorMessageIdentifier' as MessageName, M.MessageType, 'records.TProgram' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from records.TProgram as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.NotificationCompletedAdministratorMessageIdentifier inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.NotificationCompletedLearnerMessageIdentifier as MessageIdentifier, 'NotificationCompletedLearnerMessageIdentifier' as MessageName, M.MessageType, 'records.TProgram' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from records.TProgram as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.NotificationCompletedLearnerMessageIdentifier inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.NotificationStalledAdministratorMessageIdentifier as MessageIdentifier, 'NotificationStalledAdministratorMessageIdentifier' as MessageName, M.MessageType, 'records.TProgram' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from records.TProgram as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.NotificationStalledAdministratorMessageIdentifier inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.NotificationStalledLearnerMessageIdentifier as MessageIdentifier, 'NotificationStalledLearnerMessageIdentifier' as MessageName, M.MessageType, 'records.TProgram' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from records.TProgram as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.NotificationStalledLearnerMessageIdentifier inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.SurveyMessageInvitation as MessageIdentifier, 'SurveyMessageInvitation' as DatabaseColumn, M.MessageType,'surveys.QSurveyForm' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from surveys.QSurveyForm as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.SurveyMessageInvitation inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.SurveyMessageResponseCompleted as MessageIdentifier, 'SurveyMessageResponseCompleted' as DatabaseColumn, M.MessageType,'surveys.QSurveyForm' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from surveys.QSurveyForm as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.SurveyMessageResponseCompleted inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.SurveyMessageResponseConfirmed as MessageIdentifier, 'SurveyMessageResponseConfirmed' as DatabaseColumn, M.MessageType,'surveys.QSurveyForm' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from surveys.QSurveyForm as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.SurveyMessageResponseConfirmed inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier
union select O.OrganizationCode, G.SurveyMessageResponseStarted as MessageIdentifier, 'SurveyMessageResponseStarted' as DatabaseColumn, M.MessageType,'surveys.QSurveyForm' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from surveys.QSurveyForm as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join messages.QMessage as M on M.MessageIdentifier = G.SurveyMessageResponseStarted inner join accounts.TSender as S on S.SenderIdentifier = M.SenderIdentifier

union select O.OrganizationCode, G.MessageIdentifier, 'AchievementCredentialsExpiredToday' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'AchievementCredentialsExpiredToday'
union select O.OrganizationCode, G.MessageIdentifier, 'AchievementCredentialsExpiringInOneMonth' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'AchievementCredentialsExpiringInOneMonth'
union select O.OrganizationCode, G.MessageIdentifier, 'AchievementCredentialsExpiringInThreeMonths' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'AchievementCredentialsExpiringInThreeMonths'
union select O.OrganizationCode, G.MessageIdentifier, 'AchievementCredentialsExpiringInTwoMonths' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'AchievementCredentialsExpiringInTwoMonths'
union select O.OrganizationCode, G.MessageIdentifier, 'ApplicationAccessGranted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ApplicationAccessGranted'
union select O.OrganizationCode, G.MessageIdentifier, 'ApplicationAccessRequested' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ApplicationAccessRequested'
union select O.OrganizationCode, G.MessageIdentifier, 'AssessmentAttemptCompleted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'AssessmentAttemptCompleted'
union select O.OrganizationCode, G.MessageIdentifier, 'AssessmentAttemptStarted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'AssessmentAttemptStarted'
union select O.OrganizationCode, G.MessageIdentifier, 'AuthenticationAlarmTriggered' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'AuthenticationAlarmTriggered'
union select O.OrganizationCode, G.MessageIdentifier, 'CertificatePublicationConfirmation' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CertificatePublicationConfirmation'
union select O.OrganizationCode, G.MessageIdentifier, 'CertificatePublicationFailed' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CertificatePublicationFailed'
union select O.OrganizationCode, G.MessageIdentifier, 'CmdsBlogSubscriptionRequested' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CmdsBlogSubscriptionRequested'
union select O.OrganizationCode, G.MessageIdentifier, 'CmdsCollegeCertificationRequested' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CmdsCollegeCertificationRequested'
union select O.OrganizationCode, G.MessageIdentifier, 'CmdsCompetenciesExpired' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CmdsCompetenciesExpired'
union select O.OrganizationCode, G.MessageIdentifier, 'CmdsCompetencyChanged' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CmdsCompetencyChanged'
union select O.OrganizationCode, G.MessageIdentifier, 'CmdsCompetencyValidationRequested' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CmdsCompetencyValidationRequested'
union select O.OrganizationCode, G.MessageIdentifier, 'CmdsResourceChanged' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CmdsResourceChanged'
union select O.OrganizationCode, G.MessageIdentifier, 'CmdsTrainingRegistrationSubmitted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CmdsTrainingRegistrationSubmitted'
union select O.OrganizationCode, G.MessageIdentifier, 'CmdsUsersPendingApproval' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CmdsUsersPendingApproval'
union select O.OrganizationCode, G.MessageIdentifier, 'CourseCompleted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'CourseCompleted'
union select O.OrganizationCode, G.MessageIdentifier, 'DemoCandidateContactRequested' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'DemoCandidateContactRequested'
union select O.OrganizationCode, G.MessageIdentifier, 'DemoFastRegistrationSubmitted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'DemoFastRegistrationSubmitted'
union select O.OrganizationCode, G.MessageIdentifier, 'DemoJobApplicationSubmitted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'DemoJobApplicationSubmitted'
union select O.OrganizationCode, G.MessageIdentifier, 'EmployerGroupCreated' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'EmployerGroupCreated'
union select O.OrganizationCode, G.MessageIdentifier, 'EventVenueChanged' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'EventVenueChanged'
union select O.OrganizationCode, G.MessageIdentifier, 'ExamEventSchedulePublished' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ExamEventSchedulePublished'
union select O.OrganizationCode, G.MessageIdentifier, 'HelpRequested' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'HelpRequested'
union select O.OrganizationCode, G.MessageIdentifier, 'IecbcCandidateContactRequested' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'IecbcCandidateContactRequested'
union select O.OrganizationCode, G.MessageIdentifier, 'IecbcFastRegistrationSubmitted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'IecbcFastRegistrationSubmitted'
union select O.OrganizationCode, G.MessageIdentifier, 'IecbcJobApplicationSubmitted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'IecbcJobApplicationSubmitted'
union select O.OrganizationCode, G.MessageIdentifier, 'InvoicePaid' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'InvoicePaid'
union select O.OrganizationCode, G.MessageIdentifier, 'IssueOwnerChanged' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'IssueOwnerChanged'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA001' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA001'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA002' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA002'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA003' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA003'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA004' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA004'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA005' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA005'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA006' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA006'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA007' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA007'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA008' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA008'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA009' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA009'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA011' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA011'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA013' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA013'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA014' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA014'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA015' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA015'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA016' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA016'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA017' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA017'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA019' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA019'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA020' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA020'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA021' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA021'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA022' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA022'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA023' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA023'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA024' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'DirectAccess' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA024'
union select O.OrganizationCode, G.MessageIdentifier, 'ITA025' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'SmarterMail' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ITA025'
union select O.OrganizationCode, G.MessageIdentifier, 'LogbookModified' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'JournalUpdated'
union select O.OrganizationCode, G.MessageIdentifier, 'MembershipEnded' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'MembershipEnded'
union select O.OrganizationCode, G.MessageIdentifier, 'MembershipStarted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'MembershipStarted'
union select O.OrganizationCode, G.MessageIdentifier, 'PasswordChanged' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'PasswordChanged'
union select O.OrganizationCode, G.MessageIdentifier, 'PasswordResetRequested' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'PasswordResetRequested'
union select O.OrganizationCode, G.MessageIdentifier, 'PersonCodeNotEntered' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'PersonCodeNotEntered'
union select O.OrganizationCode, G.MessageIdentifier, 'PersonCommentFlagged' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'PersonCommentFlagged'
union select O.OrganizationCode, G.MessageIdentifier, 'ProgressCompleted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'ProgressCompleted'
union select O.OrganizationCode, G.MessageIdentifier, 'RegistrantContactInformationChanged' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'RegistrantContactInformationChanged'
union select O.OrganizationCode, G.MessageIdentifier, 'RegistrationComplete' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'RegistrationComplete'
union select O.OrganizationCode, G.MessageIdentifier, 'RegistrationInvitation' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'RegistrationInvitation'
union select O.OrganizationCode, G.MessageIdentifier, 'RegistrationInvitationExpired' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'RegistrationInvitationExpired'
union select O.OrganizationCode, G.MessageIdentifier, 'UnhandledExceptionIntercepted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'UnhandledExceptionIntercepted'
union select O.OrganizationCode, G.MessageIdentifier, 'UserAccountArchived' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'UserAccountArchived'
union select O.OrganizationCode, G.MessageIdentifier, 'UserAccountCreated' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'UserAccountCreated'
union select O.OrganizationCode, G.MessageIdentifier, 'UserAccountWelcomed' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'UserAccountWelcomed'
union select O.OrganizationCode, G.MessageIdentifier, 'UserEmailSubscriptionModified' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'UserEmailSubscriptionModified'
union select O.OrganizationCode, G.MessageIdentifier, 'UserEmailVerificationRequested' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'UserEmailVerificationRequested'
union select O.OrganizationCode, G.MessageIdentifier, 'UserOTPActivationCode' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'UserOTPActivationCode'
union select O.OrganizationCode, G.MessageIdentifier, 'UserRegistrationSubmitted' as MessageName, G.MessageType, 'messages.QMessage' as DatabaseTable, 'Mailgun' as ExpectedSender, S.SenderType as ActualSender from messages.QMessage as G inner join accounts.QOrganization as O on O.OrganizationIdentifier = G.OrganizationIdentifier inner join accounts.TSender as S on S.SenderIdentifier = G.SenderIdentifier where G.MessageName = 'UserRegistrationSubmitted'
)
select cte.OrganizationCode
     , cte.DatabaseTable
     , cte.MessageName
     , cte.MessageType
	 , cte.MessageIdentifier
     , cte.ExpectedSender
	 , cte.ActualSender
from CTE
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [contacts].[VMembership]
as
select Assigned
     , MembershipType
     , GroupIdentifier
     , UserIdentifier
from contacts.Membership;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[UserRole]
as
select [User].UserIdentifier
     , [User].Email     as UserEmail
     , QGroup.GroupIdentifier
     , QGroup.GroupName as GroupName
     , QGroup.GroupCode as GroupAbbreviation
from identities.[User]
     inner join contacts.VMembership on [User].UserIdentifier = VMembership.UserIdentifier
     inner join contacts.QGroup on VMembership.GroupIdentifier = QGroup.GroupIdentifier
where GroupType = 'Role'
      and QGroup.GroupName like 'CMDS %';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [banks].[QBank](
	[AssetNumber] [int] NOT NULL,
	[AttachmentCount] [int] NOT NULL,
	[BankIdentifier] [uniqueidentifier] NOT NULL,
	[BankLevel] [varchar](20) NULL,
	[BankName] [varchar](200) NOT NULL,
	[BankSize] [int] NOT NULL,
	[BankStatus] [varchar](20) NULL,
	[BankTitle] [varchar](200) NULL,
	[BankType] [varchar](20) NULL,
	[BankEdition] [varchar](20) NULL,
	[CommentCount] [int] NOT NULL,
	[FormCount] [int] NOT NULL,
	[OptionCount] [int] NOT NULL,
	[QuestionCount] [int] NOT NULL,
	[SetCount] [int] NOT NULL,
	[SpecCount] [int] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[FrameworkIdentifier] [uniqueidentifier] NULL,
	[DepartmentIdentifier] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[LastChangeTime] [datetimeoffset](7) NOT NULL,
	[LastChangeType] [varchar](100) NOT NULL,
	[LastChangeUser] [varchar](100) NOT NULL,
 CONSTRAINT [PK_QBank] PRIMARY KEY CLUSTERED 
(
	[BankIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [banks].[VBank]
AS
    SELECT
        B.AssetNumber
      , B.AttachmentCount
      , B.BankIdentifier
      , B.BankLevel
      , B.BankName
      , B.BankSize
      , B.BankStatus
      , B.BankTitle
      , B.BankType
      , B.BankEdition
      , B.CommentCount
      , B.FormCount
      , B.OptionCount
      , B.QuestionCount
      , B.SetCount
      , B.SpecCount
      , B.OrganizationIdentifier
      , B.FrameworkIdentifier
      , F.ContentTitle       AS FrameworkTitle
      , O.StandardIdentifier AS OccupationIdentifier
      , O.ContentTitle       AS OccupationTitle
    FROM
        banks.QBank              AS B
    LEFT JOIN standards.[Standard] AS F
              ON B.FrameworkIdentifier = F.StandardIdentifier

    LEFT JOIN standards.[Standard] AS O
              ON F.ParentStandardIdentifier = O.StandardIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [sites].[VAssessmentPage]
AS
SELECT p.PageIdentifier,
       p.PageIcon,
       p.IsHidden AS PageIsHidden,
       p.Title AS PageTitle,
       f.FormIdentifier,
       f.FormName,
       f.FormTitle,
       f.FormAsset,
       f.FormAssetVersion,
       f.FormCode,
       f.FormPublicationStatus,
       p.OrganizationIdentifier,
       f.BankIdentifier
FROM sites.QPage AS p
    INNER JOIN banks.QBankForm AS f
        ON p.ObjectIdentifier = f.FormIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [events].[QEventAttendee](
	[EventIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[AttendeeRole] [varchar](200) NOT NULL,
	[Assigned] [datetimeoffset](7) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QEventAttendee] PRIMARY KEY CLUSTERED 
(
	[EventIdentifier] ASC,
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [events].[QEventAssessmentForm](
	[EventIdentifier] [uniqueidentifier] NOT NULL,
	[FormIdentifier] [uniqueidentifier] NOT NULL,
	[BankIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QEventAssessmentForm] PRIMARY KEY CLUSTERED 
(
	[EventIdentifier] ASC,
	[FormIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [registrations].[QAccommodation](
	[AccommodationIdentifier] [uniqueidentifier] NOT NULL,
	[RegistrationIdentifier] [uniqueidentifier] NOT NULL,
	[AccommodationType] [varchar](50) NOT NULL,
	[AccommodationName] [varchar](100) NULL,
	[TimeExtension] [int] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QAccommodation] PRIMARY KEY CLUSTERED 
(
	[AccommodationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [reports].[VExamEventSchedule]
AS
    WITH CTE AS (
                SELECT
                           F.FormIdentifier
                         , F.BankLevelType
                         , B.BankType
                         , B.BankLevel
                         , B.OccupationTitle
                FROM
                           banks.QBankForm AS F
                INNER JOIN banks.VBank     AS B
                           ON B.BankIdentifier = F.BankIdentifier)
    SELECT
              A.OrganizationIdentifier
            , A.EventIdentifier
            , Venue.GroupName          AS VenueName
            , Venue.GroupOffice        AS VenueOffice
            , (
                  SELECT
                            STRING_AGG(CTE.OccupationTitle, '; ') WITHIN GROUP (ORDER BY CTE.OccupationTitle)
                  FROM
                            CTE
                  LEFT JOIN events.QEventAssessmentForm AS EF
                            ON EF.EventIdentifier = A.EventIdentifier
                  WHERE
                            CTE.FormIdentifier = EF.FormIdentifier
              )                        AS Trades
            , (
                  SELECT
                            STRING_AGG(CTE.BankLevelType, '; ') WITHIN GROUP (ORDER BY CTE.BankLevelType)
                  FROM
                            CTE
                  LEFT JOIN events.QEventAssessmentForm AS EF
                            ON EF.EventIdentifier = A.EventIdentifier
                  WHERE
                            CTE.FormIdentifier = EF.FormIdentifier
              )                        AS FormBankTypes
            , (
                  SELECT
                            STRING_AGG(CTE.BankLevel, '; ') WITHIN GROUP (ORDER BY CTE.BankLevel)
                  FROM
                            CTE
                  LEFT JOIN events.QEventAssessmentForm AS EF
                            ON EF.EventIdentifier = A.EventIdentifier
                  WHERE
                            CTE.FormIdentifier = EF.FormIdentifier
              )                        AS FormBankLevels
            , A.EventClassCode
            , A.EventFormat
            , A.EventScheduledStart
            , A.EventSchedulingStatus
            , (
                  SELECT
                      COUNT(*)
                  FROM
                      registrations.QRegistration AS R
                  WHERE
                      R.EventIdentifier = A.EventIdentifier
                      AND R.EligibilityStatus LIKE 'Eligible%'
              )                        AS CandidateCount
            , A.CapacityMaximum
            , (
                  SELECT
                      STRING_AGG(R.EligibilityStatus, '; ') WITHIN GROUP (ORDER BY R.EligibilityStatus)
                  FROM
                      registrations.QRegistration AS R
                  WHERE
                      R.EventIdentifier = A.EventIdentifier
              )                        AS Eligibility
            , (
                  SELECT
                      COUNT(*)
                  FROM
                      events.QEventAttendee Attendee
                  WHERE
                      Attendee.AttendeeRole LIKE '%Invigilator%'
                      AND Attendee.EventIdentifier = A.EventIdentifier
              )                        AS InvigilatorCount
            , (
                  SELECT
                             COUNT(*) AS AccommodationCount
                  FROM
                             registrations.QAccommodation AS A
                  INNER JOIN registrations.QRegistration  AS R
                             ON R.RegistrationIdentifier = A.RegistrationIdentifier
                  WHERE
                             R.EligibilityStatus LIKE 'Eligible%'
              )                        AS AccommodationCount
            , CAST('' AS VARCHAR(100)) AS AccommodationSummary
            , A.InvigilatorMinimum
            , VenueAddress.Street1     AS PhysicalAddress
            , VenueAddress.City        AS PhysicalCity
            , VenueAddress.Province    AS PhysicalProvince
            , A.VenueRoom
            , A.EventNumber
            , A.EventBillingType
    FROM
              events.QEvent AS A
    LEFT JOIN contacts.QGroup     AS Venue
              ON A.VenueLocationIdentifier = Venue.GroupIdentifier

    LEFT JOIN contacts.QGroupAddress    AS VenueAddress
              ON Venue.GroupIdentifier = VenueAddress.GroupIdentifier AND VenueAddress.AddressType = 'Physical'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[Competency]
as
select StandardIdentifier as CompetencyStandardIdentifier
     , ContentSummary     as Knowledge
     , Code               as Number
     , SourceDescriptor   as NumberOld
     , CreditHours        as ProgramHours
     , ContentDescription as Skills
     , ContentTitle       as Summary
     , ContentTitle       as Title
     , IsHidden           as IsDeleted
     , CreatedBy
     , Created
     , ModifiedBy
     , Modified
     , StandardIdentifier
from standards.[Standard]
     inner join accounts.QOrganization as T on T.OrganizationIdentifier = Standard.OrganizationIdentifier
where StandardType = 'Competency';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandardAchievement](
	[StandardIdentifier] [uniqueidentifier] NOT NULL,
	[AchievementIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QStandardAchievement] PRIMARY KEY CLUSTERED 
(
	[StandardIdentifier] ASC,
	[AchievementIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [achievements].[TAchievementStandard]
AS
SELECT
    AchievementIdentifier
   ,StandardIdentifier
   ,OrganizationIdentifier
FROM
    [standard].QStandardAchievement
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[VCmdsAchievementCompetency]
as
select XY.StandardIdentifier   as CompetencyStandardIdentifier
     , XY.AchievementIdentifier
     , Competency.Code         as Number
     , Competency.ContentTitle as Summary
     , Competency.IsHidden     as IsDeleted
from achievements.TAchievementStandard    as XY
     inner join standards.Standard        as Competency on Competency.StandardIdentifier = XY.StandardIdentifier
                                                           and Competency.StandardType = 'Competency'
     inner join accounts.QOrganization    as T on T.OrganizationIdentifier = Competency.OrganizationIdentifier
     inner join achievements.QAchievement as Achievement on Achievement.AchievementIdentifier = XY.AchievementIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [databases].[VTable]
as
select SYSTBL.schema_id              as SchemaID
     , schema_name(SYSTBL.schema_id) as SchemaName
     , SYSTBL.object_id              as TableID
     , SYSTBL.name                   as TableName
     , SYSTBL.max_column_id_used     as ColumnCount
     , cast(case SINDX_1.index_id
                when 1 then
                    1
                else
                    0
            end as bit)              as HasClusteredIndex
     , coalesce((
                    select sum(rows)
                    from sys.partitions as SPART
                    where (object_id = SYSTBL.object_id)
                          and (index_id < 2)
                )
              , 0
               )                     as [RowCount]
     , 0.0                           as IndexSizeKB
     , 0.0                           as DataSizeKB
     , SYSTBL.create_date            as DateCreated
     , SYSTBL.modify_date            as DateModified
from sys.tables             as SYSTBL
     inner join sys.indexes as SINDX_1 on SINDX_1.object_id = SYSTBL.object_id
                                          and SINDX_1.index_id < 2;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [databases].[VTableColumn]
AS
SELECT T.SchemaID,
       T.SchemaName,
       T.TableID,
       T.TableName,
       COLUMN_NAME AS ColumnName,
       DATA_TYPE AS DataType,
       CHARACTER_MAXIMUM_LENGTH AS MaximumLength,
       CAST(CASE
                WHEN IS_NULLABLE = 'YES' THEN
                    0
                WHEN IS_NULLABLE = 'NO' THEN
                    1
                ELSE
                    NULL
            END AS BIT) AS IsRequired,
       CAST(CASE
                WHEN COLUMNPROPERTY(OBJECT_ID(T.SchemaName + '.' + T.TableName), COLUMN_NAME, 'IsIdentity') = 1 THEN
                    1
                ELSE
                    0
            END AS BIT) AS IsIdentity,
       C.ORDINAL_POSITION AS OrdinalPosition,
       C.COLUMN_DEFAULT AS DefaultValue
FROM INFORMATION_SCHEMA.COLUMNS AS C
    INNER JOIN databases.VTable AS T
        ON C.TABLE_NAME = T.TableName
           AND C.TABLE_SCHEMA = T.SchemaName
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[VCmdsAchievement] AS
    WITH CTE AS ( SELECT A.AchievementIdentifier,
                         A.AchievementDescription,
                         A.AchievementLabel,
                         A.AchievementTitle,
                         A.AchievementIsEnabled,
                         A.AchievementAllowSelfDeclared,
                         CASE WHEN A.ExpirationLifetimeUnit = 'Month'
                                  THEN A.ExpirationLifetimeQuantity
                              ELSE NULL END                                                      AS ValidForCount,
                         CASE WHEN A.ExpirationLifetimeUnit = 'Month' THEN 'Month' ELSE NULL END AS ValidForUnit,
                         T.OrganizationIdentifier
                  FROM achievements.QAchievement AS A
                           INNER JOIN accounts.QOrganization AS T
                                      ON A.OrganizationIdentifier = T.OrganizationIdentifier
                  WHERE (A.AchievementDescription IS NULL OR A.AchievementDescription <> 'Hidden') )
    SELECT CTE.AchievementIdentifier,
           CTE.AchievementDescription,
           CTE.AchievementLabel,
           CTE.AchievementTitle,
           CTE.AchievementIsEnabled,
           CTE.AchievementAllowSelfDeclared,
           CTE.ValidForCount,
           CTE.ValidForUnit,
           CTE.OrganizationIdentifier,
           CASE
               WHEN OrganizationIdentifier = '8258CB0A-D1E8-4BC1-94B3-E70652503437'
                   THEN 'Enterprise'
               ELSE 'Organization'
               END AS Visibility
    FROM CTE;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandardCategory](
	[StandardIdentifier] [uniqueidentifier] NOT NULL,
	[CategoryIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ClassificationSequence] [int] NULL,
 CONSTRAINT [PK_QStandardCategory] PRIMARY KEY CLUSTERED 
(
	[StandardIdentifier] ASC,
	[CategoryIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [standards].[StandardClassification]
AS
SELECT
    CategoryIdentifier
   ,ClassificationSequence
   ,StandardIdentifier
   ,OrganizationIdentifier
FROM
    [standard].QStandardCategory
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[CompetencyCategory]
AS
SELECT Category.ItemName               AS CategoryName
     , C.Summary                       AS CompetencySummary
     , C.Number                        AS CompetencyNumber
     , C.CompetencyStandardIdentifier
     , C.Knowledge                     AS CompetencyKnowledge
     , C.Skills                        AS CompetencySkills
     , STRING_AGG('- ' + Achievement.AchievementTitle, CHAR(13) + CHAR(10)) WITHIN GROUP (ORDER BY Achievement.AchievementTitle) AS CompetencyAchievements
FROM custom_cmds.Competency                      AS C
     INNER JOIN standards.StandardClassification AS SC ON C.StandardIdentifier = SC.StandardIdentifier
     INNER JOIN utilities.TCollectionItem       AS Category ON Category.ItemIdentifier = SC.CategoryIdentifier
     LEFT JOIN(custom_cmds.VCmdsAchievementCompetency  AS AchievementCompetency
               INNER JOIN custom_cmds.VCmdsAchievement AS Achievement ON Achievement.AchievementIdentifier = AchievementCompetency.AchievementIdentifier)ON AchievementCompetency.CompetencyStandardIdentifier = C.CompetencyStandardIdentifier
WHERE C.IsDeleted = 0
GROUP BY C.CompetencyStandardIdentifier
       , Category.ItemName
       , C.Summary
       , C.Number
       , C.Knowledge
       , C.Skills;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [databases].[VSchema]
AS
SELECT schemas.schema_id AS SchemaID,
       name AS SchemaName,
       (
           SELECT COUNT(*)
           FROM sys.objects
           WHERE objects.schema_id = schemas.schema_id
       ) AS ObjectCount,
       (
           SELECT COUNT(*)
           FROM INFORMATION_SCHEMA.TABLES
           WHERE TABLE_SCHEMA = name
                 AND TABLE_TYPE = 'BASE TABLE'
       ) AS TableCount
FROM sys.schemas
WHERE name NOT LIKE 'db_%'
      AND name NOT IN ( 'guest', 'INFORMATION_SCHEMA', 'sys' );
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [databases].[VView]
AS
SELECT SchemaID,
       SchemaName,
       views.object_id AS ViewID,
       views.name AS ViewName,
       (
           SELECT COUNT(*)
           FROM sys.columns
           WHERE columns.object_id = OBJECT_ID(SchemaName + '.' + views.name)
       ) AS ColumnCount
FROM sys.views
    INNER JOIN databases.VSchema
        ON VIEWS.schema_id = VSchema.SchemaID
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [identities].[Department]
AS
SELECT GroupCreated
     , LastChangeTime
     , LastChangeUser
     , GroupCode             AS DepartmentCode
     , GroupDescription      AS DepartmentDescription
     , GroupLabel            AS DepartmentLabel
     , GroupName             AS DepartmentName
     , ParentGroupIdentifier AS DivisionIdentifier
     , ParentGroupIdentifier AS ParentDepartmentIdentifier
     , OrganizationIdentifier
     , GroupIdentifier       AS DepartmentIdentifier
FROM contacts.QGroup
WHERE GroupType = 'Department';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standards].[DepartmentProfileUser](
	[IsPrimary] [bit] NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[IsRecommended] [bit] NOT NULL,
	[IsInProgress] [bit] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[ProfileStandardIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_DepartmentProfileUser] PRIMARY KEY CLUSTERED 
(
	[DepartmentIdentifier] ASC,
	[ProfileStandardIdentifier] ASC,
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [custom_cmds].[UserProfile]
as
select Department.DepartmentIdentifier
  , DepartmentProfileUser.ProfileStandardIdentifier
  , DepartmentProfileUser.UserIdentifier
  , Department.OrganizationIdentifier
  , DepartmentProfileUser.IsPrimary
  , DepartmentProfileUser.IsRequired as IsComplianceRequired
  , case
        when DepartmentProfileUser.IsRecommended = 1 then
            'Required for Promotion'
        when DepartmentProfileUser.IsInProgress = 1 then
            'In Training'
        else
            null
    end        as CurrentStatus
from standards.DepartmentProfileUser
    inner join identities.Department on Department.DepartmentIdentifier = DepartmentProfileUser.DepartmentIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [databases].[VViewColumn]
AS
SELECT V.SchemaID,
       V.SchemaName,
       V.ViewID,
       V.ViewName,
       COLUMN_NAME AS ColumnName,
       DATA_TYPE AS DataType,
       CHARACTER_MAXIMUM_LENGTH AS MaximumLength,
       CAST(CASE
                WHEN IS_NULLABLE = 'YES' THEN
                    0
                WHEN IS_NULLABLE = 'NO' THEN
                    1
                ELSE
                    NULL
            END AS BIT) AS IsRequired,
       CAST(CASE
                WHEN COLUMNPROPERTY(OBJECT_ID(V.SchemaName + '.' + V.ViewName), COLUMN_NAME, 'IsIdentity') = 1 THEN
                    1
                ELSE
                    0
            END AS BIT) AS IsIdentity,
       C.ORDINAL_POSITION AS OrdinalPosition
FROM INFORMATION_SCHEMA.COLUMNS AS C
    INNER JOIN databases.VView AS V
        ON C.TABLE_NAME = V.ViewName
           AND C.TABLE_SCHEMA = V.SchemaName;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandardValidation](
	[StandardIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[StandardValidationIdentifier] [uniqueidentifier] NOT NULL,
	[ValidatorUserIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[Expired] [datetimeoffset](7) NULL,
	[IsValidated] [bit] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[Notified] [datetimeoffset](7) NULL,
	[SelfAssessmentDate] [datetimeoffset](7) NULL,
	[SelfAssessmentStatus] [varchar](50) NULL,
	[ValidationComment] [varchar](max) NULL,
	[ValidationDate] [datetimeoffset](7) NULL,
	[ValidationStatus] [varchar](50) NULL,
 CONSTRAINT [PK_QStandardValidation] PRIMARY KEY CLUSTERED 
(
	[StandardValidationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [standards].[StandardValidation]
AS
SELECT
    Expired
   ,IsValidated
   ,Modified
   ,Notified
   ,SelfAssessmentDate
   ,SelfAssessmentStatus
   ,StandardValidationIdentifier AS ValidationIdentifier
   ,ValidationComment
   ,ValidationDate
   ,ValidationStatus
   ,Created
   ,CreatedBy
   ,ModifiedBy
   ,StandardIdentifier
   ,UserIdentifier
   ,ValidatorUserIdentifier
   ,OrganizationIdentifier
FROM
    [standard].QStandardValidation
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[UserCompetency]
as
select V.StandardIdentifier as CompetencyStandardIdentifier
     , V.ValidationStatus
     , V.Expired            as ExpirationDate
     , V.IsValidated
     , V.ValidationComment
     , V.ValidationDate
     , V.SelfAssessmentDate
     , V.SelfAssessmentStatus
     , cast(0 as bit)       as IsModuleQuizCompleted
     , V.UserIdentifier
     , V.ValidatorUserIdentifier
     , V.Notified
from standards.StandardValidation      as V
     inner join standards.Standard     as S on V.StandardIdentifier = S.StandardIdentifier
                                               and S.StandardType = 'Competency'
     inner join accounts.QOrganization as T on T.OrganizationIdentifier = S.OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [databases].[VForeignKeyConstraint]
AS
SELECT OBJECT_SCHEMA_NAME(fk.parent_object_id) AS ForeignSchemaName,
       OBJECT_NAME(fk.parent_object_id) AS ForeignTableName,
       cpa.name AS ForeignColumnName,
       OBJECT_SCHEMA_NAME(fk.referenced_object_id) AS PrimarySchemaName,
       OBJECT_NAME(fk.referenced_object_id) AS PrimaryTableName,
       cref.name AS PrimaryColumnName,
       fk.name AS ConstraintName
FROM sys.foreign_keys fk
    INNER JOIN sys.foreign_key_columns fkc
        ON fkc.constraint_object_id = fk.object_id
    INNER JOIN sys.columns cpa
        ON fkc.parent_object_id = cpa.object_id
           AND fkc.parent_column_id = cpa.column_id
    INNER JOIN sys.columns cref
        ON fkc.referenced_object_id = cref.object_id
           AND fkc.referenced_column_id = cref.column_id
    INNER JOIN databases.VTable AS PrimaryTable
        ON OBJECT_SCHEMA_NAME(fk.referenced_object_id) = PrimaryTable.SchemaName
           AND OBJECT_NAME(fk.referenced_object_id) = PrimaryTable.TableName
    INNER JOIN databases.VTable AS ForeignTable
        ON OBJECT_SCHEMA_NAME(fk.parent_object_id) = ForeignTable.SchemaName
           AND OBJECT_NAME(fk.parent_object_id) = ForeignTable.TableName;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standards].[DepartmentProfileCompetency](
	[LifetimeMonths] [int] NULL,
	[IsCritical] [bit] NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyStandardIdentifier] [uniqueidentifier] NOT NULL,
	[ProfileStandardIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_DepartmentProfileCompetency] PRIMARY KEY CLUSTERED 
(
	[DepartmentIdentifier] ASC,
	[ProfileStandardIdentifier] ASC,
	[CompetencyStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [custom_cmds].[DepartmentProfileCompetency]
as
select DepartmentIdentifier
  , ProfileStandardIdentifier
  , CompetencyStandardIdentifier
  , case
        when IsCritical = 1 then
            'Critical'
        else
            'Non-Critical'
    end            as Criticality
  , LifetimeMonths as ValidForCount
  , case
        when LifetimeMonths is not null then
            'Months'
        else
            null
    end            as ValidForUnit
from standards.DepartmentProfileCompetency;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[UserCompetencyExpiration]
as
select min(   ValidForCount * (case
                                   when ValidForUnit = 'Years' then
                                       12
                                   else
                                       1
                               end
                              )
          ) as LifetimeInMonths
  , UserCompetency.UserIdentifier
  , UserCompetency.CompetencyStandardIdentifier
  , ExpirationDate
  , ValidationDate
  , ValidationStatus
  , UserCompetency.Notified
from custom_cmds.DepartmentProfileCompetency
    inner join contacts.Membership on Membership.GroupIdentifier = DepartmentProfileCompetency.DepartmentIdentifier
    inner join custom_cmds.UserCompetency on UserCompetency.CompetencyStandardIdentifier = DepartmentProfileCompetency.CompetencyStandardIdentifier
                                      and UserCompetency.UserIdentifier = Membership.UserIdentifier
    inner join custom_cmds.UserProfile on UserProfile.DepartmentIdentifier = DepartmentProfileCompetency.DepartmentIdentifier
                                   and UserProfile.UserIdentifier = Membership.UserIdentifier
where ValidForCount is not null
      and ValidForUnit in ( 'Years', 'Months' )
      and ExpirationDate is not null
group by UserCompetency.UserIdentifier
  , UserCompetency.CompetencyStandardIdentifier
  , ExpirationDate
  , ValidationDate
  , ValidationStatus
  , UserCompetency.Notified;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[TGroupPermission](
	[AllowExecute] [bit] NOT NULL,
	[AllowRead] [bit] NOT NULL,
	[AllowWrite] [bit] NOT NULL,
	[AllowCreate] [bit] NOT NULL,
	[AllowDelete] [bit] NOT NULL,
	[AllowAdministrate] [bit] NOT NULL,
	[AllowConfigure] [bit] NOT NULL,
	[PermissionMask] [int] NOT NULL,
	[PermissionGranted] [datetimeoffset](7) NULL,
	[PermissionGrantedBy] [uniqueidentifier] NULL,
	[ObjectIdentifier] [uniqueidentifier] NOT NULL,
	[ObjectType] [varchar](100) NOT NULL,
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[PermissionIdentifier] [uniqueidentifier] NOT NULL,
	[AllowTrialAccess] [bit] NOT NULL,
 CONSTRAINT [PK_TGroupPermission] PRIMARY KEY CLUSTERED 
(
	[PermissionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [contacts].[VEventGroupPermission]
as
select
    AllowExecute
    ,AllowRead
    ,AllowWrite
    ,AllowCreate
    ,AllowDelete
    ,AllowAdministrate
    ,AllowConfigure
    ,PermissionMask
    ,PermissionGranted
    ,PermissionGrantedBy
    ,ObjectIdentifier
    ,ObjectType
    ,GroupIdentifier
    ,OrganizationIdentifier
    ,PermissionIdentifier
    ,AllowTrialAccess
from
    contacts.TGroupPermission
where
    ObjectType = 'Event'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [databases].[VForeignKey]
AS
WITH Columns
AS (
   SELECT SchemaID AS ForeignSchemaID,
          SchemaName AS ForeignSchemaName,
          TableID AS ForeignTableID,
          TableName AS ForeignTableName,
          ColumnName AS ForeignColumnName,
          IsRequired AS ForeignColumnRequired,
          'Identifier' AS ForeignColumnPostfix
   FROM databases.VTableColumn
   WHERE ColumnName LIKE '%Identifier'
   UNION
   SELECT SchemaID,
          SchemaName,
          TableID,
          TableName,
          ColumnName,
          IsRequired,
          'Key'
   FROM databases.VTableColumn
   WHERE ColumnName LIKE '%Key')
SELECT c.ForeignSchemaID,
       c.ForeignSchemaName,
       c.ForeignTableID,
       c.ForeignTableName,
       c.ForeignColumnName,
       c.ForeignColumnRequired,
       t.SchemaID AS PrimarySchemaID,
       t.SchemaName AS PrimarySchemaName,
       t.TableID AS PrimaryTableID,
       t.TableName AS PrimaryTableName,
       t.TableName + c.ForeignColumnPostfix AS PrimaryColumnName,
       CASE
           WHEN fk.PrimaryColumnName IS NOT NULL THEN
               CAST(1 AS BIT)
           ELSE
               CAST(0 AS BIT)
       END AS IsEnforced,
       c.ForeignTableName + '_' + c.ForeignColumnName AS UniqueName,
       ROW_NUMBER() OVER (ORDER BY c.ForeignTableName, c.ForeignColumnName) AS RowNumber
FROM Columns AS c
    INNER JOIN databases.VTable AS t
        ON c.ForeignColumnName LIKE '%' + t.TableName + c.ForeignColumnPostfix
           AND
           (
               c.ForeignTableName <> t.TableName
               OR c.ForeignColumnName <> t.TableName + c.ForeignColumnPostfix COLLATE SQL_Latin1_General_CP1_CS_AS
           )
    LEFT JOIN databases.VForeignKeyConstraint AS fk
        ON fk.ForeignSchemaName = c.ForeignSchemaName
           AND fk.ForeignTableName = c.ForeignTableName
           AND fk.ForeignColumnName = c.ForeignColumnName
           AND fk.PrimaryTableName = t.TableName
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [banks].[QBankQuestionGradeItem](
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[FormIdentifier] [uniqueidentifier] NOT NULL,
	[GradeItemIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QBankQuestionGradeItem] PRIMARY KEY CLUSTERED 
(
	[QuestionIdentifier] ASC,
	[FormIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[QAttemptOption](
	[AttemptIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionSequence] [int] NOT NULL,
	[OptionKey] [int] NOT NULL,
	[OptionPoints] [decimal](7, 2) NULL,
	[OptionSequence] [int] NOT NULL,
	[OptionText] [nvarchar](max) NULL,
	[AnswerIsSelected] [bit] NULL,
	[OptionCutScore] [decimal](5, 4) NULL,
	[CompetencyItemIdentifier] [uniqueidentifier] NULL,
	[OptionIsTrue] [bit] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[OptionShape] [varchar](32) NULL,
	[OptionAnswerSequence] [int] NULL,
	[CompetencyItemLabel] [varchar](40) NULL,
	[CompetencyItemCode] [varchar](30) NULL,
	[CompetencyItemTitle] [varchar](300) NULL,
	[CompetencyAreaLabel] [varchar](40) NULL,
	[CompetencyAreaCode] [varchar](30) NULL,
	[CompetencyAreaTitle] [varchar](300) NULL,
	[CompetencyAreaIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QAttemptOption] PRIMARY KEY CLUSTERED 
(
	[AttemptIdentifier] ASC,
	[QuestionIdentifier] ASC,
	[OptionKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[QAttemptQuestion](
	[AttemptIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionPoints] [decimal](7, 2) NULL,
	[QuestionSequence] [int] NOT NULL,
	[QuestionText] [nvarchar](max) NULL,
	[AnswerOptionKey] [int] NULL,
	[AnswerOptionSequence] [int] NULL,
	[AnswerPoints] [decimal](7, 2) NULL,
	[CompetencyItemLabel] [varchar](40) NULL,
	[CompetencyItemCode] [varchar](30) NULL,
	[CompetencyItemTitle] [varchar](300) NULL,
	[CompetencyItemIdentifier] [uniqueidentifier] NULL,
	[CompetencyAreaLabel] [varchar](40) NULL,
	[CompetencyAreaCode] [varchar](30) NULL,
	[CompetencyAreaTitle] [varchar](300) NULL,
	[CompetencyAreaIdentifier] [uniqueidentifier] NULL,
	[QuestionType] [varchar](21) NOT NULL,
	[AnswerText] [nvarchar](max) NULL,
	[QuestionCutScore] [decimal](5, 4) NULL,
	[QuestionMatchDistractors] [nvarchar](max) NULL,
	[QuestionCalculationMethod] [varchar](32) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[ParentQuestionIdentifier] [uniqueidentifier] NULL,
	[PinLimit] [int] NULL,
	[HotspotImage] [varchar](512) NULL,
	[ShowShapes] [bit] NULL,
	[AnswerTimeLimit] [int] NULL,
	[AnswerAttemptLimit] [int] NULL,
	[AnswerRequestAttempt] [int] NULL,
	[AnswerFileIdentifier] [uniqueidentifier] NULL,
	[AnswerSolutionIdentifier] [uniqueidentifier] NULL,
	[QuestionTopLabel] [nvarchar](max) NULL,
	[QuestionBottomLabel] [nvarchar](max) NULL,
	[SectionIndex] [int] NULL,
	[AnswerSubmitAttempt] [int] NULL,
	[QuestionNumber] [int] NULL,
	[RubricRatingPoints] [varchar](512) NULL,
 CONSTRAINT [PK_QAttemptQuestion] PRIMARY KEY CLUSTERED 
(
	[AttemptIdentifier] ASC,
	[QuestionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [banks].[QBankQuestion](
	[BankIdentifier] [uniqueidentifier] NOT NULL,
	[BankIndex] [int] NOT NULL,
	[QuestionCode] [varchar](max) NULL,
	[QuestionDifficulty] [int] NULL,
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionLikeItemGroup] [varchar](100) NULL,
	[QuestionReference] [varchar](max) NULL,
	[QuestionTag] [varchar](100) NULL,
	[QuestionTaxonomy] [int] NULL,
	[QuestionText] [varchar](max) NULL,
	[CompetencyIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionCondition] [varchar](20) NULL,
	[QuestionFlag] [varchar](10) NULL,
	[SetIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionFirstPublished] [datetimeoffset](7) NULL,
	[QuestionSourceIdentifier] [uniqueidentifier] NULL,
	[QuestionSourceAssetNumber] [varchar](16) NULL,
	[QuestionAssetNumber] [varchar](16) NOT NULL,
	[LastChangeTime] [datetimeoffset](7) NULL,
	[LastChangeType] [varchar](100) NULL,
	[LastChangeUser] [varchar](100) NULL,
	[ParentQuestionIdentifier] [uniqueidentifier] NULL,
	[BankSubIndex] [int] NULL,
	[QuestionType] [varchar](21) NULL,
	[QuestionTags] [varchar](max) NULL,
	[RubricIdentifier] [uniqueidentifier] NULL,
	[QuestionPublicationStatus] [varchar](12) NULL,
 CONSTRAINT [PK_QBankQuestion] PRIMARY KEY CLUSTERED 
(
	[QuestionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandardConnection](
	[FromStandardIdentifier] [uniqueidentifier] NOT NULL,
	[ToStandardIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ConnectionType] [varchar](20) NOT NULL,
 CONSTRAINT [PK_QStandardConnection] PRIMARY KEY CLUSTERED 
(
	[FromStandardIdentifier] ASC,
	[ToStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [standards].[StandardConnection]
AS
SELECT
    ConnectionType
   ,FromStandardIdentifier
   ,ToStandardIdentifier
   ,OrganizationIdentifier
FROM
    [standard].QStandardConnection
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [assessments].[VPerformanceReport]
as
select
    a.AttemptIdentifier
    ,q.QuestionIdentifier
    ,a.OrganizationIdentifier
    ,a.FormIdentifier
    ,a.LearnerUserIdentifier
    ,area.StandardIdentifier as CompetencyAreaIdentifier
    ,area.ContentTitle as CompetencyAreaTitle
    ,area.StandardLabel as CompetencyAreaLabel
    ,alt_area.Alt_CompetencyAreaIdentifier
    ,alt_area.Alt_CompetencyAreaTitle
    ,alt_area.Alt_CompetencyAreaLabel
    ,q.QuestionSequence
    ,q.QuestionText
    ,isnull(b_parent.QuestionText, q.QuestionText) as ParentQuestionText
    ,isnull(b.QuestionCode, b_parent.QuestionCode) as QuestionCode
    ,isnull(b.QuestionTags, b_parent.QuestionTags) as QuestionTags
    ,f.FormClassificationInstrument
    ,isnull(isnull(p.ProgressPoints, q.AnswerPoints), 0) as Points
    ,isnull(isnull(p.ProgressMaxPoints, q.QuestionPoints), 0) as MaxPoints
    ,a.AttemptStarted
    ,a.AttemptGraded
    ,q.AnswerOptionKey
    ,q.AnswerOptionSequence
    ,o.OptionText as AnswerOptionText
    ,isnull(b_parent.QuestionType, b.QuestionType) as QuestionType
    ,f.FormName
from
    assessments.QAttempt as a with(nolock)
    inner join assessments.QAttemptQuestion as q with(nolock) on q.AttemptIdentifier = a.AttemptIdentifier
    inner join banks.QBankForm as f with(nolock) on f.FormIdentifier = a.FormIdentifier
    inner join banks.QBankQuestion as b with(nolock) on b.QuestionIdentifier = q.QuestionIdentifier
    inner join standards.Standard as c with(nolock) on c.StandardIdentifier = b.CompetencyIdentifier
    inner join standards.Standard as area with(nolock) on area.StandardIdentifier = c.ParentStandardIdentifier
    left join banks.QBankQuestion as b_parent with(nolock) on b_parent.QuestionIdentifier = b.ParentQuestionIdentifier
    left join assessments.QAttemptOption as o with(nolock) on o.AttemptIdentifier = a.AttemptIdentifier
                                                            and o.QuestionIdentifier = q.QuestionIdentifier
                                                            and o.OptionKey = q.AnswerOptionKey
    left join (
        banks.QBankQuestionGradeItem as i with(nolock)
        inner join records.QProgress as p with(nolock) on p.GradeItemIdentifier = i.GradeItemIdentifier
    ) on i.FormIdentifier = a.FormIdentifier
        and i.QuestionIdentifier = q.QuestionIdentifier
        and p.UserIdentifier = a.LearnerUserIdentifier
    outer apply (
        select top 1
            alt_area.StandardIdentifier as Alt_CompetencyAreaIdentifier
            ,alt_area.ContentTitle as Alt_CompetencyAreaTitle
            ,alt_area.StandardLabel as Alt_CompetencyAreaLabel
        from
            standards.StandardConnection as alt_conn with(nolock)
            inner join standards.Standard as alt_area with(nolock) on alt_area.StandardIdentifier = alt_conn.ToStandardIdentifier
        where
             alt_conn.FromStandardIdentifier = c.StandardIdentifier
             and alt_area.StandardType = 'Area'
    ) as alt_area
where
    a.AttemptGraded is not null
    and (b.QuestionTags is not null or b_parent.QuestionTags is not null)
    and (p.ProgressIsIgnored is null or p.ProgressIsIgnored = 0)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [achievements].[TAchievementOrganization](
	[AchievementIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TAchievementOrganization] PRIMARY KEY CLUSTERED 
(
	[AchievementIdentifier] ASC,
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandardOrganization](
	[StandardIdentifier] [uniqueidentifier] NOT NULL,
	[ConnectedOrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QStandardOrganization] PRIMARY KEY CLUSTERED 
(
	[StandardIdentifier] ASC,
	[ConnectedOrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [standards].[StandardOrganization]
AS

WITH cte AS 
(
  SELECT StandardIdentifier, OrganizationIdentifier FROM [standard].QStandardOrganization
  UNION
  SELECT StandardIdentifier, ConnectedOrganizationIdentifier FROM [standard].QStandardOrganization
)
SELECT DISTINCT StandardIdentifier, OrganizationIdentifier FROM cte;

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[BillableUser]
as
select u.UserIdentifier
	, T.OrganizationIdentifier
	, T.OrganizationCode
	, u.LastName
	, u.FirstName
	, u.FullName
	, lower(u.Email) as Email
	, u.Email as UserName
	, case 
		when P.UserAccessGranted is not null
			then cast(1 as bit)
		else cast(0 as bit)
		end as UserIsApproved
	, case 
		when P.UserAccessGranted is null
			then cast(1 as bit)
		else cast(0 as bit)
		end as UserIsDisabled
	, P.PhoneWork as Phone
	, HomeAddress.Street1 as Street
	, HomeAddress.City as City
	, HomeAddress.Province as Province
	, HomeAddress.PostalCode as PostalCode
	, T.CompanyTitle as CompanyName
	, CompanyEmployeeProfiles.ProfileCount
	, CompanyEmployeeResources.TimeSensitiveResourceCount
	, case 
		when ProfileCount > 0
			then 'A'
		when ProfileCount = 0
			and TimeSensitiveResourceCount > 0
			then 'B'
		else 'C'
		end as BillingClassification
from identities.[User] as u
inner join contacts.Person as P on P.UserIdentifier = u.UserIdentifier
inner join accounts.QOrganization as T on T.OrganizationIdentifier = P.OrganizationIdentifier
left join locations.Address as HomeAddress on P.HomeAddressIdentifier = HomeAddress.AddressIdentifier
outer apply (
	select count(*) as ProfileCount
	from standards.DepartmentProfileUser employeeProfiles
	inner join standards.StandardOrganization companyProfiles on companyProfiles.OrganizationIdentifier = T.OrganizationIdentifier
		and employeeProfiles.ProfileStandardIdentifier = companyProfiles.StandardIdentifier
	where employeeProfiles.UserIdentifier = u.UserIdentifier
	) as CompanyEmployeeProfiles
outer apply (
	select count(*) as TimeSensitiveResourceCount
	from achievements.QCredential as employeeResources
	inner join achievements.QAchievement as Achievement on Achievement.AchievementIdentifier = employeeResources.AchievementIdentifier
	inner join achievements.TAchievementOrganization companyResources on companyResources.OrganizationIdentifier = T.OrganizationIdentifier
		and companyResources.AchievementIdentifier = employeeResources.AchievementIdentifier
	where employeeResources.UserIdentifier = u.UserIdentifier
		and Achievement.AchievementLabel = 'Time-Sensitive Safety Certificate'
	) as CompanyEmployeeResources
where u.UtcArchived is null
	and P.UserAccessGranted is not null
	and T.AccountClosed is null
	and T.OrganizationCode not in ('abc', 'acme', 'archive', 'default', 'devon-usa', 'insite')
	and exists (
		select *
		from contacts.Membership as M
		inner join contacts.QGroup as G on M.GroupIdentifier = G.GroupIdentifier
		where M.MembershipType = 'Department'
			and u.UserIdentifier = M.UserIdentifier
			and G.OrganizationIdentifier = T.OrganizationIdentifier
		);
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[BillableUserSummary]
as
select (
		select COUNT(distinct OrganizationIdentifier)
		from custom_cmds.BillableUser as x
		where x.UserIdentifier = users.UserIdentifier
			and x.BillingClassification = users.BillingClassification
		) as CompanyCount
	, users.UserIdentifier
	, users.OrganizationIdentifier
	, users.OrganizationCode
	, users.LastName
	, users.FirstName
	, users.FullName
	, users.Email
	, users.UserName
	, users.UserIsApproved
	, users.UserIsDisabled
	, users.Phone
	, users.Street
	, users.City
	, users.Province
	, users.PostalCode
	, users.CompanyName
	, users.ProfileCount
	, users.TimeSensitiveResourceCount
	, users.BillingClassification
from custom_cmds.BillableUser users;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [contacts].[RoleSummary]
as
select GroupContact.GroupIdentifier
     , PersonContact.UserIdentifier
     , GroupContact.OrganizationIdentifier as GroupOrganizationIdentifier
     , GroupContact.GroupType
     , GroupContact.GroupName
     , MembershipType
     , PersonContact.FullName              as UserFullName
     , PersonContact.FirstName             as UserFirstName
     , PersonContact.LastName              as UserLastName
     , PersonContact.Email                 as UserEmail
     , case
           when PersonContact.UserPasswordHash is null then
               1
           else
               0
       end                                 as UserHasPassword
     , cast((case
                 when PersonContact.UtcArchived is null then
                     0
                 else
                     1
             end
            ) as bit)                      as UserIsArchived
     , GroupOrganization.OrganizationCode  as GroupOrganizationCode
     , GroupOrganization.CompanyName       as GroupOrganizationName
     , GroupOrganization.CompanyTitle      as GroupOrganizationTitle
     , Membership.Assigned                 as RoleAssigned
from contacts.Membership                     as Membership
     inner join contacts.QGroup        as GroupContact on GroupContact.GroupIdentifier = Membership.GroupIdentifier
     inner join identities.[User]      as PersonContact on PersonContact.UserIdentifier = Membership.UserIdentifier
     inner join accounts.QOrganization as GroupOrganization on GroupContact.OrganizationIdentifier = GroupOrganization.OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [contacts].[VGroup]
as
select G.GroupCode
     , G.GroupIdentifier
     , G.GroupName
     , G.GroupOffice
     , G.GroupPhone
     , (
           select count(*)
           from contacts.VMembership as R
           where R.GroupIdentifier = G.GroupIdentifier
       )                as GroupSize
     , G.GroupType
     , G.ParentGroupIdentifier
     , Parent.GroupName as ParentGroupName
     , T.OrganizationIdentifier
     , GSI.ItemName as GroupStatus
     , GSI.ItemIdentifier as GroupStatusItemIdentifier
     , G.GroupRegion
     , G.MessageToAdminWhenEventVenueChanged
     , G.GroupEmail
from contacts.QGroup                     as G
     inner join accounts.QOrganization   as T on T.OrganizationIdentifier = G.OrganizationIdentifier
     left join contacts.QGroup           as Parent on G.ParentGroupIdentifier = Parent.GroupIdentifier
     left join utilities.TCollectionItem as GSI on GSI.ItemIdentifier = G.GroupStatusItemIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [issues].[QIssue](
	[IssueClosed] [datetimeoffset](7) NULL,
	[IssueDescription] [varchar](6400) NULL,
	[IssueIdentifier] [uniqueidentifier] NOT NULL,
	[IssueOpened] [datetimeoffset](7) NOT NULL,
	[IssueSource] [varchar](100) NULL,
	[IssueStatusCategory] [varchar](120) NOT NULL,
	[IssueType] [varchar](50) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[AdministratorUserIdentifier] [uniqueidentifier] NULL,
	[TopicUserIdentifier] [uniqueidentifier] NULL,
	[LawyerUserIdentifier] [uniqueidentifier] NULL,
	[AttachmentCount] [int] NOT NULL,
	[CommentCount] [int] NOT NULL,
	[PersonCount] [int] NOT NULL,
	[LastChangeTime] [datetimeoffset](7) NOT NULL,
	[LastChangeType] [varchar](100) NOT NULL,
	[LastChangeUser] [uniqueidentifier] NOT NULL,
	[IssueTitle] [varchar](200) NULL,
	[IssueNumber] [int] NOT NULL,
	[EmployerGroupIdentifier] [uniqueidentifier] NULL,
	[GroupCount] [int] NOT NULL,
	[IssueReported] [datetimeoffset](7) NULL,
	[IssueOpenedBy] [uniqueidentifier] NULL,
	[IssueClosedBy] [uniqueidentifier] NULL,
	[IssueStatusIdentifier] [uniqueidentifier] NULL,
	[IssueStatusEffective] [datetimeoffset](7) NULL,
	[OwnerUserIdentifier] [uniqueidentifier] NULL,
	[ResponseSessionIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QIssue] PRIMARY KEY CLUSTERED 
(
	[IssueIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [issues].[QIssueAttachment](
	[FileName] [varchar](200) NOT NULL,
	[AttachmentPosted] [datetimeoffset](7) NOT NULL,
	[FileType] [varchar](20) NULL,
	[IssueIdentifier] [uniqueidentifier] NOT NULL,
	[PosterIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[FileIdentifier] [uniqueidentifier] NOT NULL,
	[AttachmentIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QIssueAttachment] PRIMARY KEY CLUSTERED 
(
	[AttachmentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assets].[QComment](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[CommentIdentifier] [uniqueidentifier] NOT NULL,
	[CommentCategory] [varchar](120) NULL,
	[CommentFlag] [varchar](10) NULL,
	[CommentFlagged] [datetimeoffset](7) NULL,
	[CommentIsHidden] [bit] NOT NULL,
	[CommentIsPrivate] [bit] NOT NULL,
	[CommentPosted] [datetimeoffset](7) NOT NULL,
	[CommentResolved] [datetimeoffset](7) NULL,
	[CommentRevised] [datetimeoffset](7) NULL,
	[CommentSubmitted] [datetimeoffset](7) NULL,
	[CommentText] [varchar](max) NOT NULL,
	[OriginText] [varchar](1200) NULL,
	[ContainerDescription] [varchar](2700) NULL,
	[ContainerIdentifier] [uniqueidentifier] NOT NULL,
	[ContainerSubtype] [varchar](30) NULL,
	[ContainerType] [varchar](30) NOT NULL,
	[AuthorUserIdentifier] [uniqueidentifier] NOT NULL,
	[AuthorUserName] [varchar](100) NULL,
	[AuthorUserRole] [varchar](20) NULL,
	[TrainingProviderGroupIdentifier] [uniqueidentifier] NULL,
	[RevisorUserIdentifier] [uniqueidentifier] NULL,
	[TopicUserIdentifier] [uniqueidentifier] NULL,
	[AssessmentAttemptIdentifier] [uniqueidentifier] NULL,
	[AssessmentBankIdentifier] [uniqueidentifier] NULL,
	[AssessmentFieldIdentifier] [uniqueidentifier] NULL,
	[AssessmentFormIdentifier] [uniqueidentifier] NULL,
	[AssessmentQuestionIdentifier] [uniqueidentifier] NULL,
	[AssessmentSpecificationIdentifier] [uniqueidentifier] NULL,
	[EventIdentifier] [uniqueidentifier] NULL,
	[EventStarted] [datetimeoffset](7) NULL,
	[EventFormat] [varchar](10) NULL,
	[RegistrationIdentifier] [uniqueidentifier] NULL,
	[IssueIdentifier] [uniqueidentifier] NULL,
	[LogbookIdentifier] [uniqueidentifier] NULL,
	[LogbookExperienceIdentifier] [uniqueidentifier] NULL,
	[UploadIdentifier] [uniqueidentifier] NULL,
	[TimestampCreated] [datetimeoffset](7) NOT NULL,
	[TimestampCreatedBy] [uniqueidentifier] NULL,
	[TimestampModified] [datetimeoffset](7) NULL,
	[TimestampModifiedBy] [uniqueidentifier] NULL,
	[CommentSubCategory] [varchar](120) NULL,
	[CommentTag] [varchar](50) NULL,
	[CommentAssignedToUserIdentifier] [uniqueidentifier] NULL,
	[CommentResolvedByUserIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QComment] PRIMARY KEY CLUSTERED 
(
	[CommentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [issues].[VIssue]
AS
    WITH IssueCTE AS (SELECT
                          Issue.*,
                          TTopic.FullName                      AS TopicUserName,
                          TTopic.Email                         AS TopicUserEmail,
                          TTopicPerson.EmployerGroupIdentifier AS TopicEmployerGroupIdentifier,
                          TTopicPerson.MembershipStatusItemIdentifier AS TopicMembershipStatusItemIdentifier
                      FROM
                          issues.QIssue AS Issue
                              LEFT JOIN(identities.[User] AS TTopic INNER JOIN contacts.Person AS TTopicPerson
                                        ON TTopicPerson.UserIdentifier = TTopic.UserIdentifier)
                                       ON TTopic.UserIdentifier = Issue.TopicUserIdentifier AND
                                          TTopicPerson.OrganizationIdentifier = Issue.OrganizationIdentifier)
       , TopicGroupNames AS (SELECT
                                 I.IssueIdentifier,
                                 STRING_AGG(O.GroupName, ', ') WITHIN GROUP (ORDER BY O.GroupName) AS Value
                             FROM
                                 IssueCTE AS I
                                     INNER JOIN contacts.Membership AS M
                                                ON M.UserIdentifier = I.TopicUserIdentifier
                                     INNER JOIN contacts.QGroup AS O
                                                ON O.GroupIdentifier = M.GroupIdentifier
                             WHERE
                                   O.OrganizationIdentifier = I.OrganizationIdentifier
                               AND O.GroupType NOT IN ('List', 'Role')
                               AND O.GroupIdentifier <> I.TopicEmployerGroupIdentifier
                             GROUP BY I.IssueIdentifier)
       , IssueComments AS (SELECT
                               IssueIdentifier,
                               COUNT(*) AS CommentCount
                           FROM
                               assets.QComment
                           WHERE
                               ContainerType = 'Case'
                           GROUP BY IssueIdentifier)
       , IssueAttachments AS (SELECT
                                  IssueIdentifier,
                                  COUNT(*) AS AttachmentCount
                              FROM
                                  issues.QIssueAttachment
                              GROUP BY IssueIdentifier)
    SELECT
        Issue.IssueClosed,
        Issue.IssueClosedBy,
        Issue.IssueDescription,
        Issue.IssueIdentifier,
        Issue.IssueOpened,
        Issue.IssueOpenedBy,
        Issue.IssueSource,
        Issue.IssueReported,
        Issue.IssueStatusIdentifier,
        IssueStatus.StatusName                                          AS IssueStatusName,
        IssueStatus.StatusDescription                                   AS IssueStatusDescription,
        COALESCE(IssueStatus.StatusDescription, IssueStatus.StatusName) AS IssueStatusDisplay,
        IssueStatus.StatusSequence                                      AS IssueStatusSequence,
        Issue.IssueStatusCategory,
        Issue.IssueType,
        Issue.OrganizationIdentifier,
        Issue.ResponseSessionIdentifier,
        Issue.AdministratorUserIdentifier,
        Issue.LawyerUserIdentifier,
        Issue.OwnerUserIdentifier,
        Issue.TopicUserIdentifier,
        ISNULL(IssueAttachments.AttachmentCount, 0)                     AS AttachmentCount,
        ISNULL(IssueComments.CommentCount, 0)                           AS CommentCount,
        Issue.PersonCount,
        Issue.LastChangeTime,
        Issue.LastChangeType,
        Issue.LastChangeUser,
        Issue.IssueTitle,
        Issue.IssueNumber,
        Issue.IssueStatusEffective,
        TAdministrator.FullName                                         AS AdministratorUserName,
        Issue.TopicUserName,
        Issue.TopicUserEmail,
        TOwner.FullName                                                 AS OwnerUserName,
        TOwner.FirstName                                                AS OwnerUserFirstName,
        TOwner.LastName                                                 AS OwnerUserLastName,
        TOwner.Email                                                    AS OwnerUserEmail,
        IssueEmployer.GroupIdentifier                                   AS IssueEmployerGroupIdentifier,
        IssueEmployer.GroupName                                         AS IssueEmployerGroupName,
        IssueEmployerParent.GroupIdentifier                             AS IssueEmployerGroupParentGroupIdentifier,
        IssueEmployerParent.GroupName                                   AS IssueEmployerGroupParentGroupName,
        MembershipStatus.ItemName                                       AS TopicAccountStatus,
        AssigneeEmployer.GroupName                                      AS TopicEmployerGroupName,
        TopicGroupNames.Value                                           AS TopicGroupNames,
        TLawyer.FullName                                                AS LawyerUserName,
        TLastChangeUser.FullName                                        AS LastChangeUserName
    FROM
        IssueCTE AS Issue
            LEFT JOIN TopicGroupNames
                      ON TopicGroupNames.IssueIdentifier = Issue.IssueIdentifier
            LEFT JOIN IssueComments
                      ON IssueComments.IssueIdentifier = Issue.IssueIdentifier
            LEFT JOIN IssueAttachments
                      ON IssueAttachments.IssueIdentifier = Issue.IssueIdentifier
            LEFT JOIN identities.[User] AS TAdministrator
                      ON TAdministrator.UserIdentifier = Issue.AdministratorUserIdentifier
            LEFT JOIN identities.[User] AS TOwner
                      ON TOwner.UserIdentifier = Issue.OwnerUserIdentifier
            LEFT JOIN identities.[User] AS TLawyer
                      ON TLawyer.UserIdentifier = Issue.LawyerUserIdentifier
            LEFT JOIN identities.[User] AS TLastChangeUser
                      ON TLastChangeUser.UserIdentifier = Issue.LastChangeUser
            LEFT JOIN utilities.TCollectionItem AS MembershipStatus
                      ON MembershipStatus.ItemIdentifier = Issue.TopicMembershipStatusItemIdentifier
            LEFT JOIN contacts.QGroup AS AssigneeEmployer
                      ON Issue.TopicEmployerGroupIdentifier = AssigneeEmployer.GroupIdentifier
            LEFT JOIN contacts.QGroup AS IssueEmployer
                      ON Issue.EmployerGroupIdentifier = IssueEmployer.GroupIdentifier
            LEFT JOIN contacts.QGroup AS IssueEmployerParent
                      ON IssueEmployer.ParentGroupIdentifier = IssueEmployerParent.GroupIdentifier
            LEFT JOIN issues.TIssueStatus AS IssueStatus
                      ON IssueStatus.StatusIdentifier = Issue.IssueStatusIdentifier
;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[Profile]
    AS
        WITH CTE AS ( SELECT StandardIdentifier          AS ProfileStandardIdentifier
                           , ParentStandardIdentifier    AS ParentProfileStandardIdentifier
                           , Code                        AS ProfileNumber
                           , Standard.ContentTitle       AS ProfileTitle
                           , Standard.ContentDescription AS ProfileDescription
                           , IsLocked
                           , CertificationHoursPercentCore
                           , CertificationHoursPercentNonCore
                           , IsCertificateEnabled        AS CertificationIsAvailable
                           , IsCertificateEnabled
                           , Standard.OrganizationIdentifier
                      FROM standards.Standard
                               INNER JOIN accounts.QOrganization AS T
                                          ON T.OrganizationIdentifier = Standard.OrganizationIdentifier
                      WHERE StandardType = 'Profile' )
        SELECT *
             , CASE
            WHEN OrganizationIdentifier = '8258CB0A-D1E8-4BC1-94B3-E70652503437' THEN
                'Enterprise'
            ELSE
                'Organization'
            END AS Visibility
        FROM CTE;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[BUserDepartmentCompetencyStatus](
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyStandardIdentifier] [uniqueidentifier] NOT NULL,
	[ValidationStatus] [varchar](32) NOT NULL,
	[IsValidated] [bit] NULL,
	[IsCritical] [bit] NULL,
	[IsRequired] [bit] NULL,
	[IsPrimary] [bit] NULL,
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_BUserDepartmentCompetencyStatus] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[BUserCredentialStatus](
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[AchievementIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[CredentialStatus] [varchar](32) NULL,
	[AchievementLabel] [varchar](64) NOT NULL,
	[IsGlobal] [bit] NOT NULL,
 CONSTRAINT [PK_BUserCredentialStatus] PRIMARY KEY CLUSTERED 
(
	[UserIdentifier] ASC,
	[AchievementIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[BUserProfile](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[CompanyName] [varchar](128) NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[DepartmentName] [varchar](128) NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[UserFullName] [varchar](128) NOT NULL,
	[PrimaryProfileIdentifier] [uniqueidentifier] NULL,
	[PrimaryProfileNumber] [varchar](50) NULL,
	[PrimaryProfileTitle] [varchar](max) NULL,
	[IsCritical] [bit] NOT NULL,
	[Sequence] [int] NOT NULL,
	[Heading] [varchar](128) NOT NULL,
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_BUserProfile] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[BUserEmployment](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[CompanyName] [varchar](128) NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[DepartmentName] [varchar](128) NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[UserName] [varchar](128) NOT NULL,
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_BUserEmployment] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[VUserStatus_Step3]
AS
    SELECT
            BUserEmployment.OrganizationIdentifier,
            BUserEmployment.CompanyName,
            BUserEmployment.DepartmentIdentifier,
            BUserEmployment.DepartmentName,
            BUserEmployment.UserIdentifier,
            BUserEmployment.UserName,
            p.ProfileStandardIdentifier                 AS PrimaryProfileIdentifier,
            p.ProfileNumber                             AS PrimaryProfileNumber,
            p.ProfileTitle                              AS PrimaryProfileTitle,
            SubTypes.Sequence,
            SubTypes.Heading,
            COUNT(BUserCredentialStatus.UserIdentifier) AS PrimaryRequired,
            0                                           AS SecondaryRequired,
            SUM(   CASE
                       WHEN BUserCredentialStatus.CredentialStatus = 'Valid'
                           THEN
                           1
                       ELSE
                           0
                   END
               )                                        AS PrimarySatisfied,
            CASE
                WHEN COUNT(BUserCredentialStatus.UserIdentifier) <> 0
                    THEN
                    ROUND(
                             CAST(COUNT(   CASE
                                               WHEN BUserCredentialStatus.CredentialStatus = 'Valid'
                                                   THEN
                                                   BUserCredentialStatus.UserIdentifier
                                               ELSE
                                                   NULL
                                           END
                                       ) AS DECIMAL) / CAST(COUNT(BUserCredentialStatus.UserIdentifier) AS DECIMAL), 2, 1
                         )
                ELSE
                    1
            END                                         AS PrimaryScore,
            NULL                                        AS PrimaryExpired,
            NULL                                        AS PrimaryNotCompleted,
            NULL                                        AS PrimaryNotApplicable,
            NULL                                        AS PrimaryNeedsTraining,
            NULL                                        AS PrimarySelfAssessed,
            NULL                                        AS PrimarySubmitted,
            NULL                                        AS PrimaryValidated,
            COUNT(BUserCredentialStatus.UserIdentifier) AS ComplianceRequired,
            SUM(   CASE
                       WHEN BUserCredentialStatus.CredentialStatus = 'Valid'
                           THEN
                           1
                       ELSE
                           0
                   END
               )                                        AS ComplianceSatisfied,
            CASE
                WHEN COUNT(BUserCredentialStatus.UserIdentifier) <> 0
                    THEN
                    ROUND(
                             CAST(COUNT(   CASE
                                               WHEN BUserCredentialStatus.CredentialStatus = 'Valid'
                                                   THEN
                                                   BUserCredentialStatus.UserIdentifier
                                               ELSE
                                                   NULL
                                           END
                                       ) AS DECIMAL) / CAST(COUNT(BUserCredentialStatus.UserIdentifier) AS DECIMAL), 2, 1
                         )
                ELSE
                    1
            END                                         AS ComplianceScore,
            NULL                                        AS ComplianceExpired,
            NULL                                        AS ComplianceNotCompleted,
            NULL                                        AS ComplianceNotApplicable,
            NULL                                        AS ComplianceNeedsTraining,
            NULL                                        AS ComplianceSelfAssessed,
            NULL                                        AS ComplianceSubmitted,
            NULL                                        AS ComplianceValidated,
            COUNT(BUserCredentialStatus.UserIdentifier) AS Required,
            SUM(   CASE
                       WHEN BUserCredentialStatus.CredentialStatus = 'Valid'
                           THEN
                           1
                       ELSE
                           0
                   END
               )                                        AS Satisfied,
            CASE
                WHEN COUNT(BUserCredentialStatus.UserIdentifier) <> 0
                    THEN
                    ROUND(
                             CAST(COUNT(   CASE
                                               WHEN BUserCredentialStatus.CredentialStatus = 'Valid'
                                                   THEN
                                                   BUserCredentialStatus.UserIdentifier
                                               ELSE
                                                   NULL
                                           END
                                       ) AS DECIMAL) / CAST(COUNT(BUserCredentialStatus.UserIdentifier) AS DECIMAL), 2, 1
                         )
                ELSE
                    1
            END                                         AS Score,
            NULL                                        AS Expired,
            NULL                                        AS NotCompleted,
            NULL                                        AS NotApplicable,
            NULL                                        AS NeedsTraining,
            NULL                                        AS SelfAssessed,
            NULL                                        AS Submitted,
            NULL                                        AS Validated
    FROM
            custom_cmds.BUserEmployment WITH (NOLOCK)
        LEFT JOIN
            standards.DepartmentProfileUser AS e WITH (NOLOCK)
                ON e.UserIdentifier = BUserEmployment.UserIdentifier
                   AND e.DepartmentIdentifier = BUserEmployment.DepartmentIdentifier
                   AND e.IsPrimary = 1
        CROSS JOIN
            (
                SELECT
                    1                                   AS Sequence,
                    'Time-Sensitive Safety Certificate' AS Heading,
                    'Time-Sensitive Safety Certificate' AS Name,
                    1                                   AS IsRequired
                UNION ALL
                SELECT
                    2,
                    'Additional Compliance Requirement',
                    'Additional Compliance Requirement',
                    0
                UNION ALL
                SELECT
                    5,
                    'Code of Practice',
                    'Code of Practice',
                    0
                UNION ALL
                SELECT
                    6,
                    'Safe Operating Practice',
                    'Safe Operating Practice',
                    0
                UNION ALL
                SELECT
                    7,
                    'Human Resources Document',
                    'Human Resources Document',
                    0
                UNION ALL
                SELECT
                    8,
                    'Module',
                    'Module',
                    0
                UNION ALL
                SELECT
                    9,
                    'Training Guide',
                    'Training Guide',
                    0
                UNION ALL
                SELECT
                    10,
                    'Site-Specific Operating Procedure',
                    'Site-Specific Operating Procedure',
                    0
                UNION ALL
                SELECT
                    11,
                    'Orientation',
                    'Orientation',
                    0
                UNION ALL
                SELECT
                    12,
                    'HR Learning Module',
                    'HR Learning Module',
                    0
            )                               AS SubTypes
        LEFT JOIN
            custom_cmds.BUserCredentialStatus
                ON BUserCredentialStatus.UserIdentifier = BUserEmployment.UserIdentifier
                   AND
                       (
                           e.UserIdentifier IS NOT NULL
                           OR BUserCredentialStatus.UserIdentifier NOT IN
                                  (
                                      SELECT
                                          UserIdentifier
                                      FROM
                                          standards.DepartmentProfileUser WITH (NOLOCK)
                                      WHERE
                                          IsPrimary = 1
                                  )
                       )
                   AND BUserCredentialStatus.AchievementLabel = SubTypes.Name
                   AND
                       (
                           BUserCredentialStatus.IsGlobal = 1
                           OR BUserCredentialStatus.OrganizationIdentifier = BUserEmployment.OrganizationIdentifier
                       )
        LEFT JOIN
            custom_cmds.Profile AS p WITH (NOLOCK)
                ON p.ProfileStandardIdentifier = e.ProfileStandardIdentifier
    GROUP BY
            BUserEmployment.OrganizationIdentifier,
            BUserEmployment.CompanyName,
            BUserEmployment.DepartmentIdentifier,
            BUserEmployment.DepartmentName,
            BUserEmployment.UserIdentifier,
            BUserEmployment.UserName,
            p.ProfileStandardIdentifier,
            p.ProfileNumber,
            p.ProfileTitle,
            SubTypes.Sequence,
            SubTypes.Name,
            SubTypes.Heading
    UNION ALL
    SELECT
            CompetencyEmployees.OrganizationIdentifier,
            CompetencyEmployees.CompanyName,
            CompetencyEmployees.DepartmentIdentifier,
            CompetencyEmployees.DepartmentName,
            CompetencyEmployees.UserIdentifier,
            CompetencyEmployees.UserFullName,
            CompetencyEmployees.PrimaryProfileIdentifier,
            CompetencyEmployees.PrimaryProfileNumber,
            CompetencyEmployees.PrimaryProfileTitle,
            CompetencyEmployees.Sequence,
            CompetencyEmployees.Heading,
            COUNT(   CASE
                         WHEN Competencies.IsPrimary = 1
                             THEN -- AND Competencies.IsRequired = 1
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS PrimaryRequired,
            COUNT(   CASE
                         WHEN Competencies.IsPrimary = 0
                             THEN -- AND Competencies.IsRequired = 1
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS SecondaryRequired,
            COUNT(   CASE
                         WHEN Competencies.IsPrimary = 1
                              AND
                                  (
                                      Competencies.ValidationStatus = 'Validated'
                                      OR
                                          (
                                              Competencies.ValidationStatus = 'Not Applicable'
                                              AND Competencies.IsValidated = 1
                                          )
                                  )
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS PrimarySatisfied,
            CASE
                WHEN COUNT(   CASE
                                  WHEN Competencies.IsPrimary = 1
                                      THEN
                                      1
                                  ELSE
                                      NULL
                              END
                          ) <> 0
                    THEN
                    ROUND(
                             CAST((COUNT(   CASE
                                                WHEN Competencies.ValidationStatus = 'Validated'
                                                     AND Competencies.IsPrimary = 1
                                                    THEN
                                                    1
                                                ELSE
                                                    NULL
                                            END
                                        ) + COUNT(   CASE
                                                         WHEN Competencies.ValidationStatus = 'Not Applicable'
                                                              AND Competencies.IsValidated = 1
                                                              AND Competencies.IsPrimary = 1
                                                             THEN
                                                             1
                                                         ELSE
                                                             NULL
                                                     END
                                                 )
                                  ) AS DECIMAL) / CAST(COUNT(   CASE
                                                                    WHEN Competencies.IsPrimary = 1
                                                                        THEN
                                                                        1
                                                                    ELSE
                                                                        NULL
                                                                END
                                                            ) AS DECIMAL), 2, 1
                         )
                ELSE
                    1
            END                                              AS PrimaryScore,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Expired'
                              AND Competencies.IsPrimary = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS PrimaryExpired,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Not Completed'
                              AND Competencies.IsPrimary = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS PrimaryNotCompleted,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Not Applicable'
                              AND Competencies.IsPrimary = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS PrimaryNotApplicable,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Needs Training'
                              AND Competencies.IsPrimary = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS PrimaryNeedsTraining,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Self-Assessed'
                              AND Competencies.IsPrimary = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS PrimarySelfAssessed,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Submitted for Validation'
                              AND Competencies.IsPrimary = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS PrimarySubmitted,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Validated'
                              AND Competencies.IsPrimary = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS PrimaryValidated,
            COUNT(   CASE
                         WHEN Competencies.IsRequired = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS ComplianceRequired,
            COUNT(   CASE
                         WHEN Competencies.IsRequired = 1
                              AND
                                  (
                                      Competencies.ValidationStatus = 'Validated'
                                      OR
                                          (
                                              Competencies.ValidationStatus = 'Not Applicable'
                                              AND Competencies.IsValidated = 1
                                          )
                                  )
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS ComplianceSatisfied,
            CASE
                WHEN COUNT(   CASE
                                  WHEN Competencies.IsRequired = 1
                                      THEN
                                      1
                                  ELSE
                                      NULL
                              END
                          ) <> 0
                    THEN
                    ROUND(
                             CAST((COUNT(   CASE
                                                WHEN Competencies.ValidationStatus = 'Validated'
                                                     AND Competencies.IsRequired = 1
                                                    THEN
                                                    1
                                                ELSE
                                                    NULL
                                            END
                                        ) + COUNT(   CASE
                                                         WHEN Competencies.ValidationStatus = 'Not Applicable'
                                                              AND Competencies.IsValidated = 1
                                                              AND Competencies.IsRequired = 1
                                                             THEN
                                                             1
                                                         ELSE
                                                             NULL
                                                     END
                                                 )
                                  ) AS DECIMAL) / CAST(COUNT(   CASE
                                                                    WHEN Competencies.IsRequired = 1
                                                                        THEN
                                                                        1
                                                                    ELSE
                                                                        NULL
                                                                END
                                                            ) AS DECIMAL), 2, 1
                         )
                ELSE
                    1
            END                                              AS ComplianceScore,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Expired'
                              AND Competencies.IsRequired = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS ComplianceExpired,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Not Completed'
                              AND Competencies.IsRequired = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS ComplianceNotCompleted,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Not Applicable'
                              AND Competencies.IsRequired = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS ComplianceNotApplicable,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Needs Training'
                              AND Competencies.IsRequired = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS ComplianceNeedsTraining,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Self-Assessed'
                              AND Competencies.IsRequired = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS ComplianceSelfAssessed,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Submitted for Validation'
                              AND Competencies.IsRequired = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS ComplianceSubmitted,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Validated'
                              AND Competencies.IsRequired = 1
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS ComplianceValidated,
            COUNT(Competencies.CompetencyStandardIdentifier) AS Required,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Validated'
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS Satisfied,
            CASE
                WHEN COUNT(Competencies.CompetencyStandardIdentifier) <> 0
                    THEN
                    ROUND(
                             CAST((COUNT(   CASE
                                                WHEN Competencies.ValidationStatus = 'Validated'
                                                    THEN
                                                    1
                                                ELSE
                                                    NULL
                                            END
                                        ) + COUNT(   CASE
                                                         WHEN Competencies.ValidationStatus = 'Not Applicable'
                                                              AND Competencies.IsValidated = 1
                                                             THEN
                                                             1
                                                         ELSE
                                                             NULL
                                                     END
                                                 )
                                  ) AS DECIMAL) / CAST(COUNT(Competencies.CompetencyStandardIdentifier) AS DECIMAL), 2, 1
                         )
                ELSE
                    1
            END                                              AS Score,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Expired'
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS Expired,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Not Completed'
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS NotCompleted,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Not Applicable'
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS NotApplicable,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Needs Training'
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS NeedsTraining,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Self-Assessed'
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS SelfAssessed,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Submitted for Validation'
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS Submitted,
            COUNT(   CASE
                         WHEN Competencies.ValidationStatus = 'Validated'
                             THEN
                             1
                         ELSE
                             NULL
                     END
                 )                                           AS Validated
    FROM
            custom_cmds.BUserProfile                    AS CompetencyEmployees WITH (NOLOCK)
        LEFT JOIN
            custom_cmds.BUserDepartmentCompetencyStatus AS Competencies WITH (NOLOCK)
                ON Competencies.UserIdentifier = CompetencyEmployees.UserIdentifier
                   AND Competencies.DepartmentIdentifier = CompetencyEmployees.DepartmentIdentifier
                   AND Competencies.IsCritical = CompetencyEmployees.IsCritical
    GROUP BY
            CompetencyEmployees.OrganizationIdentifier,
            CompetencyEmployees.CompanyName,
            CompetencyEmployees.DepartmentIdentifier,
            CompetencyEmployees.DepartmentName,
            CompetencyEmployees.UserIdentifier,
            CompetencyEmployees.UserFullName,
            CompetencyEmployees.PrimaryProfileIdentifier,
            CompetencyEmployees.PrimaryProfileNumber,
            CompetencyEmployees.PrimaryProfileTitle,
            CompetencyEmployees.IsCritical,
            CompetencyEmployees.Sequence,
            CompetencyEmployees.Heading;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [messages].[QSubscriberGroup](
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[MessageIdentifier] [uniqueidentifier] NOT NULL,
	[Subscribed] [datetimeoffset](7) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QSubscriberGroup] PRIMARY KEY CLUSTERED 
(
	[GroupIdentifier] ASC,
	[MessageIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [messages].[VSubscriberGroup]
as
select SG.GroupIdentifier
     , SG.MessageIdentifier
     , SG.Subscribed
     , G.GroupName
     , G.GroupCode
     , (
           select count(*)
           from contacts.VMembership as R
           where R.GroupIdentifier = G.GroupIdentifier
       )                        as GroupSize
     , M.MessageName
     , M.OrganizationIdentifier as MessageOrganizationIdentifier
from messages.QSubscriberGroup    as SG
     inner join contacts.QGroup   as G on G.GroupIdentifier = SG.GroupIdentifier
     inner join messages.QMessage as M on M.MessageIdentifier = SG.MessageIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [achievements].[VCredential] AS
    SELECT C.AchievementIdentifier
         , C.CredentialIdentifier
         , A.OrganizationIdentifier
         , AchievementOrganization.ParentOrganizationIdentifier
         , C.UserIdentifier
         , C.CredentialAssigned
         , C.CredentialDescription
         , C.CredentialExpired
         , C.CredentialGranted
         , C.CredentialGrantedDescription
         , C.CredentialGrantedScore
         , C.CredentialNecessity
         , C.CredentialPriority
         , C.CredentialRevoked
         , C.CredentialRevokedReason
         , C.CredentialRevokedScore
         , C.CredentialStatus
         , C.PublicationStatus
         , C.AuthorityIdentifier
         , C.AuthorityName
         , C.AuthorityType
         , C.AuthorityLocation
         , C.AuthorityReference
         , C.TransactionHash
         , C.PublisherAddress
         , C.CredentialHours
         , C.ExpirationType             AS CredentialExpirationType
         , C.ExpirationFixedDate        AS CredentialExpirationFixedDate
         , C.ExpirationLifetimeQuantity AS CredentialExpirationLifetimeQuantity
         , C.ExpirationLifetimeUnit     AS CredentialExpirationLifetimeUnit
         , C.CredentialExpirationExpected
         , C.CredentialExpirationReminderRequested0
         , C.CredentialExpirationReminderRequested1
         , C.CredentialExpirationReminderRequested2
         , C.CredentialExpirationReminderRequested3
         , C.CredentialExpirationReminderDelivered0
         , C.CredentialExpirationReminderDelivered1
         , C.CredentialExpirationReminderDelivered2
         , C.CredentialExpirationReminderDelivered3
         , A.AchievementLabel
         , A.AchievementTitle
         , A.AchievementDescription
         , A.AchievementIsEnabled
         , A.AchievementReportingDisabled
         , A.ExpirationType             AS AchievementExpirationType
         , A.ExpirationFixedDate        AS AchievementExpirationFixedDate
         , A.ExpirationLifetimeQuantity AS AchievementExpirationLifetimeQuantity
         , A.ExpirationLifetimeUnit     AS AchievementExpirationLifetimeUnit
         , A.CertificateLayoutCode      AS AchievementCertificateLayoutCode
         , A.HasBadgeImage              AS HasBadgeImage
         , A.BadgeImageUrl              AS BadgeImageUrl
         , U.AccessGrantedToCmds        AS UserAccessGrantedToCmds
         , U.UtcArchived                AS UserArchived
         , U.Email                      AS UserEmail
         , ISNULL(P.EmailEnabled, 0)    AS UserEmailEnabled
         , U.FirstName                  AS UserFirstName
         , U.FullName                   AS UserFullName
         , U.LastName                   AS UserLastName
         , P.EmployerGroupIdentifier
         , E.GroupName                  AS EmployerGroupName
         , GSI.ItemName                 AS EmployerGroupStatus
         , GSI.ItemIdentifier           AS EmployerGroupStatusItemIdentifier
         , E.GroupRegion                AS EmployerGroupRegion
         , C.EmployerGroupIdentifier    AS OriginalEmployerGroupIdentifier
         , OE.GroupName                 AS OriginalEmployerGroupName
         , C.EmployerGroupStatus        AS OriginalEmployerGroupStatus
         , OE.GroupRegion               AS OriginalEmployerGroupRegion
         , P.Region                     AS UserRegion
         , P.PersonCode                 AS PersonCode
         , P.FullName                   AS PersonFullName
         , A.AchievementAllowSelfDeclared
    FROM achievements.QCredential AS C WITH (NOLOCK)
             INNER JOIN achievements.QAchievement AS A WITH (NOLOCK)
                        ON A.AchievementIdentifier = C.AchievementIdentifier
             INNER JOIN accounts.QOrganization AS AchievementOrganization WITH (NOLOCK)
                        ON AchievementOrganization.OrganizationIdentifier =
                           A.OrganizationIdentifier
             INNER JOIN identities.[User] AS U WITH (NOLOCK)
                        ON U.UserIdentifier = C.UserIdentifier
             LEFT JOIN contacts.QPerson AS P WITH (NOLOCK)
                       ON P.UserIdentifier = C.UserIdentifier AND
                          P.OrganizationIdentifier = A.OrganizationIdentifier
             LEFT JOIN contacts.QGroup AS E WITH (NOLOCK)
                       ON E.GroupIdentifier = P.EmployerGroupIdentifier
             LEFT JOIN contacts.QGroup AS OE WITH (NOLOCK)
                       ON OE.GroupIdentifier = C.EmployerGroupIdentifier
             LEFT JOIN utilities.TCollectionItem AS GSI WITH (NOLOCK)
                       ON GSI.ItemIdentifier = E.GroupStatusItemIdentifier
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[VCmdsCredential] AS
    SELECT CASE
        WHEN C.CredentialPriority = 'Planned' THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
        END               AS IsInTrainingPlan
         , CASE
        WHEN C.CredentialNecessity = 'Mandatory' THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
        END               AS CredentialIsMandatory
         , C.AchievementIdentifier
         , C.OrganizationIdentifier
         , C.UserIdentifier
         , C.CredentialIdentifier
         , C.CredentialAssigned
         , C.CredentialExpired
         , C.CredentialGranted
         , C.CredentialGrantedScore
         , C.CredentialRevoked
         , C.CredentialRevokedScore
         , C.CredentialStatus
         , C.CredentialNecessity
         , C.CredentialPriority
         , C.CredentialDescription
         , C.AuthorityIdentifier
         , C.AuthorityName
         , C.AuthorityType
         , C.AuthorityLocation
         , C.AuthorityReference
         , C.CredentialHours
         , C.CredentialExpirationType
         , C.CredentialExpirationFixedDate
         , C.CredentialExpirationLifetimeQuantity
         , C.CredentialExpirationLifetimeUnit
         , C.CredentialExpirationExpected
         , C.CredentialExpirationReminderRequested0
         , C.CredentialExpirationReminderRequested1
         , C.CredentialExpirationReminderRequested2
         , C.CredentialExpirationReminderRequested3
         , C.CredentialExpirationReminderDelivered0
         , C.CredentialExpirationReminderDelivered1
         , C.CredentialExpirationReminderDelivered2
         , C.CredentialExpirationReminderDelivered3
         , C.AchievementLabel
         , C.AchievementTitle
         , C.AchievementDescription
         , C.AchievementIsEnabled
         , C.AchievementReportingDisabled
         , C.AchievementExpirationType
         , C.AchievementExpirationFixedDate
         , C.AchievementExpirationLifetimeQuantity
         , C.AchievementExpirationLifetimeUnit
         , CASE
        WHEN C.OrganizationIdentifier = '8258CB0A-D1E8-4BC1-94B3-E70652503437'
            THEN 'Enterprise'
        ELSE 'Organization'
        END               AS AchievementVisibility
         , C.UserEmail
         , C.UserFullName
         , C.UserFirstName
         , C.UserLastName
         , C.UserRegion
         , C.UserArchived
         , C.UserArchived AS UserUtcArchived
         , C.EmployerGroupIdentifier
         , C.EmployerGroupName
         , C.AchievementAllowSelfDeclared
         , C.PersonCode
         , C.PersonFullName
    FROM achievements.VCredential AS C;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[ContactExperience](
	[AuthorityCity] [varchar](256) NULL,
	[AuthorityCountry] [varchar](64) NULL,
	[AuthorityName] [varchar](256) NULL,
	[AuthorityProvince] [varchar](64) NULL,
	[Completed] [date] NULL,
	[ContactExperienceType] [varchar](64) NOT NULL,
	[CreditHours] [decimal](5, 2) NULL,
	[Deadline] [date] NULL,
	[Description] [varchar](max) NULL,
	[Expired] [date] NULL,
	[IsSuccess] [bit] NOT NULL,
	[LifetimeMonths] [int] NULL,
	[Score] [decimal](5, 4) NULL,
	[Status] [varchar](32) NOT NULL,
	[ExperienceIdentifier] [uniqueidentifier] NOT NULL,
	[Title] [varchar](256) NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ContactExperience] PRIMARY KEY CLUSTERED 
(
	[ExperienceIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[VCmdsCredentialAndExperience]
as
select '00000000-0000-0000-0000-000000000000' as ExperienceIdentifier
     , C.AchievementIdentifier
     , C.AchievementLabel
     , C.OrganizationIdentifier               as AchievementOrganizationIdentifier
     , C.CredentialIdentifier
     , C.AchievementTitle
     , C.AuthorityLocation
     , C.AuthorityName
     , C.AuthorityReference
     , C.AchievementDescription
     , C.AchievementReportingDisabled
     , C.CredentialDescription
     , C.CredentialExpired
     , C.CredentialExpirationExpected
     , C.CredentialGranted
     , C.CredentialGrantedScore
     , C.CredentialRevoked
     , C.CredentialRevokedScore
     , CredentialGrantedScore                 as GradePercent
     , C.CredentialIsMandatory
     , case
           when C.CredentialPriority = 'Planned' then
               cast(1 as bit)
           else
               cast(0 as bit)
       end                                    as CredentialIsPlanned
     , case
           when isnull(C.CredentialExpirationType, 'None') = 'None' then
               cast(0 as bit)
           else
               cast(1 as bit)
       end                                    as CredentialIsTimeSensitive
     , C.UserFullName
     , C.UserIdentifier
     , C.AchievementVisibility
     , C.CredentialStatus
     , cast(1 as bit)                         as IsSuccess
     , cast(null as decimal)                  as CreditHours
     , case
           when C.CredentialExpirationLifetimeUnit = 'Month' then
               C.CredentialExpirationLifetimeQuantity
           else
               null
       end                                    as LifetimeMonths
     , C.IsInTrainingPlan
     , C.AchievementTitle                     as ProgramTitle
     , C.UserFirstName
     , C.UserLastName
from custom_cmds.VCmdsCredential as C
union all
select er.ExperienceIdentifier
     , '00000000-0000-0000-0000-000000000000'
     , null
     , null
     , null
     , er.Title
     , isnull(er.AuthorityCity, '') + ', ' + isnull(er.AuthorityProvince, '') + ', ' + isnull(er.AuthorityCountry, '')
     , er.AuthorityName
     , null
     , null
     , cast(0 as bit)
     , er.Description
     , er.Expired
     , null
     , er.Completed
     , null
     , null
     , null
     , case
           when er.Score is not null then
               er.Score / 100
           else
               er.Score
       end                                   as GradePercent
     , cast(0 as bit)
     , cast(0 as bit)
     , case
           when er.LifetimeMonths is not null then
               cast(1 as bit)
           else
               cast(0 as bit)
       end
     , e.FullName
     , e.UserIdentifier
     , null
     , case er.Status
           when 'Completed' then
               'Valid'
           when 'Not Completed' then
               'Pending'
           when 'Expired' then
               'Expired'
           else
               er.Status
       end
     , er.IsSuccess
     , er.CreditHours
     , er.LifetimeMonths
     , null
     , null
     , e.FirstName
     , e.LastName
from contacts.ContactExperience   as er
     inner join identities.[User] as e on er.UserIdentifier = e.UserIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [contacts].[VMembershipDetail] AS
    SELECT
        MembershipIdentifier
      , MembershipEffective
      , MembershipFunction
      , M.GroupIdentifier
      , GroupName
      , GroupType
      , G.OrganizationIdentifier AS GroupOrganizationIdentifier
      , M.OrganizationIdentifier AS MembershipOrganizationIdentifier
      , UserIdentifier
      , MembershipExpiry
      , Modified
      , ModifiedBy
    FROM
        contacts.QMembership AS M
        INNER JOIN contacts.QGroup AS G ON G.GroupIdentifier = M.GroupIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [resources].[Upload](
	[UploadPrivacyScope] [varchar](8) NOT NULL,
	[Uploader] [uniqueidentifier] NOT NULL,
	[ContainerIdentifier] [uniqueidentifier] NOT NULL,
	[ContainerType] [varchar](32) NOT NULL,
	[ContentFingerprint] [varchar](24) NULL,
	[ContentSize] [int] NULL,
	[ContentType] [varchar](32) NULL,
	[Description] [varchar](300) NULL,
	[Name] [varchar](500) NOT NULL,
	[NavigateUrl] [varchar](500) NULL,
	[Uploaded] [datetimeoffset](7) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[Title] [varchar](256) NOT NULL,
	[UploadIdentifier] [uniqueidentifier] NOT NULL,
	[UploadType] [varchar](16) NOT NULL,
 CONSTRAINT [PK_Upload] PRIMARY KEY CLUSTERED 
(
	[UploadIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [assets].[VComment]
AS
	SELECT
			C.OrganizationIdentifier,
			CommentIdentifier,
			CommentResolvedByUserIdentifier,
			CommentAssignedToUserIdentifier,
			CommentCategory,
			CommentSubCategory,
			CommentFlag,
			CommentTag,
			CommentFlagged,
			CommentIsHidden,
			CommentIsPrivate,
			CommentPosted,
			CommentResolved,
			CommentRevised,
			CommentSubmitted,
			CommentText,
			ContainerDescription,
			C.ContainerIdentifier,
			ContainerSubtype,
			C.ContainerType,
			AuthorUserIdentifier,
			AuthorUserRole,
			TrainingProviderGroupIdentifier,
			RevisorUserIdentifier,
			TopicUserIdentifier,
			AssessmentAttemptIdentifier,
			AssessmentBankIdentifier,
			AssessmentFieldIdentifier,
			AssessmentFormIdentifier,
			AssessmentQuestionIdentifier,
			AssessmentSpecificationIdentifier,
			EventIdentifier,
			EventStarted,
			EventFormat,
			RegistrationIdentifier,
			IssueIdentifier,
			LogbookIdentifier,
			LogbookExperienceIdentifier,
			C.UploadIdentifier,
			TimestampCreated,
			TimestampCreatedBy,
			TimestampModified,
			TimestampModifiedBy,

			Author.FullName AS AuthorUserName, Author.Email AS AuthorEmail,
			B.BankName, B.BankTitle,
			G.GroupName AS TrainingProviderGroupName,
			Revisor.FullName AS RevisorUserName,
			Topic.FullName AS TopicUserName, Topic.Email AS TopicEmail, Topic.FirstName AS TopicFirstName, Topic.LastName AS TopicLastName,
			Upload.Name AS UploadName, Upload.NavigateUrl AS UploadUrl, Upload.ContentType AS UploadType, Upload.Uploaded AS UploadModified, Upload.ContentSize AS UploadSize, Upload.ContentFingerprint AS UploadFingerprint 
	FROM
			assets.QComment AS C
			LEFT JOIN identities.[User] Author ON Author.UserIdentifier = AuthorUserIdentifier
			LEFT JOIN banks.QBank AS B ON C.AssessmentBankIdentifier = B.BankIdentifier
			LEFT JOIN contacts.QGroup G ON C.TrainingProviderGroupIdentifier = G.GroupIdentifier
			LEFT JOIN identities.[User] Revisor ON Revisor.UserIdentifier = RevisorUserIdentifier
			LEFT JOIN identities.[User] Topic ON Topic.UserIdentifier = TopicUserIdentifier
			LEFT JOIN resources.Upload Upload ON Upload.UploadIdentifier = C.UploadIdentifier
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [surveys].[VSurveyResponseAnswer]
as
select r.ResponseSessionIdentifier
     , r.ResponseSessionStarted
	 , r.ResponseSessionCompleted
     , u.UserIdentifier
     , u.FullName           as UserName
     , u.Email              as UserEmail
     , a.ResponseAnswerText as AnswerText
     , c.ContentText        as OptionText
	 , i.SurveyOptionItemIdentifier 
	 , q.SurveyQuestionIdentifier
	 , newid() as RandomIdentifier

from surveys.QSurveyQuestion             as q
     inner join surveys.QResponseAnswer  as a on a.SurveyQuestionIdentifier = q.SurveyQuestionIdentifier
     inner join surveys.QResponseSession as r on r.ResponseSessionIdentifier = a.ResponseSessionIdentifier
     inner join identities.[User]        as u on r.RespondentUserIdentifier = u.UserIdentifier
     left join(surveys.QSurveyOptionList            as l
               inner join surveys.QSurveyOptionItem as i on i.SurveyOptionListIdentifier = l.SurveyOptionListIdentifier
               inner join contents.TContent         as c on i.SurveyOptionItemIdentifier = c.ContainerIdentifier
                                                            and c.ContentLanguage = 'en'
                                                            and c.ContentLabel = 'Title'
               inner join surveys.QResponseOption   as o on o.SurveyOptionIdentifier = i.SurveyOptionItemIdentifier
                                                            and o.ResponseOptionIsSelected = 1)on l.SurveyQuestionIdentifier = q.SurveyQuestionIdentifier
                                                                                                  and o.ResponseSessionIdentifier = r.ResponseSessionIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [messages].[QFollower](
	[MessageIdentifier] [uniqueidentifier] NOT NULL,
	[SubscriberIdentifier] [uniqueidentifier] NOT NULL,
	[FollowerIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QFollower] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [messages].[XFollowerBase]
AS
    SELECT
        F.MessageIdentifier
      , F.SubscriberIdentifier
      , F.FollowerIdentifier
      , S.Subscribed
      , UF.Email    AS FollowerEmail
      , UF.FullName AS FollowerFullName
    FROM
        messages.QFollower             AS F
    INNER JOIN identities.[User]       AS UF
               ON F.FollowerIdentifier = UF.UserIdentifier

    LEFT JOIN messages.QSubscriberUser AS S
              ON F.SubscriberIdentifier = S.UserIdentifier
                 AND F.MessageIdentifier = S.MessageIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [messages].[VFollower]
as
select F.MessageIdentifier
     , M.MessageTitle
     , F.SubscriberIdentifier
     , F.FollowerIdentifier
     , F.Subscribed
     , F.FollowerEmail
     , F.FollowerFullName
     , US.Email    as SubscriberEmail
     , US.FullName as SubscriberName
from messages.XFollowerBase       as F
     inner join identities.[User] as US on F.SubscriberIdentifier = US.UserIdentifier
     inner join messages.QMessage as M on M.MessageIdentifier = F.MessageIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [identities].[VUser]
as
select U.UserIdentifier
     , U.Email                 as UserEmail
     , U.EmailAlternate        as UserEmailAlternate
     , U.FirstName             as UserFirstName
     , U.LastName              as UserLastName
     , U.FullName              as UserFullName
     , U.TimeZone              as UserTimeZone
     , cast((case
                 when U.UtcArchived is not null then
                     1
                 else
                     0
             end
            ) as bit)          as IsArchived
     , U.UtcArchived
     , U.PhoneMobile           as UserPhoneMobile
from identities.[User] as U;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [resources].[VUpload]
AS
    SELECT
              F.UploadIdentifier
            , F.OrganizationIdentifier
            , F.ContainerIdentifier
            , F.Uploader     AS UploaderIdentifier
            , F.UploadType
            , F.UploadPrivacyScope AS AccessType
            , F.Title
            , F.Name
            , F.Description
            , F.NavigateUrl
            , F.ContainerType
            , F.ContentFingerprint
            , F.ContentSize
            , F.ContentType
            , F.Uploaded
            , U.UserIdentifier      AS UploaderKey
            , U.UserFullName AS UploaderName
    FROM
              resources.Upload AS F
    LEFT JOIN identities.VUser AS U
              ON U.UserIdentifier = F.Uploader;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [reports].[Employer]
AS
    SELECT
        Employer.GroupIdentifier AS EmployerGroupIdentifier
      , Employer.GroupName            AS EmployerGroupName
      , Employer.GroupCategory    AS EmployerGroupCategory
      , EmployerDistrict.GroupName    AS EmployerDistrictName
      , Organization.OrganizationIdentifier         AS EmployerOrganizationIdentifier
      , Organization.CompanyName       AS EmployerOrganizationName
      , (
            SELECT COUNT(*)FROM contacts.Person AS U WHERE U.EmployerGroupIdentifier = Employer.GroupIdentifier
        )                        AS EmployeeCount
    FROM
        contacts.QGroup AS Employer
        LEFT JOIN
        contacts.QGroup AS EmployerDistrict ON EmployerDistrict.GroupIdentifier = Employer.ParentGroupIdentifier
        INNER JOIN
        accounts.QOrganization AS Organization ON Organization.OrganizationIdentifier = Employer.OrganizationIdentifier
    WHERE
        EXISTS
    (
        SELECT * FROM contacts.Person AS U WHERE U.EmployerGroupIdentifier = Employer.GroupIdentifier
    );
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [contacts].[CompanyDepartment]
as
select Organization.OrganizationIdentifier
  , Organization.OrganizationIdentifier     as CompanyKey
  , Organization.CompanyTitle  as CompanyName
  , Department.DepartmentIdentifier
  , Department.DepartmentName
from identities.Department
    inner join accounts.QOrganization AS Organization on Department.OrganizationIdentifier = Organization.OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [surveys].[VSurveyResponse] as
select
    SurveyFormIdentifier
	, ResponseSessionStarted
	, ResponseSessionCompleted
	, case when ResponseSessionStarted is not null and ResponseSessionCompleted is not null
		then datediff(minute, ResponseSessionStarted, ResponseSessionCompleted)
		else NULL end as TimeTaken
	, case when ResponseSessionStarted is not null
		then 1
		else 0 end as IsStarted,
	case when ResponseSessionCompleted is not null
		then 1
		else 0 end as IsCompleted
from
    surveys.QResponseSession
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [surveys].[VSurveyResponseSummary]
AS
SELECT SurveyFormIdentifier,
       MIN(ResponseSessionStarted) AS MinResponseStarted,
       MAX(ResponseSessionCompleted) AS MaxResponseCompleted,
       AVG(TimeTaken) AS AvgResponseTimeTaken,
       SUM(IsStarted) AS SumResponseStartCount,
       SUM(IsCompleted) AS SumResponseCompleteCount
FROM surveys.VSurveyResponse
GROUP BY SurveyFormIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [contacts].[UserRegistrationDetail]
AS
    SELECT
        u.UserIdentifier
      , Registration.RegistrationIdentifier
      , Organization.OrganizationIdentifier
      , Organization.CompanyName
      , Organization.OrganizationData          AS OrganizationSnapshot
      , u.LastName
      , u.FirstName
      , u.MiddleName
      , HomeAddress.Street1        AS HomeAddressStreet1
      , HomeAddress.Street2        AS HomeAddressStreet2
      , HomeAddress.City           AS HomeAddressCity
      , HomeAddress.Province       AS HomeAddressProvince
      , HomeAddress.PostalCode     AS HomeAddressPostalCode
      , HomeAddress.Country        AS HomeAddressCountry
      , ShippingAddress.Street1    AS ShippingAddressStreet1
      , ShippingAddress.Street2    AS ShippingAddressStreet2
      , ShippingAddress.City       AS ShippingAddressCity
      , ShippingAddress.Province   AS ShippingAddressProvince
      , ShippingAddress.PostalCode AS ShippingAddressPostalCode
      , ShippingAddress.Country    AS ShippingAddressCountry
      , Person.SocialInsuranceNumber
      , Person.PersonCode
      , u.Email
      , Achievement.AchievementTitle
      , Achievement.AchievementLabel
      , Achievement.AchievementDescription
      , Activity.EventTitle
      , Activity.EventScheduledStart
      , Activity.EventScheduledEnd
      , Activity.DurationQuantity
      , Activity.DurationUnit
      , Registration.RegistrationFee
      , Registration.ApprovalStatus
      , Registration.IncludeInT2202
    FROM
        identities.[User]                  AS u
    INNER JOIN contacts.Person
               ON Person.UserIdentifier = u.UserIdentifier

    INNER JOIN accounts.QOrganization AS Organization
               ON Organization.OrganizationIdentifier = Person.OrganizationIdentifier

    INNER JOIN registrations.QRegistration AS Registration
               ON Registration.CandidateIdentifier = u.UserIdentifier

    INNER JOIN events.QEvent               AS Activity
               ON Activity.EventIdentifier = Registration.EventIdentifier

    INNER JOIN achievements.QAchievement   AS Achievement
               ON Achievement.AchievementIdentifier = Activity.AchievementIdentifier

    LEFT JOIN locations.Address            AS HomeAddress
              ON HomeAddress.AddressIdentifier = Person.HomeAddressIdentifier

    LEFT JOIN locations.Address            AS ShippingAddress
              ON ShippingAddress.AddressIdentifier = Person.ShippingAddressIdentifier
    WHERE
        Person.IsLearner = 1;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [contacts].[VPerson]
as
select P.EmployeeUnion
     , P.IsAdministrator
     , P.IsLearner
     , P.PersonCode
     , P.SocialInsuranceNumber
     , P.OrganizationIdentifier
     , P.UserIdentifier
     , P.EmployerGroupIdentifier
     , G.GroupName             as EmployerGroupName
     , GS.ItemName             as EmployerGroupStatus
     , GS.ItemIdentifier       as EmployerGroupStatusItemIdentifier
     , G.GroupRegion           as EmployerGroupRegion
     , U.Email                 as UserEmail
     , U.EmailAlternate        as UserEmailAlternate
     , P.EmailEnabled          as UserEmailEnabled
     , P.EmailAlternateEnabled as UserEmailAlternateEnabled
     , U.FirstName             as UserFirstName
     , U.LastName              as UserLastName
     , U.FullName              as UserFullName
     , P.Phone                 as UserPhone
     , U.PhoneMobile           as UserPhoneMobile
     , U.TimeZone              as UserTimeZone
     , P.BillingAddressIdentifier
     , P.HomeAddressIdentifier
     , P.ShippingAddressIdentifier
     , P.WorkAddressIdentifier
     , cast((case
                 when U.UtcArchived is not null then
                     1
                 else
                     0
             end
            ) as bit)          as IsArchived
     , U.UtcArchived
     , P.TradeworkerNumber
     , P.Birthdate
     , P.Region
     , P.EmergencyContactName
     , P.EmergencyContactPhone
     , P.EmergencyContactRelationship
     , P.Language
     , P.CandidateIsActivelySeeking
     , P.FirstLanguage
from contacts.Person             as P
     left join identities.[User] as U on U.UserIdentifier = P.UserIdentifier
     left join contacts.QGroup   as G on P.EmployerGroupIdentifier = G.GroupIdentifier
     left join utilities.TCollectionItem as GS on GS.ItemIdentifier = G.GroupStatusItemIdentifier
where P.OrganizationIdentifier in (
                                      select OrganizationIdentifier from accounts.QOrganization
                                  );
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandardGroup](
	[StandardIdentifier] [uniqueidentifier] NOT NULL,
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[Assigned] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_QStandardGroup] PRIMARY KEY CLUSTERED 
(
	[StandardIdentifier] ASC,
	[GroupIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [identities].[TDepartmentStandard]
AS
SELECT
    Assigned
   ,GroupIdentifier AS DepartmentIdentifier
   ,StandardIdentifier
   ,OrganizationIdentifier
FROM
    [standard].QStandardGroup
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[DepartmentProfile]
as
select TDepartmentStandard.StandardIdentifier   as ProfileStandardIdentifier
     , TDepartmentStandard.DepartmentIdentifier as DepartmentIdentifier
from identities.TDepartmentStandard
     inner join standards.Standard on TDepartmentStandard.StandardIdentifier = Standard.StandardIdentifier
                                      and StandardType = 'Profile'
     inner join accounts.QOrganization as T on T.OrganizationIdentifier = Standard.OrganizationIdentifier
     inner join identities.Department on Department.DepartmentIdentifier = TDepartmentStandard.DepartmentIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [custom_cmds].[Employment]
as
select Department.DepartmentIdentifier
  , DepartmentProfileUser.ProfileStandardIdentifier
  , DepartmentProfileUser.UserIdentifier
  , Department.OrganizationIdentifier
from standards.DepartmentProfileUser
    inner join identities.Department on Department.DepartmentIdentifier = DepartmentProfileUser.DepartmentIdentifier
where IsPrimary = 1;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[TCollegeCertificate](
	[CertificateIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerIdentifier] [uniqueidentifier] NOT NULL,
	[ProfileIdentifier] [uniqueidentifier] NOT NULL,
	[CertificateTitle] [varchar](100) NOT NULL,
	[CertificateAuthority] [varchar](100) NOT NULL,
	[DateRequested] [datetimeoffset](7) NULL,
	[DateSubmitted] [datetimeoffset](7) NULL,
	[DateGranted] [datetimeoffset](7) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_TCollegeCertificate] PRIMARY KEY CLUSTERED 
(
	[CertificateIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[ProfileCertification]
as
select P.StandardIdentifier as ProfileStandardIdentifier
     , DateRequested
     , DateGranted
     , DateSubmitted
     , LearnerIdentifier    as UserIdentifier
     , CertificateAuthority as AuthorityName
from custom_cmds.TCollegeCertificate   as C
     inner join standards.Standard     as P on P.StandardIdentifier = C.ProfileIdentifier
                                               and P.StandardType = 'Profile'
     inner join accounts.QOrganization as T on T.OrganizationIdentifier = P.OrganizationIdentifier
     inner join identities.[User] on C.LearnerIdentifier = [User].UserIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[ProfileCompetency]
as
select Profile.StandardIdentifier    as ProfileStandardIdentifier
     , Competency.StandardIdentifier as CompetencyStandardIdentifier
     , case
           when CreditType = 'Core'
                and StandardContainment.CreditHours is not null then
               StandardContainment.CreditHours
           else
               null
       end                           as CertificationHoursCore
     , case
           when CreditType = 'Non-Core'
                and StandardContainment.CreditHours is not null then
               StandardContainment.CreditHours
           else
               null
       end                           as CertificationHoursNonCore
from standards.StandardContainment
     inner join standards.Standard     as Profile on StandardContainment.ParentStandardIdentifier = Profile.StandardIdentifier
                                                     and Profile.StandardType = 'Profile'
     inner join accounts.QOrganization as T on T.OrganizationIdentifier = Profile.OrganizationIdentifier
     inner join standards.Standard     as Competency on StandardContainment.ChildStandardIdentifier = Competency.StandardIdentifier
                                                        and Competency.StandardType = 'Competency';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [record].[TAchievementCategory](
	[AchievementIdentifier] [uniqueidentifier] NOT NULL,
	[ItemIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[CategorySequence] [int] NULL,
	[CategoryDescription] [varchar](800) NULL,
 CONSTRAINT [PK_TAchievementCategory] PRIMARY KEY NONCLUSTERED 
(
	[AchievementIdentifier] ASC,
	[ItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[VCmdsAchievementCategory] AS SELECT
                                                       A.AchievementIdentifier, I.ItemIdentifier AS CategoryIdentifier,
                                                       I.ItemName AS CategoryName,
                                                       I.ItemDescription AS CategoryDescription,
                                                       I.ItemSequence AS ClassificationSequence,
                                                       O.OrganizationIdentifier, A.AchievementLabel
                                                   FROM
                                                       record.TAchievementCategory AS G
                                                       INNER JOIN utilities.TCollectionItem AS I
                                                       ON I.ItemIdentifier = G.ItemIdentifier
                                                       INNER JOIN achievements.QAchievement AS A
                                                       ON G.AchievementIdentifier = A.AchievementIdentifier
                                                       INNER JOIN accounts.QOrganization AS O
                                                       ON O.OrganizationIdentifier = A.OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [achievements].[TAchievementDepartment](
	[Assigned] [datetimeoffset](7) NULL,
	[Modified] [datetimeoffset](7) NULL,
	[AchievementIdentifier] [uniqueidentifier] NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_TAchievementDepartment] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[VCmdsAchievementDepartment]
as
select AD.AchievementIdentifier
     , AD.DepartmentIdentifier
from achievements.TAchievementDepartment  as AD
     inner join achievements.QAchievement as A on A.AchievementIdentifier = AD.AchievementIdentifier
     inner join identities.Department     as D on D.DepartmentIdentifier = AD.DepartmentIdentifier
     inner join accounts.QOrganization    as T on T.OrganizationIdentifier = D.OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [custom_cmds].[VCmdsAchievementOrganization]
as
select XY.AchievementIdentifier
     , XY.OrganizationIdentifier
from achievements.TAchievementOrganization as XY
     inner join achievements.QAchievement  as X on X.AchievementIdentifier = XY.AchievementIdentifier
     inner join accounts.QOrganization     as Y on XY.OrganizationIdentifier = Y.OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[VCmdsCompetencyOrganization]
AS

SELECT DISTINCT
       X.StandardIdentifier AS CompetencyStandardIdentifier
     , XY.OrganizationIdentifier

FROM standards.StandardOrganization AS XY
  INNER JOIN standards.Standard AS X
    ON X.StandardIdentifier = XY.StandardIdentifier
       AND X.StandardType = 'Competency'
  INNER JOIN accounts.QOrganization AS T
    ON T.OrganizationIdentifier = X.OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[VCmdsProfileOrganization]
AS

SELECT DISTINCT
       X.StandardIdentifier AS ProfileStandardIdentifier
     , XY.OrganizationIdentifier
FROM standards.StandardOrganization AS XY
  INNER JOIN standards.Standard AS X
    ON X.StandardIdentifier = XY.StandardIdentifier
       AND X.StandardType = 'Profile'
  INNER JOIN accounts.QOrganization AS Y
    ON Y.OrganizationIdentifier = X.OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QGroupConnection](
	[ChildGroupIdentifier] [uniqueidentifier] NOT NULL,
	[ParentGroupIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QGroupConnection] PRIMARY KEY CLUSTERED 
(
	[ChildGroupIdentifier] ASC,
	[ParentGroupIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   FUNCTION [contacts].[GetGroupDescendentRelationships]
(
    @RootGroupIdentifier uniqueidentifier
)
RETURNS TABLE
AS
RETURN
    WITH Relationships AS (
        SELECT
            ParentGroupIdentifier
           ,GroupIdentifier AS ChildGroupIdentifier
           ,CAST(1 AS bit) AS IsHierarchy
        FROM
            contacts.QGroup WITH (NOLOCK)
        WHERE
            ParentGroupIdentifier IS NOT NULL

        UNION

        SELECT
            ParentGroupIdentifier
           ,ChildGroupIdentifier
           ,CAST(0 AS bit) AS IsHierarchy
        FROM
            contacts.QGroupConnection WITH (NOLOCK)
    ), Hierarchy AS (
        SELECT
            CAST(null as uniqueidentifier) AS ParentGroupIdentifier
           ,@RootGroupIdentifier AS ChildGroupIdentifier
           ,CAST(NULL AS bit) AS IsHierarchy
           ,0 AS Depth

        UNION ALL

        SELECT
            Hierarchy.ChildGroupIdentifier
           ,Relationships.ChildGroupIdentifier
           ,Relationships.IsHierarchy
           ,Hierarchy.Depth + 1
        FROM
            Relationships
            INNER JOIN Hierarchy ON Hierarchy.ChildGroupIdentifier = Relationships.ParentGroupIdentifier
    )
    SELECT * FROM Hierarchy;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [settings].[TAction](
	[ActionIdentifier] [uniqueidentifier] NOT NULL,
	[NavigationParentActionIdentifier] [uniqueidentifier] NULL,
	[PermissionParentActionIdentifier] [uniqueidentifier] NULL,
	[ActionIcon] [varchar](30) NULL,
	[ActionList] [varchar](40) NULL,
	[ActionName] [varchar](100) NOT NULL,
	[ActionNameShort] [varchar](40) NULL,
	[ActionType] [varchar](20) NULL,
	[ActionUrl] [varchar](500) NULL,
	[ControllerPath] [varchar](100) NULL,
	[HelpUrl] [varchar](500) NULL,
	[AuthorizationRequirement] [varchar](34) NULL,
	[AuthorityType] [varchar](20) NULL,
	[ExtraBreadcrumb] [varchar](40) NULL,
 CONSTRAINT [PK_TAction] PRIMARY KEY CLUSTERED 
(
	[ActionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [setup].[VRouteNavigationNode] AS
-- This is the action navigation tree. A child node navigates back to its parent node.
WITH Hierarchy
         AS ( SELECT NavigationParentActionIdentifier                                         AS ParentActionId,
                     ActionIdentifier                                                         AS ActionId,
                     ActionType,
                     AuthorityType,
                     AuthorizationRequirement,
                     ExtraBreadcrumb,
                     0                                                                        AS ActionDepth,
                     ActionList,
                     ActionIcon,
                     ActionName,
                     ActionNameShort,
                     ActionUrl,
                     HelpUrl,
                     ControllerPath,
                     CAST('/' + CAST(ActionIdentifier AS VARCHAR(MAX)) + '/' AS VARCHAR(MAX)) AS Path,
                     CAST(ActionUrl AS VARCHAR(MAX))                                          AS SortPath
              FROM settings.TAction
              WHERE NavigationParentActionIdentifier IS NULL
              UNION ALL
              SELECT Child.NavigationParentActionIdentifier,
                     Child.ActionIdentifier,
                     Child.ActionType,
                     Child.AuthorityType,
                     Child.AuthorizationRequirement,
                     Child.ExtraBreadcrumb,
                     Hierarchy.ActionDepth + 1,
                     Child.ActionList,
                     Child.ActionIcon,
                     Child.ActionName,
                     Child.ActionNameShort,
                     Child.ActionUrl,
                     Child.HelpUrl,
                     Child.ControllerPath,
                     CAST(Hierarchy.Path + CAST(Child.ActionIdentifier AS VARCHAR(MAX)) + '/' AS VARCHAR(MAX)),
                     CAST(Hierarchy.SortPath + '/' + Child.ActionUrl AS VARCHAR(MAX))
              FROM Hierarchy
                       INNER JOIN settings.TAction AS Child
                                  ON Child.NavigationParentActionIdentifier IS NOT NULL
                                      AND Child.NavigationParentActionIdentifier = Hierarchy.ActionId
                                      AND Hierarchy.Path NOT LIKE
                                          '%/' + CAST(Child.ActionIdentifier AS VARCHAR(MAX)) + '/%' -- Prevent cycles
    )
SELECT Hierarchy.ActionDepth as RouteDepth,
       Hierarchy.ActionIcon as RouteIcon,
       Hierarchy.ActionId as RouteId,
       Hierarchy.ActionList as RouteList,
       Hierarchy.ActionName as RouteName,
       Hierarchy.ActionNameShort as RouteNameShort,
       Hierarchy.ActionType as RouteType,
       Hierarchy.ActionUrl as RouteUrl,
       REPLICATE('   ', Hierarchy.ActionDepth) + ' - ' + Hierarchy.ActionUrl AS RouteUrlIndented,
       Hierarchy.AuthorityType,
       Hierarchy.AuthorizationRequirement,
       Hierarchy.ControllerPath,
       Hierarchy.ExtraBreadcrumb,
       Hierarchy.HelpUrl,
       Hierarchy.ParentActionId as ParentRouteId,
       Hierarchy.SortPath
FROM Hierarchy
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [setup].[VRoutePermissionNode] AS
-- This is the action permission tree. A child node inherits permissions granted to its parent node.
WITH Hierarchy
         AS ( SELECT PermissionParentActionIdentifier                                         AS ParentActionId,
                     ActionIdentifier                                                         AS ActionId,
                     ActionType,
                     AuthorityType,
                     AuthorizationRequirement,
                     ExtraBreadcrumb,
                     0                                                                        AS ActionDepth,
                     ActionList,
                     ActionIcon,
                     ActionName,
                     ActionNameShort,
                     ActionUrl,
                     HelpUrl,
                     ControllerPath,
                     CAST('/' + CAST(ActionIdentifier AS VARCHAR(MAX)) + '/' AS VARCHAR(MAX)) AS Path,
                     CAST(ActionUrl AS VARCHAR(MAX))                                          AS SortPath
              FROM settings.TAction
              WHERE PermissionParentActionIdentifier IS NULL
              UNION ALL
              SELECT Child.PermissionParentActionIdentifier,
                     Child.ActionIdentifier,
                     Child.ActionType,
                     Child.AuthorityType,
                     Child.AuthorizationRequirement,
                     Child.ExtraBreadcrumb,
                     Hierarchy.ActionDepth + 1,
                     Child.ActionList,
                     Child.ActionIcon,
                     Child.ActionName,
                     Child.ActionNameShort,
                     Child.ActionUrl,
                     Child.HelpUrl,
                     Child.ControllerPath,
                     CAST(Hierarchy.Path + CAST(Child.ActionIdentifier AS VARCHAR(MAX)) + '/' AS VARCHAR(MAX)),
                     CAST(Hierarchy.SortPath + '/' + Child.ActionUrl AS VARCHAR(MAX))
              FROM Hierarchy
                       INNER JOIN settings.TAction AS Child
                                  ON Child.PermissionParentActionIdentifier IS NOT NULL
                                      AND Child.PermissionParentActionIdentifier = Hierarchy.ActionId
                                      AND Hierarchy.Path NOT LIKE
                                          '%/' + CAST(Child.ActionIdentifier AS VARCHAR(MAX)) + '/%' -- Prevent cycles
    )
SELECT Hierarchy.ActionDepth as RouteDepth,
       Hierarchy.ActionIcon as RouteIcon,
       Hierarchy.ActionId as RouteId,
       Hierarchy.ActionList as RouteList,
       Hierarchy.ActionName as RouteName,
       Hierarchy.ActionNameShort as RouteNameShort,
       Hierarchy.ActionType as RouteType,
       Lower(Hierarchy.ActionUrl) as RouteUrl,
       REPLICATE('   ', Hierarchy.ActionDepth) + ' - ' + Lower(Hierarchy.ActionUrl) AS RouteUrlIndented,
       Hierarchy.AuthorityType,
       Hierarchy.AuthorizationRequirement,
       Hierarchy.ControllerPath,
       Hierarchy.ExtraBreadcrumb,
       lower(Hierarchy.HelpUrl) as HelpUrl,
       Hierarchy.ParentActionId as ParentRouteId,
       Hierarchy.SortPath
FROM Hierarchy
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [security].[TUserSession](
	[AuthenticationErrorType] [varchar](256) NULL,
	[AuthenticationErrorMessage] [varchar](200) NULL,
	[SessionCode] [varchar](32) NOT NULL,
	[SessionIsAuthenticated] [bit] NOT NULL,
	[SessionStarted] [datetimeoffset](7) NOT NULL,
	[SessionStopped] [datetimeoffset](7) NULL,
	[SessionMinutes] [int] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserAgent] [varchar](512) NULL,
	[UserBrowser] [varchar](32) NOT NULL,
	[UserBrowserVersion] [varchar](32) NULL,
	[UserEmail] [varchar](254) NOT NULL,
	[UserHostAddress] [varchar](16) NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[UserLanguage] [varchar](5) NOT NULL,
	[SessionIdentifier] [uniqueidentifier] NOT NULL,
	[AuthenticationSource] [varchar](19) NOT NULL,
 CONSTRAINT [PK_TUserSession] PRIMARY KEY CLUSTERED 
(
	[SessionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [accounts].[VUserSession]
AS
    SELECT
        A.AuthenticationErrorType,
        A.AuthenticationErrorMessage,
        A.SessionCode,
        A.SessionIsAuthenticated,
        A.SessionStarted,
        A.SessionStopped,
        A.SessionMinutes,
        A.OrganizationIdentifier,
        A.UserAgent,
        A.UserBrowser,
        A.UserBrowserVersion,
        A.UserEmail,
        A.UserHostAddress,
        A.UserIdentifier,
        A.UserLanguage
      , U.FirstName   AS UserFirstName
      , U.LastName    AS UserLastName
    FROM
        [security].TUserSession     AS A
    INNER JOIN identities.[User] AS U
               ON U.UserIdentifier = A.UserIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [security].[RoleRouteOperation] AS

SELECT O.OrganizationIdentifier,
       O.OrganizationCode,
       G.GroupName        AS RoleName,
       lower(A.ActionUrl) AS RouteUrl,

       GP.AllowExecute,
       GP.AllowRead,
       GP.AllowWrite,
       GP.AllowCreate,
       GP.AllowDelete,
       GP.AllowAdministrate,
       GP.AllowConfigure

FROM contacts.TGroupPermission AS GP
         INNER JOIN contacts.QGroup AS G ON G.GroupIdentifier = GP.GroupIdentifier
         INNER JOIN settings.TAction AS A ON A.ActionIdentifier = GP.ObjectIdentifier
         INNER JOIN accounts.QOrganization AS O ON O.OrganizationIdentifier = G.OrganizationIdentifier

WHERE ObjectType = 'Action';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [achievements].[VAchievement]
AS
    SELECT
        OrganizationIdentifier
      , AchievementIdentifier
      , AchievementLabel
      , AchievementTitle
      , AchievementDescription
      , AchievementIsEnabled
      , CertificateLayoutCode
      , ExpirationType
      , ExpirationFixedDate
      , ExpirationLifetimeQuantity
      , ExpirationLifetimeUnit
      , (
            SELECT
                COUNT(*)
            FROM
                achievements.QCredential AS C
            WHERE
                C.AchievementIdentifier = A.AchievementIdentifier
        ) AS CredentialCount
    FROM
        achievements.QAchievement AS A;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QEnrollment](
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerIdentifier] [uniqueidentifier] NOT NULL,
	[PeriodIdentifier] [uniqueidentifier] NULL,
	[EnrollmentIdentifier] [uniqueidentifier] NOT NULL,
	[EnrollmentStarted] [datetimeoffset](7) NULL,
	[EnrollmentComment] [varchar](400) NULL,
	[EnrollmentRestart] [int] NOT NULL,
	[EnrollmentCompleted] [datetimeoffset](7) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QEnrollment] PRIMARY KEY CLUSTERED 
(
	[EnrollmentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [courses].[QModule](
	[ModuleIdentifier] [uniqueidentifier] NOT NULL,
	[UnitIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ModuleAsset] [int] NOT NULL,
	[ModuleName] [varchar](200) NOT NULL,
	[ModuleCode] [varchar](30) NULL,
	[ModuleImage] [varchar](200) NULL,
	[ModuleSequence] [int] NOT NULL,
	[SourceIdentifier] [uniqueidentifier] NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[ModuleIsAdaptive] [bit] NOT NULL,
	[PrerequisiteDeterminer] [varchar](3) NULL,
 CONSTRAINT [PK_QModule] PRIMARY KEY CLUSTERED 
(
	[ModuleIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [courses].[QUnit](
	[UnitIdentifier] [uniqueidentifier] NOT NULL,
	[CourseIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UnitAsset] [int] NOT NULL,
	[UnitName] [varchar](200) NOT NULL,
	[UnitCode] [varchar](30) NULL,
	[UnitSequence] [int] NOT NULL,
	[SourceIdentifier] [uniqueidentifier] NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[UnitIsAdaptive] [bit] NOT NULL,
	[PrerequisiteDeterminer] [varchar](3) NULL,
 CONSTRAINT [PK_QUnit] PRIMARY KEY CLUSTERED 
(
	[UnitIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [courses].[QActivity](
	[ActivityIdentifier] [uniqueidentifier] NOT NULL,
	[AssessmentFormIdentifier] [uniqueidentifier] NULL,
	[GradeItemIdentifier] [uniqueidentifier] NULL,
	[ModuleIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[PrerequisiteActivityIdentifier] [uniqueidentifier] NULL,
	[SurveyFormIdentifier] [uniqueidentifier] NULL,
	[QuizIdentifier] [uniqueidentifier] NULL,
	[ActivityAsset] [int] NOT NULL,
	[ActivityAuthorName] [varchar](100) NULL,
	[ActivityAuthorDate] [date] NULL,
	[ActivityCode] [varchar](30) NULL,
	[ActivityHook] [varchar](100) NULL,
	[ActivityImage] [varchar](200) NULL,
	[ActivityIsMultilingual] [bit] NOT NULL,
	[ActivityIsAdaptive] [bit] NOT NULL,
	[ActivityIsDispatch] [bit] NOT NULL,
	[ActivityMinutes] [int] NULL,
	[ActivityMode] [varchar](7) NULL,
	[ActivityName] [varchar](200) NOT NULL,
	[ActivityPlatform] [varchar](100) NULL,
	[ActivitySequence] [int] NOT NULL,
	[ActivityType] [varchar](30) NOT NULL,
	[ActivityUrl] [varchar](500) NULL,
	[ActivityUrlTarget] [varchar](500) NULL,
	[ActivityUrlType] [varchar](500) NULL,
	[PrerequisiteDeterminer] [varchar](3) NULL,
	[RequirementCondition] [varchar](30) NULL,
	[SourceIdentifier] [uniqueidentifier] NULL,
	[DoneButtonText] [varchar](24) NULL,
	[DoneButtonInstructions] [varchar](max) NULL,
	[DoneMarkedInstructions] [varchar](max) NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_QActivity] PRIMARY KEY CLUSTERED 
(
	[ActivityIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [courses].[VCourse]
AS
SELECT
    QCourse.CourseIdentifier
   ,QCourse.CatalogIdentifier
   ,QCourse.OrganizationIdentifier
   ,QCourse.CourseCode
   ,QCourse.CourseHook
   ,QCourse.CourseLabel
   ,QCourse.CourseName
   ,QGradebook.GradebookIdentifier
   ,QGradebook.GradebookTitle
   ,TCatalog.CatalogName
   ,ISNULL(HierarchyHelper.UnitCount, 0) AS UnitCount
   ,ISNULL(HierarchyHelper.ModuleCount, 0) AS ModuleCount
   ,ISNULL(HierarchyHelper.ActivityCount, 0) AS ActivityCount
   ,(SELECT ISNULL(COUNT(*), 0)
     FROM records.QEnrollment
     WHERE QEnrollment.GradebookIdentifier = QGradebook.GradebookIdentifier
       AND QEnrollment.EnrollmentStarted IS NOT NULL
    ) AS EnrollmentStarted
   ,(SELECT ISNULL(COUNT(*), 0)
     FROM records.QEnrollment
     WHERE QEnrollment.GradebookIdentifier = QGradebook.GradebookIdentifier
       AND (
            EXISTS(
                SELECT TOP 1 1
                FROM achievements.QCredential
                WHERE
                    QCredential.UserIdentifier = QEnrollment.LearnerIdentifier
                    AND QCredential.AchievementIdentifier = QGradebook.AchievementIdentifier
            ) OR EXISTS(
                SELECT TOP 1 1
                FROM achievements.QCredential
                WHERE
                    QCredential.UserIdentifier = QEnrollment.LearnerIdentifier
                    AND QCredential.AchievementIdentifier IN (
                        SELECT AchievementIdentifier
                        FROM records.QGradeItem
                        WHERE QGradeItem.GradebookIdentifier = QGradebook.GradebookIdentifier
                    )
            )
       )
    ) AS EnrollmentCompleted
FROM
    courses.QCourse
    LEFT JOIN records.QGradebook
        ON QGradebook.GradebookIdentifier = QCourse.GradebookIdentifier
    LEFT JOIN learning.TCatalog
        ON TCatalog.CatalogIdentifier = QCourse.CatalogIdentifier
    LEFT JOIN (
        SELECT
            QUnit.CourseIdentifier
           ,COUNT(DISTINCT QUnit.UnitIdentifier) AS UnitCount
           ,COUNT(DISTINCT QModule.ModuleIdentifier) AS ModuleCount
           ,COUNT(DISTINCT QActivity.ActivityIdentifier) AS ActivityCount
        FROM
            courses.QUnit
            LEFT JOIN courses.QModule
                ON QModule.UnitIdentifier = QUnit.UnitIdentifier
            LEFT JOIN courses.QActivity
                ON QActivity.ModuleIdentifier = QModule.ModuleIdentifier
        GROUP BY
            QUnit.CourseIdentifier
    ) AS HierarchyHelper
        ON HierarchyHelper.CourseIdentifier = QCourse.CourseIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [contacts].[GroupHierarchy] AS
    WITH Child AS
        (
            SELECT
                GroupIdentifier AS ChildGroupIdentifier
              , ParentGroupIdentifier
              , OrganizationIdentifier
              , GroupCode
              , GroupName
              , GroupType
            FROM
                contacts.QGroup
            WHERE
                ParentGroupIdentifier IS NOT NULL
            UNION
            SELECT
                ChildGroupIdentifier
              , QGroupConnection.ParentGroupIdentifier
              , X.OrganizationIdentifier
              , GroupCode
              , GroupName
              , GroupType
            FROM
                contacts.QGroupConnection
                INNER JOIN
                contacts.QGroup AS X ON X.GroupIdentifier = ChildGroupIdentifier
        )
       , Hierarchy AS
        (
            SELECT
                GroupIdentifier                                          AS GroupIdentifier
              , ParentGroupIdentifier                                    AS ParentGroupIdentifier
              , OrganizationIdentifier                                         AS OrganizationIdentifier
              , CAST(ISNULL(GroupCode, GroupName) AS NVARCHAR(MAX)) AS PathName
              , CAST(GroupIdentifier AS VARCHAR(MAX))                    AS PathKey
              , 0                                                 AS PathDepth
              , GroupName                                         AS GroupName
              , GroupType
            FROM
                contacts.QGroup
            WHERE
                ParentGroupIdentifier IS NULL
            UNION ALL
            (SELECT
                 Child.ChildGroupIdentifier
               , Child.ParentGroupIdentifier
               , Child.OrganizationIdentifier
               , CAST(Parent.PathName + ' | ' + ISNULL(Child.GroupCode, Child.GroupName) AS NVARCHAR(MAX))
               , CAST(Parent.PathKey + ',' + CAST(Child.ChildGroupIdentifier AS VARCHAR(MAX)) AS VARCHAR(MAX))
               , Parent.PathDepth + 1
               , Child.GroupName
               , Child.GroupType AS SubType
             FROM
                 Child
                 INNER JOIN
                 Hierarchy AS Parent ON Child.ParentGroupIdentifier = Parent.GroupIdentifier)
        )
    SELECT
        *
      , SUBSTRING('######', 1, PathDepth + 1) AS PathIndent
    FROM
        Hierarchy;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [contacts].[GroupToolkitPermissionSummary]
as
select G.GroupIdentifier
     , G.GroupName
     , A.ActionName
     , GP.AllowRead
     , GP.AllowWrite
     , GP.AllowCreate
     , GP.AllowDelete
     , GP.AllowAdministrate
     , GP.AllowConfigure as AllowFullControl
from contacts.TGroupPermission   as GP
     inner join contacts.QGroup  as G on GP.GroupIdentifier = G.GroupIdentifier
     inner join settings.TAction as A on GP.ObjectIdentifier = A.ActionIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [standard].[GetStandardDescendentRelationships]
(
    @RootStandardIdentifier uniqueidentifier,
    @IncludeHierarchy bit,
    @IncludeContainment bit,
    @IncludeConnection bit
)
RETURNS TABLE
AS
RETURN
    WITH Relationships AS (
        SELECT
            ParentStandardIdentifier
           ,StandardIdentifier AS ChildStandardIdentifier
           ,CAST(1 AS bit) AS IsHierarchy
           ,CAST(0 AS bit) AS IsContainment
           ,CAST(0 AS bit) AS IsConnection
        FROM
            [standard].QStandard WITH (NOLOCK)
        WHERE
            ParentStandardIdentifier IS NOT NULL

        UNION

        SELECT
            ParentStandardIdentifier
           ,ChildStandardIdentifier
           ,CAST(0 AS bit) AS IsHierarchy
           ,CAST(1 AS bit) AS IsContainment
           ,CAST(0 AS bit) AS IsConnection
        FROM
            [standard].QStandardContainment WITH (NOLOCK)

        UNION

        SELECT
            FromStandardIdentifier
           ,ToStandardIdentifier
           ,CAST(0 AS bit) AS IsHierarchy
           ,CAST(0 AS bit) AS IsContainment
           ,CAST(1 AS bit) AS IsConnection
        FROM
            [standard].QStandardConnection WITH (NOLOCK)
    ), Hierarchy AS (
        SELECT
            CAST(null as uniqueidentifier) AS ParentStandardIdentifier
           ,@RootStandardIdentifier AS ChildStandardIdentifier
           ,CAST(NULL AS bit) AS IsHierarchy
           ,CAST(NULL AS bit) AS IsContainment
           ,CAST(NULL AS bit) AS IsConnection
           ,0 AS Depth

        UNION ALL

        SELECT
            Hierarchy.ChildStandardIdentifier
           ,Relationships.ChildStandardIdentifier
           ,Relationships.IsHierarchy
           ,Relationships.IsContainment
           ,Relationships.IsConnection
           ,Hierarchy.Depth + 1
        FROM
            Relationships
            INNER JOIN Hierarchy ON Hierarchy.ChildStandardIdentifier = Relationships.ParentStandardIdentifier
        WHERE
            (Relationships.IsHierarchy = 0 OR Relationships.IsHierarchy = @IncludeHierarchy)
            AND (Relationships.IsContainment = 0 OR Relationships.IsContainment = @IncludeContainment)
            AND (Relationships.IsConnection = 0 OR Relationships.IsConnection = @IncludeConnection)
    )
    SELECT * FROM Hierarchy;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [contacts].[VGroupAction]
as
select GP.AllowRead
     , GP.AllowWrite
     , GP.AllowCreate
     , GP.AllowDelete
     , GP.AllowAdministrate
     , GP.AllowConfigure   as AllowFullControl
     , GP.GroupIdentifier
     , GP.ObjectIdentifier as ActionIdentifier
     , A.ActionName
from contacts.TGroupPermission  as GP
    inner join settings.TAction as A
        on GP.ObjectIdentifier = A.ActionIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [contacts].[VGroupDetail]
as
select QGroup.AddNewUsersAutomatically
     , QGroup.AllowSelfSubscription
     , QGroup.GroupCapacity
     , QGroup.GroupCategory
     , QGroup.GroupCode
     , QGroup.GroupCreated
     , QGroup.GroupDescription
     , QGroup.GroupEmail
     , QGroup.GroupFax
     , QGroup.GroupIdentifier
     , QGroup.GroupImage
     , QGroup.GroupIndustry
     , QGroup.GroupIndustryComment
     , QGroup.GroupLabel
     , QGroup.GroupName
     , QGroup.GroupOffice
     , QGroup.GroupPhone
     , QGroup.GroupRegion
     , QGroup.GroupSize
     , TCollectionItem.ItemName AS GroupStatus
     , TCollectionItem.ItemIdentifier AS GroupStatusItemIdentifier
     , QGroup.GroupType
     , QGroup.GroupWebSiteUrl
     , QGroup.LifetimeQuantity
     , QGroup.LifetimeUnit
     
	 , QGroup.MessageToAdminWhenEventVenueChanged
	 , QGroup.MessageToAdminWhenMembershipEnded
     , QGroup.MessageToAdminWhenMembershipStarted
     
	 , QGroup.MessageToUserWhenMembershipEnded
	 , QGroup.MessageToUserWhenMembershipStarted
	 
     , QGroup.OrganizationIdentifier
     , QGroup.ParentGroupIdentifier
     , QGroup.ShippingPreference
     , QGroup.SurveyFormIdentifier
     , QGroup.SurveyNecessity

     , QGroup.OnlyOperatorCanAddUser
from contacts.QGroup
    left join utilities.TCollectionItem
        on TCollectionItem.ItemIdentifier = QGroup.GroupStatusItemIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[TGroupSetting](
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[SettingIdentifier] [uniqueidentifier] NOT NULL,
	[SettingName] [varchar](100) NOT NULL,
	[SettingValue] [varchar](max) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_TGroupSetting] PRIMARY KEY CLUSTERED 
(
	[SettingIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [contacts].[VGroupEmployer]
AS
SELECT
    M.[GroupIdentifier]
   ,M.[UserIdentifier]
   ,M.Assigned AS AssignedEmployerContactDate
   ,g.ParentGroupIdentifier                                                                 AS ParentIdentifier
   ,g.OrganizationIdentifier
   ,p.HomeAddressIdentifier                                                                 AS EmployeeHomeAddressIdentifier
   ,p.WorkAddressIdentifier                                                                 AS EmployeeWorkAddressIdentifier
   ,p.BillingAddressIdentifier                                                              AS EmployeeBillingAddressIdentifier
   ,p.ShippingAddressIdentifier                                                             AS EmployeeShippingAddressIdentifier
   ,p.Created AS EmployerContactCreated
   ,g.GroupCode
   ,g.GroupCreated
   ,g.LastChangeTime
   ,g.LastChangeType
   ,g.LastChangeUser
   ,g.GroupName
   ,g.GroupPhone
   ,g.GroupIndustry
   ,g.GroupIndustryComment
   ,g.GroupSize
   ,u.Email
   ,u.FirstName
   ,u.MiddleName
   ,u.LastName
   ,a.Country                                                                               AS AddressCountry
   ,a.City                                                                                  AS AddressCity
   ,a.Street1                                                                               AS AddressLine
   ,a.Province                                                                              AS AddressProvince
   ,a.PostalCode                                                                            AS AddressPostalCode
   ,p.Phone                                                                                 AS Phone1
   ,u.PhoneMobile                                                                           AS Phone2
   ,p.WebSiteUrl                                                                            AS Url
   ,CASE
        WHEN u.MiddleName IS NULL
            THEN CONCAT(CONCAT(RTRIM(ISNULL(u.FirstName, '')),' '), RTRIM(ISNULL(u.LastName, '')))
        ELSE CONCAT(CONCAT(RTRIM(ISNULL(u.FirstName, '')),' '), CONCAT(RTRIM(u.MiddleName),' '), RTRIM(ISNULL(u.LastName, '')))
    END AS ContactFullName
   ,p.JobTitle                                                                              AS ContactJobTitle
   ,p.JobsApproved                                                                         AS Approved
   ,(
        SELECT MIN(s.SettingValue)
        FROM contacts.TGroupSetting AS s
        WHERE s.GroupIdentifier = g.GroupIdentifier
          AND s.SettingName = 'Company Sector'
    ) AS CompanySector                 

FROM [contacts].[Membership] AS M WITH (NOLOCK)
    INNER JOIN contacts.QGroup AS g WITH (NOLOCK) ON g.GroupIdentifier = M.[GroupIdentifier]
    INNER JOIN (contacts.Person AS p WITH(NOLOCK)
    INNER JOIN identities.[User] AS u WITH(NOLOCK) ON u.UserIdentifier = p.UserIdentifier) ON p.UserIdentifier = M.[UserIdentifier] AND p.OrganizationIdentifier = g.OrganizationIdentifier
    LEFT JOIN contacts.QGroupAddress AS a WITH(NOLOCK) ON a.GroupIdentifier = g.GroupIdentifier AND a.AddressType = 'Physical'

WHERE M.MembershipType = 'Employer Contact' AND G.GroupType = 'Employer'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [courses].[TActivity]
as
select
    ActivityIdentifier
    ,ActivityAsset
    ,ActivityAuthorName
    ,ActivityAuthorDate
    ,ActivityName
    ,ActivityCode
    ,ActivityHook
    ,ActivityImage
    ,ActivityType
    ,ActivitySequence
    ,ActivityUrl
    ,ActivityUrlTarget
    ,ActivityUrlType
    ,AssessmentFormIdentifier
    ,GradeItemIdentifier
    ,ModuleIdentifier
    ,SourceIdentifier
    ,SurveyFormIdentifier
    ,CreatedBy
    ,Created
    ,ModifiedBy
    ,Modified
    ,cast(null as varchar(30)) as PrerequisiteCondition
    ,RequirementCondition
    ,ActivityPlatform
    ,ActivityMinutes
    ,ActivityIsMultilingual
    ,ActivityMode
    ,PrerequisiteActivityIdentifier
    ,ActivityIsAdaptive
    ,OrganizationIdentifier
    ,ActivityIsDispatch
    ,PrerequisiteDeterminer
    ,cast(null as uniqueidentifier)InteractionIdentifier
    ,DoneButtonText
    ,DoneButtonInstructions
    ,DoneMarkedInstructions
    ,QuizIdentifier
from
    courses.QActivity
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [courses].[QActivityCompetency](
	[ActivityIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyStandardIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[CompetencyCode] [varchar](100) NULL,
	[RelationshipType] [varchar](10) NULL,
 CONSTRAINT [PK_QActivityCompetency] PRIMARY KEY CLUSTERED 
(
	[ActivityIdentifier] ASC,
	[CompetencyStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [courses].[TActivityCompetency]
as
select
    ActivityIdentifier
    ,CompetencyStandardIdentifier as CompetencyIdentifier
    ,CompetencyCode
    ,RelationshipType
    ,OrganizationIdentifier
from
    courses.QActivityCompetency
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [courses].[TModule]
as
select
    ModuleIdentifier
    ,ModuleAsset
    ,ModuleName
    ,ModuleCode
    ,ModuleImage
    ,ModuleSequence
    ,SourceIdentifier
    ,CreatedBy
    ,Created
    ,ModifiedBy
    ,Modified
    ,cast(null as uniqueidentifier) as GradeCategoryIdentifier
    ,UnitIdentifier
    ,ModuleIsAdaptive
    ,OrganizationIdentifier
    ,PrerequisiteDeterminer
from
    courses.QModule
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [courses].[TUnit]
as
select
    UnitIdentifier
    ,UnitAsset
    ,UnitName
    ,UnitCode
    ,UnitSequence
    ,CourseIdentifier
    ,SourceIdentifier
    ,CreatedBy
    ,Created
    ,ModifiedBy
    ,Modified
    ,UnitIsAdaptive
    ,OrganizationIdentifier
    ,PrerequisiteDeterminer
from
    courses.QUnit
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [courses].[VActivityCompetency]
AS
    SELECT
        X.RelationshipType
      , X.ActivityIdentifier
      , X.CompetencyIdentifier
      , X.CompetencyCode
      , S.StandardType  AS CompetencyType
      , S.StandardLabel AS CompetencyLabel
      , S.ContentTitle  AS CompetencyTitle
      , S.AssetNumber   AS CompetencyAsset
      , U.CourseIdentifier
    FROM
        courses.TActivityCompetency        AS X
    INNER JOIN courses.TActivity           AS A
               ON A.ActivityIdentifier = X.ActivityIdentifier

    INNER JOIN courses.TModule             AS M
               ON M.ModuleIdentifier = A.ModuleIdentifier

	INNER JOIN courses.TUnit             AS U
               ON U.UnitIdentifier = M.UnitIdentifier

    INNER JOIN courses.TCourse             AS C
               ON C.CourseIdentifier = U.CourseIdentifier

    INNER JOIN standards.Standard AS S
               ON X.CompetencyIdentifier = S.StandardIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[ActiveUser] AS
    SELECT
         U.Email
       , U.FirstName
       , U.LastName
       , U.MiddleName
       , U.FullName
       , U.UserIdentifier
       , U.AccessGrantedToCmds
       , (
             SELECT MAX(SessionStarted)
             FROM [security].TUserSession
             WHERE UserEmail = U.Email AND SessionIsAuthenticated = 1
         ) AS UtcAuthenticated
     FROM
         identities.QUser AS U
     WHERE
         U.UtcArchived IS NULL
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[PermissionList]
AS
    SELECT
        GroupIdentifier
      , GroupName
      , GroupCode AS GroupAbbreviation
    FROM
        contacts.QGroup
    WHERE
        GroupType = 'Role'
        AND GroupName LIKE 'CMDS %';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[ZUserStatus](
	[SnapshotKey] [int] IDENTITY(1,1) NOT NULL,
	[AsAt] [date] NOT NULL,
	[CountRQ_Certificate] [int] NULL,
	[CountVA_Certificate] [int] NULL,
	[Score_Certificate] [decimal](7, 4) NULL,
	[CountRQ_Requirement] [int] NULL,
	[CountVA_Requirement] [int] NULL,
	[Score_Requirement] [decimal](7, 4) NULL,
	[CountRQ_Code] [int] NULL,
	[CountVA_Code] [int] NULL,
	[Score_Code] [decimal](7, 4) NULL,
	[CountRQ_Practice] [int] NULL,
	[CountVA_Practice] [int] NULL,
	[Score_Practice] [decimal](7, 4) NULL,
	[CountRQ_Competency] [int] NULL,
	[CountEX_Competency] [int] NULL,
	[CountNC_Competency] [int] NULL,
	[CountNA_Competency] [int] NULL,
	[CountNT_Competency] [int] NULL,
	[CountSA_Competency] [int] NULL,
	[CountSV_Competency] [int] NULL,
	[CountVA_Competency] [int] NULL,
	[CountRQ_CompetencyCritical] [int] NULL,
	[CountEX_CompetencyCritical] [int] NULL,
	[CountNC_CompetencyCritical] [int] NULL,
	[CountNA_CompetencyCritical] [int] NULL,
	[CountNT_CompetencyCritical] [int] NULL,
	[CountSA_CompetencyCritical] [int] NULL,
	[CountSV_CompetencyCritical] [int] NULL,
	[CountVA_CompetencyCritical] [int] NULL,
	[CountRQ_CompetencyNoncritical] [int] NULL,
	[CountEX_CompetencyNoncritical] [int] NULL,
	[CountNC_CompetencyNoncritical] [int] NULL,
	[CountNA_CompetencyNoncritical] [int] NULL,
	[CountNT_CompetencyNoncritical] [int] NULL,
	[CountSA_CompetencyNoncritical] [int] NULL,
	[CountSV_CompetencyNoncritical] [int] NULL,
	[CountVA_CompetencyNoncritical] [int] NULL,
	[ScoreRQ_Competency] [decimal](7, 4) NULL,
	[ScoreEX_Competency] [decimal](7, 4) NULL,
	[ScoreNC_Competency] [decimal](7, 4) NULL,
	[ScoreNA_Competency] [decimal](7, 4) NULL,
	[ScoreNT_Competency] [decimal](7, 4) NULL,
	[ScoreSA_Competency] [decimal](7, 4) NULL,
	[ScoreSV_Competency] [decimal](7, 4) NULL,
	[ScoreVA_Competency] [decimal](7, 4) NULL,
	[ScoreRQ_CompetencyCritical] [decimal](7, 4) NULL,
	[ScoreEX_CompetencyCritical] [decimal](7, 4) NULL,
	[ScoreNC_CompetencyCritical] [decimal](7, 4) NULL,
	[ScoreNA_CompetencyCritical] [decimal](7, 4) NULL,
	[ScoreNT_CompetencyCritical] [decimal](7, 4) NULL,
	[ScoreSA_CompetencyCritical] [decimal](7, 4) NULL,
	[ScoreSV_CompetencyCritical] [decimal](7, 4) NULL,
	[ScoreVA_CompetencyCritical] [decimal](7, 4) NULL,
	[ScoreRQ_CompetencyNoncritical] [decimal](7, 4) NULL,
	[ScoreEX_CompetencyNoncritical] [decimal](7, 4) NULL,
	[ScoreNC_CompetencyNoncritical] [decimal](7, 4) NULL,
	[ScoreNA_CompetencyNoncritical] [decimal](7, 4) NULL,
	[ScoreNT_CompetencyNoncritical] [decimal](7, 4) NULL,
	[ScoreSA_CompetencyNoncritical] [decimal](7, 4) NULL,
	[ScoreSV_CompetencyNoncritical] [decimal](7, 4) NULL,
	[ScoreVA_CompetencyNoncritical] [decimal](7, 4) NULL,
	[Score_Competency] [decimal](7, 4) NULL,
	[Score_CompetencyCritical] [decimal](7, 4) NULL,
	[Score_CompetencyNoncritical] [decimal](7, 4) NULL,
	[CountRQ_CompetencyCriticalMandatory] [int] NULL,
	[CountEX_CompetencyCriticalMandatory] [int] NULL,
	[CountNC_CompetencyCriticalMandatory] [int] NULL,
	[CountNA_CompetencyCriticalMandatory] [int] NULL,
	[CountNT_CompetencyCriticalMandatory] [int] NULL,
	[CountSA_CompetencyCriticalMandatory] [int] NULL,
	[CountSV_CompetencyCriticalMandatory] [int] NULL,
	[CountVA_CompetencyCriticalMandatory] [int] NULL,
	[CountRQ_CompetencyNoncriticalMandatory] [int] NULL,
	[CountEX_CompetencyNoncriticalMandatory] [int] NULL,
	[CountNC_CompetencyNoncriticalMandatory] [int] NULL,
	[CountNA_CompetencyNoncriticalMandatory] [int] NULL,
	[CountNT_CompetencyNoncriticalMandatory] [int] NULL,
	[CountSA_CompetencyNoncriticalMandatory] [int] NULL,
	[CountSV_CompetencyNoncriticalMandatory] [int] NULL,
	[CountVA_CompetencyNoncriticalMandatory] [int] NULL,
	[ScoreEX_CompetencyCriticalMandatory] [decimal](7, 4) NULL,
	[ScoreNC_CompetencyCriticalMandatory] [decimal](7, 4) NULL,
	[ScoreNA_CompetencyCriticalMandatory] [decimal](7, 4) NULL,
	[ScoreNT_CompetencyCriticalMandatory] [decimal](7, 4) NULL,
	[ScoreSA_CompetencyCriticalMandatory] [decimal](7, 4) NULL,
	[ScoreSV_CompetencyCriticalMandatory] [decimal](7, 4) NULL,
	[ScoreVA_CompetencyCriticalMandatory] [decimal](7, 4) NULL,
	[ScoreEX_CompetencyNoncriticalMandatory] [decimal](7, 4) NULL,
	[ScoreNC_CompetencyNoncriticalMandatory] [decimal](7, 4) NULL,
	[ScoreNA_CompetencyNoncriticalMandatory] [decimal](7, 4) NULL,
	[ScoreNT_CompetencyNoncriticalMandatory] [decimal](7, 4) NULL,
	[ScoreSA_CompetencyNoncriticalMandatory] [decimal](7, 4) NULL,
	[ScoreSV_CompetencyNoncriticalMandatory] [decimal](7, 4) NULL,
	[ScoreVA_CompetencyNoncriticalMandatory] [decimal](7, 4) NULL,
	[Score_CompetencyCriticalMandatory] [decimal](7, 4) NULL,
	[Score_CompetencyNoncriticalMandatory] [decimal](7, 4) NULL,
	[CountRQ_Document] [int] NULL,
	[CountVA_Document] [int] NULL,
	[Score_Document] [decimal](7, 4) NULL,
	[CountRQ_Module] [int] NULL,
	[CountVA_Module] [int] NULL,
	[Score_Module] [decimal](7, 4) NULL,
	[CountRQ_Guide] [int] NULL,
	[CountVA_Guide] [int] NULL,
	[Score_Guide] [decimal](7, 4) NULL,
	[CountRQ_Procedure] [int] NULL,
	[CountVA_Procedure] [int] NULL,
	[Score_Procedure] [decimal](7, 4) NULL,
	[CountRQ_Orientation] [int] NULL,
	[CountVA_Orientation] [int] NULL,
	[Score_Orientation] [decimal](7, 4) NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[PrimaryProfileIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ZUserStatus] PRIMARY KEY CLUSTERED 
(
	[SnapshotKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_cmds].[ZUserStatusSummaryCalc]
AS
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier,
        UserIdentifier,
        PrimaryProfileIdentifier,
        AsAt                                AS SnapshotDate,
        0                                   AS Sequence,
        'Time-Sensitive Safety Certificate' AS Heading,
        NULL                                AS PrimaryNotApplicable,
        CountRQ_Certificate                 AS PrimaryRequired,
        CountVA_Certificate                 AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Certificate > 0
                THEN
                CAST(CountVA_Certificate AS FLOAT) / CountRQ_Certificate
            ELSE
                1
        END                                 AS PrimaryScore,
        NULL                                AS PrimarySubmitted,
        NULL                                AS ComplianceNotApplicable,
        CountRQ_Certificate                 AS ComplianceRequired,
        CountVA_Certificate                 AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Certificate > 0
                THEN
                CAST(CountVA_Certificate AS FLOAT) / CountRQ_Certificate
            ELSE
                1
        END                                 AS ComplianceScore,
        NULL                                AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier                AS DepartmentID,
        UserIdentifier                      AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                                AS SnapshotDate,
        1                                   AS Sequence,
        'Additional Compliance Requirement' AS Heading,
        NULL                                AS PrimaryNotApplicable,
        CountRQ_Requirement                 AS PrimaryRequired,
        CountVA_Requirement                 AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Requirement > 0
                THEN
                CAST(CountVA_Requirement AS FLOAT) / CountRQ_Requirement
            ELSE
                1
        END                                 AS PrimaryScore,
        NULL                                AS PrimarySubmitted,
        NULL                                AS ComplianceNotApplicable,
        CountRQ_Requirement                 AS ComplianceRequired,
        CountVA_Requirement                 AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Requirement > 0
                THEN
                CAST(CountVA_Requirement AS FLOAT) / CountRQ_Requirement
            ELSE
                1
        END                                 AS ComplianceScore,
        NULL                                AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier                AS DepartmentID,
        UserIdentifier                      AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                                AS SnapshotDate,
        2                                   AS Sequence,
        'Critical Competencies'             AS Heading,
        CountNA_CompetencyCritical          AS PrimaryNotApplicable,
        CountRQ_CompetencyCritical          AS PrimaryRequired,
        CountVA_CompetencyCritical          AS PrimarySatisfied,
        CASE
            WHEN CountRQ_CompetencyCritical > 0
                THEN
        (CAST(CountVA_CompetencyCritical AS FLOAT) + CountNA_CompetencyCritical) / CountRQ_CompetencyCritical
            ELSE
                1
        END                                 AS PrimaryScore,
        CountSV_CompetencyCritical          AS PrimarySubmitted,
        CountNA_CompetencyCriticalMandatory AS ComplianceNotApplicable,
        CountRQ_CompetencyCriticalMandatory AS ComplianceRequired,
        CountVA_CompetencyCriticalMandatory AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_CompetencyCriticalMandatory > 0
                THEN
        (CAST(CountVA_CompetencyCriticalMandatory AS FLOAT) + CountNA_CompetencyCriticalMandatory)
        / CountRQ_CompetencyCriticalMandatory
            ELSE
                1
        END                                 AS ComplianceScore,
        CountSV_CompetencyCriticalMandatory AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier                   AS DepartmentID,
        UserIdentifier                         AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                                   AS SnapshotDate,
        3                                      AS Sequence,
        'Non-Critical Competencies'            AS Heading,
        CountNA_CompetencyNoncritical          AS PrimaryNotApplicable,
        CountRQ_CompetencyNoncritical          AS PrimaryRequired,
        CountVA_CompetencyNoncritical          AS PrimarySatisfied,
        CASE
            WHEN CountRQ_CompetencyNoncritical > 0
                THEN
        (CAST(CountVA_CompetencyNoncritical AS FLOAT) + CountNA_CompetencyNoncritical) / CountRQ_CompetencyNoncritical
            ELSE
                1
        END                                    AS PrimaryScore,
        CountSV_CompetencyNoncritical          AS PrimarySubmitted,
        CountNA_CompetencyNoncriticalMandatory AS ComplianceNotApplicable,
        CountRQ_CompetencyNoncriticalMandatory AS ComplianceRequired,
        CountVA_CompetencyNoncriticalMandatory AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_CompetencyNoncriticalMandatory > 0
                THEN
        (CAST(CountVA_CompetencyNoncriticalMandatory AS FLOAT) + CountNA_CompetencyNoncriticalMandatory)
        / CountRQ_CompetencyNoncriticalMandatory
            ELSE
                1
        END                                    AS ComplianceScore,
        CountSV_CompetencyNoncriticalMandatory AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier    AS DepartmentID,
        UserIdentifier          AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                    AS SnapshotDate,
        4                       AS Sequence,
        'Code of Practice'      AS Heading,
        NULL                    AS PrimaryNotApplicable,
        CountRQ_Code            AS PrimaryRequired,
        CountVA_Code            AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Code > 0
                THEN
                CAST(CountVA_Code AS FLOAT) / CountRQ_Code
            ELSE
                1
        END                     AS PrimaryScore,
        NULL                    AS PrimarySubmitted,
        NULL                    AS ComplianceNotApplicable,
        CountRQ_Code            AS ComplianceRequired,
        CountVA_Code            AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Code > 0
                THEN
                CAST(CountVA_Code AS FLOAT) / CountRQ_Code
            ELSE
                1
        END                     AS ComplianceScore,
        NULL                    AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier      AS DepartmentID,
        UserIdentifier            AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                      AS SnapshotDate,
        5                         AS Sequence,
        'Safe Operating Practice' AS Heading,
        NULL                      AS PrimaryNotApplicable,
        CountRQ_Practice          AS PrimaryRequired,
        CountVA_Practice          AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Practice > 0
                THEN
                CAST(CountVA_Practice AS FLOAT) / CountRQ_Practice
            ELSE
                1
        END                       AS PrimaryScore,
        NULL                      AS PrimarySubmitted,
        NULL                      AS ComplianceNotApplicable,
        CountRQ_Practice          AS ComplianceRequired,
        CountVA_Practice          AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Practice > 0
                THEN
                CAST(CountVA_Practice AS FLOAT) / CountRQ_Practice
            ELSE
                1
        END                       AS ComplianceScore,
        NULL                      AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier       AS DepartmentID,
        UserIdentifier             AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                       AS SnapshotDate,
        6                          AS Sequence,
        'Human Resources Document' AS Heading,
        NULL                       AS PrimaryNotApplicable,
        CountRQ_Document           AS PrimaryRequired,
        CountVA_Document           AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Document > 0
                THEN
                CAST(CountVA_Document AS FLOAT) / CountRQ_Document
            ELSE
                1
        END                        AS PrimaryScore,
        NULL                       AS PrimarySubmitted,
        NULL                       AS ComplianceNotApplicable,
        CountRQ_Document           AS ComplianceRequired,
        CountVA_Document           AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Document > 0
                THEN
                CAST(CountVA_Document AS FLOAT) / CountRQ_Document
            ELSE
                1
        END                        AS ComplianceScore,
        NULL                       AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier    AS DepartmentID,
        UserIdentifier          AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                    AS SnapshotDate,
        7                       AS Sequence,
        'Module'                AS Heading,
        NULL                    AS PrimaryNotApplicable,
        CountRQ_Module          AS PrimaryRequired,
        CountVA_Module          AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Module > 0
                THEN
                CAST(CountVA_Module AS FLOAT) / CountRQ_Module
            ELSE
                1
        END                     AS PrimaryScore,
        NULL                    AS PrimarySubmitted,
        NULL                    AS ComplianceNotApplicable,
        CountRQ_Module          AS ComplianceRequired,
        CountVA_Module          AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Module > 0
                THEN
                CAST(CountVA_Module AS FLOAT) / CountRQ_Module
            ELSE
                1
        END                     AS ComplianceScore,
        NULL                    AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier    AS DepartmentID,
        UserIdentifier          AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                    AS SnapshotDate,
        8                       AS Sequence,
        'Training Guide'        AS Heading,
        NULL                    AS PrimaryNotApplicable,
        CountRQ_Guide           AS PrimaryRequired,
        CountVA_Guide           AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Guide > 0
                THEN
                CAST(CountVA_Guide AS FLOAT) / CountRQ_Guide
            ELSE
                1
        END                     AS PrimaryScore,
        NULL                    AS PrimarySubmitted,
        NULL                    AS ComplianceNotApplicable,
        CountRQ_Guide           AS ComplianceRequired,
        CountVA_Guide           AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Guide > 0
                THEN
                CAST(CountVA_Guide AS FLOAT) / CountRQ_Guide
            ELSE
                1
        END                     AS ComplianceScore,
        NULL                    AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier                AS DepartmentID,
        UserIdentifier                      AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                                AS SnapshotDate,
        9                                   AS Sequence,
        'Site-Specific Operating Procedure' AS Heading,
        NULL                                AS PrimaryNotApplicable,
        CountRQ_Procedure                   AS PrimaryRequired,
        CountVA_Procedure                   AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Procedure > 0
                THEN
                CAST(CountVA_Procedure AS FLOAT) / CountRQ_Procedure
            ELSE
                1
        END                                 AS PrimaryScore,
        NULL                                AS PrimarySubmitted,
        NULL                                AS ComplianceNotApplicable,
        CountRQ_Procedure                   AS ComplianceRequired,
        CountVA_Procedure                   AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Procedure > 0
                THEN
                CAST(CountVA_Procedure AS FLOAT) / CountRQ_Procedure
            ELSE
                1
        END                                 AS ComplianceScore,
        NULL                                AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier    AS DepartmentID,
        UserIdentifier          AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                    AS SnapshotDate,
        10                      AS Sequence,
        'Orientation'           AS Heading,
        NULL                    AS PrimaryNotApplicable,
        CountRQ_Orientation     AS PrimaryRequired,
        CountVA_Orientation     AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Orientation > 0
                THEN
                CAST(CountVA_Orientation AS FLOAT) / CountRQ_Orientation
            ELSE
                1
        END                     AS PrimaryScore,
        NULL                    AS PrimarySubmitted,
        NULL                    AS ComplianceNotApplicable,
        CountRQ_Orientation     AS ComplianceRequired,
        CountVA_Orientation     AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Orientation > 0
                THEN
                CAST(CountVA_Orientation AS FLOAT) / CountRQ_Orientation
            ELSE
                1
        END                     AS ComplianceScore,
        NULL                    AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL
    UNION ALL
    SELECT
        OrganizationIdentifier,
        DepartmentIdentifier    AS DepartmentID,
        UserIdentifier          AS EmployeeID,
        PrimaryProfileIdentifier,
        AsAt                    AS SnapshotDate,
        11                      AS Sequence,
        'HR Learning Module'    AS Heading,
        NULL                    AS PrimaryNotApplicable,
        CountRQ_Module          AS PrimaryRequired,
        CountVA_Module          AS PrimarySatisfied,
        CASE
            WHEN CountRQ_Module > 0
                THEN
                CAST(CountVA_Module AS FLOAT) / CountRQ_Module
            ELSE
                1
        END                     AS PrimaryScore,
        NULL                    AS PrimarySubmitted,
        NULL                    AS ComplianceNotApplicable,
        CountRQ_Module          AS ComplianceRequired,
        CountVA_Module          AS ComplianceSatisfied,
        CASE
            WHEN CountRQ_Module > 0
                THEN
                CAST(CountVA_Module AS FLOAT) / CountRQ_Module
            ELSE
                1
        END                     AS ComplianceScore,
        NULL                    AS ComplianceSubmitted
    FROM
        custom_cmds.ZUserStatus
    WHERE
        PrimaryProfileIdentifier IS NOT NULL;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [standards].[GetStandardRootAndDepth] (
    @StandardIdentifier uniqueidentifier
)
RETURNS TABLE
AS
RETURN
    WITH CTE AS (
        SELECT
            StandardIdentifier
           ,ParentStandardIdentifier
           ,CAST(1 AS int) AS Depth
        FROM
            standards.[Standard]
        WHERE
            StandardIdentifier = @StandardIdentifier

        UNION ALL

        SELECT
            p.StandardIdentifier
           ,p.ParentStandardIdentifier
           ,c.Depth + 1
        FROM
            standards.[Standard] AS p
            INNER JOIN CTE AS c ON c.ParentStandardIdentifier = p.StandardIdentifier
    )
    SELECT TOP 1
        StandardIdentifier AS RootStandardIdentifier
       ,Depth
    FROM
        CTE
    WHERE
        ParentStandardIdentifier IS NULL;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[AbProgram](
	[AbProgramID] [int] IDENTITY(1,1) NOT NULL,
	[DateTimeSaved] [datetimeoffset](7) NULL,
	[DateTimeSubmitted] [datetimeoffset](7) NULL,
	[Deadline] [datetimeoffset](7) NULL,
	[InsertedBy] [varchar](128) NULL,
	[InsertedOn] [datetimeoffset](7) NULL,
	[RespondentName] [varchar](50) NULL,
	[StateName] [varchar](30) NULL,
	[SurveyYear] [int] NOT NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedOn] [datetimeoffset](7) NULL,
	[IsVisibleOnTable01] [bit] NOT NULL,
	[IsVisibleOnTable02] [bit] NOT NULL,
	[IsVisibleOnTable03] [bit] NOT NULL,
	[IsVisibleOnTable04] [bit] NOT NULL,
	[IsVisibleOnTable05] [bit] NOT NULL,
	[IsVisibleOnTable06] [bit] NOT NULL,
	[IsVisibleOnTable07] [bit] NOT NULL,
	[IsVisibleOnTable08] [bit] NOT NULL,
	[IsVisibleOnTable09] [bit] NOT NULL,
	[IsVisibleOnTable10] [bit] NOT NULL,
	[IsVisibleOnTable11] [bit] NOT NULL,
	[IsVisibleOnTable12] [bit] NOT NULL,
	[IsVisibleOnTable13] [bit] NOT NULL,
	[IsVisibleOnTable14] [bit] NOT NULL,
	[IsVisibleOnTable15] [bit] NOT NULL,
	[AB001] [varchar](30) NULL,
	[AB002] [varchar](60) NULL,
	[AB003] [varchar](40) NULL,
	[AB004] [varchar](20) NULL,
	[AB005] [varchar](50) NULL,
	[AB006] [varchar](10) NULL,
	[AB007] [varchar](10) NULL,
	[AB008] [varchar](10) NULL,
	[AB009] [varchar](10) NULL,
	[AB010] [varchar](10) NULL,
	[AB011] [varchar](10) NULL,
	[AB012] [varchar](1520) NULL,
	[AB013] [varchar](10) NULL,
	[AB014] [varchar](10) NULL,
	[AB015] [varchar](10) NULL,
	[AB016] [varchar](10) NULL,
	[AB017] [varchar](10) NULL,
	[AB018] [varchar](10) NULL,
	[AB019] [varchar](10) NULL,
	[AB020] [varchar](740) NULL,
	[AB021] [varchar](1400) NULL,
	[AB022] [varchar](10) NULL,
	[AB023] [varchar](10) NULL,
	[AB024] [varchar](10) NULL,
	[AB025] [varchar](370) NULL,
	[AB026] [varchar](10) NULL,
	[AB027] [varchar](10) NULL,
	[AB028] [varchar](10) NULL,
	[AB029] [varchar](300) NULL,
	[AB030] [varchar](20) NULL,
	[AB031] [varchar](200) NULL,
	[AB032] [varchar](10) NULL,
	[AB033] [varchar](10) NULL,
	[AB034] [varchar](10) NULL,
	[AB035] [varchar](10) NULL,
	[AB036] [varchar](890) NULL,
	[AB037] [varchar](20) NULL,
	[AB038] [varchar](20) NULL,
	[AB039] [varchar](20) NULL,
	[AB040] [varchar](20) NULL,
	[AB041] [varchar](580) NULL,
	[AB042] [varchar](10) NULL,
	[AB043] [varchar](1330) NULL,
	[AB044] [varchar](10) NULL,
	[AB045] [varchar](1200) NULL,
	[AB046] [varchar](10) NULL,
	[AB047] [varchar](10) NULL,
	[AB048] [varchar](10) NULL,
	[AB049] [varchar](10) NULL,
	[AB050] [varchar](50) NULL,
	[AB051] [varchar](10) NULL,
	[AB052] [varchar](10) NULL,
	[AB053] [varchar](10) NULL,
	[AB054] [varchar](10) NULL,
	[AB055] [varchar](10) NULL,
	[AB056] [varchar](10) NULL,
	[AB057] [varchar](10) NULL,
	[AB058] [varchar](10) NULL,
	[AB059] [varchar](10) NULL,
	[AB060] [varchar](10) NULL,
	[AB061] [varchar](690) NULL,
	[AB062] [varchar](50) NULL,
	[AB063] [varchar](10) NULL,
	[AB064] [varchar](10) NULL,
	[AB065] [varchar](10) NULL,
	[AB066] [varchar](50) NULL,
	[AB067] [varchar](10) NULL,
	[AB068] [varchar](10) NULL,
	[AB069] [varchar](10) NULL,
	[AB070] [varchar](10) NULL,
	[AB071] [varchar](50) NULL,
	[AB072] [varchar](10) NULL,
	[AB073] [varchar](10) NULL,
	[AB074] [varchar](10) NULL,
	[AB075] [varchar](10) NULL,
	[AB076] [varchar](10) NULL,
	[AB077] [varchar](10) NULL,
	[AB078] [varchar](800) NULL,
	[AB079] [varchar](50) NULL,
	[AB080] [varchar](10) NULL,
	[AB081] [varchar](10) NULL,
	[AB082] [varchar](10) NULL,
	[AB083] [varchar](10) NULL,
	[AB084] [varchar](10) NULL,
	[AB085] [varchar](10) NULL,
	[AB086] [varchar](10) NULL,
	[AB087] [varchar](10) NULL,
	[AB088] [varchar](10) NULL,
	[AB089] [varchar](10) NULL,
	[AB090] [varchar](10) NULL,
	[AB091] [varchar](50) NULL,
	[AB092] [varchar](10) NULL,
	[AB093] [varchar](10) NULL,
	[AB094] [varchar](10) NULL,
	[AB095] [varchar](10) NULL,
	[AB096] [varchar](600) NULL,
	[AB097] [varchar](650) NULL,
	[AB098] [varchar](10) NULL,
	[AB099] [varchar](10) NULL,
	[AB100] [varchar](10) NULL,
	[AB101] [varchar](10) NULL,
	[AB102] [varchar](10) NULL,
	[AB103] [varchar](10) NULL,
	[AB104] [varchar](10) NULL,
	[AB105] [varchar](10) NULL,
	[AB106] [varchar](50) NULL,
	[AB107] [varchar](10) NULL,
	[AB108] [varchar](10) NULL,
	[AB109] [varchar](10) NULL,
	[AB110] [varchar](10) NULL,
	[AB111] [varchar](610) NULL,
	[AB112] [varchar](50) NULL,
	[AB113] [varchar](20) NULL,
	[AB114] [varchar](20) NULL,
	[AB115] [varchar](20) NULL,
	[AB116] [varchar](20) NULL,
	[AB117] [varchar](20) NULL,
	[AB118] [varchar](10) NULL,
	[AB119] [varchar](10) NULL,
	[AB120] [varchar](1100) NULL,
	[AB121] [varchar](10) NULL,
	[AB122] [varchar](10) NULL,
	[AB123] [varchar](850) NULL,
	[AB124] [varchar](10) NULL,
	[AB125] [varchar](2010) NULL,
	[AB126] [varchar](10) NULL,
	[AB127] [varchar](10) NULL,
	[AB128] [varchar](10) NULL,
	[AB129] [varchar](50) NULL,
	[AB130] [varchar](50) NULL,
	[AB131] [varchar](10) NULL,
	[AB132] [varchar](10) NULL,
	[AB133] [varchar](50) NULL,
	[AB134] [varchar](10) NULL,
	[AB135] [varchar](10) NULL,
	[AB136] [varchar](10) NULL,
	[AB137] [varchar](50) NULL,
	[AB138] [varchar](10) NULL,
	[AB139] [varchar](10) NULL,
	[AB140] [varchar](10) NULL,
	[AB141] [varchar](10) NULL,
	[AB142] [varchar](900) NULL,
	[AB143] [varchar](10) NULL,
	[AB144] [varchar](50) NULL,
	[AB145] [varchar](20) NULL,
	[AB146] [varchar](20) NULL,
	[AB147] [varchar](50) NULL,
	[AB148] [varchar](20) NULL,
	[AB149] [varchar](20) NULL,
	[AB150] [varchar](10) NULL,
	[AB151] [varchar](750) NULL,
	[AB152] [varchar](420) NULL,
	[AB153] [varchar](50) NULL,
	[AB154] [varchar](10) NULL,
	[AB155] [varchar](20) NULL,
	[AB156] [varchar](50) NULL,
	[AB157] [varchar](10) NULL,
	[AB158] [varchar](20) NULL,
	[AB159] [varchar](380) NULL,
	[AB160] [varchar](4000) NULL,
	[AB161] [varchar](90) NULL,
	[AB162] [varchar](10) NULL,
	[AB163] [varchar](10) NULL,
	[AB164] [varchar](10) NULL,
	[AB165] [varchar](10) NULL,
	[AB166] [varchar](80) NULL,
	[AB167] [varchar](10) NULL,
	[AB168] [varchar](20) NULL,
	[AB169] [varchar](40) NULL,
	[AB170] [varchar](500) NULL,
	[AB171] [varchar](1040) NULL,
	[ProgramFolderID] [int] NULL,
	[AB172] [varchar](20) NULL,
	[AB173] [varchar](10) NULL,
	[AB174] [varchar](20) NULL,
	[AB175] [varchar](10) NULL,
	[AB176] [varchar](10) NULL,
	[RespondentUserIdentifier] [uniqueidentifier] NULL,
	[AgencyGroupIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_AbProgram] PRIMARY KEY CLUSTERED 
(
	[AbProgramID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[HcProgram](
	[HcProgramID] [int] IDENTITY(1,1) NOT NULL,
	[DateSubmitted] [datetimeoffset](7) NULL,
	[DateTimeSaved] [datetimeoffset](7) NULL,
	[DateTimeSubmitted] [datetimeoffset](7) NULL,
	[Deadline] [datetimeoffset](7) NULL,
	[FirstName] [varchar](20) NULL,
	[InsertedBy] [varchar](128) NULL,
	[InsertedOn] [datetimeoffset](7) NULL,
	[LastName] [varchar](50) NULL,
	[SourceID] [int] NULL,
	[StateName] [varchar](40) NULL,
	[SurveyYear] [int] NOT NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedOn] [datetimeoffset](7) NULL,
	[IsVisibleOnTable01] [bit] NOT NULL,
	[IsVisibleOnTable02] [bit] NOT NULL,
	[IsVisibleOnTable03] [bit] NOT NULL,
	[IsVisibleOnTable04] [bit] NOT NULL,
	[IsVisibleOnTable05] [bit] NOT NULL,
	[IsVisibleOnTable06] [bit] NOT NULL,
	[IsVisibleOnTable07] [bit] NOT NULL,
	[IsVisibleOnTable08] [bit] NOT NULL,
	[IsVisibleOnTable09] [bit] NOT NULL,
	[IsVisibleOnTable11] [bit] NOT NULL,
	[IsVisibleOnTable12] [bit] NOT NULL,
	[IsVisibleOnTable13] [bit] NOT NULL,
	[IsVisibleOnTable14] [bit] NOT NULL,
	[IsVisibleOnTable15] [bit] NOT NULL,
	[IsVisibleOnTable16] [bit] NOT NULL,
	[IsVisibleOnTable17] [bit] NOT NULL,
	[IsVisibleOnTable18] [bit] NOT NULL,
	[IsVisibleOnTable19] [bit] NOT NULL,
	[HC001] [varchar](40) NULL,
	[HC002] [varchar](60) NULL,
	[HC003] [varchar](40) NULL,
	[HC004] [varchar](20) NULL,
	[HC005] [varchar](50) NULL,
	[HC006] [varchar](10) NULL,
	[HC007] [varchar](20) NULL,
	[HC008] [varchar](20) NULL,
	[HC009] [varchar](20) NULL,
	[HC010] [varchar](20) NULL,
	[HC011] [varchar](20) NULL,
	[HC012] [varchar](20) NULL,
	[HC013] [varchar](10) NULL,
	[HC014] [varchar](20) NULL,
	[HC015] [varchar](410) NULL,
	[HC016] [varchar](20) NULL,
	[HC017] [varchar](10) NULL,
	[HC018] [varchar](20) NULL,
	[HC019] [varchar](10) NULL,
	[HC020] [varchar](10) NULL,
	[HC021] [varchar](10) NULL,
	[HC022] [varchar](20) NULL,
	[HC023] [varchar](10) NULL,
	[HC024] [varchar](10) NULL,
	[HC025] [varchar](10) NULL,
	[HC026] [varchar](20) NULL,
	[HC027] [varchar](10) NULL,
	[HC028] [varchar](10) NULL,
	[HC029] [varchar](10) NULL,
	[HC030] [varchar](20) NULL,
	[HC031] [varchar](10) NULL,
	[HC032] [varchar](10) NULL,
	[HC033] [varchar](10) NULL,
	[HC034] [varchar](20) NULL,
	[HC035] [varchar](1090) NULL,
	[HC036] [varchar](10) NULL,
	[HC037] [varchar](20) NULL,
	[HC038] [varchar](10) NULL,
	[HC039] [varchar](10) NULL,
	[HC040] [varchar](10) NULL,
	[HC041] [varchar](20) NULL,
	[HC042] [varchar](10) NULL,
	[HC043] [varchar](10) NULL,
	[HC044] [varchar](10) NULL,
	[HC045] [varchar](20) NULL,
	[HC046] [varchar](10) NULL,
	[HC047] [varchar](10) NULL,
	[HC048] [varchar](10) NULL,
	[HC049] [varchar](20) NULL,
	[HC050] [varchar](10) NULL,
	[HC051] [varchar](10) NULL,
	[HC052] [varchar](10) NULL,
	[HC053] [varchar](10) NULL,
	[HC054] [varchar](800) NULL,
	[HC055] [varchar](10) NULL,
	[HC056] [varchar](10) NULL,
	[HC057] [varchar](10) NULL,
	[HC058] [varchar](10) NULL,
	[HC059] [varchar](10) NULL,
	[HC060] [varchar](440) NULL,
	[HC061] [varchar](10) NULL,
	[HC062] [varchar](10) NULL,
	[HC063] [varchar](10) NULL,
	[HC064] [varchar](10) NULL,
	[HC065] [varchar](10) NULL,
	[HC066] [varchar](10) NULL,
	[HC067] [varchar](10) NULL,
	[HC068] [varchar](10) NULL,
	[HC069] [varchar](10) NULL,
	[HC070] [varchar](10) NULL,
	[HC071] [varchar](10) NULL,
	[HC072] [varchar](10) NULL,
	[HC073] [varchar](10) NULL,
	[HC074] [varchar](10) NULL,
	[HC075] [varchar](20) NULL,
	[HC076] [varchar](530) NULL,
	[HC077] [varchar](10) NULL,
	[HC078] [varchar](10) NULL,
	[HC079] [varchar](10) NULL,
	[HC082] [varchar](10) NULL,
	[HC083] [varchar](10) NULL,
	[HC084] [varchar](10) NULL,
	[HC085] [varchar](820) NULL,
	[HC086] [varchar](10) NULL,
	[HC087] [varchar](10) NULL,
	[HC088] [varchar](10) NULL,
	[HC089] [varchar](10) NULL,
	[HC090] [varchar](10) NULL,
	[HC091] [varchar](10) NULL,
	[HC094] [varchar](10) NULL,
	[HC095] [varchar](10) NULL,
	[HC096] [varchar](10) NULL,
	[HC097] [varchar](10) NULL,
	[HC098] [varchar](10) NULL,
	[HC099] [varchar](1070) NULL,
	[HC100] [varchar](10) NULL,
	[HC101] [varchar](20) NULL,
	[HC102] [varchar](10) NULL,
	[HC103] [varchar](580) NULL,
	[HC104] [varchar](300) NULL,
	[HC105] [varchar](10) NULL,
	[HC106] [varchar](200) NULL,
	[HC107] [varchar](10) NULL,
	[HC108] [varchar](150) NULL,
	[HC109] [varchar](10) NULL,
	[HC110] [varchar](10) NULL,
	[HC111] [varchar](1770) NULL,
	[HC112] [varchar](10) NULL,
	[HC113] [varchar](10) NULL,
	[HC114] [varchar](10) NULL,
	[HC115] [varchar](10) NULL,
	[HC116] [varchar](10) NULL,
	[HC117] [varchar](620) NULL,
	[HC118] [varchar](20) NULL,
	[HC119] [varchar](20) NULL,
	[HC120] [varchar](10) NULL,
	[HC121] [varchar](750) NULL,
	[HC122] [varchar](330) NULL,
	[HC123] [varchar](120) NULL,
	[HC124] [varchar](260) NULL,
	[HC125] [varchar](300) NULL,
	[HC126] [varchar](500) NULL,
	[HC127] [varchar](660) NULL,
	[HC128] [varchar](800) NULL,
	[HC129] [varchar](1800) NULL,
	[HC130] [varchar](4420) NULL,
	[HC131] [varchar](10) NULL,
	[HC132] [varchar](10) NULL,
	[HC133] [varchar](800) NULL,
	[HC141] [varchar](10) NULL,
	[HC142] [varchar](10) NULL,
	[HC143] [varchar](20) NULL,
	[HC144] [varchar](10) NULL,
	[HC145] [varchar](10) NULL,
	[HC146] [varchar](10) NULL,
	[HC147] [varchar](10) NULL,
	[HC148] [varchar](10) NULL,
	[HC149] [varchar](410) NULL,
	[HC150] [varchar](10) NULL,
	[HC151] [varchar](10) NULL,
	[HC152] [varchar](20) NULL,
	[HC153] [varchar](10) NULL,
	[HC154] [varchar](10) NULL,
	[HC155] [varchar](10) NULL,
	[HC156] [varchar](10) NULL,
	[HC157] [varchar](10) NULL,
	[HC158] [varchar](260) NULL,
	[HC159] [varchar](4800) NULL,
	[HC160] [varchar](2040) NULL,
	[HC161] [varchar](1990) NULL,
	[HC162] [varchar](2200) NULL,
	[HC163] [varchar](2790) NULL,
	[HC164] [varchar](10) NULL,
	[HC165] [varchar](1870) NULL,
	[HC166] [varchar](2070) NULL,
	[HC167] [varchar](10) NULL,
	[HC168] [varchar](10) NULL,
	[HC169] [varchar](10) NULL,
	[HC170] [varchar](10) NULL,
	[HC171] [varchar](10) NULL,
	[HC172] [varchar](10) NULL,
	[HC173] [varchar](10) NULL,
	[HC174] [varchar](10) NULL,
	[HC175] [varchar](10) NULL,
	[HC176] [varchar](10) NULL,
	[HC177] [varchar](10) NULL,
	[HC178] [varchar](10) NULL,
	[HC179] [varchar](10) NULL,
	[HC180] [varchar](10) NULL,
	[HC181] [varchar](10) NULL,
	[HC182] [varchar](10) NULL,
	[HC183] [varchar](10) NULL,
	[HC184] [varchar](1460) NULL,
	[HC185] [varchar](120) NULL,
	[HC186] [varchar](1400) NULL,
	[HC187] [varchar](20) NULL,
	[HC188] [varchar](18) NULL,
	[HC189] [varchar](290) NULL,
	[HC190] [varchar](360) NULL,
	[HC191] [varchar](440) NULL,
	[HC193] [varchar](70) NULL,
	[HC194] [varchar](10) NULL,
	[HC195] [varchar](410) NULL,
	[HC196] [varchar](18) NULL,
	[HC207] [varchar](20) NULL,
	[HC208] [varchar](20) NULL,
	[HC209] [varchar](10) NULL,
	[HC210] [varchar](2190) NULL,
	[HC211] [varchar](540) NULL,
	[HC212] [varchar](20) NULL,
	[HC213] [varchar](10) NULL,
	[HC214] [varchar](10) NULL,
	[HC215] [varchar](10) NULL,
	[HC216] [varchar](10) NULL,
	[HC217] [varchar](10) NULL,
	[HC218] [varchar](10) NULL,
	[HC219] [varchar](10) NULL,
	[HC220] [varchar](10) NULL,
	[HC221] [varchar](10) NULL,
	[HC222] [varchar](10) NULL,
	[HC223] [varchar](10) NULL,
	[HC224] [varchar](10) NULL,
	[HC225] [varchar](20) NULL,
	[HC226] [varchar](10) NULL,
	[HC227] [varchar](20) NULL,
	[HC228] [varchar](20) NULL,
	[HC229] [varchar](20) NULL,
	[HC230] [varchar](20) NULL,
	[HC231] [varchar](10) NULL,
	[HC232] [varchar](20) NULL,
	[HC233] [varchar](20) NULL,
	[HC234] [varchar](10) NULL,
	[HC235] [varchar](1200) NULL,
	[HC236] [varchar](10) NULL,
	[HC237] [varchar](10) NULL,
	[HC238] [varchar](10) NULL,
	[HC239] [varchar](10) NULL,
	[HC240] [varchar](10) NULL,
	[HC241] [varchar](10) NULL,
	[HC242] [varchar](10) NULL,
	[HC243] [varchar](10) NULL,
	[HC244] [varchar](10) NULL,
	[HC245] [varchar](10) NULL,
	[HC246] [varchar](10) NULL,
	[HC247] [varchar](10) NULL,
	[HC248] [varchar](20) NULL,
	[HC249] [varchar](10) NULL,
	[HC250] [varchar](10) NULL,
	[HC251] [varchar](10) NULL,
	[HC252] [varchar](10) NULL,
	[HC253] [varchar](10) NULL,
	[HC254] [varchar](10) NULL,
	[HC255] [varchar](10) NULL,
	[HC256] [varchar](10) NULL,
	[HC257] [varchar](10) NULL,
	[HC258] [varchar](10) NULL,
	[HC259] [varchar](10) NULL,
	[HC260] [varchar](20) NULL,
	[HC261] [varchar](10) NULL,
	[HC262] [varchar](10) NULL,
	[HC263] [varchar](10) NULL,
	[HC264] [varchar](20) NULL,
	[HC265] [varchar](10) NULL,
	[HC266] [varchar](10) NULL,
	[HC267] [varchar](10) NULL,
	[HC268] [varchar](350) NULL,
	[HC269] [varchar](10) NULL,
	[HC270] [varchar](3200) NULL,
	[HC271] [varchar](10) NULL,
	[HC272] [varchar](1500) NULL,
	[HC273] [varchar](10) NULL,
	[HC274] [varchar](10) NULL,
	[HC275] [varchar](18) NULL,
	[HC276] [varchar](10) NULL,
	[HC277] [varchar](1200) NULL,
	[HC278] [varchar](10) NULL,
	[HC279] [varchar](2320) NULL,
	[HC280] [varchar](10) NULL,
	[HC281] [varchar](2320) NULL,
	[HC282] [varchar](10) NULL,
	[HC283] [varchar](900) NULL,
	[HC284] [varchar](10) NULL,
	[HC285] [varchar](1000) NULL,
	[HC286] [varchar](10) NULL,
	[HC287] [varchar](960) NULL,
	[HC288] [varchar](10) NULL,
	[HC289] [varchar](1100) NULL,
	[HC290] [varchar](10) NULL,
	[HC291] [varchar](1120) NULL,
	[HC292] [varchar](10) NULL,
	[HC293] [varchar](700) NULL,
	[HC294] [varchar](10) NULL,
	[HC295] [varchar](3810) NULL,
	[ProgramFolderID] [int] NULL,
	[HC296] [varchar](10) NULL,
	[HC297] [varchar](10) NULL,
	[HC298] [varchar](20) NULL,
	[HC299] [varchar](10) NULL,
	[HC300] [varchar](170) NULL,
	[HC301] [varchar](640) NULL,
	[HC302] [varchar](150) NULL,
	[HC303] [varchar](670) NULL,
	[HC304] [varchar](80) NULL,
	[HC305] [varchar](60) NULL,
	[HC306] [varchar](120) NULL,
	[HC307] [varchar](10) NULL,
	[HC308] [varchar](1300) NULL,
	[HC309] [varchar](10) NULL,
	[HC310] [varchar](10) NULL,
	[IsVisibleOnTable20] [bit] NOT NULL,
	[HC312] [varchar](290) NULL,
	[HC313] [varchar](10) NULL,
	[HC314] [varchar](10) NULL,
	[HC315] [varchar](20) NULL,
	[HC316] [varchar](10) NULL,
	[HC317] [varchar](10) NULL,
	[HC318] [varchar](10) NULL,
	[HC319] [varchar](10) NULL,
	[HC320] [varchar](10) NULL,
	[HC321] [varchar](10) NULL,
	[HC322] [varchar](10) NULL,
	[HC323] [varchar](10) NULL,
	[HC324] [varchar](450) NULL,
	[IsVisibleOnTable10] [bit] NOT NULL,
	[HC325] [varchar](10) NULL,
	[HC326] [varchar](4800) NULL,
	[HC327] [varchar](30) NULL,
	[HC328] [varchar](290) NULL,
	[HC329] [varchar](10) NULL,
	[HC330] [varchar](10) NULL,
	[HC331] [varchar](10) NULL,
	[HC332] [varchar](10) NULL,
	[HC333] [varchar](10) NULL,
	[HC334] [varchar](10) NULL,
	[HC335] [varchar](10) NULL,
	[HC336] [varchar](10) NULL,
	[HC337] [varchar](10) NULL,
	[HC338] [varchar](10) NULL,
	[HC339] [varchar](10) NULL,
	[HC340] [varchar](10) NULL,
	[HC341] [varchar](10) NULL,
	[HC342] [varchar](10) NULL,
	[HC343] [varchar](10) NULL,
	[HC344] [varchar](10) NULL,
	[HC345] [varchar](10) NULL,
	[OwnerUserIdentifier] [uniqueidentifier] NULL,
	[AgencyGroupIdentifier] [uniqueidentifier] NULL,
	[HC346] [varchar](10) NULL,
	[HC347] [varchar](10) NULL,
	[HC348] [varchar](10) NULL,
	[HC349] [varchar](10) NULL,
	[HC350] [varchar](10) NULL,
	[HC351] [varchar](10) NULL,
	[HC352] [varchar](10) NULL,
	[HC353] [varchar](10) NULL,
	[HC354] [varchar](10) NULL,
	[HC355] [varchar](10) NULL,
	[HC356] [varchar](10) NULL,
	[HC357] [varchar](230) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[HC134] [varchar](390) NULL,
 CONSTRAINT [PK_HcProgram] PRIMARY KEY CLUSTERED 
(
	[HcProgramID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[HiProgram](
	[HiProgramID] [int] IDENTITY(1,1) NOT NULL,
	[DateTimeSaved] [datetimeoffset](7) NULL,
	[DateTimeSubmitted] [datetimeoffset](7) NULL,
	[Deadline] [datetimeoffset](7) NULL,
	[InsertedBy] [varchar](128) NULL,
	[InsertedOn] [datetimeoffset](7) NULL,
	[RespondentName] [varchar](30) NULL,
	[StateName] [varchar](20) NULL,
	[SurveyYear] [int] NOT NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedOn] [datetimeoffset](7) NULL,
	[IsVisibleOnTable01] [bit] NOT NULL,
	[IsVisibleOnTable02] [bit] NOT NULL,
	[IsVisibleOnTable03] [bit] NOT NULL,
	[IsVisibleOnTable04] [bit] NOT NULL,
	[IsVisibleOnTable05] [bit] NOT NULL,
	[IsVisibleOnTable06] [bit] NOT NULL,
	[IsVisibleOnTable07] [bit] NOT NULL,
	[IsVisibleOnTable08] [bit] NOT NULL,
	[IsVisibleOnTable09] [bit] NOT NULL,
	[IsVisibleOnTable10] [bit] NOT NULL,
	[IsVisibleOnTable11] [bit] NOT NULL,
	[IsVisibleOnTable12] [bit] NOT NULL,
	[HI001] [varchar](30) NULL,
	[HI002] [varchar](50) NULL,
	[HI003] [varchar](30) NULL,
	[HI004] [varchar](20) NULL,
	[HI005] [varchar](50) NULL,
	[HI006] [varchar](10) NULL,
	[HI007] [varchar](10) NULL,
	[HI008] [varchar](10) NULL,
	[HI009] [varchar](10) NULL,
	[HI010] [varchar](400) NULL,
	[HI011] [varchar](600) NULL,
	[HI012] [varchar](20) NULL,
	[HI013] [varchar](20) NULL,
	[HI014] [varchar](10) NULL,
	[HI015] [varchar](10) NULL,
	[HI016] [varchar](10) NULL,
	[HI017] [varchar](10) NULL,
	[HI018] [varchar](500) NULL,
	[HI019] [varchar](1) NULL,
	[HI020] [varchar](20) NULL,
	[HI021] [varchar](10) NULL,
	[HI022] [varchar](1) NULL,
	[HI023] [varchar](1) NULL,
	[HI024] [varchar](20) NULL,
	[HI025] [varchar](10) NULL,
	[HI026] [varchar](1) NULL,
	[HI027] [varchar](10) NULL,
	[HI028] [varchar](10) NULL,
	[HI029] [varchar](1) NULL,
	[HI030] [varchar](10) NULL,
	[HI031] [varchar](10) NULL,
	[HI032] [varchar](1) NULL,
	[HI033] [varchar](10) NULL,
	[HI034] [varchar](10) NULL,
	[HI035] [varchar](1) NULL,
	[HI036] [varchar](10) NULL,
	[HI037] [varchar](10) NULL,
	[HI038] [varchar](1) NULL,
	[HI039] [varchar](20) NULL,
	[HI040] [varchar](10) NULL,
	[HI041] [varchar](500) NULL,
	[HI042] [varchar](1) NULL,
	[HI043] [varchar](20) NULL,
	[HI044] [varchar](10) NULL,
	[HI045] [varchar](1) NULL,
	[HI046] [varchar](1) NULL,
	[HI047] [varchar](20) NULL,
	[HI048] [varchar](10) NULL,
	[HI049] [varchar](1) NULL,
	[HI050] [varchar](20) NULL,
	[HI051] [varchar](20) NULL,
	[HI052] [varchar](1) NULL,
	[HI053] [varchar](20) NULL,
	[HI054] [varchar](20) NULL,
	[HI055] [varchar](1) NULL,
	[HI056] [varchar](20) NULL,
	[HI057] [varchar](20) NULL,
	[HI058] [varchar](500) NULL,
	[HI059] [varchar](20) NULL,
	[HI060] [varchar](20) NULL,
	[HI061] [varchar](500) NULL,
	[HI062] [varchar](20) NULL,
	[HI063] [varchar](700) NULL,
	[HI064] [varchar](1) NULL,
	[HI065] [varchar](10) NULL,
	[HI066] [varchar](10) NULL,
	[HI067] [varchar](1) NULL,
	[HI068] [varchar](10) NULL,
	[HI069] [varchar](10) NULL,
	[HI070] [varchar](1) NULL,
	[HI071] [varchar](10) NULL,
	[HI072] [varchar](10) NULL,
	[HI073] [varchar](1) NULL,
	[HI074] [varchar](10) NULL,
	[HI075] [varchar](10) NULL,
	[HI076] [varchar](1) NULL,
	[HI077] [varchar](10) NULL,
	[HI078] [varchar](10) NULL,
	[HI079] [varchar](1) NULL,
	[HI080] [varchar](20) NULL,
	[HI081] [varchar](10) NULL,
	[HI082] [varchar](1) NULL,
	[HI083] [varchar](10) NULL,
	[HI084] [varchar](10) NULL,
	[HI085] [varchar](1) NULL,
	[HI086] [varchar](10) NULL,
	[HI087] [varchar](10) NULL,
	[HI088] [varchar](1) NULL,
	[HI089] [varchar](10) NULL,
	[HI090] [varchar](10) NULL,
	[HI091] [varchar](1) NULL,
	[HI092] [varchar](20) NULL,
	[HI093] [varchar](20) NULL,
	[HI094] [varchar](1) NULL,
	[HI095] [varchar](20) NULL,
	[HI096] [varchar](10) NULL,
	[HI097] [varchar](1) NULL,
	[HI098] [varchar](20) NULL,
	[HI099] [varchar](10) NULL,
	[HI100] [varchar](100) NULL,
	[HI101] [varchar](10) NULL,
	[HI102] [varchar](10) NULL,
	[HI103] [varchar](900) NULL,
	[HI104] [varchar](10) NULL,
	[HI105] [varchar](10) NULL,
	[HI106] [varchar](10) NULL,
	[HI107] [varchar](10) NULL,
	[HI108] [varchar](400) NULL,
	[HI109] [varchar](10) NULL,
	[HI110] [varchar](10) NULL,
	[HI111] [varchar](10) NULL,
	[HI112] [varchar](20) NULL,
	[HI113] [varchar](500) NULL,
	[HI114] [varchar](10) NULL,
	[HI115] [varchar](10) NULL,
	[HI116] [varchar](10) NULL,
	[HI117] [varchar](10) NULL,
	[HI118] [varchar](10) NULL,
	[HI119] [varchar](10) NULL,
	[HI120] [varchar](10) NULL,
	[HI121] [varchar](10) NULL,
	[HI122] [varchar](10) NULL,
	[HI123] [varchar](10) NULL,
	[HI124] [varchar](10) NULL,
	[HI125] [varchar](10) NULL,
	[HI126] [varchar](10) NULL,
	[HI127] [varchar](10) NULL,
	[HI128] [varchar](10) NULL,
	[HI129] [varchar](10) NULL,
	[HI130] [varchar](700) NULL,
	[HI131] [varchar](1500) NULL,
	[HI132] [varchar](20) NULL,
	[HI133] [varchar](250) NULL,
	[HI134] [varchar](20) NULL,
	[HI135] [varchar](200) NULL,
	[HI136] [varchar](20) NULL,
	[HI137] [varchar](1100) NULL,
	[HI138] [varchar](10) NULL,
	[HI139] [varchar](10) NULL,
	[HI140] [varchar](800) NULL,
	[HI141] [varchar](30) NULL,
	[HI142] [varchar](80) NULL,
	[HI143] [varchar](100) NULL,
	[HI144] [varchar](20) NULL,
	[HI145] [varchar](20) NULL,
	[HI146] [varchar](20) NULL,
	[HI147] [varchar](20) NULL,
	[HI148] [varchar](20) NULL,
	[HI149] [varchar](20) NULL,
	[HI150] [varchar](20) NULL,
	[HI151] [varchar](20) NULL,
	[HI152] [varchar](20) NULL,
	[HI153] [varchar](20) NULL,
	[HI154] [varchar](20) NULL,
	[HI155] [varchar](20) NULL,
	[HI156] [varchar](20) NULL,
	[HI157] [varchar](20) NULL,
	[HI158] [varchar](20) NULL,
	[HI159] [varchar](10) NULL,
	[HI160] [varchar](10) NULL,
	[HI161] [varchar](10) NULL,
	[HI162] [varchar](10) NULL,
	[HI163] [varchar](10) NULL,
	[HI164] [varchar](10) NULL,
	[HI165] [varchar](10) NULL,
	[HI166] [varchar](10) NULL,
	[HI167] [varchar](10) NULL,
	[HI168] [varchar](10) NULL,
	[HI169] [varchar](10) NULL,
	[HI170] [varchar](10) NULL,
	[HI171] [varchar](10) NULL,
	[HI172] [varchar](10) NULL,
	[HI173] [varchar](10) NULL,
	[HI174] [varchar](10) NULL,
	[HI175] [varchar](10) NULL,
	[HI176] [varchar](10) NULL,
	[HI177] [varchar](10) NULL,
	[HI178] [varchar](10) NULL,
	[HI179] [varchar](10) NULL,
	[HI180] [varchar](20) NULL,
	[HI181] [varchar](10) NULL,
	[HI182] [varchar](10) NULL,
	[HI183] [varchar](10) NULL,
	[HI184] [varchar](10) NULL,
	[HI185] [varchar](10) NULL,
	[HI186] [varchar](10) NULL,
	[HI187] [varchar](10) NULL,
	[HI188] [varchar](10) NULL,
	[HI189] [varchar](20) NULL,
	[HI190] [varchar](20) NULL,
	[HI191] [varchar](10) NULL,
	[HI192] [varchar](20) NULL,
	[HI193] [varchar](20) NULL,
	[HI194] [varchar](10) NULL,
	[HI195] [varchar](20) NULL,
	[HI196] [varchar](23) NULL,
	[HI197] [varchar](10) NULL,
	[HI198] [varchar](10) NULL,
	[HI199] [varchar](10) NULL,
	[HI200] [varchar](23) NULL,
	[ProgramFolderID] [int] NULL,
	[HI201] [varchar](1) NULL,
	[HI202] [varchar](10) NULL,
	[HI203] [varchar](10) NULL,
	[HI204] [varchar](10) NULL,
	[HI205] [varchar](10) NULL,
	[HI206] [varchar](10) NULL,
	[HI207] [varchar](10) NULL,
	[HI208] [varchar](10) NULL,
	[HI209] [varchar](20) NULL,
	[HI210] [varchar](20) NULL,
	[HI211] [varchar](20) NULL,
	[HI212] [varchar](23) NULL,
	[HI213] [varchar](10) NULL,
	[HI214] [varchar](23) NULL,
	[HI215] [varchar](10) NULL,
	[HI216] [varchar](10) NULL,
	[HI217] [varchar](10) NULL,
	[HI218] [varchar](10) NULL,
	[RespondentUserIdentifier] [uniqueidentifier] NULL,
	[AgencyGroupIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_HiProgram] PRIMARY KEY CLUSTERED 
(
	[HiProgramID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[MfProgram](
	[MfProgramID] [int] IDENTITY(1,1) NOT NULL,
	[DateTimeSaved] [datetimeoffset](7) NULL,
	[DateTimeSubmitted] [datetimeoffset](7) NULL,
	[Deadline] [datetimeoffset](7) NULL,
	[InsertedBy] [varchar](20) NULL,
	[InsertedOn] [datetimeoffset](7) NULL,
	[RespondentName] [varchar](40) NULL,
	[StateName] [varchar](30) NULL,
	[SurveyYear] [int] NOT NULL,
	[UpdatedOn] [datetimeoffset](7) NULL,
	[UpdatedBy] [varchar](20) NULL,
	[IsVisibleOnTable01] [bit] NOT NULL,
	[IsVisibleOnTable02] [bit] NOT NULL,
	[IsVisibleOnTable03] [bit] NOT NULL,
	[IsVisibleOnTable04] [bit] NOT NULL,
	[IsVisibleOnTable05] [bit] NOT NULL,
	[IsVisibleOnTable06] [bit] NOT NULL,
	[IsVisibleOnTable07] [bit] NOT NULL,
	[IsVisibleOnTable08] [bit] NOT NULL,
	[IsVisibleOnTable09] [bit] NOT NULL,
	[IsVisibleOnTable10] [bit] NOT NULL,
	[IsVisibleOnTable11] [bit] NOT NULL,
	[IsVisibleOnTable12] [bit] NOT NULL,
	[IsVisibleOnTable13] [bit] NOT NULL,
	[MF001] [varchar](30) NULL,
	[MF002] [varchar](50) NULL,
	[MF003] [varchar](30) NULL,
	[MF004] [varchar](20) NULL,
	[MF005] [varchar](50) NULL,
	[MF006] [varchar](10) NULL,
	[MF007] [varchar](330) NULL,
	[MF008] [varchar](10) NULL,
	[MF009] [varchar](20) NULL,
	[MF010] [varchar](10) NULL,
	[MF011] [varchar](10) NULL,
	[MF012] [varchar](20) NULL,
	[MF013] [varchar](10) NULL,
	[MF014] [varchar](10) NULL,
	[MF015] [varchar](20) NULL,
	[MF016] [varchar](10) NULL,
	[MF017] [varchar](10) NULL,
	[MF018] [varchar](20) NULL,
	[MF019] [varchar](10) NULL,
	[MF020] [varchar](10) NULL,
	[MF021] [varchar](20) NULL,
	[MF022] [varchar](10) NULL,
	[MF023] [varchar](10) NULL,
	[MF024] [varchar](20) NULL,
	[MF025] [varchar](10) NULL,
	[MF026] [varchar](1) NULL,
	[MF027] [varchar](10) NULL,
	[MF028] [varchar](20) NULL,
	[MF029] [varchar](710) NULL,
	[MF030] [varchar](1) NULL,
	[MF031] [varchar](10) NULL,
	[MF032] [varchar](10) NULL,
	[MF033] [varchar](1) NULL,
	[MF034] [varchar](10) NULL,
	[MF035] [varchar](10) NULL,
	[MF036] [varchar](1) NULL,
	[MF037] [varchar](10) NULL,
	[MF038] [varchar](10) NULL,
	[MF039] [varchar](680) NULL,
	[MF040] [varchar](10) NULL,
	[MF041] [varchar](10) NULL,
	[MF042] [varchar](10) NULL,
	[MF043] [varchar](10) NULL,
	[MF044] [varchar](10) NULL,
	[MF045] [varchar](10) NULL,
	[MF046] [varchar](10) NULL,
	[MF047] [varchar](10) NULL,
	[MF048] [varchar](10) NULL,
	[MF049] [varchar](10) NULL,
	[MF050] [varchar](10) NULL,
	[MF051] [varchar](10) NULL,
	[MF052] [varchar](610) NULL,
	[MF053] [varchar](10) NULL,
	[MF054] [varchar](10) NULL,
	[MF055] [varchar](10) NULL,
	[MF057] [varchar](10) NULL,
	[MF058] [varchar](10) NULL,
	[MF059] [varchar](10) NULL,
	[MF060] [varchar](10) NULL,
	[MF061] [varchar](10) NULL,
	[MF062] [varchar](10) NULL,
	[MF063] [varchar](10) NULL,
	[MF064] [varchar](10) NULL,
	[MF065] [varchar](10) NULL,
	[MF066] [varchar](10) NULL,
	[MF067] [varchar](10) NULL,
	[MF068] [varchar](10) NULL,
	[MF069] [varchar](440) NULL,
	[MF070] [varchar](20) NULL,
	[MF071] [varchar](20) NULL,
	[MF072] [varchar](20) NULL,
	[MF073] [varchar](20) NULL,
	[MF074] [varchar](20) NULL,
	[MF075] [varchar](480) NULL,
	[MF076] [varchar](10) NULL,
	[MF077] [varchar](10) NULL,
	[MF078] [varchar](20) NULL,
	[MF079] [varchar](10) NULL,
	[MF080] [varchar](10) NULL,
	[MF081] [varchar](20) NULL,
	[MF082] [varchar](710) NULL,
	[MF083] [varchar](530) NULL,
	[MF084] [varchar](20) NULL,
	[MF085] [varchar](10) NULL,
	[MF086] [varchar](540) NULL,
	[MF087] [varchar](10) NULL,
	[MF088] [varchar](10) NULL,
	[MF089] [varchar](10) NULL,
	[MF090] [varchar](10) NULL,
	[MF092] [varchar](10) NULL,
	[MF095] [varchar](10) NULL,
	[MF096] [varchar](10) NULL,
	[MF097] [varchar](10) NULL,
	[MF098] [varchar](10) NULL,
	[MF099] [varchar](10) NULL,
	[MF100] [varchar](510) NULL,
	[MF101] [varchar](10) NULL,
	[MF102] [varchar](10) NULL,
	[MF103] [varchar](1070) NULL,
	[MF104] [varchar](1) NULL,
	[MF105] [varchar](1) NULL,
	[MF106] [varchar](10) NULL,
	[MF107] [varchar](10) NULL,
	[MF108] [varchar](1) NULL,
	[MF109] [varchar](10) NULL,
	[MF110] [varchar](10) NULL,
	[MF111] [varchar](1) NULL,
	[MF112] [varchar](1) NULL,
	[MF113] [varchar](10) NULL,
	[MF114] [varchar](10) NULL,
	[MF115] [varchar](1) NULL,
	[MF116] [varchar](10) NULL,
	[MF117] [varchar](10) NULL,
	[MF118] [varchar](1) NULL,
	[MF119] [varchar](1) NULL,
	[MF120] [varchar](10) NULL,
	[MF121] [varchar](10) NULL,
	[MF122] [varchar](1) NULL,
	[MF123] [varchar](10) NULL,
	[MF124] [varchar](10) NULL,
	[MF125] [varchar](1) NULL,
	[MF126] [varchar](1) NULL,
	[MF127] [varchar](10) NULL,
	[MF128] [varchar](10) NULL,
	[MF129] [varchar](1) NULL,
	[MF130] [varchar](10) NULL,
	[MF131] [varchar](10) NULL,
	[MF132] [varchar](1) NULL,
	[MF133] [varchar](10) NULL,
	[MF134] [varchar](10) NULL,
	[MF135] [varchar](1) NULL,
	[MF136] [varchar](10) NULL,
	[MF137] [varchar](10) NULL,
	[MF138] [varchar](1) NULL,
	[MF139] [varchar](10) NULL,
	[MF140] [varchar](10) NULL,
	[MF141] [varchar](1) NULL,
	[MF142] [varchar](10) NULL,
	[MF143] [varchar](10) NULL,
	[MF144] [varchar](1) NULL,
	[MF145] [varchar](10) NULL,
	[MF146] [varchar](10) NULL,
	[MF147] [varchar](1) NULL,
	[MF148] [varchar](10) NULL,
	[MF149] [varchar](10) NULL,
	[MF150] [varchar](1) NULL,
	[MF151] [varchar](10) NULL,
	[MF152] [varchar](20) NULL,
	[MF153] [varchar](60) NULL,
	[MF154] [varchar](10) NULL,
	[MF155] [varchar](10) NULL,
	[MF156] [varchar](900) NULL,
	[MF177] [varchar](40) NULL,
	[MF182] [varchar](1) NULL,
	[MF183] [varchar](10) NULL,
	[MF184] [varchar](20) NULL,
	[MF185] [varchar](10) NULL,
	[MF186] [varchar](1) NULL,
	[MF187] [varchar](10) NULL,
	[MF188] [varchar](20) NULL,
	[MF189] [varchar](10) NULL,
	[MF190] [varchar](1) NULL,
	[MF191] [varchar](20) NULL,
	[MF192] [varchar](20) NULL,
	[MF193] [varchar](1) NULL,
	[MF194] [varchar](10) NULL,
	[MF195] [varchar](20) NULL,
	[MF196] [varchar](10) NULL,
	[MF197] [varchar](1) NULL,
	[MF198] [varchar](10) NULL,
	[MF199] [varchar](20) NULL,
	[MF200] [varchar](10) NULL,
	[MF201] [varchar](840) NULL,
	[MF202] [varchar](50) NULL,
	[MF203] [varchar](90) NULL,
	[MF204] [varchar](23) NULL,
	[MF205] [varchar](23) NULL,
	[MF206] [varchar](10) NULL,
	[MF207] [varchar](10) NULL,
	[MF208] [varchar](200) NULL,
	[MF209] [varchar](300) NULL,
	[MF210] [varchar](300) NULL,
	[MF211] [varchar](120) NULL,
	[MF212] [varchar](70) NULL,
	[MF214] [varchar](10) NULL,
	[MF215] [varchar](1) NULL,
	[MF216] [varchar](10) NULL,
	[MF217] [varchar](10) NULL,
	[ProgramFolderID] [int] NULL,
	[MF218] [varchar](10) NULL,
	[MF219] [varchar](10) NULL,
	[MF220] [varchar](10) NULL,
	[MF221] [varchar](10) NULL,
	[MF222] [varchar](10) NULL,
	[MF223] [varchar](10) NULL,
	[MF224] [varchar](10) NULL,
	[MF225] [varchar](10) NULL,
	[MF226] [varchar](10) NULL,
	[MF227] [varchar](10) NULL,
	[MF228] [varchar](10) NULL,
	[MF229] [varchar](10) NULL,
	[RespondentUserIdentifier] [uniqueidentifier] NULL,
	[AgencyGroupIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_MfProgram] PRIMARY KEY CLUSTERED 
(
	[MfProgramID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[MrProgram](
	[MrProgramID] [int] IDENTITY(1,1) NOT NULL,
	[DateTimeSaved] [datetimeoffset](7) NULL,
	[DateTimeSubmitted] [datetimeoffset](7) NULL,
	[Deadline] [datetimeoffset](7) NULL,
	[InsertedBy] [varchar](128) NULL,
	[InsertedOn] [datetimeoffset](7) NULL,
	[RespondentName] [varchar](30) NULL,
	[StateName] [varchar](30) NULL,
	[SurveyYear] [int] NOT NULL,
	[UpdatedOn] [datetimeoffset](7) NULL,
	[UpdatedBy] [varchar](30) NULL,
	[IsVisibleOnTable14] [bit] NOT NULL,
	[IsVisibleOnTable01] [bit] NOT NULL,
	[IsVisibleOnTable02] [bit] NOT NULL,
	[IsVisibleOnTable03] [bit] NOT NULL,
	[IsVisibleOnTable04] [bit] NOT NULL,
	[IsVisibleOnTable05] [bit] NOT NULL,
	[IsVisibleOnTable06] [bit] NOT NULL,
	[IsVisibleOnTable07] [bit] NOT NULL,
	[IsVisibleOnTable08] [bit] NOT NULL,
	[IsVisibleOnTable09] [bit] NOT NULL,
	[IsVisibleOnTable10] [bit] NOT NULL,
	[IsVisibleOnTable11] [bit] NOT NULL,
	[IsVisibleOnTable12] [bit] NOT NULL,
	[IsVisibleOnTable13] [bit] NOT NULL,
	[MR001] [varchar](30) NULL,
	[MR002] [varchar](40) NULL,
	[MR003] [varchar](40) NULL,
	[MR004] [varchar](20) NULL,
	[MR005] [varchar](50) NULL,
	[MR006] [varchar](10) NULL,
	[MR007] [varchar](20) NULL,
	[MR008] [varchar](1) NULL,
	[MR009] [varchar](10) NULL,
	[MR010] [varchar](10) NULL,
	[MR011] [varchar](10) NULL,
	[MR012] [varchar](570) NULL,
	[MR013] [varchar](10) NULL,
	[MR014] [varchar](20) NULL,
	[MR015] [varchar](10) NULL,
	[MR016] [varchar](10) NULL,
	[MR017] [varchar](10) NULL,
	[MR018] [varchar](20) NULL,
	[MR019] [varchar](20) NULL,
	[MR020] [varchar](10) NULL,
	[MR021] [varchar](10) NULL,
	[MR022] [varchar](360) NULL,
	[MR023] [varchar](1) NULL,
	[MR024] [varchar](10) NULL,
	[MR025] [varchar](10) NULL,
	[MR026] [varchar](10) NULL,
	[MR027] [varchar](10) NULL,
	[MR028] [varchar](10) NULL,
	[MR029] [varchar](10) NULL,
	[MR030] [varchar](20) NULL,
	[MR031] [varchar](320) NULL,
	[MR032] [varchar](10) NULL,
	[MR033] [varchar](10) NULL,
	[MR034] [varchar](10) NULL,
	[MR035] [varchar](10) NULL,
	[MR036] [varchar](10) NULL,
	[MR037] [varchar](10) NULL,
	[MR038] [varchar](10) NULL,
	[MR039] [varchar](10) NULL,
	[MR040] [varchar](10) NULL,
	[MR041] [varchar](530) NULL,
	[MR042] [varchar](10) NULL,
	[MR043] [varchar](10) NULL,
	[MR044] [varchar](10) NULL,
	[MR045] [varchar](10) NULL,
	[MR046] [varchar](10) NULL,
	[MR047] [varchar](10) NULL,
	[MR048] [varchar](10) NULL,
	[MR049] [varchar](600) NULL,
	[MR050] [varchar](10) NULL,
	[MR051] [varchar](700) NULL,
	[MR052] [varchar](10) NULL,
	[MR053] [varchar](10) NULL,
	[MR054] [varchar](10) NULL,
	[MR055] [varchar](10) NULL,
	[MR056] [varchar](10) NULL,
	[MR057] [varchar](20) NULL,
	[MR058] [varchar](20) NULL,
	[MR059] [varchar](10) NULL,
	[MR060] [varchar](10) NULL,
	[MR061] [varchar](280) NULL,
	[MR062] [varchar](1) NULL,
	[MR063] [varchar](10) NULL,
	[MR064] [varchar](10) NULL,
	[MR065] [varchar](10) NULL,
	[MR066] [varchar](10) NULL,
	[MR067] [varchar](10) NULL,
	[MR068] [varchar](10) NULL,
	[MR069] [varchar](10) NULL,
	[MR070] [varchar](320) NULL,
	[MR071] [varchar](10) NULL,
	[MR072] [varchar](430) NULL,
	[MR078] [varchar](4000) NULL,
	[MR079] [varchar](90) NULL,
	[MR080] [varchar](250) NULL,
	[MR081] [varchar](200) NULL,
	[MR082] [varchar](20) NULL,
	[MR083] [varchar](20) NULL,
	[MR084] [varchar](10) NULL,
	[MR085] [varchar](350) NULL,
	[MR086] [varchar](10) NULL,
	[MR087] [varchar](120) NULL,
	[MR088] [varchar](20) NULL,
	[MR089] [varchar](10) NULL,
	[MR090] [varchar](10) NULL,
	[MR091] [varchar](10) NULL,
	[MR092] [varchar](10) NULL,
	[MR093] [varchar](660) NULL,
	[MR094] [varchar](20) NULL,
	[MR095] [varchar](10) NULL,
	[MR096] [varchar](20) NULL,
	[MR097] [varchar](10) NULL,
	[MR098] [varchar](20) NULL,
	[MR099] [varchar](10) NULL,
	[MR100] [varchar](20) NULL,
	[MR101] [varchar](10) NULL,
	[MR102] [varchar](110) NULL,
	[MR103] [varchar](20) NULL,
	[MR104] [varchar](20) NULL,
	[MR105] [varchar](500) NULL,
	[MR106] [varchar](70) NULL,
	[MR107] [varchar](20) NULL,
	[MR108] [varchar](10) NULL,
	[MR109] [varchar](10) NULL,
	[MR110] [varchar](10) NULL,
	[MR111] [varchar](10) NULL,
	[MR112] [varchar](90) NULL,
	[MR113] [varchar](20) NULL,
	[MR114] [varchar](10) NULL,
	[MR115] [varchar](10) NULL,
	[MR116] [varchar](10) NULL,
	[MR117] [varchar](10) NULL,
	[MR118] [varchar](10) NULL,
	[MR119] [varchar](10) NULL,
	[MR120] [varchar](220) NULL,
	[MR121] [varchar](140) NULL,
	[MR122] [varchar](800) NULL,
	[MR123] [varchar](10) NULL,
	[MR124] [varchar](10) NULL,
	[MR125] [varchar](10) NULL,
	[MR126] [varchar](10) NULL,
	[MR127] [varchar](10) NULL,
	[MR131] [varchar](10) NULL,
	[MR132] [varchar](110) NULL,
	[MR133] [varchar](10) NULL,
	[MR134] [varchar](110) NULL,
	[MR128] [varchar](10) NULL,
	[MR129] [varchar](10) NULL,
	[MR130] [varchar](10) NULL,
	[MR135] [varchar](420) NULL,
	[ProgramFolderID] [int] NULL,
	[MR136] [varchar](20) NULL,
	[MR137] [varchar](20) NULL,
	[MR138] [varchar](10) NULL,
	[MR139] [varchar](10) NULL,
	[MR140] [varchar](10) NULL,
	[MR141] [varchar](10) NULL,
	[MR142] [varchar](20) NULL,
	[MR143] [varchar](10) NULL,
	[MR144] [varchar](10) NULL,
	[MR145] [varchar](340) NULL,
	[MR146] [varchar](10) NULL,
	[MR147] [varchar](10) NULL,
	[MR148] [varchar](10) NULL,
	[MR149] [varchar](10) NULL,
	[MR150] [varchar](10) NULL,
	[MR151] [varchar](10) NULL,
	[MR152] [varchar](10) NULL,
	[MR153] [varchar](10) NULL,
	[MR154] [varchar](290) NULL,
	[MR155] [varchar](10) NULL,
	[MR156] [varchar](10) NULL,
	[MR157] [varchar](10) NULL,
	[MR158] [varchar](10) NULL,
	[MR159] [varchar](10) NULL,
	[MR160] [varchar](10) NULL,
	[MR161] [varchar](10) NULL,
	[MR162] [varchar](200) NULL,
	[MR163] [varchar](300) NULL,
	[IsVisibleOnTable15] [bit] NOT NULL,
	[IsVisibleOnTable16] [bit] NOT NULL,
	[MR164] [varchar](20) NULL,
	[MR165] [varchar](10) NULL,
	[MR166] [varchar](10) NULL,
	[MR167] [varchar](10) NULL,
	[MR168] [varchar](10) NULL,
	[MR169] [varchar](10) NULL,
	[MR170] [varchar](30) NULL,
	[MR171] [varchar](20) NULL,
	[MR172] [varchar](10) NULL,
	[MR173] [varchar](10) NULL,
	[MR174] [varchar](10) NULL,
	[MR175] [varchar](10) NULL,
	[MR176] [varchar](30) NULL,
	[MR177] [varchar](20) NULL,
	[MR178] [varchar](10) NULL,
	[MR179] [varchar](10) NULL,
	[MR180] [varchar](10) NULL,
	[MR181] [varchar](10) NULL,
	[MR182] [varchar](10) NULL,
	[MR183] [varchar](10) NULL,
	[MR184] [varchar](10) NULL,
	[MR185] [varchar](20) NULL,
	[MR186] [varchar](20) NULL,
	[RespondentUserIdentifier] [uniqueidentifier] NULL,
	[AgencyGroupIdentifier] [uniqueidentifier] NULL,
	[MR187] [varchar](20) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_MrProgram] PRIMARY KEY CLUSTERED 
(
	[MrProgramID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[PaProgram](
	[PaProgramID] [int] IDENTITY(1,1) NOT NULL,
	[DateTimeSaved] [datetimeoffset](7) NULL,
	[DateTimeSubmitted] [datetimeoffset](7) NULL,
	[Deadline] [datetimeoffset](7) NULL,
	[InsertedBy] [varchar](128) NULL,
	[InsertedOn] [datetimeoffset](7) NULL,
	[RespondentName] [varchar](40) NULL,
	[StateName] [varchar](30) NULL,
	[SurveyYear] [int] NOT NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedOn] [datetimeoffset](7) NULL,
	[IsVisibleOnTable01] [bit] NOT NULL,
	[IsVisibleOnTable02] [bit] NOT NULL,
	[IsVisibleOnTable03] [bit] NOT NULL,
	[IsVisibleOnTable04] [bit] NOT NULL,
	[PA001] [varchar](40) NULL,
	[PA002] [varchar](40) NULL,
	[PA003] [varchar](30) NULL,
	[PA004] [varchar](20) NULL,
	[PA005] [varchar](50) NULL,
	[PA006] [varchar](20) NULL,
	[PA007] [varchar](20) NULL,
	[PA008] [varchar](20) NULL,
	[PA009] [varchar](20) NULL,
	[PA010] [varchar](20) NULL,
	[PA011] [varchar](23) NULL,
	[PA012] [varchar](20) NULL,
	[PA013] [varchar](20) NULL,
	[PA014] [varchar](20) NULL,
	[PA015] [varchar](20) NULL,
	[PA016] [varchar](1010) NULL,
	[PA017] [varchar](20) NULL,
	[PA018] [varchar](20) NULL,
	[PA019] [varchar](20) NULL,
	[PA020] [varchar](20) NULL,
	[PA021] [varchar](20) NULL,
	[PA022] [varchar](1000) NULL,
	[PA023] [varchar](110) NULL,
	[PA024] [varchar](10) NULL,
	[PA025] [varchar](10) NULL,
	[PA026] [varchar](1240) NULL,
	[PA027] [varchar](20) NULL,
	[PA028] [varchar](860) NULL,
	[PA029] [varchar](1800) NULL,
	[PA030] [varchar](50) NULL,
	[PA031] [varchar](90) NULL,
	[PA032] [varchar](140) NULL,
	[PA033] [varchar](40) NULL,
	[PA034] [varchar](20) NULL,
	[PA035] [varchar](10) NULL,
	[PA036] [varchar](10) NULL,
	[PA037] [varchar](10) NULL,
	[PA038] [varchar](10) NULL,
	[PA039] [varchar](10) NULL,
	[PA040] [varchar](30) NULL,
	[PA041] [varchar](10) NULL,
	[PA042] [varchar](10) NULL,
	[ProgramFolderID] [int] NULL,
	[PA043] [varchar](20) NULL,
	[PA044] [varchar](20) NULL,
	[PA045] [varchar](20) NULL,
	[PA046] [varchar](20) NULL,
	[PA047] [varchar](20) NULL,
	[PA048] [varchar](20) NULL,
	[PA049] [varchar](20) NULL,
	[PA050] [varchar](20) NULL,
	[PA051] [varchar](20) NULL,
	[RespondentUserIdentifier] [uniqueidentifier] NULL,
	[AgencyGroupIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_PaProgram] PRIMARY KEY CLUSTERED 
(
	[PaProgramID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [custom_ncsha].[Program]
AS
    SELECT
        AgencyGroupIdentifier       AS AgencyGroupIdentifier
      , SurveyYear                  AS ProgramYear
      , StateName
      , AbProgramID                 AS ProgramID
      , 'Administration and Budget' AS ProgramName
      , 'AB'                        AS ProgramCode
    FROM
        custom_ncsha.AbProgram
    UNION
    SELECT
        AgencyGroupIdentifier          AS AgencyGroupIdentifier
      , SurveyYear                     AS ProgramYear
      , StateName
      , HiProgramID                    AS ProgramID
      , 'HOME Investment Partnerships' AS ProgramName
      , 'HI'                           AS ProgramCode
    FROM
        custom_ncsha.HiProgram
    UNION
    SELECT
        AgencyGroupIdentifier    AS AgencyGroupIdentifier
      , SurveyYear               AS ProgramYear
      , StateName
      , MrProgramID              AS ProgramID
      , 'Mortgage Revenue Bonds' AS ProgramName
      , 'MR'                     AS ProgramCode
    FROM
        custom_ncsha.MrProgram
    UNION
    SELECT
        AgencyGroupIdentifier AS AgencyGroupIdentifier
      , SurveyYear            AS ProgramYear
      , StateName
      , MfProgramID           AS ProgramID
      , 'Multifamily Bonds'   AS ProgramName
      , 'MF'                  AS ProgramCode
    FROM
        custom_ncsha.MfProgram
    UNION
    SELECT
        AgencyGroupIdentifier    AS AgencyGroupIdentifier
      , SurveyYear               AS ProgramYear
      , StateName
      , PaProgramID              AS ProgramID
      , 'Private Activity Bonds' AS ProgramName
      , 'PA'                     AS ProgramCode
    FROM
        custom_ncsha.PaProgram
    UNION
    SELECT
        AgencyGroupIdentifier            AS AgencyGroupIdentifier
      , SurveyYear                       AS ProgramYear
      , StateName
      , HcProgramID                      AS ProgramID
      , 'Low Income Housing Tax Credits' AS ProgramName
      , 'HC'                             AS ProgramCode
    FROM
        custom_ncsha.HcProgram;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [identities].[Division]
AS
SELECT OrganizationIdentifier
     , GroupIdentifier AS DivisionIdentifier
     , GroupCreated
     , GroupCode AS DivisionCode
     , GroupDescription AS DivisionDescription
     , GroupName AS DivisionName
     , LastChangeTime
     , LastChangeUser
FROM contacts.QGroup
WHERE GroupType = 'District';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [identities].[DuplicateEmail]
AS
WITH cte
AS (
   SELECT DISTINCT
          QGroup.OrganizationIdentifier
        , U.Email AS UserEmail
        , U.UserIdentifier
   FROM identities.[User] AS U
        INNER JOIN contacts.Membership ON Membership.UserIdentifier = U.UserIdentifier
        INNER JOIN contacts.QGroup ON QGroup.GroupIdentifier = Membership.GroupIdentifier
   WHERE U.Email IS NOT NULL)
SELECT cte.OrganizationIdentifier
     , UserEmail
     , COUNT(*) AS DuplicateCount
     , STRING_AGG(CAST(UserIdentifier AS VARCHAR(MAX)), ',') WITHIN GROUP (ORDER BY UserIdentifier) AS UserIdentifiers
FROM cte
GROUP BY cte.OrganizationIdentifier
       , UserEmail
HAVING COUNT(*) > 1;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [identities].[Impersonation](
	[ImpersonatedUserIdentifier] [uniqueidentifier] NOT NULL,
	[ImpersonationStarted] [datetimeoffset](7) NOT NULL,
	[ImpersonationStopped] [datetimeoffset](7) NULL,
	[ImpersonatorUserIdentifier] [uniqueidentifier] NOT NULL,
	[ImpersonationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Impersonation] PRIMARY KEY CLUSTERED 
(
	[ImpersonationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [identities].[ImpersonationSummary]
AS
SELECT Impersonator.AccessGrantedToCmds                                 AS ImpersonatorAccessGrantedToCmds
     , Impersonator.UserIdentifier                                      AS ImpersonatorUserIdentifier
     , Impersonator.FullName                                            AS ImpersonatorUserFullName
     , Person.UserIdentifier                                            AS ImpersonatedUserIdentifier
     , Person.FullName                                                  AS ImpersonatedUserFullName
     , I.ImpersonationStarted
     , I.ImpersonationStopped
     , CAST((CASE
                 WHEN Impersonator.AccountCloaked IS NOT NULL THEN
                     1
                 ELSE
                     0
             END
            ) AS BIT)                                                   AS ImpersonatorIsCloaked
FROM identities.Impersonation     AS I
     INNER JOIN identities.[User] AS Impersonator ON Impersonator.UserIdentifier = I.ImpersonatorUserIdentifier
     INNER JOIN identities.[User] AS Person ON Person.UserIdentifier = I.ImpersonatedUserIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [accounts].[TUserMock](
	[MockNumber] [int] IDENTITY(1,1) NOT NULL,
	[MockIdentifier] [uniqueidentifier] NOT NULL,
	[AccountCode] [varchar](20) NOT NULL,
	[AddressStreet] [varchar](100) NOT NULL,
	[AddressPostalCode] [varchar](20) NULL,
	[Birthdate] [date] NOT NULL,
	[Email] [varchar](254) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[Phone] [varchar](14) NOT NULL,
 CONSTRAINT [PK_TUserMock] PRIMARY KEY CLUSTERED 
(
	[MockIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [identities].[TUserMock] AS SELECT * FROM accounts.TUserMock;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [invoices].[QInvoice](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[InvoiceIdentifier] [uniqueidentifier] NOT NULL,
	[CustomerIdentifier] [uniqueidentifier] NOT NULL,
	[InvoiceStatus] [varchar](100) NOT NULL,
	[InvoiceDrafted] [datetimeoffset](7) NULL,
	[InvoiceSubmitted] [datetimeoffset](7) NULL,
	[InvoicePaid] [datetimeoffset](7) NULL,
	[InvoiceNumber] [int] NULL,
	[ReferencedInvoiceIdentifier] [uniqueidentifier] NULL,
	[BusinessCustomerGroupIdentifier] [uniqueidentifier] NULL,
	[EmployeeUserIdentifier] [uniqueidentifier] NULL,
	[IssueIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QInvoice] PRIMARY KEY CLUSTERED 
(
	[InvoiceIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [events].[QSeat](
	[SeatIdentifier] [uniqueidentifier] NOT NULL,
	[EventIdentifier] [uniqueidentifier] NOT NULL,
	[Configuration] [varchar](max) NULL,
	[Content] [nvarchar](max) NULL,
	[IsAvailable] [bit] NOT NULL,
	[IsTaxable] [bit] NOT NULL,
	[OrderSequence] [int] NULL,
	[SeatTitle] [varchar](100) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QSeat] PRIMARY KEY CLUSTERED 
(
	[SeatIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [payments].[QPayment](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[PaymentIdentifier] [uniqueidentifier] NOT NULL,
	[PaymentAmount] [decimal](7, 2) NOT NULL,
	[PaymentStatus] [varchar](10) NOT NULL,
	[PaymentStarted] [datetimeoffset](7) NULL,
	[PaymentAborted] [datetimeoffset](7) NULL,
	[PaymentDeclined] [datetimeoffset](7) NULL,
	[PaymentApproved] [datetimeoffset](7) NULL,
	[CustomerIP] [varchar](15) NULL,
	[ResultCode] [varchar](20) NULL,
	[ResultMessage] [varchar](200) NULL,
	[InvoiceIdentifier] [uniqueidentifier] NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[TransactionId] [varchar](50) NULL,
	[CardNumber] [varchar](10) NULL,
	[CardholderName] [nvarchar](32) NULL,
 CONSTRAINT [PK_QPayment] PRIMARY KEY CLUSTERED 
(
	[PaymentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [invoices].[VEventRegistrationPayment] as

select T.OrganizationCode as OrganizationCode,
       T.OrganizationIdentifier as OrganizationIdentifier,
       T.CompanyName as OrganizationName,
       
	   A.AchievementTitle,
	   I.InvoiceStatus,
	   I.InvoiceSubmitted,
       I.InvoiceIdentifier,
       I.InvoiceNumber,
       cast(null as varchar(max)) as OrderNumber,
       
	   E.EventTitle as EventName,
       E.EventScheduledStart as EventDate,
       
	   G.GroupName as EmployerName,
       U.FullName as LearnerAttendee,
       Person.PersonCode as LearnerCode,
       Customer.FullName as RegistrantCardholder,

	   P.PaymentIdentifier,
	   P.TransactionId as TransactionCode,
       P.PaymentAmount as TransactionAmount,
       P.PaymentStatus as TransactionStatus,
	   P.PaymentApproved as TransactionDate,

       R.RegistrationIdentifier,
       R.RegistrationFee,

       credit.CreditIdentifier,
       credit.CreditSubmitted,
       credit.CreditNumber,
       credit.CreditPaymentIdentifier,
       credit.CreditAmount

	   ,S.SeatTitle
	   ,R.AttendanceStatus as CurrentAttendanceStatus
	   ,R.ApprovalStatus as CurrentRegistrationStatus
	   ,R.AttendanceTaken

from events.QEvent as e
    inner join registrations.QRegistration as R on R.EventIdentifier = E.EventIdentifier
    inner join identities.[User] as U on U.UserIdentifier = R.CandidateIdentifier
    inner join payments.QPayment as P on P.PaymentIdentifier = R.PaymentIdentifier
    inner join accounts.QOrganization as T on T.OrganizationIdentifier = P.OrganizationIdentifier
    inner join invoices.QInvoice as I on I.InvoiceIdentifier = P.InvoiceIdentifier
    inner join identities.[User] as Customer on Customer.UserIdentifier = I.CustomerIdentifier
    left join contacts.Person on Person.UserIdentifier = U.UserIdentifier
                                and Person.OrganizationIdentifier = p.OrganizationIdentifier
    left join contacts.QGroup as G on G.GroupIdentifier = R.EmployerIdentifier
	left join [achievements].[QAchievement] as A on A.AchievementIdentifier = E.AchievementIdentifier
	left join events.QSeat as S on S.SeatIdentifier = R.SeatIdentifier

    outer apply (
        select top 1
            credit.InvoiceIdentifier as CreditIdentifier,
            credit.InvoiceSubmitted as CreditSubmitted,
            credit.InvoiceNumber as CreditNumber,
            creditPayment.PaymentIdentifier as CreditPaymentIdentifier,
            creditPayment.PaymentAmount as CreditAmount
        from
            invoices.QInvoice as credit
            inner join payments.QPayment as creditPayment on creditPayment.InvoiceIdentifier = credit.InvoiceIdentifier
        where
            credit.ReferencedInvoiceIdentifier = I.ReferencedInvoiceIdentifier
            and credit.InvoiceNumber > I.InvoiceNumber
            and creditPayment.PaymentAmount < 0
        order by
            credit.InvoiceNumber
    ) as credit

where R.PaymentIdentifier is not null
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [invoices].[QInvoiceItem](
	[InvoiceIdentifier] [uniqueidentifier] NOT NULL,
	[ItemIdentifier] [uniqueidentifier] NOT NULL,
	[ItemSequence] [int] NOT NULL,
	[ItemQuantity] [int] NOT NULL,
	[ItemPrice] [decimal](7, 2) NOT NULL,
	[ItemDescription] [varchar](400) NULL,
	[ProductIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[TaxRate] [decimal](5, 4) NULL,
 CONSTRAINT [PK_QInvoiceItem] PRIMARY KEY CLUSTERED 
(
	[ItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [invoices].[VInvoice]
AS
    SELECT
               I.OrganizationIdentifier
             , I.InvoiceIdentifier
             , I.CustomerIdentifier
             , I.InvoiceStatus
             , I.InvoiceDrafted
             , I.InvoiceSubmitted
             , I.InvoicePaid
             , I.InvoiceNumber
             , I.ReferencedInvoiceIdentifier
             , U.FullName AS CustomerFullName
             , U.Email    AS CustomerEmail
             , I.BusinessCustomerGroupIdentifier AS BusinessCustomerGroupIdentifier
             , I.EmployeeUserIdentifier AS EmployeeUserIdentifier
             , I.IssueIdentifier AS IssueIdentifier
             , Q2.GroupName AS BusinessCustomerName
             , U2.FullName AS EmployeeName
             , I2.IssueTitle AS IssueTitle
			, (
             SELECT CAST(
                        SUM(
                            CAST(ii.ItemQuantity * ii.ItemPrice AS decimal(19,4))
                            * (1 + COALESCE(ii.TaxRate, 0))
                        ) AS decimal(19,2)
                   )
               FROM invoices.QInvoiceItem AS ii
              WHERE ii.InvoiceIdentifier = I.InvoiceIdentifier
           ) AS InvoiceAmount
			 , (SELECT COUNT(*) FROM invoices.QInvoiceItem AS ii WHERE ii.InvoiceIdentifier = i.InvoiceIdentifier) AS ItemCount
			 , Q.GroupName AS CustomerEmployer
			 , P.PersonCode AS CustomerPersonCode
    FROM
               invoices.QInvoice AS I
    INNER JOIN identities.[User] AS U
               ON I.CustomerIdentifier = U.UserIdentifier
	LEFT JOIN contacts.Person AS P
			   ON  P.UserIdentifier = I.CustomerIdentifier and P.OrganizationIdentifier = I.OrganizationIdentifier
	LEFT JOIN contacts.QGroup AS Q
			   ON P.EmployerGroupIdentifier = Q.GroupIdentifier
    LEFT JOIN contacts.QGroup AS Q2
              ON I.BusinessCustomerGroupIdentifier = Q2.GroupIdentifier
    LEFT JOIN identities.[User] AS U2
              ON  U2.UserIdentifier = I.EmployeeUserIdentifier
    LEFT JOIN issues.QIssue AS I2
              ON  I2.IssueIdentifier = I.IssueIdentifier
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [accounts].[TDeveloper](
	[OrganizationCode] [varchar](100) NOT NULL,
	[TokenEmail] [varchar](254) NULL,
	[TokenExpiry] [datetimeoffset](7) NULL,
	[TokenIdentifier] [uniqueidentifier] NOT NULL,
	[TokenIssued] [datetimeoffset](7) NOT NULL,
	[TokenName] [varchar](100) NULL,
	[TokenSecret] [varchar](100) NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TDeveloper] PRIMARY KEY CLUSTERED 
(
	[TokenIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [contacts].[VDevPerson] AS
    SELECT o.OrganizationIdentifier
         , o.OrganizationCode
         , u.UserIdentifier
         , u.Email
         , p.PersonIdentifier
         , p.UserAccessGranted
         , isnull(p.IsAdministrator, 0) AS IsAdministrator
         , isnull(p.IsDeveloper, 0)     AS IsDeveloper
         , isnull(p.IsOperator, 0)      AS IsOperator
    FROM identities.QUser AS u
             CROSS APPLY accounts.QOrganization AS o
             LEFT JOIN contacts.QPerson AS p ON p.UserIdentifier = u.UserIdentifier AND
                                                p.OrganizationIdentifier =
                                                o.OrganizationIdentifier
    WHERE exists ( SELECT TokenEmail
                   FROM accounts.TDeveloper
                   WHERE TokenEmail LIKE '%shiftiq.com'
                     AND TokenEmail = u.Email )
      AND o.AccountClosed IS NULL;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [plugin].[CmdsBillableUserSummary]
as
select S.BillingClassification
	, S.TimeSensitiveResourceCount as LearnerAchievementCount_TimeSensitiveOnly
	, S.Email as LearnerEmail
	, S.UserIdentifier as LearnerIdentifier
	, S.UserIsApproved as LearnerIsApproved
	, S.UserIsDisabled as LearnerIsDisabled
	, S.FirstName as LearnerNameFirst
	, S.LastName as LearnerNameLast
	, S.CompanyCount as LearnerOrganizationCount
	, S.Phone as LearnerPhone
	, S.ProfileCount as LearnerProfileCount
	, S.OrganizationCode
	, S.OrganizationIdentifier
	, S.CompanyName as OrganizationName
from custom_cmds.BillableUserSummary as S;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [issues].[VIssueAttachment]
as
select [FileName]
     , FileIdentifier
     , AttachmentPosted as FileUploaded
     
	 , Inputter.UserIdentifier as InputterUserIdentifier
	 , Inputter.FullName as InputterUserName

	 , i.IssueIdentifier
     , i.IssueNumber
     , i.IssueTitle
	 , i.IssueType
          
     , i.OrganizationIdentifier

	 , u.Email as TopicUserEmail
	 , u.UserIdentifier as TopicUserIdentifier
	 , u.FullName as TopicUserName
	 
from issues.QIssueAttachment      as a
     inner join issues.QIssue     as i on i.IssueIdentifier = a.IssueIdentifier
     inner join identities.[User] as u on i.TopicUserIdentifier = u.UserIdentifier
	 left join identities.[User] as Inputter on Inputter.UserIdentifier = a.PosterIdentifier
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [issues].[QIssueUser](
	[IssueIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[IssueRole] [varchar](20) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QIssueUser] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [issues].[VIssueUser]
AS
SELECT IssueIdentifier
      ,TIssueUser.UserIdentifier
      ,IssueRole
	  ,TUser.FullName as UserFullName
FROM issues.QIssueUser AS TIssueUser
    LEFT JOIN identities.[User] AS TUser ON TUser.UserIdentifier = TIssueUser.UserIdentifier
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [identities].[QUserConnection](
	[Connected] [datetimeoffset](7) NOT NULL,
	[IsManager] [bit] NOT NULL,
	[IsSupervisor] [bit] NOT NULL,
	[IsValidator] [bit] NOT NULL,
	[FromUserIdentifier] [uniqueidentifier] NOT NULL,
	[ToUserIdentifier] [uniqueidentifier] NOT NULL,
	[IsLeader] [bit] NOT NULL,
 CONSTRAINT [PK_QUserConnection] PRIMARY KEY CLUSTERED 
(
	[FromUserIdentifier] ASC,
	[ToUserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [identities].[UserConnection]
as
select
    Connected
    ,IsLeader
    ,IsManager
    ,IsSupervisor
    ,IsValidator
    ,FromUserIdentifier
    ,ToUserIdentifier
from
    identities.QUserConnection
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [locations].[VAddress] AS 
SELECT
    City
  , Country
  , Description
  , PostalCode
  , Province
  , Street1
  , Street2
  , AddressIdentifier
FROM
    locations.Address;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assets].[TFile](
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ObjectType] [varchar](100) NOT NULL,
	[ObjectIdentifier] [uniqueidentifier] NOT NULL,
	[FileIdentifier] [uniqueidentifier] NOT NULL,
	[FileName] [varchar](200) NOT NULL,
	[FileSize] [int] NOT NULL,
	[FileLocation] [varchar](8) NOT NULL,
	[FileUrl] [varchar](500) NULL,
	[FilePath] [varchar](260) NULL,
	[FileContentType] [varchar](100) NOT NULL,
	[FileUploaded] [datetimeoffset](7) NOT NULL,
	[DocumentName] [varchar](200) NOT NULL,
	[FileDescription] [varchar](2400) NULL,
	[FileCategory] [varchar](120) NULL,
	[FileSubcategory] [varchar](120) NULL,
	[FileStatus] [varchar](20) NOT NULL,
	[FileExpiry] [datetimeoffset](7) NULL,
	[FileReceived] [datetimeoffset](7) NULL,
	[FileAlternated] [datetimeoffset](7) NULL,
	[LastActivityTime] [datetimeoffset](7) NULL,
	[LastActivityUserIdentifier] [uniqueidentifier] NULL,
	[ReviewedTime] [datetimeoffset](7) NULL,
	[ReviewedUserIdentifier] [uniqueidentifier] NULL,
	[ApprovedTime] [datetimeoffset](7) NULL,
	[ApprovedUserIdentifier] [uniqueidentifier] NULL,
	[AllowLearnerToView] [bit] NOT NULL,
 CONSTRAINT [PK_TFile] PRIMARY KEY CLUSTERED 
(
	[FileIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QExperience](
	[ExperienceIdentifier] [uniqueidentifier] NOT NULL,
	[JournalIdentifier] [uniqueidentifier] NOT NULL,
	[ValidatorUserIdentifier] [uniqueidentifier] NULL,
	[Employer] [nvarchar](100) NULL,
	[Supervisor] [nvarchar](100) NULL,
	[Instructor] [nvarchar](100) NULL,
	[ExperienceCreated] [datetimeoffset](7) NOT NULL,
	[ExperienceStarted] [date] NULL,
	[ExperienceStopped] [date] NULL,
	[ExperienceCompleted] [date] NULL,
	[ExperienceValidated] [datetimeoffset](7) NULL,
	[ExperienceHours] [decimal](20, 2) NULL,
	[ExperienceEvidence] [nvarchar](max) NULL,
	[TrainingLevel] [nvarchar](200) NULL,
	[TrainingLocation] [nvarchar](200) NULL,
	[TrainingProvider] [nvarchar](200) NULL,
	[TrainingCourseTitle] [nvarchar](200) NULL,
	[TrainingComment] [nvarchar](max) NULL,
	[SkillRating] [int] NULL,
	[Sequence] [int] NOT NULL,
	[TrainingType] [varchar](200) NULL,
	[MediaEvidenceName] [nvarchar](100) NULL,
	[MediaEvidenceType] [varchar](5) NULL,
	[MediaEvidenceFileIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QExperience] PRIMARY KEY CLUSTERED 
(
	[ExperienceIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [billing].[TProduct](
	[ProductIdentifier] [uniqueidentifier] NOT NULL,
	[ProductName] [varchar](100) NOT NULL,
	[ProductDescription] [varchar](2000) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ProductType] [varchar](30) NULL,
	[ProductPrice] [decimal](9, 2) NULL,
	[ProductCurrency] [varchar](3) NULL,
	[ProductImageUrl] [varchar](500) NULL,
	[ObjectType] [varchar](120) NULL,
	[ObjectIdentifier] [uniqueidentifier] NULL,
	[Published] [datetimeoffset](7) NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[PublishedBy] [uniqueidentifier] NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[ProductUrl] [varchar](2048) NULL,
	[ProductSummary] [varchar](500) NULL,
	[IsFeatured] [bit] NOT NULL,
	[IsTaxable] [bit] NOT NULL,
	[IndustryItemIdentifier] [uniqueidentifier] NULL,
	[OccupationItemIdentifier] [uniqueidentifier] NULL,
	[LevelItemIdentifier] [uniqueidentifier] NULL,
	[ProductQuantity] [int] NOT NULL,
 CONSTRAINT [PK_TProduct] PRIMARY KEY CLUSTERED 
(
	[ProductIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [assets].[VOrphanFile]
as
select
    FileIdentifier
    ,ObjectIdentifier
    ,ObjectType
from
    assets.TFile
where
    ObjectType = 'User'
    and ObjectIdentifier not in (select UserIdentifier from identities.QUser)

union
select
    FileIdentifier
    ,ObjectIdentifier
    ,ObjectType
from
    assets.TFile
where
    ObjectType = 'Issue'
    and FileIdentifier not in (select FileIdentifier from issues.QIssueAttachment where IssueIdentifier = ObjectIdentifier)

union
select
    FileIdentifier
    ,ObjectIdentifier
    ,ObjectType
from
    assets.TFile as f
where
    ObjectType = 'Response'
    and not exists (
        select 1 from surveys.QResponseAnswer as a
        where
            a.ResponseSessionIdentifier = f.ObjectIdentifier
            and a.ResponseAnswerText like '%/' + cast(f.FileIdentifier as varchar(max)) + '/%'
    )

union
select
    FileIdentifier
    ,ObjectIdentifier
    ,ObjectType
from
    assets.TFile as f
where
    ObjectType = 'Attempt'
    and not exists (
        select 1 from assessments.QAttemptQuestion as a
        where
            a.AttemptIdentifier = f.ObjectIdentifier
            and a.AnswerFileIdentifier = f.FileIdentifier
    )

union
select
    FileIdentifier
    ,ObjectIdentifier
    ,ObjectType
from
    assets.TFile
where
    ObjectType = 'Badge'
    and ObjectIdentifier not in (select AchievementIdentifier from achievements.QAchievement)

union
select
    FileIdentifier
    ,ObjectIdentifier
    ,ObjectType
from
    assets.TFile
where
    ObjectType = 'Product'
    and ObjectIdentifier not in (select ProductIdentifier from billing.TProduct)

union
select
    FileIdentifier
    ,ObjectIdentifier
    ,ObjectType
from
    assets.TFile
where
    ObjectType = 'LogbookExperience'
    and FileIdentifier not in (select MediaEvidenceFileIdentifier from records.QExperience)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [locations].[VOrganizationGroupAddress] as
select
    g.OrganizationIdentifier
    ,a.Country
    ,a.Province
    ,a.City
    ,count(*) as Occurrences
from
    contacts.QGroupAddress as a
    inner join contacts.QGroup as g on g.GroupIdentifier = a.GroupIdentifier
group by
    g.OrganizationIdentifier
    ,a.Country
    ,a.Province
    ,a.City
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [locations].[VOrganizationPersonAddress] AS
WITH cte AS 
(
  SELECT OrganizationIdentifier, BillingAddressIdentifier AS AddressIdentifier FROM contacts.Person WHERE BillingAddressIdentifier IS NOT NULL
  UNION ALL
  SELECT OrganizationIdentifier, HomeAddressIdentifier FROM contacts.Person WHERE HomeAddressIdentifier IS NOT NULL
  UNION ALL
  SELECT OrganizationIdentifier, ShippingAddressIdentifier FROM contacts.Person WHERE ShippingAddressIdentifier IS NOT NULL
  UNION ALL
  SELECT OrganizationIdentifier, WorkAddressIdentifier FROM contacts.Person WHERE WorkAddressIdentifier IS NOT NULL
)
SELECT cte.OrganizationIdentifier, A.Country, A.Province, A.City, COUNT(*) AS Occurrences
FROM locations.Address AS A INNER JOIN cte ON cte.AddressIdentifier = A.AddressIdentifier
GROUP BY cte.OrganizationIdentifier, A.Country, A.Province, A.City;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [messages].[ArchivedFollower]
as
select
    QFollower.MessageIdentifier
    ,QFollower.SubscriberIdentifier
    ,QFollower.FollowerIdentifier
from
    identities.[User]
    inner join messages.QFollower on QFollower.FollowerIdentifier = [User].UserIdentifier
where
    [User].UtcArchived is not null

union

select
    QFollower.MessageIdentifier
    ,QFollower.SubscriberIdentifier
    ,QFollower.FollowerIdentifier
from
    identities.[User]
    inner join messages.QFollower on QFollower.SubscriberIdentifier = [User].UserIdentifier
where
    [User].UtcArchived is not null
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [messages].[ArchivedSubscriber]
as
select
    QSubscriberUser.MessageIdentifier
    ,QSubscriberUser.UserIdentifier
from
    identities.[User]
    inner join messages.QSubscriberUser on QSubscriberUser.UserIdentifier = [User].UserIdentifier
where
    [User].UtcArchived is not null
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [messages].[QClick](
	[Clicked] [datetimeoffset](7) NOT NULL,
	[LinkIdentifier] [uniqueidentifier] NOT NULL,
	[UserBrowser] [varchar](32) NOT NULL,
	[UserHostAddress] [varchar](15) NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[ClickIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QClick] PRIMARY KEY CLUSTERED 
(
	[ClickIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [messages].[QLink](
	[ClickCount] [int] NOT NULL,
	[LinkIdentifier] [uniqueidentifier] NOT NULL,
	[LinkText] [varchar](256) NOT NULL,
	[LinkUrl] [varchar](500) NOT NULL,
	[LinkUrlHash] [varchar](500) NULL,
	[MessageIdentifier] [uniqueidentifier] NOT NULL,
	[UserCount] [int] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QLink] PRIMARY KEY CLUSTERED 
(
	[LinkIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [messages].[VClick]
as
select C.ClickIdentifier
     , C.Clicked
     , C.UserBrowser
     , C.UserHostAddress
     , M.MessageIdentifier
     , M.MessageTitle
     , M.OrganizationIdentifier
     , L.LinkIdentifier
     , L.LinkText
     , L.LinkUrl
     , U.UserIdentifier
     , U.FullName as UserFullName
     , U.Email    as UserEmail
from messages.QClick        as C
     join messages.QLink    as L on L.LinkIdentifier = C.LinkIdentifier
     join messages.QMessage as M on M.MessageIdentifier = L.MessageIdentifier
     join identities.[User] as U on U.UserIdentifier = C.UserIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [communications].[QMailout](
	[EventIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyIdentifier] [uniqueidentifier] NULL,
	[UserIdentifier] [uniqueidentifier] NULL,
	[SenderIdentifier] [uniqueidentifier] NOT NULL,
	[SenderStatus] [varchar](100) NULL,
	[SenderType] [varchar](50) NULL,
	[MessageType] [varchar](20) NOT NULL,
	[MessageName] [varchar](180) NOT NULL,
	[MessageIdentifier] [uniqueidentifier] NULL,
	[ContentBodyHtml] [nvarchar](max) NULL,
	[ContentBodyText] [nvarchar](max) NULL,
	[ContentPriority] [varchar](6) NULL,
	[ContentSubject] [nvarchar](180) NOT NULL,
	[ContentVariables] [nvarchar](max) NULL,
	[ContentAttachments] [nvarchar](max) NULL,
	[MailoutIdentifier] [uniqueidentifier] NOT NULL,
	[MailoutScheduled] [datetimeoffset](7) NOT NULL,
	[MailoutStarted] [datetimeoffset](7) NULL,
	[MailoutCancelled] [datetimeoffset](7) NULL,
	[MailoutCompleted] [datetimeoffset](7) NULL,
	[MailoutStatus] [varchar](20) NOT NULL,
	[MailoutStatusCode] [varchar](10) NULL,
	[MailoutError] [varchar](max) NULL,
	[RecipientEmailsTo] [varchar](max) NULL,
	[RecipientEmailsCc] [varchar](max) NULL,
	[RecipientEmailsBcc] [varchar](max) NULL,
	[RecipientIdentifiersTo] [varchar](max) NULL,
	[RecipientIdentifiersCc] [varchar](max) NULL,
	[RecipientIdentifiersBcc] [varchar](max) NULL,
	[RecipientListTo] [varchar](max) NULL,
	[RecipientListCc] [varchar](max) NULL,
	[RecipientListBcc] [varchar](max) NULL,
 CONSTRAINT [PK_QMailout] PRIMARY KEY CLUSTERED 
(
	[MailoutIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [communications].[QRecipient](
	[MailoutIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[UserEmail] [varchar](254) NOT NULL,
	[PersonCode] [varchar](20) NULL,
	[PersonName] [varchar](120) NULL,
	[PersonLanguage] [varchar](2) NULL,
	[RecipientIdentifier] [uniqueidentifier] NOT NULL,
	[RecipientVariables] [nvarchar](max) NULL,
	[DeliveryStarted] [datetimeoffset](7) NULL,
	[DeliveryCompleted] [datetimeoffset](7) NULL,
	[DeliveryStatus] [varchar](20) NULL,
	[DeliveryError] [varchar](max) NULL,
 CONSTRAINT [PK_QRecipient] PRIMARY KEY CLUSTERED 
(
	[RecipientIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [messages].[VMailout]
AS
SELECT M.MailoutIdentifier
     , M.MessageIdentifier
     , M.MessageType
     , M.MessageName
     , M.ContentSubject
     , M.ContentBodyHtml
     , M.ContentAttachments
     , S.SenderName
     , S.SenderEmail
     , M.MailoutStatus
     , M.MailoutStatusCode
     , M.MailoutError
     , M.MailoutScheduled
     , M.MailoutStarted
     , M.MailoutCompleted
     , M.MailoutCancelled
     , M.UserIdentifier
     , U.FullName AS UserName
     , M.OrganizationIdentifier
     , O.CompanyName AS OrganizationName
     , M.SurveyIdentifier
     , S.SenderType
     , M.EventIdentifier
     , M.ContentBodyText
     , M.SenderIdentifier
     , M.ContentVariables
     , (
         SELECT MAX(F2.AssetNumber)
         FROM surveys.QSurveyForm AS F2
         WHERE F2.SurveyFormIdentifier = M.SurveyIdentifier
       ) AS SurveyFormAsset
     , ( SELECT COUNT(*)FROM messages.QSubscriberUser AS S WHERE S.MessageIdentifier = M.MessageIdentifier ) AS SubscriberCount
     , (
         SELECT COUNT(*)
         FROM communications.QRecipient AS D
         WHERE D.MailoutIdentifier = M.MailoutIdentifier
       ) AS DeliveryCount
     , (
         SELECT COUNT(*)
         FROM communications.QRecipient AS D
         WHERE D.MailoutIdentifier = M.MailoutIdentifier
               AND D.DeliveryStatus IN ( 'Succeeded', 'Completed' )
       ) AS DeliveryCountSuccess
     , (
         SELECT COUNT(*)
         FROM communications.QRecipient AS D
         WHERE D.MailoutIdentifier = M.MailoutIdentifier
               AND D.DeliveryStatus NOT IN ( 'Succeeded', 'Completed' )
       ) AS DeliveryCountFailure
     , M.RecipientListTo
     , M.RecipientListCc
     , M.RecipientListBcc
FROM communications.QMailout AS M
  LEFT JOIN accounts.QOrganization AS O
    ON O.OrganizationIdentifier = M.OrganizationIdentifier
  LEFT JOIN identities.[User] AS U
    ON U.UserIdentifier = M.UserIdentifier
  LEFT JOIN accounts.TSender AS S
    ON M.SenderIdentifier = S.SenderIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Ensure the most current version of this view is deployed in v25.2.

CREATE VIEW [messages].[VMessage]
AS
SELECT M.AutoBccSubscribers
     , M.ContentHtml
     , M.ContentText
     , M.IsDisabled
     , M.MessageAttachments
     , M.MessageIdentifier
     , M.MessageName
     , M.MessageTitle
     , M.MessageType
     , M.SenderIdentifier
     , M.SurveyFormIdentifier
     , M.OrganizationIdentifier
     , M.LastChangeTime
     , M.LastChangeType
     , M.LastChangeUser
     , LastChangeUser.FullName AS LastChangeUserName
     , T.OrganizationCode
     , U.SenderEmail
     , U.SenderName
     , U.SenderNickname
     , U.SenderType
     , U.SystemMailbox
     , U.SenderEnabled
     , (
         SELECT COUNT(*)
         FROM [messages].QSubscriberGroup AS S
           INNER JOIN contacts.QGroup AS G
             ON G.GroupIdentifier = S.GroupIdentifier
         WHERE S.MessageIdentifier = M.MessageIdentifier
       ) AS SubscriberGroupCount
     , (
         SELECT COUNT(*)
         FROM [messages].QSubscriberUser AS S
         WHERE S.MessageIdentifier = M.MessageIdentifier
       ) AS SubscriberUserCount
     , (
         SELECT COUNT(DISTINCT GU.UserIdentifier)
         FROM [messages].QSubscriberGroup AS SG
           INNER JOIN contacts.QGroup AS G
             ON G.GroupIdentifier = SG.GroupIdentifier
           INNER JOIN contacts.Membership AS GU
             ON G.GroupIdentifier = GU.GroupIdentifier
           INNER JOIN identities.[User] AS U
             ON U.UserIdentifier = GU.UserIdentifier
         WHERE SG.MessageIdentifier = M.MessageIdentifier
       ) AS SubscriberMembershipCount
     , ( SELECT COUNT(*)FROM [messages].QLink AS L WHERE L.MessageIdentifier = M.MessageIdentifier ) AS LinkCount
     , (
         SELECT COUNT(*)
         FROM communications.QMailout
         WHERE QMailout.MessageIdentifier = M.MessageIdentifier
       ) AS MailoutCount
FROM [messages].QMessage AS M
  INNER JOIN accounts.QOrganization AS T
    ON T.OrganizationIdentifier = M.OrganizationIdentifier
  LEFT JOIN accounts.TSender AS U
    ON M.SenderIdentifier = U.SenderIdentifier
  LEFT JOIN identities.[User] AS LastChangeUser
    ON LastChangeUser.UserIdentifier = M.LastChangeUser;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [records].[VGradeItemHierarchy]
AS
    WITH Hierarchy AS (
                      SELECT
                          i.GradebookIdentifier
                        , i.ParentGradeItemIdentifier
                        , i.GradeItemIdentifier
                        , CAST(FORMAT(i.GradeItemSequence, '00') AS VARCHAR(MAX)) AS PathSequence
                        , CAST(ISNULL(i.GradeItemCode, '') AS VARCHAR(MAX))       AS PathCode
                        , 0                                                       AS PathDepth
                        , i.GradeItemName
                        , i.GradeItemType
                        , i.GradeItemFormat
                      FROM
                          records.QGradeItem AS i
                      WHERE
                          i.ParentGradeItemIdentifier IS NULL
                      UNION ALL
                      SELECT
                          i.GradebookIdentifier
                        , i.ParentGradeItemIdentifier
                        , i.GradeItemIdentifier
                        , CAST(Parent.PathSequence + '.' + CAST(FORMAT(i.GradeItemSequence, '00') AS VARCHAR(MAX)) AS VARCHAR(MAX))
                        , CAST(Parent.PathCode + '.' + ISNULL(i.GradeItemCode, '') AS VARCHAR(MAX))
                        , 1 + Parent.PathDepth AS PathDepth
                        , i.GradeItemName
                        , i.GradeItemType
                        , i.GradeItemFormat
                      FROM
                          Hierarchy                 AS Parent
                      INNER JOIN records.QGradeItem AS i
                                 ON i.ParentGradeItemIdentifier = Parent.GradeItemIdentifier)
    SELECT
        *
      , SUBSTRING('######', 1, PathDepth + 1) AS PathIndent
    FROM
        Hierarchy;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [utilities].[TCollection](
	[CollectionName] [varchar](250) NOT NULL,
	[CollectionReferences] [varchar](100) NULL,
	[CollectionTool] [varchar](100) NOT NULL,
	[CollectionPackage] [varchar](100) NULL,
	[CollectionProcess] [varchar](100) NOT NULL,
	[CollectionType] [varchar](20) NOT NULL,
	[CollectionIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TCollection] PRIMARY KEY CLUSTERED 
(
	[CollectionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [achievements].[TAchievementCategory] AS SELECT
                                                    I.ItemFolder AS AchievementLabel,
                                                    I.ItemIdentifier AS CategoryIdentifier, I.ItemName AS CategoryName,
                                                    I.ItemDescription AS CategoryDescription, I.OrganizationIdentifier
                                                FROM
                                                    utilities.TCollection AS C
                                                    INNER JOIN utilities.TCollectionItem AS I
                                                    ON I.CollectionIdentifier = C.CollectionIdentifier
                                                WHERE
                                                    C.CollectionIdentifier = '86A5FDB2-313B-4026-B8D3-DFE1EA85A3B0';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QJournal](
	[JournalIdentifier] [uniqueidentifier] NOT NULL,
	[JournalSetupIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[JournalCreated] [datetimeoffset](7) NOT NULL,
	[LastUpdateNotificationToAdmin] [datetimeoffset](7) NULL,
	[LastCommentNotificationToAdmin] [datetimeoffset](7) NULL,
	[LastCommentNotificationToUser] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_QJournal] PRIMARY KEY CLUSTERED 
(
	[JournalIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QJournalSetupUser](
	[JournalSetupIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[UserRole] [varchar](50) NOT NULL,
	[EnrollmentIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QJournalSetupUser] PRIMARY KEY CLUSTERED 
(
	[EnrollmentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [records].[VJournalSetupUser]
as
select
    s.JournalSetupIdentifier
    ,s.UserIdentifier
    ,js.OrganizationIdentifier
    ,j.JournalIdentifier
    ,p.EmployerGroupIdentifier
    ,js.AchievementIdentifier
    ,s.UserRole
    ,js.JournalSetupName
    ,js.JournalSetupCreated
    ,u.FullName as UserFullName
    ,u.Email as UserEmail
    ,u.EmailAlternate as UserEmailAlternate
    ,g.GroupName as EmployerGroupName
    ,p.PersonCode
from
    records.QJournalSetupUser as s
    inner join records.QJournalSetup as js on js.JournalSetupIdentifier = s.JournalSetupIdentifier
    inner join identities.[User] as u on u.UserIdentifier = s.UserIdentifier
    left join contacts.Person as p on p.UserIdentifier = s.UserIdentifier
                                    and p.OrganizationIdentifier = js.OrganizationIdentifier
    left join contacts.QGroup as g on g.GroupIdentifier = p.EmployerGroupIdentifier
    left join records.QJournal as j on j.JournalSetupIdentifier = s.JournalSetupIdentifier
                                    and j.UserIdentifier = s.UserIdentifier
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[TProgramEnrollment](
	[EnrollmentIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerUserIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ProgramIdentifier] [uniqueidentifier] NOT NULL,
	[ProgressStarted] [datetimeoffset](7) NULL,
	[ProgressCompleted] [datetimeoffset](7) NULL,
	[Created] [datetimeoffset](7) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[MessageCompletedSentCount] [int] NOT NULL,
	[MessageStalledSentCount] [int] NOT NULL,
 CONSTRAINT [PK_TProgramEnrollment] PRIMARY KEY CLUSTERED 
(
	[EnrollmentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [records].[VProgramEnrollment]
as
select P.ProgramIdentifier
     , P.ProgramCode
     , P.ProgramDescription
     , P.ProgramName
     , P.OrganizationIdentifier
     , U.UserIdentifier
     , U.FullName       as UserFullName
     , U.Email          as UserEmail
     , U.EmailAlternate as UserEmailAlternate
     , U.PhoneMobile    as UserPhone
     , PU.EnrollmentIdentifier
     , PU.Created       as ProgressAssigned
     , PU.ProgressStarted
     , PU.ProgressCompleted
     , PU.Created       as CreatedWhen
     , Creator.FullName as CreatedWho
     , TimeTaken        = case
                              when PU.ProgressStarted is null then
                                  null
                              when PU.ProgressCompleted is null then
                                  datediff(day, PU.ProgressStarted, sysdatetimeoffset())
                              else
                                  datediff(day, PU.ProgressStarted, PU.ProgressCompleted)
                          end
from identities.[User]                     as U
     inner join records.TProgramEnrollment as PU on PU.LearnerUserIdentifier = U.UserIdentifier
     inner join records.TProgram           as P on P.ProgramIdentifier = PU.ProgramIdentifier
     left join identities.[User]           as Creator on Creator.UserIdentifier = PU.CreatedBy;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [achievements].[TAchievementClassification]
AS
SELECT G.ItemIdentifier AS CategoryIdentifier,
       G.CategorySequence AS ClassificationSequence,
       G.AchievementIdentifier,
       G.OrganizationIdentifier
FROM record.TAchievementCategory AS G;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [records].[VLearnerActivity]
AS
SELECT

	   U.UserIdentifier AS LearnerIdentifier,
	   U.FullName AS LearnerName,
	   U.LastName AS LearnerNameLast,
       U.FirstName AS LearnerNameFirst,
       U.Email AS LearnerEmail,
	   P.Gender AS LearnerGender,
	   P.Created AS LearnerCreated,
	   P.PersonCode,

	   p.Phone AS LearnerPhone,
	   CONVERT(DATE, p.Birthdate) AS LearnerBirthdate,
	   p.JobTitle AS LearnerOccupation,
	   p.ConsentConsultation AS LearnerConsent,
	   CASE WHEN P.IsAdministrator = 1 THEN 'Administrator' WHEN P.IsLearner = 1 THEN 'Learner' ELSE NULL END AS LearnerRole,

	   P.Citizenship AS LearnerCitizenship,
	   P.ImmigrationNumber,
	   P.ImmigrationLandingDate AS ImmigrationArrival,
	   CASE 
	   WHEN P.ImmigrationLandingDate IS NOT NULL AND P.ImmigrationLandingDate  > P.Created THEN 'Pre-Arrival'
	   WHEN P.ImmigrationLandingDate IS NOT NULL AND P.ImmigrationLandingDate <= P.Created THEN 'Post-Arrival' 
	   ELSE NULL
	   END AS ImmigrationStatus,
	   P.ImmigrationDestination,

	   P.Referrer AS ReferrerName,
	   P.ReferrerOther AS ReferrerOther,

	   Program.GroupIdentifier AS ProgramIdentifier,
	   Program.GroupName AS ProgramName,

	   G.GradebookIdentifier,
       G.GradebookTitle AS GradebookName,

	   A.AchievementIdentifier,
	   A.AchievementLabel,
	   A.AchievementTitle,

       M.Assigned AS EnrollmentStarted,

	   CASE 
	   WHEN C.CredentialStatus = 'Valid' THEN '3. Completed'
	   WHEN E.EnrollmentStarted IS NOT NULL THEN '2. Started'
	   ELSE '1. Registered'
	   END AS EnrollmentStatus,

	   CASE
	   WHEN C.CredentialStatus = 'Valid' THEN 'Granted'
	   WHEN E.EnrollmentStarted IS NOT NULL THEN 'Not Granted'
	   ELSE 'Not Granted'
	   END AS CertificateStatus,

	   C.CredentialGranted AS CertificateGranted,

	   Program.OrganizationIdentifier,

	   (SELECT MIN(S.SessionStarted) FROM [security].TUserSession AS S WHERE S.UserIdentifier = LearnerIdentifier) AS SessionStartedFirst,
	   (SELECT MAX(S.SessionStarted) FROM [security].TUserSession AS S WHERE S.UserIdentifier = LearnerIdentifier) AS SessionStartedLast,
	   (SELECT COUNT(*) FROM [security].TUserSession AS S WHERE S.UserIdentifier = LearnerIdentifier) AS SessionCount,
	   (SELECT SUM(S.SessionMinutes) FROM [security].TUserSession AS S WHERE S.UserIdentifier = LearnerIdentifier) AS SessionMinutes,

       MembershipStatusItem.ItemIdentifier AS MembershipStatusItemIdentifier,
       MembershipStatusItem.ItemName       AS MembershipStatusItemName,
       
       EmployerGroup.GroupIdentifier AS EmployerGroupIdentifier,
       EmployerGroup.GroupName       AS EmployerGroupName

FROM identities.[User] AS U
INNER JOIN contacts.Person AS P ON P.UserIdentifier = U.UserIdentifier AND P.IsLearner = 1 
INNER JOIN contacts.Membership AS M ON M.UserIdentifier = P.UserIdentifier
INNER JOIN contacts.QGroup AS Program ON Program.GroupIdentifier = M.GroupIdentifier AND Program.GroupCategory = 'Program'
INNER JOIN accounts.QOrganization AS T ON T.OrganizationIdentifier = Program.OrganizationIdentifier AND T.OrganizationIdentifier = P.OrganizationIdentifier
LEFT JOIN (records.QEnrollment AS E 
	INNER JOIN records.QGradebook AS G ON G.GradebookIdentifier = E.GradebookIdentifier 
	INNER JOIN achievements.QAchievement AS A ON A.AchievementIdentifier = G.AchievementIdentifier
	LEFT JOIN achievements.QCredential AS C ON C.AchievementIdentifier = A.AchievementIdentifier AND C.UserIdentifier = E.LearnerIdentifier) 
	ON E.LearnerIdentifier = U.UserIdentifier AND G.OrganizationIdentifier = T.OrganizationIdentifier
LEFT JOIN utilities.TCollectionItem AS MembershipStatusItem on MembershipStatusItem.ItemIdentifier = P.MembershipStatusItemIdentifier
LEFT JOIN contacts.QGroup AS EmployerGroup ON EmployerGroup.GroupIdentifier = P.EmployerGroupIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [courses].[QCourseEnrollment](
	[CourseEnrollmentIdentifier] [uniqueidentifier] NOT NULL,
	[CourseIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerUserIdentifier] [uniqueidentifier] NOT NULL,
	[CourseStarted] [datetimeoffset](7) NOT NULL,
	[MessageStalledSentCount] [int] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[CourseCompleted] [datetimeoffset](7) NULL,
	[MessageCompletedSentCount] [int] NOT NULL,
 CONSTRAINT [PK_QCourseEnrollment] PRIMARY KEY CLUSTERED 
(
	[CourseEnrollmentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [courses].[TCourseUser]
as
select
    CourseIdentifier
    ,LearnerUserIdentifier as UserIdentifier
    ,CourseStarted
    ,MessageStalledSentCount
    ,OrganizationIdentifier
    ,CourseCompleted
    ,MessageCompletedSentCount
    ,CourseEnrollmentIdentifier as EnrollmentIdentifier
from
    courses.QCourseEnrollment
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[TTask](
	[ProgramIdentifier] [uniqueidentifier] NOT NULL,
	[ObjectIdentifier] [uniqueidentifier] NOT NULL,
	[ObjectType] [varchar](100) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[TaskCompletionRequirement] [varchar](40) NOT NULL,
	[TaskIdentifier] [uniqueidentifier] NOT NULL,
	[TaskIsRequired] [bit] NOT NULL,
	[TaskLifetimeMonths] [int] NULL,
	[TaskSequence] [int] NOT NULL,
	[TaskImage] [varchar](200) NULL,
	[TaskIsPlanned] [bit] NOT NULL,
 CONSTRAINT [PK_TTask] PRIMARY KEY CLUSTERED 
(
	[TaskIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [learning].[VProgram]
    AS
        SELECT AchievementElseCommand,
               AchievementFixedDate,
               AchievementIdentifier,
               AchievementThenCommand,
               AchievementWhenChange,
               AchievementWhenGrade,
               P.CatalogIdentifier,
               CatalogName,
               CompletionTaskIdentifier,
               P.GroupIdentifier,
               G.GroupName,
               NotificationCompletedAdministratorMessageIdentifier,
               NotificationCompletedLearnerMessageIdentifier,
               NotificationStalledAdministratorMessageIdentifier,
               NotificationStalledLearnerMessageIdentifier,
               NotificationStalledReminderLimit,
               NotificationStalledTriggerDay,
               P.OrganizationIdentifier,
               ProgramCode,
               ProgramDescription,
               ProgramIcon,
               ProgramIdentifier,
               ProgramImage,
               ProgramName,
               ProgramSlug,
               ProgramTag,
               ProgramType,

               ( SELECT COUNT(*)
                 FROM learning.TProgramCategory AS PC
                 WHERE PC.ProgramIdentifier = P.ProgramIdentifier ) AS CategoryCount,

               ( SELECT COUNT(*)
                 FROM records.TProgramEnrollment AS E
                 WHERE E.ProgramIdentifier = P.ProgramIdentifier )  AS EnrollmentCount,

            ( SELECT COUNT(*)
                 FROM records.TTask AS T
                 WHERE T.ProgramIdentifier = P.ProgramIdentifier )  AS TaskCount

        FROM records.TProgram AS P
                 LEFT JOIN contacts.QGroup AS G
                           ON G.GroupIdentifier = P.GroupIdentifier
                 LEFT JOIN TCatalog AS C ON C.CatalogIdentifier = P.CatalogIdentifier
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [records].[VTopStudent]
as
select
    progress.ProgressIdentifier
    ,u.FullName as UserFullName
    ,a.AchievementTitle
    ,g.GroupName as EmployerName
    ,g.GroupRegion as EmployerRegion
    ,item.GradeItemName
    ,progress.ProgressPercent
from
    records.QProgress as progress
    inner join records.QGradeItem as item on item.GradeItemIdentifier = progress.GradeItemIdentifier
    inner join records.QGradebook as gradebook on gradebook.GradebookIdentifier = progress.GradebookIdentifier
    inner join achievements.QAchievement as a on a.AchievementIdentifier = gradebook.AchievementIdentifier
    inner join identities.[User] as u on u.UserIdentifier = progress.UserIdentifier
    inner join contacts.Person as p on p.UserIdentifier = u.UserIdentifier
                                    and p.OrganizationIdentifier = gradebook.OrganizationIdentifier
    left join contacts.QGroup as g on g.GroupIdentifier = p.EmployerGroupIdentifier
where
    progress.ProgressPercent is not null
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitText]
(
    @input NVARCHAR(MAX)
  , @delimiter NVARCHAR(1)
)
RETURNS TABLE
AS
RETURN SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS ItemNumber
            , value                                      AS ItemText
       FROM STRING_SPLIT(@input, @delimiter);
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- The newid() operator cannot be used inside a function.
CREATE VIEW [database].[GetNewIdentifier]
AS
SELECT NEWID() AS NewIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Only include primary key constraints in the Primary Key view.

CREATE VIEW [databases].[VPrimaryKey]
AS
SELECT 
    SS.name AS SchemaName,
    ST.name AS TableName,
    SC.name AS ColumnName,
    SKC.name AS ConstraintName,
    CAST(STY.name AS VARCHAR(20)) + '(' + 
        CAST(
            CASE STY.name
                WHEN 'NVARCHAR' THEN (SC.max_length / 2)
                ELSE SC.max_length
            END 
        AS VARCHAR(20)) + ')' AS DataType,
    SC.is_identity AS IsIdentity
FROM 
    sys.key_constraints AS SKC
    INNER JOIN sys.tables AS ST ON ST.object_id = SKC.parent_object_id
    INNER JOIN sys.schemas AS SS ON SS.schema_id = ST.schema_id
    INNER JOIN sys.index_columns AS SIC ON SIC.object_id = ST.object_id AND SIC.index_id = SKC.unique_index_id
    INNER JOIN sys.columns AS SC ON SC.object_id = ST.object_id AND SC.column_id = SIC.column_id
    INNER JOIN sys.types AS STY ON SC.user_type_id = STY.user_type_id
WHERE 
    SKC.type = 'PK'; -- Only include primary key constraints
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [metadata].[VPrimaryKey]
AS
SELECT 
  SS.name AS SchemaName,
  ST.name AS TableName,
  SC.name AS ColumnName,
  SKC.name AS ConstraintName,
  CAST(STY.name AS VARCHAR(20)) + '(' + 
    CAST(CASE STY.name
             WHEN 'NVARCHAR' THEN (SC.max_length / 2)
             ELSE SC.max_length
         END AS VARCHAR(20)) + ')' AS DataType,
  SC.is_identity AS IsIdentity
FROM sys.key_constraints AS SKC
INNER JOIN sys.tables AS ST ON ST.object_id = SKC.parent_object_id
INNER JOIN sys.schemas AS SS ON SS.schema_id = ST.schema_id
INNER JOIN sys.index_columns AS SIC ON SIC.object_id = ST.object_id AND SIC.index_id = SKC.unique_index_id
INNER JOIN sys.columns AS SC ON SC.object_id = ST.object_id AND SC.column_id = SIC.column_id
INNER JOIN sys.types AS STY ON SC.user_type_id = STY.user_type_id
WHERE SKC.type = 'PK';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [metadata].[VSchema]
AS
SELECT 
  name AS SchemaName,
  (
    SELECT COUNT(*)
    FROM sys.objects
    WHERE objects.schema_id = schemas.schema_id
  ) AS ObjectCount,
  (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = name
          AND TABLE_TYPE = 'BASE TABLE'
  ) AS TableCount
FROM sys.schemas
WHERE name NOT LIKE 'db_%'
      AND name NOT IN ('guest', 'INFORMATION_SCHEMA', 'sys');
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [accounts].[TSenderOrganization](
	[SenderIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TSenderOrganization] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [accounts].[TUserAuthenticationFactor](
	[UserAuthenticationFactorIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[OtpTokenRefreshed] [datetimeoffset](7) NOT NULL,
	[OtpRecoveryPhrases] [varchar](254) NOT NULL,
	[OtpMode] [int] NOT NULL,
	[ShortLivedCookieToken] [uniqueidentifier] NULL,
	[ShortCookieTokenStartTime] [datetimeoffset](7) NULL,
	[OtpTokenSecret] [varchar](512) NULL,
 CONSTRAINT [PK_TUserAuthenticationFactor] PRIMARY KEY CLUSTERED 
(
	[UserAuthenticationFactorIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [accounts].[TUserComment](
	[CommentIdentifier] [uniqueidentifier] NOT NULL,
	[CommentCreated] [datetimeoffset](7) NOT NULL,
	[CommentCreatedBy] [uniqueidentifier] NOT NULL,
	[CommentText] [varchar](2600) NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TUserComment] PRIMARY KEY CLUSTERED 
(
	[CommentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [accounts].[TUserSetting](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[Name] [varchar](128) NOT NULL,
	[ValueType] [varchar](256) NOT NULL,
	[ValueJson] [varchar](max) NOT NULL,
	[SettingIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TUserSetting] PRIMARY KEY CLUSTERED 
(
	[SettingIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[QAttemptMatch](
	[AttemptIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionSequence] [int] NOT NULL,
	[MatchSequence] [int] NOT NULL,
	[MatchLeftText] [nvarchar](max) NOT NULL,
	[MatchRightText] [nvarchar](max) NOT NULL,
	[AnswerText] [nvarchar](max) NULL,
	[MatchPoints] [decimal](7, 2) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QAttemptMatch] PRIMARY KEY CLUSTERED 
(
	[AttemptIdentifier] ASC,
	[QuestionIdentifier] ASC,
	[MatchSequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[QAttemptPin](
	[AttemptIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionSequence] [int] NOT NULL,
	[OptionKey] [int] NULL,
	[OptionPoints] [decimal](7, 2) NULL,
	[OptionSequence] [int] NULL,
	[OptionText] [nvarchar](max) NULL,
	[PinSequence] [int] NOT NULL,
	[PinX] [int] NOT NULL,
	[PinY] [int] NOT NULL,
 CONSTRAINT [PK_QAttemptPin] PRIMARY KEY CLUSTERED 
(
	[AttemptIdentifier] ASC,
	[QuestionIdentifier] ASC,
	[PinSequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[QAttemptSection](
	[AttemptIdentifier] [uniqueidentifier] NOT NULL,
	[SectionIndex] [int] NOT NULL,
	[SectionIdentifier] [uniqueidentifier] NULL,
	[SectionStarted] [datetimeoffset](7) NULL,
	[SectionCompleted] [datetimeoffset](7) NULL,
	[SectionDuration] [int] NULL,
	[TimeLimit] [int] NULL,
	[TimerType] [varchar](8) NULL,
	[IsBreakTimer] [bit] NOT NULL,
	[ShowWarningNextTab] [bit] NOT NULL,
 CONSTRAINT [PK_QAttemptSection] PRIMARY KEY CLUSTERED 
(
	[AttemptIdentifier] ASC,
	[SectionIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[QAttemptSolution](
	[AttemptIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionSequence] [int] NOT NULL,
	[SolutionIdentifier] [uniqueidentifier] NOT NULL,
	[SolutionSequence] [int] NOT NULL,
	[SolutionOptionsOrder] [varchar](512) NOT NULL,
	[SolutionPoints] [decimal](7, 2) NOT NULL,
	[SolutionCutScore] [decimal](5, 4) NULL,
	[AnswerIsMatched] [bit] NULL,
 CONSTRAINT [PK_QAttemptSolution] PRIMARY KEY CLUSTERED 
(
	[AttemptIdentifier] ASC,
	[QuestionIdentifier] ASC,
	[SolutionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[TQuiz](
	[QuizIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[QuizType] [varchar](20) NOT NULL,
	[QuizName] [nvarchar](100) NOT NULL,
	[QuizData] [nvarchar](4000) NOT NULL,
	[TimeLimit] [int] NOT NULL,
	[AttemptLimit] [int] NOT NULL,
	[PassingScore] [decimal](5, 4) NULL,
	[MaximumPoints] [decimal](7, 2) NULL,
	[PassingPoints] [decimal](7, 2) NULL,
	[PassingAccuracy] [decimal](3, 2) NOT NULL,
	[PassingWpm] [int] NOT NULL,
	[PassingKph] [int] NOT NULL,
 CONSTRAINT [PK_TQuiz] PRIMARY KEY CLUSTERED 
(
	[QuizIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assessments].[TQuizAttempt](
	[AttemptIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[QuizIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerIdentifier] [uniqueidentifier] NOT NULL,
	[AttemptCreated] [datetimeoffset](7) NOT NULL,
	[AttemptStarted] [datetimeoffset](7) NULL,
	[AttemptCompleted] [datetimeoffset](7) NULL,
	[QuizGradebookIdentifier] [uniqueidentifier] NOT NULL,
	[QuizType] [varchar](20) NOT NULL,
	[QuizName] [nvarchar](100) NOT NULL,
	[QuizData] [nvarchar](4000) NULL,
	[QuizTimeLimit] [int] NOT NULL,
	[QuizPassingAccuracy] [decimal](3, 2) NOT NULL,
	[QuizPassingWpm] [int] NOT NULL,
	[QuizPassingKph] [int] NOT NULL,
	[AttemptData] [nvarchar](4000) NULL,
	[AttemptIsPassing] [bit] NULL,
	[AttemptScore] [decimal](9, 8) NULL,
	[AttemptDuration] [decimal](9, 3) NULL,
	[AttemptMistakes] [int] NULL,
	[AttemptAccuracy] [decimal](5, 4) NULL,
	[AttemptCharsPerMin] [int] NULL,
	[AttemptWordsPerMin] [int] NULL,
	[AttemptKeystrokesPerHour] [int] NULL,
	[AttemptSpeed] [decimal](9, 2) NULL,
 CONSTRAINT [PK_TQuizAttempt] PRIMARY KEY CLUSTERED 
(
	[AttemptIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assets].[QGlossaryTerm](
	[TermName] [varchar](200) NOT NULL,
	[TermStatus] [varchar](10) NOT NULL,
	[Proposed] [datetimeoffset](7) NOT NULL,
	[ProposedBy] [varchar](100) NOT NULL,
	[Approved] [datetimeoffset](7) NULL,
	[ApprovedBy] [varchar](100) NULL,
	[LastRevised] [datetimeoffset](7) NULL,
	[LastRevisedBy] [varchar](100) NULL,
	[RevisionCount] [int] NOT NULL,
	[TermIdentifier] [uniqueidentifier] NOT NULL,
	[GlossaryIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QGlossaryTerm] PRIMARY KEY CLUSTERED 
(
	[TermIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assets].[QGlossaryTermContent](
	[RelationshipIdentifier] [uniqueidentifier] NOT NULL,
	[TermIdentifier] [uniqueidentifier] NOT NULL,
	[ContainerType] [varchar](100) NOT NULL,
	[ContainerIdentifier] [uniqueidentifier] NOT NULL,
	[ContentLabel] [varchar](100) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QGlossaryTermContent] PRIMARY KEY CLUSTERED 
(
	[RelationshipIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assets].[TFileActivity](
	[FileIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[ActivityIdentifier] [uniqueidentifier] NOT NULL,
	[ActivityTime] [datetimeoffset](7) NOT NULL,
	[ActivityChanges] [varchar](max) NOT NULL,
 CONSTRAINT [PK_TFileActivity] PRIMARY KEY CLUSTERED 
(
	[ActivityIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [assets].[TFileClaim](
	[FileIdentifier] [uniqueidentifier] NOT NULL,
	[ClaimIdentifier] [uniqueidentifier] NOT NULL,
	[ObjectType] [varchar](100) NOT NULL,
	[ObjectIdentifier] [uniqueidentifier] NOT NULL,
	[ClaimGranted] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_TFileClaim] PRIMARY KEY CLUSTERED 
(
	[ClaimIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [backups].[Aggregate](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateType] [varchar](100) NOT NULL,
	[AggregateClass] [varchar](200) NOT NULL,
	[AggregateExpires] [datetimeoffset](7) NULL,
	[OriginOrganization] [uniqueidentifier] NOT NULL,
	[AchievementUpgraded] [datetimeoffset](7) NULL,
	[RootAggregateIdentifier] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [backups].[Change](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateVersion] [int] NOT NULL,
	[OriginOrganization] [uniqueidentifier] NOT NULL,
	[OriginUser] [uniqueidentifier] NOT NULL,
	[ChangeTime] [datetimeoffset](7) NOT NULL,
	[ChangeType] [varchar](100) NOT NULL,
	[ChangeData] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [backups].[QPerson](
	[PersonIdentifier] [uniqueidentifier] NOT NULL,
	[Phone] [varchar](30) NULL,
	[PhoneFax] [varchar](32) NULL,
	[PhoneHome] [varchar](32) NULL,
	[PhoneOther] [varchar](32) NULL,
	[PhoneWork] [varchar](32) NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [backups].[QResponseAnswer](
	[ResponseSessionIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyQuestionIdentifier] [uniqueidentifier] NOT NULL,
	[RespondentUserIdentifier] [uniqueidentifier] NOT NULL,
	[ResponseAnswerText] [nvarchar](max) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [backups].[QResponseOption](
	[ResponseSessionIdentifier] [uniqueidentifier] NOT NULL,
	[SurveyOptionIdentifier] [uniqueidentifier] NOT NULL,
	[ResponseOptionIsSelected] [bit] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[SurveyQuestionIdentifier] [uniqueidentifier] NOT NULL,
	[OptionSequence] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [backups].[QUser](
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[PhoneMobile] [varchar](32) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [banks].[QBankOption](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[BankIdentifier] [uniqueidentifier] NOT NULL,
	[SetIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[OptionKey] [int] NOT NULL,
	[CompetencyIdentifier] [uniqueidentifier] NULL,
	[OptionText] [varchar](max) NULL,
 CONSTRAINT [PK_QBankOption] PRIMARY KEY CLUSTERED 
(
	[QuestionIdentifier] ASC,
	[OptionKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [banks].[QBankQuestionAttachment](
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[UploadIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QBankQuestionAttachment] PRIMARY KEY CLUSTERED 
(
	[QuestionIdentifier] ASC,
	[UploadIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [banks].[QBankQuestionSubCompetency](
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[SubCompetencyIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QBankQuestionSubCompetency] PRIMARY KEY CLUSTERED 
(
	[QuestionIdentifier] ASC,
	[SubCompetencyIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [banks].[QBankSpecification](
	[BankIdentifier] [uniqueidentifier] NOT NULL,
	[CalcDisclosure] [varchar](30) NOT NULL,
	[CalcPassingScore] [decimal](3, 2) NOT NULL,
	[SpecAsset] [int] NOT NULL,
	[SpecConsequence] [varchar](10) NOT NULL,
	[SpecFormCount] [int] NOT NULL,
	[SpecFormLimit] [int] NOT NULL,
	[SpecIdentifier] [uniqueidentifier] NOT NULL,
	[SpecName] [varchar](200) NOT NULL,
	[SpecQuestionLimit] [int] NOT NULL,
	[SpecType] [varchar](8) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[CriterionTagCount] [int] NULL,
	[CriterionPivotCount] [int] NULL,
	[CriterionAllCount] [int] NULL,
 CONSTRAINT [PK_QBankSpecification] PRIMARY KEY CLUSTERED 
(
	[SpecIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [billing].[TOrder](
	[OrderIdentifier] [uniqueidentifier] NOT NULL,
	[InvoiceItemIdentifier] [uniqueidentifier] NOT NULL,
	[InvoiceIdentifier] [uniqueidentifier] NOT NULL,
	[CustomerUserIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[OrderCompleted] [datetimeoffset](7) NOT NULL,
	[ProductIdentifier] [uniqueidentifier] NOT NULL,
	[ProductUrl] [varchar](500) NULL,
	[ProductName] [varchar](254) NULL,
	[ProductType] [varchar](30) NULL,
	[DiscountAmount] [decimal](12, 2) NOT NULL,
	[TaxAmount] [decimal](12, 2) NOT NULL,
	[TaxRate] [decimal](5, 4) NOT NULL,
	[TotalAmount] [decimal](12, 2) NOT NULL,
	[ManagerUserIdentifier] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TOrder] PRIMARY KEY CLUSTERED 
(
	[OrderIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [billing].[TOrderItem](
	[OrderItemIdentifier] [uniqueidentifier] NOT NULL,
	[OrderIdentifier] [uniqueidentifier] NOT NULL,
	[ProductIdentifier] [uniqueidentifier] NULL,
	[ProductName] [varchar](254) NULL,
	[OrderItemType] [varchar](30) NULL,
	[OrderItemQuantity] [int] NOT NULL,
	[UnitPrice] [decimal](12, 2) NOT NULL,
	[LineTotalAmount]  AS (isnull(CONVERT([decimal](12,2),[OrderItemQuantity]*[UnitPrice]),(0))) PERSISTED NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_TOrderItem] PRIMARY KEY CLUSTERED 
(
	[OrderItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [billing].[TTax](
	[TaxIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[CountryCode] [varchar](2) NOT NULL,
	[RegionCode] [varchar](10) NOT NULL,
	[TaxName] [varchar](20) NOT NULL,
	[TaxRate] [decimal](5, 4) NOT NULL,
 CONSTRAINT [PK_TTax] PRIMARY KEY CLUSTERED 
(
	[TaxIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [communications].[QCarbonCopy](
	[CarbonCopyIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[RecipientIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QCarbonCopy] PRIMARY KEY CLUSTERED 
(
	[CarbonCopyIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QGroupTag](
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[GroupTag] [varchar](100) NOT NULL,
	[TagIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QGroupTag] PRIMARY KEY CLUSTERED 
(
	[TagIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QMembershipDeletion](
	[DeletionIdentifier] [uniqueidentifier] NOT NULL,
	[DeletionWhen] [datetimeoffset](7) NOT NULL,
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[MembershipIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QMembershipDeletion] PRIMARY KEY CLUSTERED 
(
	[DeletionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QMembershipReason](
	[MembershipIdentifier] [uniqueidentifier] NOT NULL,
	[ReasonIdentifier] [uniqueidentifier] NOT NULL,
	[ReasonType] [varchar](30) NOT NULL,
	[ReasonSubtype] [varchar](30) NULL,
	[ReasonEffective] [datetimeoffset](7) NOT NULL,
	[ReasonExpiry] [datetimeoffset](7) NULL,
	[PersonOccupation] [varchar](100) NULL,
	[LastChangeType] [varchar](100) NOT NULL,
	[Created] [datetimeoffset](7) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[Modified] [datetimeoffset](7) NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QMembershipReason] PRIMARY KEY CLUSTERED 
(
	[ReasonIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[QPersonSecret](
	[PersonIdentifier] [uniqueidentifier] NOT NULL,
	[SecretIdentifier] [uniqueidentifier] NOT NULL,
	[SecretType] [varchar](30) NOT NULL,
	[SecretName] [varchar](100) NOT NULL,
	[SecretExpiry] [datetimeoffset](7) NOT NULL,
	[SecretLifetimeLimit] [int] NULL,
	[SecretValue] [varchar](100) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [contacts].[TPersonField](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[FieldIdentifier] [uniqueidentifier] NOT NULL,
	[FieldName] [varchar](100) NOT NULL,
	[FieldValue] [nvarchar](max) NOT NULL,
	[FieldSequence] [int] NULL,
 CONSTRAINT [PK_TPersonField] PRIMARY KEY CLUSTERED 
(
	[FieldIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [courses].[QCoursePrerequisite](
	[CoursePrerequisiteIdentifier] [uniqueidentifier] NOT NULL,
	[CourseIdentifier] [uniqueidentifier] NOT NULL,
	[ObjectIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ObjectType] [varchar](100) NOT NULL,
	[TriggerChange] [varchar](30) NOT NULL,
	[TriggerConditionScoreFrom] [int] NULL,
	[TriggerConditionScoreThru] [int] NULL,
	[TriggerIdentifier] [uniqueidentifier] NOT NULL,
	[TriggerType] [varchar](30) NOT NULL,
 CONSTRAINT [PK_QCoursePrerequisite] PRIMARY KEY CLUSTERED 
(
	[CoursePrerequisiteIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [courses].[TCourseDistribution](
	[CourseDistributionIdentifier] [uniqueidentifier] NOT NULL,
	[ProductIdentifier] [uniqueidentifier] NOT NULL,
	[CourseIdentifier] [uniqueidentifier] NULL,
	[ManagerUserIdentifier] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[CourseEnrollmentIdentifier] [uniqueidentifier] NULL,
	[DistributionAssigned] [datetimeoffset](7) NOT NULL,
	[DistributionStatus] [varchar](20) NOT NULL,
	[DistributionRedeemed] [datetimeoffset](7) NULL,
	[DistributionExpiry] [datetimeoffset](7) NULL,
	[DistributionComment] [nvarchar](500) NULL,
	[EventIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_TCourseDistribution] PRIMARY KEY CLUSTERED 
(
	[CourseDistributionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [courses].[TLtiLink](
	[LinkIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[AssetNumber] [int] NOT NULL,
	[ToolProviderType] [varchar](20) NOT NULL,
	[ToolProviderName] [varchar](20) NOT NULL,
	[ToolProviderUrl] [varchar](500) NOT NULL,
	[ToolConsumerKey] [varchar](20) NOT NULL,
	[ToolConsumerSecret] [varchar](20) NOT NULL,
	[ResourceCode] [varchar](20) NULL,
	[ResourceName] [varchar](100) NULL,
	[ResourceSummary] [varchar](100) NULL,
	[ResourceTitle] [varchar](100) NOT NULL,
	[ResourceParameters] [varchar](max) NULL,
 CONSTRAINT [PK_TLtiLink] PRIMARY KEY CLUSTERED 
(
	[LinkIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[QCmdsInvoice](
	[InvoiceKey] [int] IDENTITY(1,1) NOT NULL,
	[Currency] [varchar](50) NULL,
	[DateSubmitted] [date] NULL,
	[InvoiceNumber] [varchar](32) NOT NULL,
	[PeriodYear] [int] NOT NULL,
	[PeriodMonth] [int] NOT NULL,
	[TaxAmount] [decimal](9, 2) NOT NULL,
	[TaxRate] [decimal](9, 2) NOT NULL,
	[TaxType] [varchar](4) NULL,
	[TotalAmount] [decimal](9, 2) NOT NULL,
	[TotalAmountTaxable] [decimal](9, 2) NOT NULL,
	[CompanyName] [varchar](148) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QCmdsInvoice] PRIMARY KEY CLUSTERED 
(
	[InvoiceKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[QCmdsInvoiceFee](
	[InvoiceFeeKey] [int] IDENTITY(1,1) NOT NULL,
	[FromDate] [date] NOT NULL,
	[ThruDate] [date] NOT NULL,
	[BillingClassification] [varchar](1) NOT NULL,
	[PricePerUserPerPeriodPerCompany] [decimal](11, 2) NULL,
	[SharedCompanyCount] [int] NOT NULL,
	[CompanyName] [varchar](148) NULL,
	[UserEmail] [varchar](254) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QCmdsInvoiceFee] PRIMARY KEY CLUSTERED 
(
	[InvoiceFeeKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[QCmdsInvoiceItem](
	[InvoiceItemKey] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceKey] [int] NOT NULL,
	[Description] [varchar](max) NULL,
	[Quantity] [decimal](7, 2) NOT NULL,
	[Sequence] [int] NULL,
	[UnitName] [varchar](32) NOT NULL,
	[UnitPrice] [decimal](11, 2) NULL,
	[IsTaxable] [bit] NOT NULL,
	[MultiplierQuantity] [int] NOT NULL,
	[MultiplierUnitName] [varchar](32) NOT NULL,
	[Category] [varchar](120) NULL,
	[SubCategory] [varchar](120) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QCmdsInvoiceItem] PRIMARY KEY CLUSTERED 
(
	[InvoiceItemKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[QUserStatus](
	[UserStatusKey] [int] IDENTITY(1,1) NOT NULL,
	[AsAt] [datetime] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationName] [varchar](256) NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[DepartmentName] [varchar](256) NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[UserFullName] [varchar](128) NOT NULL,
	[PrimaryProfileIdentifier] [uniqueidentifier] NULL,
	[PrimaryProfileNumber] [varchar](50) NULL,
	[PrimaryProfileTitle] [varchar](256) NULL,
	[ItemNumber] [int] NOT NULL,
	[ItemName] [varchar](64) NOT NULL,
	[CountRQ_Primary] [int] NULL,
	[CountVAVNCP_Primary] [int] NULL,
	[Score_Primary] [decimal](5, 2) NULL,
	[CountEX_Primary] [int] NULL,
	[CountNC_Primary] [int] NULL,
	[CountNA_Primary] [int] NULL,
	[CountNT_Primary] [int] NULL,
	[CountSA_Primary] [int] NULL,
	[CountSV_Primary] [int] NULL,
	[CountVA_Primary] [int] NULL,
	[CountRQ_Mandatory] [int] NULL,
	[CountVAVNCP_Mandatory] [int] NULL,
	[CountEX_Mandatory] [int] NULL,
	[CountNC_Mandatory] [int] NULL,
	[CountNA_Mandatory] [int] NULL,
	[CountNT_Mandatory] [int] NULL,
	[CountSA_Mandatory] [int] NULL,
	[CountSV_Mandatory] [int] NULL,
	[CountVAVN_Mandatory] [int] NULL,
	[Score_Mandatory] [decimal](5, 2) NULL,
	[CountEX] [int] NULL,
	[CountNC] [int] NULL,
	[CountNA] [int] NULL,
	[CountNT] [int] NULL,
	[CountSA] [int] NULL,
	[CountSV] [int] NULL,
	[CountVA] [int] NULL,
	[CountRQ] [int] NULL,
	[CountVAVNCP] [int] NULL,
	[Score] [decimal](5, 2) NULL,
 CONSTRAINT [PK_QUserStatus] PRIMARY KEY CLUSTERED 
(
	[UserStatusKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[TUserStatus](
	[UserName] [varchar](100) NULL,
	[ItemNumber] [int] NOT NULL,
	[ItemName] [varchar](50) NOT NULL,
	[ListDomain] [varchar](10) NOT NULL,
	[CountCP] [int] NOT NULL,
	[CountEX] [int] NOT NULL,
	[CountNC] [int] NOT NULL,
	[CountNA] [int] NOT NULL,
	[CountNT] [int] NOT NULL,
	[CountSA] [int] NOT NULL,
	[CountSV] [int] NOT NULL,
	[CountVA] [int] NOT NULL,
	[CountRQ] [int] NOT NULL,
	[Score] [decimal](3, 2) NULL,
	[OrganizationName] [varchar](100) NULL,
	[DepartmentName] [varchar](100) NULL,
	[AsAt] [datetimeoffset](7) NOT NULL,
	[CountVN] [int] NOT NULL,
	[DepartmentRole] [varchar](20) NULL,
	[Progress] [decimal](3, 2) NULL,
	[TagNecessity] [varchar](10) NULL,
	[TagCriticality] [varchar](20) NULL,
	[TagPrimacy] [varchar](10) NULL,
	[ListFolder] [varchar](100) NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TUserStatus] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_cmds].[ZUserStatusSummary](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[DepartmentIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[PrimaryProfileIdentifier] [uniqueidentifier] NULL,
	[SnapshotDate] [date] NULL,
	[Sequence] [int] NOT NULL,
	[Heading] [varchar](50) NOT NULL,
	[PrimaryNotApplicable] [int] NULL,
	[PrimaryRequired] [int] NULL,
	[PrimarySatisfied] [int] NULL,
	[PrimaryScore] [decimal](7, 4) NULL,
	[PrimarySubmitted] [int] NULL,
	[ComplianceNotApplicable] [int] NULL,
	[ComplianceRequired] [int] NULL,
	[ComplianceSatisfied] [int] NULL,
	[ComplianceScore] [decimal](7, 4) NULL,
	[ComplianceSubmitted] [int] NULL,
	[SummaryIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ZUserStatusSummary] PRIMARY KEY NONCLUSTERED 
(
	[SummaryIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ita].[ExamDistributionRequest](
	[RequestIdentifier] [uniqueidentifier] NOT NULL,
	[RequestedBy] [uniqueidentifier] NOT NULL,
	[Requested] [datetimeoffset](7) NOT NULL,
	[JobCode] [varchar](100) NOT NULL,
	[JobStatus] [varchar](100) NULL,
	[JobErrors] [varchar](max) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ExamDistributionRequest] PRIMARY KEY CLUSTERED 
(
	[RequestIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ita].[Individual](
	[IndividualKey] [int] NOT NULL,
	[CrmIdentifier] [uniqueidentifier] NULL,
	[Birthdate] [date] NULL,
	[Gender] [varchar](20) NULL,
	[Name] [varchar](100) NOT NULL,
	[FirstName] [varchar](30) NULL,
	[MiddleName] [varchar](50) NULL,
	[LastName] [varchar](30) NULL,
	[Mobile] [varchar](15) NULL,
	[Email] [varchar](254) NULL,
	[PersonalEducationNumber] [int] NULL,
	[Phone] [varchar](15) NULL,
	[HashCode] [varchar](32) NOT NULL,
	[AddressLine1] [varchar](100) NULL,
	[AddressLine2] [varchar](100) NULL,
	[AddressCity] [varchar](100) NULL,
	[AddressProvince] [varchar](100) NULL,
	[AddressPostalCode] [varchar](100) NULL,
	[AboriginalIndicator] [varchar](100) NULL,
	[AboriginalIdentity] [varchar](100) NULL,
	[IsNew] [bit] NOT NULL,
	[Refreshed] [datetimeoffset](7) NOT NULL,
	[RefreshedBy] [uniqueidentifier] NOT NULL,
	[ContactIdentifier] [uniqueidentifier] NULL,
	[ProgramType] [varchar](100) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[IsActive] [varchar](1) NULL,
	[IsDeceased] [varchar](1) NULL,
	[IsMerged] [varchar](1) NULL,
 CONSTRAINT [PK_Individual] PRIMARY KEY CLUSTERED 
(
	[IndividualKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[Counter](
	[Category] [varchar](120) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Name] [varchar](310) NOT NULL,
	[Scope] [varchar](40) NOT NULL,
	[Year] [int] NOT NULL,
	[Value] [decimal](16, 2) NOT NULL,
	[Unit] [varchar](20) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[CounterIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Counter] PRIMARY KEY CLUSTERED 
(
	[CounterIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[Field](
	[Category] [varchar](130) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Name] [varchar](310) NOT NULL,
	[Unit] [varchar](20) NOT NULL,
	[IsNumeric] [bit] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Field] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[Filter](
	[FilterID] [uniqueidentifier] NOT NULL,
	[AuthorUserIdentifier] [uniqueidentifier] NOT NULL,
	[FilterName] [varchar](80) NOT NULL,
	[FilterData] [varchar](max) NOT NULL,
	[DateSaved] [datetimeoffset](7) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Filter] PRIMARY KEY NONCLUSTERED 
(
	[FilterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[History](
	[RecordID] [uniqueidentifier] NOT NULL,
	[RecordTime] [datetimeoffset](7) NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[UserName] [varchar](40) NOT NULL,
	[UserEmail] [varchar](254) NOT NULL,
	[EventType] [varchar](70) NOT NULL,
	[EventData] [nvarchar](max) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_History] PRIMARY KEY NONCLUSTERED 
(
	[RecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [IX_History_01] ON [custom_ncsha].[History]
(
	[RecordTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[ProgramFolder](
	[ProgramFolderID] [int] IDENTITY(1,1) NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ProgramFolder] PRIMARY KEY CLUSTERED 
(
	[ProgramFolderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[ProgramFolderComment](
	[ProgramFolderID] [int] NOT NULL,
	[CommentID] [int] NOT NULL,
	[CommentIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ProgramFolderComment] PRIMARY KEY CLUSTERED 
(
	[CommentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [custom_ncsha].[TReportMapping](
	[MappingIdentifier] [uniqueidentifier] NOT NULL,
	[ReportTable] [varchar](10) NOT NULL,
	[ReportColumn] [varchar](10) NOT NULL,
	[SurveyIdentifier] [uniqueidentifier] NOT NULL,
	[QuestionIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[ReportColumnOther] [varchar](10) NULL,
 CONSTRAINT [PK_TReportMapping] PRIMARY KEY CLUSTERED 
(
	[MappingIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_TReportMapping_Unique] UNIQUE NONCLUSTERED 
(
	[ReportTable] ASC,
	[ReportColumn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [events].[QEventTimer](
	[EventIdentifier] [uniqueidentifier] NOT NULL,
	[TimerDescription] [varchar](200) NULL,
	[TimerStatus] [varchar](20) NOT NULL,
	[TriggerCommand] [uniqueidentifier] NOT NULL,
	[TriggerTime] [datetimeoffset](7) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QEventTimer] PRIMARY KEY CLUSTERED 
(
	[TriggerCommand] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [integration].[TMoodleEvent](
	[ActivityIdentifier] [uniqueidentifier] NOT NULL,
	[CourseIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[EventIdentifier] [uniqueidentifier] NOT NULL,
	[EventData] [varchar](max) NOT NULL,
	[EventWhen] [datetimeoffset](7) NOT NULL,
	[Action] [varchar](255) NOT NULL,
	[Anonymous] [bit] NOT NULL,
	[CallbackUrl] [varchar](500) NOT NULL,
	[Component] [varchar](255) NOT NULL,
	[ContextId] [int] NOT NULL,
	[ContextInstanceId] [varchar](255) NOT NULL,
	[ContextLevel] [int] NOT NULL,
	[CourseId] [varchar](255) NOT NULL,
	[Crud] [varchar](1) NOT NULL,
	[EduLevel] [int] NOT NULL,
	[EventName] [varchar](255) NOT NULL,
	[IdNumber] [varchar](255) NULL,
	[ObjectId] [varchar](255) NOT NULL,
	[ObjectTable] [varchar](255) NOT NULL,
	[RelatedUserId] [varchar](255) NULL,
	[ShortName] [varchar](255) NOT NULL,
	[Target] [varchar](255) NOT NULL,
	[TimeCreated] [bigint] NOT NULL,
	[Token] [varchar](1024) NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[UserId] [varchar](255) NOT NULL,
	[OtherInstanceId] [varchar](255) NULL,
	[OtherLoadedContent] [varchar](255) NULL,
	[OtherAttemptId] [varchar](255) NULL,
	[OtherCmiElement] [varchar](255) NULL,
	[OtherCmiValue] [varchar](255) NULL,
	[OtherFinalGrade] [int] NULL,
	[OtherItemId] [varchar](255) NULL,
	[OtherOverridden] [bit] NULL,
 CONSTRAINT [PK_TMoodleEvent] PRIMARY KEY CLUSTERED 
(
	[EventIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [integration].[TScormEvent](
	[EventIdentifier] [uniqueidentifier] NOT NULL,
	[EventWhen] [datetimeoffset](7) NOT NULL,
	[EventData] [varchar](max) NOT NULL,
 CONSTRAINT [PK_TScormEvent] PRIMARY KEY CLUSTERED 
(
	[EventIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [integration].[TScormRegistration](
	[LearnerIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerName] [varchar](100) NULL,
	[LearnerEmail] [varchar](254) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ScormPackageHook] [varchar](100) NOT NULL,
	[ScormRegistrationIdentifier] [uniqueidentifier] NOT NULL,
	[ScormRegistrationCompletion] [varchar](20) NULL,
	[ScormRegistrationSuccess] [varchar](20) NULL,
	[ScormRegistrationScore] [decimal](5, 4) NULL,
	[ScormRegistrationTrackedSeconds] [int] NULL,
	[ScormRegistrationInstance] [int] NULL,
	[ScormLaunchCount] [int] NOT NULL,
	[ScormLaunchedFirst] [datetimeoffset](7) NOT NULL,
	[ScormLaunchedLast] [datetimeoffset](7) NOT NULL,
	[ScormAccessedFirst] [datetimeoffset](7) NULL,
	[ScormAccessedLast] [datetimeoffset](7) NULL,
	[ScormCompleted] [datetimeoffset](7) NULL,
	[ScormSynchronized] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_TScormRegistration] PRIMARY KEY CLUSTERED 
(
	[ScormRegistrationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [integration].[TScormRegistrationActivity](
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
	[ScormRegistrationIdentifier] [uniqueidentifier] NOT NULL,
	[ActivityIdentifier] [uniqueidentifier] NOT NULL,
	[CourseIdentifier] [uniqueidentifier] NOT NULL,
	[GradeItemIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TScormRegistrationActivity] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [integration].[TScormStatement](
	[ActorName] [varchar](100) NULL,
	[ObjectDefinitionName] [varchar](100) NULL,
	[RegistrationIdentifier] [uniqueidentifier] NOT NULL,
	[StatementData] [varchar](max) NOT NULL,
	[StatementIdentifier] [uniqueidentifier] NOT NULL,
	[StatementTimestamp] [datetimeoffset](7) NULL,
	[VerbDisplay] [varchar](100) NULL,
 CONSTRAINT [PK_TScormStatement] PRIMARY KEY CLUSTERED 
(
	[StatementIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [issues].[QIssueGroup](
	[IssueIdentifier] [uniqueidentifier] NOT NULL,
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[IssueRole] [varchar](20) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[JoinIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QIssueGroup] PRIMARY KEY CLUSTERED 
(
	[JoinIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [logs].[Aggregate](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateType] [varchar](100) NOT NULL,
	[AggregateClass] [varchar](200) NOT NULL,
	[AggregateExpires] [datetimeoffset](7) NULL,
	[OriginOrganization] [uniqueidentifier] NOT NULL,
	[AchievementUpgraded] [datetimeoffset](7) NULL,
	[RootAggregateIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Aggregate] PRIMARY KEY CLUSTERED 
(
	[AggregateIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [logs].[AggregateBuffer](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateType] [varchar](100) NOT NULL,
	[AggregateClass] [varchar](200) NOT NULL,
	[AggregateExpires] [datetimeoffset](7) NULL,
	[OriginOrganization] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AggregateBuffer] PRIMARY KEY NONCLUSTERED 
(
	[AggregateIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [logs].[BAggregateSerializationTest](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateType] [varchar](100) NOT NULL,
	[AggregateData] [nvarchar](max) NULL,
	[DataFailure] [bit] NULL,
	[AggregateSnapshot] [nvarchar](max) NULL,
	[SnapshotFailure] [bit] NULL,
 CONSTRAINT [PK_BAggregateSerializationTest] PRIMARY KEY CLUSTERED 
(
	[AggregateIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [logs].[Change](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateVersion] [int] NOT NULL,
	[OriginOrganization] [uniqueidentifier] NOT NULL,
	[OriginUser] [uniqueidentifier] NOT NULL,
	[ChangeTime] [datetimeoffset](7) NOT NULL,
	[ChangeType] [varchar](100) NOT NULL,
	[ChangeData] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Change] PRIMARY KEY CLUSTERED 
(
	[AggregateIdentifier] ASC,
	[AggregateVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [logs].[ChangeBuffer](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateVersion] [int] NOT NULL,
	[OriginOrganization] [uniqueidentifier] NOT NULL,
	[OriginUser] [uniqueidentifier] NOT NULL,
	[ChangeTime] [datetimeoffset](7) NOT NULL,
	[ChangeType] [varchar](100) NOT NULL,
	[ChangeData] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ChangeBuffer] PRIMARY KEY NONCLUSTERED 
(
	[AggregateIdentifier] ASC,
	[AggregateVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [logs].[Command](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[ExpectedVersion] [int] NULL,
	[OriginOrganization] [uniqueidentifier] NOT NULL,
	[OriginUser] [uniqueidentifier] NOT NULL,
	[CommandClass] [varchar](200) NOT NULL,
	[CommandType] [varchar](100) NOT NULL,
	[CommandData] [nvarchar](max) NOT NULL,
	[CommandIdentifier] [uniqueidentifier] NOT NULL,
	[SendStatus] [varchar](20) NULL,
	[SendError] [varchar](max) NULL,
	[SendScheduled] [datetimeoffset](7) NULL,
	[SendStarted] [datetimeoffset](7) NULL,
	[SendCompleted] [datetimeoffset](7) NULL,
	[SendCancelled] [datetimeoffset](7) NULL,
	[BookmarkAdded] [datetimeoffset](7) NULL,
	[BookmarkExpired] [datetimeoffset](7) NULL,
	[RecurrenceInterval] [int] NULL,
	[RecurrenceUnit] [varchar](10) NULL,
	[RecurrenceWeekdays] [varchar](27) NULL,
	[CommandDescription] [varchar](max) NULL,
 CONSTRAINT [PK_Command] PRIMARY KEY CLUSTERED 
(
	[CommandIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [logs].[Snapshot](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateVersion] [int] NOT NULL,
	[AggregateState] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Snapshot] PRIMARY KEY CLUSTERED 
(
	[AggregateIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [metadata].[TEntity](
	[StorageStructure] [varchar](20) NOT NULL,
	[StorageSchema] [varchar](30) NOT NULL,
	[StorageTable] [varchar](40) NOT NULL,
	[StorageTableRename] [varchar](40) NULL,
	[StorageKey] [varchar](80) NOT NULL,
	[ComponentType] [varchar](20) NOT NULL,
	[ComponentName] [varchar](30) NOT NULL,
	[ComponentPart] [varchar](40) NOT NULL,
	[EntityName] [varchar](50) NOT NULL,
	[CollectionSlug] [varchar](50) NOT NULL,
	[CollectionKey] [varchar](60) NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TEntity] PRIMARY KEY CLUSTERED 
(
	[EntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [payments].[TDiscount](
	[DiscountCode] [varchar](20) NOT NULL,
	[DiscountPercent] [decimal](5, 2) NOT NULL,
	[DiscountDescription] [varchar](max) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[DiscountIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TDiscount] PRIMARY KEY CLUSTERED 
(
	[DiscountIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QAchievementPrerequisite](
	[AchievementIdentifier] [uniqueidentifier] NOT NULL,
	[PrerequisiteIdentifier] [uniqueidentifier] NOT NULL,
	[PrerequisiteAchievementIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QAchievementPrerequisite] PRIMARY KEY CLUSTERED 
(
	[PrerequisiteIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QAreaRequirement](
	[JournalSetupIdentifier] [uniqueidentifier] NOT NULL,
	[AreaStandardIdentifier] [uniqueidentifier] NOT NULL,
	[AreaHours] [decimal](20, 2) NULL,
 CONSTRAINT [PK_QAreaRequirement] PRIMARY KEY NONCLUSTERED 
(
	[JournalSetupIdentifier] ASC,
	[AreaStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QCompetencyRequirement](
	[JournalSetupIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyStandardIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyHours] [decimal](20, 2) NULL,
	[JournalItems] [int] NULL,
	[SkillRating] [int] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[IncludeHoursToArea] [bit] NOT NULL,
 CONSTRAINT [PK_QCompetencyRequirement] PRIMARY KEY CLUSTERED 
(
	[JournalSetupIdentifier] ASC,
	[CompetencyStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QCredentialHistory](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateVersion] [int] NOT NULL,
	[AchievementIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[ChangeTime] [datetimeoffset](7) NOT NULL,
	[ChangeBy] [uniqueidentifier] NOT NULL,
	[CredentialActionDate] [datetimeoffset](7) NULL,
	[CredentialActionType] [varchar](50) NOT NULL,
	[CredentialScore] [decimal](5, 4) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QCredentialHistory] PRIMARY KEY CLUSTERED 
(
	[AggregateIdentifier] ASC,
	[AggregateVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QEnrollmentHistory](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateVersion] [int] NOT NULL,
	[ChangeTime] [datetimeoffset](7) NOT NULL,
	[ChangeBy] [uniqueidentifier] NOT NULL,
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerIdentifier] [uniqueidentifier] NOT NULL,
	[EnrollmentIdentifier] [uniqueidentifier] NOT NULL,
	[EnrollmentType] [varchar](50) NOT NULL,
	[EnrollmentTime] [datetimeoffset](7) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QEnrollmentHistory] PRIMARY KEY CLUSTERED 
(
	[AggregateIdentifier] ASC,
	[AggregateVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QExperienceCompetency](
	[ExperienceIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyStandardIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyHours] [decimal](20, 2) NULL,
	[SkillRating] [int] NULL,
	[SatisfactionLevel] [varchar](50) NULL,
 CONSTRAINT [PK_QExperienceCompetency] PRIMARY KEY CLUSTERED 
(
	[ExperienceIdentifier] ASC,
	[CompetencyStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QGradebookCompetencyValidation](
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyIdentifier] [uniqueidentifier] NOT NULL,
	[ValidationPoints] [decimal](28, 4) NULL,
	[ValidationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QGradebookCompetencyValidation] PRIMARY KEY CLUSTERED 
(
	[ValidationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QGradebookEvent](
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[EventIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QGradebookEvent] PRIMARY KEY CLUSTERED 
(
	[GradebookIdentifier] ASC,
	[EventIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QGradeItemCompetency](
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[GradeItemIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QGradeItemCompetency] PRIMARY KEY CLUSTERED 
(
	[GradebookIdentifier] ASC,
	[GradeItemIdentifier] ASC,
	[CompetencyIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QJournalSetupField](
	[JournalSetupFieldIdentifier] [uniqueidentifier] NOT NULL,
	[JournalSetupIdentifier] [uniqueidentifier] NOT NULL,
	[FieldType] [varchar](50) NOT NULL,
	[FieldIsRequired] [bit] NOT NULL,
	[Sequence] [int] NOT NULL,
 CONSTRAINT [PK_QJournalSetupField] PRIMARY KEY CLUSTERED 
(
	[JournalSetupFieldIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QJournalSetupGroup](
	[JournalSetupIdentifier] [uniqueidentifier] NOT NULL,
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_QJournalSetupGroup] PRIMARY KEY CLUSTERED 
(
	[JournalSetupIdentifier] ASC,
	[GroupIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QProgressHistory](
	[AggregateIdentifier] [uniqueidentifier] NOT NULL,
	[AggregateVersion] [int] NOT NULL,
	[ChangeTime] [datetimeoffset](7) NOT NULL,
	[ChangeBy] [uniqueidentifier] NOT NULL,
	[GradebookIdentifier] [uniqueidentifier] NOT NULL,
	[GradeItemIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[ProgressType] [varchar](50) NOT NULL,
	[ProgressTime] [datetimeoffset](7) NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QProgressHistory] PRIMARY KEY CLUSTERED 
(
	[AggregateIdentifier] ASC,
	[AggregateVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QRubric](
	[RubricIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[RubricTitle] [varchar](100) NOT NULL,
	[RubricDescription] [varchar](800) NULL,
	[RubricPoints] [decimal](5, 2) NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QRubric] PRIMARY KEY CLUSTERED 
(
	[RubricIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QRubricCriterion](
	[RubricIdentifier] [uniqueidentifier] NOT NULL,
	[RubricCriterionIdentifier] [uniqueidentifier] NOT NULL,
	[CriterionTitle] [varchar](100) NOT NULL,
	[CriterionDescription] [varchar](1700) NULL,
	[CriterionPoints] [decimal](5, 2) NOT NULL,
	[CriterionSequence] [int] NOT NULL,
	[IsRange] [bit] NOT NULL,
 CONSTRAINT [PK_QRubricCriterion] PRIMARY KEY CLUSTERED 
(
	[RubricCriterionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[QRubricRating](
	[RubricCriterionIdentifier] [uniqueidentifier] NOT NULL,
	[RubricRatingIdentifier] [uniqueidentifier] NOT NULL,
	[RatingTitle] [varchar](100) NOT NULL,
	[RatingDescription] [varchar](800) NULL,
	[RatingPoints] [decimal](5, 2) NOT NULL,
	[RatingSequence] [int] NOT NULL,
 CONSTRAINT [PK_QRubricRating] PRIMARY KEY CLUSTERED 
(
	[RubricRatingIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[TCertificateLayout](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[CertificateLayoutCode] [varchar](100) NOT NULL,
	[CertificateLayoutData] [varchar](1200) NOT NULL,
	[CertificateLayoutIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TCertificateLayout] PRIMARY KEY CLUSTERED 
(
	[CertificateLayoutIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[TPrerequisite](
	[ObjectIdentifier] [uniqueidentifier] NOT NULL,
	[ObjectType] [varchar](100) NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[PrerequisiteIdentifier] [uniqueidentifier] NOT NULL,
	[TriggerChange] [varchar](30) NOT NULL,
	[TriggerConditionScoreFrom] [int] NULL,
	[TriggerConditionScoreThru] [int] NULL,
	[TriggerIdentifier] [uniqueidentifier] NOT NULL,
	[TriggerType] [varchar](30) NOT NULL,
 CONSTRAINT [PK_TPrerequisite] PRIMARY KEY CLUSTERED 
(
	[PrerequisiteIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[TProgramGroupEnrollment](
	[ProgramGroupEnrollmentIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[GroupIdentifier] [uniqueidentifier] NOT NULL,
	[ProgramIdentifier] [uniqueidentifier] NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TProgramGroupEnrollment] PRIMARY KEY CLUSTERED 
(
	[ProgramGroupEnrollmentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [records].[TTaskEnrollment](
	[EnrollmentIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerUserIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[TaskIdentifier] [uniqueidentifier] NOT NULL,
	[ObjectIdentifier] [uniqueidentifier] NOT NULL,
	[ProgressStarted] [datetimeoffset](7) NULL,
	[ProgressCompleted] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_TTaskEnrollment] PRIMARY KEY CLUSTERED 
(
	[EnrollmentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [registrations].[QRegistrationInstructor](
	[RegistrationIdentifier] [uniqueidentifier] NOT NULL,
	[InstructorIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QRegistrationInstructor] PRIMARY KEY CLUSTERED 
(
	[RegistrationIdentifier] ASC,
	[InstructorIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [reports].[ApiRequest](
	[DeveloperName] [varchar](100) NULL,
	[DeveloperEmail] [varchar](254) NULL,
	[DeveloperHostAddress] [varchar](15) NULL,
	[RequestStarted] [datetimeoffset](7) NOT NULL,
	[RequestMethod] [varchar](6) NOT NULL,
	[RequestUri] [varchar](500) NOT NULL,
	[RequestContentType] [varchar](100) NULL,
	[RequestContentData] [varchar](max) NULL,
	[RequestHeaders] [varchar](5000) NULL,
	[RequestStatus] [varchar](16) NOT NULL,
	[ValidationStatus] [varchar](16) NOT NULL,
	[ValidationErrors] [varchar](max) NULL,
	[ResponseCompleted] [datetimeoffset](7) NULL,
	[ResponseContentType] [varchar](100) NULL,
	[ResponseContentData] [nvarchar](max) NULL,
	[ResponseStatusName] [varchar](40) NULL,
	[ResponseStatusNumber] [int] NULL,
	[ResponseTime] [int] NULL,
	[ExecutionErrors] [varchar](500) NULL,
	[UserIdentifier] [uniqueidentifier] NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
	[RequestDirection] [varchar](3) NOT NULL,
	[RequestPath] [varchar](200) NULL,
	[RequestIdentifier] [uniqueidentifier] NOT NULL,
	[ResponseLogIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ApiRequest] PRIMARY KEY CLUSTERED 
(
	[RequestIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [reports].[QLearnerProgramSummary](
	[AsAt] [datetimeoffset](7) NOT NULL,
	[SummaryIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[LearnerAccountCreated] [datetimeoffset](7) NULL,
	[LearnerAccessGranted] [datetimeoffset](7) NULL,
	[LearnerAddedToProgram] [datetimeoffset](7) NULL,
	[ImmigrationArrivalDate] [date] NULL,
	[ImmigrationArrivalStatus] [varchar](4) NULL,
	[LearnerEmail] [varchar](254) NOT NULL,
	[LearnerName] [varchar](120) NOT NULL,
	[LearnerGender] [varchar](100) NULL,
	[ReferrerName] [varchar](100) NULL,
	[ReferrerNameOther] [varchar](200) NULL,
	[ReferrerProvince] [nvarchar](max) NULL,
	[ReferrerRole] [varchar](100) NULL,
	[ReferrerIndustry] [nvarchar](max) NULL,
	[LearnerStreams] [varchar](200) NULL,
	[ProgramName] [varchar](100) NULL,
	[ProgramStatus] [varchar](20) NULL,
	[ProgramGradeItems] [int] NULL,
	[ProgramGradeItemsCompleted] [int] NULL,
 CONSTRAINT [PK_QLearnerProgramSummary] PRIMARY KEY CLUSTERED 
(
	[SummaryIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [reports].[TMeasurement](
	[MeasurementIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[ContainerIdentifier] [uniqueidentifier] NULL,
	[ContainerType] [varchar](100) NULL,
	[ContainerName] [varchar](100) NULL,
	[AsAt] [datetimeoffset](7) NOT NULL,
	[AsAtDate] [date] NOT NULL,
	[AsAtYear] [int] NOT NULL,
	[AsAtMonth] [int] NOT NULL,
	[AsAtDay] [int] NOT NULL,
	[AsAtWeek] [int] NOT NULL,
	[AsAtQuarter] [int] NOT NULL,
	[IntervalType] [varchar](10) NOT NULL,
	[VariableRoot] [varchar](100) NULL,
	[VariableList] [varchar](100) NULL,
	[VariableItem] [varchar](300) NOT NULL,
	[QuantityValue] [decimal](16, 2) NOT NULL,
	[QuantityValueText] [varchar](100) NULL,
	[QuantityUnit] [varchar](20) NOT NULL,
	[QuantityType] [varchar](10) NULL,
	[QuantityFunction] [varchar](20) NOT NULL,
	[QuantityDelta] [decimal](16, 2) NULL,
	[QuantityDeltaText] [varchar](max) NULL,
	[UniquePath] [varchar](704) NOT NULL,
	[UniquePathHash] [int] NOT NULL,
 CONSTRAINT [PK_TMeasurement] PRIMARY KEY CLUSTERED 
(
	[MeasurementIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [resources].[UploadRelation](
	[ContainerIdentifier] [uniqueidentifier] NOT NULL,
	[ContainerType] [varchar](32) NOT NULL,
	[UploadIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_UploadRelation] PRIMARY KEY CLUSTERED 
(
	[ContainerIdentifier] ASC,
	[UploadIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [security].[TPartitionSetting](
	[SettingIdentifier] [uniqueidentifier] NOT NULL,
	[SettingName] [varchar](100) NOT NULL,
	[SettingValue] [varchar](500) NOT NULL,
 CONSTRAINT [PK_TPartitionSetting] PRIMARY KEY CLUSTERED 
(
	[SettingIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [settings].[TSequence](
	[OrganizationIdentifier] [uniqueidentifier] NOT NULL,
	[SequenceType] [varchar](30) NOT NULL,
	[SequenceNumber] [int] NOT NULL,
 CONSTRAINT [PK_TSequence] PRIMARY KEY CLUSTERED 
(
	[OrganizationIdentifier] ASC,
	[SequenceType] ASC,
	[SequenceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standard].[QStandardTier](
	[RootStandardIdentifier] [uniqueidentifier] NOT NULL,
	[ItemStandardIdentifier] [uniqueidentifier] NOT NULL,
	[TierNumber] [int] NOT NULL,
	[TierName] [varchar](12) NOT NULL,
 CONSTRAINT [PK_QStandardTier] PRIMARY KEY CLUSTERED 
(
	[ItemStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standards].[QDocument](
	[DocumentIdentifier] [uniqueidentifier] NOT NULL,
	[DocumentAsset] [int] NOT NULL,
	[DocumentAuthorDate] [datetimeoffset](7) NULL,
	[DocumentAuthorName] [varchar](100) NULL,
	[DocumentType] [varchar](40) NOT NULL,
	[IsTemplate] [bit] NOT NULL,
	[Language] [nvarchar](8) NULL,
	[DocumentLabel] [varchar](64) NULL,
 CONSTRAINT [PK_QDocument] PRIMARY KEY CLUSTERED 
(
	[DocumentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standards].[QDocumentCompetency](
	[DocumentIdentifier] [uniqueidentifier] NOT NULL,
	[CompetencyIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QDocumentCompetency] PRIMARY KEY CLUSTERED 
(
	[DocumentIdentifier] ASC,
	[CompetencyIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [standards].[QRelatedDocument](
	[DocumentIdentifier] [uniqueidentifier] NOT NULL,
	[RelatedDocumentIdentifier] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QRelatedDocument] PRIMARY KEY CLUSTERED 
(
	[DocumentIdentifier] ASC,
	[RelatedDocumentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [surveys].[QSurveyCondition](
	[MaskingSurveyOptionItemIdentifier] [uniqueidentifier] NOT NULL,
	[MaskedSurveyQuestionIdentifier] [uniqueidentifier] NOT NULL,
	[OrganizationIdentifier] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QSurveyCondition] PRIMARY KEY CLUSTERED 
(
	[MaskingSurveyOptionItemIdentifier] ASC,
	[MaskedSurveyQuestionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [utilities].[Upgrade](
	[ScriptName] [varchar](128) NOT NULL,
	[UtcUpgraded] [datetimeoffset](7) NULL,
	[ScriptData] [varchar](300) NULL,
	[UpgradeIdentifier] [uniqueidentifier] NOT NULL,
	[ScriptHash] [varchar](256) NULL,
 CONSTRAINT [PK_Upgrade] PRIMARY KEY CLUSTERED 
(
	[UpgradeIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QOrganization_Code] ON [accounts].[QOrganization]
(
	[OrganizationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TDeveloper_UniqueSecret] ON [accounts].[TDeveloper]
(
	[TokenSecret] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TUserSetting_Unique] ON [accounts].[TUserSetting]
(
	[OrganizationIdentifier] ASC,
	[UserIdentifier] ASC,
	[Name] ASC,
	[ValueType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QCredential_Unique] ON [achievements].[QCredential]
(
	[AchievementIdentifier] ASC,
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QCredential_User] ON [achievements].[QCredential]
(
	[UserIdentifier] ASC
)
INCLUDE([CredentialDescription]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttempt_Assessor] ON [assessments].[QAttempt]
(
	[AssessorUserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttempt_AssessorForm] ON [assessments].[QAttempt]
(
	[AssessorUserIdentifier] ASC,
	[FormIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttempt_Form] ON [assessments].[QAttempt]
(
	[FormIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttempt_OrganizationAndLearnerUser] ON [assessments].[QAttempt]
(
	[OrganizationIdentifier] ASC,
	[LearnerUserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttemptOption1] ON [assessments].[QAttemptOption]
(
	[QuestionIdentifier] ASC
)
INCLUDE([OptionKey]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttemptOption2] ON [assessments].[QAttemptOption]
(
	[AttemptIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttemptQuestion_CompetencyItemIdentifier] ON [assessments].[QAttemptQuestion]
(
	[CompetencyItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttemptQuestion1] ON [assessments].[QAttemptQuestion]
(
	[QuestionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttemptQuestion2] ON [assessments].[QAttemptQuestion]
(
	[AttemptIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QAttemptSection_Attempt] ON [assessments].[QAttemptSection]
(
	[AttemptIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QComment_Category] ON [assets].[QComment]
(
	[CommentCategory] ASC
)
INCLUDE([ContainerIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QComment_Issue] ON [assets].[QComment]
(
	[IssueIdentifier] ASC
)
INCLUDE([CommentPosted]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QGlossaryTermContent_Unique] ON [assets].[QGlossaryTermContent]
(
	[ContainerIdentifier] ASC,
	[ContentLabel] ASC,
	[TermIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_TFile_ObjectType] ON [assets].[TFile]
(
	[ObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TFile_OrganizationIdentifier_ObjectIdentifier] ON [assets].[TFile]
(
	[OrganizationIdentifier] ASC,
	[ObjectIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_TFileClaim_Object] ON [assets].[TFileClaim]
(
	[FileIdentifier] ASC,
	[ObjectType] ASC,
	[ObjectIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QBankForm1] ON [banks].[QBankForm]
(
	[BankIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QBankQuestionSubCompetency_QuestionIdentifier] ON [banks].[QBankQuestionSubCompetency]
(
	[QuestionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QMailout_Status] ON [communications].[QMailout]
(
	[MailoutStarted] ASC,
	[MailoutCancelled] ASC,
	[MailoutCompleted] ASC,
	[MailoutScheduled] ASC,
	[MailoutStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QRecipient_DeliveryStatus] ON [communications].[QRecipient]
(
	[UserIdentifier] ASC,
	[DeliveryStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QRecipient_Mailout] ON [communications].[QRecipient]
(
	[MailoutIdentifier] ASC
)
INCLUDE([UserEmail]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QGroup_Category] ON [contacts].[QGroup]
(
	[OrganizationIdentifier] ASC,
	[GroupCategory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QGroup_GroupType] ON [contacts].[QGroup]
(
	[GroupType] ASC
)
INCLUDE([OrganizationIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QGroup_GroupType_GroupName] ON [contacts].[QGroup]
(
	[GroupType] ASC,
	[GroupName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QGroup_Name] ON [contacts].[QGroup]
(
	[OrganizationIdentifier] ASC,
	[GroupName] ASC
)
INCLUDE([GroupIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QGroupAddress_Unique] ON [contacts].[QGroupAddress]
(
	[GroupIdentifier] ASC,
	[AddressType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QGroupTag_Unique] ON [contacts].[QGroupTag]
(
	[GroupIdentifier] ASC,
	[GroupTag] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QMembership_Function] ON [contacts].[QMembership]
(
	[MembershipFunction] ASC
)
INCLUDE([GroupIdentifier],[UserIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QMembership_Group] ON [contacts].[QMembership]
(
	[GroupIdentifier] ASC
)
INCLUDE([MembershipFunction],[UserIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QMembership_Unique] ON [contacts].[QMembership]
(
	[UserIdentifier] ASC,
	[GroupIdentifier] ASC
)
INCLUDE([MembershipFunction]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QMembershipDeletion_UserGroup] ON [contacts].[QMembershipDeletion]
(
	[GroupIdentifier] ASC,
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QMembershipReason_MembershipIdentifier] ON [contacts].[QMembershipReason]
(
	[MembershipIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QPerson_AccessGranted] ON [contacts].[QPerson]
(
	[OrganizationIdentifier] ASC,
	[UserAccessGranted] ASC
)
INCLUDE([UserIdentifier],[PersonCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QPerson_IsLearner] ON [contacts].[QPerson]
(
	[UserIdentifier] ASC,
	[IsLearner] ASC
)
INCLUDE([OrganizationIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QPerson_OrganizationIdentifier] ON [contacts].[QPerson]
(
	[OrganizationIdentifier] ASC
)
INCLUDE([UserIdentifier],[PersonCode],[JobTitle],[Phone]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_QPerson_PersonCode] ON [contacts].[QPerson]
(
	[OrganizationIdentifier] ASC,
	[PersonCode] ASC
)
WHERE ([PersonCode] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TGroupPermission] ON [contacts].[TGroupPermission]
(
	[ObjectIdentifier] ASC,
	[GroupIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TContent_ContainerIdentifier] ON [contents].[TContent]
(
	[ContainerIdentifier] ASC
)
INCLUDE([ContentLabel],[ContentLanguage]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TContent_Organization] ON [contents].[TContent]
(
	[OrganizationIdentifier] ASC
)
INCLUDE([ContainerIdentifier],[ContentLabel],[ContentLanguage],[ContentSnip],[ContentText],[ContentHtml],[ContainerType],[ContentSequence],[ContentIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TContent_Unique] ON [contents].[TContent]
(
	[OrganizationIdentifier] ASC,
	[ContainerIdentifier] ASC,
	[ContentLabel] ASC,
	[ContentLanguage] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QActivity_Assessment] ON [courses].[QActivity]
(
	[AssessmentFormIdentifier] ASC
)
WHERE ([AssessmentFormIdentifier] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QActivity_GradeItem] ON [courses].[QActivity]
(
	[GradeItemIdentifier] ASC
)
WHERE ([GradeItemIdentifier] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QActivity_Survey] ON [courses].[QActivity]
(
	[SurveyFormIdentifier] ASC
)
WHERE ([SurveyFormIdentifier] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QCourseUser_Unique] ON [courses].[QCourseEnrollment]
(
	[CourseIdentifier] ASC,
	[LearnerUserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BUserEmployment_Unique] ON [custom_cmds].[BUserEmployment]
(
	[DepartmentIdentifier] ASC,
	[OrganizationIdentifier] ASC,
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BUserProfile_Unique] ON [custom_cmds].[BUserProfile]
(
	[DepartmentIdentifier] ASC,
	[OrganizationIdentifier] ASC,
	[UserIdentifier] ASC,
	[Sequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QInvoiceFee_Organization] ON [custom_cmds].[QCmdsInvoiceFee]
(
	[CompanyName] ASC
)
INCLUDE([BillingClassification],[PricePerUserPerPeriodPerCompany],[SharedCompanyCount],[UserEmail]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QUserStatus1] ON [custom_cmds].[QUserStatus]
(
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TUserStatus_Department] ON [custom_cmds].[TUserStatus]
(
	[DepartmentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_TUserStatus_Item] ON [custom_cmds].[TUserStatus]
(
	[ListDomain] ASC
)
INCLUDE([ItemName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TUserStatus_Organization] ON [custom_cmds].[TUserStatus]
(
	[OrganizationIdentifier] ASC
)
INCLUDE([AsAt],[UserIdentifier],[DepartmentIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TUserStatus_OrganizationAsAt] ON [custom_cmds].[TUserStatus]
(
	[AsAt] ASC,
	[OrganizationIdentifier] ASC
)
INCLUDE([UserIdentifier],[DepartmentIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TUserStatus_Unique] ON [custom_cmds].[TUserStatus]
(
	[ItemName] ASC,
	[AsAt] ASC,
	[UserIdentifier] ASC,
	[DepartmentIdentifier] ASC,
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_UserComplianceSnapshot_SnapshotDate] ON [custom_cmds].[ZUserStatus]
(
	[AsAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ZUserStatusSummary_SnapshotDate] ON [custom_cmds].[ZUserStatusSummary]
(
	[SnapshotDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Counter_Unique] ON [custom_ncsha].[Counter]
(
	[CounterIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_NcshaFilter1] ON [custom_ncsha].[Filter]
(
	[FilterName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QUser_AccessGrantedToCmds] ON [identities].[QUser]
(
	[AccessGrantedToCmds] ASC
)
INCLUDE([FirstName],[LastName],[UserIdentifier],[UtcArchived]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QUser_Archived] ON [identities].[QUser]
(
	[AccessGrantedToCmds] ASC,
	[UtcArchived] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QUser_UniqueEmail] ON [identities].[QUser]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TScormRegistration_Unique] ON [integration].[TScormRegistration]
(
	[LearnerIdentifier] ASC,
	[ScormPackageHook] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QIssue_OrganizationIdentifier] ON [issues].[QIssue]
(
	[OrganizationIdentifier] ASC
)
INCLUDE([IssueOpened]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QIssueAttachment_IssueIdentifier] ON [issues].[QIssueAttachment]
(
	[IssueIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QIssueAttachment_Unique] ON [issues].[QIssueAttachment]
(
	[FileName] ASC,
	[IssueIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QIssueGroup_Unique] ON [issues].[QIssueGroup]
(
	[GroupIdentifier] ASC,
	[IssueIdentifier] ASC,
	[IssueRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QIssueUser_Unique] ON [issues].[QIssueUser]
(
	[UserIdentifier] ASC,
	[IssueIdentifier] ASC,
	[IssueRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Aggregate_Organization] ON [logs].[Aggregate]
(
	[OriginOrganization] ASC
)
INCLUDE([AggregateIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Aggregate_Root] ON [logs].[Aggregate]
(
	[RootAggregateIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Change_ChangeType] ON [logs].[Change]
(
	[ChangeType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Command_Organization] ON [logs].[Command]
(
	[OriginOrganization] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QClick_OrganizationIdentifier] ON [messages].[QClick]
(
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QFollower_Unique] ON [messages].[QFollower]
(
	[FollowerIdentifier] ASC,
	[MessageIdentifier] ASC,
	[SubscriberIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QMessage_OrganizationIdentifier] ON [messages].[QMessage]
(
	[OrganizationIdentifier] ASC
)
INCLUDE([MessageType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QMessage_SurveyFormIdentifier] ON [messages].[QMessage]
(
	[SurveyFormIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QSubscriberUser1] ON [messages].[QSubscriberUser]
(
	[MessageIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TDiscount_Code] ON [payments].[TDiscount]
(
	[DiscountCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QAchievementPrerequisite_Unique] ON [records].[QAchievementPrerequisite]
(
	[AchievementIdentifier] ASC,
	[PrerequisiteAchievementIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QEnrollment_Learner] ON [records].[QEnrollment]
(
	[LearnerIdentifier] ASC
)
INCLUDE([GradebookIdentifier],[EnrollmentStarted]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QEnrollment_Unique] ON [records].[QEnrollment]
(
	[GradebookIdentifier] ASC,
	[LearnerIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QValidation_Unique] ON [records].[QGradebookCompetencyValidation]
(
	[CompetencyIdentifier] ASC,
	[GradebookIdentifier] ASC,
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QGradeItem_Gradebook] ON [records].[QGradeItem]
(
	[GradebookIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QGradeItem_Parent] ON [records].[QGradeItem]
(
	[ParentGradeItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_QJournal_JournalSetup_User] ON [records].[QJournal]
(
	[JournalSetupIdentifier] ASC,
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_QJournalSetupField] ON [records].[QJournalSetupField]
(
	[JournalSetupIdentifier] ASC,
	[FieldType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QJournalSetupUser_Unique] ON [records].[QJournalSetupUser]
(
	[EnrollmentIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QProgress_GradebookIdentifier] ON [records].[QProgress]
(
	[GradebookIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QProgress_GradeItemIdentifier] ON [records].[QProgress]
(
	[GradeItemIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QProgress_OrganizationIdentifier] ON [records].[QProgress]
(
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QProgress_ProgressCreated_ProgressIdentifier] ON [records].[QProgress]
(
	[ProgressAdded] DESC,
	[ProgressIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QProgress_UserIdentifier] ON [records].[QProgress]
(
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_QProgress_Item_User] ON [records].[QProgress]
(
	[GradeItemIdentifier] ASC,
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QRubricCriterion_RubricIdentifier] ON [records].[QRubricCriterion]
(
	[RubricIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QRubricRating_RubricCriterionIdentifier] ON [records].[QRubricRating]
(
	[RubricCriterionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_TProgramGroupEnrollment] ON [records].[TProgramGroupEnrollment]
(
	[GroupIdentifier] ASC,
	[ProgramIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QRegistration_Event] ON [registrations].[QRegistration]
(
	[EventIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApiRequest_Organization] ON [reports].[ApiRequest]
(
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UniqueMeasurement] ON [reports].[TMeasurement]
(
	[OrganizationIdentifier] ASC,
	[ContainerIdentifier] ASC,
	[AsAtDate] ASC,
	[IntervalType] ASC,
	[UniquePathHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TReport_CreatedBy] ON [reports].[TReport]
(
	[CreatedBy] ASC
)
INCLUDE([ReportIdentifier],[OrganizationIdentifier],[UserIdentifier],[ReportTitle],[ReportData],[ReportDescription],[ReportType],[Created],[Modified],[ModifiedBy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Upload_Url] ON [resources].[Upload]
(
	[NavigateUrl] ASC,
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Upload1] ON [resources].[Upload]
(
	[ContainerIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_UploadRelation_UploadIdentifier] ON [resources].[UploadRelation]
(
	[UploadIdentifier] ASC
)
INCLUDE([ContainerIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_TUserSession_SessionCode] ON [security].[TUserSession]
(
	[SessionCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TUserSession_User] ON [security].[TUserSession]
(
	[UserIdentifier] ASC
)
INCLUDE([SessionMinutes]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TUserSession_UserStart] ON [security].[TUserSession]
(
	[SessionStarted] ASC,
	[UserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TAction_Url] ON [settings].[TAction]
(
	[ActionUrl] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QPage_Parent] ON [sites].[QPage]
(
	[ParentPageIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QPage_ParentPageIdentifier] ON [sites].[QPage]
(
	[ParentPageIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QStandard_ParentStandardIdentifier] ON [standard].[QStandard]
(
	[ParentStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_QStandard_StandardType_OrganizationIdentifier] ON [standard].[QStandard]
(
	[StandardType] ASC,
	[OrganizationIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QStandardContainment_ChildStandardIdentifier] ON [standard].[QStandardContainment]
(
	[ChildStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QStandardContainment_ParentStandardIdentifier] ON [standard].[QStandardContainment]
(
	[ParentStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QStandardGroup_DepartmentIdentifier] ON [standard].[QStandardGroup]
(
	[GroupIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QStandardTier_RootStandardIdentifier] ON [standard].[QStandardTier]
(
	[RootStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QStandardValidation_StandardIdentifier] ON [standard].[QStandardValidation]
(
	[StandardIdentifier] ASC
)
INCLUDE([UserIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QStandardValidation_UserIdentifier] ON [standard].[QStandardValidation]
(
	[UserIdentifier] ASC
)
INCLUDE([StandardIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DepartmentProfileCompetency1] ON [standards].[DepartmentProfileCompetency]
(
	[CompetencyStandardIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DepartmentProfilePerson_IsPrimary] ON [standards].[DepartmentProfileUser]
(
	[IsPrimary] ASC
)
INCLUDE([DepartmentIdentifier],[ProfileStandardIdentifier],[UserIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DepartmentProfilePerson_ProfileAssetID] ON [standards].[DepartmentProfileUser]
(
	[ProfileStandardIdentifier] ASC
)
INCLUDE([UserIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DepartmentProfilePerson1] ON [standards].[DepartmentProfileUser]
(
	[UserIdentifier] ASC
)
INCLUDE([DepartmentIdentifier],[ProfileStandardIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QResponseAnswer_SurveyQuestionIdentifier] ON [surveys].[QResponseAnswer]
(
	[SurveyQuestionIdentifier] ASC
)
INCLUDE([ResponseSessionIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QResponseOption_ResponseOptionIsSelected] ON [surveys].[QResponseOption]
(
	[ResponseOptionIsSelected] ASC
)
INCLUDE([SurveyOptionIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QResponseOption_SurveyOptionIdentifier] ON [surveys].[QResponseOption]
(
	[SurveyOptionIdentifier] ASC
)
INCLUDE([ResponseSessionIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QResponseSession_SurveyFormIdentifier] ON [surveys].[QResponseSession]
(
	[SurveyFormIdentifier] ASC
)
INCLUDE([ResponseSessionIdentifier]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QResponseSession_SurveyRespondent] ON [surveys].[QResponseSession]
(
	[SurveyFormIdentifier] ASC,
	[RespondentUserIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_QSurveyOptionList_SurveyQuestionIdentifier] ON [surveys].[QSurveyOptionList]
(
	[SurveyQuestionIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TCollection_Name] ON [utilities].[TCollection]
(
	[CollectionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TCollectionItem_Number] ON [utilities].[TCollectionItem]
(
	[ItemNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TUpgrade_Unique] ON [utilities].[Upgrade]
(
	[ScriptName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [accounts].[TSender] ADD  CONSTRAINT [DF_TSender_SenderEnabled]  DEFAULT ((0)) FOR [SenderEnabled]
GO
ALTER TABLE [accounts].[TUserSessionCache] ADD  CONSTRAINT [DF_TUserSessionCache_CacheIdentifier]  DEFAULT (newid()) FOR [CacheIdentifier]
GO
ALTER TABLE [accounts].[TUserSetting] ADD  CONSTRAINT [DF_TUserSetting_SettingIdentifier]  DEFAULT (newid()) FOR [SettingIdentifier]
GO
ALTER TABLE [achievements].[QAchievement] ADD  CONSTRAINT [DF_QAchievement_AchievementIsEnabled]  DEFAULT ((0)) FOR [AchievementIsEnabled]
GO
ALTER TABLE [achievements].[QAchievement] ADD  CONSTRAINT [DF_QAchievement_AchievementReportingDisabled]  DEFAULT ((0)) FOR [AchievementReportingDisabled]
GO
ALTER TABLE [achievements].[QAchievement] ADD  CONSTRAINT [DF_QAchievement_HasBadgeImage]  DEFAULT ((0)) FOR [HasBadgeImage]
GO
ALTER TABLE [achievements].[QAchievement] ADD  CONSTRAINT [DF_QAchievement_AchievementAllowSelfDeclared]  DEFAULT ((0)) FOR [AchievementAllowSelfDeclared]
GO
ALTER TABLE [achievements].[TAchievementDepartment] ADD  CONSTRAINT [DF_TAchievementDepartment_JoinIdentifier]  DEFAULT (newid()) FOR [JoinIdentifier]
GO
ALTER TABLE [assessments].[QAttempt] ADD  CONSTRAINT [DF_QAttempt_TabNavigationEnabled]  DEFAULT ((1)) FOR [TabNavigationEnabled]
GO
ALTER TABLE [assets].[QComment] ADD  CONSTRAINT [DF_QComment_CommentIsHidden]  DEFAULT ((0)) FOR [CommentIsHidden]
GO
ALTER TABLE [assets].[QComment] ADD  CONSTRAINT [DF_QComment_CommentIsPrivate]  DEFAULT ((0)) FOR [CommentIsPrivate]
GO
ALTER TABLE [assets].[QGlossaryTerm] ADD  CONSTRAINT [DF_QGlossaryTerm_RevisionCount]  DEFAULT ((0)) FOR [RevisionCount]
GO
ALTER TABLE [banks].[QBank] ADD  CONSTRAINT [DF_QBank_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [banks].[QBankForm] ADD  CONSTRAINT [DF_QBankForm_FormAssetVersion]  DEFAULT ((0)) FOR [FormAssetVersion]
GO
ALTER TABLE [banks].[QBankForm] ADD  CONSTRAINT [DF_QBankForm_FormAttemptLimit]  DEFAULT ((0)) FOR [FormAttemptLimit]
GO
ALTER TABLE [banks].[QBankForm] ADD  CONSTRAINT [DF_QBankForm_FormThirdPartyAssessmentIsEnabled]  DEFAULT ((0)) FOR [FormThirdPartyAssessmentIsEnabled]
GO
ALTER TABLE [billing].[TOrderItem] ADD  CONSTRAINT [DF_TOrderItem_OrderItemQuantity]  DEFAULT ((1)) FOR [OrderItemQuantity]
GO
ALTER TABLE [billing].[TOrderItem] ADD  CONSTRAINT [DF_TOrderItem_UnitPrice]  DEFAULT ((0)) FOR [UnitPrice]
GO
ALTER TABLE [billing].[TProduct] ADD  CONSTRAINT [DF_TProduct_IsFeatured]  DEFAULT ((0)) FOR [IsFeatured]
GO
ALTER TABLE [billing].[TProduct] ADD  CONSTRAINT [DF_TProduct_IsTaxable]  DEFAULT ((0)) FOR [IsTaxable]
GO
ALTER TABLE [billing].[TProduct] ADD  CONSTRAINT [DF_TProduct_ProductQuantity]  DEFAULT ((0)) FOR [ProductQuantity]
GO
ALTER TABLE [billing].[TTax] ADD  CONSTRAINT [DF_TTax_CountryCode]  DEFAULT ('CA') FOR [CountryCode]
GO
ALTER TABLE [contacts].[QGroupAddress] ADD  CONSTRAINT [DF_QGroupAddress_AddressIdentifier]  DEFAULT (newid()) FOR [AddressIdentifier]
GO
ALTER TABLE [contacts].[QGroupTag] ADD  CONSTRAINT [DF_QGroupTag_TagIdentifier]  DEFAULT (newid()) FOR [TagIdentifier]
GO
ALTER TABLE [contacts].[QPerson] ADD  CONSTRAINT [DF_QPerson_MarketingEmailEnabled]  DEFAULT ((1)) FOR [MarketingEmailEnabled]
GO
ALTER TABLE [contacts].[QPerson] ADD  CONSTRAINT [DF_QPerson_IsArchived]  DEFAULT ((0)) FOR [IsArchived]
GO
ALTER TABLE [contacts].[QPerson] ADD  CONSTRAINT [DF_QPerson_IsDeveloper]  DEFAULT ((0)) FOR [IsDeveloper]
GO
ALTER TABLE [contacts].[TGroupPermission] ADD  CONSTRAINT [DF_TGroupPermission_AllowExecute]  DEFAULT ((0)) FOR [AllowExecute]
GO
ALTER TABLE [contacts].[TGroupPermission] ADD  CONSTRAINT [DF_TGroupPermission_AllowRead]  DEFAULT ((0)) FOR [AllowRead]
GO
ALTER TABLE [contacts].[TGroupPermission] ADD  CONSTRAINT [DF_TGroupPermission_AllowWrite]  DEFAULT ((0)) FOR [AllowWrite]
GO
ALTER TABLE [contacts].[TGroupPermission] ADD  CONSTRAINT [DF_TGroupPermission_AllowCreate]  DEFAULT ((0)) FOR [AllowCreate]
GO
ALTER TABLE [contacts].[TGroupPermission] ADD  CONSTRAINT [DF_TGroupPermission_AllowDelete]  DEFAULT ((0)) FOR [AllowDelete]
GO
ALTER TABLE [contacts].[TGroupPermission] ADD  CONSTRAINT [DF_TGroupPermission_AllowAdministrate]  DEFAULT ((0)) FOR [AllowAdministrate]
GO
ALTER TABLE [contacts].[TGroupPermission] ADD  CONSTRAINT [DF_TGroupPermission_AllowConfigure]  DEFAULT ((0)) FOR [AllowConfigure]
GO
ALTER TABLE [contacts].[TGroupPermission] ADD  CONSTRAINT [DF_TGroupPermission_PermissionMask]  DEFAULT ((0)) FOR [PermissionMask]
GO
ALTER TABLE [courses].[TLtiLink] ADD  CONSTRAINT [DF_TLtiLink_LinkIdentifier]  DEFAULT (newid()) FOR [LinkIdentifier]
GO
ALTER TABLE [custom_cmds].[BUserDepartmentCompetencyStatus] ADD  CONSTRAINT [DF_BUserDepartmentCompetencyStatus_JoinIdentifier]  DEFAULT (newid()) FOR [JoinIdentifier]
GO
ALTER TABLE [custom_cmds].[BUserEmployment] ADD  CONSTRAINT [DF_BUserEmployment_JoinIdentifier]  DEFAULT (newid()) FOR [JoinIdentifier]
GO
ALTER TABLE [custom_cmds].[BUserProfile] ADD  CONSTRAINT [DF_BUserProfile_JoinIdentifier]  DEFAULT (newid()) FOR [JoinIdentifier]
GO
ALTER TABLE [custom_cmds].[TUserStatus] ADD  CONSTRAINT [DF_TUserStatus_ItemNumber]  DEFAULT ((0)) FOR [ItemNumber]
GO
ALTER TABLE [custom_cmds].[TUserStatus] ADD  CONSTRAINT [DF_TUserStatus_CountVN]  DEFAULT ((0)) FOR [CountVN]
GO
ALTER TABLE [custom_cmds].[TUserStatus] ADD  CONSTRAINT [DF_TUserStatus_JoinIdentifier]  DEFAULT (newid()) FOR [JoinIdentifier]
GO
ALTER TABLE [custom_cmds].[ZUserStatusSummary] ADD  CONSTRAINT [DF_ZUserStatusSummary_SummaryIdentifier]  DEFAULT (newid()) FOR [SummaryIdentifier]
GO
ALTER TABLE [custom_ita].[Individual] ADD  CONSTRAINT [DF_Individual_IsNew]  DEFAULT ((0)) FOR [IsNew]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable01]  DEFAULT ((1)) FOR [IsVisibleOnTable01]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable02]  DEFAULT ((1)) FOR [IsVisibleOnTable02]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable03]  DEFAULT ((1)) FOR [IsVisibleOnTable03]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable04]  DEFAULT ((1)) FOR [IsVisibleOnTable04]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable05]  DEFAULT ((1)) FOR [IsVisibleOnTable05]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable06]  DEFAULT ((1)) FOR [IsVisibleOnTable06]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable07]  DEFAULT ((1)) FOR [IsVisibleOnTable07]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable08]  DEFAULT ((1)) FOR [IsVisibleOnTable08]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable09]  DEFAULT ((1)) FOR [IsVisibleOnTable09]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable10]  DEFAULT ((1)) FOR [IsVisibleOnTable10]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable11]  DEFAULT ((1)) FOR [IsVisibleOnTable11]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable12]  DEFAULT ((1)) FOR [IsVisibleOnTable12]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable13]  DEFAULT ((1)) FOR [IsVisibleOnTable13]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable14]  DEFAULT ((1)) FOR [IsVisibleOnTable14]
GO
ALTER TABLE [custom_ncsha].[AbProgram] ADD  CONSTRAINT [DF_AbProgram_IsVisibleOnTable15]  DEFAULT ((1)) FOR [IsVisibleOnTable15]
GO
ALTER TABLE [custom_ncsha].[Counter] ADD  CONSTRAINT [DF_Counter_CounterIdentifier]  DEFAULT (newid()) FOR [CounterIdentifier]
GO
ALTER TABLE [custom_ncsha].[Filter] ADD  CONSTRAINT [DF_Filter_FilterID]  DEFAULT (newid()) FOR [FilterID]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable01]  DEFAULT ((1)) FOR [IsVisibleOnTable01]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable02]  DEFAULT ((1)) FOR [IsVisibleOnTable02]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable03]  DEFAULT ((1)) FOR [IsVisibleOnTable03]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable04]  DEFAULT ((1)) FOR [IsVisibleOnTable04]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable05]  DEFAULT ((1)) FOR [IsVisibleOnTable05]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable06]  DEFAULT ((1)) FOR [IsVisibleOnTable06]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable07]  DEFAULT ((1)) FOR [IsVisibleOnTable07]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable08]  DEFAULT ((1)) FOR [IsVisibleOnTable08]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable09]  DEFAULT ((1)) FOR [IsVisibleOnTable09]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable11]  DEFAULT ((1)) FOR [IsVisibleOnTable11]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable12]  DEFAULT ((1)) FOR [IsVisibleOnTable12]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable13]  DEFAULT ((1)) FOR [IsVisibleOnTable13]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable14]  DEFAULT ((1)) FOR [IsVisibleOnTable14]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable15]  DEFAULT ((1)) FOR [IsVisibleOnTable15]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable16]  DEFAULT ((1)) FOR [IsVisibleOnTable16]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable17]  DEFAULT ((1)) FOR [IsVisibleOnTable17]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable18]  DEFAULT ((1)) FOR [IsVisibleOnTable18]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable19]  DEFAULT ((1)) FOR [IsVisibleOnTable19]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable20]  DEFAULT ((1)) FOR [IsVisibleOnTable20]
GO
ALTER TABLE [custom_ncsha].[HcProgram] ADD  CONSTRAINT [DF_HcProgram_IsVisibleOnTable10]  DEFAULT ((1)) FOR [IsVisibleOnTable10]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable01]  DEFAULT ((1)) FOR [IsVisibleOnTable01]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable02]  DEFAULT ((1)) FOR [IsVisibleOnTable02]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable03]  DEFAULT ((1)) FOR [IsVisibleOnTable03]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable04]  DEFAULT ((1)) FOR [IsVisibleOnTable04]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable05]  DEFAULT ((1)) FOR [IsVisibleOnTable05]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable06]  DEFAULT ((1)) FOR [IsVisibleOnTable06]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable07]  DEFAULT ((1)) FOR [IsVisibleOnTable07]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable08]  DEFAULT ((1)) FOR [IsVisibleOnTable08]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable09]  DEFAULT ((1)) FOR [IsVisibleOnTable09]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable10]  DEFAULT ((1)) FOR [IsVisibleOnTable10]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable11]  DEFAULT ((1)) FOR [IsVisibleOnTable11]
GO
ALTER TABLE [custom_ncsha].[HiProgram] ADD  CONSTRAINT [DF_HiProgram_IsVisibleOnTable12]  DEFAULT ((1)) FOR [IsVisibleOnTable12]
GO
ALTER TABLE [custom_ncsha].[History] ADD  CONSTRAINT [DF_History_RecordID]  DEFAULT (newid()) FOR [RecordID]
GO
ALTER TABLE [custom_ncsha].[History] ADD  CONSTRAINT [DF_History_RecordTime]  DEFAULT (getutcdate()) FOR [RecordTime]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable01]  DEFAULT ((1)) FOR [IsVisibleOnTable01]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable02]  DEFAULT ((1)) FOR [IsVisibleOnTable02]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable03]  DEFAULT ((1)) FOR [IsVisibleOnTable03]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable04]  DEFAULT ((1)) FOR [IsVisibleOnTable04]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable05]  DEFAULT ((1)) FOR [IsVisibleOnTable05]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable06]  DEFAULT ((1)) FOR [IsVisibleOnTable06]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable07]  DEFAULT ((1)) FOR [IsVisibleOnTable07]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable08]  DEFAULT ((1)) FOR [IsVisibleOnTable08]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable09]  DEFAULT ((1)) FOR [IsVisibleOnTable09]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable10]  DEFAULT ((1)) FOR [IsVisibleOnTable10]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable11]  DEFAULT ((1)) FOR [IsVisibleOnTable11]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable12]  DEFAULT ((1)) FOR [IsVisibleOnTable12]
GO
ALTER TABLE [custom_ncsha].[MfProgram] ADD  CONSTRAINT [DF_MfProgram_IsVisibleOnTable13]  DEFAULT ((1)) FOR [IsVisibleOnTable13]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable14]  DEFAULT ((1)) FOR [IsVisibleOnTable14]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable01]  DEFAULT ((1)) FOR [IsVisibleOnTable01]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable02]  DEFAULT ((1)) FOR [IsVisibleOnTable02]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable03]  DEFAULT ((1)) FOR [IsVisibleOnTable03]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable04]  DEFAULT ((1)) FOR [IsVisibleOnTable04]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable05]  DEFAULT ((1)) FOR [IsVisibleOnTable05]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable06]  DEFAULT ((1)) FOR [IsVisibleOnTable06]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable07]  DEFAULT ((1)) FOR [IsVisibleOnTable07]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable08]  DEFAULT ((1)) FOR [IsVisibleOnTable08]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable09]  DEFAULT ((1)) FOR [IsVisibleOnTable09]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable10]  DEFAULT ((1)) FOR [IsVisibleOnTable10]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable11]  DEFAULT ((1)) FOR [IsVisibleOnTable11]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable12]  DEFAULT ((1)) FOR [IsVisibleOnTable12]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable13]  DEFAULT ((1)) FOR [IsVisibleOnTable13]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable15]  DEFAULT ((1)) FOR [IsVisibleOnTable15]
GO
ALTER TABLE [custom_ncsha].[MrProgram] ADD  CONSTRAINT [DF_MrProgram_IsVisibleOnTable16]  DEFAULT ((1)) FOR [IsVisibleOnTable16]
GO
ALTER TABLE [custom_ncsha].[PaProgram] ADD  CONSTRAINT [DF_PaProgram_IsVisibleOnTable01]  DEFAULT ((1)) FOR [IsVisibleOnTable01]
GO
ALTER TABLE [custom_ncsha].[PaProgram] ADD  CONSTRAINT [DF_PaProgram_IsVisibleOnTable02]  DEFAULT ((1)) FOR [IsVisibleOnTable02]
GO
ALTER TABLE [custom_ncsha].[PaProgram] ADD  CONSTRAINT [DF_PaProgram_IsVisibleOnTable03]  DEFAULT ((1)) FOR [IsVisibleOnTable03]
GO
ALTER TABLE [custom_ncsha].[PaProgram] ADD  CONSTRAINT [DF_PaProgram_IsVisibleOnTable04]  DEFAULT ((1)) FOR [IsVisibleOnTable04]
GO
ALTER TABLE [events].[QEvent] ADD  CONSTRAINT [DF_QEvent_WaitlistEnabled]  DEFAULT ((1)) FOR [WaitlistEnabled]
GO
ALTER TABLE [events].[QEvent] ADD  CONSTRAINT [DF_QEvent_PersonCodeIsRequired]  DEFAULT ((0)) FOR [PersonCodeIsRequired]
GO
ALTER TABLE [events].[QEvent] ADD  CONSTRAINT [DF_QEvent_AllowMultipleRegistrations]  DEFAULT ((0)) FOR [AllowMultipleRegistrations]
GO
ALTER TABLE [events].[QSeat] ADD  CONSTRAINT [DF_QSeat_IsAvailable]  DEFAULT ((0)) FOR [IsAvailable]
GO
ALTER TABLE [events].[QSeat] ADD  CONSTRAINT [DF_QSeat_IsTaxable]  DEFAULT ((0)) FOR [IsTaxable]
GO
ALTER TABLE [identities].[Impersonation] ADD  CONSTRAINT [DF_Impersonation_ImpersonationIdentifier]  DEFAULT (newid()) FOR [ImpersonationIdentifier]
GO
ALTER TABLE [identities].[QUser] ADD  CONSTRAINT [DF_QUser_AccessGrantedToCmds]  DEFAULT ((0)) FOR [AccessGrantedToCmds]
GO
ALTER TABLE [identities].[QUser] ADD  CONSTRAINT [DF_QUser_MultiFactorAuthentication]  DEFAULT ((0)) FOR [MultiFactorAuthentication]
GO
ALTER TABLE [identities].[QUserConnection] ADD  CONSTRAINT [DF_QUserConnection_IsManager]  DEFAULT ((0)) FOR [IsManager]
GO
ALTER TABLE [identities].[QUserConnection] ADD  CONSTRAINT [DF_QUserConnection_IsSupervisor]  DEFAULT ((0)) FOR [IsSupervisor]
GO
ALTER TABLE [identities].[QUserConnection] ADD  CONSTRAINT [DF_QUserConnection_IsValidator]  DEFAULT ((0)) FOR [IsValidator]
GO
ALTER TABLE [invoices].[QInvoiceItem] ADD  CONSTRAINT [DF_QInvoiceItem_ItemSequence]  DEFAULT ((0)) FOR [ItemSequence]
GO
ALTER TABLE [issues].[QIssue] ADD  CONSTRAINT [DF_QIssue_GroupCount]  DEFAULT ((0)) FOR [GroupCount]
GO
ALTER TABLE [issues].[QIssueAttachment] ADD  CONSTRAINT [DF_QIssueAttachment_AttachmentIdentifier]  DEFAULT (newid()) FOR [AttachmentIdentifier]
GO
ALTER TABLE [issues].[QIssueGroup] ADD  CONSTRAINT [DF_QIssueGroup_JoinIdentifier]  DEFAULT (newid()) FOR [JoinIdentifier]
GO
ALTER TABLE [issues].[QIssueUser] ADD  CONSTRAINT [DF_QIssueUser_JoinIdentifier]  DEFAULT (newid()) FOR [JoinIdentifier]
GO
ALTER TABLE [learning].[TCatalog] ADD  CONSTRAINT [DF_TCatalog_CatalogIdentifier]  DEFAULT (newsequentialid()) FOR [CatalogIdentifier]
GO
ALTER TABLE [learning].[TCatalog] ADD  CONSTRAINT [DF_TCatalog_IsHidden]  DEFAULT ((0)) FOR [IsHidden]
GO
ALTER TABLE [messages].[QClick] ADD  CONSTRAINT [DF_QClick_ClickIdentifier]  DEFAULT (newid()) FOR [ClickIdentifier]
GO
ALTER TABLE [messages].[QFollower] ADD  CONSTRAINT [DF_QFollower_JoinIdentifier]  DEFAULT (newid()) FOR [JoinIdentifier]
GO
ALTER TABLE [messages].[QMessage] ADD  CONSTRAINT [DF_QMessage_IsDisabled]  DEFAULT ((0)) FOR [IsDisabled]
GO
ALTER TABLE [messages].[QMessage] ADD  CONSTRAINT [DF_QMessage_AutoBccSubscribers]  DEFAULT ((0)) FOR [AutoBccSubscribers]
GO
ALTER TABLE [payments].[TDiscount] ADD  CONSTRAINT [DF_TDiscount_DiscountIdentifier]  DEFAULT (newid()) FOR [DiscountIdentifier]
GO
ALTER TABLE [records].[QGradebook] ADD  CONSTRAINT [DF_QGradebook_IsLocked]  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [records].[QGradebookCompetencyValidation] ADD  CONSTRAINT [DF_QGradebookCompetencyValidation_ValidationIdentifier]  DEFAULT (newid()) FOR [ValidationIdentifier]
GO
ALTER TABLE [records].[QGradeItem] ADD  CONSTRAINT [DF_QGradeItem_GradeItemIsReported]  DEFAULT ((0)) FOR [GradeItemIsReported]
GO
ALTER TABLE [records].[QJournalSetup] ADD  CONSTRAINT [DF_QJournalSetup_IsValidationRequired]  DEFAULT ((0)) FOR [IsValidationRequired]
GO
ALTER TABLE [records].[QJournalSetup] ADD  CONSTRAINT [DF_QJournalSetup_AllowLogbookDownload]  DEFAULT ((0)) FOR [AllowLogbookDownload]
GO
ALTER TABLE [records].[QJournalSetupUser] ADD  CONSTRAINT [DF_QJournalSetupUser_EnrollmentIdentifier]  DEFAULT (newid()) FOR [EnrollmentIdentifier]
GO
ALTER TABLE [records].[QProgress] ADD  CONSTRAINT [DF_QProgress_ProgressIsPublished]  DEFAULT ((0)) FOR [ProgressIsPublished]
GO
ALTER TABLE [records].[QProgress] ADD  CONSTRAINT [DF_QProgress_ProgressIsCompleted]  DEFAULT ((0)) FOR [ProgressIsCompleted]
GO
ALTER TABLE [records].[QProgress] ADD  CONSTRAINT [DF_QProgress_ProgressIsLocked]  DEFAULT ((0)) FOR [ProgressIsLocked]
GO
ALTER TABLE [records].[QProgress] ADD  CONSTRAINT [DF_QProgress_ProgressIsDisabled]  DEFAULT ((0)) FOR [ProgressIsDisabled]
GO
ALTER TABLE [records].[QProgress] ADD  CONSTRAINT [DF_QProgress_ProgressIsIgnored]  DEFAULT ((0)) FOR [ProgressIsIgnored]
GO
ALTER TABLE [records].[TProgram] ADD  CONSTRAINT [DF_TProgram_IsHidden]  DEFAULT ((0)) FOR [IsHidden]
GO
ALTER TABLE [records].[TProgramEnrollment] ADD  CONSTRAINT [DF_TProgramEnrollment_MessageCompletedSentCount]  DEFAULT ((0)) FOR [MessageCompletedSentCount]
GO
ALTER TABLE [records].[TProgramEnrollment] ADD  CONSTRAINT [DF_TProgramEnrollment_MessageStalledSentCount]  DEFAULT ((0)) FOR [MessageStalledSentCount]
GO
ALTER TABLE [records].[TTask] ADD  CONSTRAINT [DF_TTask_TaskIsPlanned]  DEFAULT ((0)) FOR [TaskIsPlanned]
GO
ALTER TABLE [registrations].[QRegistration] ADD  CONSTRAINT [DF_QRegistration_IncludeInT2202]  DEFAULT ((0)) FOR [IncludeInT2202]
GO
ALTER TABLE [reports].[ApiRequest] ADD  CONSTRAINT [DF_ApiRequest_RequestIdentifier]  DEFAULT (newid()) FOR [RequestIdentifier]
GO
ALTER TABLE [security].[TUserSession] ADD  CONSTRAINT [DF_TUserSession_SessionIsAuthenticated]  DEFAULT ((0)) FOR [SessionIsAuthenticated]
GO
ALTER TABLE [security].[TUserSession] ADD  CONSTRAINT [DF_TUserSession_SessionIdentifier]  DEFAULT (newid()) FOR [SessionIdentifier]
GO
ALTER TABLE [sites].[QPage] ADD  CONSTRAINT [DF_QPage_IsAccessDenied]  DEFAULT ((0)) FOR [IsAccessDenied]
GO
ALTER TABLE [standards].[DepartmentProfileCompetency] ADD  CONSTRAINT [DF_DepartmentProfileCompetency_IsCritical]  DEFAULT ((0)) FOR [IsCritical]
GO
ALTER TABLE [standards].[DepartmentProfileUser] ADD  CONSTRAINT [DF_DepartmentProfileUser_IsPrimary]  DEFAULT ((0)) FOR [IsPrimary]
GO
ALTER TABLE [standards].[DepartmentProfileUser] ADD  CONSTRAINT [DF_DepartmentProfileUser_IsRequired]  DEFAULT ((0)) FOR [IsRequired]
GO
ALTER TABLE [standards].[DepartmentProfileUser] ADD  CONSTRAINT [DF_DepartmentProfileUser_IsRecommended]  DEFAULT ((0)) FOR [IsRecommended]
GO
ALTER TABLE [standards].[DepartmentProfileUser] ADD  CONSTRAINT [DF_DepartmentProfileUser_IsInProgress]  DEFAULT ((0)) FOR [IsInProgress]
GO
ALTER TABLE [surveys].[QResponseOption] ADD  CONSTRAINT [DF_QResponseOption_ResponseOptionIsSelected]  DEFAULT ((0)) FOR [ResponseOptionIsSelected]
GO
ALTER TABLE [surveys].[QResponseSession] ADD  CONSTRAINT [DF_QResponseSession_ResponseIsLocked]  DEFAULT ((0)) FOR [ResponseIsLocked]
GO
ALTER TABLE [surveys].[QSurveyForm] ADD  CONSTRAINT [DF_QSurveyForm_RequireUserAuthentication]  DEFAULT ((0)) FOR [RequireUserAuthentication]
GO
ALTER TABLE [surveys].[QSurveyForm] ADD  CONSTRAINT [DF_QSurveyForm_RequireUserIdentification]  DEFAULT ((0)) FOR [RequireUserIdentification]
GO
ALTER TABLE [surveys].[QSurveyForm] ADD  CONSTRAINT [DF_QSurveyForm_EnableUserConfidentiality]  DEFAULT ((0)) FOR [EnableUserConfidentiality]
GO
ALTER TABLE [surveys].[QSurveyForm] ADD  CONSTRAINT [DF_QSurveyForm_DisplaySummaryChart]  DEFAULT ((0)) FOR [DisplaySummaryChart]
GO
ALTER TABLE [surveys].[QSurveyQuestion] ADD  CONSTRAINT [DF_QSurveyQuestion_SurveyQuestionIsRequired]  DEFAULT ((0)) FOR [SurveyQuestionIsRequired]
GO
ALTER TABLE [surveys].[QSurveyQuestion] ADD  CONSTRAINT [DF_QSurveyQuestion_SurveyQuestionListEnableBranch]  DEFAULT ((0)) FOR [SurveyQuestionListEnableBranch]
GO
ALTER TABLE [surveys].[QSurveyQuestion] ADD  CONSTRAINT [DF_QSurveyQuestion_SurveyQuestionListEnableOtherText]  DEFAULT ((0)) FOR [SurveyQuestionListEnableOtherText]
GO
ALTER TABLE [surveys].[QSurveyQuestion] ADD  CONSTRAINT [DF_QSurveyQuestion_SurveyQuestionListEnableRandomization]  DEFAULT ((0)) FOR [SurveyQuestionListEnableRandomization]
GO
ALTER TABLE [surveys].[QSurveyQuestion] ADD  CONSTRAINT [DF_QSurveyQuestion_SurveyQuestionNumberEnableStatistics]  DEFAULT ((0)) FOR [SurveyQuestionNumberEnableStatistics]
GO
ALTER TABLE [surveys].[QSurveyQuestion] ADD  CONSTRAINT [DF_QSurveyQuestion_SurveyQuestionIsNested]  DEFAULT ((0)) FOR [SurveyQuestionIsNested]
GO
ALTER TABLE [surveys].[QSurveyQuestion] ADD  CONSTRAINT [DF_QSurveyQuestion_SurveyQuestionListEnableGroupMembership]  DEFAULT ((0)) FOR [SurveyQuestionListEnableGroupMembership]
GO
ALTER TABLE [utilities].[TCollection] ADD  CONSTRAINT [DF_TCollection_CollectionIdentifier]  DEFAULT (newid()) FOR [CollectionIdentifier]
GO
ALTER TABLE [utilities].[Upgrade] ADD  CONSTRAINT [DF_Upgrade_UpgradeIdentifier]  DEFAULT (newid()) FOR [UpgradeIdentifier]
GO
ALTER TABLE [billing].[TOrderItem]  WITH CHECK ADD  CONSTRAINT [CK_TOrderItem_OrderItemQuantity_Positive] CHECK  (([OrderItemQuantity]>(0)))
GO
ALTER TABLE [billing].[TOrderItem] CHECK CONSTRAINT [CK_TOrderItem_OrderItemQuantity_Positive]
GO
ALTER TABLE [issues].[QIssueFileRequirement]  WITH CHECK ADD  CONSTRAINT [CK_RequestedFrom] CHECK  (([RequestedFrom]='Other' OR [RequestedFrom]='Candidate'))
GO
ALTER TABLE [issues].[QIssueFileRequirement] CHECK CONSTRAINT [CK_RequestedFrom]
GO
ALTER TABLE [resources].[Upload]  WITH NOCHECK ADD  CONSTRAINT [CHK_Upload_ContentSize] CHECK  ((NOT ([UploadType]='Lotus File' OR [UploadType]='CMDS File') OR [ContentSize] IS NOT NULL AND [ContentSize]>(0)))
GO
ALTER TABLE [resources].[Upload] CHECK CONSTRAINT [CHK_Upload_ContentSize]
GO
ALTER TABLE [utilities].[TCollectionItem]  WITH CHECK ADD  CONSTRAINT [CK_TCollectionItem_ItemColor] CHECK  (([ItemColor]='dark' OR [ItemColor]='light' OR [ItemColor]='danger' OR [ItemColor]='warning' OR [ItemColor]='info' OR [ItemColor]='success' OR [ItemColor]='primary' OR [ItemColor]='default'))
GO
ALTER TABLE [utilities].[TCollectionItem] CHECK CONSTRAINT [CK_TCollectionItem_ItemColor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [achievements].[RefreshAchievementOrganization]

AS 

INSERT INTO achievements.TAchievementOrganization
(
    AchievementIdentifier
  , OrganizationIdentifier
)
SELECT DISTINCT
       ad.AchievementIdentifier
     , d.OrganizationIdentifier
FROM achievements.TAchievementDepartment AS ad
     INNER JOIN contacts.QGroup         AS d ON d.GroupIdentifier = ad.DepartmentIdentifier
     INNER JOIN accounts.QOrganization       AS t ON t.OrganizationIdentifier = d.OrganizationIdentifier
WHERE t.AccountClosed IS NULL
      AND NOT EXISTS
(
    SELECT *
    FROM achievements.TAchievementOrganization AS at
    WHERE at.OrganizationIdentifier = d.OrganizationIdentifier
          AND at.AchievementIdentifier = ad.AchievementIdentifier
);
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [assessments].[DenormalizeAttempt] (
    @Attempt UNIQUEIDENTIFIER
)
AS
BEGIN

    -- The denormalized competency attributes must be updated for each question answered.

    UPDATE
        assessments.QAttemptQuestion
    SET
        CompetencyItemIdentifier = QBankQuestion.CompetencyIdentifier
      , CompetencyItemLabel = VCompetency.CompetencyLabel
      , CompetencyItemCode = SUBSTRING(VCompetency.CompetencyCode, 1, 30)
      , CompetencyItemTitle = SUBSTRING(VCompetency.CompetencyTitle, 1, 300)
      , CompetencyAreaIdentifier = VCompetency.AreaIdentifier
      , CompetencyAreaLabel = VCompetency.AreaLabel
      , CompetencyAreaCode = SUBSTRING(VCompetency.AreaCode, 1, 30)
      , CompetencyAreaTitle = SUBSTRING(VCompetency.AreaTitle, 1, 300)
    FROM
        assessments.QAttemptQuestion
        INNER JOIN banks.QBankQuestion
            ON QBankQuestion.QuestionIdentifier = QAttemptQuestion.QuestionIdentifier
        LEFT JOIN standards.VCompetency
            ON VCompetency.CompetencyIdentifier = QBankQuestion.CompetencyIdentifier
    WHERE
        QAttemptQuestion.AttemptIdentifier = @Attempt

    UPDATE
        assessments.QAttemptOption
    SET
        CompetencyItemIdentifier = QBankOption.CompetencyIdentifier
      , CompetencyItemLabel = VCompetency.CompetencyLabel
      , CompetencyItemCode = SUBSTRING(VCompetency.CompetencyCode, 1, 30)
      , CompetencyItemTitle = SUBSTRING(VCompetency.CompetencyTitle, 1, 300)
      , CompetencyAreaIdentifier = VCompetency.AreaIdentifier
      , CompetencyAreaLabel = VCompetency.AreaLabel
      , CompetencyAreaCode = SUBSTRING(VCompetency.AreaCode, 1, 30)
      , CompetencyAreaTitle = SUBSTRING(VCompetency.AreaTitle, 1, 300)
    FROM
        assessments.QAttemptOption
        INNER JOIN banks.QBankOption
            ON QBankOption.QuestionIdentifier = QAttemptOption.QuestionIdentifier
               AND QBankOption.OptionKey = QAttemptOption.OptionKey
        LEFT JOIN standards.VCompetency
            ON VCompetency.CompetencyIdentifier = QBankOption.CompetencyIdentifier
    WHERE
        QAttemptOption.AttemptIdentifier = @Attempt

    -- The grade must be calculated.

    UPDATE assessments.QAttempt
    SET AttemptGrade = (CASE WHEN AttemptIsPassing = 1 THEN 'Pass' ELSE 'Fail' END)
    WHERE AttemptIdentifier = @Attempt
          AND
          (
              AttemptGrade IS NULL
              OR AttemptGrade <> (CASE WHEN AttemptIsPassing = 1 THEN 'Pass' ELSE 'Fail' END)
          )
          AND (AttemptGraded IS NOT NULL);

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [assessments].[SelectQuestionFeedbackForAnalysis]
    @QuestionIdentifier UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        question.QuestionIdentifier,
        comment.CommentIdentifier,
        attempt.AssessorUserIdentifier,
        comment.AuthorUserIdentifier,
        CAST(
            CASE 
                WHEN attempt.AssessorUserIdentifier IS NOT NULL 
                     AND comment.AuthorUserIdentifier IS NOT NULL 
                     AND attempt.AssessorUserIdentifier = comment.AuthorUserIdentifier 
                THEN 1 
                ELSE 0 
            END 
        AS bit) AS IsSameAssessor,
        CAST(form.FormThirdPartyAssessmentIsEnabled AS bit) AS IsFormThirdPartyAssessmentEnabled
    FROM assessments.QAttempt                    AS attempt
         INNER JOIN banks.QBankForm              AS form ON attempt.FormIdentifier = form.FormIdentifier
         INNER JOIN assessments.QAttemptQuestion AS question ON attempt.AttemptIdentifier = question.AttemptIdentifier
         INNER JOIN assets.QComment              AS comment ON question.QuestionIdentifier = comment.AssessmentQuestionIdentifier
                                                            AND question.AttemptIdentifier = comment.AssessmentAttemptIdentifier
    WHERE question.QuestionIdentifier = @QuestionIdentifier;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [contacts].[DeleteQGroup](@GroupIdentifier UNIQUEIDENTIFIER)
AS
BEGIN
    DELETE contacts.TGroupPermission WHERE GroupIdentifier = @GroupIdentifier;
    DELETE contacts.QGroupAddress WHERE GroupIdentifier = @GroupIdentifier;
    DELETE contacts.QGroup WHERE GroupIdentifier = @GroupIdentifier;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [contacts].[SavePersonField]
(
	@OrganizationIdentifier uniqueidentifier,
	@UserIdentifier uniqueidentifier,
	@FieldName varchar(100),
	@FieldValue nvarchar(max)
) 
as
begin

	declare @sql nvarchar(max) = null;
	declare @parameters nvarchar(max);

	if exists (select * from contacts.TPersonField where OrganizationIdentifier = @OrganizationIdentifier and UserIdentifier = @UserIdentifier and FieldName = @FieldName)
		set @sql = 'UPDATE contacts.TPersonField SET FieldValue = @Value WHERE UserIdentifier = @User AND OrganizationIdentifier = @Organization and FieldName = @Field';

	else
		set @sql = 'INSERT INTO contacts.TPersonField (OrganizationIdentifier,UserIdentifier,FieldIdentifier,FieldName,FieldValue) VALUES (@Organization,@User,NEWID(),@Field,@Value)';

	if @sql is not null
	begin
		set @parameters = N'@Field varchar(100), @Value nvarchar(max), @User uniqueidentifier, @Organization uniqueidentifier';  
		execute sp_executesql @sql, @parameters, @Field = @FieldName, @Value = @FieldValue, @User = @UserIdentifier, @Organization = @OrganizationIdentifier;
	end;

end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [contacts].[SelectDuplicateGroups](@OrganizationIdentifier uniqueidentifier)
as
with Duplicates
as (
   select QGroup.GroupName
        , count(*) as count
   from contacts.QGroup
        inner join accounts.QOrganization as Organization on Organization.OrganizationIdentifier = QGroup.OrganizationIdentifier
   where Organization.OrganizationIdentifier = @OrganizationIdentifier
         and QGroup.GroupType = 'Employer'
   group by QGroup.GroupName
   having count(*) > 1)
   , Duplicates2
as (select QGroup.GroupIdentifier
         , QGroup.GroupName
         , (
               select count(*)
               from contacts.VMembership
               where VMembership.GroupIdentifier = QGroup.GroupIdentifier
           )                                                                             as PersonCount
         , iif(
               Billing.Street1 is null
               and Billing.Street2 is null
               and Billing.City is null
               and Billing.Province is null
               and Billing.PostalCode is null
             , null
             , isnull(Billing.Street1, '') + ', ' + isnull(Billing.Street2 + ', ', '') + isnull(Billing.City + ', ', '') + isnull(Billing.Province + ', ', '')
               + isnull(Billing.PostalCode, ''))                                         as BillingAddress
         , iif(
               Shipping.Street1 is null
               and Shipping.Street2 is null
               and Shipping.City is null
               and Shipping.Province is null
               and Shipping.PostalCode is null
             , null
             , isnull(Shipping.Street1, '') + ', ' + isnull(Shipping.Street2 + ', ', '') + isnull(Shipping.City + ', ', '')
               + isnull(Shipping.Province + ', ', '') + isnull(Shipping.PostalCode, '')) as ShippingAddress
         , iif(
               Physical.Street1 is null
               and Physical.Street2 is null
               and Physical.City is null
               and Physical.Province is null
               and Physical.PostalCode is null
             , null
             , isnull(Physical.Street1, '') + ', ' + isnull(Physical.Street2 + ', ', '') + isnull(Physical.City + ', ', '')
               + isnull(Physical.Province + ', ', '') + isnull(Physical.PostalCode, '')) as PhysicalAddress
    from contacts.QGroup
         inner join accounts.QOrganization as Organization on Organization.OrganizationIdentifier = QGroup.OrganizationIdentifier
         inner join Duplicates on Duplicates.GroupName = QGroup.GroupName
         left join contacts.QGroupAddress  as Billing on Billing.GroupIdentifier = QGroup.GroupIdentifier
                                                         and Billing.AddressType = 'Billing'
         left join contacts.QGroupAddress  as Shipping on Shipping.GroupIdentifier = QGroup.GroupIdentifier
                                                          and Shipping.AddressType = 'Shipping'
         left join contacts.QGroupAddress  as Physical on Physical.GroupIdentifier = QGroup.GroupIdentifier
                                                          and Physical.AddressType = 'Physical'
    where Organization.OrganizationIdentifier = @OrganizationIdentifier
          and QGroup.GroupType = 'Employer')
select *
     , row_number() over (partition by GroupName
                          order by GroupName
                                 , PersonCount desc
                                 , GroupIdentifier
                         ) as GroupNumber
from Duplicates2;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [contacts].[SelectUnusedPeople](@OrganizationIdentifier UNIQUEIDENTIFIER, @ExcludeAdministrators BIT)
AS
SELECT u.UserIdentifier
     , p.OrganizationIdentifier
     , u.FullName
     , u.Email
     , p.PersonCode
FROM identities.[User] AS u
         INNER JOIN contacts.QPerson AS p ON p.UserIdentifier = u.UserIdentifier
         INNER JOIN accounts.QOrganization AS t ON t.OrganizationIdentifier = p.OrganizationIdentifier
WHERE t.OrganizationIdentifier = @OrganizationIdentifier

  AND u.UserIdentifier NOT IN ( SELECT CandidateIdentifier
                                FROM registrations.QRegistration )
  AND u.UserIdentifier NOT IN ( SELECT LearnerIdentifier
                                FROM records.QEnrollment )
  AND (@ExcludeAdministrators = 0 OR p.IsAdministrator = 0)
  AND (p.IsLearner = 1)
  AND u.UserIdentifier NOT IN ( SELECT UserIdentifier
                                FROM events.QEventAttendee );
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [courses].[RemoveActivity] (@ActivityId uniqueidentifier)
as
begin
    delete contents.TContent where ContainerIdentifier = @ActivityId
    delete courses.QCoursePrerequisite where ObjectIdentifier = @ActivityId
    delete courses.QActivityCompetency where ActivityIdentifier = @ActivityId
    delete courses.QActivity where ActivityIdentifier = @ActivityId
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [courses].[RemoveCourse] (@CourseId uniqueidentifier)
as
begin
    declare @UnitIds table (UnitId uniqueidentifier not null)
    declare @ModuleIds table (ModuleId uniqueidentifier not null)
    declare @ActivityIds table (ActivityId uniqueidentifier not null)

    insert @UnitIds select UnitIdentifier from courses.QUnit where CourseIdentifier = @CourseId
    insert @ModuleIds select ModuleIdentifier from courses.QModule where UnitIdentifier in (select UnitId from @UnitIds)
    insert @ActivityIds select ActivityIdentifier from courses.QActivity where ModuleIdentifier in (select ModuleId from @ModuleIds)

    delete contents.TContent where ContainerIdentifier = @CourseId
    delete contents.TContent where ContainerIdentifier in (select UnitId from @UnitIds)
    delete contents.TContent where ContainerIdentifier in (select ModuleId from @ModuleIds)
    delete contents.TContent where ContainerIdentifier in (select ActivityId from @ActivityIds)

    delete courses.QCourseEnrollment where CourseIdentifier = @CourseId
    delete courses.QCoursePrerequisite where CourseIdentifier = @CourseId

    delete courses.QActivityCompetency where ActivityIdentifier in (select ActivityId from @ActivityIds)
    delete courses.QActivity where ActivityIdentifier in (select ActivityId from @ActivityIds)
    delete courses.QModule where ModuleIdentifier in (select ModuleId from @ModuleIds)
    delete courses.QUnit where UnitIdentifier in (select UnitId from @UnitIds)
    delete courses.QCourse where CourseIdentifier = @CourseId
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [courses].[RemoveModule] (@ModuleId uniqueidentifier)
as
begin
    declare @ActivityIds table (ActivityId uniqueidentifier not null)

    insert @ActivityIds select ActivityIdentifier from courses.QActivity where ModuleIdentifier  = @ModuleId

    delete contents.TContent where ContainerIdentifier = @ModuleId
    delete contents.TContent where ContainerIdentifier in (select ActivityId from @ActivityIds)

    delete courses.QCoursePrerequisite where ObjectIdentifier = @ModuleId
    delete courses.QCoursePrerequisite where ObjectIdentifier in (select ActivityId from @ActivityIds)

    delete courses.QActivityCompetency where ActivityIdentifier in (select ActivityId from @ActivityIds)
    delete courses.QActivity where ActivityIdentifier in (select ActivityId from @ActivityIds)
    delete courses.QModule where ModuleIdentifier = @ModuleId
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [courses].[RemoveUnit] (@UnitId uniqueidentifier)
as
begin
    declare @ModuleIds table (ModuleId uniqueidentifier not null)
    declare @ActivityIds table (ActivityId uniqueidentifier not null)

    insert @ModuleIds select ModuleIdentifier from courses.QModule where UnitIdentifier = @UnitId
    insert @ActivityIds select ActivityIdentifier from courses.QActivity where ModuleIdentifier in (select ModuleId from @ModuleIds)

    delete contents.TContent where ContainerIdentifier = @UnitId
    delete contents.TContent where ContainerIdentifier in (select ModuleId from @ModuleIds)
    delete contents.TContent where ContainerIdentifier in (select ActivityId from @ActivityIds)

    delete courses.QCoursePrerequisite where ObjectIdentifier = @UnitId
    delete courses.QCoursePrerequisite where ObjectIdentifier in (select ModuleId from @ModuleIds)
    delete courses.QCoursePrerequisite where ObjectIdentifier in (select ActivityId from @ActivityIds)

    delete courses.QActivityCompetency where ActivityIdentifier in (select ActivityId from @ActivityIds)
    delete courses.QActivity where ActivityIdentifier in (select ActivityId from @ActivityIds)
    delete courses.QModule where ModuleIdentifier in (select ModuleId from @ModuleIds)
    delete courses.QUnit where UnitIdentifier = @UnitId
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[DeleteUnusedDepartmentProfileCompetencies1]
(@ProfileStandardIdentifier uniqueidentifier)
AS
    DELETE
    standards.DepartmentProfileCompetency
    FROM
        standards.DepartmentProfileCompetency
    WHERE
        CompetencyStandardIdentifier NOT IN
        (
            SELECT
                pc.CompetencyStandardIdentifier
            FROM
                custom_cmds.ProfileCompetency pc
                INNER JOIN
                custom_cmds.DepartmentProfile dp ON dp.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
            WHERE
                dp.DepartmentIdentifier   = DepartmentProfileCompetency.DepartmentIdentifier
              AND dp.ProfileStandardIdentifier = DepartmentProfileCompetency.ProfileStandardIdentifier
        )
      AND ProfileStandardIdentifier = @ProfileStandardIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[DeleteUnusedDepartmentProfileCompetencies2]
(@DepartmentIdentifier uniqueidentifier)
AS
    DELETE
    standards.DepartmentProfileCompetency
    FROM
        standards.DepartmentProfileCompetency
    WHERE
        CompetencyStandardIdentifier NOT IN
        (
            SELECT
                pc.CompetencyStandardIdentifier
            FROM
                custom_cmds.ProfileCompetency pc
                INNER JOIN
                custom_cmds.DepartmentProfile dp ON dp.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
            WHERE
                dp.DepartmentIdentifier   = DepartmentProfileCompetency.DepartmentIdentifier
              AND dp.ProfileStandardIdentifier = DepartmentProfileCompetency.ProfileStandardIdentifier
        )
      AND DepartmentIdentifier = @DepartmentIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[InsertDepartmentProfileCompetencies]
(
    @DepartmentIdentifier uniqueidentifier
  , @ProfileStandardIdentifier uniqueidentifier
)
AS
    INSERT standards.DepartmentProfileCompetency
    (
        DepartmentIdentifier, ProfileStandardIdentifier, CompetencyStandardIdentifier, IsCritical
    )
    SELECT
        @DepartmentIdentifier
      , @ProfileStandardIdentifier
      , CompetencyStandardIdentifier
      , 0
    FROM
        custom_cmds.ProfileCompetency
    WHERE
        ProfileStandardIdentifier = @ProfileStandardIdentifier
      AND CompetencyStandardIdentifier NOT IN
          (
              SELECT
                  CompetencyStandardIdentifier
              FROM
                  custom_cmds.DepartmentProfileCompetency
              WHERE
                  DepartmentIdentifier = @DepartmentIdentifier
                AND ProfileStandardIdentifier = @ProfileStandardIdentifier
          );
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[InsertDepartmentProfileCompetenciesByProfile](
    @ProfileStandardIdentifier uniqueidentifier
)
AS
BEGIN
    insert standards.DepartmentProfileCompetency
    (
        DepartmentIdentifier
        , ProfileStandardIdentifier
        , CompetencyStandardIdentifier
        , IsCritical
    )
    select dp.DepartmentIdentifier
            , pc.ProfileStandardIdentifier
            , pc.CompetencyStandardIdentifier
            , 0
    from custom_cmds.ProfileCompetency            as pc
            inner join custom_cmds.DepartmentProfile as dp on dp.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
    where pc.ProfileStandardIdentifier = @ProfileStandardIdentifier
            and not exists (
                                select *
                                from standards.DepartmentProfileCompetency
                                where DepartmentIdentifier = dp.DepartmentIdentifier
                                    and ProfileStandardIdentifier = @ProfileStandardIdentifier
                                    and CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                            );
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[PublishGlobalModule] 
AS
WITH CTE AS
    (
        SELECT
            R.AchievementIdentifier
          , T.OrganizationIdentifier
        FROM
            achievements.QAchievement AS R
            INNER JOIN
            accounts.QOrganization  AS T ON T.OrganizationIdentifier = R.OrganizationIdentifier
        WHERE
            R.AchievementLabel = 'Module'
          AND T.OrganizationCode = 'cmds'
    )
INSERT INTO achievements.TAchievementOrganization
(
    AchievementIdentifier, OrganizationIdentifier
)
SELECT
    AchievementIdentifier
  , Organization.OrganizationIdentifier
FROM
    CTE
    CROSS JOIN accounts.QOrganization AS Organization
WHERE
    Organization.ParentOrganizationIdentifier = CTE.OrganizationIdentifier
  AND Organization.AccountClosed IS NULL
  AND NOT EXISTS
(
    SELECT
        *
    FROM
        achievements.TAchievementOrganization AS X
    WHERE
        X.AchievementIdentifier = CTE.AchievementIdentifier
      AND X.OrganizationIdentifier = Organization.OrganizationIdentifier
);
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [custom_cmds].[QSelectUserStatusHome]
(
    @UserIdentifier uniqueidentifier
  , @OrganizationIdentifier uniqueidentifier
  , @PrimaryOnly int
)
as
    begin

        declare @ProfileIdentifier uniqueidentifier = (
                                                          select max(ProfileStandardIdentifier)
                                                          from custom_cmds.Employment
                                                          where UserIdentifier = @UserIdentifier
                                                                and OrganizationIdentifier = @OrganizationIdentifier
                                                      );

        with cte as (
                        select min(DepartmentProfileCompetency.Criticality) as CompetencyCriticality
                             , Competency.StandardIdentifier                as CompetencyKey
                             , Competency.Code                              as CompetencyNumber
                             , Competency.ContentTitle                      as CompetencyTitle
                             , max(cast(UserCompetency.IsValidated as int)) as ValidationIsValidated
                             , max(   case
                                          when UserProfile.IsRequired = 1 then
                                              1
                                          else
                                              0
                                      end
                                  )                                         as ValidationIsMandatory
                             , max(   case
                                          when UserProfile.ProfileStandardIdentifier = @ProfileIdentifier then
                                              1
                                          else
                                              0
                                      end
                                  )                                         as ProfileIsPrimary
                             , UserCompetency.ValidationStatus              as ValidationStatus
                        from standards.DepartmentProfileUser                    as UserProfile
                             inner join standards.StandardContainment           as ProfileCompetency on ProfileCompetency.ParentStandardIdentifier = UserProfile.ProfileStandardIdentifier
                             inner join standards.StandardValidation            as UserCompetency on UserCompetency.UserIdentifier = UserProfile.UserIdentifier
                                                                                                     and UserCompetency.StandardIdentifier = ProfileCompetency.ChildStandardIdentifier
                             inner join standards.Standard                      as Competency on Competency.StandardIdentifier = UserCompetency.StandardIdentifier
                             inner join custom_cmds.DepartmentProfileCompetency as DepartmentProfileCompetency on DepartmentProfileCompetency.CompetencyStandardIdentifier = ProfileCompetency.ChildStandardIdentifier
                                                                                                                  and DepartmentProfileCompetency.DepartmentIdentifier = UserProfile.DepartmentIdentifier
                                                                                                                  and DepartmentProfileCompetency.ProfileStandardIdentifier = UserProfile.ProfileStandardIdentifier
                             inner join contacts.QGroup                         as Department on Department.GroupIdentifier = UserProfile.DepartmentIdentifier
                        where Competency.IsHidden = 0
                              and UserProfile.UserIdentifier = @UserIdentifier
                              and (
                                      @PrimaryOnly = 0
                                      or UserProfile.IsPrimary = 1
                                  )
                        group by Competency.StandardIdentifier
                               , Competency.Code
                               , Competency.ContentTitle
                               , UserCompetency.ValidationStatus
                    )
        select 'Critical'                    as Name
             , count(distinct CompetencyKey) as Value
        from cte
        where cte.CompetencyCriticality = 'Critical'
              and cte.ValidationIsMandatory = 1
              and (
                      @PrimaryOnly = 0
                      or cte.ProfileIsPrimary = 1
                  )
        union
        select 'Critical Validated'          as Name
             , count(distinct CompetencyKey) as Value
        from cte
        where cte.CompetencyCriticality = 'Critical'
              and cte.ValidationIsMandatory = 1
              and (
                      @PrimaryOnly = 0
                      or cte.ProfileIsPrimary = 1
                  )
              and (
                      cte.ValidationStatus = 'Validated'
                      or (
                             cte.ValidationStatus = 'Not Applicable'
                             and cte.ValidationIsValidated = 1
                         )
                  )
        union
        select 'Critical Submitted'          as Name
             , count(distinct CompetencyKey) as Value
        from cte
        where cte.CompetencyCriticality = 'Critical'
              and cte.ValidationIsMandatory = 1
              and (
                      @PrimaryOnly = 0
                      or cte.ProfileIsPrimary = 1
                  )
              and cte.ValidationStatus = 'Submitted'
        union
        select 'Non-Critical'                as Name
             , count(distinct CompetencyKey) as Value
        from cte
        where cte.CompetencyCriticality = 'Non-Critical'
              and cte.ValidationIsMandatory = 1
              and (
                      @PrimaryOnly = 0
                      or cte.ProfileIsPrimary = 1
                  )
        union
        select 'Non-Critical Validated'      as Name
             , count(distinct CompetencyKey) as Value
        from cte
        where cte.CompetencyCriticality = 'Non-Critical'
              and cte.ValidationIsMandatory = 1
              and (
                      @PrimaryOnly = 0
                      or cte.ProfileIsPrimary = 1
                  )
              and (
                      cte.ValidationStatus = 'Validated'
                      or (
                             cte.ValidationStatus = 'Not Applicable'
                             and cte.ValidationIsValidated = 1
                         )
                  )
        union
        select 'Non-Critical Submitted'      as Name
             , count(distinct CompetencyKey) as Value
        from cte
        where cte.CompetencyCriticality = 'Non-Critical'
              and cte.ValidationIsMandatory = 1
              and (
                      @PrimaryOnly = 0
                      or cte.ProfileIsPrimary = 1
                  )
              and cte.ValidationStatus = 'Submitted';

    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[RefreshQInvoice]
(
    @FromDate datetime,
    @ThruDate datetime,
    @UnitPricePerPeriodClassA decimal(11,2)
)
AS
BEGIN
    DECLARE @Year  int = YEAR(@FromDate),
            @Month int = MONTH(@FromDate);

    TRUNCATE TABLE custom_cmds.QCmdsInvoiceFee;
    TRUNCATE TABLE custom_cmds.QCmdsInvoiceItem;
    TRUNCATE TABLE custom_cmds.QCmdsInvoice;

    INSERT INTO custom_cmds.QCmdsInvoiceFee
    (
        UserEmail,
        CompanyName,
        FromDate,
        ThruDate,
        BillingClassification,
        PricePerUserPerPeriodPerCompany,
        SharedCompanyCount
    )
    SELECT 
        Email,
        CompanyName,
        @FromDate,
        @ThruDate,
        (
            SELECT MIN(BillingClassification)
            FROM custom_cmds.BillableUserSummary x
            WHERE x.UserIdentifier = users.UserIdentifier
              AND x.OrganizationIdentifier = users.OrganizationIdentifier
        ) AS BillingClassification,
        ROUND(CAST((
            CASE (
                SELECT MIN(BillingClassification)
                FROM custom_cmds.BillableUserSummary x
                WHERE x.UserIdentifier = users.UserIdentifier
                  AND x.OrganizationIdentifier = users.OrganizationIdentifier
            )
            WHEN 'A' THEN @UnitPricePerPeriodClassA
            ELSE 0
            END
        ) AS decimal(11,2)) / CompanyCount, 2) AS PricePerUserPerPeriodPerCompany,
        CompanyCount AS SharedCompanyCount
    FROM custom_cmds.BillableUserSummary AS users
    WHERE NOT EXISTS (
        SELECT *
        FROM custom_cmds.QCmdsInvoiceFee
        WHERE UserEmail = users.Email
          AND CompanyName = users.CompanyName
          AND FromDate = @FromDate
          AND ThruDate = @ThruDate
    );

    INSERT INTO custom_cmds.QCmdsInvoice
    (
        CompanyName,
        Currency,
        InvoiceNumber,
        DateSubmitted,
        PeriodYear,
        PeriodMonth,
        TaxAmount,
        TaxRate,
        TaxType,
        TotalAmount,
        TotalAmountTaxable
    )
    SELECT 
        fees.CompanyName,
        'CAD',
        CAST(@Year AS nvarchar(4)) + '-' + REPLACE(STR(ROW_NUMBER() OVER (ORDER BY companies.CompanyTitle), 3), SPACE(1), '0'),
        GETUTCDATE(),
        @Year,
        @Month,
        0,
        5,
        'GST',
        0,
        0
    FROM custom_cmds.QCmdsInvoiceFee fees
    INNER JOIN accounts.QOrganization companies ON companies.CompanyTitle = fees.CompanyName
    WHERE NOT EXISTS (
        SELECT *
        FROM custom_cmds.QCmdsInvoice
        WHERE CompanyName = fees.CompanyName
          AND PeriodYear = @Year
          AND PeriodMonth = @Month
    )
    GROUP BY fees.CompanyName, companies.CompanyTitle;

    INSERT INTO custom_cmds.QCmdsInvoiceItem
    (
        InvoiceKey,
        Sequence,
        Quantity,
        UnitName,
        MultiplierQuantity,
        MultiplierUnitName,
        Category,
        Description,
        UnitPrice,
        IsTaxable
    )
    SELECT 
        sales.InvoiceKey,
        ROW_NUMBER() OVER (PARTITION BY sales.InvoiceKey ORDER BY SharedCompanyCount, BillingClassification) AS Sequence,
        COUNT(DISTINCT fees.UserEmail) AS Quantity,
        'Class ' + BillingClassification + ' Users' AS UnitName,
        1 AS MultiplierQuantity,
        'Months' AS MultiplierUnitName,
        CASE 
            WHEN SharedCompanyCount = 1 THEN 'Dedicated (100%)'
            ELSE 'Shared (' + CAST(CAST(ROUND(100.0 / SharedCompanyCount, 0) AS INT) AS nvarchar(4)) + '%)'
        END AS Category,
        CASE 
            WHEN SharedCompanyCount = 1 THEN 'Dedicated (100%)'
            ELSE 'Shared (' + CAST(CAST(ROUND(100.0 / SharedCompanyCount, 0) AS INT) AS nvarchar(4)) + '%)'
        END AS Description,
        PricePerUserPerPeriodPerCompany,
        1
    FROM custom_cmds.QCmdsInvoiceFee fees
    INNER JOIN accounts.QOrganization companies ON companies.CompanyTitle = fees.CompanyName
    INNER JOIN custom_cmds.QCmdsInvoice sales ON sales.CompanyName = fees.CompanyName
    WHERE sales.PeriodYear = @Year
      AND sales.PeriodMonth = @Month
      AND sales.InvoiceKey NOT IN (
          SELECT InvoiceKey FROM custom_cmds.QCmdsInvoiceItem
      )
    GROUP BY sales.InvoiceKey, sales.CompanyName, BillingClassification, SharedCompanyCount, PricePerUserPerPeriodPerCompany
    ORDER BY sales.InvoiceKey, sales.CompanyName, SharedCompanyCount DESC, BillingClassification;

    UPDATE custom_cmds.QCmdsInvoice
    SET TotalAmountTaxable = ISNULL(a.TotalAmountTaxable, 0),
        TaxAmount = ROUND(ISNULL(a.TotalAmountTaxable, 0) * sales.TaxRate / 100.0, 2),
        TotalAmount = ISNULL(a.TotalAmount, 0)
    FROM custom_cmds.QCmdsInvoice sales
    OUTER APPLY (
        SELECT 
            SUM(ROUND(Quantity * UnitPrice, 2)) AS TotalAmount,
            SUM(CASE 
                    WHEN IsTaxable = 1 THEN ROUND(Quantity * UnitPrice, 2)
                    ELSE 0
                END) AS TotalAmountTaxable
        FROM custom_cmds.QCmdsInvoiceItem
        WHERE InvoiceKey = sales.InvoiceKey
    ) a
    WHERE sales.PeriodYear = @Year
      AND sales.PeriodMonth = @Month;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[RefreshQUserStatus]
AS
BEGIN
    EXEC custom_cmds.RefreshQUserStatus_Step1;
    EXEC custom_cmds.RefreshQUserStatus_Step2;
    EXEC custom_cmds.RefreshQUserStatus_Step3;
    CREATE NONCLUSTERED INDEX IX_QUserStatus1 ON custom_cmds.QUserStatus (OrganizationIdentifier);
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [custom_cmds].[RefreshQUserStatus_Step1]
as
    begin
        drop table if exists custom_cmds.BUserDepartmentCompetencyStatus;

        create table custom_cmds.BUserDepartmentCompetencyStatus
        (
            UserIdentifier uniqueidentifier not null
          , DepartmentIdentifier uniqueidentifier not null
          , CompetencyStandardIdentifier uniqueidentifier not null
          , ValidationStatus varchar(32) not null
          , IsValidated bit null
          , IsCritical bit null
          , IsRequired bit null
          , IsPrimary bit null
          , JoinIdentifier uniqueidentifier not null primary key
                default (newid())
        );

        drop table if exists custom_cmds.BUserCredentialStatus;

        create table custom_cmds.BUserCredentialStatus
        (
            UserIdentifier uniqueidentifier not null
          , AchievementIdentifier uniqueidentifier not null
          , OrganizationIdentifier uniqueidentifier not null
          , CredentialStatus varchar(32) null
          , AchievementLabel varchar(64) not null
          , IsGlobal bit not null
          ,
          primary key (
                          UserIdentifier
                        , AchievementIdentifier
                      )
        );

        drop table if exists custom_cmds.BUserProfile;

        create table custom_cmds.BUserProfile
        (
            OrganizationIdentifier uniqueidentifier not null
          , CompanyName varchar(128) not null
          , DepartmentIdentifier uniqueidentifier not null
          , DepartmentName varchar(128) not null
          , UserIdentifier uniqueidentifier not null
          , UserFullName varchar(128) not null
          , PrimaryProfileIdentifier uniqueidentifier null
          , PrimaryProfileNumber varchar(50) null
          , PrimaryProfileTitle varchar(max) null
          , IsCritical bit not null
          , [Sequence] int not null
          , Heading varchar(128) not null
          , JoinIdentifier uniqueidentifier not null primary key
                default (newid())
        );

        create unique nonclustered index IX_BUserProfile_Unique
        on custom_cmds.BUserProfile (
                                        DepartmentIdentifier
                                      , OrganizationIdentifier
                                      , UserIdentifier
                                      , [Sequence]
                                    );

        drop table if exists custom_cmds.BUserEmployment;

        create table custom_cmds.BUserEmployment
        (
            OrganizationIdentifier uniqueidentifier not null
          , CompanyName varchar(128) not null
          , DepartmentIdentifier uniqueidentifier not null
          , DepartmentName varchar(128) not null
          , UserIdentifier uniqueidentifier not null
          , UserName varchar(128) not null
          , JoinIdentifier uniqueidentifier not null primary key
                default (newid())
        );

        create unique nonclustered index IX_BUserEmployment_Unique
        on custom_cmds.BUserEmployment (
                                           DepartmentIdentifier
                                         , OrganizationIdentifier
                                         , UserIdentifier
                                       );

        drop table if exists custom_cmds.QUserStatus;

        create table custom_cmds.QUserStatus
        (
            UserStatusKey int identity(1, 1) not null
          , AsAt datetime not null
          , OrganizationIdentifier uniqueidentifier not null
          , OrganizationName varchar(256) not null
          , DepartmentIdentifier uniqueidentifier not null
          , DepartmentName varchar(256) not null
          , UserIdentifier uniqueidentifier not null
          , UserFullName varchar(128) not null
          , PrimaryProfileIdentifier uniqueidentifier null
          , PrimaryProfileNumber varchar(50) null
          , PrimaryProfileTitle varchar(256) null
          , ItemNumber int not null
          , ItemName varchar(64) not null
          , CountRQ_Primary int null
          , CountVAVNCP_Primary int null
          , Score_Primary decimal(5, 2) null
          , CountEX_Primary int null
          , CountNC_Primary int null
          , CountNA_Primary int null
          , CountNT_Primary int null
          , CountSA_Primary int null
          , CountSV_Primary int null
          , CountVA_Primary int null
          , CountRQ_Mandatory int null
          , CountVAVNCP_Mandatory int null
          , CountEX_Mandatory int null
          , CountNC_Mandatory int null
          , CountNA_Mandatory int null
          , CountNT_Mandatory int null
          , CountSA_Mandatory int null
          , CountSV_Mandatory int null
          , CountVAVN_Mandatory int null
          , Score_Mandatory decimal(5, 2) null
          , CountEX int null
          , CountNC int null
          , CountNA int null
          , CountNT int null
          , CountSA int null
          , CountSV int null
          , CountVA int null
          , CountRQ int null
          , CountVAVNCP int null
          , Score decimal(5, 2) null
          , constraint PK_QUserStatus
                primary key clustered (UserStatusKey asc)
        );

    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[RefreshQUserStatus_Step2]
AS
BEGIN

    -- Drop and create temporary table.
    IF OBJECT_ID('tempdb..#ValidatedCompetencies') IS NOT NULL
        DROP TABLE #ValidatedCompetencies;

    CREATE TABLE #ValidatedCompetencies
    (
        UserIdentifier     UNIQUEIDENTIFIER NOT NULL,
        StandardIdentifier UNIQUEIDENTIFIER NOT NULL,
        ValidationStatus   VARCHAR(40)      NULL,
        IsValidated        BIT              NOT NULL DEFAULT (0),
        PRIMARY KEY (UserIdentifier, StandardIdentifier)
    );

    -- Load temporary table.
    INSERT INTO #ValidatedCompetencies (UserIdentifier, StandardIdentifier, ValidationStatus, IsValidated)
    SELECT V.UserIdentifier
         , V.StandardIdentifier
         , V.ValidationStatus
         , CASE
        WHEN V.ValidationStatus IN ('Validated', 'Not Applicable') AND V.IsValidated = 1 THEN 1
        ELSE 0
        END
    FROM standards.StandardValidation AS V WITH (NOLOCK)
             INNER JOIN identities.[User] AS U WITH (NOLOCK)
                        ON V.UserIdentifier = U.UserIdentifier AND U.UtcArchived IS NULL
             INNER JOIN [standard].QStandard AS S WITH (NOLOCK)
                        ON V.StandardIdentifier = S.StandardIdentifier AND S.IsHidden = 0

    GROUP BY V.UserIdentifier, V.StandardIdentifier, V.ValidationStatus,
             CASE
                 WHEN V.ValidationStatus IN
                      ('Validated',
                       'Not Applicable') AND
                      V.IsValidated = 1 THEN 1
                 ELSE 0
                 END;

    -- Load buffer table.
    INSERT INTO custom_cmds.BUserDepartmentCompetencyStatus ( UserIdentifier, DepartmentIdentifier
                                                            , CompetencyStandardIdentifier, ValidationStatus
                                                            , IsValidated, IsCritical, IsRequired, IsPrimary)
    SELECT U.UserIdentifier
         , U.DepartmentIdentifier
         , V.StandardIdentifier
         , V.ValidationStatus
         , V.IsValidated
         , MAX(CONVERT(INT, DPC.IsCritical)) AS IsCritical
         , MAX(CONVERT(INT, U.IsRequired))   AS IsRequired
         , MAX(CONVERT(INT, U.IsPrimary))    AS IsPrimary
    FROM standards.DepartmentProfileUser AS U WITH (NOLOCK)
             INNER JOIN standards.Standard AS P WITH (NOLOCK)
                        ON P.StandardIdentifier = U.ProfileStandardIdentifier
             INNER JOIN standards.DepartmentProfileCompetency AS DPC WITH (NOLOCK)
                        ON DPC.DepartmentIdentifier = U.DepartmentIdentifier
                            AND DPC.ProfileStandardIdentifier = U.ProfileStandardIdentifier
             INNER JOIN standards.StandardContainment AS PC WITH (NOLOCK)
                        ON PC.ParentStandardIdentifier = U.ProfileStandardIdentifier
                            AND PC.ChildStandardIdentifier = DPC.CompetencyStandardIdentifier
             INNER JOIN #ValidatedCompetencies AS V WITH (NOLOCK)
                        ON V.UserIdentifier = U.UserIdentifier
                            AND V.StandardIdentifier = PC.ChildStandardIdentifier
             INNER JOIN standards.Standard AS Competency WITH (NOLOCK)
                        ON Competency.StandardIdentifier = V.StandardIdentifier
                            AND Competency.IsHidden = 0
    WHERE NOT EXISTS
              ( SELECT *
                FROM contacts.QPerson AS EmployeePerson WITH (NOLOCK)
                         INNER JOIN accounts.QOrganization AS T WITH (NOLOCK)
                                    ON T.OrganizationIdentifier = EmployeePerson.OrganizationIdentifier
                WHERE EmployeePerson.UserIdentifier = U.UserIdentifier
                  AND P.OrganizationIdentifier = T.OrganizationIdentifier
                  AND (
                    EmployeePerson.UserAccessGranted IS NULL
                    ) )
    GROUP BY U.UserIdentifier, U.DepartmentIdentifier, V.StandardIdentifier, V.ValidationStatus, V.IsValidated;

    INSERT INTO custom_cmds.BUserCredentialStatus
    ( UserIdentifier
    , AchievementIdentifier
    , OrganizationIdentifier
    , CredentialStatus
    , AchievementLabel
    , IsGlobal)
    SELECT ec.UserIdentifier
         , ec.AchievementIdentifier
         , ec.OrganizationIdentifier
         , ec.CredentialStatus
         , ec.AchievementLabel
         , CASE
        WHEN AchievementVisibility = 'Enterprise'
            THEN
            1
        ELSE
            0
        END AS IsGlobal
    FROM custom_cmds.VCmdsCredential AS ec WITH (NOLOCK)
    WHERE ec.CredentialNecessity = 'Mandatory';

    INSERT INTO custom_cmds.BUserProfile
    ( OrganizationIdentifier
    , CompanyName
    , DepartmentIdentifier
    , DepartmentName
    , UserIdentifier
    , UserFullName
    , PrimaryProfileIdentifier
    , PrimaryProfileNumber
    , PrimaryProfileTitle
    , IsCritical
    , Sequence
    , Heading)
    SELECT Company.OrganizationIdentifier
         , Company.CompanyName
         , Department.DepartmentIdentifier
         , Department.DepartmentName
         , Person.UserIdentifier
         , isnull(QPerson.[FullName], Person.FullName)
         , Profile.ProfileStandardIdentifier
         , Profile.ProfileNumber
         , Profile.ProfileTitle
         , SubTypes.IsCritical
         , SubTypes.Sequence
         , SubTypes.Heading
    FROM contacts.Membership WITH (NOLOCK)
             INNER JOIN identities.[User] AS Person WITH (NOLOCK)
                        ON Person.UserIdentifier = Membership.UserIdentifier
                            AND Person.UtcArchived IS NULL
                            AND Person.AccessGrantedToCmds = 1

             INNER JOIN identities.Department WITH (NOLOCK)
                        ON Department.DepartmentIdentifier = Membership.GroupIdentifier

             INNER JOIN accounts.QOrganization AS Company WITH (NOLOCK)
                        ON Company.OrganizationIdentifier = Department.OrganizationIdentifier

             INNER JOIN contacts.QPerson ON QPerson.UserIdentifier = Person.UserIdentifier AND
                                            QPerson.OrganizationIdentifier = Company.OrganizationIdentifier

             LEFT JOIN( standards.DepartmentProfileUser AS Employment WITH (NOLOCK)
        INNER JOIN custom_cmds.Profile WITH (NOLOCK)
                        ON Profile.ProfileStandardIdentifier = Employment.ProfileStandardIdentifier
                            AND Employment.IsPrimary = 1 )
                      ON Employment.UserIdentifier = Person.UserIdentifier
                          AND Employment.DepartmentIdentifier = Membership.GroupIdentifier
             CROSS JOIN
         ( SELECT 3                       AS Sequence
                , 'Critical Competencies' AS Heading
                , 1                       AS IsCritical
           UNION ALL
           SELECT 4
                , 'Non-Critical Competencies'
                , 0 AS IsCritical ) AS SubTypes
    WHERE Person.UtcArchived IS NULL;

    INSERT INTO custom_cmds.BUserEmployment
    ( OrganizationIdentifier
    , CompanyName
    , DepartmentIdentifier
    , DepartmentName
    , UserIdentifier
    , UserName)
    SELECT companies.OrganizationIdentifier
         , companies.CompanyName
         , d.DepartmentIdentifier
         , d.DepartmentName
         , contacts.UserIdentifier
         , isnull(QPerson.[FullName], contacts.FullName)
    FROM contacts.Membership WITH (NOLOCK)
             INNER JOIN identities.[User] AS contacts WITH (NOLOCK)
                        ON Membership.UserIdentifier = contacts.UserIdentifier
                            AND contacts.UtcArchived IS NULL
                            AND contacts.AccessGrantedToCmds = 1

             INNER JOIN identities.Department AS d WITH (NOLOCK)
                        ON d.DepartmentIdentifier = Membership.GroupIdentifier

             INNER JOIN accounts.QOrganization AS companies WITH (NOLOCK)
                        ON companies.OrganizationIdentifier = d.OrganizationIdentifier

             INNER JOIN contacts.QPerson ON QPerson.UserIdentifier = contacts.UserIdentifier AND
                                            QPerson.OrganizationIdentifier = companies.OrganizationIdentifier

    WHERE contacts.UtcArchived IS NULL;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[RefreshQUserStatus_Step3]
AS
    BEGIN
        SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

        SET DEADLOCK_PRIORITY LOW;

        INSERT custom_cmds.QUserStatus
            (
                AsAt
              , OrganizationIdentifier
              , OrganizationName
              , DepartmentIdentifier
              , DepartmentName
              , UserIdentifier
              , UserFullName
              , PrimaryProfileIdentifier
              , PrimaryProfileNumber
              , PrimaryProfileTitle
              , ItemNumber
              , ItemName
              , CountRQ_Primary
              , CountVAVNCP_Primary
              , Score_Primary
              , CountEX_Primary
              , CountNC_Primary
              , CountNA_Primary
              , CountNT_Primary
              , CountSA_Primary
              , CountSV_Primary
              , CountVA_Primary
              , CountRQ_Mandatory
              , CountVAVNCP_Mandatory
              , Score_Mandatory
              , CountEX_Mandatory
              , CountNC_Mandatory
              , CountNA_Mandatory
              , CountNT_Mandatory
              , CountSA_Mandatory
              , CountSV_Mandatory
              , CountVAVN_Mandatory
              , CountRQ
              , CountVAVNCP
              , Score
              , CountEX
              , CountNC
              , CountNA
              , CountNT
              , CountSA
              , CountSV
              , CountVA
            )
               SELECT
                   GETUTCDATE()
                 , OrganizationIdentifier
                 , CompanyName
                 , DepartmentIdentifier
                 , DepartmentName
                 , UserIdentifier
                 , UserName
                 , PrimaryProfileIdentifier
                 , PrimaryProfileNumber
                 , PrimaryProfileTitle
                 , Sequence
                 , Heading
                 , PrimaryRequired
                 , PrimarySatisfied
                 , PrimaryScore
                 , PrimaryExpired
                 , PrimaryNotCompleted
                 , PrimaryNotApplicable
                 , PrimaryNeedsTraining
                 , PrimarySelfAssessed
                 , PrimarySubmitted
                 , PrimaryValidated
                 , ComplianceRequired
                 , ComplianceSatisfied
                 , ComplianceScore
                 , ComplianceExpired
                 , ComplianceNotCompleted
                 , ComplianceNotApplicable
                 , ComplianceNeedsTraining
                 , ComplianceSelfAssessed
                 , ComplianceSubmitted
                 , ComplianceValidated
                 , PrimaryRequired + SecondaryRequired
                 , Satisfied
                 , Score
                 , Expired
                 , NotCompleted
                 , NotApplicable
                 , NeedsTraining
                 , SelfAssessed
                 , Submitted
                 , Validated
               FROM
                   custom_cmds.VUserStatus_Step3 WITH (NOLOCK);

    END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[RefreshTUserStatus]
(
    @OrganizationIdentifier     uniqueidentifier
  , @DepartmentIdentifier uniqueidentifier
  , @UserIdentifier       uniqueidentifier
)
AS
BEGIN

    DROP TABLE IF EXISTS #OrganizationUserCredential;
    DROP TABLE IF EXISTS #OrganizationUserCompetency;
    DROP TABLE IF EXISTS #UserCompetency;

    DECLARE @Now datetimeoffset = SYSDATETIMEOFFSET();
    DECLARE @MaxAsAt datetimeoffset = (
        SELECT
            MAX(AsAt)
        FROM
            custom_cmds.TUserStatus
        WHERE
            OrganizationIdentifier = @OrganizationIdentifier
            AND (@DepartmentIdentifier IS NULL OR DepartmentIdentifier = @DepartmentIdentifier)
            AND (@UserIdentifier IS NULL OR UserIdentifier = @UserIdentifier)
    );

    IF @MaxAsAt IS NULL OR DATEDIFF(MINUTE, @MaxAsAt, @Now) <= 60
      BEGIN
        DELETE
            custom_cmds.TUserStatus
        WHERE
            AsAt = @MaxAsAt
            AND OrganizationIdentifier = @OrganizationIdentifier
            AND (@DepartmentIdentifier IS NULL OR DepartmentIdentifier = @DepartmentIdentifier)
            AND (@UserIdentifier IS NULL OR UserIdentifier = @UserIdentifier);
      END;

    -- Next, load a table with all the organization/user/resource tuples in use. 
    -- These are the resources assigned to users where the users are in departments to which the same resources are also assigned.
    CREATE TABLE #OrganizationUserCredential
    (
        OrganizationIdentifier          uniqueidentifier         NOT NULL
      , DepartmentIdentifier uniqueidentifier         NOT NULL
      , DepartmentRole     VARCHAR(20) NOT NULL
      , UserIdentifier            uniqueidentifier         NOT NULL
      , AchievementIdentifier UNIQUEIDENTIFIER         NOT NULL
      , AchievementLabel   VARCHAR(64)
      , IsRequired         INT
      , IsAssigned         INT
      , ValidationStatus   VARCHAR(32) NOT NULL
        
        PRIMARY KEY
        (
            OrganizationIdentifier
          , UserIdentifier
          , DepartmentIdentifier
          , AchievementIdentifier
        )
    );
    
    INSERT INTO #OrganizationUserCredential
    (
        OrganizationIdentifier, DepartmentIdentifier, DepartmentRole, UserIdentifier, AchievementIdentifier, AchievementLabel, IsRequired, IsAssigned, ValidationStatus
    )
    SELECT DISTINCT
        Organization.OrganizationIdentifier
      , Department.DepartmentIdentifier
      , xy.MembershipType
      , Person.UserIdentifier
      , QAchievement.AchievementIdentifier
      , AchievementLabel
      , CASE
            WHEN PersonAssignment.CredentialNecessity = 'Mandatory'
                THEN
                CAST(1 AS BIT)
            ELSE
                CAST(0 AS BIT)
        END AS IsRequired
      , CASE
            WHEN PersonAssignment.CredentialPriority = 'Planned'
                THEN
                CAST(1 AS BIT)
            ELSE
                CAST(0 AS BIT)
        END AS IsAssigned
      , PersonAssignment.CredentialStatus
    FROM
        contacts.Membership AS xy

        INNER JOIN identities.Department
            ON xy.GroupIdentifier = Department.DepartmentIdentifier

        INNER JOIN accounts.QOrganization AS Organization
            ON Department.OrganizationIdentifier = Organization.OrganizationIdentifier

        INNER JOIN identities.[User] AS Person
            ON xy.UserIdentifier = Person.UserIdentifier

        INNER JOIN achievements.QCredential AS PersonAssignment
            ON PersonAssignment.UserIdentifier = Person.UserIdentifier

        INNER JOIN achievements.QAchievement
            ON PersonAssignment.AchievementIdentifier = QAchievement.AchievementIdentifier

    WHERE
        Organization.OrganizationIdentifier = @OrganizationIdentifier
		AND xy.MembershipType IS NOT NULL
		AND xy.MembershipType <> 'Administration'
        AND Person.AccessGrantedToCmds = 1
        AND Person.UtcArchived IS NULL
        AND PersonAssignment.CredentialStatus IS NOT NULL
        -- AND QAchievement.AchievementLabel NOT IN ( 'Orientation' )
        AND
        (
            QAchievement.OrganizationIdentifier = Organization.OrganizationIdentifier
            OR EXISTS
            (
                SELECT
                    *
                FROM
                    achievements.TAchievementOrganization
                WHERE
                    TAchievementOrganization.OrganizationIdentifier = Department.OrganizationIdentifier
                AND TAchievementOrganization.AchievementIdentifier = QAchievement.AchievementIdentifier
            )
        )
        -- AND PersonAssignment.CredentialPriority = 'Planned'
        AND PersonAssignment.CredentialNecessity = 'Mandatory'
        AND
        (
            @DepartmentIdentifier IS NULL
            OR Department.DepartmentIdentifier = @DepartmentIdentifier
        )
        AND
        (
            @UserIdentifier IS NULL
            OR Person.UserIdentifier = @UserIdentifier
        );

    -- Next, count the number of resources with each validation status.
    WITH R AS (
        SELECT
            #OrganizationUserCredential.OrganizationIdentifier
          , #OrganizationUserCredential.UserIdentifier
          , #OrganizationUserCredential.DepartmentIdentifier
          , #OrganizationUserCredential.DepartmentRole
          , AchievementLabel AS ItemName
          , SUM(CASE IsRequired WHEN 1 THEN 1 ELSE 0 END) AS CountRQ
        FROM
            #OrganizationUserCredential
        GROUP BY
            #OrganizationUserCredential.OrganizationIdentifier
          , #OrganizationUserCredential.UserIdentifier
          , #OrganizationUserCredential.DepartmentIdentifier
          , #OrganizationUserCredential.DepartmentRole
          , #OrganizationUserCredential.AchievementLabel
    ), V AS (
        SELECT
            *
        FROM
        (
            SELECT
                OrganizationIdentifier
              , UserIdentifier
              , DepartmentIdentifier
              , DepartmentRole
              , AchievementLabel AS ItemName
              , AchievementIdentifier
              , CASE ValidationStatus
                    WHEN 'Valid'   THEN 'CountCP'
                    WHEN 'Expired' THEN 'CountEX'
                    WHEN 'Pending' THEN 'CountNC'
                END AS VStatus
            FROM
                #OrganizationUserCredential
        ) AS t
        PIVOT
        (
            COUNT(AchievementIdentifier)
            FOR VStatus IN (CountCP, CountEX, CountNC, CountNA, CountNT, CountSA, CountSV, CountVA)
        ) AS x
    )
    INSERT INTO custom_cmds.TUserStatus
    (
        AsAt, OrganizationIdentifier, UserIdentifier, DepartmentIdentifier, DepartmentRole, ItemName, ListDomain, CountCP, CountEX, CountNC, CountNA, CountNT, CountSA, CountSV, CountVA, CountRQ
    )
    SELECT
        @Now
      , V.OrganizationIdentifier
      , V.UserIdentifier
      , V.DepartmentIdentifier
      , V.DepartmentRole
      , V.ItemName
      , 'Resource'
      , V.CountCP
      , V.CountEX
      , V.CountNC
      , V.CountNA
      , V.CountNT
      , V.CountSA
      , V.CountSV
      , V.CountVA
      , R.CountRQ
    FROM
        R
        INNER JOIN V
            ON V.OrganizationIdentifier = R.OrganizationIdentifier
               AND V.UserIdentifier = R.UserIdentifier
               AND V.DepartmentIdentifier = R.DepartmentIdentifier
               AND V.DepartmentRole = R.DepartmentRole
               AND V.ItemName = R.ItemName;

    -- Next, load a table with all the organization/user/competency tuples in use. 
    -- These are the competencies assigned to users where the users are assigned to profiles containing those competencies in those departments.
    CREATE TABLE #OrganizationUserCompetency
    (
          OrganizationIdentifier             uniqueidentifier NOT NULL
        , DepartmentIdentifier    uniqueidentifier NOT NULL
        , DepartmentRole        VARCHAR(20) NULL
        , UserIdentifier               uniqueidentifier NOT NULL
        , CompetencyStandardIdentifier uniqueidentifier NOT NULL
        , IsCritical            INT
        , IsPrimary             INT
        , IsRequired            INT
        , IsValidated           INT
        
          PRIMARY KEY (
              OrganizationIdentifier
            , UserIdentifier
            , DepartmentIdentifier
            , CompetencyStandardIdentifier
          )
    );

    INSERT INTO #OrganizationUserCompetency
    (
        OrganizationIdentifier, DepartmentIdentifier, DepartmentRole, UserIdentifier, CompetencyStandardIdentifier, IsCritical, IsPrimary, IsRequired, IsValidated
    )
    SELECT
          Department.OrganizationIdentifier
        , DepartmentProfileUser.DepartmentIdentifier
        , Membership.MembershipType
        , UserCompetency.UserIdentifier
        , UserCompetency.StandardIdentifier
        , MAX(CAST(IsCritical AS INT))
        , MAX(CAST(IsPrimary AS INT))
        , MAX(CAST(DepartmentProfileUser.IsRequired AS INT))
        , MAX(CAST(UserCompetency.IsValidated AS INT))
    FROM
        standards.DepartmentProfileUser WITH (NOLOCK)
        INNER JOIN
        standards.DepartmentProfileCompetency WITH (NOLOCK) ON DepartmentProfileCompetency.DepartmentIdentifier = DepartmentProfileUser.DepartmentIdentifier
                                                           AND DepartmentProfileCompetency.ProfileStandardIdentifier = DepartmentProfileUser.ProfileStandardIdentifier
        INNER JOIN
        standards.StandardContainment AS ProfileCompetency WITH (NOLOCK) ON ProfileCompetency.ParentStandardIdentifier = DepartmentProfileUser.ProfileStandardIdentifier
                                                                        AND ProfileCompetency.ChildStandardIdentifier = DepartmentProfileCompetency.CompetencyStandardIdentifier
        INNER JOIN
        standards.StandardValidation  AS UserCompetency WITH (NOLOCK) ON UserCompetency.UserIdentifier = DepartmentProfileUser.UserIdentifier
                                                                     AND UserCompetency.StandardIdentifier = ProfileCompetency.ChildStandardIdentifier
        INNER JOIN
        identities.Department ON Department.DepartmentIdentifier = DepartmentProfileUser.DepartmentIdentifier

        INNER JOIN
        standards.Standard AS Competency ON UserCompetency.StandardIdentifier = Competency.StandardIdentifier
                                          AND Competency.StandardType = 'Competency'

        LEFT OUTER JOIN
        contacts.Membership ON Membership.GroupIdentifier = Department.DepartmentIdentifier
                           AND Membership.UserIdentifier = DepartmentProfileUser.UserIdentifier
    WHERE
        Department.OrganizationIdentifier = @OrganizationIdentifier
        AND
        (
            @DepartmentIdentifier IS NULL
            OR Department.DepartmentIdentifier = @DepartmentIdentifier
        )
        AND
        (
            @UserIdentifier IS NULL
            OR UserCompetency.UserIdentifier = @UserIdentifier
        )
    GROUP BY
          Department.OrganizationIdentifier
        , UserCompetency.UserIdentifier
        , DepartmentProfileUser.DepartmentIdentifier
        , Membership.MembershipType
        , UserCompetency.StandardIdentifier;

    CREATE TABLE #UserCompetency
    (
          UserIdentifier               uniqueidentifier         NOT NULL
        , CompetencyStandardIdentifier uniqueidentifier         NOT NULL
        , ValidationStatus      VARCHAR(32) NULL
        
        PRIMARY KEY
        (
              UserIdentifier
            , CompetencyStandardIdentifier
        )
    );

    INSERT INTO #UserCompetency
    (
        UserIdentifier, CompetencyStandardIdentifier, ValidationStatus
    )
    SELECT
        StandardValidation.UserIdentifier
        , StandardIdentifier
        , ValidationStatus
    FROM
        standards.StandardValidation
        INNER JOIN
        identities.[User] ON [User].UserIdentifier = StandardValidation.UserIdentifier
                    AND AccessGrantedToCmds = 1
                    AND UtcArchived IS NULL;

    WITH R AS (
        SELECT 
              #OrganizationUserCompetency.OrganizationIdentifier
            , #OrganizationUserCompetency.UserIdentifier
            , #OrganizationUserCompetency.DepartmentIdentifier
            , #OrganizationUserCompetency.DepartmentRole
            , ValueHelper.ItemName
            , SUM(CASE IsRequired WHEN 0 THEN 1 ELSE 1 END) AS CountRQ
            , COUNT(*) AS CountTotal
            , IsRequired, IsCritical, IsPrimary
        FROM
            #UserCompetency
            INNER JOIN
            #OrganizationUserCompetency ON #OrganizationUserCompetency.UserIdentifier = #UserCompetency.UserIdentifier
                                     AND #OrganizationUserCompetency.CompetencyStandardIdentifier = #UserCompetency.CompetencyStandardIdentifier
            OUTER APPLY (
                SELECT
                    CASE IsRequired WHEN 1 THEN 'Mandatory ' ELSE 'Optional ' END 
                    + CASE IsCritical WHEN 0 THEN 'Non-Critical ' ELSE 'Critical ' END 
                    + CASE IsPrimary WHEN 0 THEN 'Secondary ' ELSE 'Primary ' END 
                    + 'Competency' AS ItemName
            ) AS ValueHelper
        GROUP BY
              #OrganizationUserCompetency.OrganizationIdentifier
            , #OrganizationUserCompetency.UserIdentifier
            , #OrganizationUserCompetency.DepartmentIdentifier
            , #OrganizationUserCompetency.DepartmentRole
            , ValueHelper.ItemName
            , IsRequired
            , IsCritical
            , IsPrimary
    ), V AS (
        SELECT
            *
        FROM
        (
            SELECT DISTINCT
                  #OrganizationUserCompetency.OrganizationIdentifier
                , #OrganizationUserCompetency.UserIdentifier
                , #OrganizationUserCompetency.DepartmentIdentifier
                , #OrganizationUserCompetency.DepartmentRole
                , CASE IsRequired WHEN 1 THEN 'Mandatory ' ELSE 'Optional ' END 
                    + CASE IsCritical WHEN 0 THEN 'Non-Critical ' ELSE 'Critical ' END 
                    + CASE IsPrimary WHEN 0 THEN 'Secondary ' ELSE 'Primary ' END 
                    + 'Competency' AS ItemName
                , #OrganizationUserCompetency.CompetencyStandardIdentifier
                , CASE 
                    WHEN ValidationStatus = 'Not Applicable' AND IsValidated = 1 THEN 'CountVN'
                    WHEN ValidationStatus = 'Completed'                THEN 'CountCP'
                    WHEN ValidationStatus = 'Completed'                THEN 'CountCP'
                    WHEN ValidationStatus = 'Expired'                  THEN 'CountEX'
                    WHEN ValidationStatus = 'Not Completed'            THEN 'CountNC'
                    WHEN ValidationStatus = 'Not Applicable' AND IsValidated = 0 THEN 'CountNA'
                    WHEN ValidationStatus = 'Needs Training'           THEN 'CountNT'
                    WHEN ValidationStatus = 'Self-Assessed'            THEN 'CountSA'
                    WHEN ValidationStatus = 'Submitted for Validation' THEN 'CountSV'
                    WHEN ValidationStatus = 'Validated'                THEN 'CountVA'
                  END AS CurrentStatus
            FROM
            #UserCompetency
            INNER JOIN
            #OrganizationUserCompetency ON #OrganizationUserCompetency.UserIdentifier = #UserCompetency.UserIdentifier
                                     AND #OrganizationUserCompetency.CompetencyStandardIdentifier = #UserCompetency.CompetencyStandardIdentifier
        ) AS t
        PIVOT
        (
            COUNT(CompetencyStandardIdentifier)
            FOR CurrentStatus IN (CountCP, CountEX, CountNC, CountNA, CountNT, CountSA, CountSV, CountVA, CountVN)
        ) AS p
    )
    INSERT INTO custom_cmds.TUserStatus
    (
        AsAt, OrganizationIdentifier, UserIdentifier, DepartmentIdentifier, DepartmentRole, ItemName, ListDomain, CountCP, CountEX, CountNC, CountNA, CountNT, CountSA, CountSV, CountVA, CountVN, CountRQ
    )
    SELECT
          @Now
        , V.OrganizationIdentifier
        , V.UserIdentifier
        , V.DepartmentIdentifier
        , V.DepartmentRole
        , V.ItemName
        , 'Standard'
        , ISNULL(V.CountCP, 0)
        , ISNULL(V.CountEX, 0)
        , ISNULL(V.CountNC, 0)
        , ISNULL(V.CountNA, 0)
        , ISNULL(V.CountNT, 0)
        , ISNULL(V.CountSA, 0)
        , ISNULL(V.CountSV, 0)
        , ISNULL(V.CountVA, 0)
        , ISNULL(V.CountVN, 0)
        , ISNULL(R.CountRQ, 0)
    FROM
        R
        INNER JOIN
        V ON V.OrganizationIdentifier = R.OrganizationIdentifier
            AND V.UserIdentifier = R.UserIdentifier
            AND V.DepartmentIdentifier = R.DepartmentIdentifier
            AND (V.DepartmentRole IS NULL AND R.DepartmentRole IS NULL OR V.DepartmentRole = R.DepartmentRole)
            AND V.ItemName = R.ItemName;

    -- Denormalize FK references.
    UPDATE
        custom_cmds.TUserStatus
    SET
        UserName = FullName
    FROM
        identities.[User]
    WHERE
        [User].UserIdentifier = TUserStatus.UserIdentifier;

    UPDATE
        custom_cmds.TUserStatus
    SET
        DepartmentName = Department.DepartmentName
    FROM
        identities.Department
    WHERE
        Department.DepartmentIdentifier = TUserStatus.DepartmentIdentifier;

    UPDATE
        custom_cmds.TUserStatus
    SET
        OrganizationName = CompanyName
    FROM
        accounts.QOrganization AS Organization
    WHERE
        Organization.OrganizationIdentifier = TUserStatus.OrganizationIdentifier;

    UPDATE custom_cmds.TUserStatus SET ItemNumber = 1 WHERE ItemName = 'Time-Sensitive Safety Certificate';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 2 WHERE ItemName = 'Additional Compliance Requirement';

    UPDATE custom_cmds.TUserStatus SET ItemNumber = 3 WHERE ItemName = 'Mandatory Critical Primary Competency';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 4 WHERE ItemName = 'Mandatory Non-Critical Primary Competency';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 5 WHERE ItemName = 'Mandatory Critical Secondary Competency';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 6 WHERE ItemName = 'Mandatory Non-Critical Secondary Competency';

    UPDATE custom_cmds.TUserStatus SET ItemNumber = 7 WHERE ItemName = 'Optional Critical Primary Competency';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 8 WHERE ItemName = 'Optional Non-Critical Primary Competency';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 9 WHERE ItemName = 'Optional Critical Secondary Competency';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 10 WHERE ItemName = 'Optional Non-Critical Secondary Competency';

    UPDATE custom_cmds.TUserStatus SET ItemNumber = 11 WHERE ItemName = 'Code of Practice';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 12 WHERE ItemName = 'Safe Operating Practice';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 13 WHERE ItemName = 'Human Resources Document';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 14 WHERE ItemName = 'Module';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 15 WHERE ItemName = 'Training Guide';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 16 WHERE ItemName = 'Site-Specific Operating Procedure';
	  UPDATE custom_cmds.TUserStatus SET ItemNumber = 17 WHERE ItemName = 'Orientation';
    UPDATE custom_cmds.TUserStatus SET ItemNumber = 18 WHERE ItemName = 'HR Learning Module';

    UPDATE custom_cmds.TUserStatus SET ListFolder = ItemName WHERE ListDomain = 'Resource';
    UPDATE custom_cmds.TUserStatus SET ListFolder = 'Competency' WHERE ListDomain = 'Standard' AND ItemName LIKE '% Competency';

    UPDATE custom_cmds.TUserStatus SET TagNecessity = 'Mandatory' WHERE ListDomain = 'Standard' AND ItemName LIKE 'Mandatory % Competency';
    UPDATE custom_cmds.TUserStatus SET TagCriticality = 'Critical' WHERE ListDomain = 'Standard' AND ItemName LIKE '% Critical % Competency';
    UPDATE custom_cmds.TUserStatus SET TagPrimacy = 'Primary' WHERE ListDomain = 'Standard' AND ItemName LIKE '% Primary Competency';

    UPDATE custom_cmds.TUserStatus SET TagNecessity = 'Optional' WHERE ListDomain = 'Standard' AND ItemName LIKE 'Optional % Competency';
    UPDATE custom_cmds.TUserStatus SET TagCriticality = 'Non-Critical' WHERE ListDomain = 'Standard' AND ItemName LIKE '% Non-Critical % Competency';
    UPDATE custom_cmds.TUserStatus SET TagPrimacy = 'Secondary' WHERE ListDomain = 'Standard' AND ItemName LIKE '% Secondary Competency';

    -- Calculate scores

    UPDATE
        custom_cmds.TUserStatus
    SET
        Score = 1
    WHERE
        ListDomain = 'Resource'
        AND CountCP >= CountRQ;

    UPDATE
        custom_cmds.TUserStatus
    SET
        Score = FLOOR(100.0 * (CAST(CountCP AS DECIMAL)) / CountRQ) / 100.0
    WHERE
        ListDomain = 'Resource'
        AND CountRQ > 0
        AND ISNULL(Score, 0) <> 1;

    UPDATE
        custom_cmds.TUserStatus
    SET
        Score = 1
    WHERE
        ListDomain = 'Standard'
        AND CountVA >= CountRQ;

    UPDATE
        custom_cmds.TUserStatus
    SET
        Score = FLOOR(100.0 * (CAST(CountVA AS DECIMAL) + CAST(CountVN AS DECIMAL)) / CountRQ) / 100.0
    WHERE
        ListDomain = 'Standard'
        AND CountRQ > 0
        AND ISNULL(Score, 0) <> 1;

	UPDATE custom_cmds.TUserStatus SET Progress = Score WHERE ISNULL(Progress,0) <> ISNULL(Score,0) OR (Progress IS NULL);

    -- Drop temporary tables.
    DROP TABLE IF EXISTS #OrganizationUserResource;
    DROP TABLE IF EXISTS #OrganizationUserCompetency;
    DROP TABLE IF EXISTS #UserCompetency;
    
	-- CMDS does not run stats on Courses.
	DELETE FROM custom_cmds.TUserStatus WHERE ItemNumber IS NULL AND ItemName = 'Course';

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [custom_cmds].[SelectBillableUsers]
(
    @FromDate as datetime
  , @ThruDate as datetime
  , @UnitPricePerPeriodClassA as money
  , @UnitPricePerPeriodClassB as money
  , @UnitPricePerPeriodClassC as money
)
as
select BillingClassification
     , CompanyName
     , SharedCompanyCount
     , case
           when SharedCompanyCount = 1 then
               'Dedicated (100%)'
           else
               'Shared (' + cast(round(100 / SharedCompanyCount, 0) as nvarchar(4)) + '%)'
       end               as Category
     , count(*)          as UserCount
     , round(   (case BillingClassification
                     when 'A' then
                         @UnitPricePerPeriodClassA
                     when 'B' then
                         @UnitPricePerPeriodClassB
                     else
                         @UnitPricePerPeriodClassC
                 end
                ) / SharedCompanyCount
              , 2
            )            as UnitPrice
     , count(*) * round(   (case BillingClassification
                                when 'A' then
                                    @UnitPricePerPeriodClassA
                                when 'B' then
                                    @UnitPricePerPeriodClassB
                                else
                                    @UnitPricePerPeriodClassC
                            end
                           ) / SharedCompanyCount
                         , 2
                       ) as Amount
from custom_cmds.QCmdsInvoiceFee fees
where FromDate = @FromDate
      and ThruDate = @ThruDate
group by BillingClassification
       , CompanyName
       , SharedCompanyCount
order by BillingClassification
       , CompanyName
       , SharedCompanyCount asc;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectCollegeCertificateEligibility]
    (
        @Departments     NVARCHAR(MAX)
      , @Employees       NVARCHAR(MAX)
      , @CertificateType NVARCHAR(50)
      , @AuthorityName   NVARCHAR(100)
    )
AS
BEGIN

    DECLARE @DepartmentTable TABLE
        (
            ID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
        );

    INSERT INTO @DepartmentTable
        (
            ID
        )
                SELECT
                    CAST(ItemText AS UNIQUEIDENTIFIER)
                FROM
                    dbo.SplitText(@Departments, ',');

    DECLARE @EmployeeTable TABLE
        (
            ID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
        );

    INSERT INTO @EmployeeTable
        (
            ID
        )
                SELECT
                    CAST(ItemText AS UNIQUEIDENTIFIER)
                FROM
                    dbo.SplitText(@Employees, ',');

    WITH EmployeeCertificates AS (SELECT
                                      Organization.CompanyTitle                                                                                AS CompanyName
                                    , d.DepartmentIdentifier                                                                             AS DepartmentID
                                    , d.DepartmentName                                                                                   AS DepartmentName
                                    , persons.UserIdentifier                                                                             AS PersonID
                                    , persons.FullName
                                    , ec.ProfileStandardIdentifier
                                    , ec.ProfileNumber
                                    , ec.ProfileTitle
                                    , ec.CoreHoursCompleted
                                    , ec.NonCoreHoursCompleted
                                    , ec.CoreHoursTotal
                                    , ec.NonCoreHoursTotal
                                    , ec.CertificationHoursPercentCore
                                    , ec.CertificationHoursPercentNonCore
                                    , ec.DateRequested
                                    , ec.DateGranted
                                    , ec.DateSubmitted
                                    , ec.InstitutionName
                                    , CAST(ISNULL(ec.CertificationHoursPercentCore * ec.CoreHoursTotal / 100, 0) AS DECIMAL(5, 2))       AS CoreHoursRequired
                                    , CAST(ISNULL(ec.CertificationHoursPercentNonCore * ec.NonCoreHoursTotal / 100, 0) AS DECIMAL(5, 2)) AS NonCoreHoursRequired
                                    , CASE
                                          WHEN ec.DateGranted IS NOT NULL
                                              THEN
                                              'Granted by College'
                                          WHEN ec.DateRequested IS NOT NULL
                                               AND ec.DateSubmitted IS NOT NULL
                                              THEN
                                              'Submitted to College'
                                          WHEN ec.DateRequested IS NOT NULL
                                              THEN
                                              'Requested'
                                      END                                                                                                AS CertificateType
                                  FROM
                                      contacts.Membership    AS m
                                  INNER JOIN identities.[User]     AS persons
                                             ON persons.UserIdentifier = m.UserIdentifier

                                  INNER JOIN identities.Department AS d
                                             ON d.DepartmentIdentifier = m.GroupIdentifier

                                  INNER JOIN accounts.QOrganization AS Organization
                                             ON Organization.OrganizationIdentifier = d.OrganizationIdentifier
                                  CROSS APPLY
                                      (
                                          SELECT
                                              P.ProfileStandardIdentifier
                                            , P.ProfileNumber
                                            , P.ProfileTitle
                                            , ISNULL(
                                                  (
                                                      SELECT
                                                          SUM(Q.CertificationHoursCore)
                                                      FROM
                                                          custom_cmds.Competency               AS S
                                                      INNER JOIN custom_cmds.UserCompetency    AS C
                                                                 ON S.CompetencyStandardIdentifier = C.CompetencyStandardIdentifier

                                                      INNER JOIN custom_cmds.ProfileCompetency AS Q
                                                                 ON P.ProfileStandardIdentifier = Q.ProfileStandardIdentifier
                                                                    AND S.CompetencyStandardIdentifier = Q.CompetencyStandardIdentifier
                                                      WHERE
                                                          C.UserIdentifier = persons.UserIdentifier
                                                          AND C.ValidationStatus IN (
                                                                                        'Validated', 'Expired'
                                                                                    )
                                                          AND S.IsDeleted = 0
                                                  ), 0
                                                    )            AS CoreHoursCompleted
                                            , ISNULL(
                                                  (
                                                      SELECT
                                                          SUM(Q.CertificationHoursNonCore)
                                                      FROM
                                                          custom_cmds.Competency               AS S
                                                      INNER JOIN custom_cmds.UserCompetency    AS C
                                                                 ON S.CompetencyStandardIdentifier = C.CompetencyStandardIdentifier

                                                      INNER JOIN custom_cmds.ProfileCompetency AS Q
                                                                 ON P.ProfileStandardIdentifier = Q.ProfileStandardIdentifier
                                                                    AND S.CompetencyStandardIdentifier = Q.CompetencyStandardIdentifier
                                                      WHERE
                                                          C.UserIdentifier = persons.UserIdentifier
                                                          AND C.ValidationStatus IN (
                                                                                        'Validated', 'Expired'
                                                                                    )
                                                          AND S.IsDeleted = 0
                                                  ), 0
                                                    )            AS NonCoreHoursCompleted
                                            , ISNULL(
                                                  (
                                                      SELECT
                                                          SUM(Q.CertificationHoursCore)
                                                      FROM
                                                          custom_cmds.Competency               AS S
                                                      INNER JOIN custom_cmds.ProfileCompetency AS Q
                                                                 ON P.ProfileStandardIdentifier = Q.ProfileStandardIdentifier
                                                                    AND S.CompetencyStandardIdentifier = Q.CompetencyStandardIdentifier
                                                      WHERE
                                                          S.IsDeleted = 0
                                                  ), 0
                                                    )            AS CoreHoursTotal
                                            , ISNULL(
                                                  (
                                                      SELECT
                                                          SUM(Q.CertificationHoursNonCore)
                                                      FROM
                                                          custom_cmds.Competency               AS S
                                                      INNER JOIN custom_cmds.ProfileCompetency AS Q
                                                                 ON P.ProfileStandardIdentifier = Q.ProfileStandardIdentifier
                                                                    AND S.CompetencyStandardIdentifier = Q.CompetencyStandardIdentifier
                                                      WHERE
                                                          S.IsDeleted = 0
                                                  ), 0
                                                    )            AS NonCoreHoursTotal
                                            , P.CertificationHoursPercentCore
                                            , P.CertificationHoursPercentNonCore
                                            , cert.DateRequested
                                            , cert.DateGranted
                                            , cert.DateSubmitted
                                            , cert.AuthorityName AS InstitutionName
                                          FROM
                                              custom_cmds.Profile                     AS P
                                          INNER JOIN custom_cmds.VCmdsProfileOrganization   AS CP
                                                     ON CP.ProfileStandardIdentifier = P.ProfileStandardIdentifier

                                          INNER JOIN custom_cmds.ProfileCertification AS cert
                                                     ON cert.ProfileStandardIdentifier = P.ProfileStandardIdentifier
                                                        AND cert.UserIdentifier = persons.UserIdentifier
                                          WHERE
                                              CP.OrganizationIdentifier = d.OrganizationIdentifier
                                              AND P.ProfileStandardIdentifier IN
                                                      (
                                                          SELECT
                                                              dp.ProfileStandardIdentifier
                                                          FROM
                                                              contacts.Membership            AS m
                                                          INNER JOIN identities.Department         AS all_departments
                                                                     ON all_departments.DepartmentIdentifier = m.GroupIdentifier

                                                          INNER JOIN custom_cmds.DepartmentProfile AS dp
                                                                     ON dp.DepartmentIdentifier = m.GroupIdentifier
                                                          WHERE
                                                              m.UserIdentifier = persons.UserIdentifier
                                                              AND all_departments.OrganizationIdentifier = d.OrganizationIdentifier
                                                      )
                                      )                            AS ec
                                  WHERE
                                      (
                                          ec.CoreHoursTotal <> 0
                                          OR ec.NonCoreHoursTotal <> 0
                                      ))
    SELECT
        *
    FROM
        EmployeeCertificates
    WHERE
        EXISTS
        (
            SELECT
                *
            FROM
                @DepartmentTable
            WHERE
                ID = EmployeeCertificates.DepartmentID
        )
        AND CertificateType IS NOT NULL
        AND EXISTS
        (
            SELECT
                *
            FROM
                @EmployeeTable
            WHERE
                ID = EmployeeCertificates.PersonID
        )
        AND
            (
                @CertificateType IS NULL
                OR @CertificateType = CertificateType
            )
        AND
            (
                @AuthorityName IS NULL
                OR @AuthorityName = InstitutionName
            )
    ORDER BY
        CertificateType
      , FullName
      , ProfileTitle;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectCompanyProfileCompetenciesForInsert]
(
    @OrganizationIdentifier    uniqueidentifier
   ,@ProfileStandardIdentifier uniqueidentifier
)
AS
BEGIN
    SELECT
        CompetencyStandardIdentifier AS StandardIdentifier
    FROM
        custom_cmds.ProfileCompetency
    WHERE
        ProfileStandardIdentifier = @ProfileStandardIdentifier
        AND CompetencyStandardIdentifier NOT IN
                (
                    SELECT
                        CompetencyStandardIdentifier
                    FROM
                        custom_cmds.VCmdsCompetencyOrganization
                    WHERE
                        OrganizationIdentifier = @OrganizationIdentifier
                );
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [custom_cmds].[SelectCompetenciesExpiring]
(
    @NotifiedAt datetimeoffset(7)
  , @ExpiredAt datetimeoffset(7)
)
as
begin

    -- Create a temporary table that contains expiring competencies only.
    create table #Expiries
    (
        CompetencyStandardIdentifier uniqueidentifier
      , CompetencyNumber varchar(256)
      , CompetencyTitle varchar(max)
      , UserIdentifier uniqueidentifier
      , DateCompleted datetimeoffset
      , DateExpired datetimeoffset
      , Notified datetimeoffset
    );

    insert into #Expiries
    (
        CompetencyStandardIdentifier
      , CompetencyNumber
      , CompetencyTitle
      , UserIdentifier
      , DateCompleted
      , DateExpired
      , Notified
    )
    select Competency.StandardIdentifier as CompetencyStandardIdentifier
         , Competency.Code               as CompetencyNumber
         , CompetencyTitle.ContentText   as CompetencyTitle
         , UserCompetency.UserIdentifier
         , UserCompetency.ValidationDate as DateCompleted
         , UserCompetency.Expired        as DateExpired
         , UserCompetency.Notified
    from standards.StandardValidation      as UserCompetency
         inner join standards.Standard     as Competency on Competency.StandardIdentifier = UserCompetency.StandardIdentifier
         inner join accounts.QOrganization as T on T.OrganizationIdentifier = Competency.OrganizationIdentifier
         left join contents.TContent       as CompetencyTitle on CompetencyTitle.ContainerIdentifier = Competency.StandardIdentifier
                                                                 and CompetencyTitle.ContentLabel = 'Title'
                                                                 and CompetencyTitle.ContentLanguage = 'en'
    where UserCompetency.Expired is not null
          and UserCompetency.Expired < @ExpiredAt
          and
          (
              UserCompetency.Notified is null
              or UserCompetency.Notified < @NotifiedAt
          );

    -- Create a temporary table that contains only the people who have expiring competencies.
    create table #People
    (
        UserIdentifier uniqueidentifier
      , FullName varchar(128)
      , Email varchar(254)
      , DepartmentIdentifier uniqueidentifier
      , DepartmentName varchar(148)
      , OrganizationIdentifier uniqueidentifier
      , CompanyName varchar(128)
    );

    insert into #People
    (
        UserIdentifier
      , FullName
      , Email
      , DepartmentIdentifier
      , DepartmentName
      , OrganizationIdentifier
      , CompanyName
    )
    select Person.UserIdentifier
         , Person.FullName
         , Person.Email
         , Department.DepartmentIdentifier
         , Department.DepartmentName
         , Organization.OrganizationIdentifier
         , Organization.CompanyTitle
    from identities.[User]                 as Person
         inner join contacts.Membership on Person.UserIdentifier = Membership.UserIdentifier
                                           and Membership.MembershipType <> 'Administration'
         inner join identities.Department on Department.DepartmentIdentifier = Membership.GroupIdentifier
         inner join accounts.QOrganization as Organization on Organization.OrganizationIdentifier = Department.OrganizationIdentifier
                                                              and Organization.AccountClosed is null
    where Person.UtcArchived is null
          and Person.AccessGrantedToCmds = 1
          and Person.UserIdentifier in
              (
                  select distinct
                         UserIdentifier
                  from #Expiries
                  where #Expiries.UserIdentifier = Person.UserIdentifier
              );

    -- Join the temporary tables to obtain the result set.
    select People.OrganizationIdentifier
         , Expiries.UserIdentifier
         , Expiries.CompetencyStandardIdentifier
         , People.CompanyName
         , string_agg(DepartmentName, ', ') within group (order by DepartmentName) as DepartmentNames
         , People.FullName
         , People.Email
         , Expiries.CompetencyNumber
         , Expiries.CompetencyTitle
         , Expiries.DateCompleted
         , Expiries.DateExpired
         , Expiries.Notified
    from #Expiries          as Expiries
         inner join #People as People on Expiries.UserIdentifier = People.UserIdentifier
         inner join custom_cmds.UserProfile on UserProfile.UserIdentifier = People.UserIdentifier
         inner join custom_cmds.ProfileCompetency on ProfileCompetency.ProfileStandardIdentifier = UserProfile.ProfileStandardIdentifier
                                                     and ProfileCompetency.CompetencyStandardIdentifier = Expiries.CompetencyStandardIdentifier
    group by Expiries.CompetencyStandardIdentifier
           , Expiries.CompetencyNumber
           , Expiries.CompetencyTitle
           , Expiries.DateCompleted
           , Expiries.DateExpired
           , Expiries.Notified
           , People.OrganizationIdentifier
           , People.CompanyName
           , Expiries.UserIdentifier
           , People.FullName
           , People.Email;

    drop table #Expiries;
    drop table #People;

end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [custom_cmds].[SelectCompetenciesExpiringByDate]
(
    @Now datetimeoffset
)
as
begin
    select
        Employee.UserIdentifier
       ,Employee.UserIdentifier
       ,Employee.LastName as PersonLastName
       ,Employee.FirstName as PersonFirstName
       ,Employee.FullName as PersonName
       ,EmployeeCompetencyStandard.StandardIdentifier as CompetencyStandardIdentifier
       ,EmployeeCompetencyStandard.Code as CompetencyNumber
       ,EmployeeCompetencyStandardTitle.ContentText as CompetencySummary
       ,EmployeeCompetency.ValidationStatus
       ,EmployeeCompetency.ValidationDate
       ,cast(datefromparts(
            year(@Now)
           ,DepartmentOrganization.CompetencyAutoExpirationMonth
           ,DepartmentOrganization.CompetencyAutoExpirationDay) as datetimeoffset
        ) as NewExpirationDate
       ,Profile.StandardIdentifier as ProfileStandardIdentifier
       ,Profile.Code as ProfileNumber
       ,D.DepartmentIdentifier
       ,D.DepartmentName
       ,DepartmentOrganization.OrganizationIdentifier
       ,DepartmentOrganization.CompanyTitle as CompanyName
    from
        identities.[User] as Employee
        inner join standards.DepartmentProfileUser as DPU
            on DPU.UserIdentifier = Employee.UserIdentifier
        inner join identities.Department as D
            on D.DepartmentIdentifier = DPU.DepartmentIdentifier
        inner join standards.Standard as Profile
            on Profile.StandardIdentifier = DPU.ProfileStandardIdentifier
        inner join accounts.QOrganization as DepartmentOrganization
            on DepartmentOrganization.OrganizationIdentifier = D.OrganizationIdentifier
        inner join standards.StandardValidation as EmployeeCompetency
            on EmployeeCompetency.UserIdentifier = Employee.UserIdentifier
        inner join standards.Standard as EmployeeCompetencyStandard
            on EmployeeCompetencyStandard.StandardIdentifier = EmployeeCompetency.StandardIdentifier
        inner join standards.DepartmentProfileCompetency as CompetencySettings
            on CompetencySettings.CompetencyStandardIdentifier = EmployeeCompetencyStandard.StandardIdentifier
                and CompetencySettings.DepartmentIdentifier = DPU.DepartmentIdentifier
                and CompetencySettings.ProfileStandardIdentifier = DPU.ProfileStandardIdentifier
        left join contents.TContent as EmployeeCompetencyStandardTitle on EmployeeCompetencyStandardTitle.ContainerIdentifier = EmployeeCompetencyStandard.StandardIdentifier
                                                        and EmployeeCompetencyStandardTitle.ContentLabel = 'Title'
                                                        and EmployeeCompetencyStandardTitle.ContentLanguage = 'en'
    where
        Profile.StandardType = 'Profile' 
        and DepartmentOrganization.CompetencyAutoExpirationMode in ('2','Date')
        and EmployeeCompetencyStandard.StandardType = 'Competency'
        and EmployeeCompetency.ValidationStatus = 'Validated'
        and CompetencySettings.LifetimeMonths is not null
        and datefromparts(
            year(EmployeeCompetency.ValidationDate)
           ,DepartmentOrganization.CompetencyAutoExpirationMonth
           ,DepartmentOrganization.CompetencyAutoExpirationDay
        ) < @Now
        and datefromparts(
            year(@Now)
           ,DepartmentOrganization.CompetencyAutoExpirationMonth
           ,DepartmentOrganization.CompetencyAutoExpirationDay
        ) > EmployeeCompetency.ValidationDate
end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [custom_cmds].[SelectCompetenciesExpiringByInterval](@Now DATETIMEOFFSET)
AS
BEGIN

WITH CompetencySettings AS (
    SELECT
        DPC.ProfileStandardIdentifier,
        DPC.DepartmentIdentifier,
        DPC.LifetimeMonths,
        Competency.StandardIdentifier AS CompetencyStandardIdentifier,
        Competency.Code AS CompetencyNumber,
        Competency.ContentTitle AS CompetencySummary,
        D.DepartmentName,
        DepartmentOrganization.OrganizationIdentifier,
        DepartmentOrganization.CompanyName AS OrganizationName
    FROM
        standards.DepartmentProfileCompetency AS DPC
        INNER JOIN identities.Department AS D ON D.DepartmentIdentifier = DPC.DepartmentIdentifier
	    INNER JOIN standards.Standard AS Competency ON Competency.StandardIdentifier = DPC.CompetencyStandardIdentifier
        INNER JOIN accounts.QOrganization AS DepartmentOrganization ON DepartmentOrganization.OrganizationIdentifier = D.OrganizationIdentifier
    WHERE
        DPC.LifetimeMonths IS NOT NULL
        AND DepartmentOrganization.CompetencyAutoExpirationMode = 'Interval'
)
SELECT DISTINCT
    Employee.UserIdentifier,
    Employee.UserIdentifier,
    Employee.LastName AS PersonLastName,
    Employee.FirstName AS PersonFirstName,
    Employee.FullName AS PersonName,
    CompetencySettings.CompetencyStandardIdentifier,
    CompetencySettings.CompetencyNumber,
    CompetencySettings.CompetencySummary,
    EmployeeCompetency.ValidationStatus,
    EmployeeCompetency.ValidationDate,
    ValueHelper.NewExpirationDate,
    CompetencySettings.DepartmentIdentifier,
    CompetencySettings.DepartmentName,
    CompetencySettings.OrganizationIdentifier,
    CompetencySettings.OrganizationName AS CompanyName
FROM 
    standards.StandardValidation AS EmployeeCompetency
    INNER JOIN identities.[User] AS Employee ON Employee.UserIdentifier = EmployeeCompetency.UserIdentifier
    INNER JOIN CompetencySettings ON CompetencySettings.CompetencyStandardIdentifier = EmployeeCompetency.StandardIdentifier
    OUTER APPLY (
        SELECT
            CAST(DATEADD(MONTH, CompetencySettings.LifetimeMonths, EmployeeCompetency.ValidationDate) AS DATETIMEOFFSET) AS NewExpirationDate
    ) AS ValueHelper
WHERE
    ValueHelper.NewExpirationDate < @Now
    AND EmployeeCompetency.ValidationStatus = 'Validated'
    AND EXISTS(
        SELECT TOP(1) 1
        FROM
            standards.DepartmentProfileUser AS DPU
        WHERE
            DPU.DepartmentIdentifier = CompetencySettings.DepartmentIdentifier
            AND DPU.ProfileStandardIdentifier = CompetencySettings.ProfileStandardIdentifier
            AND DPU.UserIdentifier = EmployeeCompetency.UserIdentifier
    );

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectCompetencyStatusPerUser](
    @OrganizationIdentifier UNIQUEIDENTIFIER
, @DepartmentIdentifier UNIQUEIDENTIFIER
, @UserIdentifier UNIQUEIDENTIFIER
, @ProfileStandardIdentifier UNIQUEIDENTIFIER
, @IsPrimary BIT
)
AS
BEGIN
    IF @DepartmentIdentifier IS NULL
        BEGIN
            ;
            WITH CTE
                     AS ( SELECT person.UserIdentifier
                               , DepartmentIdentifier
                               , ValidationStatus
                               , count(*) AS CompetencyCount
                          FROM identities.[User] AS person
                                   INNER JOIN standards.DepartmentProfileUser
                                              ON DepartmentProfileUser.UserIdentifier = person.UserIdentifier
                                   INNER JOIN standards.StandardContainment
                                              ON StandardContainment.ParentStandardIdentifier =
                                                 DepartmentProfileUser.ProfileStandardIdentifier
                                   INNER JOIN standards.StandardValidation ON StandardValidation.StandardIdentifier =
                                                                              StandardContainment.ChildStandardIdentifier
                              AND StandardValidation.UserIdentifier = DepartmentProfileUser.UserIdentifier
                          WHERE (
                              @UserIdentifier IS NULL
                                  OR person.UserIdentifier = @UserIdentifier
                              )
                            AND (
                              @ProfileStandardIdentifier IS NOT NULL
                                  AND DepartmentProfileUser.ProfileStandardIdentifier = @ProfileStandardIdentifier
                                  OR @ProfileStandardIdentifier IS NULL
                                  AND @IsPrimary = 1
                                  AND DepartmentProfileUser.IsPrimary = 1
                                  OR @ProfileStandardIdentifier IS NULL
                                  AND @IsPrimary = 0
                              )
                          GROUP BY person.UserIdentifier
                                 , DepartmentIdentifier
                                 , ValidationStatus )
            SELECT Department.DepartmentName
                 , CTE.DepartmentIdentifier
                 , Membership.MembershipType
                 , CTE.UserIdentifier
                 , Person.FullName AS UserFullName
                 , CTE.ValidationStatus
                 , CTE.CompetencyCount
            FROM CTE
                     INNER JOIN contacts.Membership ON Membership.UserIdentifier = CTE.UserIdentifier
                AND Membership.GroupIdentifier = CTE.DepartmentIdentifier
                     INNER JOIN identities.Department ON Department.DepartmentIdentifier = CTE.DepartmentIdentifier
                AND Department.OrganizationIdentifier = @OrganizationIdentifier
                     INNER JOIN accounts.QOrganization AS Organization
                                ON Organization.OrganizationIdentifier = Department.OrganizationIdentifier
                     INNER JOIN contacts.QPerson AS Person ON Person.UserIdentifier = CTE.UserIdentifier AND
                                                              Person.OrganizationIdentifier =
                                                              Organization.OrganizationIdentifier

            ORDER BY DepartmentName
                   , FullName
                   , ValidationStatus
            OPTION (RECOMPILE);

        END;
    ELSE
        BEGIN
            ;
            WITH CTE
                     AS ( SELECT person.UserIdentifier
                               , DepartmentIdentifier
                               , ValidationStatus
                               , count(*) AS CompetencyCount
                          FROM identities.[User] AS person
                                   INNER JOIN standards.DepartmentProfileUser
                                              ON DepartmentProfileUser.UserIdentifier = person.UserIdentifier
                                   INNER JOIN standards.StandardContainment
                                              ON StandardContainment.ParentStandardIdentifier =
                                                 DepartmentProfileUser.ProfileStandardIdentifier
                                   INNER JOIN standards.StandardValidation ON StandardValidation.StandardIdentifier =
                                                                              StandardContainment.ChildStandardIdentifier
                              AND StandardValidation.UserIdentifier = DepartmentProfileUser.UserIdentifier
                          WHERE (
                              @UserIdentifier IS NULL
                                  OR person.UserIdentifier = @UserIdentifier
                              )
                            AND (
                              @ProfileStandardIdentifier IS NOT NULL
                                  AND DepartmentProfileUser.ProfileStandardIdentifier = @ProfileStandardIdentifier
                                  OR @ProfileStandardIdentifier IS NULL
                                  AND @IsPrimary = 1
                                  AND DepartmentProfileUser.IsPrimary = 1
                                  OR @ProfileStandardIdentifier IS NULL
                                  AND @IsPrimary = 0
                              )
                          GROUP BY person.UserIdentifier
                                 , DepartmentIdentifier
                                 , ValidationStatus )
            SELECT Department.DepartmentName
                 , CTE.DepartmentIdentifier
                 , relation.MembershipType
                 , CTE.UserIdentifier
                 , Person.FullName AS UserFullName
                 , CTE.ValidationStatus
                 , CTE.CompetencyCount
            FROM CTE
                     INNER JOIN contacts.Membership AS relation ON relation.UserIdentifier = CTE.UserIdentifier
                AND relation.GroupIdentifier = CTE.DepartmentIdentifier
                     INNER JOIN identities.Department ON Department.DepartmentIdentifier = CTE.DepartmentIdentifier
                     INNER JOIN contacts.QPerson AS Person ON Person.UserIdentifier = CTE.UserIdentifier AND
                                                              Person.OrganizationIdentifier =
                                                              Department.OrganizationIdentifier

            WHERE relation.GroupIdentifier = @DepartmentIdentifier
            ORDER BY DepartmentName
                   , FullName
                   , ValidationStatus
            OPTION (RECOMPILE);

        END;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [custom_cmds].[SelectDepartmentCompetencies]
    @DepartmentIdentifier uniqueidentifier
  , @ProfileStandardIdentifier uniqueidentifier
  , @IsTimeSensitive bit
  , @Criticality varchar(100)
  , @PriorityMustBeNull bit
  , @ShowExcludedCompetencies bit
as
    begin
        declare @PriorityCanBeNull as bit = 0;

        if @PriorityMustBeNull = 0
           and @Criticality is not null
            begin
                set @PriorityCanBeNull = case
                                             when @Criticality = 'Non-Critical' then
                                                 1
                                             else
                                                 0
                                         end;
            end;

        if @ShowExcludedCompetencies = 1
            begin
                declare @DepartmentName nvarchar(256)
                      , @CompanyName    nvarchar(256);

                select @DepartmentName = Department.DepartmentName
                     , @CompanyName    = Organization.CompanyTitle
                from identities.Department
                     inner join accounts.QOrganization as Organization on Organization.OrganizationIdentifier = Department.OrganizationIdentifier
                where Department.DepartmentIdentifier = @DepartmentIdentifier;

                select @DepartmentIdentifier     as DepartmentID
                     , @DepartmentName           as DepartmentName
                     , @CompanyName              as CompanyName
                     , Code                      as Number
                     , StandardTitle.ContentText as Summary
                     , null                      as ProfileNumber
                     , null                      as ProfileTitle
                     , cast(0 as bit)            as IsTimeSensitive
                     , null                      as ValidForText
                     , null                      as PriorityText
                     , null                      as LevelName
                from standards.Standard
                     inner join accounts.QOrganization as Organization on Organization.OrganizationIdentifier = Standard.OrganizationIdentifier
                     left join contents.TContent       as StandardTitle on StandardTitle.ContainerIdentifier = Standard.StandardIdentifier
                                                                           and StandardTitle.ContentLabel = 'Title'
                                                                           and StandardTitle.ContentLanguage = 'en'
                where StandardType = 'Competency'
                      and StandardIdentifier in (
                                                    select StandardContainment.ChildStandardIdentifier -- CompetencyID
                                                    from identities.TDepartmentStandard
                                                         inner join standards.Standard on Standard.StandardIdentifier = Standard.StandardIdentifier
                                                         inner join accounts.QOrganization as Organization on Organization.OrganizationIdentifier = Standard.OrganizationIdentifier
                                                         inner join standards.StandardContainment on StandardContainment.ParentStandardIdentifier = TDepartmentStandard.StandardIdentifier -- ProfileStandardIdentifier
                                                    where TDepartmentStandard.DepartmentIdentifier = @DepartmentIdentifier
                                                          and Standard.StandardType = 'Profile'
                                                          and (
                                                                  @ProfileStandardIdentifier is null
                                                                  or TDepartmentStandard.StandardIdentifier = @ProfileStandardIdentifier
                                                              )
                                                )
                      and StandardIdentifier not in (
                                                        select CompetencyStandardIdentifier as CompetencyID
                                                        from standards.DepartmentProfileCompetency
                                                        where DepartmentIdentifier = @DepartmentIdentifier
                                                              and (
                                                                      @ProfileStandardIdentifier is null
                                                                      or ProfileStandardIdentifier = @ProfileStandardIdentifier
                                                                  )
                                                    );
            end;
        else
            begin
                declare @IsCiritical bit = case
                                               when @Criticality = 'Critical' then
                                                   1
                                               when @Criticality = 'Non-Critical' then
                                                   0
                                               else
                                                   null
                                           end;

                select Department.DepartmentIdentifier
                     , Department.DepartmentName
                     , Organization.CompanyTitle   as CompanyName
                     , c.Code                      as Number
                     , CompetencyTitle.ContentText as Summary
                     , p.Code                      as ProfileNumber
                     , ProfileTitle.ContentText    as ProfileTitle
                     , case
                           when cs.LifetimeMonths is not null then
                               cast(1 as bit)
                           else
                               cast(0 as bit)
                       end                         as IsTimeSensitive
                     , case
                           when cs.LifetimeMonths is null then
                               ''
                           else
                               'Valid for ' + cast(cs.LifetimeMonths as nvarchar(50)) + ' Months'
                       end                         as ValidForText
                     , case
                           when cs.IsCritical = 1 then
                               'Critical'
                           else
                               'Non-Critical'
                       end                         as PriorityText
                     , null                        as LevelName
                from identities.Department
                     inner join accounts.QOrganization                as Organization on Organization.OrganizationIdentifier = Department.OrganizationIdentifier
                     inner join standards.DepartmentProfileCompetency as cs on cs.DepartmentIdentifier = Department.DepartmentIdentifier
                     inner join standards.Standard                    as c on c.StandardIdentifier = cs.CompetencyStandardIdentifier
                     inner join accounts.QOrganization                as CompetencyOrganization on CompetencyOrganization.OrganizationIdentifier = c.OrganizationIdentifier
                     inner join standards.Standard                    as p on p.StandardIdentifier = cs.ProfileStandardIdentifier
                     inner join accounts.QOrganization                as ProfileOrganization on ProfileOrganization.OrganizationIdentifier = p.OrganizationIdentifier
                     inner join standards.StandardContainment         as pc on pc.ChildStandardIdentifier = cs.CompetencyStandardIdentifier
                                                                               and pc.ParentStandardIdentifier = cs.ProfileStandardIdentifier
                     left join contents.TContent                      as CompetencyTitle on CompetencyTitle.ContainerIdentifier = c.StandardIdentifier
                                                                                            and CompetencyTitle.ContentLabel = 'Title'
                                                                                            and CompetencyTitle.ContentLanguage = 'en'
                     left join contents.TContent                      as ProfileTitle on ProfileTitle.ContainerIdentifier = p.StandardIdentifier
                                                                                         and ProfileTitle.ContentLabel = 'Title'
                                                                                         and ProfileTitle.ContentLanguage = 'en'
                where Department.DepartmentIdentifier = @DepartmentIdentifier
                      and c.StandardType = 'Competency'
                      and p.StandardType = 'Profile'
                      and (
                              @ProfileStandardIdentifier is null
                              or p.StandardIdentifier = @ProfileStandardIdentifier
                          )
                      and (
                              @IsTimeSensitive is null
                              or @IsTimeSensitive = 1
                                 and cs.LifetimeMonths is not null
                              or @IsTimeSensitive = 0
                                 and cs.LifetimeMonths is null
                          )
                      and (
                              @PriorityMustBeNull = 0
                              and (
                                      @IsCiritical is null
                                      or cs.IsCritical = @IsCiritical
                                      or cs.IsCritical = 0
                                         and @PriorityCanBeNull = 1
                                  )
                              or @PriorityMustBeNull = 1
                                 and cs.IsCritical = 0
                          );
            end;
    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectOrganizationAchievements]
    (
        @OrganizationIdentifier UNIQUEIDENTIFIER,
        @AchievementType        VARCHAR(100),
        @ExcludeElm             BIT,
        @ExcludeTss             BIT
    )
AS
    BEGIN

        CREATE TABLE #TempAchievement
            (
                AchievementIdentifier  UNIQUEIDENTIFIER NOT NULL,
                AchievementLabel       VARCHAR(MAX)     COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
                AchievementTitle       NVARCHAR(MAX)    COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
                OrganizationIdentifier UNIQUEIDENTIFIER NOT NULL
            );

        INSERT INTO #TempAchievement
            (
                AchievementIdentifier,
                AchievementLabel,
                AchievementTitle,
                OrganizationIdentifier
            )
                    SELECT
                            R.AchievementIdentifier,
                            R.AchievementLabel,
                            R.AchievementTitle,
                            T.OrganizationIdentifier
                    FROM
                            achievements.QAchievement AS R
                        INNER JOIN
                            accounts.QOrganization    AS T
                                ON T.OrganizationIdentifier = R.OrganizationIdentifier
                    WHERE
                            R.AchievementLabel = ISNULL(@AchievementType, R.AchievementLabel)
                            AND
                                (
                                    @ExcludeElm = 0
                                    OR R.AchievementLabel NOT IN (
                                                                     'Module', 'HR Learning Module'
                                                                 )
                                )
                            AND
                                (
                                    @ExcludeTss = 0
                                    OR R.AchievementLabel <> 'Time-Sensitive Safety Certificate'
                                )
                            AND R.AchievementLabel NOT IN (
                                                              'Lesson', 'Template', 'Web Page'
                                                          );

        CREATE TABLE #TempCompetency
            (
                StandardIdentifier UNIQUEIDENTIFIER NOT NULL,
                CompetencyNumber   VARCHAR(256)     NULL,
                UploadIdentifier   UNIQUEIDENTIFIER NOT NULL,
                PRIMARY KEY (StandardIdentifier, UploadIdentifier)
            );

        INSERT INTO #TempCompetency
            (
                StandardIdentifier,
                CompetencyNumber,
                UploadIdentifier
            )
                    SELECT
                            C.StandardIdentifier,
                            C.Code AS CompetencyNumber,
                            R.UploadIdentifier
                    FROM
                            standards.Standard       AS C
                        INNER JOIN
                            resources.UploadRelation AS R
                                ON R.ContainerIdentifier = C.StandardIdentifier
                        INNER JOIN
                            accounts.QOrganization   AS T
                                ON T.OrganizationIdentifier = C.OrganizationIdentifier
                    WHERE
                            C.IsHidden = 0;

        SELECT
                R.AchievementIdentifier,
                R.AchievementLabel,
                NULL                              AS ResourceNumber,
                R.AchievementTitle,
                CmdsResourceCategory.CategoryName AS CategoryTitle,
                Upload.UploadIdentifier,
                Upload.ContainerIdentifier        AS UploadContainerIdentifier,
                Upload.Name                       AS UploadName,
                Upload.Title                      AS UploadTitle,
                Upload.ContentSize,
                Upload.UploadType,
                Competency.StandardIdentifier     AS CompetencyStandardIdentifier,
                Competency.CompetencyNumber
        FROM
                accounts.QOrganization               AS Organization
            INNER JOIN
                resources.Upload
                    ON Upload.ContainerIdentifier = Organization.OrganizationIdentifier
            INNER JOIN
                resources.UploadRelation
                    ON UploadRelation.UploadIdentifier = Upload.UploadIdentifier
            INNER JOIN
                #TempAchievement                     AS R
                    ON R.AchievementIdentifier = UploadRelation.ContainerIdentifier
            LEFT JOIN
                custom_cmds.VCmdsAchievementCategory AS CmdsResourceCategory
                    ON CmdsResourceCategory.AchievementIdentifier = R.AchievementIdentifier
            LEFT JOIN
                #TempCompetency                      AS Competency
                    ON Competency.UploadIdentifier = Upload.UploadIdentifier
        WHERE
                Organization.OrganizationIdentifier = @OrganizationIdentifier
        UNION ALL
        SELECT
                R.AchievementIdentifier,
                R.AchievementLabel,
                NULL                              AS ResourceNumber,
                R.AchievementTitle,
                CoreResourceCategory.CategoryName AS CategoryTitle,
                Upload.UploadIdentifier,
                Upload.ContainerIdentifier        AS UploadContainerIdentifier,
                Upload.Name                       AS UploadName,
                Upload.Title                      AS UploadTitle,
                Upload.ContentSize,
                Upload.UploadType,
                Competency.StandardIdentifier,
                Competency.CompetencyNumber
        FROM
                accounts.QOrganization               AS Organization
            INNER JOIN
                #TempAchievement                     AS R
                    ON R.OrganizationIdentifier = Organization.OrganizationIdentifier
            INNER JOIN
                resources.Upload
                    ON Upload.ContainerIdentifier = AchievementIdentifier
            LEFT JOIN
                custom_cmds.VCmdsAchievementCategory AS CmdsResourceCategory
                    ON CmdsResourceCategory.AchievementIdentifier = R.AchievementIdentifier
            LEFT JOIN
                achievements.TAchievementCategory    AS CoreResourceCategory
                    ON CoreResourceCategory.CategoryIdentifier = CmdsResourceCategory.CategoryIdentifier
            LEFT JOIN
                #TempCompetency                      AS Competency
                    ON Competency.UploadIdentifier = Upload.UploadIdentifier
        WHERE
                Organization.OrganizationIdentifier = @OrganizationIdentifier
        UNION ALL
        SELECT
                R.AchievementIdentifier,
                R.AchievementLabel,
                NULL,
                R.AchievementTitle,
                CmdsResourceCategory.CategoryName,
                NULL,
                NULL,
                NULL,
                NULL,
                NULL,
                NULL,
                NULL,
                NULL
        FROM
                custom_cmds.VCmdsAchievementOrganization
            INNER JOIN
                #TempAchievement                     AS R
                    ON R.AchievementIdentifier = VCmdsAchievementOrganization.AchievementIdentifier
            LEFT JOIN
                custom_cmds.VCmdsAchievementCategory AS CmdsResourceCategory
                    ON CmdsResourceCategory.AchievementIdentifier = R.AchievementIdentifier
        WHERE
                VCmdsAchievementOrganization.OrganizationIdentifier = @OrganizationIdentifier
                AND VCmdsAchievementOrganization.AchievementIdentifier NOT IN
                        (
                            SELECT
                                    VCmdsAchievement.AchievementIdentifier
                            FROM
                                    resources.Upload
                                INNER JOIN
                                    resources.UploadRelation
                                        ON UploadRelation.UploadIdentifier = Upload.UploadIdentifier
                                INNER JOIN
                                    custom_cmds.VCmdsAchievement
                                        ON VCmdsAchievement.AchievementIdentifier = UploadRelation.ContainerIdentifier
                        )
        UNION ALL
        SELECT
                C.AchievementIdentifier,
                C.AchievementLabel,
                NULL                  AS ResourceNumber,
                A.AchievementTitle,
                C.CategoryName        AS CategoryTitle,
                U.UploadIdentifier,
                U.ContainerIdentifier AS UploadContainerIdentifier,
                U.Name                AS UploadName,
                U.Title               AS UploadTitle,
                U.ContentSize,
                U.UploadType,
                NULL,
                NULL
        FROM
                custom_cmds.VCmdsAchievementCategory AS C
            INNER JOIN
                resources.Upload                     AS U
                    ON C.AchievementIdentifier = U.ContainerIdentifier
            INNER JOIN
                achievements.QAchievement            AS A
                    ON A.AchievementIdentifier = C.AchievementIdentifier
            INNER JOIN
                accounts.QOrganization               AS T
                    ON T.OrganizationIdentifier = A.OrganizationIdentifier
        WHERE
                T.OrganizationIdentifier = @OrganizationIdentifier
                AND A.AchievementLabel = ISNULL(@AchievementType, A.AchievementLabel)
        ORDER BY
            CategoryTitle,
            AchievementLabel,
            ResourceNumber,
            AchievementTitle,
            UploadTitle,
            UploadIdentifier,
            CompetencyNumber;

        DROP TABLE #TempCompetency;
        DROP TABLE #TempAchievement;

    END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectPhoneList](
    @Departments VARCHAR(MAX)
, @RoleTypes VARCHAR(100)
, @Roles VARCHAR(MAX)
, @IsApproved BIT
, @OrganizationIdentifier UNIQUEIDENTIFIER
)
AS
BEGIN

    SELECT DISTINCT U.UserIdentifier
                  , U.LastName
                  , U.FirstName
                  , LOWER(U.Email)         AS Email
                  , P.PhoneWork            AS Phone
                  , WorkAddress.Street1    AS Street
                  , WorkAddress.City       AS City
                  , WorkAddress.Province   AS Province
                  , WorkAddress.PostalCode AS PostalCode
    FROM contacts.QPerson AS P
             INNER JOIN identities.[User] AS U ON U.UserIdentifier = P.UserIdentifier
             LEFT JOIN locations.Address AS WorkAddress ON P.WorkAddressIdentifier = WorkAddress.AddressIdentifier
    WHERE P.OrganizationIdentifier = @OrganizationIdentifier
      AND @Departments IS NOT NULL
      AND EXISTS
        ( SELECT *
          FROM contacts.Membership AS DU
          WHERE DU.UserIdentifier = U.UserIdentifier
            AND DU.GroupIdentifier IN
                ( SELECT CAST(ItemText AS UNIQUEIDENTIFIER)
                  FROM dbo.SplitText(@Departments, ',') )
            AND (
              @RoleTypes IS NULL
                  OR DU.MembershipType IN
                     ( SELECT ItemText
                       FROM dbo.SplitText(@RoleTypes, ',') )
              ) )
      AND (
        @Roles IS NULL
            OR EXISTS
            ( SELECT *
              FROM [contacts].Membership AS R
              WHERE R.UserIdentifier = U.UserIdentifier
                AND R.GroupIdentifier IN
                    ( SELECT CAST(ItemText AS UNIQUEIDENTIFIER)
                      FROM dbo.SplitText(@Roles, ',') ) )
        )
      AND (
        (
            @IsApproved = 1
                AND P.UserAccessGranted IS NOT NULL
            )
            OR
        (
            @IsApproved = 0
                AND P.UserAccessGranted IS NULL
            )
        )
      AND U.UtcArchived IS NULL
      AND U.Email IS NOT NULL
    ORDER BY U.LastName
           , U.FirstName
           , LOWER(U.Email);
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectProfileCompetenciesForInsert](
    @ProfileStandardIdentifier uniqueidentifier
)
AS
BEGIN
    select distinct
            pc.CompetencyStandardIdentifier as StandardIdentifier
            , cp.OrganizationIdentifier     as OrganizationIdentifier
    from custom_cmds.ProfileCompetency                   pc
            inner join custom_cmds.VCmdsProfileOrganization cp on cp.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
    where (@ProfileStandardIdentifier IS NULL OR pc.ProfileStandardIdentifier = @ProfileStandardIdentifier)
            and not exists (
                                select *
                                from standards.StandardOrganization as x
                                where x.StandardIdentifier = pc.CompetencyStandardIdentifier
                                    and x.OrganizationIdentifier = cp.OrganizationIdentifier
                            );
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [custom_cmds].[SelectQUserStatus]
(
    @OrganizationIdentifier uniqueidentifier
  , @Departments as IdentifierList readonly
  , @Users as IdentifierList readonly
  , @Sequences as IntegerList readonly
  , @Option int -- 1 = primary profiles only; 2 = profiles that require compliance; 3 = all profiles
  , @ExcludeUsersWithoutProfile bit
) as;
with                                   Scores as (
                                                     select OrganizationIdentifier
                                                          , OrganizationName        as CompanyName
                                                          , DepartmentIdentifier
                                                          , DepartmentName
                                                          , UserIdentifier
                                                          , UserFullName
                                                          , PrimaryProfileIdentifier
                                                          , PrimaryProfileNumber
                                                          , PrimaryProfileTitle
                                                          , ItemNumber              as Sequence
                                                          , ItemName                as Heading
                                                          , case @Option
                                                                when 1 then
                                                                    CountRQ_Primary
                                                                when 2 then
                                                                    CountRQ_Mandatory
                                                                else
                                                                    CountRQ
                                                            end                     as Required
                                                          , case @Option
                                                                when 1 then
                                                                    CountVAVNCP_Primary
                                                                when 2 then
                                                                    CountVAVNCP_Mandatory
                                                                else
                                                                    CountVAVNCP
                                                            end                     as Satisfied
                                                          , case @Option
                                                                when 1 then
                                                                    Score_Primary
                                                                when 2 then
                                                                    Score_Mandatory
                                                                else
                                                                    Score
                                                            end                     as Score
                                                          , case @Option
                                                                when 1 then
                                                                    CountEX_Primary
                                                                when 2 then
                                                                    CountEX_Mandatory
                                                                else
                                                                    CountEX
                                                            end                     as Expired
                                                          , case @Option
                                                                when 1 then
                                                                    CountNC_Primary
                                                                when 2 then
                                                                    CountNC_Mandatory
                                                                else
                                                                    CountNC
                                                            end                     as NotCompleted
                                                          , case @Option
                                                                when 1 then
                                                                    CountNA_Primary
                                                                when 2 then
                                                                    CountNA_Mandatory
                                                                else
                                                                    CountNA
                                                            end                     as NotApplicable
                                                          , case @Option
                                                                when 1 then
                                                                    CountNT_Primary
                                                                when 2 then
                                                                    CountNT_Mandatory
                                                                else
                                                                    CountNT
                                                            end                     as NeedsTraining
                                                          , case @Option
                                                                when 1 then
                                                                    CountSA_Primary
                                                                when 2 then
                                                                    CountSA_Mandatory
                                                                else
                                                                    CountSA
                                                            end                     as SelfAssessed
                                                          , case @Option
                                                                when 1 then
                                                                    CountSV_Primary
                                                                when 2 then
                                                                    CountSV_Mandatory
                                                                else
                                                                    CountSV
                                                            end                     as Submitted
                                                          , case @Option
                                                                when 1 then
                                                                    CountVA_Primary
                                                                when 2 then
                                                                    CountVAVN_Mandatory
                                                                else
                                                                    CountVA
                                                            end                     as Validated
                                                     from custom_cmds.QUserStatus
                                                     where OrganizationIdentifier = @OrganizationIdentifier
                                                           and (
                                                                   DepartmentIdentifier in (
                                                                                               select IdentifierItem from @Departments
                                                                                           )
                                                                   or 0 = (
                                                                              select count(*)from @Departments
                                                                          )
                                                               )
                                                           and (
                                                                   UserIdentifier in (
                                                                                         select IdentifierItem from @Users
                                                                                     )
                                                                   or 0 = (
                                                                              select count(*)from @Users
                                                                          )
                                                               )
                                                           and (
                                                                   ItemNumber in (
                                                                                     select IntegerItem from @Sequences
                                                                                 )
                                                                   or 0 = (
                                                                              select count(*)from @Sequences
                                                                          )
                                                               )
                                                           and (
                                                                   @Option = 1
                                                                   and PrimaryProfileIdentifier is not null
                                                                   or @Option in ( 2, 3 )
                                                               )
                                                 )
select newid()
    as
    SummaryIdentifier
     , Scores.OrganizationIdentifier
     , Scores.CompanyName
     , Scores.DepartmentIdentifier
     , Scores.DepartmentName
     , Scores.UserIdentifier
     , Scores.UserFullName
     , Scores.PrimaryProfileIdentifier
     , Scores.PrimaryProfileNumber
     , Scores.PrimaryProfileTitle
     , Scores.Sequence
     , Scores.Heading
     , Scores.Required
     , Scores.Satisfied
     , Scores.Score
     , Scores.Expired
     , Scores.NotCompleted
     , Scores.NotApplicable
     , Scores.NeedsTraining
     , Scores.SelfAssessed
     , Scores.Submitted
     , Scores.Validated
from Scores
where (
          @ExcludeUsersWithoutProfile = 0
          or UserIdentifier in (
                                   select UserIdentifier from Scores where Required <> 0
                               )
      )
order by UserFullName
       , Sequence;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectQUserStatusPerDepartment]
    (
        @OrganizationIdentifier           UNIQUEIDENTIFIER
      , @Departments AS             IdentifierList READONLY
      , @Users AS                   IdentifierList READONLY
      , @Sequences AS               IntegerList    READONLY
      , @StartDate                  DATE
      , @EndDate                    DATE
      , @Option                     INT -- 1 = primary profiles only; 2 = profiles that require compliance
      , @ExcludeUsersWithoutProfile BIT
      , @AccessDepartment           BIT
      , @AccessCompany              BIT
      , @AccessAdministration       BIT
    )
AS
    SELECT
        UserStatus.DepartmentIdentifier
      , UserStatus.DepartmentName
      , (
            SELECT
                COUNT(*)
            FROM
                contacts.Membership AS DU
            WHERE
                DU.GroupIdentifier = UserStatus.DepartmentIdentifier
                AND
                    (
                        (
                            DU.MembershipType = 'Department'
                            AND @AccessDepartment = 1
                        )
                        OR
                            (
                                DU.MembershipType = 'Company'
                                AND @AccessCompany = 1
                            )
                        OR
                            (
                                DU.MembershipType = 'Administration'
                                AND @AccessAdministration = 1
                            )
                    )
        ) AS UserCount
    FROM
        custom_cmds.QUserStatus AS UserStatus
    WHERE
        OrganizationIdentifier = @OrganizationIdentifier
        AND
            (
                UserStatus.DepartmentIdentifier IN
                    (
                        SELECT
                            IdentifierItem
                        FROM
                            @Departments
                    )
                OR 0 =
                    (
                        SELECT
                            COUNT(*)
                        FROM
                            @Departments
                    )
            )
        AND
            (
                UserStatus.UserIdentifier IN
                    (
                        SELECT
                            IdentifierItem
                        FROM
                            @Users
                    )
                OR 0 =
                    (
                        SELECT
                            COUNT(*)
                        FROM
                            @Users
                    )
            )
        AND
            (
                PrimaryProfileIdentifier IS NOT NULL
                OR @Option = 2
            )
        AND
            (
                UserStatus.ItemNumber IN
                    (
                        SELECT
                            IntegerItem
                        FROM
                            @Sequences
                    )
                OR 0 =
                    (
                        SELECT
                            COUNT(*)
                        FROM
                            @Sequences
                    )
            )
        AND
            (
                @ExcludeUsersWithoutProfile = 0
                OR (CASE @Option
                        WHEN 1
                            THEN
                            UserStatus.CountRQ_Primary
                        WHEN 2
                            THEN
                            UserStatus.CountRQ_Mandatory
                        ELSE
                            CountRQ
                    END
                   ) <> 0
            )
    GROUP BY
        DepartmentIdentifier
      , DepartmentName
    ORDER BY
        DepartmentName;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectTrainingCompletionDates](
  @Departments NVARCHAR(MAX)
, @Resources NVARCHAR(MAX)
, @IsRequired BIT
, @CredentialGrantedStartDate DATE
, @CredentialGrantedEndDate DATE
, @CredentialStatus VARCHAR(16)
, @MembershipType VARCHAR(200)
, @IncludeSelfDeclaredCredentials BIT
) AS
BEGIN
    ;
    SET NOCOUNT ON;

    WITH CTE AS ( SELECT ISNULL(ec.EmployerGroupName, Organization.CompanyTitle) AS CompanyName
                       , Department.DepartmentName
                       , p.UserIdentifier
                       , Person.FullName
                       , c.AchievementTitle                                      AS ResourceTitle
                       , c.AchievementLabel
                       , ec.CredentialGranted                                    AS DateCompleted
                       , ec.CredentialExpirationExpected                         AS ExpirationDate
                       , ec.CredentialIsMandatory                                AS IsRequired
                       , CASE
            WHEN ec.CredentialExpirationLifetimeQuantity IS NOT NULL AND
                 ec.CredentialExpirationLifetimeUnit IS NOT NULL THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
            END                                                                  AS IsTimeSensitive
                       , ec.CredentialStatus
                       , CASE ec.CredentialStatus WHEN 'Valid' THEN ec.CredentialGrantedScore
                                                  ELSE NULL END                  AS GradePercent
                  FROM custom_cmds.VCmdsCredential AS ec
                           INNER JOIN custom_cmds.VCmdsAchievement AS c
                                      ON c.AchievementIdentifier = ec.AchievementIdentifier
                           INNER JOIN contacts.Membership AS m ON m.UserIdentifier = ec.UserIdentifier
                           INNER JOIN custom_cmds.ActiveUser AS p ON p.UserIdentifier = ec.UserIdentifier
                           INNER JOIN identities.Department ON Department.DepartmentIdentifier = m.GroupIdentifier
                           INNER JOIN accounts.QOrganization AS Organization
                                      ON Organization.OrganizationIdentifier = Department.OrganizationIdentifier
                           INNER JOIN contacts.QPerson AS Person
                                      ON Person.OrganizationIdentifier = Department.OrganizationIdentifier AND
                                         Person.UserIdentifier = p.UserIdentifier
                  WHERE (@CredentialStatus IS NULL OR ec.CredentialStatus = @CredentialStatus)
                    AND m.GroupIdentifier IN
                        ( SELECT CAST(ItemText AS UNIQUEIDENTIFIER) FROM dbo.SplitText(@Departments, ',') )
                    AND ec.AchievementIdentifier IN
                        ( SELECT CAST(ItemText AS UNIQUEIDENTIFIER) FROM dbo.SplitText(@Resources, ',') )
                    AND (@IsRequired IS NULL OR ec.CredentialIsMandatory = @IsRequired)
                    AND (@CredentialGrantedStartDate IS NULL OR
                         CAST(ec.CredentialGranted AS DATE) >= @CredentialGrantedStartDate)
                    AND (@CredentialGrantedEndDate IS NULL OR
                         CAST(ec.CredentialGranted AS DATE) <= @CredentialGrantedEndDate)
                    AND m.MembershipType IN ( SELECT ItemText
                                              FROM dbo.SplitText(@MembershipType, ',') )
                    AND (ec.AuthorityType IS NULL OR ec.AuthorityType <> 'Self' OR
                         @IncludeSelfDeclaredCredentials = 1) )
    SELECT CTE.CompanyName
         , STRING_AGG(CTE.DepartmentName, '; ') WITHIN GROUP (ORDER BY CTE.DepartmentName) AS DepartmentName
         , CTE.UserIdentifier
         , CTE.FullName
         , CTE.ResourceTitle
         , CTE.ResourceTitle                                                               AS AchievementTitle
         , CTE.AchievementLabel
         , CTE.DateCompleted
         , CTE.ExpirationDate
         , CTE.IsRequired
         , CTE.IsTimeSensitive
         , CTE.GradePercent
         , CTE.CredentialStatus
    FROM CTE
    GROUP BY CTE.CompanyName, CTE.UserIdentifier, CTE.FullName, CTE.ResourceTitle, CTE.AchievementLabel
           , CTE.DateCompleted
           , CTE.ExpirationDate, CTE.IsRequired, CTE.IsTimeSensitive, CTE.GradePercent, CTE.CredentialStatus
    OPTION (OPTIMIZE FOR UNKNOWN);

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectTrainingExpiryDates](
    @Departments NVARCHAR(MAX),
    @Resources NVARCHAR(MAX),
    @IsRequired BIT,
    @MinimumPassingGrade DECIMAL(10, 4)
) AS
SELECT Organization.OrganizationIdentifier,
       Organization.CompanyTitle   AS  CompanyName,
       Department.DepartmentIdentifier,
       Department.DepartmentName,
       p.UserIdentifier,
       p.FirstName,
       p.LastName,
       Person.FullName,
       c.AchievementIdentifier,
       c.AchievementTitle,
       ec.CredentialGranted            DateCompleted,
       ec.CredentialExpirationExpected ExpirationDate,
       ec.CredentialIsMandatory    AS  IsRequired,
       CASE
           WHEN ec.CredentialExpirationLifetimeQuantity IS NOT NULL AND ec.CredentialExpirationLifetimeUnit IS NOT NULL
               THEN CAST(1 AS BIT)
           ELSE CAST(0 AS BIT) END AS  IsTimeSensitive,
       CASE
           WHEN ec.CredentialStatus = 'Expired' OR
                ec.CredentialStatus = 'Valid' AND ec.CredentialExpirationExpected < GETUTCDATE() THEN 'Expired'
           WHEN ec.CredentialStatus = 'Valid' THEN 'Valid'
           ELSE 'Pending' END      AS  Status,
       CASE
           WHEN (ec.CredentialStatus = 'Expired' OR ec.CredentialStatus = 'Valid') AND
                (c.AchievementLabel = 'Time-Sensitive Safety Certificate' OR
                 c.AchievementLabel = 'Additional Compliance Requirement' OR
                 (ec.CredentialGranted IS NOT NULL AND c.AchievementLabel NOT IN ('Module', 'HR Learning Module')))
               THEN CAST(1 AS BIT)
           ELSE CAST(0 AS BIT) END AS  IsQuizPassed
FROM custom_cmds.VCmdsCredential AS ec WITH (NOLOCK)
         INNER JOIN custom_cmds.VCmdsAchievement AS c WITH (NOLOCK)
                    ON c.AchievementIdentifier = ec.AchievementIdentifier
         INNER JOIN contacts.Membership AS m WITH (NOLOCK) ON m.UserIdentifier = ec.UserIdentifier
         INNER JOIN custom_cmds.ActiveUser AS p WITH (NOLOCK) ON p.UserIdentifier = ec.UserIdentifier
         INNER JOIN identities.Department WITH (NOLOCK) ON Department.DepartmentIdentifier = m.GroupIdentifier
         INNER JOIN accounts.QOrganization AS Organization WITH (NOLOCK)
                    ON Organization.OrganizationIdentifier = Department.OrganizationIdentifier
         INNER JOIN contacts.QPerson AS Person ON Person.OrganizationIdentifier = Department.OrganizationIdentifier AND
                                                  Person.UserIdentifier = p.UserIdentifier
WHERE m.GroupIdentifier IN ( SELECT CAST(ItemText AS UNIQUEIDENTIFIER) FROM dbo.SplitText(@Departments, ',') )
  AND ec.AchievementIdentifier IN ( SELECT CAST(ItemText AS UNIQUEIDENTIFIER) FROM dbo.SplitText(@Resources, ',') )
  AND (@IsRequired IS NULL OR ec.CredentialIsMandatory = @IsRequired)
  AND m.MembershipType = 'Department';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectTrainingRequirementsPerCompetency](
    @Departments IdentifierList READONLY
, @Status NVARCHAR(1024)
) AS
SELECT DISTINCT Department.DepartmentIdentifier
              , Department.DepartmentName
              , p.UserIdentifier
              , QPerson.FullName
              , c.Code                                              AS Number
              , ISNULL(CompetencyTitle.ContentText, c.ContentTitle) AS Summary
FROM identities.Department
         INNER JOIN contacts.Membership AS m ON m.GroupIdentifier = Department.DepartmentIdentifier
         INNER JOIN identities.[User] AS p ON p.UserIdentifier = m.UserIdentifier
         INNER JOIN contacts.QPerson ON QPerson.UserIdentifier = m.UserIdentifier AND
                                        QPerson.OrganizationIdentifier = Department.OrganizationIdentifier
         INNER JOIN standards.DepartmentProfileUser AS ep ON ep.UserIdentifier = p.UserIdentifier AND
                                                             ep.DepartmentIdentifier = Department.DepartmentIdentifier
         INNER JOIN standards.StandardContainment AS pc ON pc.ParentStandardIdentifier = ep.ProfileStandardIdentifier
         INNER JOIN standards.DepartmentProfileCompetency AS cs
                    ON cs.DepartmentIdentifier = Department.DepartmentIdentifier AND
                       cs.CompetencyStandardIdentifier = pc.ChildStandardIdentifier AND
                       cs.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
         INNER JOIN standards.StandardValidation AS ec
                    ON ec.StandardIdentifier = pc.ChildStandardIdentifier AND ec.UserIdentifier = p.UserIdentifier
         INNER JOIN standards.Standard AS c ON c.StandardIdentifier = pc.ChildStandardIdentifier
         LEFT JOIN contents.TContent AS CompetencyTitle
                   ON CompetencyTitle.ContainerIdentifier = c.StandardIdentifier AND
                      CompetencyTitle.ContentLabel = 'Title' AND CompetencyTitle.ContentLanguage = 'en'
WHERE Department.DepartmentIdentifier IN ( SELECT IdentifierItem FROM @Departments )
  AND m.MembershipType = 'Department'
  AND p.UtcArchived IS NULL
  AND QPerson.UserAccessGranted IS NOT NULL
  AND ec.ValidationStatus = @Status
ORDER BY DepartmentName, DepartmentIdentifier, Number, FullName;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectTrainingRequirementsPerUser](
    @OrganizationIdentifier UNIQUEIDENTIFIER
, @Departments VARCHAR(MAX)
, @Status VARCHAR(MAX)
) AS
SELECT DISTINCT Organization.OrganizationIdentifier
              , Organization.CompanyTitle  AS CompanyName
              , Department.GroupIdentifier AS DepartmentIdentifier
              , Department.GroupName       AS DepartmentName
              , U.UserIdentifier
              , QPerson.FullName
              , Competency.Number
              , Competency.Summary
FROM contacts.QGroup AS Department WITH (NOLOCK)
         INNER JOIN accounts.QOrganization AS Organization WITH (NOLOCK)
                    ON Organization.OrganizationIdentifier = Department.OrganizationIdentifier
         INNER JOIN contacts.Membership WITH (NOLOCK) ON Membership.GroupIdentifier = Department.GroupIdentifier
         INNER JOIN identities.[User] AS U WITH (NOLOCK) ON U.UserIdentifier = Membership.UserIdentifier
         INNER JOIN contacts.QPerson WITH (NOLOCK) ON QPerson.UserIdentifier = Membership.UserIdentifier AND
                                                      QPerson.OrganizationIdentifier = Department.OrganizationIdentifier
         INNER JOIN custom_cmds.UserCompetency WITH (NOLOCK)
                    ON UserCompetency.UserIdentifier = Membership.UserIdentifier
         INNER JOIN standards.DepartmentProfileUser WITH (NOLOCK)
                    ON DepartmentProfileUser.UserIdentifier = Membership.UserIdentifier AND
                       DepartmentProfileUser.DepartmentIdentifier = Department.GroupIdentifier
         INNER JOIN standards.StandardContainment WITH (NOLOCK) ON StandardContainment.ParentStandardIdentifier =
                                                                   DepartmentProfileUser.ProfileStandardIdentifier AND
                                                                   StandardContainment.ChildStandardIdentifier =
                                                                   UserCompetency.CompetencyStandardIdentifier
         INNER JOIN standards.DepartmentProfileCompetency WITH (NOLOCK)
                    ON DepartmentProfileCompetency.DepartmentIdentifier = Department.GroupIdentifier AND
                       DepartmentProfileCompetency.CompetencyStandardIdentifier =
                       StandardContainment.ChildStandardIdentifier AND
                       DepartmentProfileCompetency.ProfileStandardIdentifier =
                       DepartmentProfileUser.ProfileStandardIdentifier
         INNER JOIN custom_cmds.Competency WITH (NOLOCK)
                    ON Competency.CompetencyStandardIdentifier = StandardContainment.ChildStandardIdentifier
WHERE Department.OrganizationIdentifier = @OrganizationIdentifier
  AND (@Departments IS NULL OR EXISTS
    ( SELECT *
      FROM dbo.SplitText(@Departments, ',')
      WHERE CAST(ItemText AS UNIQUEIDENTIFIER) = Department.GroupIdentifier ))
  AND Competency.IsDeleted = 0
  AND UserCompetency.ValidationStatus = @Status
  AND QPerson.UserAccessGranted IS NOT NULL
ORDER BY CompanyName, DepartmentName, FullName, Number
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectUnusedCompanyCompetencyByOrganization](
    @OrganizationIdentifier uniqueidentifier
)
AS
    SELECT DISTINCT
        StandardOrganization.StandardIdentifier
       ,StandardOrganization.OrganizationIdentifier
    FROM
        standards.StandardOrganization
        INNER JOIN
        custom_cmds.VCmdsCompetencyOrganization cc ON cc.CompetencyStandardIdentifier = StandardOrganization.StandardIdentifier
                                 AND cc.OrganizationIdentifier             = StandardOrganization.OrganizationIdentifier
    WHERE
        cc.CompetencyStandardIdentifier NOT IN
        (
            SELECT
                pc.CompetencyStandardIdentifier
            FROM
                custom_cmds.ProfileCompetency pc
                INNER JOIN
                custom_cmds.VCmdsProfileOrganization    cp ON cp.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
            WHERE
                cp.OrganizationIdentifier = cc.OrganizationIdentifier
        )
      AND cc.OrganizationIdentifier = @OrganizationIdentifier;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectUnusedCompanyCompetencyByProfile](
    @ProfileStandardIdentifier uniqueidentifier
)
AS
BEGIN
    CREATE TABLE #ProfileCompetency
    (
        CompetencyStandardIdentifier uniqueidentifier NOT NULL
        , OrganizationIdentifier             uniqueidentifier NOT NULL
        , PRIMARY KEY (
                        CompetencyStandardIdentifier
                        , OrganizationIdentifier
                    )
    );

    INSERT #ProfileCompetency
    SELECT DISTINCT
            ProfileCompetency.CompetencyStandardIdentifier
            , VCmdsProfileOrganization.OrganizationIdentifier
    FROM
        custom_cmds.ProfileCompetency
        INNER JOIN
        custom_cmds.VCmdsProfileOrganization ON VCmdsProfileOrganization.ProfileStandardIdentifier            = ProfileCompetency.ProfileStandardIdentifier
        INNER JOIN
        custom_cmds.VCmdsProfileOrganization AS CompanyProfile2 ON CompanyProfile2.OrganizationIdentifier = VCmdsProfileOrganization.OrganizationIdentifier
    WHERE
        CompanyProfile2.ProfileStandardIdentifier = @ProfileStandardIdentifier;

    SELECT DISTINCT
        StandardOrganization.StandardIdentifier
       ,StandardOrganization.OrganizationIdentifier
    FROM
        standards.StandardOrganization
        INNER JOIN
        custom_cmds.VCmdsCompetencyOrganization cc ON cc.CompetencyStandardIdentifier = StandardOrganization.StandardIdentifier
                                    AND cc.OrganizationIdentifier             = StandardOrganization.OrganizationIdentifier
        INNER JOIN
        custom_cmds.VCmdsProfileOrganization    cp ON cp.OrganizationIdentifier             = cc.OrganizationIdentifier
    WHERE
        NOT EXISTS
    (
        SELECT
            *
        FROM
            #ProfileCompetency
        WHERE
            CompetencyStandardIdentifier = cc.CompetencyStandardIdentifier
            AND OrganizationIdentifier           = cp.OrganizationIdentifier
    )
        AND cp.ProfileStandardIdentifier = @ProfileStandardIdentifier;

    DROP TABLE #ProfileCompetency;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectUserStatusResource]
(
    @OrganizationIdentifier          uniqueidentifier
  , @DepartmentIdentifier uniqueidentifier
  , @UserIdentifier            uniqueidentifier
)
AS
BEGIN
    SELECT DISTINCT
        Organization.OrganizationIdentifier
       ,Department.DepartmentIdentifier
       ,Person.UserIdentifier
       ,A.AchievementIdentifier as ResourceIdentifier
       ,AchievementLabel as ResourceType
       ,ResourceTitleEn.ContentText                                             AS ResourceTitle
       ,PersonAssignment.CredentialStatus as ValidationStatus
       ,CASE CredentialStatus WHEN 'Completed' THEN 1 ELSE 0 END                AS CountCP
       ,CASE CredentialStatus WHEN 'Expired' THEN 1 ELSE 0 END                  AS CountEX
       ,CASE CredentialStatus WHEN 'Not Completed' THEN 1 ELSE 0 END            AS CountNC
       ,CASE CredentialStatus WHEN 'Not Applicable' THEN 1 ELSE 0 END           AS CountNA
       ,CASE CredentialStatus WHEN 'Needs Training' THEN 1 ELSE 0 END           AS CountNT
       ,CASE CredentialStatus WHEN 'Self-Assessed' THEN 1 ELSE 0 END            AS CountSA
       ,CASE CredentialStatus WHEN 'Submitted for Validation' THEN 1 ELSE 0 END AS CountSV
       ,CASE CredentialStatus WHEN 'Validated' THEN 1 ELSE 0 END                AS CountVA
       ,CASE PersonAssignment.CredentialNecessity WHEN 'Mandatory' THEN 1 ELSE 0 END               AS CountRQ
    FROM
        contacts.Membership AS xy
        INNER JOIN identities.Department
            ON xy.GroupIdentifier = Department.DepartmentIdentifier
        INNER JOIN accounts.QOrganization AS Organization
            ON Department.OrganizationIdentifier = Organization.OrganizationIdentifier
        INNER JOIN identities.[User] AS Person
            ON xy.UserIdentifier = Person.UserIdentifier
        INNER JOIN achievements.QCredential AS PersonAssignment
            ON PersonAssignment.UserIdentifier = Person.UserIdentifier
        INNER JOIN achievements.QAchievement as A
            ON PersonAssignment.AchievementIdentifier = A.AchievementIdentifier
        LEFT JOIN contents.TContent AS ResourceTitleEn
            ON ResourceTitleEn.ContainerIdentifier = A.AchievementIdentifier
               AND ResourceTitleEn.ContentLabel = 'Title'
               AND ResourceTitleEn.ContentLanguage = 'en'
                
    WHERE
        Organization.OrganizationIdentifier = @OrganizationIdentifier
        AND Person.AccessGrantedToCmds = 1
        AND Person.UtcArchived IS NULL
        AND PersonAssignment.CredentialStatus IS NOT NULL
        AND A.AchievementLabel NOT IN ('Link', 'Lesson', 'Assessment')
        AND (
            A.OrganizationIdentifier = Department.OrganizationIdentifier OR EXISTS(
                SELECT
                    *
                FROM
                    achievements.TAchievementOrganization
                WHERE
                    TAchievementOrganization.OrganizationIdentifier = Department.OrganizationIdentifier
                    AND TAchievementOrganization.AchievementIdentifier = A.AchievementIdentifier
            )
        )
        AND PersonAssignment.CredentialPriority = 'Planned'
        AND PersonAssignment.CredentialNecessity = 'Mandatory'
        AND (@DepartmentIdentifier IS NULL OR Department.DepartmentIdentifier = @DepartmentIdentifier)
        AND (@UserIdentifier IS NULL OR Person.UserIdentifier = @UserIdentifier);
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectUserStatusStandard]
    (
        @OrganizationIdentifier     UNIQUEIDENTIFIER
      , @DepartmentIdentifier UNIQUEIDENTIFIER
      , @UserIdentifier       UNIQUEIDENTIFIER
    )
AS
    BEGIN;
        WITH OrganizationUserCompetency AS (SELECT
                                          DPU.DepartmentIdentifier
                                        , UserCompetency.UserIdentifier
                                        , UserCompetency.StandardIdentifier                 AS CompetencyStandardIdentifier
                                        , CAST(MAX(DPC.IsCritical + 0) AS BIT)              AS IsCritical
                                        , CAST(MAX(DPU.IsPrimary + 0) AS BIT)               AS IsPrimary
                                        , CAST(MAX(DPU.IsRequired + 0) AS BIT)              AS IsRequired
                                        , STRING_AGG(Profile.Code, ', ') WITHIN GROUP (ORDER BY Profile.Code) AS ProfileNumbers
                                        , COUNT(DISTINCT Profile.Code)                      AS ProfileCount
                                        , COUNT(DISTINCT UserCompetency.StandardIdentifier) AS CompetencyCount
                                      FROM
                                          standards.DepartmentProfileUser              AS DPU
                                      INNER JOIN standards.Standard                    AS Profile
                                                 ON Profile.StandardIdentifier = DPU.ProfileStandardIdentifier

                                      INNER JOIN standards.DepartmentProfileCompetency AS DPC
                                                 ON DPC.DepartmentIdentifier = DPU.DepartmentIdentifier
                                                    AND DPC.ProfileStandardIdentifier = DPU.ProfileStandardIdentifier

                                      INNER JOIN standards.StandardContainment         AS ProfileCompetency
                                                 ON ProfileCompetency.ParentStandardIdentifier = DPU.ProfileStandardIdentifier
                                                    AND ProfileCompetency.ChildStandardIdentifier = DPC.CompetencyStandardIdentifier

                                      INNER JOIN standards.StandardValidation          AS UserCompetency
                                                 ON UserCompetency.UserIdentifier = DPU.UserIdentifier
                                                    AND UserCompetency.StandardIdentifier = ProfileCompetency.ChildStandardIdentifier
                                      GROUP BY
                                          UserCompetency.UserIdentifier
                                        , DPU.DepartmentIdentifier
                                        , UserCompetency.StandardIdentifier)
        SELECT
            Department.OrganizationIdentifier
          , Department.DepartmentIdentifier
          , U.UserIdentifier
          , S.StandardIdentifier
          , S.StandardIdentifier
          , STitle.ContentText                                     AS StandardTitle
          , ISNULL(S.StandardLabel, S.StandardType) + ' ' + S.Code AS StandardMnemonic
          , CASE
                WHEN OrganizationUserCompetency.ProfileCount = 0
                    THEN
                    'No Profile'
                WHEN OrganizationUserCompetency.ProfileCount = 1
                    THEN
                    'Profile '
                ELSE
                    'Profiles '
            END + OrganizationUserCompetency.ProfileNumbers              AS StandardMetadata
          , V.ValidationStatus
          , CASE OrganizationUserCompetency.IsRequired
                WHEN 1
                    THEN
                    'Mandatory '
                ELSE
                    'Optional '
            END + CASE OrganizationUserCompetency.IsCritical
                      WHEN 0
                          THEN
                          'Non-Critical '
                      ELSE
                          'Critical '
                  END + CASE OrganizationUserCompetency.IsPrimary
                            WHEN 0
                                THEN
                                'Secondary '
                            ELSE
                                'Primary '
                        END + 'Competency'                         AS StatisticName
          , CASE V.ValidationStatus
                WHEN 'Completed'
                    THEN
                    1
                ELSE
                    0
            END                                                    AS CountCP
          , CASE V.ValidationStatus
                WHEN 'Expired'
                    THEN
                    1
                ELSE
                    0
            END                                                    AS CountEX
          , CASE V.ValidationStatus
                WHEN 'Not Completed'
                    THEN
                    1
                ELSE
                    0
            END                                                    AS CountNC
          , CASE
                WHEN V.ValidationStatus = 'Not Applicable'
                     AND V.IsValidated = 0
                    THEN
                    1
                ELSE
                    0
            END                                                    AS CountNA
          , CASE V.ValidationStatus
                WHEN 'Needs Training'
                    THEN
                    1
                ELSE
                    0
            END                                                    AS CountNT
          , CASE V.ValidationStatus
                WHEN 'Self-Assessed'
                    THEN
                    1
                ELSE
                    0
            END                                                    AS CountSA
          , CASE V.ValidationStatus
                WHEN 'Submitted for Validation'
                    THEN
                    1
                ELSE
                    0
            END                                                    AS CountSV
          , CASE V.ValidationStatus
                WHEN 'Validated'
                    THEN
                    1
                ELSE
                    0
            END                                                    AS CountVA
          , CASE
                WHEN V.ValidationStatus = 'Not Applicable'
                     AND V.IsValidated = 1
                    THEN
                    1
                ELSE
                    0
            END                                                    AS CountVN
          , 1                                                      AS CountRQ
        FROM
            standards.StandardValidation AS V
        INNER JOIN standards.Standard    AS S
                   ON S.StandardIdentifier = V.StandardIdentifier

        INNER JOIN identities.[User]     AS U
                   ON U.UserIdentifier = V.UserIdentifier

        INNER JOIN OrganizationUserCompetency
                   ON OrganizationUserCompetency.UserIdentifier = V.UserIdentifier
                      AND OrganizationUserCompetency.CompetencyStandardIdentifier = V.StandardIdentifier

        INNER JOIN identities.Department
                   ON Department.DepartmentIdentifier = OrganizationUserCompetency.DepartmentIdentifier

        LEFT JOIN contents.TContent      AS STitle
                  ON STitle.ContainerIdentifier = S.StandardIdentifier
                     AND STitle.ContentLabel = 'Title'
                     AND STitle.ContentLanguage = 'en'
        WHERE
            Department.OrganizationIdentifier = @OrganizationIdentifier
            AND S.StandardType = 'Competency'
            AND U.AccessGrantedToCmds = 1
            AND U.UtcArchived IS NULL
            AND
                (
                    @UserIdentifier IS NULL
                    OR U.UserIdentifier = @UserIdentifier
                )
            AND
                (
                    @DepartmentIdentifier IS NULL
                    OR Department.DepartmentIdentifier = @DepartmentIdentifier
                );
    END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectZUserStatus]
    (
        @OrganizationIdentifier           UNIQUEIDENTIFIER
      , @Departments AS             IdentifierList READONLY
      , @Users AS                   IdentifierList READONLY
      , @Sequences AS               IntegerList    READONLY
      , @StartDate                  DATE
      , @EndDate                    DATE
      , @Option                     INT -- 1 = primary profiles only; 2 = profiles that require compliance
      , @ExcludeUsersWithoutProfile BIT
    )
AS
    BEGIN
        DECLARE
            @SnapshotDate1 DATE
          , @SnapshotDate2 DATE
          , @SnapshotDate3 DATE;

        SELECT
            @SnapshotDate1 = CASE
                                 WHEN ROW_NUMBER() OVER (ORDER BY
                                                             SnapshotDate DESC
                                                        ) = 1
                                     THEN
                                     SnapshotDate
                                 ELSE
                                     @SnapshotDate1
                             END
          , @SnapshotDate2 = CASE
                                 WHEN ROW_NUMBER() OVER (ORDER BY
                                                             SnapshotDate DESC
                                                        ) = 2
                                     THEN
                                     SnapshotDate
                                 ELSE
                                     @SnapshotDate2
                             END
          , @SnapshotDate3 = CASE
                                 WHEN ROW_NUMBER() OVER (ORDER BY
                                                             SnapshotDate DESC
                                                        ) = 3
                                     THEN
                                     SnapshotDate
                                 ELSE
                                     @SnapshotDate3
                             END
        FROM
            (
                SELECT DISTINCT TOP 3
                       AsAt AS SnapshotDate
                FROM
                    custom_cmds.ZUserStatus
                WHERE
                    AsAt
                BETWEEN @StartDate AND @EndDate
                ORDER BY
                    AsAt DESC
            ) t;

        WITH Scores AS (SELECT
                            OrganizationIdentifier
                          , OrganizationName              AS CompanyName
                          , DepartmentIdentifier
                          , DepartmentName
                          , UserIdentifier
                          , UserFullName
                          , PrimaryProfileIdentifier
                          , PrimaryProfileNumber
                          , PrimaryProfileTitle
                          , ItemNumber              AS Sequence
                          , ItemName                AS Heading
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                                     CountRQ_Primary
                                 WHEN 2
                                     THEN
                                     CountRQ_Mandatory
                                 ELSE
                                     CountRQ
                             END
                            )                       AS Required
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                                     CountVAVNCP_Primary
                                 WHEN 2
                                     THEN
                                     CountVAVNCP_Mandatory
                                 ELSE
                                     CountVAVNCP
                             END
                            )                       AS Satisfied
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                          (CASE
                               WHEN CountRQ_Primary > 0
                                   THEN
                          (CAST(ISNULL(CountVAVNCP_Primary, 0) AS DECIMAL(16, 3))) / CountRQ_Primary
                               ELSE
                                   1
                           END
                          )
                                 WHEN 2
                                     THEN
                          (CASE
                               WHEN CountRQ_Mandatory > 0
                                   THEN
                          (CAST(ISNULL(CountVAVNCP_Mandatory, 0) AS DECIMAL(16, 3))) / CountRQ_Mandatory
                               ELSE
                                   1
                           END
                          )
                                 ELSE
                          (CASE
                               WHEN CountRQ > 0
                                   THEN
                          (CAST(ISNULL(CountVAVNCP, 0) AS DECIMAL(16, 3))) / CountRQ
                               ELSE
                                   1
                           END
                          )
                             END
                            )                       AS Score
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                                     CountEX_Primary
                                 WHEN 2
                                     THEN
                                     CountEX_Mandatory
                                 ELSE
                                     CountEX
                             END
                            )                       AS Expired
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                                     CountNC_Primary
                                 WHEN 2
                                     THEN
                                     CountNC_Mandatory
                                 ELSE
                                     CountNC
                             END
                            )                       AS NotCompleted
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                                     CountNA_Primary
                                 WHEN 2
                                     THEN
                                     CountNA_Mandatory
                                 ELSE
                                     CountNA
                             END
                            )                       AS NotApplicable
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                                     CountNT_Primary
                                 WHEN 2
                                     THEN
                                     CountNT_Mandatory
                                 ELSE
                                     CountNT
                             END
                            )                       AS NeedsTraining
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                                     CountSA_Primary
                                 WHEN 2
                                     THEN
                                     CountSA_Mandatory
                                 ELSE
                                     CountSA
                             END
                            )                       AS SelfAssessed
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                                     CountSV_Primary
                                 WHEN 2
                                     THEN
                                     CountSV_Mandatory
                                 ELSE
                                     CountSV
                             END
                            )                       AS Submitted
                          , (CASE @Option
                                 WHEN 1
                                     THEN
                                     CountVA_Primary
                                 WHEN 2
                                     THEN
                                     CountVAVN_Mandatory
                                 ELSE
                                     CountVA
                             END
                            )                       AS Validated
                          , ci1.SnapshotDate        AS SnapshotDate1
                          , ci1.Score               AS Score1
                          , ci2.SnapshotDate        AS SnapshotDate2
                          , ci2.Score               AS Score2
                          , ci3.SnapshotDate        AS SnapshotDate3
                          , ci3.Score               AS Score3
                        FROM
                            custom_cmds.QUserStatus AS ec
                        OUTER APPLY
                            (
                                SELECT
                                    ci1.SnapshotDate
                                  , (CASE @Option
                                         WHEN 1
                                             THEN
                                             ci1.PrimaryScore
                                         WHEN 2
                                             THEN
                                             ci1.ComplianceScore
                                     END
                                    ) AS Score
                                FROM
                                    custom_cmds.ZUserStatusSummary ci1
                                WHERE
                                    ci1.OrganizationIdentifier = ec.OrganizationIdentifier
                                    AND ci1.DepartmentIdentifier = ec.DepartmentIdentifier
                                    AND ci1.UserIdentifier = ec.UserIdentifier
                                    AND ci1.Heading = ec.ItemName
                                    AND ci1.SnapshotDate = @SnapshotDate1
                                    AND
                                        (
                                            ci1.PrimaryProfileIdentifier IS NOT NULL
                                            OR @Option = 2
                                        )
                            )                       ci1
                        OUTER APPLY
                            (
                                SELECT
                                    ci2.SnapshotDate
                                  , (CASE @Option
                                         WHEN 1
                                             THEN
                                             ci2.PrimaryScore
                                         WHEN 2
                                             THEN
                                             ci2.ComplianceScore
                                     END
                                    ) AS Score
                                FROM
                                    custom_cmds.ZUserStatusSummary ci2
                                WHERE
                                    ci2.OrganizationIdentifier = ec.OrganizationIdentifier
                                    AND ci2.DepartmentIdentifier = ec.DepartmentIdentifier
                                    AND ci2.UserIdentifier = ec.UserIdentifier
                                    AND ci2.Heading = ec.ItemName
                                    AND ci2.SnapshotDate = @SnapshotDate2
                                    AND
                                        (
                                            ci2.PrimaryProfileIdentifier IS NOT NULL
                                            OR @Option = 2
                                        )
                            ) ci2
                        OUTER APPLY
                            (
                                SELECT
                                    ci3.SnapshotDate
                                  , (CASE @Option
                                         WHEN 1
                                             THEN
                                             ci3.PrimaryScore
                                         WHEN 2
                                             THEN
                                             ci3.ComplianceScore
                                     END
                                    ) AS Score
                                FROM
                                    custom_cmds.ZUserStatusSummary ci3
                                WHERE
                                    ci3.OrganizationIdentifier = ec.OrganizationIdentifier
                                    AND ci3.DepartmentIdentifier = ec.DepartmentIdentifier
                                    AND ci3.UserIdentifier = ec.UserIdentifier
                                    AND ci3.Heading = ec.ItemName
                                    AND ci3.SnapshotDate = @SnapshotDate3
                                    AND
                                        (
                                            ci3.PrimaryProfileIdentifier IS NOT NULL
                                            OR @Option = 2
                                        )
                            ) ci3
                        WHERE
                            OrganizationIdentifier = @OrganizationIdentifier
                            AND
                                (
                                    DepartmentIdentifier IN
                                        (
                                            SELECT
                                                IdentifierItem
                                            FROM
                                                @Departments
                                        )
                                    OR 0 =
                                        (
                                            SELECT
                                                COUNT(*)
                                            FROM
                                                @Departments
                                        )
                                )
                            AND
                                (
                                    UserIdentifier IN
                                        (
                                            SELECT
                                                IdentifierItem
                                            FROM
                                                @Users
                                        )
                                    OR 0 =
                                        (
                                            SELECT
                                                COUNT(*)
                                            FROM
                                                @Users
                                        )
                                )
                            AND
                                (
                                    PrimaryProfileIdentifier IS NOT NULL
                                    OR @Option = 2
                                )
                            AND
                                (
                                    ItemNumber IN
                                        (
                                            SELECT
                                                IntegerItem
                                            FROM
                                                @Sequences
                                        )
                                    OR 0 =
                                        (
                                            SELECT
                                                COUNT(*)
                                            FROM
                                                @Sequences
                                        )
                                ))
        SELECT
            *
        FROM
            Scores
        WHERE
            (
                @ExcludeUsersWithoutProfile = 0
                OR UserIdentifier IN
                       (
                           SELECT
                               UserIdentifier
                           FROM
                               Scores
                           WHERE
                               Required <> 0
                       )
            )
        ORDER BY
            UserFullName
          , Sequence;
    END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectZUserStatusChart](
    @OrganizationIdentifier     UNIQUEIDENTIFIER
,   @Departments AS             IdentifierList READONLY
,   @Users AS                   IdentifierList READONLY
,   @Sequences AS               IntegerList READONLY
,   @StartDate                  DATE
,   @EndDate                    DATE
,   @Option                     INT -- 1 = primary profiles only; 2 = profiles that require compliance
,   @ExcludeUsersWithoutProfile BIT
) AS
BEGIN

    DECLARE @DepartmentBuffer TABLE (
        Identifier UNIQUEIDENTIFIER NOT NULL
            PRIMARY KEY
    );

    INSERT INTO @DepartmentBuffer (
        Identifier
    )
    SELECT DISTINCT IdentifierItem
    FROM
        @Departments;

    DECLARE @UserBuffer TABLE (
        Identifier UNIQUEIDENTIFIER NOT NULL
            PRIMARY KEY
    );

    INSERT INTO @UserBuffer (
        Identifier
    )
    SELECT DISTINCT IdentifierItem
    FROM
        @Users;

    DECLARE @SequenceBuffer TABLE (
        Identifier INT NOT NULL
            PRIMARY KEY
    );

    INSERT INTO @SequenceBuffer (
        Identifier
    )
    SELECT DISTINCT IntegerItem
    FROM
        @Sequences;

    DECLARE @StatusBuffer TABLE (
        UserIdentifier UNIQUEIDENTIFIER NOT NULL,
        DepartmentIdentifier UNIQUEIDENTIFIER NOT NULL
    );

    INSERT INTO @StatusBuffer (
        UserIdentifier, DepartmentIdentifier
    )
    SELECT DISTINCT h2.UserIdentifier, h2.DepartmentIdentifier
    FROM
        custom_cmds.ZUserStatusSummary AS h2
    WHERE
        h2.OrganizationIdentifier = @OrganizationIdentifier
        AND h2.SnapshotDate BETWEEN @StartDate AND @EndDate
        AND (h2.PrimaryProfileIdentifier IS NOT NULL OR @Option = 2)
        AND h2.ComplianceRequired <> 0;

    DECLARE @Part1 TABLE (
        DepartmentIdentifier UNIQUEIDENTIFIER NOT NULL,
        Sequence INT NOT NULL,
        Heading VARCHAR(50) NOT NULL,
        SnapshotDate DateTime NOT NULL,
        Score DECIMAL(5,4) NULL
    )
    INSERT INTO @Part1 (
        DepartmentIdentifier, Sequence, Heading, SnapshotDate, Score
    )
    SELECT
        h1.DepartmentIdentifier, h1.Sequence + 1 AS Sequence, h1.Heading, h1.SnapshotDate,
        CASE @Option
            WHEN 1 THEN CAST(SUM(h1.PrimarySatisfied) AS DECIMAL) / NULLIF(SUM(h1.PrimaryRequired), 0)
            WHEN 2 THEN CAST(SUM(h1.ComplianceSatisfied) AS DECIMAL) / NULLIF(SUM(h1.ComplianceRequired), 0)
        END AS Score
    FROM
        custom_cmds.ZUserStatusSummary AS h1
    WHERE
        (EXISTS (
            SELECT *
            FROM @UserBuffer
            WHERE h1.UserIdentifier = Identifier
                ) OR 0 = (
            SELECT COUNT(*)
            FROM @UserBuffer
                         ))
        AND h1.OrganizationIdentifier = @OrganizationIdentifier
        AND (EXISTS (
            SELECT *
            FROM @DepartmentBuffer
            WHERE h1.DepartmentIdentifier = Identifier
                    ) OR 0 = (
            SELECT COUNT(*)
            FROM @DepartmentBuffer
                             ))
        AND h1.SnapshotDate BETWEEN @StartDate AND @EndDate
        AND (h1.PrimaryProfileIdentifier IS NOT NULL OR @Option = 2)
        AND (@ExcludeUsersWithoutProfile = 0 OR EXISTS (
            SELECT *
            FROM
                @StatusBuffer AS h2
            WHERE
                h2.DepartmentIdentifier = h1.DepartmentIdentifier
                AND h1.UserIdentifier = h2.UserIdentifier
                                                       ))
    GROUP BY h1.DepartmentIdentifier, h1.Sequence, h1.Heading, h1.SnapshotDate

    OPTION (RECOMPILE);

    DECLARE @Part2 TABLE (
        DepartmentIdentifier UNIQUEIDENTIFIER NOT NULL,
        Sequence INT NOT NULL,
        Heading VARCHAR(50) NOT NULL,
        SnapshotDate DateTime NULL,
        Score DECIMAL(5,4) NULL
    )
    INSERT INTO @Part2 (
        DepartmentIdentifier, Sequence, Heading, SnapshotDate, Score
    )
    SELECT
        ec1.DepartmentIdentifier, ec1.ItemNumber AS Sequence, ec1.ItemName AS Heading, NULL,
        CASE @Option
            WHEN 1 THEN CAST(SUM(ec1.CountVAVNCP_Primary) AS DECIMAL) / NULLIF(SUM(ec1.CountRQ_Primary), 0)
            WHEN 2 THEN CAST(SUM(ec1.CountVAVNCP_Mandatory) AS DECIMAL) / NULLIF(SUM(ec1.CountRQ_Mandatory), 0)
        END AS Score
    FROM
        custom_cmds.QUserStatus AS ec1
    WHERE
        (EXISTS (
            SELECT *
            FROM @UserBuffer
            WHERE ec1.UserIdentifier = Identifier
                ) OR 0 = (
            SELECT COUNT(*)
            FROM @UserBuffer
                         ))
        AND ec1.OrganizationIdentifier = @OrganizationIdentifier
        AND (EXISTS (
            SELECT * FROM @DepartmentBuffer WHERE ec1.DepartmentIdentifier = Identifier
                    ) OR 0 = (
            SELECT COUNT(*)
            FROM @DepartmentBuffer
                             ))
        AND (ec1.PrimaryProfileIdentifier IS NOT NULL OR @Option = 2)
        AND (@ExcludeUsersWithoutProfile = 0 OR EXISTS
            (
                SELECT ec2.UserIdentifier
                FROM
                    custom_cmds.QUserStatus AS ec2
                WHERE
                    ec2.OrganizationIdentifier = ec1.OrganizationIdentifier
                    AND ec2.DepartmentIdentifier = ec1.DepartmentIdentifier
                    AND (ec2.PrimaryProfileIdentifier IS NOT NULL OR @Option = 2)
                    AND ec2.CountRQ_Mandatory <> 0
                    AND ec2.UserIdentifier = ec1.UserIdentifier
            ))
    GROUP BY ec1.DepartmentIdentifier, ec1.ItemNumber, ec1.ItemName

    OPTION (RECOMPILE);

    DECLARE @Part3 TABLE (
        DepartmentIdentifier UNIQUEIDENTIFIER NOT NULL,
        Sequence INT NOT NULL,
        Heading VARCHAR(50) NOT NULL,
        SnapshotDate DateTime NULL,
        Score DECIMAL(5,4) NULL
    )
    INSERT INTO @Part3 (
        DepartmentIdentifier, Sequence, Heading, SnapshotDate, Score
    )
    SELECT DepartmentIdentifier, Sequence, Heading, SnapshotDate, Score
    FROM
        @Part1
    UNION ALL
    SELECT DepartmentIdentifier, Sequence, Heading, SnapshotDate, Score
    FROM
        @Part2;

    SELECT DepartmentIdentifier, Sequence, Heading, SnapshotDate, Score
    FROM
        @Part3
    WHERE
        EXISTS (
            SELECT Identifier
            FROM @SequenceBuffer
            WHERE Identifier = Sequence
               )
        OR 0 = (
            SELECT COUNT(*)
            FROM @SequenceBuffer
               )
    ORDER BY DepartmentIdentifier, Sequence, SnapshotDate;

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_cmds].[SelectZUserStatusHistory](
  @OrganizationIdentifier UNIQUEIDENTIFIER
, @Departments VARCHAR(MAX)
, @UserIdentifier UNIQUEIDENTIFIER
, @StartDate DATE
, @EndDate DATE
, @Option INT -- 1 = primary profiles only; 2 = profiles that require compliance; 3 = all profiles
)
AS
SELECT Organization.OrganizationIdentifier               AS OrganizationIdentifier
     , Organization.CompanyTitle                         AS CompanyName
     , c_department.DepartmentIdentifier
     , c_department.DepartmentName
     , c_employee.UserIdentifier                         AS EmployeeUserIdentifier
     , COALESCE(Person.FullName,c_employee.FullName)     AS EmployeeName
     , s.AsAt                                            AS SnapshotDate
     , cast(datepart(YY, s.AsAt) AS NVARCHAR(4)) + '-' + CASE
    WHEN datepart(MM, s.AsAt) < 10 THEN
        '0'
    ELSE
        ''
    END + cast(datepart(MM, s.AsAt) AS NVARCHAR(4)) + '-' + CASE
           WHEN datepart(DD, s.AsAt) < 10 THEN
               '0'
           ELSE
               ''
           END
    + cast(datepart(DD, s.AsAt) AS NVARCHAR(4))          AS DatePeriod
     , CASE @Option
    WHEN 1 THEN
        s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical
    WHEN 2 THEN
        s.CountRQ_CompetencyCriticalMandatory + s.CountRQ_CompetencyNoncriticalMandatory
    ELSE
        s.CountRQ_Competency
    END                                                  AS CompetencyCountRequired
     , CASE @Option
    WHEN 1 THEN
        s.CountEX_CompetencyCritical + s.CountEX_CompetencyNoncritical
    WHEN 2 THEN
        s.CountEX_CompetencyCriticalMandatory + s.CountEX_CompetencyNoncriticalMandatory
    ELSE
        s.CountEX_Competency
    END                                                  AS CompetencyCountExpired
     , CASE @Option
    WHEN 1 THEN
        s.CountNC_CompetencyCritical + s.CountNC_CompetencyNoncritical
    WHEN 2 THEN
        s.CountNC_CompetencyCriticalMandatory + s.CountNC_CompetencyNoncriticalMandatory
    ELSE
        s.CountNC_Competency
    END                                                  AS CompetencyCountNotCompleted
     , CASE @Option
    WHEN 1 THEN
        s.CountNA_CompetencyCritical + s.CountNA_CompetencyNoncritical
    WHEN 2 THEN
        s.CountNA_CompetencyCriticalMandatory + s.CountNA_CompetencyNoncriticalMandatory
    ELSE
        s.CountNA_Competency
    END                                                  AS CompetencyCountNotApplicable
     , CASE @Option
    WHEN 1 THEN
        s.CountNT_CompetencyCritical + s.CountNT_CompetencyNoncritical
    WHEN 2 THEN
        s.CountNT_CompetencyCriticalMandatory + s.CountNT_CompetencyNoncriticalMandatory
    ELSE
        s.CountNT_Competency
    END                                                  AS CompetencyCountNeedsTraining
     , CASE @Option
    WHEN 1 THEN
        s.CountSA_CompetencyCritical + s.CountSA_CompetencyNoncritical
    WHEN 2 THEN
        s.CountSA_CompetencyCriticalMandatory + s.CountSA_CompetencyNoncriticalMandatory
    ELSE
        s.CountSA_Competency
    END                                                  AS CompetencyCountSelfAssessed
     , CASE @Option
    WHEN 1 THEN
        s.CountSV_CompetencyCritical + s.CountSV_CompetencyNoncritical
    WHEN 2 THEN
        s.CountSV_CompetencyCriticalMandatory + s.CountSV_CompetencyNoncriticalMandatory
    ELSE
        s.CountSV_Competency
    END                                                  AS CompetencyCountSubmitted
     , CASE @Option
    WHEN 1 THEN
        s.CountVA_CompetencyCritical + s.CountVA_CompetencyNoncritical
    WHEN 2 THEN
        s.CountVA_CompetencyCriticalMandatory + s.CountVA_CompetencyNoncriticalMandatory
    ELSE
        s.CountVA_Competency
    END                                                  AS CompetencyCountValidated
     , CASE @Option
    WHEN 1 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical) <> 0 THEN
                round(
                        cast(100 * (s.CountEX_CompetencyCritical + s.CountEX_CompetencyNoncritical) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    WHEN 2 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCriticalMandatory + s.CountRQ_CompetencyNoncriticalMandatory) <> 0 THEN
                round(
                        cast(100 * (s.CountEX_CompetencyCriticalMandatory +
                                    s.CountEX_CompetencyNoncriticalMandatory) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCriticalMandatory +
                                   s.CountRQ_CompetencyNoncriticalMandatory AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    ELSE
        100 * s.ScoreEX_Competency
    END                                                  AS CompetencyPercentExpired
     , CASE @Option
    WHEN 1 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical) <> 0 THEN
                round(
                        cast(100 * (s.CountNC_CompetencyCritical + s.CountNC_CompetencyNoncritical) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    WHEN 2 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCriticalMandatory + s.CountRQ_CompetencyNoncriticalMandatory) <> 0 THEN
                round(
                        cast(100 * (s.CountNC_CompetencyCriticalMandatory +
                                    s.CountNC_CompetencyNoncriticalMandatory) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCriticalMandatory +
                                   s.CountRQ_CompetencyNoncriticalMandatory AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    ELSE
        100 * s.ScoreNC_Competency
    END                                                  AS CompetencyPercentNotCompleted
     , CASE @Option
    WHEN 1 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical) <> 0 THEN
                round(
                        cast(100 * (s.CountNA_CompetencyCritical + s.CountNA_CompetencyNoncritical) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    WHEN 2 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCriticalMandatory + s.CountRQ_CompetencyNoncriticalMandatory) <> 0 THEN
                round(
                        cast(100 * (s.CountNA_CompetencyCriticalMandatory +
                                    s.CountNA_CompetencyNoncriticalMandatory) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCriticalMandatory +
                                   s.CountRQ_CompetencyNoncriticalMandatory AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    ELSE
        100 * s.ScoreNA_Competency
    END                                                  AS CompetencyPercentNotApplicable
     , CASE @Option
    WHEN 1 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical) <> 0 THEN
                round(
                        cast(100 * (s.CountNT_CompetencyCritical + s.CountNT_CompetencyNoncritical) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    WHEN 2 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCriticalMandatory + s.CountRQ_CompetencyNoncriticalMandatory) <> 0 THEN
                round(
                        cast(100 * (s.CountNT_CompetencyCriticalMandatory +
                                    s.CountNT_CompetencyNoncriticalMandatory) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCriticalMandatory +
                                   s.CountRQ_CompetencyNoncriticalMandatory AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    ELSE
        100 * s.ScoreNT_Competency
    END                                                  AS CompetencyPercentNeedsTraining
     , CASE @Option
    WHEN 1 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical) <> 0 THEN
                round(
                        cast(100 * (s.CountSA_CompetencyCritical + s.CountSA_CompetencyNoncritical) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    WHEN 2 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCriticalMandatory + s.CountRQ_CompetencyNoncriticalMandatory) <> 0 THEN
                round(
                        cast(100 * (s.CountSA_CompetencyCriticalMandatory +
                                    s.CountSA_CompetencyNoncriticalMandatory) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCriticalMandatory +
                                   s.CountRQ_CompetencyNoncriticalMandatory AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    ELSE
        100 * s.ScoreSA_Competency
    END                                                  AS CompetencyPercentSelfAssessed
     , CASE @Option
    WHEN 1 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical) <> 0 THEN
                round(
                        cast(100 * (s.CountSV_CompetencyCritical + s.CountSV_CompetencyNoncritical) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    WHEN 2 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCriticalMandatory + s.CountRQ_CompetencyNoncriticalMandatory) <> 0 THEN
                round(
                        cast(100 * (s.CountSV_CompetencyCriticalMandatory +
                                    s.CountSV_CompetencyNoncriticalMandatory) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCriticalMandatory +
                                   s.CountRQ_CompetencyNoncriticalMandatory AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    ELSE
        100 * s.ScoreSV_Competency
    END                                                  AS CompetencyPercentSubmitted
     , CASE @Option
    WHEN 1 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical) <> 0 THEN
                round(
                        cast(100 * (s.CountVA_CompetencyCritical + s.CountVA_CompetencyNoncritical) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCritical + s.CountRQ_CompetencyNoncritical AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    WHEN 2 THEN
        (CASE
            WHEN (s.CountRQ_CompetencyCriticalMandatory + s.CountRQ_CompetencyNoncriticalMandatory) <> 0 THEN
                round(
                        cast(100 * (s.CountVA_CompetencyCriticalMandatory +
                                    s.CountVA_CompetencyNoncriticalMandatory) AS DECIMAL)
                            / cast(s.CountRQ_CompetencyCriticalMandatory +
                                   s.CountRQ_CompetencyNoncriticalMandatory AS DECIMAL)
                    , 1
                )
            ELSE
                100
            END
            )
    ELSE
        100 * s.ScoreVA_Competency
    END                                                  AS CompetencyPercentValidated
FROM custom_cmds.ZUserStatus AS s
         INNER JOIN accounts.QOrganization AS Organization
                    ON Organization.OrganizationIdentifier = s.OrganizationIdentifier
         INNER JOIN identities.Department AS c_department ON c_department.DepartmentIdentifier = s.DepartmentIdentifier
         INNER JOIN identities.QUser AS c_employee ON c_employee.UserIdentifier = s.UserIdentifier
         INNER JOIN contacts.QPerson AS Person ON Person.UserIdentifier = c_employee.UserIdentifier AND
                                                  Person.OrganizationIdentifier = Organization.OrganizationIdentifier
WHERE s.AsAt
    BETWEEN @StartDate AND @EndDate
  AND s.OrganizationIdentifier = @OrganizationIdentifier
  AND (
    @Departments IS NULL
        OR s.DepartmentIdentifier IN
           ( SELECT cast(ItemText AS UNIQUEIDENTIFIER)
             FROM dbo.SplitText(@Departments, ',') )
    )
  AND (
    @UserIdentifier IS NULL
        OR s.UserIdentifier = @UserIdentifier
    )
  AND c_employee.UtcArchived IS NULL
  AND (
    @Option = 1
        AND s.CountEX_CompetencyCritical IS NOT NULL
        OR @Option = 2
        AND s.CountEX_CompetencyCriticalMandatory IS NOT NULL
        OR @Option = 3
        AND s.CountEX_Competency IS NOT NULL
    )
  AND CountRQ_Competency <> 0
ORDER BY DatePeriod;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_ncsha].[CopyAbProgram]
    (
        @SourceSurveyYear INT
      , @DestSurveyYear   INT
      , @Deadline         DATETIMEOFFSET
    )
AS
    INSERT custom_ncsha.AbProgram
        (
            InsertedOn
          , DateTimeSaved
          , SurveyYear
          , Deadline
          , RespondentUserIdentifier
          , AgencyGroupIdentifier
          , RespondentName
          , StateName
          , AB001
          , AB002
          , AB003
          , AB004
          , AB005
          , AB006
        )
           SELECT
               GETUTCDATE()
             , GETUTCDATE()
             , @DestSurveyYear
             , @Deadline
             , RespondentUserIdentifier
             , AgencyGroupIdentifier
             , RespondentName
             , StateName
             , AB001
             , AB002
             , AB003
             , AB004
             , AB005
             , AB006
           FROM
               custom_ncsha.AbProgram
           WHERE
               SurveyYear = @SourceSurveyYear;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_ncsha].[CopyHcProgram]
    (
        @SourceSurveyYear INT
      , @DestSurveyYear   INT
      , @Deadline         DATETIMEOFFSET
    )
AS
    INSERT custom_ncsha.HcProgram
        (
            InsertedOn
          , DateTimeSaved
          , SurveyYear
          , Deadline
          , OwnerUserIdentifier
          , AgencyGroupIdentifier
          , HC001
          , HC002
          , HC003
          , HC004
          , HC005
        )
           SELECT
               GETUTCDATE()
             , GETUTCDATE()
             , @DestSurveyYear
             , @Deadline
             , OwnerUserIdentifier
             , AgencyGroupIdentifier
             , HC001
             , HC002
             , HC003
             , HC004
             , HC005
           FROM
               custom_ncsha.HcProgram
           WHERE
               SurveyYear = @SourceSurveyYear;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_ncsha].[CopyHiProgram]
    (
        @SourceSurveyYear INT
      , @DestSurveyYear   INT
      , @Deadline         DATETIMEOFFSET
    )
AS
    INSERT custom_ncsha.HiProgram
        (
            InsertedOn
          , DateTimeSaved
          , SurveyYear
          , Deadline
          , RespondentUserIdentifier
          , AgencyGroupIdentifier
          , RespondentName
          , StateName
          , HI001
          , HI002
          , HI003
          , HI004
          , HI005
        )
           SELECT
               GETUTCDATE()
             , GETUTCDATE()
             , @DestSurveyYear
             , @Deadline
             , RespondentUserIdentifier
             , AgencyGroupIdentifier
             , RespondentName
             , StateName
             , HI001
             , HI002
             , HI003
             , HI004
             , HI005
           FROM
               custom_ncsha.HiProgram
           WHERE
               SurveyYear = @SourceSurveyYear;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_ncsha].[CopyMfProgram]
    (
        @SourceSurveyYear INT
      , @DestSurveyYear   INT
      , @Deadline         DATETIMEOFFSET
    )
AS
    INSERT custom_ncsha.MfProgram
        (
            InsertedOn
          , DateTimeSaved
          , SurveyYear
          , Deadline
          , RespondentUserIdentifier
          , AgencyGroupIdentifier
          , RespondentName
          , StateName
          , MF001
          , MF002
          , MF003
          , MF004
          , MF005
        )
           SELECT
               GETUTCDATE()
             , GETUTCDATE()
             , @DestSurveyYear
             , @Deadline
             , RespondentUserIdentifier
             , AgencyGroupIdentifier
             , RespondentName
             , StateName
             , MF001
             , MF002
             , MF003
             , MF004
             , MF005
           FROM
               custom_ncsha.MfProgram
           WHERE
               SurveyYear = @SourceSurveyYear;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_ncsha].[CopyMrProgram]
    (
        @SourceSurveyYear INT
      , @DestSurveyYear   INT
      , @Deadline         DATETIMEOFFSET
    )
AS
    INSERT custom_ncsha.MrProgram
        (
            InsertedOn
          , DateTimeSaved
          , SurveyYear
          , Deadline
          , RespondentUserIdentifier
          , AgencyGroupIdentifier
          , RespondentName
          , StateName
          , MR001
          , MR002
          , MR003
          , MR004
          , MR005
        )
           SELECT
               GETUTCDATE()
             , GETUTCDATE()
             , @DestSurveyYear
             , @Deadline
             , RespondentUserIdentifier
             , AgencyGroupIdentifier
             , RespondentName
             , StateName
             , MR001
             , MR002
             , MR003
             , MR004
             , MR005
           FROM
               custom_ncsha.MrProgram
           WHERE
               SurveyYear = @SourceSurveyYear;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [custom_ncsha].[CopyPaProgram]
    (
        @SourceSurveyYear INT
      , @DestSurveyYear   INT
      , @Deadline         DATETIMEOFFSET
    )
AS
    INSERT custom_ncsha.PaProgram
        (
            InsertedOn
          , DateTimeSaved
          , SurveyYear
          , Deadline
          , RespondentUserIdentifier
          , AgencyGroupIdentifier
          , RespondentName
          , StateName
          , PA001
          , PA002
          , PA003
          , PA004
          , PA005
        )
           SELECT
               GETUTCDATE()
             , GETUTCDATE()
             , @DestSurveyYear
             , @Deadline
             , RespondentUserIdentifier
             , AgencyGroupIdentifier
             , RespondentName
             , StateName
             , PA001
             , PA002
             , PA003
             , PA004
             , PA005
           FROM
               custom_ncsha.PaProgram
           WHERE
               SurveyYear = @SourceSurveyYear;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [database].[InsertUpgrade]
(
    @ScriptName VARCHAR(100)
  , @ScriptHash BINARY(32)
)
AS
BEGIN
    IF NOT EXISTS
    (
        SELECT *
        FROM utilities.Upgrade
        WHERE ScriptName = @ScriptName
    )
        INSERT INTO utilities.Upgrade
        (
            ScriptName
          , ScriptHash
          , UtcUpgraded
        )
        VALUES
        (@ScriptName, @ScriptHash, GETUTCDATE());
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [database].[RefreshConstraintNames]
as
begin
    declare X cursor for
    select distinct
        'exec sp_rename ''' + object_schema_name(constid) + '.[' + object_name(constid) + ']'', ''' + 'PK_'
        + object_name(id) + ''', ''object'';'
    from sysconstraints
    where objectproperty(constid, 'IsPrimaryKey') = 1
          and object_name(id) <> 'dtproperties'
          and object_name(constid) <> 'PK_' + object_name(id)
    union
    select 'exec sp_rename ''' + object_schema_name(constid) + '.[' + object_name(constid) + ']'', ''' + 'DF_'
           + object_name(id) + '_' + col_name(id, colid) + ''', ''object'''
    from sysconstraints
    where objectproperty(constid, 'IsDefaultCnst') = 1
          and object_name(id) <> 'dtproperties'
          and object_name(constid) <> 'DF_' + object_name(id) + '_' + col_name(id, colid);

    declare @SqlStatement nvarchar(max);

    open X;

    fetch next from X
    into @SqlStatement;

    while @@fetch_status = 0
    begin
        exec sp_executesql @SqlStatement;

        fetch next from X
        into @SqlStatement;
    end;

    close X;
    deallocate X;
end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [database].[RefreshProcedures]
AS
BEGIN
    DECLARE @ObjName NVARCHAR(128);
    DECLARE @ObjType CHAR(2);
    DECLARE @ObjTypeName NVARCHAR(128);
    DECLARE @ModuleName NVARCHAR(128);

    DECLARE ObjCursor CURSOR FOR
    SELECT '[' + SCHEMA_NAME(schema_id) + '].[' + name + ']'
      , type
      , type_desc
    FROM sys.all_objects
    WHERE type IN ( 'FN', 'IF', 'P', 'TF', 'X' )
          AND is_ms_shipped = 0;

    OPEN ObjCursor;

    FETCH NEXT FROM ObjCursor
    INTO @ObjName
      , @ObjType
      , @ObjTypeName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @ObjTypeName = CASE @ObjType
                               WHEN 'FN' THEN
                                   'scalar function'
                               WHEN 'IF' THEN
                                   'inline table-valued function'
                               WHEN 'P' THEN
                                   'stored procedure'
                               WHEN 'TF' THEN
                                   'table-valued-function'
                               WHEN 'X' THEN
                                   'extended stored procedure'
                               ELSE
                                   @ObjTypeName
                           END;

        PRINT 'Refreshing ' + @ObjTypeName + '...	' + @ObjName;

        EXEC sp_refreshsqlmodule @ObjName;

        FETCH NEXT FROM ObjCursor
        INTO @ObjName
          , @ObjType
          , @ObjTypeName;
    END;

    CLOSE ObjCursor;
    DEALLOCATE ObjCursor;

    PRINT 'Refresh process is done successfully';
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [database].[RefreshViews]
(@varViewName VARCHAR(500) = NULL)
AS
DECLARE @p_viewname NVARCHAR(500);

BEGIN
    IF (@varViewName IS NOT NULL OR @varViewName <> '')
    BEGIN
        -- Validate specific view.
        SET @varViewName = ('[' + @varViewName + ']');

        PRINT 'Refreshing View ... ' + @varViewName;

        EXEC sp_refreshview @varViewName;
    END;
    ELSE
    BEGIN
        -- Validate all views.
        DECLARE @CrsrView CURSOR;

        SET @CrsrView = CURSOR FOR
        SELECT TABLE_SCHEMA + '.' + TABLE_NAME
        FROM INFORMATION_SCHEMA.VIEWS
        ORDER BY TABLE_SCHEMA + '.' + TABLE_NAME;

        OPEN @CrsrView;

        FETCH NEXT FROM @CrsrView
        INTO @p_viewname;

        WHILE (@@FETCH_STATUS = 0)
        BEGIN
            SET @varViewName = ('[' + @varViewName + ']');

            PRINT 'Refreshing View ... ' + @p_viewname;

            EXEC sp_refreshview @p_viewname;

            FETCH NEXT FROM @CrsrView
            INTO @p_viewname;
        END;

        CLOSE @CrsrView;
        DEALLOCATE @CrsrView;

        PRINT 'Refresh completed successfully';
    END;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [database].[SelectUpgrades]
AS
BEGIN
	SELECT ScriptName, ScriptHash FROM utilities.Upgrade ORDER BY ScriptName;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [database].[UpdateUpgrade]
(
    @ScriptName VARCHAR(100)
  , @ScriptHash BINARY(32)
)
AS
BEGIN
	UPDATE utilities.Upgrade SET ScriptHash = @ScriptHash WHERE ScriptName = @ScriptName AND ScriptHash IS NULL;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [databases].[ChaseOrganizationIdentifiers] as 
begin

  declare @EmptyGuid uniqueidentifier = cast(cast(0 as binary) as uniqueidentifier);

	update achievements.QCredential set OrganizationIdentifier = O.OrganizationIdentifier from achievements.QAchievement as O where QCredential.OrganizationIdentifier is null and QCredential.AchievementIdentifier = O.AchievementIdentifier;
	update achievements.TAchievementClassification set OrganizationIdentifier = O.OrganizationIdentifier from achievements.QAchievement as O where TAchievementClassification.OrganizationIdentifier is null and TAchievementClassification.AchievementIdentifier = O.AchievementIdentifier;
	update achievements.TAchievementDepartment set OrganizationIdentifier = O.OrganizationIdentifier from achievements.QAchievement as O where TAchievementDepartment.OrganizationIdentifier is null and TAchievementDepartment.AchievementIdentifier = O.AchievementIdentifier;

	update assessments.QAttemptMatch set OrganizationIdentifier = O.OrganizationIdentifier from assessments.QAttempt as O where QAttemptMatch.OrganizationIdentifier is null and QAttemptMatch.AttemptIdentifier = O.AttemptIdentifier;
	update assessments.QAttemptOption set OrganizationIdentifier = O.OrganizationIdentifier from assessments.QAttempt as O where QAttemptOption.OrganizationIdentifier is null and QAttemptOption.AttemptIdentifier = O.AttemptIdentifier;
	update assessments.QAttemptQuestion set OrganizationIdentifier = O.OrganizationIdentifier from assessments.QAttempt as O where QAttemptQuestion.OrganizationIdentifier is null and QAttemptQuestion.AttemptIdentifier = O.AttemptIdentifier;

	update banks.QBankQuestionAttachment set OrganizationIdentifier = O.OrganizationIdentifier from banks.QBankQuestion as O where QBankQuestionAttachment.OrganizationIdentifier is null and QBankQuestionAttachment.QuestionIdentifier = O.QuestionIdentifier;

	update contacts.QGroupAddress set OrganizationIdentifier = O.OrganizationIdentifier from contacts.QGroup as O where QGroupAddress.OrganizationIdentifier is null and QGroupAddress.GroupIdentifier = O.GroupIdentifier;
	update contacts.QGroupConnection set OrganizationIdentifier = O.OrganizationIdentifier from contacts.QGroup as O where QGroupConnection.OrganizationIdentifier is null and QGroupConnection.ParentGroupIdentifier = O.GroupIdentifier;
	update contacts.TGroupPermission set OrganizationIdentifier = O.OrganizationIdentifier from contacts.QGroup as O where (TGroupPermission.OrganizationIdentifier is null or TGroupPermission.OrganizationIdentifier = @EmptyGuid) and TGroupPermission.GroupIdentifier = O.GroupIdentifier;
	update contacts.TGroupSetting set OrganizationIdentifier = O.OrganizationIdentifier from contacts.QGroup as O where TGroupSetting.OrganizationIdentifier is null and TGroupSetting.GroupIdentifier = O.GroupIdentifier;

	update custom_cmds.QCmdsInvoice set OrganizationIdentifier = '8258CB0A-D1E8-4BC1-94B3-E70652503437' where OrganizationIdentifier is null;
	update custom_cmds.QCmdsInvoiceFee set OrganizationIdentifier = '8258CB0A-D1E8-4BC1-94B3-E70652503437' where OrganizationIdentifier is null;
	update custom_cmds.QCmdsInvoiceItem set OrganizationIdentifier = '8258CB0A-D1E8-4BC1-94B3-E70652503437' where OrganizationIdentifier is null;
	update custom_cmds.TCollegeCertificate set OrganizationIdentifier = '8258CB0A-D1E8-4BC1-94B3-E70652503437' where OrganizationIdentifier is null;

	update custom_ita.ExamDistributionRequest set OrganizationIdentifier = 'EFB530A0-8B3C-448C-9FB4-DDF7602489A6' where OrganizationIdentifier is null;
	update custom_ita.Individual set OrganizationIdentifier = 'EFB530A0-8B3C-448C-9FB4-DDF7602489A6' where OrganizationIdentifier is null;

	update custom_ncsha.AbProgram set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.Counter set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.Field set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.Filter set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.HcProgram set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.HiProgram set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.History set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.MfProgram set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.MrProgram set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.PaProgram set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.ProgramFolder set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.ProgramFolderComment set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;
	update custom_ncsha.TReportMapping set OrganizationIdentifier = 'CF500796-48F1-45C9-B6E3-5C26AAF127B0' where OrganizationIdentifier is null;

	update events.QEventAssessmentForm set OrganizationIdentifier = O.OrganizationIdentifier from events.QEvent as O where QEventAssessmentForm.OrganizationIdentifier is null and QEventAssessmentForm.EventIdentifier = O.EventIdentifier;
	update events.QEventTimer set OrganizationIdentifier = O.OrganizationIdentifier from events.QEvent as O where QEventTimer.OrganizationIdentifier is null and QEventTimer.EventIdentifier = O.EventIdentifier;
	update events.QSeat set OrganizationIdentifier = O.OrganizationIdentifier from events.QEvent as O where QSeat.OrganizationIdentifier is null and QSeat.EventIdentifier = O.EventIdentifier;

	update assets.QGlossaryTermContent set OrganizationIdentifier = O.OrganizationIdentifier from assets.QGlossaryTerm as O where QGlossaryTermContent.OrganizationIdentifier is null and QGlossaryTermContent.TermIdentifier = O.TermIdentifier;

	update invoices.QInvoiceItem set OrganizationIdentifier = O.OrganizationIdentifier from invoices.QInvoice as O where QInvoiceItem.OrganizationIdentifier = @EmptyGuid and QInvoiceItem.InvoiceIdentifier = O.InvoiceIdentifier;

	update issues.QIssueAttachment set OrganizationIdentifier = O.OrganizationIdentifier from issues.QIssue as O where QIssueAttachment.OrganizationIdentifier is null and QIssueAttachment.IssueIdentifier = O.IssueIdentifier;
	update issues.QIssueGroup set OrganizationIdentifier = O.OrganizationIdentifier from issues.QIssue as O where QIssueGroup.OrganizationIdentifier is null and QIssueGroup.IssueIdentifier = O.IssueIdentifier;
	update issues.QIssueUser set OrganizationIdentifier = O.OrganizationIdentifier from issues.QIssue as O where QIssueUser.OrganizationIdentifier is null and QIssueUser.IssueIdentifier = O.IssueIdentifier;

	update jobs.TApplication set OrganizationIdentifier = O.OrganizationIdentifier from jobs.TOpportunity as O where TApplication.OrganizationIdentifier is null and TApplication.OpportunityIdentifier = O.OpportunityIdentifier;

	update messages.QLink set OrganizationIdentifier = O.OrganizationIdentifier from messages.QMessage as O where QLink.OrganizationIdentifier is null and QLink.MessageIdentifier = O.MessageIdentifier;
	update messages.QClick set OrganizationIdentifier = O.OrganizationIdentifier from messages.QLink as O where QClick.OrganizationIdentifier is null and QClick.LinkIdentifier = O.LinkIdentifier;
	update messages.QFollower set OrganizationIdentifier = O.OrganizationIdentifier from messages.QMessage as O where QFollower.OrganizationIdentifier is null and QFollower.MessageIdentifier = O.MessageIdentifier;
	update messages.QSubscriberGroup set OrganizationIdentifier = O.OrganizationIdentifier from messages.QMessage as O where QSubscriberGroup.OrganizationIdentifier is null and QSubscriberGroup.MessageIdentifier = O.MessageIdentifier;
	update messages.QSubscriberUser set OrganizationIdentifier = O.OrganizationIdentifier from messages.QMessage as O where QSubscriberUser.OrganizationIdentifier is null and QSubscriberUser.MessageIdentifier = O.MessageIdentifier;

	update records.QCompetencyRequirement set OrganizationIdentifier = O.OrganizationIdentifier from standards.Standard as O where QCompetencyRequirement.OrganizationIdentifier is null and QCompetencyRequirement.CompetencyStandardIdentifier = O.StandardIdentifier;
	update records.QCredentialHistory set OrganizationIdentifier = O.OrganizationIdentifier from achievements.QCredential as O where QCredentialHistory.AggregateIdentifier is null and QCredentialHistory.AggregateIdentifier = O.CredentialIdentifier;
	update records.QEnrollment set OrganizationIdentifier = O.OrganizationIdentifier from records.QGradebook as O where QEnrollment.OrganizationIdentifier is null and QEnrollment.GradebookIdentifier = O.GradebookIdentifier;
	update records.QEnrollmentHistory set OrganizationIdentifier = O.OrganizationIdentifier from records.QEnrollment as O where QEnrollmentHistory.OrganizationIdentifier is null and QEnrollmentHistory.AggregateIdentifier = O.EnrollmentIdentifier;
	update records.QGradeItem set OrganizationIdentifier = O.OrganizationIdentifier from records.QGradebook as O where QGradeItem.OrganizationIdentifier is null and QGradeItem.GradebookIdentifier = O.GradebookIdentifier;
	update records.QProgress set OrganizationIdentifier = O.OrganizationIdentifier from records.QGradebook as O where QProgress.OrganizationIdentifier is null and QProgress.GradebookIdentifier = O.GradebookIdentifier;
	update records.QProgressHistory set OrganizationIdentifier = O.OrganizationIdentifier from records.QProgress as O where QProgressHistory.OrganizationIdentifier is null and QProgressHistory.AggregateIdentifier = O.ProgressIdentifier;
	update records.TProgram set OrganizationIdentifier = O.OrganizationIdentifier from contacts.QGroup as O where TProgram.GroupIdentifier = O.GroupIdentifier and (TProgram.OrganizationIdentifier is null or TProgram.OrganizationIdentifier = @EmptyGuid);
	update records.TProgramEnrollment set OrganizationIdentifier = O.OrganizationIdentifier from records.TProgram as O where TProgramEnrollment.ProgramIdentifier = O.ProgramIdentifier and (TProgramEnrollment.OrganizationIdentifier is null or TProgramEnrollment.OrganizationIdentifier = @EmptyGuid);

	update registrations.QAccommodation set OrganizationIdentifier = O.OrganizationIdentifier from registrations.QRegistration as O where QAccommodation.OrganizationIdentifier is null and QAccommodation.RegistrationIdentifier = O.RegistrationIdentifier;
	update registrations.QRegistrationTimer set OrganizationIdentifier = O.OrganizationIdentifier from registrations.QRegistration as O where QRegistrationTimer.OrganizationIdentifier is null and QRegistrationTimer.RegistrationIdentifier = O.RegistrationIdentifier;

	update resources.UploadRelation set OrganizationIdentifier = O.OrganizationIdentifier from resources.Upload as O where UploadRelation.OrganizationIdentifier is null and UploadRelation.UploadIdentifier = O.UploadIdentifier;

	update standards.DepartmentProfileCompetency set OrganizationIdentifier = O.OrganizationIdentifier from contacts.QGroup as O where DepartmentProfileCompetency.OrganizationIdentifier is null and DepartmentProfileCompetency.DepartmentIdentifier = O.GroupIdentifier;
	update standards.DepartmentProfileUser set OrganizationIdentifier = O.OrganizationIdentifier from contacts.QGroup as O where DepartmentProfileUser.OrganizationIdentifier is null and DepartmentProfileUser.DepartmentIdentifier = O.GroupIdentifier;
	update standards.StandardValidation set OrganizationIdentifier = O.OrganizationIdentifier from standards.Standard as O where StandardValidation.OrganizationIdentifier is null and StandardValidation.StandardIdentifier = O.StandardIdentifier;
	update standards.StandardValidationChange set OrganizationIdentifier = O.OrganizationIdentifier from standards.Standard as O where StandardValidationChange.OrganizationIdentifier is null and StandardValidationChange.StandardIdentifier = O.StandardIdentifier;

	update surveys.QResponseAnswer set OrganizationIdentifier = O.OrganizationIdentifier from surveys.QResponseSession as O where QResponseAnswer.OrganizationIdentifier is null and QResponseAnswer.ResponseSessionIdentifier = O.ResponseSessionIdentifier;
	update surveys.QResponseOption set OrganizationIdentifier = O.OrganizationIdentifier from surveys.QResponseSession as O where QResponseOption.OrganizationIdentifier is null and QResponseOption.ResponseSessionIdentifier = O.ResponseSessionIdentifier;

	update surveys.QSurveyQuestion set OrganizationIdentifier = O.OrganizationIdentifier from surveys.QSurveyForm as O where QSurveyQuestion.OrganizationIdentifier is null and QSurveyQuestion.SurveyFormIdentifier = O.SurveyFormIdentifier;
	update surveys.QSurveyOptionList set OrganizationIdentifier = O.OrganizationIdentifier from surveys.QSurveyQuestion as O where QSurveyOptionList.OrganizationIdentifier is null and QSurveyOptionList.SurveyQuestionIdentifier = O.SurveyQuestionIdentifier;
	update surveys.QSurveyOptionItem set OrganizationIdentifier = O.OrganizationIdentifier from surveys.QSurveyOptionList as O where QSurveyOptionItem.OrganizationIdentifier is null and QSurveyOptionItem.SurveyOptionListIdentifier = O.SurveyOptionListIdentifier;
	update surveys.QSurveyCondition set OrganizationIdentifier = O.OrganizationIdentifier from surveys.QSurveyOptionItem as O where QSurveyCondition.OrganizationIdentifier is null and QSurveyCondition.MaskingSurveyOptionItemIdentifier = O.SurveyOptionItemIdentifier;
	update surveys.QSurveyRespondent set OrganizationIdentifier = O.OrganizationIdentifier from surveys.QSurveyForm as O where QSurveyRespondent.OrganizationIdentifier is null and QSurveyRespondent.SurveyFormIdentifier = O.SurveyFormIdentifier;

end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [logs].[DeleteSnapshot]
(
    @AggregateIdentifier uniqueidentifier
   ,@AggregateType varchar(100)
)
AS
BEGIN

    DELETE
        logs.[Snapshot]
    FROM
        logs.[Snapshot]
        INNER JOIN logs.[Aggregate] ON [Aggregate].AggregateIdentifier = [Snapshot].AggregateIdentifier
    WHERE
        [Snapshot].AggregateIdentifier = @AggregateIdentifier
        AND [Aggregate].AggregateType = @AggregateType;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [logs].[MyMessage]
(
    @UserIdentifier uniqueidentifier
  , @MailoutIdentifier uniqueidentifier
)
as
begin

    select M.MailoutIdentifier
         , R.UserIdentifier
         , M.ContentSubject
         , M.ContentBodyHtml
         , M.ContentBodyText
         , S.SenderName
         , S.SenderEmail
         , R.PersonName as RecipientName
         , R.UserEmail as RecipientEmail
         , R.RecipientVariables
         , M.MailoutScheduled
         , R.DeliveryCompleted
    from communications.QMailout as M
         inner join communications.QRecipient as R
            on R.MailoutIdentifier = M.MailoutIdentifier
               and M.MailoutIdentifier = @MailoutIdentifier
         left join accounts.TSender as S
            on S.SenderIdentifier = M.SenderIdentifier
    where R.UserIdentifier = @UserIdentifier;

end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [logs].[MyMessages]
(
    @UserIdentifier uniqueidentifier
  , @OrganizationIdentifier uniqueidentifier
)
as
    begin

        select M.MailoutIdentifier
             , R.UserIdentifier
             , M.ContentSubject
             , M.ContentBodyHtml
             , M.ContentBodyText
             , M.ContentVariables
             , S.SenderName
             , S.SenderEmail
             , R.PersonName as RecipientName
             , R.UserEmail  as RecipientEmail
             , R.RecipientVariables
             , M.MailoutScheduled
             , R.DeliveryCompleted
        from messages.VMailout                    as M
             inner join communications.QRecipient as R on R.MailoutIdentifier = M.MailoutIdentifier
             left join accounts.TSender           as S on S.SenderIdentifier = M.SenderIdentifier
        where R.UserIdentifier = @UserIdentifier
              and R.DeliveryStatus in ( 'Completed', 'Succeeded' )
              and (
                      M.OrganizationIdentifier = @OrganizationIdentifier
                      or M.OrganizationIdentifier in (
                                                         select ParentOrganizationIdentifier
                                                         from accounts.QOrganization
                                                         where OrganizationIdentifier = @OrganizationIdentifier
                                                               and ParentOrganizationIdentifier is not null
                                                     )
                  );

    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [logs].[MyMessagesCount]
(
    @UserIdentifier uniqueidentifier
  , @OrganizationIdentifier uniqueidentifier
)
as
    begin;

        select count(*) as RecordCount
        from messages.VMailout                    as M
             inner join communications.QRecipient as R on R.MailoutIdentifier = M.MailoutIdentifier
        where R.UserIdentifier = @UserIdentifier
              and R.DeliveryStatus in ( 'Completed', 'Succeeded' )
              and (
                      M.OrganizationIdentifier = @OrganizationIdentifier
                      or exists (
                                    select ParentOrganizationIdentifier
                                    from accounts.QOrganization
                                    where OrganizationIdentifier = @OrganizationIdentifier
                                          and ParentOrganizationIdentifier is not null
                                          and M.OrganizationIdentifier = ParentOrganizationIdentifier
                                )
                  );
    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [messages].[CountMessageReferences]( @M uniqueidentifier ) 
as
begin

        select 'Groups/Notifications/EventVenueChanged' as Name, count(*) as Value from contacts.QGroup where MessageToAdminWhenEventVenueChanged = @M
  union select 'Groups/Notifications/MembershipStarted' as Name, count(*) as Value from contacts.QGroup where (MessageToAdminWhenMembershipStarted = @M or MessageToUserWhenMembershipStarted = @M)
  union select 'Groups/Notifications/MembershipEnded' as Name, count(*) as Value from contacts.QGroup where (MessageToAdminWhenMembershipEnded = @M or MessageToUserWhenMembershipEnded = @M)
  
  union select 'Messages/Mailouts' as Name, count(*) as Value from communications.QMailout where MessageIdentifier = @M
  union select 'Messages/Emails' as Name, count(*) as Value from communications.QRecipient as r inner join communications.QMailout as m on r.MailoutIdentifier = m.MailoutIdentifier where MessageIdentifier = @M
  union select 'Messages/Followers' as Name, count(*) as Value from messages.QFollower where MessageIdentifier = @M
  union select 'Messages/Links' as Name, count(*) as Value from messages.QLink where MessageIdentifier = @M
  union select 'Messages' as Name, count(*) as Value from messages.QMessage where MessageIdentifier = @M
  union select 'Messages/Subscribers/Groups' as Name, count(*) as Value from messages.QSubscriberGroup where MessageIdentifier = @M
  union select 'Messages/Subscribers/Users' as Name, count(*) as Value from messages.QSubscriberUser where MessageIdentifier = @M

  union select 'Gradebooks/Notifications/ProgressCompleted' as Name, count(*) as Value from records.QGradeItem where ProgressCompletedMessageIdentifier = @M

  union select 'Surveys/Invitations' as Name, count(*) as Value from surveys.QSurveyForm where SurveyMessageInvitation = @M
  union select 'Surveys/Notifications/ResponseCompleted' as Name, count(*) as Value from surveys.QSurveyForm where SurveyMessageResponseCompleted = @M
  union select 'Surveys/Notifications/ResponseConfirmed' as Name, count(*) as Value from surveys.QSurveyForm where SurveyMessageResponseConfirmed = @M
  union select 'Surveys/Notifications/ResponseStarted' as Name, count(*) as Value from surveys.QSurveyForm where SurveyMessageResponseStarted = @M
  ;

end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [record].[SelectGradebookEnrollmentsForCredentials]( @LearnerId UNIQUEIDENTIFIER, @AchievementId UNIQUEIDENTIFIER, @CredentialStatuses VARCHAR(100) )
AS

BEGIN

  SELECT DISTINCT C.CourseIdentifier, E.LearnerIdentifier FROM courses.TCourse AS C
  INNER JOIN records.QGradebook AS G ON C.GradebookIdentifier = G.GradebookIdentifier
  INNER JOIN records.QGradeItem AS I ON I.GradebookIdentifier = G.GradebookIdentifier
  INNER JOIN records.QEnrollment AS E ON E.GradebookIdentifier = G.GradebookIdentifier
  INNER JOIN achievements.QCredential AS R ON R.AchievementIdentifier = @AchievementId AND R.UserIdentifier = E.LearnerIdentifier
  WHERE (G.AchievementIdentifier = @AchievementId OR I.AchievementIdentifier = @AchievementId)
  AND R.CredentialStatus IS NOT NULL 
  AND EXISTS (SELECT * FROM STRING_SPLIT(@CredentialStatuses, ',') WHERE TRIM(Value) = R.CredentialStatus)
  AND (@LearnerId IS NULL OR @LearnerId = R.UserIdentifier)

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [records].[DeleteRubric](@RubricId uniqueidentifier)
AS
BEGIN
    DELETE contents.TContent WHERE ContainerType = 'RubricRating' AND ContainerIdentifier IN (
        SELECT
            RubricRatingIdentifier
        FROM
            records.QRubricRating
            INNER JOIN records.QRubricCriterion ON QRubricCriterion.RubricCriterionIdentifier = QRubricRating.RubricCriterionIdentifier
        WHERE
            QRubricCriterion.RubricIdentifier = @RubricId);

    DELETE contents.TContent WHERE ContainerType = 'RubricCriterion' AND ContainerIdentifier IN (
        SELECT RubricCriterionIdentifier FROM records.QRubricCriterion WHERE RubricIdentifier = @RubricId);

    DELETE contents.TContent WHERE ContainerType = 'Rubric' AND ContainerIdentifier = @RubricId;

    DELETE
        records.QRubricRating
    WHERE
        RubricCriterionIdentifier IN (
            SELECT RubricCriterionIdentifier
            FROM records.QRubricCriterion
            WHERE RubricIdentifier = @RubricId);

    DELETE records.QRubricCriterion WHERE RubricIdentifier = @RubricId;

    DELETE records.QRubric WHERE RubricIdentifier = @RubricId;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [records].[DeleteRubricCriterion](@RubricCriterionId uniqueidentifier)
AS
BEGIN
    DELETE contents.TContent WHERE ContainerType = 'RubricRating' AND ContainerIdentifier IN (
        SELECT RubricRatingIdentifier FROM records.QRubricRating WHERE RubricCriterionIdentifier = @RubricCriterionId);
    DELETE contents.TContent WHERE ContainerType = 'RubricCriterion' AND ContainerIdentifier = @RubricCriterionId;

    DELETE records.QRubricRating WHERE RubricCriterionIdentifier = @RubricCriterionId;
    DELETE records.QRubricCriterion WHERE RubricCriterionIdentifier = @RubricCriterionId;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [records].[GetProgramAndTaskLearnersCredentials]
(@ProgramIdentifier uniqueidentifier)
as
    begin;

With CompletionOfCourses As(
	  Select 

		TE.LearnerUserIdentifier as LearnerUserIdentifier
		,LGC.CredentialIdentifier as CredentialIdentifier
		
	  from records.TTask AS T
		inner join records.TTaskEnrollment as TE ON TE.TaskIdentifier = T.TaskIdentifier
		inner join courses.TCourse       as C on C.CourseIdentifier = T.ObjectIdentifier
															and C.GradebookIdentifier is not null
		inner join records.QGradebook       as G on G.GradebookIdentifier = C.GradebookIdentifier
														and G.AchievementIdentifier is not null
		inner join achievements.QCredential    as LGC on LGC.AchievementIdentifier = G.AchievementIdentifier
															and LGC.UserIdentifier = TE.LearnerUserIdentifier
  
	  where 
		T.ProgramIdentifier = @ProgramIdentifier
		And T.ObjectType = 'Course' 
		AND LGC.CredentialIdentifier is not null
	), 
		CompletionOfAchievement As(
	  Select 
		TE.LearnerUserIdentifier as LearnerUserIdentifier
		,LA.CredentialIdentifier as CredentialIdentifier

	  
	  from records.TTask AS T
	  inner join records.TTaskEnrollment as TE ON TE.TaskIdentifier = T.TaskIdentifier
        inner join achievements.QCredential    as LA on LA.AchievementIdentifier = T.ObjectIdentifier
                                                            and LA.UserIdentifier = TE.LearnerUserIdentifier
  
  where T.ProgramIdentifier = @ProgramIdentifier
  And T.ObjectType = 'Achievement' 
),
	CompletionOfLogbook As(
		  Select 
			
			TE.LearnerUserIdentifier as LearnerUserIdentifier
			,LA.CredentialIdentifier as CredentialIdentifier
		
		  from records.TTask AS T
			inner join records.TTaskEnrollment as TE ON TE.TaskIdentifier = T.TaskIdentifier
			inner join records.QJournalSetup       as L on L.JournalSetupIdentifier = T.ObjectIdentifier
															and L.AchievementIdentifier is not null
			inner join achievements.QCredential    as LA on LA.AchievementIdentifier = L.AchievementIdentifier
															and LA.UserIdentifier = TE.LearnerUserIdentifier
  
	  where T.ProgramIdentifier = @ProgramIdentifier
	  And T.ObjectType = 'Logbook' 
	  AND LA.CredentialIdentifier is not null
	),
	CompletionOfProgram As(
		Select 
			PE.LearnerUserIdentifier AS LearnerUserIdentifier 
			,LA.CredentialIdentifier as CredentialIdentifier
	  
		from records.TProgram AS P
			inner join records.TProgramEnrollment as PE ON PE.ProgramIdentifier = P.ProgramIdentifier
			inner join achievements.QCredential    as LA on LA.AchievementIdentifier = P.AchievementIdentifier
															and LA.UserIdentifier = PE.LearnerUserIdentifier
  
		where P.ProgramIdentifier = @ProgramIdentifier
			AND LA.CredentialIdentifier is not null
	),
	UnionAllCompletionCount AS
	(
		SELECT * FROM CompletionOfCourses
		UNION ALL
		SELECT * FROM CompletionOfAchievement
		UNION ALL
		SELECT * FROM CompletionOfLogbook
		UNION ALL
		SELECT * FROM CompletionOfProgram
	)

			select 
				R.LearnerUserIdentifier AS LearnerUserIdentifier
				,CredentialIdentifier as CredentialIdentifier

			from UnionAllCompletionCount as R
    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [records].[GetProgramEnrollmentTaskCompletionCounterForUsers]
(@ProgramIdentifier uniqueidentifier)
as
    begin;
        with tempCompletionCounter as (
                                          select E.ProgramIdentifier
                                               , ObjectIdentifier
                                               , TaskIdentifier
                                               , E.UserFullName
                                               , E.UserIdentifier
                                               , case
                                                     when ObjectType = 'Survey' then
                                                         case
                                                             when S.ResponseSessionCompleted is not null then
                                                                 1
                                                             else
                                                                 0
                                                         end
                                                     when ObjectType = 'Achievement' then
                                                         case
                                                             when A.CredentialGranted is not null then
                                                                 1
                                                             else
                                                                 0
                                                         end
                                                     else
                                                         case
                                                             when LA.CredentialGranted is not null then
                                                                 1
                                                             else
                                                                 0
                                                         end
                                                 end               as CompletionCounter
                                               , ObjectType
                                          from records.TTask                         as T
                                               inner join records.VProgramEnrollment as E on E.ProgramIdentifier = T.ProgramIdentifier
                                               left join surveys.QResponseSession    as S on S.OrganizationIdentifier = T.OrganizationIdentifier
                                                                                                 and S.RespondentUserIdentifier = E.UserIdentifier
                                                                                                 and T.ObjectIdentifier = S.SurveyFormIdentifier
                                                                                                 and S.ResponseSessionStatus = 'Completed'
                                               left join achievements.QCredential    as A on A.AchievementIdentifier = T.ObjectIdentifier
                                                                                                 and A.UserIdentifier = E.UserIdentifier
                                               left join records.QJournalSetup       as L on L.JournalSetupIdentifier = T.ObjectIdentifier
                                                                                                 and L.AchievementIdentifier is not null
                                               left join achievements.QCredential    as LA on LA.AchievementIdentifier = L.AchievementIdentifier
                                                                                                  and LA.UserIdentifier = E.UserIdentifier
                                          where T.ProgramIdentifier = @ProgramIdentifier
                                      )
        select ProgramIdentifier
             , UserIdentifier
             , sum(CompletionCounter) as CompletionCounter
             , count(TaskIdentifier)  as TaskCount
        from tempCompletionCounter
        group by ProgramIdentifier
               , UserIdentifier;
    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [records].[GetProgramLastTaskCompletionDate]
(
    @ProgramIdentifier UNIQUEIDENTIFIER,
	@UserIdentifier UNIQUEIDENTIFIER
)
AS
BEGIN
	  SELECT TOP(1) 
	  "CompletionDate" = 
	  CASE
		  WHEN ObjectType = 'Survey' THEN 
			S.ResponseSessionCompleted
		  WHEN ObjectType = 'Achievement' THEN 
			A.CredentialGranted
		  ELSE 
			LA.CredentialGranted
	  END
  FROM [records].[TTask] AS T
  Left JOIN [records].[VProgramEnrollment] AS E 
	ON E.ProgramIdentifier = T.ProgramIdentifier
  LEFT JOIN [surveys].[QResponseSession] AS S 
	ON S.OrganizationIdentifier = T.OrganizationIdentifier 
	AND S.RespondentUserIdentifier = E.UserIdentifier
	AND T.ObjectIdentifier = S.SurveyFormIdentifier
	AND S.ResponseSessionStatus = 'Completed'
  LEFT JOIN [achievements].[QCredential] AS A
	ON A.AchievementIdentifier = T.ObjectIdentifier
	AND A.UserIdentifier = E.UserIdentifier
  LEFT JOIN [records].[QJournalSetup] AS L
	ON L.JournalSetupIdentifier = T.ObjectIdentifier
	AND L.AchievementIdentifier IS NOT NULL
  LEFT JOIN [achievements].[QCredential] AS LA
	ON LA.AchievementIdentifier = L.AchievementIdentifier
	AND LA.UserIdentifier = E.UserIdentifier
  Where T.ProgramIdentifier = @ProgramIdentifier and E.UserIdentifier = @UserIdentifier
  Order By CompletionDate desc
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [records].[GetProgramLearnerAchievements]
(
    @ProgramIdentifier UNIQUEIDENTIFIER
)
as
    begin;

SELECT 
	PE.LearnerUserIdentifier
	,LA.CredentialIdentifier as CredentialIdentifier
	  
FROM records.TProgram AS P
	left join records.TProgramEnrollment as PE ON PE.ProgramIdentifier = P.ProgramIdentifier
	left join achievements.QCredential    as LA on LA.AchievementIdentifier = P.AchievementIdentifier
													and LA.UserIdentifier = PE.LearnerUserIdentifier
	left join achievements.QAchievement    as LGA on LGA.AchievementIdentifier = P.AchievementIdentifier
  
WHERE P.ProgramIdentifier = @ProgramIdentifier
	AND LA.CredentialIdentifier is not null

    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [records].[GetProgramTaskCompletionCountForUser]
(
    @ProgramIdentifier uniqueidentifier
  , @UserIdentifier uniqueidentifier
) as
with                                    CompletionOfCourses as (
                                                                   select T.ProgramIdentifier as ProgramIdentifier
                                                                        , T.ObjectIdentifier  as ObjectIdentifier
                                                                        , T.TaskIdentifier    as TaskIdentifier
                                                                        , case
                                                                              when LGC.CredentialGranted is not null then
                                                                                  1
                                                                              else
                                                                                  0
                                                                          end                 as CompletionCounter
                                                                   from records.TTask                 as T
                                                                   left join courses.TCourse       as C on C.CourseIdentifier = T.ObjectIdentifier
																														and C.GradebookIdentifier is not null
																	left join records.QGradebook       as G on G.GradebookIdentifier = C.GradebookIdentifier
																													and G.AchievementIdentifier is not null
																	left join achievements.QCredential    as LGC on LGC.AchievementIdentifier = G.AchievementIdentifier
																														and LGC.UserIdentifier = @UserIdentifier
                                                                   where T.ProgramIdentifier = @ProgramIdentifier
                                                                         and T.ObjectType = 'Course'
                                                               )
                                      , CompletionOfSurveys as (
                                                                   select T.ProgramIdentifier as ProgramIdentifier
                                                                        , T.ObjectIdentifier  as ObjectIdentifier
                                                                        , T.TaskIdentifier    as TaskIdentifier
                                                                        , case
                                                                              when T.ObjectType = 'Survey' then
                                                                                  case
                                                                                      when S.ResponseSessionCompleted is not null then
                                                                                          1
                                                                                      else
                                                                                          0
                                                                                  end
                                                                              else
                                                                                  0
                                                                          end                 as CompletionCounter
                                                                   from records.TTask                      as T
                                                                        left join surveys.QResponseSession as S on S.OrganizationIdentifier = T.OrganizationIdentifier
                                                                                                                   and S.RespondentUserIdentifier = @UserIdentifier
                                                                                                                   and T.ObjectIdentifier = S.SurveyFormIdentifier
                                                                                                                   and S.ResponseSessionStatus = 'Completed'
                                                                   where T.ProgramIdentifier = @ProgramIdentifier
                                                                         and T.ObjectType = 'Survey'
                                                               )
                                      , CompletionOfAchievement as (
                                                                       select T.ProgramIdentifier as ProgramIdentifier
                                                                            , T.ObjectIdentifier  as ObjectIdentifier
                                                                            , T.TaskIdentifier    as TaskIdentifier
                                                                            , case
                                                                                  when T.ObjectType = 'Achievement' then
                                                                                      case
                                                                                          when A.CredentialGranted is not null then
                                                                                              1
                                                                                          else
                                                                                              0
                                                                                      end
                                                                                  else
                                                                                      0
                                                                              end                 as CompletionCounter
                                                                       from records.TTask                      as T
                                                                            left join achievements.QCredential as A on A.AchievementIdentifier = T.ObjectIdentifier
                                                                                                                       and A.UserIdentifier = @UserIdentifier
																			Left join records.TProgram as P on P.ProgramIdentifier = @ProgramIdentifier
																			
                                                                       where T.ProgramIdentifier = @ProgramIdentifier AND P.AchievementIdentifier <> T.ObjectIdentifier
                                                                             and T.ObjectType = 'Achievement'
                                                                   )
                                      , CompletionOfLogbook as (
                                                                   select T.ProgramIdentifier as ProgramIdentifier
                                                                        , T.ObjectIdentifier  as ObjectIdentifier
                                                                        , T.TaskIdentifier    as TaskIdentifier
                                                                        , case
                                                                              when T.ObjectType = 'Logbook' then
                                                                                  case
                                                                                      when LA.CredentialGranted is not null then
                                                                                          1
                                                                                      else
                                                                                          0
                                                                                  end
                                                                              else
                                                                                  0
                                                                          end                 as CompletionCounter
                                                                   from records.TTask                      as T
                                                                        left join records.QJournalSetup    as L on L.JournalSetupIdentifier = T.ObjectIdentifier
                                                                                                                   and L.AchievementIdentifier is not null
                                                                        left join achievements.QCredential as LA on LA.AchievementIdentifier = L.AchievementIdentifier
                                                                                                                    and LA.UserIdentifier = @UserIdentifier
                                                                   where T.ProgramIdentifier = @ProgramIdentifier
                                                                         and T.ObjectType = 'Logbook'
                                                               )
                                      , CompletionOfAssesments as (
                                                                      select T.ProgramIdentifier as ProgramIdentifier
                                                                           , T.ObjectIdentifier  as ObjectIdentifier
                                                                           , T.TaskIdentifier    as TaskIdentifier
                                                                           , case
                                                                                 when T.ObjectType = 'AssessmentForm' then
                                                                                     case
                                                                                         when S.AttemptIsPassing = 1 then
                                                                                             1
                                                                                         else
                                                                                             0
                                                                                     end
                                                                                 else
                                                                                     0
                                                                             end                 as CompletionCounter
                                                                      from records.TTask                  as T
                                                                           left join assessments.QAttempt as S on S.OrganizationIdentifier = T.OrganizationIdentifier
                                                                                                                  and S.LearnerUserIdentifier = @UserIdentifier
                                                                                                                  and T.ObjectIdentifier = S.FormIdentifier
                                                                                                                  and S.AttemptIsPassing = 1
                                                                      where T.ProgramIdentifier = @ProgramIdentifier
                                                                            and T.ObjectType = 'AssessmentForm'
                                                                  )
                                      , UnionAllCompletionCount as (
                                                                       select *
                                                                       from CompletionOfCourses
                                                                       union all
                                                                       select *
                                                                       from CompletionOfSurveys
                                                                       union all
                                                                       select *
                                                                       from CompletionOfLogbook
                                                                       union all
                                                                       select *
                                                                       from CompletionOfAssesments
                                                                       union all
                                                                       select *
                                                                       from CompletionOfAchievement
                                                                   )
select @ProgramIdentifier                as ProgramIdentifier
     , @UserIdentifier                   as UserIdentifier
     , isnull(sum(CompletionCounter), 0) as CompletionCounter
     , count(TaskIdentifier)
                                         as
     TaskCount
from UnionAllCompletionCount;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [records].[GetProgramTaskCompletionForUser]
(
  @ProgramIdentifier uniqueidentifier
, @UserIdentifier uniqueidentifier
)
as
  begin;
  
    with CompletionOfCourses as (
                                  select T.ProgramIdentifier as ProgramIdentifier
                                       , T.ObjectIdentifier  as ObjectIdentifier
                                       , T.TaskIdentifier    as TaskIdentifier
                                       , LGC.UserIdentifier  as UserIdentifier
                                       , T.ObjectType
                                       , case
                                           when T.ObjectType = 'Course' then
                                             case
                                               when LGC.CredentialGranted is not null then
                                                 1
                                               else
                                                 0
                                             end
                                           else
                                             0
                                         end                 as CompletionCounter
                                  from records.TTask                      as T
                                       left join courses.TCourse          as C on C.CourseIdentifier = T.ObjectIdentifier
                                                                                  and C.GradebookIdentifier is not null
                                       left join records.QGradebook       as G on G.GradebookIdentifier = C.GradebookIdentifier
                                                                                  and G.AchievementIdentifier is not null
                                       left join achievements.QCredential as LGC on LGC.AchievementIdentifier = G.AchievementIdentifier
                                                                                    and LGC.UserIdentifier = @UserIdentifier
                                  where T.ProgramIdentifier = @ProgramIdentifier
                                        and T.ObjectType = 'Course'
                                )
       , CompletionOfLogbook as (
                                  select T.ProgramIdentifier as ProgramIdentifier
                                       , T.ObjectIdentifier  as ObjectIdentifier
                                       , T.TaskIdentifier    as TaskIdentifier
                                       , LA.UserIdentifier   as UserIdentifier
                                       , T.ObjectType
                                       , case
                                           when T.ObjectType = 'Logbook' then
                                             case
                                               when LA.CredentialGranted is not null then
                                                 1
                                               else
                                                 0
                                             end
                                           else
                                             0
                                         end                 as CompletionCounter
                                  from records.TTask                      as T
                                       left join records.QJournalSetup    as L on L.JournalSetupIdentifier = T.ObjectIdentifier
                                                                                  and L.AchievementIdentifier is not null
                                       left join achievements.QCredential as LA on LA.AchievementIdentifier = L.AchievementIdentifier
                                                                                   and LA.UserIdentifier = @UserIdentifier
                                  where T.ProgramIdentifier = @ProgramIdentifier
                                        and T.ObjectType = 'Logbook'
                                )
       , CompletionOfSurveys as (
                                  select T.ProgramIdentifier        as ProgramIdentifier
                                       , T.ObjectIdentifier         as ObjectIdentifier
                                       , T.TaskIdentifier           as TaskIdentifier
                                       , S.RespondentUserIdentifier as UserIdentifier
                                       , T.ObjectType
                                       , case
                                           when T.ObjectType = 'Survey' then
                                             case
                                               when S.ResponseSessionStatus = 'Completed' then
                                                 1
                                               else
                                                 0
                                             end
                                           else
                                             0
                                         end                        as CompletionCounter
                                  from records.TTask                      as T
                                       left join surveys.QResponseSession as S on S.OrganizationIdentifier = T.OrganizationIdentifier
                                                                                  and S.RespondentUserIdentifier = @UserIdentifier
                                                                                  and T.ObjectIdentifier = S.SurveyFormIdentifier
                                                                                  and S.ResponseSessionStatus = 'Completed'
                                  where T.ProgramIdentifier = @ProgramIdentifier
                                        and T.ObjectType = 'Survey'
                                )
       , CompletionOfAchievement as (
                                      select T.ProgramIdentifier as ProgramIdentifier
                                           , T.ObjectIdentifier  as ObjectIdentifier
                                           , T.TaskIdentifier    as TaskIdentifier
                                           , LA.UserIdentifier   as UserIdentifier
                                           , T.ObjectType
                                           , case
                                               when T.ObjectType = 'Achievement' then
                                                 case
                                                   when LA.CredentialGranted is not null then
                                                     1
                                                   else
                                                     0
                                                 end
                                               else
                                                 0
                                             end                 as CompletionCounter
                                      from records.TTask                      as T
                                           left join achievements.QCredential as LA on LA.AchievementIdentifier = T.ObjectIdentifier
                                                                                       and LA.UserIdentifier = @UserIdentifier
                                           left join records.TProgram         as P on P.ProgramIdentifier = @ProgramIdentifier
                                      where T.ProgramIdentifier = @ProgramIdentifier
                                            and (
                                                  P.AchievementIdentifier is null
                                                  or P.AchievementIdentifier <> T.ObjectIdentifier
                                                )
                                            and T.ObjectType = 'Achievement'
                                    )
       , CompletionOfAssesments as (
                                     select T.ProgramIdentifier     as ProgramIdentifier
                                          , T.ObjectIdentifier      as ObjectIdentifier
                                          , T.TaskIdentifier        as TaskIdentifier
                                          , S.LearnerUserIdentifier as UserIdentifier
                                          , T.ObjectType
                                          , case
                                              when T.ObjectType = 'AssessmentForm' then
                                                case
                                                  when S.AttemptIsPassing = 1 then
                                                    1
                                                  else
                                                    0
                                                end
                                              else
                                                0
                                            end                     as CompletionCounter
                                     from records.TTask                  as T
                                          left join assessments.QAttempt as S on S.OrganizationIdentifier = T.OrganizationIdentifier
                                                                                 and S.LearnerUserIdentifier = @UserIdentifier
                                                                                 and T.ObjectIdentifier = S.FormIdentifier
                                                                                 and S.AttemptIsPassing = 1
                                     where T.ProgramIdentifier = @ProgramIdentifier
                                           and T.ObjectType = 'AssessmentForm'
                                   )
       , UnionAllCompletionCount as (
                                      select *
                                      from CompletionOfCourses
                                      union all
                                      select *
                                      from CompletionOfLogbook
                                      union all
                                      select *
                                      from CompletionOfSurveys
                                      union all
                                      select *
                                      from CompletionOfAssesments
                                      union all
                                      select *
                                      from CompletionOfAchievement
                                    )
    select ProgramIdentifier
         , ObjectIdentifier
         , ObjectType
         , TaskIdentifier
         , CompletionCounter
         , UserIdentifier
    from UnionAllCompletionCount;
    
  end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [records].[MyCompletedCourses]
(
    @UserIdentifier UNIQUEIDENTIFIER,
    @OrganizationIdentifier UNIQUEIDENTIFIER
)
AS
BEGIN;
    SELECT VCredential.AchievementIdentifier AS AchievementIdentifier,
           CredentialIdentifier AS CredentialIdentifier,
           VCredential.OrganizationIdentifier AS OrganizationIdentifier,
           TCourse.CourseIdentifier AS CourseIdentifier,
           QGradebook.GradebookIdentifier AS GradebookIdentifier,
           UserIdentifier AS LearnerIdentifier,
           QPage.PageIdentifier AS PageIdentifier,
           CourseName AS CourseName,
           CourseHook AS CourseHook,
           CourseImage AS CourseImage,
           CourseSlug AS CourseSlug,
           TContent.ContentText AS PageCourseImage,
           AchievementLabel AS AchievementLabel,
           AchievementTitle AS AchievementTitle,
           sites.GetQPagePath(sites.QPage.PageIdentifier, 0) AS CourseUrl,
           CAST(1 AS BIT) AS IsCompleted
    FROM achievements.VCredential
        INNER JOIN records.QGradebook
            ON QGradebook.AchievementIdentifier = VCredential.AchievementIdentifier
        INNER JOIN courses.TCourse
            ON TCourse.GradebookIdentifier = QGradebook.GradebookIdentifier
        LEFT OUTER JOIN sites.QPage
            ON QPage.ObjectIdentifier = TCourse.CourseIdentifier
               AND QPage.OrganizationIdentifier = TCourse.OrganizationIdentifier
        LEFT OUTER JOIN contents.TContent
            ON TContent.ContainerIdentifier = QPage.PageIdentifier
               AND QPage.OrganizationIdentifier = TContent.OrganizationIdentifier
               AND TContent.ContentLabel = 'ImageURL'
               AND TContent.ContentLanguage = 'en'
    WHERE VCredential.UserIdentifier = @UserIdentifier
          AND VCredential.OrganizationIdentifier = @OrganizationIdentifier
          AND TCourse.CourseIdentifier IS NOT NULL
          AND QGradebook.GradebookIdentifier IS NOT NULL
          AND
          (
              NOT EXISTS
    (
        SELECT *
        FROM contacts.TGroupPermission
        WHERE TGroupPermission.ObjectIdentifier = TCourse.CourseIdentifier
              AND TGroupPermission.OrganizationIdentifier = @OrganizationIdentifier
    )
              OR EXISTS
    (
        SELECT *
        FROM contacts.TGroupPermission AS Permission
            INNER JOIN contacts.QGroup
                ON QGroup.GroupIdentifier = Permission.GroupIdentifier
            INNER JOIN contacts.Membership
                ON QGroup.GroupIdentifier = Membership.GroupIdentifier
        WHERE Permission.ObjectIdentifier = TCourse.CourseIdentifier
              AND Permission.OrganizationIdentifier = @OrganizationIdentifier
              AND QGroup.OrganizationIdentifier = @OrganizationIdentifier
              AND Membership.UserIdentifier = @UserIdentifier
    )
          );
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [records].[MyEnrolledCourses]
(
    @UserIdentifier UNIQUEIDENTIFIER,
    @OrganizationIdentifier UNIQUEIDENTIFIER
)
AS
BEGIN;
    SELECT TCourse.CourseIdentifier AS CourseIdentifier,
           QEnrollment.GradebookIdentifier AS GradebookIdentifier,
           LearnerIdentifier AS LearnerIdentifier,
           TCourse.OrganizationIdentifier AS OrganizationIdentifier,
           EnrollmentIdentifier AS EnrollmentIdentifier,
           PeriodIdentifier AS PeriodIdentifier,
           QPage.PageIdentifier AS PageIdentifier,
           EnrollmentStarted AS EnrollmentStarted,
           EnrollmentCompleted AS EnrollmentCompleted,
           CourseName AS CourseName,
           CourseHook AS CourseHook,
           CourseImage AS CourseImage,
           CourseSlug AS CourseSlug,
           TContent.ContentText AS PageCourseImage,
           sites.GetQPagePath(sites.QPage.PageIdentifier, 0) AS CourseUrl
    FROM records.QEnrollment
        INNER JOIN courses.TCourse
            ON TCourse.GradebookIdentifier = QEnrollment.GradebookIdentifier
        LEFT OUTER JOIN sites.QPage
            ON QPage.ObjectIdentifier = TCourse.CourseIdentifier
               AND QPage.OrganizationIdentifier = TCourse.OrganizationIdentifier
        LEFT OUTER JOIN contents.TContent
            ON TContent.ContainerIdentifier = QPage.PageIdentifier
               AND QPage.OrganizationIdentifier = TContent.OrganizationIdentifier
               AND TContent.ContentLabel = 'ImageURL'
               AND TContent.ContentLanguage = 'en'
    WHERE LearnerIdentifier = @UserIdentifier
          AND TCourse.OrganizationIdentifier = @OrganizationIdentifier
          AND TCourse.CourseIdentifier IS NOT NULL
          AND CourseName IS NOT NULL
          AND
          (
              NOT EXISTS
    (
        SELECT *
        FROM contacts.TGroupPermission
        WHERE TGroupPermission.ObjectIdentifier = TCourse.CourseIdentifier
              AND TGroupPermission.OrganizationIdentifier = @OrganizationIdentifier
    )
              OR EXISTS
    (
        SELECT *
        FROM contacts.TGroupPermission AS Permission
            INNER JOIN contacts.QGroup
                ON QGroup.GroupIdentifier = Permission.GroupIdentifier
            INNER JOIN contacts.Membership
                ON QGroup.GroupIdentifier = Membership.GroupIdentifier
        WHERE Permission.ObjectIdentifier = TCourse.CourseIdentifier
              AND Permission.OrganizationIdentifier = @OrganizationIdentifier
              AND QGroup.OrganizationIdentifier = @OrganizationIdentifier
              AND Membership.UserIdentifier = @UserIdentifier
    )
          );
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [records].[MyEnrolledCoursesCount]
(
    @UserIdentifier uniqueidentifier
  , @OrganizationIdentifier uniqueidentifier
)
as
    begin
        select count(*)
        from records.QEnrollment
             inner join courses.TCourse on TCourse.GradebookIdentifier = QEnrollment.GradebookIdentifier
        where QEnrollment.LearnerIdentifier = @UserIdentifier
              and TCourse.OrganizationIdentifier = @OrganizationIdentifier
              and TCourse.CourseIdentifier is not null
              and TCourse.CourseName is not null
              and (
                      not exists (
                                     select *
                                     from contacts.TGroupPermission
                                     where TGroupPermission.ObjectIdentifier = TCourse.CourseIdentifier
                                           and TGroupPermission.OrganizationIdentifier = @OrganizationIdentifier
                                 )
                      or exists (
                                    select *
                                    from contacts.TGroupPermission as Permission
                                         inner join contacts.QGroup on QGroup.GroupIdentifier = Permission.GroupIdentifier
                                         inner join contacts.Membership on QGroup.GroupIdentifier = Membership.GroupIdentifier
                                    where Permission.ObjectIdentifier = TCourse.CourseIdentifier
                                          and Permission.OrganizationIdentifier = @OrganizationIdentifier
                                          and QGroup.OrganizationIdentifier = @OrganizationIdentifier
                                          and Membership.UserIdentifier = @UserIdentifier
                                )
                  );
    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [reports].[RefreshLearnerProgramSummary](@OrganizationIdentifier UNIQUEIDENTIFIER)
AS
BEGIN

    WITH Participant AS ( SELECT GETUTCDATE()        AS AsAt

                               , P.Created           AS LearnerAccountCreated
                               , P.UserAccessGranted AS LearnerAccessGranted

                               , U.Email             AS LearnerEmail
                               , U.FullName          AS LearnerName
                               , P.Gender            AS LearnerGender
                               , P.Referrer          AS ReferrerName
                               , P.ReferrerOther     AS ReferrerNameOther
                               , P.ImmigrationLandingDate
                               , CASE WHEN P.ImmigrationLandingDate > GETDATE() THEN 'Pre'
                                      WHEN P.ImmigrationLandingDate <= GETDATE() THEN 'Post'
                                      ELSE NULL END  AS ArrivalStatus
                               , NULL                AS LearnerStreams
                               , NULL                AS ReferrerProvince -- (SELECT toolbox.ConcatenateText(A.Province, ', ') FROM contacts.[Group] AS G INNER JOIN locations.Address AS A ON A.AddressIdentifier = G.BillingAddressIdentifier WHERE G.GroupName = U.Referrer AND GroupType = 'Employer')
                               , NULL                AS ReferrerRole     -- (SELECT toolbox.ConcatenateText(G.GroupLabel, ', ') FROM contacts.[Group] AS G WHERE G.GroupName = U.Referrer AND GroupType = 'Employer')
                               , NULL                AS ReferrerIndustry -- (SELECT toolbox.ConcatenateText(G.GroupCategory, ', ') FROM contacts.[Group] AS G WHERE G.GroupName = U.Referrer AND GroupType = 'Employer')

                               , U.UserIdentifier
                               , T.OrganizationIdentifier

                          FROM identities.[User] AS U WITH (NOLOCK)
                                   INNER JOIN contacts.QPerson AS P WITH (NOLOCK)
                                              ON P.UserIdentifier = U.UserIdentifier
                                   INNER JOIN accounts.QOrganization AS T WITH (NOLOCK)
                                              ON T.OrganizationIdentifier = P.OrganizationIdentifier

                          WHERE T.OrganizationIdentifier = @OrganizationIdentifier
                            AND P.IsLearner = 1 ),

         Progress AS ( SELECT U.LearnerIdentifier                                               AS UserIdentifier
                            , G.OrganizationIdentifier
                            , G.GradebookTitle                                                  AS GradebookName
                            , U.EnrollmentStarted                                               AS GradebookUserAdded
                            , ( SELECT COUNT(*)
                                FROM records.QGradeItem AS I WITH (NOLOCK)
                                WHERE I.GradeItemType = 'Score'
                                  AND I.GradeItemFormat IN ('Boolean', 'Percent')
                                  AND I.GradebookIdentifier = G.GradebookIdentifier )           AS GradebookItems
                            , ( SELECT COUNT(*)
                                FROM records.QProgress AS P WITH (NOLOCK)
                                WHERE P.ProgressCompleted IS NOT NULL
                                  AND P.UserIdentifier = U.LearnerIdentifier
                                  AND P.GradebookIdentifier = G.GradebookIdentifier )           AS GradebookItemsCompleted
                            , (CASE WHEN C.CredentialStatus = 'Valid' THEN 'Yes' ELSE 'No' END) AS GradebookCompleted

                       FROM records.QEnrollment AS U WITH (NOLOCK)
                                INNER JOIN records.QGradebook AS G WITH (NOLOCK)
                                           ON G.GradebookIdentifier = U.GradebookIdentifier AND
                                              G.OrganizationIdentifier = @OrganizationIdentifier
                                LEFT JOIN achievements.QCredential AS C WITH (NOLOCK)
                                          ON C.AchievementIdentifier = G.AchievementIdentifier AND
                                             C.UserIdentifier = U.LearnerIdentifier AND C.CredentialStatus = 'Valid' )

    INSERT
    INTO reports.QLearnerProgramSummary
    ( AsAt
    , SummaryIdentifier
    , OrganizationIdentifier
    , UserIdentifier
    , LearnerAccountCreated
    , LearnerAccessGranted
    , LearnerAddedToProgram
    , ImmigrationArrivalDate
    , ImmigrationArrivalStatus
    , LearnerEmail
    , LearnerName
    , LearnerGender
    , ReferrerName
    , ReferrerNameOther
    , ReferrerProvince
    , ReferrerRole
    , ReferrerIndustry
    , LearnerStreams
    , ProgramName
    , ProgramStatus
    , ProgramGradeItems
    , ProgramGradeItemsCompleted)

    SELECT AsAt

         , NEWID()
         , @OrganizationIdentifier
         , X.UserIdentifier

         , X.LearnerAccountCreated
         , X.LearnerAccessGranted
         , Y.GradebookUserAdded
         , X.ImmigrationLandingDate

         , X.ArrivalStatus
         , X.LearnerEmail
         , X.LearnerName
         , X.LearnerGender
         , X.ReferrerName
         , X.ReferrerNameOther
         , X.ReferrerProvince
         , X.ReferrerRole
         , X.ReferrerIndustry

         , X.LearnerStreams
         , Y.GradebookName
         , Y.GradebookCompleted
         , Y.GradebookItems
         , Y.GradebookItemsCompleted

    FROM Participant AS X
             LEFT JOIN Progress AS Y
                       ON X.UserIdentifier = Y.UserIdentifier AND X.OrganizationIdentifier = Y.OrganizationIdentifier

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [settings].[IncrementSequence](@organization UNIQUEIDENTIFIER, @type VARCHAR(30), @count INT, @startNumber INT)
AS 
BEGIN

  BEGIN TRANSACTION
    DECLARE @maximum INT = ISNULL((SELECT MAX(SequenceNumber) FROM settings.TSequence WHERE OrganizationIdentifier = @organization AND SequenceType = @type),@startNumber-1);
    DECLARE @i INT = 1;
    WHILE @i <= @count
    BEGIN
	  INSERT INTO settings.TSequence (OrganizationIdentifier, SequenceType, SequenceNumber) SELECT @organization, @type, @maximum + @i;
  	  SET @i = @i + 1;
    END
  COMMIT TRANSACTION
  
  SELECT TOP (@count) SequenceNumber 
  FROM settings.TSequence 
  WHERE OrganizationIdentifier = @organization AND SequenceType = @type 
  ORDER BY SequenceNumber DESC

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sites].[GetHelpPages] (@Keyword nvarchar(200))
as
    declare @Like nvarchar(max) =
        case
            when @Keyword is not null then '%' + @Keyword + '%'
            else null
        end

    select
        P.WebSiteIdentifier
        ,P.WebPageIdentifier
        ,P.PathUrl
        ,P.WebPageTitle
        ,C.ContentLabel
        ,C.ContentSnip
        ,C.ContentText
        ,C.ContentHtml
    from
        sites.VWebPageHierarchy as P
        left join contents.TContent as C on C.ContainerIdentifier = P.WebPageIdentifier
    where
        P.WebSiteIdentifier = '3F59182E-F745-4867-B38C-AD90014EFCEB' -- Global Help Site
        and C.ContentLanguage = 'en'
        and C.ContentLabel in ('Body', 'Paragraphs')
	    and (
            @Like is null
	        or P.WebPageTitle like @Like
	        or C.ContentSnip like @Like
	        or C.ContentText like @Like
	        or C.ContentHtml like @Like
	    )
    order by
        P.WebPageTitle
        ,P.PathUrl
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sites].[GetOrgHelpPages] (@OrganizationId uniqueidentifier, @Keyword nvarchar(200))
as
    declare @Like nvarchar(max) =
        case
            when @Keyword is not null then '%' + @Keyword + '%'
            else null
        end

    select
        P.WebSiteIdentifier
        ,P.WebPageIdentifier
        ,P.PathUrl
        ,P.WebPageTitle
        ,P.WebPageType
        ,C.ContentLabel
        ,C.ContentSnip
        ,C.ContentText
        ,C.ContentHtml
    from
        sites.VWebPageHierarchy as P
        inner join contents.TContent as C on C.ContainerIdentifier = P.WebPageIdentifier
    where
        P.OrganizationIdentifier = @OrganizationId
        and P.SiteIsPortal = 1
        and C.ContentLanguage = 'en'
        and C.ContentLabel in ('Body', 'Paragraphs')
	    and (
            @Like is null
	        or P.WebPageTitle like @Like
	        or C.ContentSnip like @Like
	        or C.ContentText like @Like
	        or C.ContentHtml like @Like
	    )
    order by
        P.WebPageTitle
        ,P.PathUrl
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sites].[GetQDownstreamPages]
(
    @PageIdentifier UNIQUEIDENTIFIER
)
AS
BEGIN
    WITH Hierarchy AS (
        SELECT
            *
        FROM
            sites.QPage
        WHERE
            QPage.PageIdentifier = @PageIdentifier

        UNION ALL

        SELECT
            c.*
        FROM
            Hierarchy AS p
            INNER JOIN sites.QPage AS c ON c.ParentPageIdentifier = p.PageIdentifier
    )
    SELECT
        *
    FROM
        Hierarchy;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sites].[GetQSitePages]
(
    @SiteIdentifier UNIQUEIDENTIFIER
)
AS
BEGIN
      WITH Downstream AS (
        SELECT
            *
           ,0 AS Depth
        FROM
            sites.QPage
        WHERE
            QPage.SiteIdentifier = @SiteIdentifier
            AND QPage.ParentPageIdentifier IS NULL

        UNION ALL

        SELECT
            c.*
           ,p.Depth + 1
        FROM
            Downstream AS p
            INNER JOIN sites.QPage AS c ON c.ParentPageIdentifier = p.PageIdentifier
        WHERE
            c.SiteIdentifier = @SiteIdentifier
    )
    SELECT
        *
    FROM
        Downstream
    ORDER BY
        Depth
       ,Sequence;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sites].[GetQTreePages]
(
    @PageIdentifier UNIQUEIDENTIFIER
)
AS
BEGIN
    WITH Upstream AS (
        SELECT
            QPage.PageIdentifier
           ,QPage.ParentPageIdentifier
           ,'/' + CAST(PageIdentifier AS VARCHAR(MAX)) + '/' AS IdPath
           ,0 AS Depth
        FROM
            sites.QPage
        WHERE
            QPage.PageIdentifier = @PageIdentifier

        UNION ALL

        SELECT
            p.PageIdentifier
           ,p.ParentPageIdentifier
           ,c.IdPath + CAST(p.PageIdentifier AS VARCHAR(MAX)) + '/'
           ,c.Depth + 1
        FROM
            Upstream AS c
            INNER JOIN sites.QPage AS p ON c.ParentPageIdentifier = p.PageIdentifier
        WHERE
            c.IdPath NOT LIKE '%/' + CAST(p.PageIdentifier AS VARCHAR(MAX)) + '/%'
    ), Downstream AS (
        SELECT
            *
           ,'/' + CAST(PageIdentifier AS VARCHAR(MAX)) + '/' AS IdPath
           ,0 AS Depth
        FROM
            sites.QPage
        WHERE
            QPage.PageIdentifier = (SELECT TOP 1 PageIdentifier FROM Upstream ORDER BY Depth DESC)

        UNION ALL

        SELECT
            c.*
           ,p.IdPath + CAST(c.PageIdentifier AS VARCHAR(MAX)) + '/'
           ,p.Depth + 1
        FROM
            Downstream AS p
            INNER JOIN sites.QPage AS c ON c.ParentPageIdentifier = p.PageIdentifier
        WHERE
            p.IdPath NOT LIKE '%/' + CAST(c.PageIdentifier AS VARCHAR(MAX)) + '/%'
    )
    SELECT
        *
    FROM
        Downstream
    ORDER BY
        Depth
       ,Sequence;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [standards].[GetUserStandardFrameworks]
(
    @OrganizationIdentifier uniqueidentifier
   ,@UserIdentifier uniqueidentifier
)
as
with StartPoints as (
    select distinct
        QAttemptQuestion.CompetencyItemIdentifier as StandardIdentifier
    from
        assessments.QAttempt
        inner join assessments.QAttemptQuestion on QAttemptQuestion.AttemptIdentifier = QAttempt.AttemptIdentifier
    where
        QAttempt.LearnerUserIdentifier = @UserIdentifier
        and QAttempt.OrganizationIdentifier = @OrganizationIdentifier
        and QAttemptQuestion.CompetencyItemIdentifier is not null
        and QAttemptQuestion.CompetencyItemIdentifier <> '00000000-0000-0000-0000-000000000000'
), AllEdges as  (
    select
        ParentStandardIdentifier
       ,ChildStandardIdentifier
    from
        [standard].QStandardContainment

    union

    select
        ParentStandardIdentifier
       ,StandardIdentifier
    from
        [standard].QStandard
    where
        ParentStandardIdentifier is not null
), Upstream as (
    select
        ParentStandardIdentifier
       ,ChildStandardIdentifier
    from
        StartPoints
        inner join AllEdges on AllEdges.ChildStandardIdentifier = StartPoints.StandardIdentifier

    union all

    select
        AllEdges.ParentStandardIdentifier
       ,AllEdges.ChildStandardIdentifier
    from
        Upstream
        inner join AllEdges on AllEdges.ChildStandardIdentifier = Upstream.ParentStandardIdentifier
)
select
    Upstream.ParentStandardIdentifier
from
    Upstream
    inner join [standard].QStandard on QStandard.StandardIdentifier = Upstream.ParentStandardIdentifier
where
    QStandard.StandardType = 'Framework'

union

select
    QStandard.StandardIdentifier
from
    StartPoints
    inner join [standard].QStandard on QStandard.StandardIdentifier = StartPoints.StandardIdentifier
where
    QStandard.StandardType = 'Framework';
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [standards].[SelectStandardConnectionDownstream]
(@StandardFilter IdentifierList READONLY)
AS
BEGIN
    DECLARE @Iteration INT = 0;
    DECLARE @Result TABLE (
        FromStandardIdentifier UNIQUEIDENTIFIER NOT NULL
       ,ToStandardIdentifier UNIQUEIDENTIFIER NOT NULL
       ,ConnectionType VARCHAR(20) NOT NULL
       ,Iteration INT NOT NULL
    );

    SET NOCOUNT ON;

    INSERT INTO @Result
    SELECT
        FromStandardIdentifier
       ,ToStandardIdentifier
       ,ConnectionType
       ,@Iteration
    FROM
        standards.StandardConnection
    WHERE
        FromStandardIdentifier IN
        (
            SELECT IdentifierItem FROM @StandardFilter
        );

    WHILE @@ROWCOUNT > 0 AND @Iteration <= 100
      BEGIN
        SET @Iteration += 1;

        INSERT INTO @Result
        SELECT
            FromStandardIdentifier
           ,ToStandardIdentifier
           ,ConnectionType
           ,@Iteration
        FROM
            standards.StandardConnection
        WHERE
            FromStandardIdentifier IN
            (
                SELECT
                    ToStandardIdentifier
                FROM
                    @Result
                WHERE
                    Iteration = @Iteration - 1
                    AND ToStandardIdentifier NOT IN (SELECT FromStandardIdentifier FROM @Result)
            );

        IF @Iteration = 101
          BEGIN
            RAISERROR('The maximum iteration 100 has been exhausted before statement completion.', 16, 1);

            RETURN;
          END;
      END;

    SET NOCOUNT OFF;

    SELECT
        FromStandardIdentifier
       ,ToStandardIdentifier
       ,ConnectionType
    FROM
        @Result;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [standards].[SelectStandardConnectionUpstream]
(@StandardFilter IdentifierList READONLY)
AS
BEGIN
    DECLARE @Iteration INT = 0;
    DECLARE @Result TABLE (
        FromStandardIdentifier UNIQUEIDENTIFIER NOT NULL
       ,ToStandardIdentifier UNIQUEIDENTIFIER NOT NULL
       ,ConnectionType VARCHAR(20) NOT NULL
       ,Iteration INT NOT NULL
    );

    SET NOCOUNT ON;

    INSERT INTO @Result
    SELECT
        FromStandardIdentifier
       ,ToStandardIdentifier
       ,ConnectionType
       ,@Iteration
    FROM
        standards.StandardConnection
    WHERE
        ToStandardIdentifier IN
        (
            SELECT IdentifierItem FROM @StandardFilter
        );

    WHILE @@ROWCOUNT > 0 AND @Iteration <= 100
      BEGIN
        SET @Iteration += 1;

        INSERT INTO @Result
        SELECT
            FromStandardIdentifier
           ,ToStandardIdentifier
           ,ConnectionType
           ,@Iteration
        FROM
            standards.StandardConnection
        WHERE
            ToStandardIdentifier IN
            (
                SELECT
                    FromStandardIdentifier
                FROM
                    @Result
                WHERE
                    Iteration = @Iteration - 1
                    AND FromStandardIdentifier NOT IN (SELECT ToStandardIdentifier FROM @Result)
            );

        IF @Iteration = 101
          BEGIN
            RAISERROR('The maximum iteration 100 has been exhausted before statement completion.', 16, 1);

            RETURN;
          END;
      END;

    SET NOCOUNT OFF;

    SELECT
        FromStandardIdentifier
       ,ToStandardIdentifier
       ,ConnectionType
    FROM
        @Result;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [standards].[SelectStandardContainmentTree](
    @StandardFilter IdentifierList readonly
)
AS
BEGIN
    WITH AllEdges AS (
        SELECT
            ParentStandardIdentifier
           ,ChildStandardIdentifier
           ,ChildSequence
           ,0 AS Sequence
        FROM
            standards.StandardContainment

        UNION

        SELECT DISTINCT
            ParentStandardIdentifier
           ,StandardIdentifier
           ,0
           ,Sequence
        FROM
            standards.Standard
        WHERE
            ParentStandardIdentifier is not null
    ), Edges AS (
        SELECT
            ParentStandardIdentifier
           ,ChildStandardIdentifier
           ,ChildSequence
           ,Sequence
        FROM
            AllEdges
        WHERE
            ParentStandardIdentifier IN
            (
                select IdentifierItem FROM @StandardFilter
            )

        UNION ALL

        SELECT
            AllEdges.ParentStandardIdentifier
           ,AllEdges.ChildStandardIdentifier
           ,AllEdges.ChildSequence
           ,AllEdges.Sequence
        FROM
            AllEdges
            INNER JOIN Edges ON Edges.ChildStandardIdentifier = AllEdges.ParentStandardIdentifier
    )
    SELECT DISTINCT * FROM Edges ORDER BY Edges.ChildSequence, Sequence;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [standards].[SelectUpstreamStandards]
(@AssetFilter IdentifierList readonly)
as
    begin;
        with AllEdges as
            (
                select ParentStandardIdentifier, ChildStandardIdentifier from standards.StandardContainment
                union
                select distinct
                       ParentStandardIdentifier
                     , StandardIdentifier
                from
                    standards.Standard
                where
                    ParentStandardIdentifier is not null
            )
           , Edges as
            (
                select
                    ParentStandardIdentifier
                  , ChildStandardIdentifier
                  , 0 as Distance
                from
                    AllEdges
                where
                    ChildStandardIdentifier in
                    (
                        select IdentifierItem from @AssetFilter
                    )
                union all
                select
                    AllEdges.ParentStandardIdentifier
                  , AllEdges.ChildStandardIdentifier
                  , Edges.Distance + 1
                from
                    AllEdges
                    inner join
                    Edges on Edges.ParentStandardIdentifier = AllEdges.ChildStandardIdentifier
            )
        select distinct * from Edges;
    end;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [surveys].[GetValueFromColumn] @id VARCHAR(max), @schema sysname, @table sysname, @column sysname, @idcolumn sysname
AS
BEGIN
    DECLARE @sqlCommand varchar(max) = 'SELECT [' + @column + '] FROM [' + @schema + '].[' + @table + '] WHERE '
	   
   SELECT @sqlCommand = @sqlCommand 
   + '[' + @idcolumn + '] = ''' + @id + ''''
   EXEC (@sqlCommand)
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [surveys].[QResponseAnalysis]
(
    @QuestionIdX uniqueidentifier
  , @QuestionIdY uniqueidentifier
)
AS
BEGIN
    DECLARE @Language varchar(2);
    DECLARE @XControlType varchar(20);
    DECLARE @YControlType varchar(20);

    DECLARE @Answers TABLE
    (
        ResponseSessionIdentifier uniqueidentifier
       ,SurveyOptionIdentifier uniqueidentifier
       ,INDEX IX_Answers1 NONCLUSTERED (ResponseSessionIdentifier, SurveyOptionIdentifier)
    );

    DECLARE @XValues TABLE
    (
        SurveyOptionIdentifier uniqueidentifier
       ,Sequence int
       ,Text nvarchar(max)
    );

    DECLARE @YValues TABLE
    (
        SurveyOptionIdentifier uniqueidentifier
       ,Sequence int
       ,Text nvarchar(max)
    );

    SELECT
        @Language = QSurveyForm.SurveyFormLanguage
       ,@XControlType = QSurveyQuestion.SurveyQuestionType
    FROM
        surveys.QSurveyForm
        INNER JOIN surveys.QSurveyQuestion ON QSurveyQuestion.SurveyFormIdentifier = QSurveyForm.SurveyFormIdentifier
    where
        QSurveyQuestion.SurveyQuestionIdentifier = @QuestionIdX;

    SELECT
        @YControlType = QSurveyQuestion.SurveyQuestionType
    FROM
        surveys.QSurveyForm
        INNER JOIN surveys.QSurveyQuestion ON QSurveyQuestion.SurveyFormIdentifier = QSurveyForm.SurveyFormIdentifier
    where
        QSurveyQuestion.SurveyQuestionIdentifier = @QuestionIdY;

    if @XControlType = 'CheckList'
      BEGIN
        INSERT INTO @XValues VALUES ('00000000-0000-0000-0000-000000000001', 1, 'No');
        INSERT INTO @XValues VALUES ('00000000-0000-0000-0000-000000000002', 2, 'Yes');

        INSERT INTO @Answers
        SELECT
            QResponseSession.ResponseSessionIdentifier
           ,CASE WHEN QResponseOption.ResponseOptionIsSelected = 1
                THEN
                    '00000000-0000-0000-0000-000000000002'
                ELSE
                    '00000000-0000-0000-0000-000000000001'
            END
        FROM
            surveys.QResponseSession
            INNER JOIN surveys.QResponseOption ON QResponseOption.ResponseSessionIdentifier = QResponseSession.ResponseSessionIdentifier
            INNER JOIN surveys.QSurveyOptionItem ON QSurveyOptionItem.SurveyOptionItemIdentifier = QResponseOption.SurveyOptionIdentifier
            INNER JOIN surveys.QSurveyOptionList ON QSurveyOptionList.SurveyOptionListIdentifier = QSurveyOptionItem.SurveyOptionListIdentifier
        WHERE
            QSurveyOptionList.SurveyQuestionIdentifier = @QuestionIdX;
      END
    ELSE
      BEGIN
        INSERT INTO @XValues
        SELECT
            QSurveyOptionItem.SurveyOptionItemIdentifier
           ,QSurveyOptionItem.SurveyOptionItemSequence
           ,ISNULL(TContent.ContentText,'Option #' + CAST(QSurveyOptionItem.SurveyOptionItemSequence AS varchar))
        FROM
            surveys.QSurveyOptionItem
            INNER JOIN surveys.QSurveyOptionList ON QSurveyOptionList.SurveyOptionListIdentifier = QSurveyOptionItem.SurveyOptionListIdentifier
            LEFT JOIN contents.TContent ON TContent.ContainerIdentifier = QSurveyOptionItem.SurveyOptionListIdentifier
                                           AND TContent.ContentLabel = 'Title'
                                           AND TContent.ContentLanguage = @Language
        WHERE
            QSurveyOptionList.SurveyQuestionIdentifier = @QuestionIdX;

        INSERT INTO @Answers
        SELECT
            QResponseSession.ResponseSessionIdentifier
           ,QSurveyOptionItem.SurveyOptionItemIdentifier
        FROM
            surveys.QResponseSession
            INNER JOIN surveys.QResponseOption ON QResponseOption.ResponseSessionIdentifier = QResponseSession.ResponseSessionIdentifier
            INNER JOIN surveys.QSurveyOptionItem ON QSurveyOptionItem.SurveyOptionItemIdentifier = QResponseOption.SurveyOptionIdentifier
            INNER JOIN surveys.QSurveyOptionList ON QSurveyOptionList.SurveyOptionListIdentifier = QSurveyOptionItem.SurveyOptionListIdentifier
        WHERE
            QSurveyOptionList.SurveyQuestionIdentifier = @QuestionIdX
            AND QResponseOption.ResponseOptionIsSelected = 1;
    END

    INSERT INTO @XValues VALUES (NULL, NULL, 'Total');

    if @YControlType = 'CheckList'
      BEGIN
        INSERT INTO @YValues VALUES ('00000000-0000-0000-0000-000000000001', 1, 'No');
        INSERT INTO @YValues VALUES ('00000000-0000-0000-0000-000000000002', 2, 'Yes');

        INSERT INTO @Answers
        SELECT
            QResponseSession.ResponseSessionIdentifier
           ,CASE WHEN QResponseOption.ResponseOptionIsSelected = 1
                THEN
                    '00000000-0000-0000-0000-000000000002'
                ELSE
                    '00000000-0000-0000-0000-000000000001'
            END
        FROM
            surveys.QResponseSession
            INNER JOIN surveys.QResponseOption ON QResponseOption.ResponseSessionIdentifier = QResponseSession.ResponseSessionIdentifier
            INNER JOIN surveys.QSurveyOptionItem ON QSurveyOptionItem.SurveyOptionItemIdentifier = QResponseOption.SurveyOptionIdentifier
            INNER JOIN surveys.QSurveyOptionList ON QSurveyOptionList.SurveyOptionListIdentifier = QSurveyOptionItem.SurveyOptionListIdentifier
        WHERE
            QSurveyOptionList.SurveyQuestionIdentifier = @QuestionIdY;
      END
    ELSE
      BEGIN
        INSERT INTO @YValues
        SELECT
            QSurveyOptionItem.SurveyOptionItemIdentifier
           ,QSurveyOptionItem.SurveyOptionItemSequence
           ,ISNULL(TContent.ContentText,'Option #' + CAST(QSurveyOptionItem.SurveyOptionItemSequence AS varchar))
        FROM
            surveys.QSurveyOptionItem
            INNER JOIN surveys.QSurveyOptionList ON QSurveyOptionList.SurveyOptionListIdentifier = QSurveyOptionItem.SurveyOptionListIdentifier
            LEFT JOIN contents.TContent ON TContent.ContainerIdentifier = QSurveyOptionItem.SurveyOptionListIdentifier
                                           AND TContent.ContentLabel = 'Title'
                                           AND TContent.ContentLanguage = @Language
        WHERE
            QSurveyOptionList.SurveyQuestionIdentifier = @QuestionIdY;

        INSERT INTO @Answers
        SELECT
            QResponseSession.ResponseSessionIdentifier
           ,QSurveyOptionItem.SurveyOptionItemIdentifier
        FROM
            surveys.QResponseSession
            INNER JOIN surveys.QResponseOption ON QResponseOption.ResponseSessionIdentifier = QResponseSession.ResponseSessionIdentifier
            INNER JOIN surveys.QSurveyOptionItem ON QSurveyOptionItem.SurveyOptionItemIdentifier = QResponseOption.SurveyOptionIdentifier
            INNER JOIN surveys.QSurveyOptionList ON QSurveyOptionList.SurveyOptionListIdentifier = QSurveyOptionItem.SurveyOptionListIdentifier
        WHERE
            QSurveyOptionList.SurveyQuestionIdentifier = @QuestionIdY
            AND QResponseOption.ResponseOptionIsSelected = 1;
    END

    INSERT INTO @YValues VALUES (NULL, NULL, 'Total');

    SELECT
        x.SurveyOptionIdentifier AS XSurveyOptionIdentifier
       ,y.SurveyOptionIdentifier AS YSurveyOptionIdentifier
       ,x.Text                 AS XSurveyOptionTitle
       ,y.Text                 AS YSurveyOptionTitle
       ,(
            SELECT COUNT(*)
            FROM @Answers
            WHERE SurveyOptionIdentifier IN (SELECT yt.SurveyOptionIdentifier FROM @YValues AS yt)
        )                        AS YTotalValue
       ,(
            SELECT COUNT(*)
            FROM @Answers
            WHERE SurveyOptionIdentifier IN (SELECT xt.SurveyOptionIdentifier FROM @XValues AS xt)
        )                        AS XTotalValue
       ,(
            SELECT COUNT(*) FROM @Answers AS a WHERE a.SurveyOptionIdentifier = y.SurveyOptionIdentifier
        )                        AS YValue
       ,(
            SELECT COUNT(*) FROM @Answers AS a WHERE a.SurveyOptionIdentifier = x.SurveyOptionIdentifier
        )                        AS XValue
       ,(
            SELECT
                COUNT(*)
            FROM
                @Answers AS a
            WHERE
                SurveyOptionIdentifier = x.SurveyOptionIdentifier
                AND a.ResponseSessionIdentifier IN
                (
                    SELECT a1.ResponseSessionIdentifier
                    FROM @Answers AS a1
                    WHERE a1.SurveyOptionIdentifier = y.SurveyOptionIdentifier
                )
        )                        AS CrossValue
    FROM
        @XValues AS x
        CROSS JOIN @YValues AS y
    ORDER BY
        CASE WHEN x.Sequence IS NOT NULL
            THEN 0
            ELSE 1
        END
       ,x.Sequence
       ,CASE WHEN y.Sequence IS NOT NULL
            THEN 0
            ELSE 1
        END
       ,y.Sequence;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create trigger [contacts].[QMembership_AfterDelete] on [contacts].[QMembership] for delete as
begin

  insert into contacts.QMembershipDeletion
  (
      DeletionIdentifier
	, DeletionWhen
    , GroupIdentifier
    , MembershipIdentifier
    , OrganizationIdentifier
    , UserIdentifier
  )
  select 
      newid()
	, getutcdate()
    , GroupIdentifier
    , MembershipIdentifier
    , OrganizationIdentifier
    , UserIdentifier
  from Deleted

end;
GO
ALTER TABLE [contacts].[QMembership] ENABLE TRIGGER [QMembership_AfterDelete]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create trigger [contacts].[TriggerOnGroupPermissionChanged] on [contacts].[TGroupPermission] after insert, update as
begin
UPDATE contacts.TGroupPermission
SET PermissionMask = 
      (CASE WHEN Inserted.AllowExecute      = 1 THEN  1 ELSE 0 END)
    | (CASE WHEN Inserted.AllowRead         = 1 THEN  2 ELSE 0 END)
    | (CASE WHEN Inserted.AllowWrite        = 1 THEN  4 ELSE 0 END)
    | (CASE WHEN Inserted.AllowCreate       = 1 THEN  8 ELSE 0 END)
    | (CASE WHEN Inserted.AllowDelete       = 1 THEN 16 ELSE 0 END)
    | (CASE WHEN Inserted.AllowAdministrate = 1 THEN 32 ELSE 0 END)
    | (CASE WHEN Inserted.AllowConfigure    = 1 THEN 64 ELSE 0 END)
from Inserted
where Inserted.PermissionIdentifier = TGroupPermission.PermissionIdentifier;
end;
GO
ALTER TABLE [contacts].[TGroupPermission] ENABLE TRIGGER [TriggerOnGroupPermissionChanged]
GO
