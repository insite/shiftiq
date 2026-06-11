using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class BankQuestionReader(IDbContextFactory<TableDbContext> context) : IEntityReader
{
    public async Task<List<BankQuestionEntity>> CollectByBankIdAsync(Guid bankId)
    {
        using var db = context.CreateDbContext();

        return await db.BankQuestion
            .Where(x => x.BankIdentifier == bankId)
            .ToListAsync();
    }
}
