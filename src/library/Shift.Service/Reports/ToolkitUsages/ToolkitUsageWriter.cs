using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Reports;

public class ToolkitUsageWriter(IDbContextFactory<TableDbContext> context): IEntityWriter
{
    public async Task<int> CalculateAsync()
    {
        using var db = context.CreateDbContext();

        return (await db.Database
            .SqlQuery<int>($"exec reports.CalcToolkitUsage")
            .ToListAsync())
            .First();
    }
}