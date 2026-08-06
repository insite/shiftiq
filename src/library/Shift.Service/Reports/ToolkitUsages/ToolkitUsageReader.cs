using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Reports;

public class ToolkitUsageReader(IDbContextFactory<TableDbContext> context): IEntityReader
{
    public async Task<ToolkitUsageEntity[]> CollectAsync(Guid organizationId, Guid userId)
    {
        using var db = context.CreateDbContext();

        return await db.TToolkitUsage
            .Where(x => x.OrganizationIdentifier == organizationId && x.UserIdentifier == userId)
            .ToArrayAsync();
    }
}