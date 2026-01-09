using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Progress;

public class QCredentialWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QCredentialWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(QCredentialEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.CredentialIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QCredential.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(QCredentialEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.CredentialIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid credential, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QCredential.SingleOrDefaultAsync(x => x.CredentialIdentifier == credential, cancellation);
        if (entity == null)
            return false;

        db.QCredential.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid credential, CancellationToken cancellation, TableDbContext db)
        => await db.QCredential.AsNoTracking().AnyAsync(x => x.CredentialIdentifier == credential, cancellation);
}