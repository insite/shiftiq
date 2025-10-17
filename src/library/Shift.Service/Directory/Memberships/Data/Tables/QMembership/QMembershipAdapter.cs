using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class QMembershipAdapter : IEntityAdapter
{
    public void Copy(ModifyMembership modify, QMembershipEntity entity)
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

    public string Serialize(IEnumerable<MembershipModel> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public QMembershipEntity ToEntity(CreateMembership create)
    {
        var entity = new QMembershipEntity
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

    public IEnumerable<MembershipModel> ToModel(IEnumerable<QMembershipEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public MembershipModel ToModel(QMembershipEntity entity)
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

    public IEnumerable<MembershipMatch> ToMatch(IEnumerable<QMembershipEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public MembershipMatch ToMatch(QMembershipEntity entity)
    {
        var match = new MembershipMatch
        {
            MembershipIdentifier = entity.MembershipIdentifier

        };

        return match;
    }
}