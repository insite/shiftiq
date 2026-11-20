using System.Reflection;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class QPersonSecretAdapter : IEntityAdapter
{
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

    public IEnumerable<PersonSecretModel> ToModel(IEnumerable<QPersonSecretEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public PersonSecretModel ToModel(QPersonSecretEntity entity)
    {
        var model = new PersonSecretModel
        {
            PersonId = entity.PersonIdentifier,
            SecretId = entity.SecretIdentifier,
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
            SecretId = entity.SecretIdentifier

        };

        return match;
    }
}
