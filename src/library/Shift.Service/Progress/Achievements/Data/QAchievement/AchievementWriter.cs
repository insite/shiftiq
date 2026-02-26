using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Progress;

public class AchievementWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public AchievementWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(AchievementEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AchievementIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QAchievement.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid achievement, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QAchievement.SingleOrDefaultAsync(x => x.AchievementIdentifier == achievement, cancellation);
        if (entity == null)
            return false;

        db.QAchievement.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(AchievementEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AchievementIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid achievement, CancellationToken cancellation, TableDbContext db)
        => await db.QAchievement.AsNoTracking().AnyAsync(x => x.AchievementIdentifier == achievement, cancellation);
}