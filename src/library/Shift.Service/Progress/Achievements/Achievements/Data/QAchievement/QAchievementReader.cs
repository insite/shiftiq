using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Achievement;

public class QAchievementReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly QAchievementAdapter _adapter;
    private readonly IShiftIdentityService _identity;
    public QAchievementReader(IDbContextFactory<TableDbContext> context, QAchievementAdapter adapter, IShiftIdentityService identity)
    {
        _context = context;
        _adapter = adapter;
        _identity = identity;
    }

    public async Task<bool> AssertAsync(Guid achievement, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QAchievement
            .AnyAsync(x => x.AchievementIdentifier == achievement, cancellation);
    }

    public async Task<QAchievementEntity?> RetrieveAsync(Guid achievement, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QAchievement
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AchievementIdentifier == achievement, cancellation);
    }

    public async Task<int> CountAsync(IAchievementCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QAchievementEntity>> CollectAsync(IAchievementCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<AchievementMatch>> SearchAsync(IAchievementCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QAchievementEntity> BuildQueryable(TableDbContext db, IAchievementCriteria criteria)
    {
        var query = db.QAchievement.AsNoTracking().AsQueryable();

        criteria.OrganizationIdentifier = _identity.OrganizationId;

        if (criteria.OrganizationIdentifier != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        // if (criteria.AchievementLabel != null)
        //    query = query.Where(x => x.AchievementLabel == criteria.AchievementLabel);

        if (criteria.AchievementTitle != null)
            query = query.Where(x => x.AchievementTitle.Contains(criteria.AchievementTitle));

        // if (criteria.AchievementDescription != null)
        //    query = query.Where(x => x.AchievementDescription == criteria.AchievementDescription);

        // if (criteria.AchievementIsEnabled != null)
        //    query = query.Where(x => x.AchievementIsEnabled == criteria.AchievementIsEnabled);

        // if (criteria.ExpirationType != null)
        //    query = query.Where(x => x.ExpirationType == criteria.ExpirationType);

        // if (criteria.ExpirationFixedDate != null)
        //    query = query.Where(x => x.ExpirationFixedDate == criteria.ExpirationFixedDate);

        // if (criteria.ExpirationLifetimeQuantity != null)
        //    query = query.Where(x => x.ExpirationLifetimeQuantity == criteria.ExpirationLifetimeQuantity);

        // if (criteria.ExpirationLifetimeUnit != null)
        //    query = query.Where(x => x.ExpirationLifetimeUnit == criteria.ExpirationLifetimeUnit);

        // if (criteria.CertificateLayoutCode != null)
        //    query = query.Where(x => x.CertificateLayoutCode == criteria.CertificateLayoutCode);

        // if (criteria.AchievementType != null)
        //    query = query.Where(x => x.AchievementType == criteria.AchievementType);

        // if (criteria.AchievementReportingDisabled != null)
        //    query = query.Where(x => x.AchievementReportingDisabled == criteria.AchievementReportingDisabled);

        // if (criteria.BadgeImageUrl != null)
        //    query = query.Where(x => x.BadgeImageUrl == criteria.BadgeImageUrl);

        // if (criteria.HasBadgeImage != null)
        //    query = query.Where(x => x.HasBadgeImage == criteria.HasBadgeImage);

        return query;
    }

    public static async Task<IEnumerable<AchievementMatch>> ToMatchesAsync(
        IQueryable<QAchievementEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new AchievementMatch
            {
                AchievementId = entity.AchievementIdentifier,
                AchievementTitle = entity.AchievementTitle
            })
            .ToListAsync(cancellation);

        return matches;
    }
}