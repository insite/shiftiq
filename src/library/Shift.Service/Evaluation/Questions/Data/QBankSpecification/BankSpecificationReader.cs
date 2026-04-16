using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class BankSpecificationReader(IDbContextFactory<TableDbContext> context) : IEntityReader
{
    public async Task<BankSpecificationEntity?> RetrieveAsync(Guid specId)
    {
        using var db = context.CreateDbContext();

        return await db.BankSpecification
            .Where(x => x.SpecIdentifier == specId)
            .FirstOrDefaultAsync();
    }
}
