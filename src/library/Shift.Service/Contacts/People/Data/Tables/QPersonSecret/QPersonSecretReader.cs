using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Directory;

public class QPersonSecretReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QPersonSecretReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(
        Guid secret,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QPersonSecret
            .AnyAsync(x => x.SecretIdentifier == secret, cancellation);
    }

    public async Task<QPersonSecretEntity?> RetrieveAsync(
        Guid secret,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QPersonSecret
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SecretIdentifier == secret, cancellation);
    }

    public async Task<int> CountAsync(
        IPersonSecretCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QPersonSecretEntity>> CollectAsync(
        IPersonSecretCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<QPersonSecretEntity>> DownloadAsync(
        IPersonSecretCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<PersonSecretMatch>> SearchAsync(
        IPersonSecretCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QPersonSecretEntity> BuildQueryable(
        TableDbContext db,
        IPersonSecretCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = db.QPersonSecret
            .AsNoTracking()
            .AsQueryable();

        if (criteria.PersonId.HasValue)
            query = query.Where(x => x.PersonIdentifier == criteria.PersonId.Value);

        if (!string.IsNullOrEmpty(criteria.SecretName))
            query = query.Where(x => x.SecretName == criteria.SecretName);

        if (criteria.SecretExpirySince.HasValue)
            query = query.Where(x => x.SecretExpiry >= criteria.SecretExpirySince);

        if (criteria.SecretExpiryBefore.HasValue)
            query = query.Where(x => x.SecretExpiry < criteria.SecretExpiryBefore);

        return query;
    }

    public static async Task<IEnumerable<PersonSecretMatch>> ToMatchesAsync(
        IQueryable<QPersonSecretEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PersonSecretMatch
            {
                SecretId = entity.SecretIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}