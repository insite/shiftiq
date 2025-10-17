using System.Reflection;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class QPersonSecretAdapter : IEntityAdapter
{
    public void Copy(ModifyPersonSecret modify, QPersonSecretEntity entity)
    {
        entity.PersonIdentifier = modify.PersonIdentifier;
        entity.SecretType = modify.SecretType;
        entity.SecretName = modify.SecretName;
        entity.SecretExpiry = modify.SecretExpiry;
        entity.SecretLifetimeLimit = modify.SecretLifetimeLimit;
        entity.SecretValue = modify.SecretValue;

    }

    public string Serialize(IEnumerable<PersonSecretModel> models, string format)
    {
        var content = string.Empty;

        if (format.ToLower() == "csv")
        {
            var csv = new CsvExportHelper(models);

            var properties = typeof(PersonSecretModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name);

            foreach (var property in properties)
                csv.AddMapping(property, property);

            content = csv.GetString();
        }
        else // The default export file format is JSON.
        {
            content = JsonConvert.SerializeObject(models, Formatting.Indented);
        }

        return content;
    }

    public QPersonSecretEntity ToEntity(CreatePersonSecret create)
    {
        var entity = new QPersonSecretEntity
        {
            PersonIdentifier = create.PersonIdentifier,
            SecretIdentifier = create.SecretIdentifier,
            SecretType = create.SecretType,
            SecretName = create.SecretName,
            SecretExpiry = create.SecretExpiry,
            SecretLifetimeLimit = create.SecretLifetimeLimit,
            SecretValue = create.SecretValue
        };
        return entity;
    }

    public IEnumerable<PersonSecretModel> ToModel(IEnumerable<QPersonSecretEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public PersonSecretModel ToModel(QPersonSecretEntity entity)
    {
        var model = new PersonSecretModel
        {
            PersonIdentifier = entity.PersonIdentifier,
            SecretIdentifier = entity.SecretIdentifier,
            SecretType = entity.SecretType,
            SecretName = entity.SecretName,
            SecretExpiry = entity.SecretExpiry,
            SecretLifetimeLimit = entity.SecretLifetimeLimit,
            SecretValue = entity.SecretValue
        };

        return model;
    }

    public IEnumerable<PersonSecretMatch> ToMatch(IEnumerable<QPersonSecretEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public PersonSecretMatch ToMatch(QPersonSecretEntity entity)
    {
        var match = new PersonSecretMatch
        {
            SecretIdentifier = entity.SecretIdentifier

        };

        return match;
    }
}