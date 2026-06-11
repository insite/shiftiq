using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Directory;

public class PersonWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public PersonWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(PersonEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.PersonIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QPerson.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid person, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QPerson.SingleOrDefaultAsync(x => x.PersonIdentifier == person, cancellation);
        if (entity == null)
            return false;

        db.QPerson.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(PersonEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.PersonIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid person, CancellationToken cancellation, TableDbContext db)
        => await db.QPerson.AsNoTracking().AnyAsync(x => x.PersonIdentifier == person, cancellation);
}