using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Application.Events.Read;
using InSite.Domain.Events;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web
{
    public static class ClassEventHelper
    {
        #region Classes

        private class QEventJson
        {
            public string EventBillingType { get; set; }
            public string EventClassCode { get; set; }
            public ContentEventClass Content { get; set; }
            public string EventDescription { get; set; }
            public string EventFormat { get; set; }
            public string EventPublicationStatus { get; set; }
            public string EventRequisitionStatus { get; set; }
            public string EventSchedulingStatus { get; set; }
            public string EventSource { get; set; }
            public string EventSummary { get; set; }
            public string EventTitle { get; set; }
            public string EventType { get; set; }
            public string DistributionCode { get; set; }
            public string DistributionErrors { get; set; }
            public string DistributionProcess { get; set; }
            public string DistributionStatus { get; set; }
            public string DurationUnit { get; set; }
            public string ExamMaterialReturnShipmentCode { get; set; }
            public string ExamMaterialReturnShipmentCondition { get; set; }
            public string ExamType { get; set; }

            public string PublicationErrors { get; set; }
            public string VenueRoom { get; set; }

            public bool WaitlistEnabled { get; set; }

            public int? CapacityMaximum { get; set; }
            public int? CapacityMinimum { get; set; }
            public int? DurationQuantity { get; set; }
            public int? ExamDurationInMinutes { get; set; }
            public int? InvigilatorMinimum { get; set; }
            public int? SendReminderBeforeDays { get; set; }

            public decimal? CreditHours { get; set; }

            public DateTimeOffset? EventScheduledEnd { get; set; }
            public DateTimeOffset EventScheduledStart { get; set; }
            public DateTimeOffset? DistributionExpected { get; set; }
            public DateTimeOffset? DistributionOrdered { get; set; }
            public DateTimeOffset? DistributionShipped { get; set; }
            public DateTimeOffset? DistributionTracked { get; set; }
            public DateTimeOffset? ExamMaterialReturnShipmentReceived { get; set; }
            public DateTimeOffset? ExamStarted { get; set; }
            public DateTimeOffset? RegistrationDeadline { get; set; }
            public DateTimeOffset? RegistrationStart { get; set; }

            public Guid? ReminderLearnerMessageId { get; set; }
            public Guid? ReminderInstructorMessageId { get; set; }

            public ICollection<Seat> Seats { get; set; }
            public ICollection<RegistrationField> RegistrationFields { get; set; }
            public ICollection<PrivacyGroup> PrivacyGroups { get; set; }
        }

        private class Seat
        {
            public bool IsAvailable { get; set; }
            public bool IsTaxable { get; set; }
            public int? OrderSequence { get; set; }
            public string SeatTitle { get; set; }
            public string SeatName { get; set; }
            public string SeatDescription { get; set; }
            public string SeatAgreement { get; set; }
            public List<SeatConfiguration.Price> Prices { get; set; }
        }

        private class SeatPrice
        {
            public decimal Amount { get; set; }
            public string Name { get; set; }
            public string GroupStatus { get; set; }
        }

        private class PrivacyGroup
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }

        #endregion

        public static byte[] Serialize(QEvent classEvent)
        {
            var data = Map(classEvent);
            var json = JsonHelper.JsonExport(data);

            return Encoding.UTF8.GetBytes(json);
        }

        public static QEvent Deserialize(string json, Func<Guid, bool> messageExists, out List<Guid> privacyGroups)
        {
            QEventJson data;

            try
            {
                data = JsonHelper.JsonImport<QEventJson>(json);
            }
            catch (JsonReaderException)
            {
                privacyGroups = new List<Guid>();
                return null;
            }
            catch (ApplicationError)
            {
                privacyGroups = new List<Guid>();
                return null;
            }

            privacyGroups = data.PrivacyGroups?
                .Select(pg => pg.ID)
                .ToList()
                ?? new List<Guid>();

            var result = new QEvent
            {
                EventBillingType = data.EventBillingType,
                EventClassCode = data.EventClassCode,
                Content = data.Content.Serialize(),
                EventDescription = data.EventDescription,
                EventFormat = data.EventFormat,
                EventPublicationStatus = data.EventPublicationStatus,
                EventRequisitionStatus = data.EventRequisitionStatus,
                EventSchedulingStatus = data.EventSchedulingStatus,
                EventSource = data.EventSource,
                EventSummary = data.EventSummary,
                EventTitle = data.EventTitle,
                EventType = data.EventType,
                DistributionCode = data.DistributionCode,
                DistributionErrors = data.DistributionErrors,
                DistributionProcess = data.DistributionProcess,
                DistributionStatus = data.DistributionStatus,
                DurationUnit = data.DurationUnit,
                ExamMaterialReturnShipmentCode = data.ExamMaterialReturnShipmentCode,
                ExamMaterialReturnShipmentCondition = data.ExamMaterialReturnShipmentCondition,
                ExamType = data.ExamType,
                PublicationErrors = data.PublicationErrors,
                VenueRoom = data.VenueRoom,
                WaitlistEnabled = data.WaitlistEnabled,
                CapacityMaximum = data.CapacityMaximum,
                CapacityMinimum = data.CapacityMinimum,
                DurationQuantity = data.DurationQuantity,
                ExamDurationInMinutes = data.ExamDurationInMinutes,
                InvigilatorMinimum = data.InvigilatorMinimum,
                SendReminderBeforeDays = data.SendReminderBeforeDays,
                CreditHours = data.CreditHours,
                EventScheduledEnd = data.EventScheduledEnd,
                EventScheduledStart = data.EventScheduledStart,
                DistributionExpected = data.DistributionExpected,
                DistributionOrdered = data.DistributionOrdered,
                DistributionShipped = data.DistributionShipped,
                DistributionTracked = data.DistributionTracked,
                ExamMaterialReturnShipmentReceived = data.ExamMaterialReturnShipmentReceived,
                ExamStarted = data.ExamStarted,
                RegistrationDeadline = data.RegistrationDeadline,
                RegistrationStart = data.RegistrationStart,
                WhenEventReminderRequestedNotifyLearnerMessageIdentifier = data.ReminderLearnerMessageId.HasValue && messageExists(data.ReminderLearnerMessageId.Value)
                    ? data.ReminderLearnerMessageId
                    : null,
                WhenEventReminderRequestedNotifyInstructorMessageIdentifier = data.ReminderInstructorMessageId.HasValue && messageExists(data.ReminderInstructorMessageId.Value)
                    ? data.ReminderInstructorMessageId
                    : null,
                Seats = data.Seats.EmptyIfNull().Select(seat =>
                {
                    var configuration = new SeatConfiguration();
                    configuration.Prices = seat.Prices;

                    var seatContent = new ContentSeat();
                    seatContent.Title.Default = seat.SeatName;
                    seatContent.Description.Default = seat.SeatDescription;
                    seatContent.AddOrGet("Agreement").Default = seat.SeatAgreement;

                    return new QSeat
                    {
                        Content = seatContent.Serialize(),
                        Configuration = JsonConvert.SerializeObject(configuration),
                        IsAvailable = seat.IsAvailable,
                        IsTaxable = seat.IsTaxable,
                        OrderSequence = seat.OrderSequence,
                        SeatTitle = seat.SeatTitle,
                    };
                }).ToList()
            };

            result.SetRegistrationFields(data.RegistrationFields);

            return result;
        }

        private static QEventJson Map(QEvent @event)
        {
            var groupPermissions = TGroupPermissionSearch
            .Bind(
                x => new PrivacyGroup()
                {
                    ID = x.GroupIdentifier,
                    Name = x.Group.GroupName,
                    Type = x.Group.GroupType
                },
                x => x.ObjectIdentifier == @event.EventIdentifier
            )
            .OrderBy(x => x.Name)
            .ToList();


            return new QEventJson
            {
                EventBillingType = @event.EventBillingType,
                EventClassCode = @event.EventClassCode,
                Content = ContentEventClass.Deserialize(@event.Content),
                EventDescription = @event.EventDescription,
                EventFormat = @event.EventFormat,
                EventPublicationStatus = @event.EventPublicationStatus,
                EventRequisitionStatus = @event.EventRequisitionStatus,
                EventSchedulingStatus = @event.EventSchedulingStatus,
                EventSource = @event.EventSource,
                EventSummary = @event.EventSummary,
                EventTitle = @event.EventTitle,
                EventType = @event.EventType,
                DistributionCode = @event.DistributionCode,
                DistributionErrors = @event.DistributionErrors,
                DistributionProcess = @event.DistributionProcess,
                DistributionStatus = @event.DistributionStatus,
                DurationUnit = @event.DurationUnit,
                ExamMaterialReturnShipmentCode = @event.ExamMaterialReturnShipmentCode,
                ExamMaterialReturnShipmentCondition = @event.ExamMaterialReturnShipmentCondition,
                ExamType = @event.ExamType,
                PublicationErrors = @event.PublicationErrors,
                VenueRoom = @event.VenueRoom,
                WaitlistEnabled = @event.WaitlistEnabled,
                CapacityMaximum = @event.CapacityMaximum,
                CapacityMinimum = @event.CapacityMinimum,
                DurationQuantity = @event.DurationQuantity,
                ExamDurationInMinutes = @event.ExamDurationInMinutes,
                InvigilatorMinimum = @event.InvigilatorMinimum,
                SendReminderBeforeDays = @event.SendReminderBeforeDays,
                CreditHours = @event.CreditHours,
                EventScheduledEnd = @event.EventScheduledEnd,
                EventScheduledStart = @event.EventScheduledStart,
                DistributionExpected = @event.DistributionExpected,
                DistributionOrdered = @event.DistributionOrdered,
                DistributionShipped = @event.DistributionShipped,
                DistributionTracked = @event.DistributionTracked,
                ExamMaterialReturnShipmentReceived = @event.ExamMaterialReturnShipmentReceived,
                ExamStarted = @event.ExamStarted,
                RegistrationDeadline = @event.RegistrationDeadline,
                RegistrationStart = @event.RegistrationStart,
                ReminderLearnerMessageId = @event.WhenEventReminderRequestedNotifyLearnerMessageIdentifier,
                ReminderInstructorMessageId = @event.WhenEventReminderRequestedNotifyInstructorMessageIdentifier,
                PrivacyGroups = groupPermissions,
                Seats = @event.Seats.EmptyIfNull().Select(seat =>
                {
                    var content = ContentSeat.Deserialize(seat.Content);
                    var configuration = seat.Configuration.IsNotEmpty()
                        ? JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration)
                        : new SeatConfiguration();

                    return new Seat
                    {
                        Prices = configuration.Prices,
                        IsAvailable = seat.IsAvailable,
                        IsTaxable = seat.IsTaxable,
                        OrderSequence = seat.OrderSequence,
                        SeatTitle = seat.SeatTitle,
                        SeatName = content.Title.Default,
                        SeatDescription = content.Description.Default,
                        SeatAgreement = content.Get("Agreement")?.Default
                    };
                }).ToArray(),
                RegistrationFields = @event.GetRegistrationFields().ToArray()
            };
        }
    }
}