using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Learning;

public class CourseWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public CourseWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(CourseEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.CourseIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.Course.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid course, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.Course.SingleOrDefaultAsync(x => x.CourseIdentifier == course, cancellation);
        if (entity == null)
            return false;

        db.Course.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(CourseEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.CourseIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid course, CancellationToken cancellation, TableDbContext db)
		=> await db.Course.AsNoTracking().AnyAsync(x => x.CourseIdentifier == course, cancellation);
}