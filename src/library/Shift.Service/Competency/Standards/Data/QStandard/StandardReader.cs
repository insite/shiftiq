using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Competency;

public interface IStandardReader : IEntityReader
{
    Task<bool> AssertAsync(Guid standard, CancellationToken cancellation = default);
    Task<StandardEntity?> RetrieveAsync(Guid standard, CancellationToken cancellation = default);
    Task<int> CountAsync(IStandardCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<StandardEntity>> CollectAsync(IStandardCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<StandardEntity>> DownloadAsync(IStandardCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<StandardMatch>> SearchAsync(IStandardCriteria criteria, CancellationToken cancellation = default);
}

internal class StandardReader : IStandardReader
{
    private const string DefaultSort = "ContentName";
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly IShiftIdentityService _identity;

    public StandardReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService identity)
    {
        _context = context;
        _identity = identity;
    }

    public async Task<bool> AssertAsync(
        Guid standard,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QStandard
            .AnyAsync(x => x.StandardIdentifier == standard, cancellation);
    }

    public async Task<StandardEntity?> RetrieveAsync(
        Guid standard,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QStandard
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.StandardIdentifier == standard, cancellation);
    }

    public async Task<int> CountAsync(
        IStandardCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<StandardEntity>> CollectAsync(
        IStandardCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort ?? DefaultSort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<StandardEntity>> DownloadAsync(
        IStandardCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<StandardMatch>> SearchAsync(
        IStandardCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort ?? DefaultSort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<StandardEntity> BuildQueryable(
        TableDbContext db,
        IStandardCriteria criteria)
    {
        var q = db.QStandard.AsNoTracking().AsQueryable();

        // Force the organization identifier in the criteria to match the caller's context

        criteria.OrganizationId = _identity.OrganizationId;

        q = q.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (!string.IsNullOrEmpty(criteria.ContentTitle))
            q = q.Where(x => x.ContentTitle!.Contains(criteria.ContentTitle));

        if (!string.IsNullOrEmpty(criteria.StandardType))
            q = q.Where(x => x.StandardType == criteria.StandardType);

        return q;
    }

    public static async Task<IEnumerable<StandardMatch>> ToMatchesAsync(
        IQueryable<StandardEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new StandardMatch
            {
                Code = entity.Code,
                Id = entity.StandardIdentifier,
                Name = entity.ContentName,
                Title = entity.ContentTitle,
                Type = entity.StandardType
            })
            .ToListAsync(cancellation);

        return matches;
    }
}