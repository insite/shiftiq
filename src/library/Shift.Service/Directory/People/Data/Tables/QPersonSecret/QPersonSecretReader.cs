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
        var q = db.QPersonSecret
            .AsNoTracking()
            .AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // TODO: Implement search criteria

        // if (criteria.PersonIdentifier != null)
        //    query = query.Where(x => x.PersonIdentifier == criteria.PersonIdentifier);

        // if (criteria.SecretIdentifier != null)
        //    query = query.Where(x => x.SecretIdentifier == criteria.SecretIdentifier);

        // if (criteria.SecretType != null)
        //    query = query.Where(x => x.SecretType == criteria.SecretType);

        // if (criteria.SecretName != null)
        //    query = query.Where(x => x.SecretName == criteria.SecretName);

        // if (criteria.SecretExpiry != null)
        //    query = query.Where(x => x.SecretExpiry == criteria.SecretExpiry);

        // if (criteria.SecretLifetimeLimit != null)
        //    query = query.Where(x => x.SecretLifetimeLimit == criteria.SecretLifetimeLimit);

        // if (criteria.SecretValue != null)
        //    query = query.Where(x => x.SecretValue == criteria.SecretValue);

        return q;
    }

    public static async Task<IEnumerable<PersonSecretMatch>> ToMatchesAsync(
        IQueryable<QPersonSecretEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PersonSecretMatch
            {
                SecretIdentifier = entity.SecretIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}