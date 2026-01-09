using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Content;

public class FileWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FileWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(FileEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.FileIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.TFile.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid file, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TFile.SingleOrDefaultAsync(x => x.FileIdentifier == file, cancellation);
        if (entity == null)
            return false;

        db.TFile.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(FileEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.FileIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid file, CancellationToken cancellation, TableDbContext db)
        => await db.TFile.AsNoTracking().AnyAsync(x => x.FileIdentifier == file, cancellation);
}