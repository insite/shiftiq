using Microsoft.EntityFrameworkCore;

using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Security;

public class TUserFieldReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly TUserFieldAdapter _adapter;

    public TUserFieldReader(IDbContextFactory<TableDbContext> context, TUserFieldAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    public async Task<bool> AssertAsync(Guid setting, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        return await db.TUserSetting
            .AnyAsync(x => x.SettingIdentifier == setting, cancellation);
    }

    public async Task<TUserFieldEntity?> RetrieveAsync(Guid setting, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        return await db.TUserSetting
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SettingIdentifier == setting, cancellation);
    }

    public async Task<int> CountAsync(IUserFieldCriteria criteria, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        return await BuildQuery(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<TUserFieldEntity>> CollectAsync(IUserFieldCriteria criteria, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        return await BuildQuery(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserFieldMatch>> SearchAsync(IUserFieldCriteria criteria, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var entities = await BuildQuery(db, criteria)
            .ToListAsync(cancellation);

        return _adapter.ToMatch(entities);
    }

    private IQueryable<TUserFieldEntity> BuildQuery(TableDbContext db, IUserFieldCriteria criteria)
    {
        var query = db.TUserSetting.AsNoTracking().AsQueryable();

        // TODO: Implement search criteria

        // if (criteria.OrganizationIdentifier != null)
        //    query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        // if (criteria.UserIdentifier != null)
        //    query = query.Where(x => x.UserIdentifier == criteria.UserIdentifier);

        // if (criteria.Name != null)
        //    query = query.Where(x => x.Name == criteria.Name);

        // if (criteria.ValueType != null)
        //    query = query.Where(x => x.ValueType == criteria.ValueType);

        // if (criteria.ValueJson != null)
        //    query = query.Where(x => x.ValueJson == criteria.ValueJson);

        // if (criteria.SettingIdentifier != null)
        //    query = query.Where(x => x.SettingIdentifier == criteria.SettingIdentifier);

        if (criteria.Filter != null)
        {
            query = query
                .Skip((criteria.Filter.Page - 1) * criteria.Filter.PageSize)
                .Take(criteria.Filter.PageSize);
        }

        return query;
    }
}