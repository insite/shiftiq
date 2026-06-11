using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Progress;

public class QGradebookEnrollmentReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly QGradebookEnrollmentAdapter _adapter;

    public QGradebookEnrollmentReader(IDbContextFactory<TableDbContext> context, QGradebookEnrollmentAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    public async Task<bool> AssertAsync(Guid enrollment, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QGradebookEnrollment
            .AnyAsync(x => x.EnrollmentIdentifier == enrollment, cancellation);
    }

    public async Task<QGradebookEnrollmentEntity?> RetrieveAsync(Guid enrollment, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QGradebookEnrollment
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EnrollmentIdentifier == enrollment, cancellation);
    }

    public async Task<int> CountAsync(IGradebookEnrollmentCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QGradebookEnrollmentEntity>> CollectAsync(IGradebookEnrollmentCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<GradebookEnrollmentMatch>> SearchAsync(IGradebookEnrollmentCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QGradebookEnrollmentEntity> BuildQueryable(TableDbContext db, IGradebookEnrollmentCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = db.QGradebookEnrollment.AsNoTracking().AsQueryable();

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.EnrollmentCompletedSince.HasValue)
            query = query.Where(x => x.EnrollmentCompleted >= criteria.EnrollmentCompletedSince);

        if (criteria.EnrollmentCompletedBefore.HasValue)
            query = query.Where(x => x.EnrollmentCompleted < criteria.EnrollmentCompletedBefore);

        if (criteria.EnrollmentStartedSince.HasValue)
            query = query.Where(x => x.EnrollmentStarted >= criteria.EnrollmentStartedSince);

        if (criteria.EnrollmentStartedBefore.HasValue)
            query = query.Where(x => x.EnrollmentStarted < criteria.EnrollmentStartedBefore);

        return query;
    }

    public static async Task<IEnumerable<GradebookEnrollmentMatch>> ToMatchesAsync(
    IQueryable<QGradebookEnrollmentEntity> queryable,
    CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new GradebookEnrollmentMatch
            {
                EnrollmentId = entity.EnrollmentIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}