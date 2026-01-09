namespace Shift.Service.Booking;

using Shift.Common;
using Shift.Contract;

public class EventAdapter : IEntityAdapter
{
    public void Copy(ModifyEvent modify, EventEntity entity)
    {
        entity.EventBillingType = modify.EventBillingType;
        entity.EventClassCode = modify.EventClassCode;
        entity.ExamDurationInMinutes = modify.ExamDurationInMinutes;
        entity.EventFormat = modify.EventFormat;
        entity.EventNumber = modify.EventNumber;
        entity.EventSchedulingStatus = modify.EventSchedulingStatus;
        entity.EventTitle = modify.EventTitle;
        entity.EventType = modify.EventType;
        entity.DistributionCode = modify.DistributionCode;
        entity.DistributionErrors = modify.DistributionErrors;
        entity.DistributionExpected = modify.DistributionExpected;
        entity.DistributionProcess = modify.DistributionProcess;
        entity.DistributionOrdered = modify.DistributionOrdered;
        entity.DistributionShipped = modify.DistributionShipped;
        entity.DistributionStatus = modify.DistributionStatus;
        entity.DistributionTracked = modify.DistributionTracked;
        entity.ExamStarted = modify.ExamStarted;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.VenueLocationIdentifier = modify.VenueLocationIdentifier;
        entity.VenueRoom = modify.VenueRoom;
        entity.PublicationErrors = modify.PublicationErrors;
        entity.VenueCoordinatorIdentifier = modify.VenueCoordinatorIdentifier;
        entity.EventDescription = modify.EventDescription;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
        entity.EventScheduledStart = modify.EventScheduledStart;
        entity.EventScheduledEnd = modify.EventScheduledEnd;
        entity.DurationQuantity = modify.DurationQuantity;
        entity.DurationUnit = modify.DurationUnit;
        entity.EventSource = modify.EventSource;
        entity.CreditHours = modify.CreditHours;
        entity.CapacityMinimum = modify.CapacityMinimum;
        entity.CapacityMaximum = modify.CapacityMaximum;
        entity.ExamType = modify.ExamType;
        entity.AchievementIdentifier = modify.AchievementIdentifier;
        entity.RegistrationDeadline = modify.RegistrationDeadline;
        entity.EventSummary = modify.EventSummary;
        entity.EventPublicationStatus = modify.EventPublicationStatus;
        entity.Content = modify.Content;
        entity.WaitlistEnabled = modify.WaitlistEnabled;
        entity.RegistrationStart = modify.RegistrationStart;
        entity.EventRequisitionStatus = modify.EventRequisitionStatus;
        entity.InvigilatorMinimum = modify.InvigilatorMinimum;
        entity.ExamMaterialReturnShipmentCode = modify.ExamMaterialReturnShipmentCode;
        entity.ExamMaterialReturnShipmentReceived = modify.ExamMaterialReturnShipmentReceived;
        entity.ExamMaterialReturnShipmentCondition = modify.ExamMaterialReturnShipmentCondition;
        entity.IntegrationWithholdGrades = modify.IntegrationWithholdGrades;
        entity.IntegrationWithholdDistribution = modify.IntegrationWithholdDistribution;
        entity.VenueOfficeIdentifier = modify.VenueOfficeIdentifier;
        entity.AppointmentType = modify.AppointmentType;
        entity.RegistrationLocked = modify.RegistrationLocked;
        entity.AllowRegistrationWithLink = modify.AllowRegistrationWithLink;
        entity.LearnerRegistrationGroupIdentifier = modify.LearnerRegistrationGroupIdentifier;
        entity.PersonCodeIsRequired = modify.PersonCodeIsRequired;
        entity.AllowMultipleRegistrations = modify.AllowMultipleRegistrations;
        entity.EventCalendarColor = modify.EventCalendarColor;
        entity.RegistrationFields = modify.RegistrationFields;
        entity.MandatorySurveyFormIdentifier = modify.MandatorySurveyFormIdentifier;
        entity.BillingCodeEnabled = modify.BillingCodeEnabled;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public EventEntity ToEntity(CreateEvent create)
    {
        var entity = new EventEntity
        {
            EventBillingType = create.EventBillingType,
            EventClassCode = create.EventClassCode,
            ExamDurationInMinutes = create.ExamDurationInMinutes,
            EventFormat = create.EventFormat,
            EventIdentifier = create.EventIdentifier,
            EventNumber = create.EventNumber,
            EventSchedulingStatus = create.EventSchedulingStatus,
            EventTitle = create.EventTitle,
            EventType = create.EventType,
            DistributionCode = create.DistributionCode,
            DistributionErrors = create.DistributionErrors,
            DistributionExpected = create.DistributionExpected,
            DistributionProcess = create.DistributionProcess,
            DistributionOrdered = create.DistributionOrdered,
            DistributionShipped = create.DistributionShipped,
            DistributionStatus = create.DistributionStatus,
            DistributionTracked = create.DistributionTracked,
            ExamStarted = create.ExamStarted,
            OrganizationIdentifier = create.OrganizationIdentifier,
            VenueLocationIdentifier = create.VenueLocationIdentifier,
            VenueRoom = create.VenueRoom,
            PublicationErrors = create.PublicationErrors,
            VenueCoordinatorIdentifier = create.VenueCoordinatorIdentifier,
            EventDescription = create.EventDescription,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser,
            EventScheduledStart = create.EventScheduledStart,
            EventScheduledEnd = create.EventScheduledEnd,
            DurationQuantity = create.DurationQuantity,
            DurationUnit = create.DurationUnit,
            EventSource = create.EventSource,
            CreditHours = create.CreditHours,
            CapacityMinimum = create.CapacityMinimum,
            CapacityMaximum = create.CapacityMaximum,
            ExamType = create.ExamType,
            AchievementIdentifier = create.AchievementIdentifier,
            RegistrationDeadline = create.RegistrationDeadline,
            EventSummary = create.EventSummary,
            EventPublicationStatus = create.EventPublicationStatus,
            Content = create.Content,
            WaitlistEnabled = create.WaitlistEnabled,
            RegistrationStart = create.RegistrationStart,
            EventRequisitionStatus = create.EventRequisitionStatus,
            InvigilatorMinimum = create.InvigilatorMinimum,
            ExamMaterialReturnShipmentCode = create.ExamMaterialReturnShipmentCode,
            ExamMaterialReturnShipmentReceived = create.ExamMaterialReturnShipmentReceived,
            ExamMaterialReturnShipmentCondition = create.ExamMaterialReturnShipmentCondition,
            IntegrationWithholdGrades = create.IntegrationWithholdGrades,
            IntegrationWithholdDistribution = create.IntegrationWithholdDistribution,
            VenueOfficeIdentifier = create.VenueOfficeIdentifier,
            AppointmentType = create.AppointmentType,
            RegistrationLocked = create.RegistrationLocked,
            AllowRegistrationWithLink = create.AllowRegistrationWithLink,
            LearnerRegistrationGroupIdentifier = create.LearnerRegistrationGroupIdentifier,
            PersonCodeIsRequired = create.PersonCodeIsRequired,
            AllowMultipleRegistrations = create.AllowMultipleRegistrations,
            EventCalendarColor = create.EventCalendarColor,
            RegistrationFields = create.RegistrationFields,
            MandatorySurveyFormIdentifier = create.MandatorySurveyFormIdentifier,
            BillingCodeEnabled = create.BillingCodeEnabled
        };
        return entity;
    }

    public IEnumerable<EventModel> ToModel(IEnumerable<EventEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public EventModel ToModel(EventEntity entity)
    {
        var model = new EventModel
        {
            EventBillingType = entity.EventBillingType,
            EventClassCode = entity.EventClassCode,
            ExamDurationInMinutes = entity.ExamDurationInMinutes,
            EventFormat = entity.EventFormat,
            EventIdentifier = entity.EventIdentifier,
            EventNumber = entity.EventNumber,
            EventSchedulingStatus = entity.EventSchedulingStatus,
            EventTitle = entity.EventTitle,
            EventType = entity.EventType,
            DistributionCode = entity.DistributionCode,
            DistributionErrors = entity.DistributionErrors,
            DistributionExpected = entity.DistributionExpected,
            DistributionProcess = entity.DistributionProcess,
            DistributionOrdered = entity.DistributionOrdered,
            DistributionShipped = entity.DistributionShipped,
            DistributionStatus = entity.DistributionStatus,
            DistributionTracked = entity.DistributionTracked,
            ExamStarted = entity.ExamStarted,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            VenueLocationIdentifier = entity.VenueLocationIdentifier,
            VenueRoom = entity.VenueRoom,
            PublicationErrors = entity.PublicationErrors,
            VenueCoordinatorIdentifier = entity.VenueCoordinatorIdentifier,
            EventDescription = entity.EventDescription,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser,
            EventScheduledStart = entity.EventScheduledStart,
            EventScheduledEnd = entity.EventScheduledEnd,
            DurationQuantity = entity.DurationQuantity,
            DurationUnit = entity.DurationUnit,
            EventSource = entity.EventSource,
            CreditHours = entity.CreditHours,
            CapacityMinimum = entity.CapacityMinimum,
            CapacityMaximum = entity.CapacityMaximum,
            ExamType = entity.ExamType,
            AchievementIdentifier = entity.AchievementIdentifier,
            RegistrationDeadline = entity.RegistrationDeadline,
            EventSummary = entity.EventSummary,
            EventPublicationStatus = entity.EventPublicationStatus,
            Content = entity.Content,
            WaitlistEnabled = entity.WaitlistEnabled,
            RegistrationStart = entity.RegistrationStart,
            EventRequisitionStatus = entity.EventRequisitionStatus,
            InvigilatorMinimum = entity.InvigilatorMinimum,
            ExamMaterialReturnShipmentCode = entity.ExamMaterialReturnShipmentCode,
            ExamMaterialReturnShipmentReceived = entity.ExamMaterialReturnShipmentReceived,
            ExamMaterialReturnShipmentCondition = entity.ExamMaterialReturnShipmentCondition,
            IntegrationWithholdGrades = entity.IntegrationWithholdGrades,
            IntegrationWithholdDistribution = entity.IntegrationWithholdDistribution,
            VenueOfficeIdentifier = entity.VenueOfficeIdentifier,
            AppointmentType = entity.AppointmentType,
            RegistrationLocked = entity.RegistrationLocked,
            AllowRegistrationWithLink = entity.AllowRegistrationWithLink,
            LearnerRegistrationGroupIdentifier = entity.LearnerRegistrationGroupIdentifier,
            PersonCodeIsRequired = entity.PersonCodeIsRequired,
            AllowMultipleRegistrations = entity.AllowMultipleRegistrations,
            EventCalendarColor = entity.EventCalendarColor,
            RegistrationFields = entity.RegistrationFields,
            MandatorySurveyFormIdentifier = entity.MandatorySurveyFormIdentifier,
            BillingCodeEnabled = entity.BillingCodeEnabled
        };

        return model;
    }

    public IEnumerable<EventMatch> ToMatch(IEnumerable<EventEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public EventMatch ToMatch(EventEntity entity)
    {
        var match = new EventMatch
        {
            EventIdentifier = entity.EventIdentifier

        };

        return match;
    }
}