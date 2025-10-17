using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Security;

public class QUserConnectionReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QUserConnectionReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(
        Guid fromUser, Guid toUser,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QUserConnection
            .AnyAsync(x => x.FromUserIdentifier == fromUser && x.ToUserIdentifier == toUser, cancellation);
    }

    public async Task<QUserConnectionEntity?> RetrieveAsync(
        Guid fromUser, Guid toUser,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QUserConnection
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FromUserIdentifier == fromUser && x.ToUserIdentifier == toUser, cancellation);
    }

    public async Task<int> CountAsync(
        IUserConnectionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QUserConnectionEntity>> CollectAsync(
        IUserConnectionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<QUserConnectionEntity>> DownloadAsync(
        IUserConnectionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserConnectionMatch>> SearchAsync(
        IUserConnectionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QUserConnectionEntity> BuildQueryable(
        TableDbContext db,
        IUserConnectionCriteria criteria)
    {
        var q = db.QUserConnection
            .AsNoTracking()
            .AsQueryable();

        // TODO: Implement search criteria

        // if (criteria.Connected != null)
        //    query = query.Where(x => x.Connected == criteria.Connected);

        // if (criteria.IsManager != null)
        //    query = query.Where(x => x.IsManager == criteria.IsManager);

        // if (criteria.IsSupervisor != null)
        //    query = query.Where(x => x.IsSupervisor == criteria.IsSupervisor);

        // if (criteria.IsValidator != null)
        //    query = query.Where(x => x.IsValidator == criteria.IsValidator);

        // if (criteria.FromUserIdentifier != null)
        //    query = query.Where(x => x.FromUserIdentifier == criteria.FromUserIdentifier);

        // if (criteria.ToUserIdentifier != null)
        //    query = query.Where(x => x.ToUserIdentifier == criteria.ToUserIdentifier);

        // if (criteria.IsLeader != null)
        //    query = query.Where(x => x.IsLeader == criteria.IsLeader);

        return q;
    }

    public static async Task<IEnumerable<UserConnectionMatch>> ToMatchesAsync(
        IQueryable<QUserConnectionEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new UserConnectionMatch
            {
                FromUserIdentifier = entity.FromUserIdentifier,
                ToUserIdentifier = entity.ToUserIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}