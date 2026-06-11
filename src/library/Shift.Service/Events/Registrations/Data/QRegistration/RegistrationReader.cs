using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Booking;

public class RegistrationReader : IEntityReader
{
    private string DefaultSort = "RegistrationIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    public RegistrationReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid registration, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.RegistrationIdentifier == registration && (organization == null || organization == x.OrganizationIdentifier), cancellation);

        }, cancellation);
    }

    public Task<List<RegistrationEntity>> CollectAsync(IRegistrationCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IRegistrationCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<RegistrationEntity> DownloadAsync(IRegistrationCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<RegistrationEntity?> RetrieveAsync(Guid registration, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.RegistrationIdentifier == registration, cancellation);

        }, cancellation);
    }

    public Task<List<RegistrationMatch>> SearchAsync(IRegistrationCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            query = query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
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
    private IQueryable<RegistrationEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.QRegistration
            .AsNoTracking();

        return query;
    }

    private IQueryable<RegistrationEntity> BuildQueryable(TableDbContext db, IRegistrationCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId.Value);

        if (criteria.AttendanceTakenSince.HasValue)
            query = query.Where(x => x.AttendanceTaken >= criteria.AttendanceTakenSince);

        if (criteria.AttendanceTakenBefore.HasValue)
            query = query.Where(x => x.AttendanceTaken < criteria.AttendanceTakenBefore);

        if (criteria.DistributionExpectedSince.HasValue)
            query = query.Where(x => x.DistributionExpected >= criteria.DistributionExpectedSince);

        if (criteria.DistributionExpectedBefore.HasValue)
            query = query.Where(x => x.DistributionExpected < criteria.DistributionExpectedBefore);

        if (criteria.EligibilityUpdatedSince.HasValue)
            query = query.Where(x => x.EligibilityUpdated >= criteria.EligibilityUpdatedSince);

        if (criteria.EligibilityUpdatedBefore.HasValue)
            query = query.Where(x => x.EligibilityUpdated < criteria.EligibilityUpdatedBefore);

        if (criteria.GradeAssignedSince.HasValue)
            query = query.Where(x => x.GradeAssigned >= criteria.GradeAssignedSince);

        if (criteria.GradeAssignedBefore.HasValue)
            query = query.Where(x => x.GradeAssigned < criteria.GradeAssignedBefore);

        if (criteria.GradePublishedSince.HasValue)
            query = query.Where(x => x.GradePublished >= criteria.GradePublishedSince);

        if (criteria.GradePublishedBefore.HasValue)
            query = query.Where(x => x.GradePublished < criteria.GradePublishedBefore);

        if (criteria.GradeReleasedSince.HasValue)
            query = query.Where(x => x.GradeReleased >= criteria.GradeReleasedSince);

        if (criteria.GradeReleasedBefore.HasValue)
            query = query.Where(x => x.GradeReleased < criteria.GradeReleasedBefore);

        if (criteria.GradeWithheldSince.HasValue)
            query = query.Where(x => x.GradeWithheld >= criteria.GradeWithheldSince);

        if (criteria.GradeWithheldBefore.HasValue)
            query = query.Where(x => x.GradeWithheld < criteria.GradeWithheldBefore);

        if (criteria.LastChangeTimeSince.HasValue)
            query = query.Where(x => x.LastChangeTime >= criteria.LastChangeTimeSince);

        if (criteria.LastChangeTimeBefore.HasValue)
            query = query.Where(x => x.LastChangeTime < criteria.LastChangeTimeBefore);

        if (criteria.RegistrationRequestedOnSince.HasValue)
            query = query.Where(x => x.RegistrationRequestedOn >= criteria.RegistrationRequestedOnSince);

        if (criteria.RegistrationRequestedOnBefore.HasValue)
            query = query.Where(x => x.RegistrationRequestedOn < criteria.RegistrationRequestedOnBefore);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<RegistrationMatch>> ToMatchesAsync(IQueryable<RegistrationEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new RegistrationMatch
            {
                RegistrationIdentifier = entity.RegistrationIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}