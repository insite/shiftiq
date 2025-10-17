using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Content;

public interface IFileClaimReader : IEntityReader
{
    Task<bool> AssertAsync(Guid claim, CancellationToken cancellation = default);
    Task<FileClaimEntity?> RetrieveAsync(Guid claim, CancellationToken cancellation = default);
    Task<int> CountAsync(IFileClaimCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<FileClaimEntity>> CollectAsync(IFileClaimCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<FileClaimEntity>> DownloadAsync(IFileClaimCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<FileClaimMatch>> SearchAsync(IFileClaimCriteria criteria, CancellationToken cancellation = default);
}

internal class FileClaimReader : IFileClaimReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FileClaimReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(
        Guid claim,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TFileClaim
            .AnyAsync(x => x.ClaimIdentifier == claim, cancellation);
    }

    public async Task<FileClaimEntity?> RetrieveAsync(
        Guid claim,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TFileClaim
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClaimIdentifier == claim, cancellation);
    }

    public async Task<int> CountAsync(
        IFileClaimCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<FileClaimEntity>> CollectAsync(
        IFileClaimCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<FileClaimEntity>> DownloadAsync(
        IFileClaimCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<FileClaimMatch>> SearchAsync(
        IFileClaimCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<FileClaimEntity> BuildQueryable(
        TableDbContext db,
        IFileClaimCriteria criteria)
    {
        var q = db.TFileClaim
            .AsNoTracking()
            .AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // if (criteria.FileIdentifier != null)
        //    q = q.Where(x => x.FileIdentifier == criteria.FileIdentifier);

        // if (criteria.ClaimIdentifier != null)
        //    q = q.Where(x => x.ClaimIdentifier == criteria.ClaimIdentifier);

        // if (criteria.ObjectType != null)
        //    q = q.Where(x => x.ObjectType == criteria.ObjectType);

        // if (criteria.ObjectIdentifier != null)
        //    q = q.Where(x => x.ObjectIdentifier == criteria.ObjectIdentifier);

        // if (criteria.ClaimGranted != null)
        //    q = q.Where(x => x.ClaimGranted == criteria.ClaimGranted);

        return q;
    }

    public static async Task<IEnumerable<FileClaimMatch>> ToMatchesAsync(
        IQueryable<FileClaimEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new FileClaimMatch
            {
                ClaimIdentifier = entity.ClaimIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}