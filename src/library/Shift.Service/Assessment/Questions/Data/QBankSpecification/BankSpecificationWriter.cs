using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class BankSpecificationWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public BankSpecificationWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(BankSpecificationEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SpecIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.BankSpecification.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid spec, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.BankSpecification.SingleOrDefaultAsync(x => x.SpecIdentifier == spec, cancellation);
        if (entity == null)
            return false;

        db.BankSpecification.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(BankSpecificationEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SpecIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid spec, CancellationToken cancellation, TableDbContext db)
		=> await db.BankSpecification.AsNoTracking().AnyAsync(x => x.SpecIdentifier == spec, cancellation);
}