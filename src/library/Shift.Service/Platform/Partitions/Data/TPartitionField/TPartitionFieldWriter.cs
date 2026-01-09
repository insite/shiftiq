using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Security;

public class TPartitionFieldWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public TPartitionFieldWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(TPartitionFieldEntity entity, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SettingIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.TPartitionSetting.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(TPartitionFieldEntity entity, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SettingIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid setting, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TPartitionSetting.SingleOrDefaultAsync(x => x.SettingIdentifier == setting, cancellation);
        if (entity == null)
            return false;

        db.TPartitionSetting.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid setting, CancellationToken cancellation, TableDbContext db)
        => await db.TPartitionSetting.AsNoTracking().AnyAsync(x => x.SettingIdentifier == setting, cancellation);
}