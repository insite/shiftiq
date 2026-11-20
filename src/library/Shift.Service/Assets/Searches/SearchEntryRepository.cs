using Microsoft.EntityFrameworkCore;

using Shift.Service.Content.Searches.Dtos;

namespace Shift.Service.Content.Searches;

public class SearchEntryRepository(IDbContextFactory<TableDbContext> context)
{
    internal async Task<SearchEntryDto[]> SearchAsync(Guid organizationId, Guid userId, bool isOperator, string searchText, int take, int skip, CancellationToken cancellation)
    {
        await using var db = await context.CreateDbContextAsync(cancellation);

        return await db.Database
            .SqlQuery<SearchEntryDto>(
                $"""
                exec assets.Search
                    @OrganizationIdentifier = {organizationId}
                    ,@UserIdentifier = {userId}
                    ,@IsOperator = {isOperator}
                    ,@SearchText = {searchText}
                    ,@Take = {take}
                    ,@Skip = {skip}
                """
            )
            .ToArrayAsync(cancellation);
    }

    internal async Task CreateSearchAsync(bool isCmds, CancellationToken cancellation)
    {
        await using var db = await context.CreateDbContextAsync(cancellation);

        db.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

        await db.Database.ExecuteSqlAsync($"exec assets.CreateSearch @IsCmds = {isCmds}", cancellation);
    }

    internal async Task<int> ProcessSearchQueueObjectAsync(CancellationToken cancellation)
    {
        await using var db = await context.CreateDbContextAsync(cancellation);

        db.Database.SetCommandTimeout(TimeSpan.FromMinutes(30));

        return (await db.Database
            .SqlQuery<int>($"exec assets.ProcessSearchQueueObject")
            .ToListAsync(cancellation))
            .Single();
            
    }

    internal async Task<int> ProcessSearchQueuePermissionAsync(CancellationToken cancellation)
    {
        await using var db = await context.CreateDbContextAsync(cancellation);

        db.Database.SetCommandTimeout(TimeSpan.FromMinutes(30));

        return (await db.Database
            .SqlQuery<int>($"exec assets.ProcessSearchQueuePermission")
            .ToListAsync(cancellation))
            .Single();
            
    }
}