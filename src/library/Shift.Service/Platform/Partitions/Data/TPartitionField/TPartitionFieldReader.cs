using Microsoft.EntityFrameworkCore;

using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Security;

public class TPartitionFieldReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly TPartitionFieldAdapter _adapter;

    public TPartitionFieldReader(IDbContextFactory<TableDbContext> context, TPartitionFieldAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    public async Task<bool> AssertAsync(Guid setting, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TPartitionSetting
            .AnyAsync(x => x.SettingIdentifier == setting, cancellation);
    }

    public async Task<TPartitionFieldEntity?> RetrieveAsync(Guid setting, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TPartitionSetting
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SettingIdentifier == setting, cancellation);
    }

    public async Task<int> CountAsync(IPartitionFieldCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQuery(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<TPartitionFieldEntity>> CollectAsync(IPartitionFieldCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQuery(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<PartitionFieldMatch>> SearchAsync(IPartitionFieldCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entities = await BuildQuery(db, criteria)
            .ToListAsync(cancellation);

        return _adapter.ToMatch(entities);
    }

    private IQueryable<TPartitionFieldEntity> BuildQuery(TableDbContext db, IPartitionFieldCriteria criteria)
    {
        var query = db.TPartitionSetting.AsNoTracking().AsQueryable();

        // TODO: Implement search criteria

        // if (criteria.SettingIdentifier != null)
        //    query = query.Where(x => x.SettingIdentifier == criteria.SettingIdentifier);

        // if (criteria.SettingName != null)
        //    query = query.Where(x => x.SettingName == criteria.SettingName);

        // if (criteria.SettingValue != null)
        //    query = query.Where(x => x.SettingValue == criteria.SettingValue);

        if (criteria.Filter != null)
        {
            query = query
                .Skip((criteria.Filter.Page - 1) * criteria.Filter.PageSize)
                .Take(criteria.Filter.PageSize);
        }

        return query;
    }
}