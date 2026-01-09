using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class SubmissionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public SubmissionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(SubmissionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ResponseSessionIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.Submission.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid responseSession, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.Submission.SingleOrDefaultAsync(x => x.ResponseSessionIdentifier == responseSession, cancellation);
        if (entity == null)
            return false;

        db.Submission.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(SubmissionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ResponseSessionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid responseSession, CancellationToken cancellation, TableDbContext db)
        => await db.Submission.AsNoTracking().AnyAsync(x => x.ResponseSessionIdentifier == responseSession, cancellation);
}