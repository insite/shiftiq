using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class PendingPersonAdapter : IEntityAdapter
{
    public void Copy(ModifyPendingPerson modify, PendingPersonEntity entity)
    {
        entity.UserFirstName = modify.UserFirstName;
        entity.UserLastName = modify.UserLastName;
        entity.UserEmail = modify.UserEmail;
        entity.PersonCode = modify.PersonCode;
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public PendingPersonEntity ToEntity(CreatePendingPerson create)
    {
        var entity = new PendingPersonEntity
        {
            PendingPersonIdentifier = UniqueIdentifier.Create(),
            UserFirstName = create.UserFirstName,
            UserLastName = create.UserLastName,
            UserEmail = create.UserEmail,
            PersonCode = create.PersonCode,
            EmployeeStatus = create.EmployeeStatus
        };
        return entity;
    }

    public PendingPersonModel[] ToModel(IEnumerable<PendingPersonEntity> entities)
    {
        return entities.Select(e => ToModel(e)).ToArray();
    }

    public PendingPersonModel ToModel(PendingPersonEntity entity)
    {
        var model = new PendingPersonModel
        {
            PendingPersonIdentifier = entity.PendingPersonIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            GroupIdentifier = entity.GroupIdentifier,
            SubmittedBy = entity.SubmittedBy,
            PersonCode = entity.PersonCode,
            UserEmail = entity.UserEmail,
            UserFirstName = entity.UserFirstName,
            UserLastName = entity.UserLastName,
            UserMiddleName = entity.UserMiddleName,
            JobDivision = entity.JobDivision,
            JobTitle = entity.JobTitle,
            WorkAddressStreet1 = entity.WorkAddressStreet1,
            WorkAddressStreet2 = entity.WorkAddressStreet2,
            WorkAddressCity = entity.WorkAddressCity,
            WorkAddressProvince = entity.WorkAddressProvince,
            WorkAddressPostalCode = entity.WorkAddressPostalCode,
            EmployeeStatus = entity.EmployeeStatus,
            SubmittedAt = entity.SubmittedAt,
        };

        return model;
    }
}
