using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Content;

public class TInputReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly TInputAdapter _adapter;

    public TInputReader(IDbContextFactory<TableDbContext> context, TInputAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    public async Task<bool> AssertAsync(Guid content, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TInput
            .AnyAsync(x => x.ContentIdentifier == content, cancellation);
    }

    public async Task<TInputEntity?> RetrieveAsync(Guid content, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TInput
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ContentIdentifier == content, cancellation);
    }

    public async Task<int> CountAsync(IInputCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<TInputEntity>> CollectAsync(IInputCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<InputMatch>> SearchAsync(IInputCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<TInputEntity> BuildQueryable(
        TableDbContext db,
        IInputCriteria criteria)
    {
        var q = db.TInput.AsNoTracking().AsQueryable();

        if (criteria.ContainerIdentifier != null)
            q = q.Where(x => x.ContainerIdentifier == criteria.ContainerIdentifier);

        if (!string.IsNullOrEmpty(criteria.ContainerType))
            q = q.Where(x => x.ContainerType == criteria.ContainerType);

        if (!string.IsNullOrEmpty(criteria.ContentLabel))
            q = q.Where(x => x.ContentLabel == criteria.ContentLabel);

        if (!string.IsNullOrEmpty(criteria.ContentLanguage))
            q = q.Where(x => x.ContentLanguage == criteria.ContentLanguage);

        return q;
    }

    public static async Task<IEnumerable<InputMatch>> ToMatchesAsync(
        IQueryable<TInputEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new InputMatch
            {
                ContentIdentifier = entity.ContentIdentifier
            })
            .ToListAsync(cancellation);

        return matches;
    }
}