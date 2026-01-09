using Newtonsoft.Json;

using Shift.Common;
using Shift.Contract;

using DomainOrganizationModel = InSite.Domain.Organizations.OrganizationState;

namespace Shift.Service.Security;

public class OrganizationAdapter : IEntityAdapter
{
    public void Copy(ModifyOrganization modify, OrganizationEntity entity)
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
        entity.PersonFullNamePolicy = modify.PersonFullNamePolicy;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public OrganizationEntity ToEntity(CreateOrganization create)
    {
        var entity = new OrganizationEntity
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
            OrganizationData = create.OrganizationData,
            PersonFullNamePolicy = create.PersonFullNamePolicy
        };
        return entity;
    }

    public IEnumerable<OrganizationModel> ToModel(IEnumerable<OrganizationEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public OrganizationModel ToModel(OrganizationEntity entity)
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
            OrganizationData = entity.OrganizationData,
            PersonFullNamePolicy = entity.PersonFullNamePolicy
        };

        return model;
    }

    public IEnumerable<OrganizationMatch> ToMatch(IEnumerable<OrganizationEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public OrganizationMatch ToMatch(OrganizationEntity entity)
    {
        var match = new OrganizationMatch
        {
            OrganizationIdentifier = entity.OrganizationIdentifier,
            ParentOrganizationIdentifier = entity.ParentOrganizationIdentifier,
            CompanyName = entity.CompanyName
        };

        return match;
    }

    public DomainOrganizationModel ToData(OrganizationModel model)
    {
        return JsonConvert.DeserializeObject<DomainOrganizationModel>(model.OrganizationData)!;
    }
}