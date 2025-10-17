using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Booking;

public interface IEventUserReader : IEntityReader
{
    Task<bool> AssertAsync(Guid @event, Guid user, CancellationToken cancellation = default);
    Task<EventUserEntity?> RetrieveAsync(Guid @event, Guid user, CancellationToken cancellation = default);
    Task<int> CountAsync(IEventUserCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<EventUserEntity>> CollectAsync(IEventUserCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<EventUserEntity>> DownloadAsync(IEventUserCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<EventUserMatch>> SearchAsync(IEventUserCriteria criteria, CancellationToken cancellation = default);
}

public class EventUserReader : IEventUserReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly IShiftIdentityService _identityService;

    public EventUserReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<bool> AssertAsync(
        Guid @event, Guid user,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QEventUser
            .AnyAsync(x => x.EventIdentifier == @event && x.UserIdentifier == user, cancellation);
    }

    public async Task<EventUserEntity?> RetrieveAsync(
        Guid @event, Guid user,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QEventUser
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EventIdentifier == @event && x.UserIdentifier == user, cancellation);
    }

    public async Task<int> CountAsync(
        IEventUserCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<EventUserEntity>> CollectAsync(
        IEventUserCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<EventUserEntity>> DownloadAsync(
        IEventUserCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<EventUserMatch>> SearchAsync(
        IEventUserCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<EventUserEntity> BuildQueryable(
        TableDbContext db,
        IEventUserCriteria criteria)
    {
        var q = db.QEventUser
            .Include(x => x.Event)
            .Include(x => x.User)
            .AsNoTracking()
            .AsQueryable();

        q = q.Where(x => x.OrganizationIdentifier == _identityService.OrganizationId);

        if (criteria.Role != null)
            q = q.Where(x => x.AttendeeRole == criteria.Role);

        if (criteria.EventId != null)
            q = q.Where(x => x.EventIdentifier == criteria.EventId);

        if (criteria.UserId != null)
            q = q.Where(x => x.UserIdentifier == criteria.UserId);

        return q;
    }

    public static async Task<IEnumerable<EventUserMatch>> ToMatchesAsync(
        IQueryable<EventUserEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
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
}