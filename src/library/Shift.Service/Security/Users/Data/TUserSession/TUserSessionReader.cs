using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Security;

public class TUserSessionReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public TUserSessionReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(
        Guid session,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TUserSession
            .AnyAsync(x => x.SessionIdentifier == session, cancellation);
    }

    public async Task<TUserSessionEntity?> RetrieveAsync(
        Guid session,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TUserSession
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SessionIdentifier == session, cancellation);
    }

    public async Task<int> CountAsync(
        IUserSessionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<TUserSessionEntity>> CollectAsync(
        IUserSessionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<TUserSessionEntity>> DownloadAsync(
        IUserSessionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserSessionMatch>> SearchAsync(
        IUserSessionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<TUserSessionEntity> BuildQueryable(
        TableDbContext db,
        IUserSessionCriteria criteria)
    {
        var q = db.TUserSession
            .AsNoTracking()
            .AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // TODO: Implement search criteria

        // if (criteria.AuthenticationErrorType != null)
        //    query = query.Where(x => x.AuthenticationErrorType == criteria.AuthenticationErrorType);

        // if (criteria.AuthenticationErrorMessage != null)
        //    query = query.Where(x => x.AuthenticationErrorMessage == criteria.AuthenticationErrorMessage);

        // if (criteria.SessionCode != null)
        //    query = query.Where(x => x.SessionCode == criteria.SessionCode);

        // if (criteria.SessionIsAuthenticated != null)
        //    query = query.Where(x => x.SessionIsAuthenticated == criteria.SessionIsAuthenticated);

        // if (criteria.SessionStarted != null)
        //    query = query.Where(x => x.SessionStarted == criteria.SessionStarted);

        // if (criteria.SessionStopped != null)
        //    query = query.Where(x => x.SessionStopped == criteria.SessionStopped);

        // if (criteria.SessionMinutes != null)
        //    query = query.Where(x => x.SessionMinutes == criteria.SessionMinutes);

        // if (criteria.OrganizationIdentifier != null)
        //    query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        // if (criteria.UserAgent != null)
        //    query = query.Where(x => x.UserAgent == criteria.UserAgent);

        // if (criteria.UserBrowser != null)
        //    query = query.Where(x => x.UserBrowser == criteria.UserBrowser);

        // if (criteria.UserBrowserVersion != null)
        //    query = query.Where(x => x.UserBrowserVersion == criteria.UserBrowserVersion);

        // if (criteria.UserEmail != null)
        //    query = query.Where(x => x.UserEmail == criteria.UserEmail);

        // if (criteria.UserHostAddress != null)
        //    query = query.Where(x => x.UserHostAddress == criteria.UserHostAddress);

        // if (criteria.UserIdentifier != null)
        //    query = query.Where(x => x.UserIdentifier == criteria.UserIdentifier);

        // if (criteria.UserLanguage != null)
        //    query = query.Where(x => x.UserLanguage == criteria.UserLanguage);

        // if (criteria.SessionIdentifier != null)
        //    query = query.Where(x => x.SessionIdentifier == criteria.SessionIdentifier);

        // if (criteria.AuthenticationSource != null)
        //    query = query.Where(x => x.AuthenticationSource == criteria.AuthenticationSource);

        return q;
    }

    public static async Task<IEnumerable<UserSessionMatch>> ToMatchesAsync(
        IQueryable<TUserSessionEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new UserSessionMatch
            {
                SessionIdentifier = entity.SessionIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}