using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Booking;

public class QEventReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly IShiftIdentityService _identity;

    public QEventReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService identity)
    {
        _context = context;
        _identity = identity;
    }

    public async Task<bool> AssertAsync(
        Guid @event,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QEvent
            .AnyAsync(x => x.EventIdentifier == @event, cancellation);
    }

    public async Task<QEventEntity?> RetrieveAsync(
        Guid @event,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QEvent
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EventIdentifier == @event, cancellation);
    }

    public async Task<int> CountAsync(
        IEventCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QEventEntity>> CollectAsync(
        IEventCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<QEventEntity>> DownloadAsync(
        IEventCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<EventMatch>> SearchAsync(
        IEventCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QEventEntity> BuildQueryable(
        TableDbContext db,
        IEventCriteria criteria)
    {
        var q = db.QEvent.AsNoTracking().AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // TODO: Implement search criteria

        // if (criteria.EventBillingType != null)
        //    query = query.Where(x => x.EventBillingType == criteria.EventBillingType);

        // if (criteria.EventClassCode != null)
        //    query = query.Where(x => x.EventClassCode == criteria.EventClassCode);

        // if (criteria.ExamDurationInMinutes != null)
        //    query = query.Where(x => x.ExamDurationInMinutes == criteria.ExamDurationInMinutes);

        // if (criteria.EventFormat != null)
        //    query = query.Where(x => x.EventFormat == criteria.EventFormat);

        // if (criteria.EventIdentifier != null)
        //    query = query.Where(x => x.EventIdentifier == criteria.EventIdentifier);

        // if (criteria.EventNumber != null)
        //    query = query.Where(x => x.EventNumber == criteria.EventNumber);

        // if (criteria.EventSchedulingStatus != null)
        //    query = query.Where(x => x.EventSchedulingStatus == criteria.EventSchedulingStatus);

        // if (criteria.EventTitle != null)
        //    query = query.Where(x => x.EventTitle == criteria.EventTitle);

        // if (criteria.EventType != null)
        //    query = query.Where(x => x.EventType == criteria.EventType);

        // if (criteria.DistributionCode != null)
        //    query = query.Where(x => x.DistributionCode == criteria.DistributionCode);

        // if (criteria.DistributionErrors != null)
        //    query = query.Where(x => x.DistributionErrors == criteria.DistributionErrors);

        // if (criteria.DistributionExpected != null)
        //    query = query.Where(x => x.DistributionExpected == criteria.DistributionExpected);

        // if (criteria.DistributionProcess != null)
        //    query = query.Where(x => x.DistributionProcess == criteria.DistributionProcess);

        // if (criteria.DistributionOrdered != null)
        //    query = query.Where(x => x.DistributionOrdered == criteria.DistributionOrdered);

        // if (criteria.DistributionShipped != null)
        //    query = query.Where(x => x.DistributionShipped == criteria.DistributionShipped);

        // if (criteria.DistributionStatus != null)
        //    query = query.Where(x => x.DistributionStatus == criteria.DistributionStatus);

        // if (criteria.DistributionTracked != null)
        //    query = query.Where(x => x.DistributionTracked == criteria.DistributionTracked);

        // if (criteria.ExamStarted != null)
        //    query = query.Where(x => x.ExamStarted == criteria.ExamStarted);

        // if (criteria.OrganizationIdentifier != null)
        //    query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        // if (criteria.VenueLocationIdentifier != null)
        //    query = query.Where(x => x.VenueLocationIdentifier == criteria.VenueLocationIdentifier);

        // if (criteria.VenueRoom != null)
        //    query = query.Where(x => x.VenueRoom == criteria.VenueRoom);

        // if (criteria.PublicationErrors != null)
        //    query = query.Where(x => x.PublicationErrors == criteria.PublicationErrors);

        // if (criteria.VenueCoordinatorIdentifier != null)
        //    query = query.Where(x => x.VenueCoordinatorIdentifier == criteria.VenueCoordinatorIdentifier);

        // if (criteria.EventDescription != null)
        //    query = query.Where(x => x.EventDescription == criteria.EventDescription);

        // if (criteria.LastChangeTime != null)
        //    query = query.Where(x => x.LastChangeTime == criteria.LastChangeTime);

        // if (criteria.LastChangeType != null)
        //    query = query.Where(x => x.LastChangeType == criteria.LastChangeType);

        // if (criteria.LastChangeUser != null)
        //    query = query.Where(x => x.LastChangeUser == criteria.LastChangeUser);

        // if (criteria.EventScheduledStart != null)
        //    query = query.Where(x => x.EventScheduledStart == criteria.EventScheduledStart);

        // if (criteria.EventScheduledEnd != null)
        //    query = query.Where(x => x.EventScheduledEnd == criteria.EventScheduledEnd);

        // if (criteria.DurationQuantity != null)
        //    query = query.Where(x => x.DurationQuantity == criteria.DurationQuantity);

        // if (criteria.DurationUnit != null)
        //    query = query.Where(x => x.DurationUnit == criteria.DurationUnit);

        // if (criteria.EventSource != null)
        //    query = query.Where(x => x.EventSource == criteria.EventSource);

        // if (criteria.CreditHours != null)
        //    query = query.Where(x => x.CreditHours == criteria.CreditHours);

        // if (criteria.CapacityMinimum != null)
        //    query = query.Where(x => x.CapacityMinimum == criteria.CapacityMinimum);

        // if (criteria.CapacityMaximum != null)
        //    query = query.Where(x => x.CapacityMaximum == criteria.CapacityMaximum);

        // if (criteria.ExamType != null)
        //    query = query.Where(x => x.ExamType == criteria.ExamType);

        // if (criteria.AchievementIdentifier != null)
        //    query = query.Where(x => x.AchievementIdentifier == criteria.AchievementIdentifier);

        // if (criteria.RegistrationDeadline != null)
        //    query = query.Where(x => x.RegistrationDeadline == criteria.RegistrationDeadline);

        // if (criteria.EventSummary != null)
        //    query = query.Where(x => x.EventSummary == criteria.EventSummary);

        // if (criteria.EventPublicationStatus != null)
        //    query = query.Where(x => x.EventPublicationStatus == criteria.EventPublicationStatus);

        // if (criteria.Content != null)
        //    query = query.Where(x => x.Content == criteria.Content);

        // if (criteria.WaitlistEnabled != null)
        //    query = query.Where(x => x.WaitlistEnabled == criteria.WaitlistEnabled);

        // if (criteria.RegistrationStart != null)
        //    query = query.Where(x => x.RegistrationStart == criteria.RegistrationStart);

        // if (criteria.EventRequisitionStatus != null)
        //    query = query.Where(x => x.EventRequisitionStatus == criteria.EventRequisitionStatus);

        // if (criteria.InvigilatorMinimum != null)
        //    query = query.Where(x => x.InvigilatorMinimum == criteria.InvigilatorMinimum);

        // if (criteria.ExamMaterialReturnShipmentCode != null)
        //    query = query.Where(x => x.ExamMaterialReturnShipmentCode == criteria.ExamMaterialReturnShipmentCode);

        // if (criteria.ExamMaterialReturnShipmentReceived != null)
        //    query = query.Where(x => x.ExamMaterialReturnShipmentReceived == criteria.ExamMaterialReturnShipmentReceived);

        // if (criteria.ExamMaterialReturnShipmentCondition != null)
        //    query = query.Where(x => x.ExamMaterialReturnShipmentCondition == criteria.ExamMaterialReturnShipmentCondition);

        // if (criteria.IntegrationWithholdGrades != null)
        //    query = query.Where(x => x.IntegrationWithholdGrades == criteria.IntegrationWithholdGrades);

        // if (criteria.IntegrationWithholdDistribution != null)
        //    query = query.Where(x => x.IntegrationWithholdDistribution == criteria.IntegrationWithholdDistribution);

        // if (criteria.VenueOfficeIdentifier != null)
        //    query = query.Where(x => x.VenueOfficeIdentifier == criteria.VenueOfficeIdentifier);

        // if (criteria.AppointmentType != null)
        //    query = query.Where(x => x.AppointmentType == criteria.AppointmentType);

        // if (criteria.RegistrationLocked != null)
        //    query = query.Where(x => x.RegistrationLocked == criteria.RegistrationLocked);

        // if (criteria.AllowRegistrationWithLink != null)
        //    query = query.Where(x => x.AllowRegistrationWithLink == criteria.AllowRegistrationWithLink);

        // if (criteria.LearnerRegistrationGroupIdentifier != null)
        //    query = query.Where(x => x.LearnerRegistrationGroupIdentifier == criteria.LearnerRegistrationGroupIdentifier);

        // if (criteria.PersonCodeIsRequired != null)
        //    query = query.Where(x => x.PersonCodeIsRequired == criteria.PersonCodeIsRequired);

        // if (criteria.AllowMultipleRegistrations != null)
        //    query = query.Where(x => x.AllowMultipleRegistrations == criteria.AllowMultipleRegistrations);

        // if (criteria.EventCalendarColor != null)
        //    query = query.Where(x => x.EventCalendarColor == criteria.EventCalendarColor);

        // if (criteria.RegistrationFields != null)
        //    query = query.Where(x => x.RegistrationFields == criteria.RegistrationFields);

        // if (criteria.MandatorySurveyFormIdentifier != null)
        //    query = query.Where(x => x.MandatorySurveyFormIdentifier == criteria.MandatorySurveyFormIdentifier);

        // if (criteria.BillingCodeEnabled != null)
        //    query = query.Where(x => x.BillingCodeEnabled == criteria.BillingCodeEnabled);

        return q;
    }

    public static async Task<IEnumerable<EventMatch>> ToMatchesAsync(
        IQueryable<QEventEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new EventMatch
            {
                EventIdentifier = entity.EventIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}