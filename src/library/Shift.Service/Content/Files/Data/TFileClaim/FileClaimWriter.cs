using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Content;

public interface IFileClaimWriter : IEntityWriter
{
    Task<bool> CreateAsync(FileClaimEntity entity, CancellationToken cancellation = default);
    Task<bool> DeleteAsync(Guid claim, CancellationToken cancellation = default);
    Task<bool> ModifyAsync(FileClaimEntity entity, CancellationToken cancellation = default);
}

internal class FileClaimWriter : IFileClaimWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FileClaimWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(FileClaimEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ClaimIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.TFileClaim.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid claim, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TFileClaim.SingleOrDefaultAsync(x => x.ClaimIdentifier == claim, cancellation);
        if (entity == null)
            return false;

        db.TFileClaim.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(FileClaimEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ClaimIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid claim, CancellationToken cancellation, TableDbContext db)
		=> await db.TFileClaim.AsNoTracking().AnyAsync(x => x.ClaimIdentifier == claim, cancellation);
}