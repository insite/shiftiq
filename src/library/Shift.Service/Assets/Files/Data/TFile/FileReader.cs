using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Content;

public class FileReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private string DefaultSort = "FileUploaded DESC, FileIdentifier";

    public FileReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid file, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.FileIdentifier == file && (organization == null || organization == x.OrganizationIdentifier), cancellation);

        }, cancellation);
    }

    public Task<List<FileEntity>> CollectAsync(IFileCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

        }, cancellation);
    }

    public Task<int> CountAsync(IFileCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<FileEntity> DownloadAsync(IFileCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<FileEntity?> RetrieveAsync(Guid file, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.FileIdentifier == file, cancellation);

        }, cancellation);
    }

    public Task<List<FileMatch>> SearchAsync(IFileCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            query = query
                .OrderBy(!string.IsNullOrEmpty(criteria.Filter.Sort) ? criteria.Filter.Sort : DefaultSort)
                .ApplyPaging(criteria.Filter);

            return ToMatchesAsync(query, cancellation);

        }, cancellation);
    }

    /// <summary>
    /// Creates a queryable for events
    /// </summary>
    /// <remarks>
    /// If you call .Include() on the DbSet then remember to use .AsSplitQuery() so that cartesian explosion is avoided.
    /// When using split queries with Skip/Take on EF versions prior to 10, pay special attention to make your query
    /// ordering fully unique, otherwise the result set is non-deterministic.
    /// </remarks>
    private IQueryable<FileEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.TFile
            .AsNoTracking();

        return query;
    }

    private IQueryable<FileEntity> BuildQueryable(TableDbContext db, IFileCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId.Value);

        if (criteria.UserId.HasValue)
            query = query.Where(x => x.UserIdentifier == criteria.UserId.Value);

        if (criteria.FileIds != null && criteria.FileIds.Length > 0)
            query = query.Where(x => criteria.FileIds.Contains(x.FileIdentifier));

        if (criteria.ObjectTypeExact.IsNotEmpty())
            query = query.Where(x => x.ObjectType == criteria.ObjectTypeExact!);

        if (criteria.ObjectId.HasValue)
            query = query.Where(x => x.ObjectIdentifier == criteria.ObjectId.Value);

        if (criteria.ObjectIdentifierContains.IsNotEmpty())
            query = query.Where(x => x.ObjectIdentifier.ToString().Contains(criteria.ObjectIdentifierContains!));

        if (criteria.FileNameContains.IsNotEmpty())
            query = query.Where(x => x.FileName.Contains(criteria.FileNameContains!));

        if (criteria.FileUploadedSince.HasValue)
            query = query.Where(x => x.FileUploaded >= criteria.FileUploadedSince.Value);

        if (criteria.FileUploadedBefore.HasValue)
            query = query.Where(x => x.FileUploaded < criteria.FileUploadedBefore.Value);

        if (criteria.DocumentNameContains.IsNotEmpty())
            query = query.Where(x => x.DocumentName.Contains(criteria.DocumentNameContains!));

        if (criteria.HasClaims.HasValue)
        {
            if (criteria.HasClaims.Value)
                query = query.Where(x => x.Claims.Any());
            else
                query = query.Where(x => !x.Claims.Any());
        }

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<FileMatch>> ToMatchesAsync(IQueryable<FileEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new FileMatch
            {
                OrganizationId = entity.OrganizationIdentifier,
                OrganizationCode = entity.Organization.OrganizationCode,
                ObjectType = entity.ObjectType,
                ObjectId = entity.ObjectIdentifier,
                FileId = entity.FileIdentifier,
                FileLocation = entity.FileLocation,
                FileName = entity.FileName,
                DocumentName = entity.DocumentName,
                FileSize = entity.FileSize,
                FileUploaded = entity.FileUploaded,
                UserId = entity.UserIdentifier,
                UserFullName = entity.User.FullName,
                HasClaims = entity.Claims.Any()
            })
            .ToListAsync(cancellation);

        return matches;
    }
}