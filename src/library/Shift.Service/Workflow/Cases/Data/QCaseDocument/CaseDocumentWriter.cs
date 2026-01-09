using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class CaseDocumentWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public CaseDocumentWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(CaseDocumentEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttachmentIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.QCaseDocument.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid attachment, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QCaseDocument.SingleOrDefaultAsync(x => x.AttachmentIdentifier == attachment, cancellation);
        if (entity == null)
            return false;

        db.QCaseDocument.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(CaseDocumentEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttachmentIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid attachment, CancellationToken cancellation, TableDbContext db)
		=> await db.QCaseDocument.AsNoTracking().AnyAsync(x => x.AttachmentIdentifier == attachment, cancellation);
}