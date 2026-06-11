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
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = db.TUserSetting.AsNoTracking().AsQueryable();

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.Filter != null)
        {
            query = query
                .Skip((criteria.Filter.Page - 1) * criteria.Filter.PageSize)
                .Take(criteria.Filter.PageSize);
        }

        return query;
    }
}