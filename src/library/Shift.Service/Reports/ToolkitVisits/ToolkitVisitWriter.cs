using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Reports;

public class ToolkitVisitWriter(IDbContextFactory<TableDbContext> context): IEntityWriter
{
    public async Task<bool> SaveVisitAsync(Guid organizationId, Guid userId, Guid tokenId, Guid actionId, DateTimeOffset visited)
    {
        using var db = context.CreateDbContext();

        var entity = new ToolkitVisitEntity
        {
            ToolkitVisitIdentifier = UniqueIdentifier.Create(),
            OrganizationIdentifier = organizationId,
            UserIdentifier = userId,
            TokenIdentifier = tokenId,
            ActionIdentifier = actionId,
            Visited = visited
        };

        await db.TToolkitVisit.AddAsync(entity);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("Cannot insert duplicate key row"))
                return false;

            throw;
        }

        return true;
    }
}