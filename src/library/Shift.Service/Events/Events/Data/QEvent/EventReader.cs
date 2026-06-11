using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Booking;

public class EventReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public EventReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid @event, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.EventIdentifier == @event && (organization == null || organization == x.OrganizationIdentifier), cancellation);

        }, cancellation);
    }

    public Task<List<EventEntity>> CollectAsync(IEventCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query
                .OrderBy(criteria.Filter.Sort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

        }, cancellation);
    }

    public Task<int> CountAsync(IEventCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<EventEntity> DownloadAsync(IEventCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<EventEntity?> RetrieveAsync(Guid @event, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.EventIdentifier == @event, cancellation);

        }, cancellation);
    }

    public Task<List<EventMatch>> SearchAsync(IEventCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            query = query
                .OrderBy(criteria.Filter.Sort)
                .ApplyPaging(criteria.Filter);

            return ToMatchesAsync(query, cancellation);

        }, cancellation);
    }

    /// <summary>
    /// Creates a queryable for events
    /// </summary>
    /// <remarks>
    /// If you call .Include() on the DbSet then remember to use .AsSplitQuery() so that cartesian explosion is avoided.
    /// When using split queries with Skip/Take on EF versions prior to 10, pay special attention to make your query
    /// ordering fully unique, otherwise the result set is non-deterministic.
    /// </remarks>
    private IQueryable<EventEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.QEvent
            .AsNoTracking();

        return query;
    }

    private IQueryable<EventEntity> BuildQueryable(TableDbContext db, IEventCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId.Value);

        if (criteria.DistributionExpectedSince.HasValue)
            query = query.Where(x => x.DistributionExpected >= criteria.DistributionExpectedSince);

        if (criteria.DistributionExpectedBefore.HasValue)
            query = query.Where(x => x.DistributionExpected < criteria.DistributionExpectedBefore);

        if (criteria.DistributionOrderedSince.HasValue)
            query = query.Where(x => x.DistributionOrdered >= criteria.DistributionOrderedSince);

        if (criteria.DistributionOrderedBefore.HasValue)
            query = query.Where(x => x.DistributionOrdered < criteria.DistributionOrderedBefore);

        if (criteria.DistributionShippedSince.HasValue)
            query = query.Where(x => x.DistributionShipped >= criteria.DistributionShippedSince);

        if (criteria.DistributionShippedBefore.HasValue)
            query = query.Where(x => x.DistributionShipped < criteria.DistributionShippedBefore);

        if (criteria.DistributionTrackedSince.HasValue)
            query = query.Where(x => x.DistributionTracked >= criteria.DistributionTrackedSince);

        if (criteria.DistributionTrackedBefore.HasValue)
            query = query.Where(x => x.DistributionTracked < criteria.DistributionTrackedBefore);

        if (criteria.EventScheduledEndSince.HasValue)
            query = query.Where(x => x.EventScheduledEnd >= criteria.EventScheduledEndSince);

        if (criteria.EventScheduledEndBefore.HasValue)
            query = query.Where(x => x.EventScheduledEnd < criteria.EventScheduledEndBefore);

        if (criteria.EventScheduledStartSince.HasValue)
            query = query.Where(x => x.EventScheduledStart >= criteria.EventScheduledStartSince);

        if (criteria.EventScheduledStartBefore.HasValue)
            query = query.Where(x => x.EventScheduledStart < criteria.EventScheduledStartBefore);

        if (criteria.ExamMaterialReturnShipmentReceivedSince.HasValue)
            query = query.Where(x => x.ExamMaterialReturnShipmentReceived >= criteria.ExamMaterialReturnShipmentReceivedSince);

        if (criteria.ExamMaterialReturnShipmentReceivedBefore.HasValue)
            query = query.Where(x => x.ExamMaterialReturnShipmentReceived < criteria.ExamMaterialReturnShipmentReceivedBefore);

        if (criteria.ExamStartedSince.HasValue)
            query = query.Where(x => x.ExamStarted >= criteria.ExamStartedSince);

        if (criteria.ExamStartedBefore.HasValue)
            query = query.Where(x => x.ExamStarted < criteria.ExamStartedBefore);

        if (criteria.LastChangeTimeSince.HasValue)
            query = query.Where(x => x.LastChangeTime >= criteria.LastChangeTimeSince);

        if (criteria.LastChangeTimeBefore.HasValue)
            query = query.Where(x => x.LastChangeTime < criteria.LastChangeTimeBefore);

        if (criteria.RegistrationDeadlineSince.HasValue)
            query = query.Where(x => x.RegistrationDeadline >= criteria.RegistrationDeadlineSince);

        if (criteria.RegistrationDeadlineBefore.HasValue)
            query = query.Where(x => x.RegistrationDeadline < criteria.RegistrationDeadlineBefore);

        if (criteria.RegistrationLockedSince.HasValue)
            query = query.Where(x => x.RegistrationLocked >= criteria.RegistrationLockedSince);

        if (criteria.RegistrationLockedBefore.HasValue)
            query = query.Where(x => x.RegistrationLocked < criteria.RegistrationLockedBefore);

        if (criteria.RegistrationStartSince.HasValue)
            query = query.Where(x => x.RegistrationStart >= criteria.RegistrationStartSince);

        if (criteria.RegistrationStartBefore.HasValue)
            query = query.Where(x => x.RegistrationStart < criteria.RegistrationStartBefore);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    private async Task<List<EventMatch>> ToMatchesAsync(IQueryable<EventEntity> query, CancellationToken cancellation = default)
    {
        var matches = await query
            .Select(entity => new EventMatch
            {
                EventId = entity.EventIdentifier
            })
            .ToListAsync(cancellation);

        return matches;
    }
}
