using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Messaging;

public class RecipientWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public RecipientWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(RecipientEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.RecipientIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.Recipient.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid recipient, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.Recipient.SingleOrDefaultAsync(x => x.RecipientIdentifier == recipient, cancellation);
        if (entity == null)
            return false;

        db.Recipient.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(RecipientEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.RecipientIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid recipient, CancellationToken cancellation, TableDbContext db)
        => await db.Recipient.AsNoTracking().AnyAsync(x => x.RecipientIdentifier == recipient, cancellation);
}