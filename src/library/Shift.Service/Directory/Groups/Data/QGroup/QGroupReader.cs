using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Directory;

public class QGroupReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QGroupReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(
        Guid group,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QGroup
            .AnyAsync(x => x.GroupIdentifier == group, cancellation);
    }

    public async Task<QGroupEntity?> RetrieveAsync(
        Guid group,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QGroup
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GroupIdentifier == group, cancellation);
    }

    public async Task<int> CountAsync(
        IGroupCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QGroupEntity>> CollectAsync(
        IGroupCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<QGroupEntity>> DownloadAsync(
        IGroupCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<Guid[]> SearchUserRolesAsync(Guid? parentOrganizationId, Guid organizationId, Guid userId)
    {
        using var db = _context.CreateDbContext();

        return await db.QMembership.Where(m => m.UserIdentifier == userId)
            .Join(
                db.QGroup
                    .Where(g =>
                        (g.OrganizationIdentifier == organizationId || (parentOrganizationId != null && g.OrganizationIdentifier == parentOrganizationId))
                        && g.GroupType == "Role"
                    ),
                m => m.GroupIdentifier,
                g => g.GroupIdentifier,
                (m, g) => m.GroupIdentifier
            )
            .ToArrayAsync();
    }

    public async Task<IEnumerable<GroupMatch>> SearchAsync(
        IGroupCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QGroupEntity> BuildQueryable(
        TableDbContext db,
        IGroupCriteria criteria)
    {
        var q = db.QGroup
            .AsNoTracking()
            .AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // TODO: Implement search criteria

        // if (criteria.GroupIdentifier != null)
        //    query = query.Where(x => x.GroupIdentifier == criteria.GroupIdentifier);

        // if (criteria.OrganizationIdentifier != null)
        //    query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        // if (criteria.ParentGroupIdentifier != null)
        //    query = query.Where(x => x.ParentGroupIdentifier == criteria.ParentGroupIdentifier);

        // if (criteria.SurveyFormIdentifier != null)
        //    query = query.Where(x => x.SurveyFormIdentifier == criteria.SurveyFormIdentifier);

        // if (criteria.MessageToUserWhenMembershipStarted != null)
        //    query = query.Where(x => x.MessageToUserWhenMembershipStarted == criteria.MessageToUserWhenMembershipStarted);

        // if (criteria.MessageToAdminWhenMembershipStarted != null)
        //    query = query.Where(x => x.MessageToAdminWhenMembershipStarted == criteria.MessageToAdminWhenMembershipStarted);

        // if (criteria.MessageToAdminWhenEventVenueChanged != null)
        //    query = query.Where(x => x.MessageToAdminWhenEventVenueChanged == criteria.MessageToAdminWhenEventVenueChanged);

        // if (criteria.AddNewUsersAutomatically != null)
        //    query = query.Where(x => x.AddNewUsersAutomatically == criteria.AddNewUsersAutomatically);

        // if (criteria.AllowSelfSubscription != null)
        //    query = query.Where(x => x.AllowSelfSubscription == criteria.AllowSelfSubscription);

        // if (criteria.GroupCapacity != null)
        //    query = query.Where(x => x.GroupCapacity == criteria.GroupCapacity);

        // if (criteria.GroupCategory != null)
        //    query = query.Where(x => x.GroupCategory == criteria.GroupCategory);

        // if (criteria.GroupCode != null)
        //    query = query.Where(x => x.GroupCode == criteria.GroupCode);

        // if (criteria.GroupCreated != null)
        //    query = query.Where(x => x.GroupCreated == criteria.GroupCreated);

        // if (criteria.GroupType != null)
        //    query = query.Where(x => x.GroupType == criteria.GroupType);

        // if (criteria.GroupDescription != null)
        //    query = query.Where(x => x.GroupDescription == criteria.GroupDescription);

        // if (criteria.GroupFax != null)
        //    query = query.Where(x => x.GroupFax == criteria.GroupFax);

        // if (criteria.GroupImage != null)
        //    query = query.Where(x => x.GroupImage == criteria.GroupImage);

        // if (criteria.GroupIndustry != null)
        //    query = query.Where(x => x.GroupIndustry == criteria.GroupIndustry);

        // if (criteria.GroupIndustryComment != null)
        //    query = query.Where(x => x.GroupIndustryComment == criteria.GroupIndustryComment);

        // if (criteria.GroupLabel != null)
        //    query = query.Where(x => x.GroupLabel == criteria.GroupLabel);

        // if (criteria.GroupName != null)
        //    query = query.Where(x => x.GroupName == criteria.GroupName);

        // if (criteria.GroupOffice != null)
        //    query = query.Where(x => x.GroupOffice == criteria.GroupOffice);

        // if (criteria.GroupPhone != null)
        //    query = query.Where(x => x.GroupPhone == criteria.GroupPhone);

        // if (criteria.GroupRegion != null)
        //    query = query.Where(x => x.GroupRegion == criteria.GroupRegion);

        // if (criteria.GroupSize != null)
        //    query = query.Where(x => x.GroupSize == criteria.GroupSize);

        // if (criteria.GroupStatus != null)
        //    query = query.Where(x => x.GroupStatus == criteria.GroupStatus);

        // if (criteria.GroupWebSiteUrl != null)
        //    query = query.Where(x => x.GroupWebSiteUrl == criteria.GroupWebSiteUrl);

        // if (criteria.ShippingPreference != null)
        //    query = query.Where(x => x.ShippingPreference == criteria.ShippingPreference);

        // if (criteria.SurveyNecessity != null)
        //    query = query.Where(x => x.SurveyNecessity == criteria.SurveyNecessity);

        // if (criteria.LastChangeTime != null)
        //    query = query.Where(x => x.LastChangeTime == criteria.LastChangeTime);

        // if (criteria.LastChangeType != null)
        //    query = query.Where(x => x.LastChangeType == criteria.LastChangeType);

        // if (criteria.LastChangeUser != null)
        //    query = query.Where(x => x.LastChangeUser == criteria.LastChangeUser);

        // if (criteria.GroupEmail != null)
        //    query = query.Where(x => x.GroupEmail == criteria.GroupEmail);

        // if (criteria.SocialMediaUrls != null)
        //    query = query.Where(x => x.SocialMediaUrls == criteria.SocialMediaUrls);

        // if (criteria.GroupExpiry != null)
        //    query = query.Where(x => x.GroupExpiry == criteria.GroupExpiry);

        // if (criteria.LifetimeUnit != null)
        //    query = query.Where(x => x.LifetimeUnit == criteria.LifetimeUnit);

        // if (criteria.LifetimeQuantity != null)
        //    query = query.Where(x => x.LifetimeQuantity == criteria.LifetimeQuantity);

        // if (criteria.MessageToAdminWhenMembershipEnded != null)
        //    query = query.Where(x => x.MessageToAdminWhenMembershipEnded == criteria.MessageToAdminWhenMembershipEnded);

        // if (criteria.MessageToUserWhenMembershipEnded != null)
        //    query = query.Where(x => x.MessageToUserWhenMembershipEnded == criteria.MessageToUserWhenMembershipEnded);

        // if (criteria.MembershipProductIdentifier != null)
        //    query = query.Where(x => x.MembershipProductIdentifier == criteria.MembershipProductIdentifier);

        // if (criteria.AllowJoinGroupUsingLink != null)
        //    query = query.Where(x => x.AllowJoinGroupUsingLink == criteria.AllowJoinGroupUsingLink);

        return q;
    }

    public static async Task<IEnumerable<GroupMatch>> ToMatchesAsync(
        IQueryable<QGroupEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new GroupMatch
            {
                GroupIdentifier = entity.GroupIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}