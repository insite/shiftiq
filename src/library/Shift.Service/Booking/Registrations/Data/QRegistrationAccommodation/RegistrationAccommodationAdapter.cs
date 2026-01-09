using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class RegistrationAccommodationAdapter : IEntityAdapter
{
    public void Copy(ModifyRegistrationAccommodation modify, RegistrationAccommodationEntity entity)
    {
        entity.RegistrationIdentifier = modify.RegistrationIdentifier;
        entity.AccommodationType = modify.AccommodationType;
        entity.AccommodationName = modify.AccommodationName;
        entity.TimeExtension = modify.TimeExtension;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public RegistrationAccommodationEntity ToEntity(CreateRegistrationAccommodation create)
    {
        var entity = new RegistrationAccommodationEntity
        {
            AccommodationIdentifier = create.AccommodationIdentifier,
            RegistrationIdentifier = create.RegistrationIdentifier,
            AccommodationType = create.AccommodationType,
            AccommodationName = create.AccommodationName,
            TimeExtension = create.TimeExtension,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<RegistrationAccommodationModel> ToModel(IEnumerable<RegistrationAccommodationEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public RegistrationAccommodationModel ToModel(RegistrationAccommodationEntity entity)
    {
        var model = new RegistrationAccommodationModel
        {
            AccommodationIdentifier = entity.AccommodationIdentifier,
            RegistrationIdentifier = entity.RegistrationIdentifier,
            AccommodationType = entity.AccommodationType,
            AccommodationName = entity.AccommodationName,
            TimeExtension = entity.TimeExtension,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<RegistrationAccommodationMatch> ToMatch(IEnumerable<RegistrationAccommodationEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public RegistrationAccommodationMatch ToMatch(RegistrationAccommodationEntity entity)
    {
        var match = new RegistrationAccommodationMatch
        {
            AccommodationIdentifier = entity.AccommodationIdentifier

        };

        return match;
    }
}