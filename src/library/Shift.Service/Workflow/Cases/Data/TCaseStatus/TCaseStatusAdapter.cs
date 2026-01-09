using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Cases;

public class TCaseStatusAdapter : IEntityAdapter
{
    public CaseStatusModel ToModel(TCaseStatusEntity entity)
    {
        return new CaseStatusModel
        {
            OrganizationIdentifier = entity.OrganizationIdentifier,
            CaseType = entity.CaseType,
            StatusIdentifier = entity.StatusIdentifier,
            StatusName = entity.StatusName,
            StatusSequence = entity.StatusSequence,
            StatusCategory = entity.StatusCategory,
            ReportCategory = entity.ReportCategory,
            StatusDescription = entity.StatusDescription
        };
    }

    public IEnumerable<CaseStatusModel> ToModel(IEnumerable<TCaseStatusEntity> entities) => entities.Select(ToModel);

    public string Serialize(IEnumerable<CaseStatusModel> models, string format, string includes)
    {
        return format.ToLower() switch
        {
            "json" => JsonHelper.SerializeJson(models, includes),
            "csv" => CsvHelper.SerializeCsv(models, includes),
            _ => string.Empty
        };
    }

    public TCaseStatusEntity ToEntity(CreateCaseStatus command)
    {
        return new TCaseStatusEntity
        {
            CaseType = command.CaseType,
            StatusName = command.StatusName,
            StatusSequence = command.StatusSequence,
            StatusCategory = command.StatusCategory,
            ReportCategory = command.ReportCategory,
            StatusDescription = command.StatusDescription
        };
    }

    public void Copy(ModifyCaseStatus command, TCaseStatusEntity entity)
    {
        entity.StatusName = command.StatusName;
        entity.StatusSequence = command.StatusSequence;
        entity.StatusCategory = command.StatusCategory;
        entity.ReportCategory = command.ReportCategory;
        entity.StatusDescription = command.StatusDescription;
    }
}

