using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Content;

public interface IFileReader : IEntityReader
{
    Task<bool> AssertAsync(Guid file, CancellationToken cancellation = default);
    Task<FileEntity?> RetrieveAsync(Guid file, CancellationToken cancellation = default);
    Task<int> CountAsync(IFileCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<FileEntity>> CollectAsync(IFileCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<FileEntity>> DownloadAsync(IFileCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<FileMatch>> SearchAsync(IFileCriteria criteria, CancellationToken cancellation = default);
}

public class FileReader : IFileReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FileReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(
        Guid file,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TFile
            .AnyAsync(x => x.FileIdentifier == file, cancellation);
    }

    public async Task<FileEntity?> RetrieveAsync(
        Guid file,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TFile
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FileIdentifier == file, cancellation);
    }

    public async Task<int> CountAsync(
        IFileCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<FileEntity>> CollectAsync(
        IFileCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<FileEntity>> DownloadAsync(
        IFileCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .Include(x => x.Organization)
            .Include(x => x.User)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<FileMatch>> SearchAsync(
        IFileCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<FileEntity> BuildQueryable(
        TableDbContext db,
        IFileCriteria criteria)
    {
        var q = db.TFile
            .AsNoTracking()
            .AsQueryable();

        if (criteria.UserIdentifier.HasValue)
            q = q.Where(x => x.UserIdentifier == criteria.UserIdentifier.Value);

        if (criteria.OrganizationIdentifier.HasValue)
            q = q.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier.Value);

        if (criteria.ObjectTypeExact.IsNotEmpty())
            q = q.Where(x => x.ObjectType == criteria.ObjectTypeExact!);

        if (criteria.ObjectIdentifier.HasValue)
            q = q.Where(x => x.ObjectIdentifier == criteria.ObjectIdentifier.Value);

        if (criteria.ObjectIdentifierContains.IsNotEmpty())
            q = q.Where(x => x.ObjectIdentifier.ToString().Contains(criteria.ObjectIdentifierContains!));

        if (criteria.FileNameContains.IsNotEmpty())
            q = q.Where(x => x.FileName.Contains(criteria.FileNameContains!));

        if (criteria.FileUploadedSince.HasValue)
            q = q.Where(x => x.FileUploaded >= criteria.FileUploadedSince.Value);

        if (criteria.FileUploadedBefore.HasValue)
            q = q.Where(x => x.FileUploaded < criteria.FileUploadedBefore.Value);

        if (criteria.DocumentNameContains.IsNotEmpty())
            q = q.Where(x => x.DocumentName.Contains(criteria.DocumentNameContains!));

        if (criteria.HasClaims.HasValue)
        {
            if (criteria.HasClaims.Value)
                q = q.Where(x => x.Claims.Any());
            else
                q = q.Where(x => !x.Claims.Any());
        }

        if (criteria.Filter?.Sort.NullIfEmpty() == null)
            q = q.OrderByDescending(x => x.FileUploaded);
        else
            q = q.OrderBy(criteria.Filter.Sort);

        return q;
    }

    public static async Task<IEnumerable<FileMatch>> ToMatchesAsync(
        IQueryable<FileEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new FileMatch
            {
                OrganizationIdentifier = entity.OrganizationIdentifier,
                OrganizationCode = entity.Organization.OrganizationCode,
                ObjectType = entity.ObjectType,
                ObjectIdentifier = entity.ObjectIdentifier,
                FileIdentifier = entity.FileIdentifier,
                FileLocation = entity.FileLocation,
                FileName = entity.FileName,
                DocumentName = entity.DocumentName,
                FileSize = entity.FileSize,
                FileUploaded = entity.FileUploaded,
                UserIdentifier = entity.UserIdentifier,
                UserFullName = entity.User.FullName,
                HasClaims = entity.Claims.Any()
            })
            .ToListAsync(cancellation);

        return matches;
    }
}