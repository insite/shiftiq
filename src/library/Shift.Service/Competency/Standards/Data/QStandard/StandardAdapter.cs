using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Competency;

public class StandardAdapter : IEntityAdapter
{
    public void Copy(ModifyStandard modify, StandardEntity entity)
    {
        entity.AssetNumber = modify.AssetNumber;
        entity.AuthorDate = modify.AuthorDate;
        entity.AuthorName = modify.AuthorName;
        entity.BankIdentifier = modify.BankIdentifier;
        entity.BankSetIdentifier = modify.BankSetIdentifier;
        entity.CalculationArgument = modify.CalculationArgument;
        entity.CanvasIdentifier = modify.CanvasIdentifier;
        entity.CategoryItemIdentifier = modify.CategoryItemIdentifier;
        entity.CertificationHoursPercentCore = modify.CertificationHoursPercentCore;
        entity.CertificationHoursPercentNonCore = modify.CertificationHoursPercentNonCore;
        entity.Code = modify.Code;
        entity.CompetencyScoreCalculationMethod = modify.CompetencyScoreCalculationMethod;
        entity.CompetencyScoreSummarizationMethod = modify.CompetencyScoreSummarizationMethod;
        entity.ContentDescription = modify.ContentDescription;
        entity.ContentName = modify.ContentName;
        entity.ContentSettings = modify.ContentSettings;
        entity.ContentSummary = modify.ContentSummary;
        entity.ContentTitle = modify.ContentTitle;
        entity.Created = modify.Created;
        entity.CreatedBy = modify.CreatedBy;
        entity.CreditHours = modify.CreditHours;
        entity.CreditIdentifier = modify.CreditIdentifier;
        entity.DatePosted = modify.DatePosted;
        entity.DepartmentGroupIdentifier = modify.DepartmentGroupIdentifier;
        entity.DocumentType = modify.DocumentType;
        entity.Icon = modify.Icon;
        entity.IndustryItemIdentifier = modify.IndustryItemIdentifier;
        entity.IsCertificateEnabled = modify.IsCertificateEnabled;
        entity.IsFeedbackEnabled = modify.IsFeedbackEnabled;
        entity.IsHidden = modify.IsHidden;
        entity.IsLocked = modify.IsLocked;
        entity.IsPractical = modify.IsPractical;
        entity.IsPublished = modify.IsPublished;
        entity.IsTemplate = modify.IsTemplate;
        entity.IsTheory = modify.IsTheory;
        entity.Language = modify.Language;
        entity.LevelCode = modify.LevelCode;
        entity.LevelType = modify.LevelType;
        entity.MajorVersion = modify.MajorVersion;
        entity.MasteryPoints = modify.MasteryPoints;
        entity.MinorVersion = modify.MinorVersion;
        entity.Modified = modify.Modified;
        entity.ModifiedBy = modify.ModifiedBy;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.ParentStandardIdentifier = modify.ParentStandardIdentifier;
        entity.PointsPossible = modify.PointsPossible;
        entity.Sequence = modify.Sequence;
        entity.SourceDescriptor = modify.SourceDescriptor;
        entity.StandardAlias = modify.StandardAlias;
        entity.StandardLabel = modify.StandardLabel;
        entity.StandardPrivacyScope = modify.StandardPrivacyScope;
        entity.StandardStatus = modify.StandardStatus;
        entity.StandardStatusLastUpdateTime = modify.StandardStatusLastUpdateTime;
        entity.StandardStatusLastUpdateUser = modify.StandardStatusLastUpdateUser;
        entity.StandardTier = modify.StandardTier;
        entity.StandardType = modify.StandardType;
        entity.Tags = modify.Tags;
        entity.UtcPublished = modify.UtcPublished;
        entity.StandardHook = modify.StandardHook;
        entity.PassingScore = modify.PassingScore;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public StandardEntity ToEntity(CreateStandard create)
    {
        var entity = new StandardEntity
        {
            AssetNumber = create.AssetNumber,
            AuthorDate = create.AuthorDate,
            AuthorName = create.AuthorName,
            BankIdentifier = create.BankIdentifier,
            BankSetIdentifier = create.BankSetIdentifier,
            CalculationArgument = create.CalculationArgument,
            CanvasIdentifier = create.CanvasIdentifier,
            CategoryItemIdentifier = create.CategoryItemIdentifier,
            CertificationHoursPercentCore = create.CertificationHoursPercentCore,
            CertificationHoursPercentNonCore = create.CertificationHoursPercentNonCore,
            Code = create.Code,
            CompetencyScoreCalculationMethod = create.CompetencyScoreCalculationMethod,
            CompetencyScoreSummarizationMethod = create.CompetencyScoreSummarizationMethod,
            ContentDescription = create.ContentDescription,
            ContentName = create.ContentName,
            ContentSettings = create.ContentSettings,
            ContentSummary = create.ContentSummary,
            ContentTitle = create.ContentTitle,
            Created = create.Created,
            CreatedBy = create.CreatedBy,
            CreditHours = create.CreditHours,
            CreditIdentifier = create.CreditIdentifier,
            DatePosted = create.DatePosted,
            DepartmentGroupIdentifier = create.DepartmentGroupIdentifier,
            DocumentType = create.DocumentType,
            Icon = create.Icon,
            IndustryItemIdentifier = create.IndustryItemIdentifier,
            IsCertificateEnabled = create.IsCertificateEnabled,
            IsFeedbackEnabled = create.IsFeedbackEnabled,
            IsHidden = create.IsHidden,
            IsLocked = create.IsLocked,
            IsPractical = create.IsPractical,
            IsPublished = create.IsPublished,
            IsTemplate = create.IsTemplate,
            IsTheory = create.IsTheory,
            Language = create.Language,
            LevelCode = create.LevelCode,
            LevelType = create.LevelType,
            MajorVersion = create.MajorVersion,
            MasteryPoints = create.MasteryPoints,
            MinorVersion = create.MinorVersion,
            Modified = create.Modified,
            ModifiedBy = create.ModifiedBy,
            OrganizationIdentifier = create.OrganizationIdentifier,
            ParentStandardIdentifier = create.ParentStandardIdentifier,
            PointsPossible = create.PointsPossible,
            Sequence = create.Sequence,
            SourceDescriptor = create.SourceDescriptor,
            StandardAlias = create.StandardAlias,
            StandardIdentifier = create.StandardIdentifier,
            StandardLabel = create.StandardLabel,
            StandardPrivacyScope = create.StandardPrivacyScope,
            StandardStatus = create.StandardStatus,
            StandardStatusLastUpdateTime = create.StandardStatusLastUpdateTime,
            StandardStatusLastUpdateUser = create.StandardStatusLastUpdateUser,
            StandardTier = create.StandardTier,
            StandardType = create.StandardType,
            Tags = create.Tags,
            UtcPublished = create.UtcPublished,
            StandardHook = create.StandardHook,
            PassingScore = create.PassingScore
        };
        return entity;
    }

    public IEnumerable<StandardModel> ToModel(IEnumerable<StandardEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public StandardModel ToModel(StandardEntity entity)
    {
        var model = new StandardModel
        {
            AssetNumber = entity.AssetNumber,
            AuthorDate = entity.AuthorDate,
            AuthorName = entity.AuthorName,
            BankId = entity.BankIdentifier,
            BankSetId = entity.BankSetIdentifier,
            CalculationArgument = entity.CalculationArgument,
            CanvasIdentifier = entity.CanvasIdentifier,
            CategoryItemId = entity.CategoryItemIdentifier,
            CertificationHoursPercentCore = entity.CertificationHoursPercentCore,
            CertificationHoursPercentNonCore = entity.CertificationHoursPercentNonCore,
            Code = entity.Code,
            CompetencyScoreCalculationMethod = entity.CompetencyScoreCalculationMethod,
            CompetencyScoreSummarizationMethod = entity.CompetencyScoreSummarizationMethod,
            ContentDescription = entity.ContentDescription,
            ContentName = entity.ContentName,
            ContentSettings = entity.ContentSettings,
            ContentSummary = entity.ContentSummary,
            ContentTitle = entity.ContentTitle,
            Created = entity.Created,
            CreatedBy = entity.CreatedBy,
            CreditHours = entity.CreditHours,
            CreditIdentifier = entity.CreditIdentifier,
            DatePosted = entity.DatePosted,
            DepartmentGroupId = entity.DepartmentGroupIdentifier,
            DocumentType = entity.DocumentType,
            Icon = entity.Icon,
            IndustryItemId = entity.IndustryItemIdentifier,
            IsCertificateEnabled = entity.IsCertificateEnabled,
            IsFeedbackEnabled = entity.IsFeedbackEnabled,
            IsHidden = entity.IsHidden,
            IsLocked = entity.IsLocked,
            IsPractical = entity.IsPractical,
            IsPublished = entity.IsPublished,
            IsTemplate = entity.IsTemplate,
            IsTheory = entity.IsTheory,
            Language = entity.Language,
            LevelCode = entity.LevelCode,
            LevelType = entity.LevelType,
            MajorVersion = entity.MajorVersion,
            MasteryPoints = entity.MasteryPoints,
            MinorVersion = entity.MinorVersion,
            Modified = entity.Modified,
            ModifiedBy = entity.ModifiedBy,
            OrganizationId = entity.OrganizationIdentifier,
            ParentStandardId = entity.ParentStandardIdentifier,
            PointsPossible = entity.PointsPossible,
            Sequence = entity.Sequence,
            SourceDescriptor = entity.SourceDescriptor,
            StandardAlias = entity.StandardAlias,
            StandardId = entity.StandardIdentifier,
            StandardLabel = entity.StandardLabel,
            StandardPrivacyScope = entity.StandardPrivacyScope,
            StandardStatus = entity.StandardStatus,
            StandardStatusLastUpdateTime = entity.StandardStatusLastUpdateTime,
            StandardStatusLastUpdateUser = entity.StandardStatusLastUpdateUser,
            StandardTier = entity.StandardTier,
            StandardType = entity.StandardType,
            Tags = entity.Tags,
            UtcPublished = entity.UtcPublished,
            StandardHook = entity.StandardHook,
            PassingScore = entity.PassingScore
        };

        return model;
    }

    public IEnumerable<StandardMatch> ToMatch(IEnumerable<StandardEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public StandardMatch ToMatch(StandardEntity entity)
    {
        var match = new StandardMatch
        {
            Code = entity.Code,
            Id = entity.StandardIdentifier,
            Name = entity.ContentName,
            Title = entity.ContentTitle,
            Type = entity.StandardType
        };

        return match;
    }
}