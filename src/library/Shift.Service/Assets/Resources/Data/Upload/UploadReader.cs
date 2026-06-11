using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Content;

public class UploadReader(IDbContextFactory<TableDbContext> context) : IEntityReader
{
    public Task<List<UploadEntity>> CollectAsync(Guid organizationId, IEnumerable<Guid> uploadIds)
    {
        return ExecuteAsync(db =>
        {
            return db.Upload
                .Where(x => x.OrganizationIdentifier == organizationId && uploadIds.Contains(x.UploadIdentifier))
                .OrderBy(x => x.UploadIdentifier)
                .ToListAsync();
        });
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query)
    {
        using var db = context.CreateDbContext();
        return await query(db);
    }
}
