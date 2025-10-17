namespace Shift.Service.Security;

using Shift.Common;
using Shift.Contract;

using DomainOrganizationModel = InSite.Domain.Organizations.OrganizationState;

public class QOrganizationAdapter : IEntityAdapter
{
    private readonly IJsonSerializerBase _jsonSerializer;

    public QOrganizationAdapter(IJsonSerializerBase jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }

    public void Copy(ModifyOrganization modify, QOrganizationEntity entity)
    {
        entity.AccountClosed = modify.AccountClosed;
        entity.AccountOpened = modify.AccountOpened;
        entity.AccountStatus = modify.AccountStatus;
        entity.CompanyDomain = modify.CompanyDomain;
        entity.CompanyName = modify.CompanyName;
        entity.CompanySize = modify.CompanySize;
        entity.CompanySummary = modify.CompanySummary;
        entity.CompanyTitle = modify.CompanyTitle;
        entity.CompanyWebSiteUrl = modify.CompanyWebSiteUrl;
        entity.CompetencyAutoExpirationMode = modify.CompetencyAutoExpirationMode;
        entity.CompetencyAutoExpirationMonth = modify.CompetencyAutoExpirationMonth;
        entity.CompetencyAutoExpirationDay = modify.CompetencyAutoExpirationDay;
        entity.StandardContentLabels = modify.StandardContentLabels;
        entity.OrganizationCode = modify.OrganizationCode;
        entity.OrganizationLogoUrl = modify.OrganizationLogoUrl;
        entity.TimeZone = modify.TimeZone;
        entity.AdministratorUserIdentifier = modify.AdministratorUserIdentifier;
        entity.GlossaryIdentifier = modify.GlossaryIdentifier;
        entity.ParentOrganizationIdentifier = modify.ParentOrganizationIdentifier;
        entity.OrganizationData = modify.OrganizationData;
    }

    public QOrganizationEntity ToEntity(CreateOrganization create)
    {
        var entity = new QOrganizationEntity
        {
            AccountClosed = create.AccountClosed,
            AccountOpened = create.AccountOpened,
            AccountStatus = create.AccountStatus,
            CompanyDomain = create.CompanyDomain,
            CompanyName = create.CompanyName,
            CompanySize = create.CompanySize,
            CompanySummary = create.CompanySummary,
            CompanyTitle = create.CompanyTitle,
            CompanyWebSiteUrl = create.CompanyWebSiteUrl,
            CompetencyAutoExpirationMode = create.CompetencyAutoExpirationMode,
            CompetencyAutoExpirationMonth = create.CompetencyAutoExpirationMonth,
            CompetencyAutoExpirationDay = create.CompetencyAutoExpirationDay,
            StandardContentLabels = create.StandardContentLabels,
            OrganizationCode = create.OrganizationCode,
            OrganizationLogoUrl = create.OrganizationLogoUrl,
            TimeZone = create.TimeZone,
            AdministratorUserIdentifier = create.AdministratorUserIdentifier,
            GlossaryIdentifier = create.GlossaryIdentifier,
            ParentOrganizationIdentifier = create.ParentOrganizationIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            OrganizationData = create.OrganizationData
        };
        return entity;
    }

    public IEnumerable<OrganizationModel> ToModel(IEnumerable<QOrganizationEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public OrganizationModel ToModel(QOrganizationEntity entity)
    {
        var model = new OrganizationModel
        {
            AccountClosed = entity.AccountClosed,
            AccountOpened = entity.AccountOpened,
            AccountStatus = entity.AccountStatus,
            CompanyDomain = entity.CompanyDomain,
            CompanyName = entity.CompanyName,
            CompanySize = entity.CompanySize,
            CompanySummary = entity.CompanySummary,
            CompanyTitle = entity.CompanyTitle,
            CompanyWebSiteUrl = entity.CompanyWebSiteUrl,
            CompetencyAutoExpirationMode = entity.CompetencyAutoExpirationMode,
            CompetencyAutoExpirationMonth = entity.CompetencyAutoExpirationMonth,
            CompetencyAutoExpirationDay = entity.CompetencyAutoExpirationDay,
            StandardContentLabels = entity.StandardContentLabels,
            OrganizationCode = entity.OrganizationCode,
            OrganizationLogoUrl = entity.OrganizationLogoUrl,
            TimeZone = entity.TimeZone,
            AdministratorUserIdentifier = entity.AdministratorUserIdentifier,
            GlossaryIdentifier = entity.GlossaryIdentifier,
            ParentOrganizationIdentifier = entity.ParentOrganizationIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            OrganizationData = entity.OrganizationData
        };

        return model;
    }

    public DomainOrganizationModel ToData(OrganizationModel model)
    {
        return _jsonSerializer.Deserialize<DomainOrganizationModel>(model.OrganizationData);
    }
}