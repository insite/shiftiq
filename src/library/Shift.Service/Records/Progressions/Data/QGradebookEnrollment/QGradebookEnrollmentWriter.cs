using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Progress;

public class QGradebookEnrollmentWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QGradebookEnrollmentWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(QGradebookEnrollmentEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.EnrollmentIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QGradebookEnrollment.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(QGradebookEnrollmentEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.EnrollmentIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid enrollment, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QGradebookEnrollment.SingleOrDefaultAsync(x => x.EnrollmentIdentifier == enrollment, cancellation);
        if (entity == null)
            return false;

        db.QGradebookEnrollment.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid enrollment, CancellationToken cancellation, TableDbContext db)
        => await db.QGradebookEnrollment.AsNoTracking().AnyAsync(x => x.EnrollmentIdentifier == enrollment, cancellation);
}