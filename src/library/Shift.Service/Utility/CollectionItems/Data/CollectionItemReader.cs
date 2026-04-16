using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Utility;

public class CollectionItemReader(IDbContextFactory<TableDbContext> context) : IEntityReader
{
    public Task<List<CollectionItemEntity>> CollectAsync(Guid organizationId, string collectionName)
    {
        return ExecuteAsync(db =>
        {
            return db.TCollectionItem
                .Where(x => x.OrganizationIdentifier == organizationId)
                .Join(db.TCollection.Where(x => x.CollectionName == collectionName),
                    i => i.CollectionIdentifier,
                    c => c.CollectionIdentifier,
                    (i, c) => i
                )
                .OrderBy(x => x.ItemSequence)
                .ToListAsync();
        });
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query)
    {
        using var db = context.CreateDbContext();
        return await query(db);
    }    
}
