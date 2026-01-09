using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class RegistrationInstructorAdapter : IEntityAdapter
{
    public void Copy(ModifyRegistrationInstructor modify, RegistrationInstructorEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public RegistrationInstructorEntity ToEntity(CreateRegistrationInstructor create)
    {
        var entity = new RegistrationInstructorEntity
        {
            RegistrationIdentifier = create.RegistrationIdentifier,
            InstructorIdentifier = create.InstructorIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<RegistrationInstructorModel> ToModel(IEnumerable<RegistrationInstructorEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public RegistrationInstructorModel ToModel(RegistrationInstructorEntity entity)
    {
        var model = new RegistrationInstructorModel
        {
            RegistrationIdentifier = entity.RegistrationIdentifier,
            InstructorIdentifier = entity.InstructorIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<RegistrationInstructorMatch> ToMatch(IEnumerable<RegistrationInstructorEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public RegistrationInstructorMatch ToMatch(RegistrationInstructorEntity entity)
    {
        var match = new RegistrationInstructorMatch
        {
            InstructorIdentifier = entity.InstructorIdentifier,
            RegistrationIdentifier = entity.RegistrationIdentifier

        };

        return match;
    }
}