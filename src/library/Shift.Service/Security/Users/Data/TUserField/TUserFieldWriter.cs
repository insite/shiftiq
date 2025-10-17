using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Security;

public class TUserFieldWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public TUserFieldWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(TUserFieldEntity entity, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SettingIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.TUserSetting.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(TUserFieldEntity entity, CancellationToken cancellation)
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

        var entity = await db.TUserSetting.SingleOrDefaultAsync(x => x.SettingIdentifier == setting, cancellation);
        if (entity == null)
            return false;

        db.TUserSetting.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid setting, CancellationToken cancellation, TableDbContext db)
        => await db.TUserSetting.AsNoTracking().AnyAsync(x => x.SettingIdentifier == setting, cancellation);
}