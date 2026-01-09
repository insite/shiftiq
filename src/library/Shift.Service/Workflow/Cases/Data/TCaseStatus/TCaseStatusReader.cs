using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Cases;

public class TCaseStatusReader : IEntityReader
{
    private const string DefaultSort = "StatusSequence";

    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly IShiftIdentityService _identity;

    public TCaseStatusReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService identity)
    {
        _context = context;
        _identity = identity;
    }

    public async Task<bool> AssertAsync(
        Guid statusId,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();
        return await db.TCaseStatus
            .AnyAsync(x => x.StatusIdentifier == statusId, cancellation);
    }

    public async Task<TCaseStatusEntity?> RetrieveAsync(
        Guid statusId,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();
        return await db.TCaseStatus
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.StatusIdentifier == statusId, cancellation);
    }

    public async Task<int> CountAsync(
        ICaseStatusCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();
        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<TCaseStatusEntity>> CollectAsync(
        ICaseStatusCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();
        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort ?? DefaultSort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<TCaseStatusEntity>> DownloadAsync(
        ICaseStatusCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();
        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<CaseStatusMatch>> SearchAsync(
        ICaseStatusCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();
        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort ?? DefaultSort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<TCaseStatusEntity> BuildQueryable(
        TableDbContext db,
        ICaseStatusCriteria criteria)
    {
        criteria.OrganizationIdentifier = criteria.OrganizationIdentifier ?? _identity.OrganizationId;

        var q = db.TCaseStatus.AsNoTracking().AsQueryable();

        q = q.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        if (!string.IsNullOrEmpty(criteria.CaseTypeExact))
            q = q.Where(x => x.CaseType == criteria.CaseTypeExact);

        if (!string.IsNullOrEmpty(criteria.CaseTypeContains))
            q = q.Where(x => x.CaseType.Contains(criteria.CaseTypeContains));

        if (!string.IsNullOrEmpty(criteria.StatusNameExact))
            q = q.Where(x => x.StatusName == criteria.StatusNameExact);

        if (!string.IsNullOrEmpty(criteria.StatusNameContains))
            q = q.Where(x => x.StatusName.Contains(criteria.StatusNameContains));

        if (!string.IsNullOrEmpty(criteria.StatusCategoryExact))
            q = q.Where(x => x.StatusCategory == criteria.StatusCategoryExact);

        if (!string.IsNullOrEmpty(criteria.StatusCategoryContains))
            q = q.Where(x => x.StatusCategory.Contains(criteria.StatusCategoryContains));

        if (!string.IsNullOrEmpty(criteria.ReportCategoryExact))
            q = q.Where(x => x.ReportCategory == criteria.ReportCategoryExact);

        if (!string.IsNullOrEmpty(criteria.ReportCategoryContains))
            q = q.Where(x => x.ReportCategory!.Contains(criteria.ReportCategoryContains));

        if (criteria.StatusSequenceSince != null)
            q = q.Where(x => x.StatusSequence >= criteria.StatusSequenceSince);

        if (criteria.StatusSequenceBefore != null)
            q = q.Where(x => x.StatusSequence < criteria.StatusSequenceBefore);

        return q;
    }

    public static async Task<IEnumerable<CaseStatusMatch>> ToMatchesAsync(
        IQueryable<TCaseStatusEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new CaseStatusMatch
            {
                StatusIdentifier = entity.StatusIdentifier,
                StatusName = entity.StatusName,
                CaseType = entity.CaseType,
                StatusCategory = entity.StatusCategory,
                StatusSequence = entity.StatusSequence
            })
            .ToListAsync(cancellation);

        return matches;
    }
}
