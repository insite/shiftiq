using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class MembershipAdapter : IEntityAdapter
{
    public void Copy(ModifyMembership modify, MembershipEntity entity)
    {
        entity.MembershipEffective = modify.MembershipEffective;
        entity.MembershipFunction = modify.MembershipFunction;
        entity.GroupIdentifier = modify.GroupIdentifier;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.UserIdentifier = modify.UserIdentifier;
        entity.MembershipExpiry = modify.MembershipExpiry;
        entity.Modified = modify.Modified;
        entity.ModifiedBy = modify.ModifiedBy;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public MembershipEntity ToEntity(CreateMembership create)
    {
        var entity = new MembershipEntity
        {
            MembershipIdentifier = create.MembershipIdentifier,
            MembershipEffective = create.MembershipEffective,
            MembershipFunction = create.MembershipFunction,
            GroupIdentifier = create.GroupIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            UserIdentifier = create.UserIdentifier,
            MembershipExpiry = create.MembershipExpiry,
            Modified = create.Modified,
            ModifiedBy = create.ModifiedBy
        };
        return entity;
    }

    public IEnumerable<MembershipModel> ToModel(IEnumerable<MembershipEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public MembershipModel ToModel(MembershipEntity entity)
    {
        var model = new MembershipModel
        {
            MembershipIdentifier = entity.MembershipIdentifier,
            MembershipEffective = entity.MembershipEffective,
            MembershipFunction = entity.MembershipFunction,
            GroupIdentifier = entity.GroupIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            UserIdentifier = entity.UserIdentifier,
            MembershipExpiry = entity.MembershipExpiry,
            Modified = entity.Modified,
            ModifiedBy = entity.ModifiedBy
        };

        return model;
    }

    public IEnumerable<MembershipMatch> ToMatch(IEnumerable<MembershipEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public MembershipMatch ToMatch(MembershipEntity entity)
    {
        var match = new MembershipMatch
        {
            MembershipIdentifier = entity.MembershipIdentifier

        };

        return match;
    }
}