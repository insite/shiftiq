using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Security;

public class QUserReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly IShiftIdentityService _identityService;

    public QUserReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<bool> AssertAsync(
        Guid user,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QUser
            .AnyAsync(x => x.UserIdentifier == user, cancellation);
    }

    public async Task<QUserEntity?> RetrieveAsync(
        Guid user,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QUser
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserIdentifier == user, cancellation);
    }

    public async Task<int> CountAsync(
        IUserCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QUserEntity>> CollectAsync(
        IUserCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<QUserEntity>> DownloadAsync(
        IUserCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserMatch>> SearchAsync(
        IUserCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QUserEntity> BuildQueryable(
        TableDbContext db,
        IUserCriteria criteria)
    {
        var q = db.QUser
            .AsNoTracking()
            .AsQueryable();

        var organizationId = _identityService.OrganizationId;

        q = q.Where(u => 0 < u.People.Count(p => p.OrganizationIdentifier == organizationId));

        if (criteria.UserEmailExact.IsNotEmpty())
            q = q.Where(x => x.Email == criteria.UserEmailExact);

        if (criteria.UserFullNameContains.IsNotEmpty())
            q = q.Where(x => x.FullName.Contains(criteria.UserFullNameContains));

        if (criteria.Filter?.Sort.NullIfEmpty() == null)
            q = q.OrderBy(x => x.FullName);
        else
            q = q.OrderBy(criteria.Filter.Sort);

        return q;
    }

    public static async Task<IEnumerable<UserMatch>> ToMatchesAsync(
        IQueryable<QUserEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new UserMatch
            {
                UserIdentifier = entity.UserIdentifier,
                FullName = entity.FullName
            })
            .ToListAsync(cancellation);

        return matches;
    }
}