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
        entity.AdministratorUserIdentifier = modify.AdministratorUserId;
        entity.GlossaryIdentifier = modify.GlossaryId;
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
            AdministratorUserIdentifier = create.AdministratorUserId,
            GlossaryIdentifier = create.GlossaryId,
            OrganizationIdentifier = create.OrganizationId,
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
            AdministratorUserId = entity.AdministratorUserIdentifier,
            GlossaryId = entity.GlossaryIdentifier,
            OrganizationId = entity.OrganizationIdentifier,
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
            OrganizationId = entity.OrganizationIdentifier,
            CompanyName = entity.CompanyName
        };

        return match;
    }

    public DomainOrganizationModel ToData(OrganizationModel model)
    {
        return JsonConvert.DeserializeObject<DomainOrganizationModel>(model.OrganizationData)!;
    }
}