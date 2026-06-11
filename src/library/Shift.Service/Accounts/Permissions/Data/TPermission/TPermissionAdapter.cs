using System.Reflection;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class TPermissionAdapter : IEntityAdapter
{
    public void Copy(ModifyPermission modify, TPermissionEntity entity)
    {
        entity.AllowRead = modify.AllowRead;
        entity.AllowWrite = modify.AllowWrite;
        entity.AllowCreate = modify.AllowCreate;
        entity.AllowDelete = modify.AllowDelete;
        entity.AllowAdministrate = modify.AllowAdministrate;
        entity.AllowConfigure = modify.AllowConfigure;
        entity.PermissionGranted = modify.PermissionGranted;
        entity.PermissionGrantedBy = modify.PermissionGrantedBy;
        entity.ObjectIdentifier = modify.ObjectId;
        entity.ObjectType = modify.ObjectType;
        entity.GroupIdentifier = modify.GroupId;
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.AllowTrialAccess = modify.AllowTrialAccess;

    }

    public string Serialize(IEnumerable<PermissionModel> models, string format)
    {
        var content = string.Empty;

        if (format.ToLower() == "csv")
        {
            var csv = new CsvExportHelper(models);

            var properties = typeof(PermissionModel)
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

    public TPermissionEntity ToEntity(CreatePermission create)
    {
        var entity = new TPermissionEntity
        {
            AllowRead = create.AllowRead,
            AllowWrite = create.AllowWrite,
            AllowCreate = create.AllowCreate,
            AllowDelete = create.AllowDelete,
            AllowAdministrate = create.AllowAdministrate,
            AllowConfigure = create.AllowConfigure,
            PermissionGranted = create.PermissionGranted,
            PermissionGrantedBy = create.PermissionGrantedBy,
            ObjectIdentifier = create.ObjectId,
            ObjectType = create.ObjectType,
            GroupIdentifier = create.GroupId,
            OrganizationIdentifier = create.OrganizationId,
            PermissionIdentifier = create.PermissionId,
            AllowTrialAccess = create.AllowTrialAccess
        };
        return entity;
    }

    public IEnumerable<PermissionModel> ToModel(IEnumerable<TPermissionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public PermissionModel ToModel(TPermissionEntity entity)
    {
        var model = new PermissionModel
        {
            AllowRead = entity.AllowRead,
            AllowWrite = entity.AllowWrite,
            AllowCreate = entity.AllowCreate,
            AllowDelete = entity.AllowDelete,
            AllowAdministrate = entity.AllowAdministrate,
            AllowConfigure = entity.AllowConfigure,
            PermissionGranted = entity.PermissionGranted,
            PermissionGrantedBy = entity.PermissionGrantedBy,
            ObjectId = entity.ObjectIdentifier,
            ObjectType = entity.ObjectType,
            GroupId = entity.GroupIdentifier,
            OrganizationId = entity.OrganizationIdentifier,
            PermissionId = entity.PermissionIdentifier,
            AllowTrialAccess = entity.AllowTrialAccess,

            GroupName = entity.Group?.GroupName,
            OrganizationCode = entity.Organization?.OrganizationCode
        };

        return model;
    }

    public IEnumerable<PermissionMatch> ToMatch(IEnumerable<TPermissionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public PermissionMatch ToMatch(TPermissionEntity entity)
    {
        var match = new PermissionMatch
        {
            PermissionId = entity.PermissionIdentifier

        };

        return match;
    }
}