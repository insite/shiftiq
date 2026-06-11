using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class CaseDocumentRequestWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public CaseDocumentRequestWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(CaseDocumentRequestEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.CaseIdentifier, entity.RequestedFileCategory, cancellation, db);
        if (exists)
            return false;

        await db.QCaseDocumentRequest.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid issue, string requestedFileCategory, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QCaseDocumentRequest.SingleOrDefaultAsync(x => x.CaseIdentifier == issue && x.RequestedFileCategory == requestedFileCategory, cancellation);
        if (entity == null)
            return false;

        db.QCaseDocumentRequest.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(CaseDocumentRequestEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.CaseIdentifier, entity.RequestedFileCategory, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid issue, string requestedFileCategory, CancellationToken cancellation, TableDbContext db)
        => await db.QCaseDocumentRequest.AsNoTracking().AnyAsync(x => x.CaseIdentifier == issue && x.RequestedFileCategory == requestedFileCategory, cancellation);
}