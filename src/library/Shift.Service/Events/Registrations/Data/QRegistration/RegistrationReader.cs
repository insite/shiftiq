using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
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

    public Task<int> CountDetailsAsync(IRegistrationDetailsCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildDetailsQueryable(db, criteria);

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

    public Task<List<RegistrationDetailsMatch>> SearchDetailsAsync(IRegistrationDetailsCriteria criteria, CancellationToken cancellation = default)
    {
        var physAddressType = AddressType.Physical.GetName();

        return ExecuteAsync(async db =>
        {
            var query = BuildDetailsQueryable(db, criteria)
                .Select(x => new
                {
                    Registration = x,
                    Candidate = x.CandidateUser!,
                    Person = x.CandidateUser!.People.FirstOrDefault(y => y.OrganizationIdentifier == x.OrganizationIdentifier),
                    Event = x.Event!,
                    EventVenueAddress = x.Event!.VenueLocation!.Addresses
                        .Where(y => y.AddressType == physAddressType)
                        .FirstOrDefault(),
                    Instructors = x.Event!.Users
                        .Where(y => y.AttendeeRole == "Instructor" && y.User != null)
                        .OrderBy(y => y.User!.FullName)
                        .Select(y => y.User!)
                })
                .Select(x => new
                {
                    RegistrationIdentifier = x.Registration.RegistrationIdentifier,
                    ApprovalStatus = x.Registration.ApprovalStatus,
                    Accommodation = string.Join(";", x.Registration.Accommodations
                        .OrderBy(y => y.AccommodationType)
                        .Select(y => y.AccommodationType)
                        .Distinct()),

                    FirstName = x.Candidate.FirstName,
                    MiddleName = x.Candidate.MiddleName,
                    LastName = x.Candidate.LastName,
                    Email = x.Candidate.Email,
                    Birthdate = x.Person!.Birthdate,
                    PersonCode = x.Person.PersonCode,
                    PreferredLanguage = x.Person.Language ?? "en",

                    EventTitle = x.Event.EventTitle,
                    EventVenue = x.Event.VenueLocation!.GroupName,
                    EventVenueAddress = x.EventVenueAddress,
                    EventVenueAddressSort =
                        (x.EventVenueAddress!.Street1 ?? "") + " " + (x.EventVenueAddress.Street2 ?? "") + " " +
                        (x.EventVenueAddress.City ?? "") + " " + (x.EventVenueAddress.Province ?? "") + " " +
                        (x.EventVenueAddress.PostalCode ?? "") + " " + (x.EventVenueAddress.Country ?? ""),
                    VenueIdentifier = x.Event.VenueLocationIdentifier,
                    BuildingAndRoom = x.Event.VenueRoom,
                    EventScheduledStart = x.Event.EventScheduledStart,
                    EventScheduledEnd = x.Event.EventScheduledEnd,
                    Instructor = string.Join(";", x.Instructors.Select(y => y.FullName)),
                    InstructorEmail = string.Join(";", x.Instructors.Select(y => y.Email)),
                });

            var sortExpression = DefaultSort;

            if (criteria.Filter.Sort.IsNotEmpty())
            {
                sortExpression = criteria.Filter.Sort;

                if (QuerySort.TryParse(sortExpression, out var sort))
                {
                    var venueAddress = sort.Find("EventVenueAddress");
                    if (venueAddress != null)
                        venueAddress.Field = "EventVenueAddressSort";

                    sortExpression = sort.ToString();
                }
            }

            var result = await query
                .OrderBy(sortExpression)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

            return result
                .Select(x => new RegistrationDetailsMatch
                {
                    RegistrationIdentifier = x.RegistrationIdentifier,
                    ApprovalStatus = x.ApprovalStatus,
                    Accommodation = x.Accommodation,

                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Birthdate = x.Birthdate,
                    PersonCode = x.PersonCode,
                    PreferredLanguage = x.PreferredLanguage,

                    EventTitle = x.EventTitle,
                    EventVenue = x.EventVenue,
                    EventVenueAddress = x.EventVenueAddress == null ? null : LocationHelper.ToString(
                        x.EventVenueAddress.Street1,
                        x.EventVenueAddress.Street2,
                        x.EventVenueAddress.City,
                        x.EventVenueAddress.Province,
                        x.EventVenueAddress.PostalCode,
                        x.EventVenueAddress.Country,
                        null,
                        null),
                    VenueIdentifier = x.VenueIdentifier,
                    BuildingAndRoom = x.BuildingAndRoom,
                    EventScheduledStart = x.EventScheduledStart,
                    EventScheduledEnd = x.EventScheduledEnd,
                    Instructor = x.Instructor.NullIfEmpty(),
                    InstructorEmail = x.InstructorEmail.NullIfEmpty(),
                })
                .ToList();
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

    private IQueryable<RegistrationEntity> BuildDetailsQueryable(TableDbContext db, IRegistrationDetailsCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId.Value);

        if (!string.IsNullOrWhiteSpace(criteria.ApprovalStatus))
            query = query.Where(x => x.ApprovalStatus == criteria.ApprovalStatus);

        var title = criteria.EventTitle;

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(x => x.Event!.EventTitle != null && x.Event.EventTitle.Contains(title));

        if (criteria.EventScheduledStartSince.HasValue)
            query = query.Where(x => x.Event!.EventScheduledStart >= criteria.EventScheduledStartSince);

        if (criteria.EventScheduledStartBefore.HasValue)
            query = query.Where(x => x.Event!.EventScheduledStart < criteria.EventScheduledStartBefore);

        if (criteria.EventScheduledEndSince.HasValue)
            query = query.Where(x => x.Event!.EventScheduledEnd >= criteria.EventScheduledEndSince);

        if (criteria.EventScheduledEndBefore.HasValue)
            query = query.Where(x => x.Event!.EventScheduledEnd < criteria.EventScheduledEndBefore);

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
