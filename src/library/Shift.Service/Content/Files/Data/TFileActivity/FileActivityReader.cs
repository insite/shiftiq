using Microsoft.EntityFrameworkCore;

using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Content;

public interface IFileActivityReader : IEntityReader
{
    Task<bool> AssertAsync(Guid activity, CancellationToken cancellation = default);
    Task<FileActivityEntity?> RetrieveAsync(Guid activity, CancellationToken cancellation = default);
    Task<int> CountAsync(IFileActivityCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<FileActivityEntity>> CollectAsync(IFileActivityCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<FileActivityEntity>> DownloadAsync(IFileActivityCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<FileActivityMatch>> SearchAsync(IFileActivityCriteria criteria, CancellationToken cancellation = default);
}

internal class FileActivityReader : IFileActivityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FileActivityReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(
        Guid activity,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TFileActivity
            .AnyAsync(x => x.ActivityIdentifier == activity, cancellation);
    }

    public async Task<FileActivityEntity?> RetrieveAsync(
        Guid activity,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TFileActivity
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ActivityIdentifier == activity, cancellation);
    }

    public async Task<int> CountAsync(
        IFileActivityCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<FileActivityEntity>> CollectAsync(
        IFileActivityCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<FileActivityEntity>> DownloadAsync(
        IFileActivityCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<FileActivityMatch>> SearchAsync(
        IFileActivityCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await ToMatchesAsync(BuildQueryable(db, criteria), cancellation);
    }

    private IQueryable<FileActivityEntity> BuildQueryable(
        TableDbContext db,
        IFileActivityCriteria criteria)
    {
        var q = db.TFileActivity
            .AsNoTracking()
            .AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // if (criteria.FileIdentifier != null)
        //    q = q.Where(x => x.FileIdentifier == criteria.FileIdentifier);

        // if (criteria.UserIdentifier != null)
        //    q = q.Where(x => x.UserIdentifier == criteria.UserIdentifier);

        // if (criteria.ActivityIdentifier != null)
        //    q = q.Where(x => x.ActivityIdentifier == criteria.ActivityIdentifier);

        // if (criteria.ActivityTime != null)
        //    q = q.Where(x => x.ActivityTime == criteria.ActivityTime);

        // if (criteria.ActivityChanges != null)
        //    q = q.Where(x => x.ActivityChanges == criteria.ActivityChanges);

        return q;
    }

    public static async Task<IEnumerable<FileActivityMatch>> ToMatchesAsync(
        IQueryable<FileActivityEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new FileActivityMatch
            {
                ActivityIdentifier = entity.ActivityIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}