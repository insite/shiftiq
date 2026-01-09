using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class BankQuestionAttachmentWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public BankQuestionAttachmentWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(BankQuestionAttachmentEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.QuestionIdentifier, entity.UploadIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.BankQuestionAttachment.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid question, Guid upload, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.BankQuestionAttachment.SingleOrDefaultAsync(x => x.QuestionIdentifier == question && x.UploadIdentifier == upload, cancellation);
        if (entity == null)
            return false;

        db.BankQuestionAttachment.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(BankQuestionAttachmentEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.QuestionIdentifier, entity.UploadIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid question, Guid upload, CancellationToken cancellation, TableDbContext db)
		=> await db.BankQuestionAttachment.AsNoTracking().AnyAsync(x => x.QuestionIdentifier == question && x.UploadIdentifier == upload, cancellation);
}