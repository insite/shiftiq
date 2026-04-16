using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace Shift.Service.Metadata.Sequences.Data;

public class Sequence(IDbContextFactory<TableDbContext> context) : ISequence
{
    public async Task<int> IncrementAsync(Guid organizationId, SequenceType type, int startNumber = 1)
    {
        return (await IncrementManyAsync(organizationId, type, 1, startNumber)).First();
    }

    public async Task<int[]> IncrementManyAsync(Guid organizationId, SequenceType type, int count, int startNumber = 1)
    {
        const string query = "settings.IncrementSequence @OrganizationIdentifier, @SequenceType, @SequenceCount, @StartNumber";

        if (startNumber <= 0)
            startNumber = 1;

        using var db = context.CreateDbContext();

        var sequences = await db.Database
            .SqlQueryRaw<int>(query, new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId),
                new SqlParameter("@SequenceType", type.ToString()),
                new SqlParameter("@SequenceCount", count),
                new SqlParameter("@StartNumber", startNumber)
            })
            .ToListAsync();

        return sequences.OrderBy(x => x).ToArray();
    }
}
