using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Booking;

public class EventUserReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private readonly IShiftIdentityService _auth;

    public EventUserReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService auth)
    {
        _context = context;
        _auth = auth;
    }

    public Task<bool> AssertAsync(Guid @event, Guid user, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.EventIdentifier == @event && x.UserIdentifier == user, cancellation);

        }, cancellation);
    }

    public Task<List<EventUserEntity>> CollectAsync(IEventUserCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query
                .OrderBy(criteria.Filter.Sort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync();

        }, cancellation);
    }

    public Task<int> CountAsync(IEventUserCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<EventUserEntity> DownloadAsync(IEventUserCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<EventUserEntity?> RetrieveAsync(Guid @event, Guid user, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.EventIdentifier == @event && x.UserIdentifier == user, cancellation);

        }, cancellation);
    }

    public Task<List<EventUserMatch>> SearchAsync(IEventUserCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<EventUserEntity> BuildQueryable(TableDbContext db)
    {
        ValidateOrganizationContext();

        var query = db.QEventUser
            .Include(x => x.Event)
            .Include(x => x.User)
            .AsNoTracking()
            .Where(x => x.OrganizationIdentifier == _auth.OrganizationId);

        return query;
    }

    private IQueryable<EventUserEntity> BuildQueryable(TableDbContext db, IEventUserCriteria criteria)
    {
        var query = BuildQueryable(db);

        if (criteria.Role != null)
            query = query.Where(x => x.AttendeeRole == criteria.Role);

        if (criteria.EventId != null)
            query = query.Where(x => x.EventIdentifier == criteria.EventId);

        if (criteria.UserId != null)
            query = query.Where(x => x.UserIdentifier == criteria.UserId);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    private async Task<List<EventUserMatch>> ToMatchesAsync(IQueryable<EventUserEntity> query, CancellationToken cancellation = default)
    {
        var matches = await query
            .Select(entity => new EventUserMatch
            {
                EventId = entity.EventIdentifier,
                EventTitle = entity.Event!.EventTitle,

                UserId = entity.UserIdentifier,
                UserName = entity.User!.FullName,

                RelationCreated = entity.Assigned,
                RelationRole = entity.AttendeeRole

            })
            .ToListAsync(cancellation);

        return matches;
    }

    private void ValidateOrganizationContext()
    {
        if (_auth.OrganizationId == Guid.Empty)
            throw new InvalidOperationException("Organization context is required");
    }
}