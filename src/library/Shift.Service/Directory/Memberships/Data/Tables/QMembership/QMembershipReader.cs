using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Directory;

public class QMembershipReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QMembershipReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(
        Guid membership,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QMembership
            .AnyAsync(x => x.MembershipIdentifier == membership, cancellation);
    }

    public async Task<QMembershipEntity?> RetrieveAsync(
        Guid membership,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QMembership
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MembershipIdentifier == membership, cancellation);
    }

    public async Task<int> CountAsync(
        IMembershipCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QMembershipEntity>> CollectAsync(
        IMembershipCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<QMembershipEntity>> DownloadAsync(
        IMembershipCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<MembershipMatch>> SearchAsync(
        IMembershipCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QMembershipEntity> BuildQueryable(
        TableDbContext db,
        IMembershipCriteria criteria)
    {
        var q = db.QMembership
            .AsNoTracking()
            .AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // TODO: Implement search criteria

        // if (criteria.MembershipIdentifier != null)
        //    query = query.Where(x => x.MembershipIdentifier == criteria.MembershipIdentifier);

        // if (criteria.MembershipEffective != null)
        //    query = query.Where(x => x.MembershipEffective == criteria.MembershipEffective);

        // if (criteria.MembershipFunction != null)
        //    query = query.Where(x => x.MembershipFunction == criteria.MembershipFunction);

        // if (criteria.GroupIdentifier != null)
        //    query = query.Where(x => x.GroupIdentifier == criteria.GroupIdentifier);

        // if (criteria.OrganizationIdentifier != null)
        //    query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        // if (criteria.UserIdentifier != null)
        //    query = query.Where(x => x.UserIdentifier == criteria.UserIdentifier);

        // if (criteria.MembershipExpiry != null)
        //    query = query.Where(x => x.MembershipExpiry == criteria.MembershipExpiry);

        // if (criteria.Modified != null)
        //    query = query.Where(x => x.Modified == criteria.Modified);

        // if (criteria.ModifiedBy != null)
        //    query = query.Where(x => x.ModifiedBy == criteria.ModifiedBy);

        return q;
    }

    public static async Task<IEnumerable<MembershipMatch>> ToMatchesAsync(
        IQueryable<QMembershipEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new MembershipMatch
            {
                MembershipIdentifier = entity.MembershipIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}