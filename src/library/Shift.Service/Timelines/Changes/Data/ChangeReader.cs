using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Timeline;

public class ChangeReader(IDbContextFactory<TableDbContext> context) : IEntityReader
{
    public Task<List<ChangeEntity>> CollectAsync(Guid aggregateId, IEnumerable<string> changeTypes)
    {
        return ExecuteAsync(db =>
        {
            return db.Change
                .Where(x =>
                    x.AggregateIdentifier == aggregateId
                    && changeTypes.Contains(x.ChangeType)
                )
                .OrderBy(x => x.AggregateVersion)
                .ToListAsync();
        });
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query)
    {
        using var db = context.CreateDbContext();
        return await query(db);
    }
}
