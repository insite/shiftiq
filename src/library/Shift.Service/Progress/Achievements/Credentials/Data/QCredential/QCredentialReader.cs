using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Progress;

public class QCredentialReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QCredentialReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(Guid credential, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QCredential
            .AnyAsync(x => x.CredentialIdentifier == credential, cancellation);
    }

    public async Task<QCredentialEntity?> RetrieveAsync(Guid credential, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QCredential
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CredentialIdentifier == credential, cancellation);
    }

    public async Task<int> CountAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QCredentialEntity>> CollectAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<CredentialMatch>> SearchAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QCredentialEntity> BuildQueryable(TableDbContext db, ICredentialCriteria criteria)
    {
        var q = db.QCredential.AsNoTracking().AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // TODO: Implement search criteria

        // if (criteria.AchievementIdentifier != null)
        //    query = query.Where(x => x.AchievementIdentifier == criteria.AchievementIdentifier);

        // if (criteria.UserIdentifier != null)
        //    query = query.Where(x => x.UserIdentifier == criteria.UserIdentifier);

        // if (criteria.CredentialGranted != null)
        //    query = query.Where(x => x.CredentialGranted == criteria.CredentialGranted);

        // if (criteria.CredentialRevoked != null)
        //    query = query.Where(x => x.CredentialRevoked == criteria.CredentialRevoked);

        // if (criteria.CredentialExpired != null)
        //    query = query.Where(x => x.CredentialExpired == criteria.CredentialExpired);

        // if (criteria.ExpirationType != null)
        //    query = query.Where(x => x.ExpirationType == criteria.ExpirationType);

        // if (criteria.ExpirationFixedDate != null)
        //    query = query.Where(x => x.ExpirationFixedDate == criteria.ExpirationFixedDate);

        // if (criteria.ExpirationLifetimeQuantity != null)
        //    query = query.Where(x => x.ExpirationLifetimeQuantity == criteria.ExpirationLifetimeQuantity);

        // if (criteria.ExpirationLifetimeUnit != null)
        //    query = query.Where(x => x.ExpirationLifetimeUnit == criteria.ExpirationLifetimeUnit);

        // if (criteria.CredentialAssigned != null)
        //    query = query.Where(x => x.CredentialAssigned == criteria.CredentialAssigned);

        // if (criteria.CredentialStatus != null)
        //    query = query.Where(x => x.CredentialStatus == criteria.CredentialStatus);

        // if (criteria.CredentialReminderType != null)
        //    query = query.Where(x => x.CredentialReminderType == criteria.CredentialReminderType);

        // if (criteria.AuthorityName != null)
        //    query = query.Where(x => x.AuthorityName == criteria.AuthorityName);

        // if (criteria.AuthorityLocation != null)
        //    query = query.Where(x => x.AuthorityLocation == criteria.AuthorityLocation);

        // if (criteria.AuthorityReference != null)
        //    query = query.Where(x => x.AuthorityReference == criteria.AuthorityReference);

        // if (criteria.CredentialDescription != null)
        //    query = query.Where(x => x.CredentialDescription == criteria.CredentialDescription);

        // if (criteria.CredentialHours != null)
        //    query = query.Where(x => x.CredentialHours == criteria.CredentialHours);

        // if (criteria.CredentialExpirationExpected != null)
        //    query = query.Where(x => x.CredentialExpirationExpected == criteria.CredentialExpirationExpected);

        // if (criteria.CredentialExpirationReminderRequested0 != null)
        //    query = query.Where(x => x.CredentialExpirationReminderRequested0 == criteria.CredentialExpirationReminderRequested0);

        // if (criteria.CredentialExpirationReminderRequested1 != null)
        //    query = query.Where(x => x.CredentialExpirationReminderRequested1 == criteria.CredentialExpirationReminderRequested1);

        // if (criteria.CredentialExpirationReminderRequested2 != null)
        //    query = query.Where(x => x.CredentialExpirationReminderRequested2 == criteria.CredentialExpirationReminderRequested2);

        // if (criteria.CredentialExpirationReminderRequested3 != null)
        //    query = query.Where(x => x.CredentialExpirationReminderRequested3 == criteria.CredentialExpirationReminderRequested3);

        // if (criteria.CredentialExpirationReminderDelivered0 != null)
        //    query = query.Where(x => x.CredentialExpirationReminderDelivered0 == criteria.CredentialExpirationReminderDelivered0);

        // if (criteria.CredentialExpirationReminderDelivered1 != null)
        //    query = query.Where(x => x.CredentialExpirationReminderDelivered1 == criteria.CredentialExpirationReminderDelivered1);

        // if (criteria.CredentialExpirationReminderDelivered2 != null)
        //    query = query.Where(x => x.CredentialExpirationReminderDelivered2 == criteria.CredentialExpirationReminderDelivered2);

        // if (criteria.CredentialExpirationReminderDelivered3 != null)
        //    query = query.Where(x => x.CredentialExpirationReminderDelivered3 == criteria.CredentialExpirationReminderDelivered3);

        // if (criteria.CredentialIdentifier != null)
        //    query = query.Where(x => x.CredentialIdentifier == criteria.CredentialIdentifier);

        // if (criteria.CredentialNecessity != null)
        //    query = query.Where(x => x.CredentialNecessity == criteria.CredentialNecessity);

        // if (criteria.CredentialPriority != null)
        //    query = query.Where(x => x.CredentialPriority == criteria.CredentialPriority);

        // if (criteria.AuthorityIdentifier != null)
        //    query = query.Where(x => x.AuthorityIdentifier == criteria.AuthorityIdentifier);

        // if (criteria.AuthorityType != null)
        //    query = query.Where(x => x.AuthorityType == criteria.AuthorityType);

        // if (criteria.CredentialRevokedReason != null)
        //    query = query.Where(x => x.CredentialRevokedReason == criteria.CredentialRevokedReason);

        // if (criteria.CredentialGrantedDescription != null)
        //    query = query.Where(x => x.CredentialGrantedDescription == criteria.CredentialGrantedDescription);

        // if (criteria.CredentialGrantedScore != null)
        //    query = query.Where(x => x.CredentialGrantedScore == criteria.CredentialGrantedScore);

        // if (criteria.CredentialRevokedScore != null)
        //    query = query.Where(x => x.CredentialRevokedScore == criteria.CredentialRevokedScore);

        // if (criteria.TransactionHash != null)
        //    query = query.Where(x => x.TransactionHash == criteria.TransactionHash);

        // if (criteria.PublisherAddress != null)
        //    query = query.Where(x => x.PublisherAddress == criteria.PublisherAddress);

        // if (criteria.PublicationStatus != null)
        //    query = query.Where(x => x.PublicationStatus == criteria.PublicationStatus);

        // if (criteria.OrganizationIdentifier != null)
        //    query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        // if (criteria.EmployerGroupIdentifier != null)
        //    query = query.Where(x => x.EmployerGroupIdentifier == criteria.EmployerGroupIdentifier);

        // if (criteria.EmployerGroupStatus != null)
        //    query = query.Where(x => x.EmployerGroupStatus == criteria.EmployerGroupStatus);

        return q;
    }

    public static async Task<IEnumerable<CredentialMatch>> ToMatchesAsync(
        IQueryable<QCredentialEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new CredentialMatch
            {
                CredentialIdentifier = entity.CredentialIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}