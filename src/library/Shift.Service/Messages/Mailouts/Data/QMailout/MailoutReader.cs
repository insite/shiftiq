using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Messaging;

public class MailoutReader : IEntityReader
{
    private string DefaultSort = "MailoutIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    public MailoutReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid mailout, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.MailoutIdentifier == mailout && (organization == null || x.OrganizationIdentifier == organization), cancellation);

        }, cancellation);
    }

    public Task<List<MailoutEntity>> CollectAsync(IMailoutCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

        }, cancellation);
    }

    public Task<int> CountAsync(IMailoutCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<MailoutEntity> DownloadAsync(IMailoutCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<MailoutEntity?> RetrieveAsync(Guid mailout, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.MailoutIdentifier == mailout, cancellation);

        }, cancellation);
    }

    public Task<List<MailoutMatch>> SearchAsync(IMailoutCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            query = query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter);

            return ToMatchesAsync(query, cancellation);

        }, cancellation);
    }

    private IQueryable<MailoutEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.Mailout
            .AsNoTracking();

        return query;
    }

    private IQueryable<MailoutEntity> BuildQueryable(TableDbContext db, IMailoutCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.MailoutCancelledSince.HasValue)
            query = query.Where(x => x.MailoutCancelled >= criteria.MailoutCancelledSince);

        if (criteria.MailoutCancelledBefore.HasValue)
            query = query.Where(x => x.MailoutCancelled < criteria.MailoutCancelledBefore);

        if (criteria.MailoutCompletedSince.HasValue)
            query = query.Where(x => x.MailoutCompleted >= criteria.MailoutCompletedSince);

        if (criteria.MailoutCompletedBefore.HasValue)
            query = query.Where(x => x.MailoutCompleted < criteria.MailoutCompletedBefore);

        if (criteria.MailoutScheduledSince.HasValue)
            query = query.Where(x => x.MailoutScheduled >= criteria.MailoutScheduledSince);

        if (criteria.MailoutScheduledBefore.HasValue)
            query = query.Where(x => x.MailoutScheduled < criteria.MailoutScheduledBefore);

        if (criteria.MailoutStartedSince.HasValue)
            query = query.Where(x => x.MailoutStarted >= criteria.MailoutStartedSince);

        if (criteria.MailoutStartedBefore.HasValue)
            query = query.Where(x => x.MailoutStarted < criteria.MailoutStartedBefore);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<MailoutMatch>> ToMatchesAsync(IQueryable<MailoutEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new MailoutMatch
            {
                MailoutId = entity.MailoutIdentifier
            })
            .ToListAsync(cancellation);

        return matches;
    }
}