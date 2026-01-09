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
        var q = db.QGradebookEnrollment.AsNoTracking().AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // TODO: Implement search criteria

        // if (criteria.GradebookIdentifier != null)
        //    query = query.Where(x => x.GradebookIdentifier == criteria.GradebookIdentifier);

        // if (criteria.LearnerIdentifier != null)
        //    query = query.Where(x => x.LearnerIdentifier == criteria.LearnerIdentifier);

        // if (criteria.PeriodIdentifier != null)
        //    query = query.Where(x => x.PeriodIdentifier == criteria.PeriodIdentifier);

        // if (criteria.EnrollmentIdentifier != null)
        //    query = query.Where(x => x.EnrollmentIdentifier == criteria.EnrollmentIdentifier);

        // if (criteria.EnrollmentStarted != null)
        //    query = query.Where(x => x.EnrollmentStarted == criteria.EnrollmentStarted);

        // if (criteria.EnrollmentComment != null)
        //    query = query.Where(x => x.EnrollmentComment == criteria.EnrollmentComment);

        // if (criteria.EnrollmentRestart != null)
        //    query = query.Where(x => x.EnrollmentRestart == criteria.EnrollmentRestart);

        // if (criteria.EnrollmentCompleted != null)
        //    query = query.Where(x => x.EnrollmentCompleted == criteria.EnrollmentCompleted);

        // if (criteria.OrganizationIdentifier != null)
        //    query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        return q;
    }

    public static async Task<IEnumerable<GradebookEnrollmentMatch>> ToMatchesAsync(
    IQueryable<QGradebookEnrollmentEntity> queryable,
    CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new GradebookEnrollmentMatch
            {
                EnrollmentIdentifier = entity.EnrollmentIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}