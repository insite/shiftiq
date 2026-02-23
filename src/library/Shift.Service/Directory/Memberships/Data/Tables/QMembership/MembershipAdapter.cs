using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class MembershipAdapter : IEntityAdapter
{
    public void Copy(ModifyMembership modify, MembershipEntity entity)
    {
        entity.MembershipEffective = modify.MembershipEffective;
        entity.MembershipFunction = modify.MembershipFunction;
        entity.GroupIdentifier = modify.GroupId;
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.UserIdentifier = modify.UserId;
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
            MembershipIdentifier = create.MembershipId,
            MembershipEffective = create.MembershipEffective,
            MembershipFunction = create.MembershipFunction,
            GroupIdentifier = create.GroupId,
            OrganizationIdentifier = create.OrganizationId,
            UserIdentifier = create.UserId,
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
            MembershipId = entity.MembershipIdentifier,
            MembershipEffective = entity.MembershipEffective,
            MembershipFunction = entity.MembershipFunction,
            GroupId = entity.GroupIdentifier,
            OrganizationId = entity.OrganizationIdentifier,
            UserId = entity.UserIdentifier,
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
            MembershipId = entity.MembershipIdentifier

        };

        return match;
    }
}