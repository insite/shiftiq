using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Security;

public class QOrganizationReader : QueryRunner, IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly QOrganizationAdapter _adapter;

    public QOrganizationReader(IDbContextFactory<TableDbContext> context, QOrganizationAdapter adapter)
    {
        _context = context;
        _adapter = adapter;

        RegisterQuery<TestOrganizationQuery>(q => Execute((TestOrganizationQuery)q));
    }

    public async Task<bool> AssertAsync(Guid organization, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QOrganization
            .AnyAsync(x => x.OrganizationIdentifier == organization, cancellation);
    }

    public async Task<QOrganizationEntity?> RetrieveAsync(Guid organization, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QOrganization
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrganizationIdentifier == organization, cancellation);
    }

    public async Task<int> CountAsync(IOrganizationCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QOrganizationEntity>> CollectAsync(IOrganizationCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<OrganizationMatch>> SearchAsync(IOrganizationCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QOrganizationEntity> BuildQueryable(TableDbContext db, IOrganizationCriteria criteria)
    {
        var query = db.QOrganization.AsNoTracking().AsQueryable();

        if (criteria.CompanyNameContains.IsNotEmpty())
            query = query.Where(x => x.CompanyName!.Contains(criteria.CompanyNameContains));

        if (criteria.OrganizationCode.IsNotEmpty())
            query = query.Where(x => x.OrganizationCode == criteria.OrganizationCode);

        if (criteria.Filter?.Sort.NullIfEmpty() == null)
            query = query.OrderBy(x => x.CompanyName);
        else
            query = query.OrderBy(criteria.Filter.Sort);

        return query;
    }

    public static async Task<IEnumerable<OrganizationMatch>> ToMatchesAsync(
        IQueryable<QOrganizationEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new OrganizationMatch
            {
                OrganizationIdentifier = entity.OrganizationIdentifier,
                ParentOrganizationIdentifier = entity.ParentOrganizationIdentifier,
                CompanyName = entity.CompanyName
            })
            .ToListAsync(cancellation);

        return matches;
    }

    public OrganizationMatch[] Execute(TestOrganizationQuery query)
    {
        using var db = _context.CreateDbContext();

        var matches = db.QOrganization
          .AsNoTracking()
          .AsQueryable();

        return matches.Select(x => new OrganizationMatch
        {
            OrganizationIdentifier = x.OrganizationIdentifier,
            ParentOrganizationIdentifier = x.ParentOrganizationIdentifier,
            CompanyName = x.CompanyName
        }).ToArray();
    }
}