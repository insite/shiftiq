using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Messaging;

public class RecipientReader : IEntityReader
{
    private string DefaultSort = "RecipientIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    public RecipientReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid recipient, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.RecipientIdentifier == recipient && (organization == null || x.OrganizationIdentifier == organization), cancellation);

        }, cancellation);
    }

    public Task<List<RecipientEntity>> CollectAsync(IRecipientCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IRecipientCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<RecipientEntity> DownloadAsync(IRecipientCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<RecipientEntity?> RetrieveAsync(Guid recipient, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.RecipientIdentifier == recipient, cancellation);

        }, cancellation);
    }

    public Task<List<RecipientMatch>> SearchAsync(IRecipientCriteria criteria, CancellationToken cancellation = default)
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

    private IQueryable<RecipientEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.Recipient
            .AsNoTracking();

        return query;
    }

    private IQueryable<RecipientEntity> BuildQueryable(TableDbContext db, IRecipientCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.DeliveryCompletedSince.HasValue)
            query = query.Where(x => x.DeliveryCompleted >= criteria.DeliveryCompletedSince);

        if (criteria.DeliveryCompletedBefore.HasValue)
            query = query.Where(x => x.DeliveryCompleted < criteria.DeliveryCompletedBefore);

        if (criteria.DeliveryStartedSince.HasValue)
            query = query.Where(x => x.DeliveryStarted >= criteria.DeliveryStartedSince);

        if (criteria.DeliveryStartedBefore.HasValue)
            query = query.Where(x => x.DeliveryStarted < criteria.DeliveryStartedBefore);

        if (criteria.MailoutId != null)
            query = query.Where(x => x.MailoutIdentifier == criteria.MailoutId);

        if (criteria.UserEmail != null)
            query = query.Where(x => x.UserEmail == criteria.UserEmail);

        if (criteria.UserId != null)
            query = query.Where(x => x.UserIdentifier == criteria.UserId);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<RecipientMatch>> ToMatchesAsync(IQueryable<RecipientEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new RecipientMatch
            {
                RecipientId = entity.RecipientIdentifier
            })
            .ToListAsync(cancellation);

        return matches;
    }
}