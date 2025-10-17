using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Progress;

public interface IPeriodReader : IEntityReader
{
    Task<bool> AssertAsync(Guid period, CancellationToken cancellation = default);
    Task<PeriodEntity?> RetrieveAsync(Guid period, CancellationToken cancellation = default);
    Task<int> CountAsync(IPeriodCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<PeriodEntity>> CollectAsync(IPeriodCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<PeriodEntity>> DownloadAsync(IPeriodCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<PeriodMatch>> SearchAsync(IPeriodCriteria criteria, CancellationToken cancellation = default);
}

internal class PeriodReader : IPeriodReader
{
    private const string DefaultSort = "PeriodName";

    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly IShiftIdentityService _identity;

    public PeriodReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService identity)
    {
        _context = context;
        _identity = identity;
    }

    public async Task<bool> AssertAsync(
        Guid period,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QPeriod
            .AnyAsync(x => x.PeriodIdentifier == period, cancellation);
    }

    public async Task<PeriodEntity?> RetrieveAsync(
        Guid period,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QPeriod
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PeriodIdentifier == period, cancellation);
    }

    public async Task<int> CountAsync(
        IPeriodCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<PeriodEntity>> CollectAsync(
        IPeriodCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort ?? DefaultSort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<PeriodEntity>> DownloadAsync(
        IPeriodCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<PeriodMatch>> SearchAsync(
        IPeriodCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort ?? DefaultSort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<PeriodEntity> BuildQueryable(
        TableDbContext db,
        IPeriodCriteria criteria)
    {
        criteria.OrganizationId = _identity.OrganizationId;

        var q = db.QPeriod.AsNoTracking().AsQueryable();

        q = q.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.Name != null)
            q = q.Where(x => x.PeriodName.Contains(criteria.Name));

        if (criteria.StartSince != null)
            q = q.Where(x => x.PeriodStart >= criteria.StartSince);

        if (criteria.StartBefore != null)
            q = q.Where(x => x.PeriodStart < criteria.StartBefore);

        if (criteria.EndSince != null)
            q = q.Where(x => x.PeriodEnd >= criteria.EndSince);

        if (criteria.EndBefore != null)
            q = q.Where(x => x.PeriodEnd < criteria.EndBefore);

        return q;
    }

    public static async Task<IEnumerable<PeriodMatch>> ToMatchesAsync(
        IQueryable<PeriodEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PeriodMatch
            {
                Id = entity.PeriodIdentifier,
                Name = entity.PeriodName
            })
            .ToListAsync(cancellation);

        return matches;
    }
}