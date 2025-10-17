using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Directory;

public interface IPersonReader : IEntityReader
{
    Task<bool> AssertAsync(Guid person, CancellationToken cancellation = default);
    Task<PersonEntity?> RetrieveAsync(Guid person, CancellationToken cancellation = default);
    Task<int> CountAsync(IPersonCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<PersonEntity>> CollectAsync(IPersonCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<PersonEntity>> DownloadAsync(IPersonCriteria criteria, CancellationToken cancellation = default);
    Task<IEnumerable<PersonMatch>> SearchAsync(IPersonCriteria criteria, CancellationToken cancellation = default);
}

internal class PersonReader : IPersonReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly IShiftIdentityService _identityService;

    public PersonReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<bool> AssertAsync(
        Guid person,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QPerson
            .AnyAsync(x => x.PersonIdentifier == person, cancellation);
    }

    public async Task<PersonEntity?> RetrieveAsync(
        Guid person,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QPerson
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PersonIdentifier == person, cancellation);
    }

    public async Task<int> CountAsync(
        IPersonCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<PersonEntity>> CollectAsync(
        IPersonCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<PersonEntity>> DownloadAsync(
        IPersonCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<PersonMatch>> SearchAsync(
        IPersonCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort ?? "User.FullName")
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<PersonEntity> BuildQueryable(
        TableDbContext db,
        IPersonCriteria criteria)
    {
        var q = db.QPerson
            .Include(x => x.User!.Events)
            .AsNoTracking()
            .AsQueryable();

        var organizationId = criteria.OrganizationIdentifier ?? _identityService.OrganizationId;

        q = q.Where(x => x.OrganizationIdentifier == organizationId);

        if (!string.IsNullOrEmpty(criteria.EmailExact))
            q = q.Where(x => x.User!.Email == criteria.EmailExact);

        if (criteria.EventRole != null)
            q = q.Where(x => x.User!.Events.Any(e => e.OrganizationIdentifier == x.OrganizationIdentifier && e.AttendeeRole == criteria.EventRole));

        if (!string.IsNullOrEmpty(criteria.FullName))
            q = q.Where(x => x.User!.FullName.Contains(criteria.FullName));

        if (criteria.UserIdentifier != null)
            q = q.Where(x => x.UserIdentifier == criteria.UserIdentifier);

        return q;
    }

    public static async Task<IEnumerable<PersonMatch>> ToMatchesAsync(
        IQueryable<PersonEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PersonMatch
            {
                PersonId = entity.PersonIdentifier,
                UserId = entity.UserIdentifier,

                UserEmail = entity.User != null ? entity.User.Email : "-",
                UserName = entity.User != null ? entity.User.FullName : "-",

                IsAdministrator = entity.IsAdministrator,
                IsDeveloper = entity.IsDeveloper,
                IsOperator = entity.IsOperator,

                TimeZone = entity.User!.TimeZone
            })
            .ToListAsync(cancellation);

        return matches;
    }
}