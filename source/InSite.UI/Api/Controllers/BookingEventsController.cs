using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using InSite.Admin.Events.Candidates.Controls;
using InSite.Api.Settings;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Web.Infrastructure;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Booking")]
    public class EventsController : ApiBaseController
    {
        private class CalendarEvent
        {
            public Guid EventID { get; set; }
            public string Title { get; set; }
            public string AppointmentType { get; set; }
            public string Description { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string ThemeColor { get; set; }
            public string Url { get; set; }
        }

        private static Dictionary<string, (string Url, string Color)> _settings = new Dictionary<string, (string Url, string Color)>(StringComparer.OrdinalIgnoreCase)
        {
            { EventType.Appointment.ToString(), ("/ui/portal/events/appointments/outline?event=", "info") },
            { EventType.Class.ToString(), ("/ui/portal/events/classes/outline?event=", "primary") },
            { EventType.Exam.ToString(), ("/ui/portal/events/exams/outline?event=", "danger") }
        };

        private static (string Url, string Color) _defaultSetting = ("/ui/portal/events/appointments/outline?event=", "#28a745");

        [HttpGet]
        [Route("api/events/list-events")]
        public HttpResponseMessage List(string date)
        {
            var filter = new QEventFilter
            {
                OrganizationIdentifier = CurrentOrganization.Identifier,
                EventPublicationStatus = PublicationStatus.Published.GetDescription()
            };

            if (DateTime.TryParse(date, out var start))
            {
                var firstDay = new DateTime(start.Year, start.Month, 1);

                filter.EventScheduledSince = new DateTimeOffset(firstDay, CurrentUser.TimeZone.BaseUtcOffset);
                filter.EventScheduledBefore = filter.EventScheduledSince.Value.AddMonths(1);
            }

            var events = GetEvents(filter);

            return JsonSuccess(events);
        }

        /// <summary>
        /// Add a new event.
        /// </summary>
        [HttpPost]
        [Route("api/events/{event}")]
        public HttpResponseMessage PostEvent([FromUri] Guid @event, AddEvent add)
        {
            try
            {
                if (ServiceLocator.EventSearch.GetEvent(@event) != null)
                    return JsonError($"Event Already Exists: {@event}", HttpStatusCode.BadRequest);

                var organization = GetOrganization().Identifier;

                var (exam, error, created) = RegistrationsController.CreateExam(organization, @event, add.EventStart, add.EventExamType, add.EventBillingCode, add.EventExamFormat, add.EventVenue);
                if (exam == null)
                    return JsonError(error, HttpStatusCode.BadRequest);

                SendCommand(new AdjustCandidateCapacity(@event, null, add.RegistrationLimit, ToggleType.Disabled));

                return JsonSuccess(new { Exam = exam.Value, @event }, created ? HttpStatusCode.Created : HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        [HttpPost, Route("api/events/{event}/cancel")]
        public HttpResponseMessage PostEvent([FromUri] Guid @event, Shift.Sdk.UI.CancelEvent cancel)
        {
            try
            {
                var organization = GetOrganization().Identifier;
                var eventEntity = ServiceLocator.EventSearch.GetEvent(@event);

                if (eventEntity == null || eventEntity.OrganizationIdentifier != organization || eventEntity.EventType != "Exam")
                    return JsonError($"Exam event not found.", HttpStatusCode.BadRequest);

                if (eventEntity.EventSchedulingStatus == "Cancelled")
                    return JsonError($"Exam event is already cancelled..", HttpStatusCode.BadRequest);

                var registrations = ServiceLocator.RegistrationSearch.GetRegistrationsByEvent(@event);
                if (registrations.Any(x => x.GradingStatus != "Cancelled"))
                    return JsonError($"The exam event cannot be canceled because it contains active registrations.", HttpStatusCode.BadRequest);

                var reason = "Cancelled from Shift API";
                if (cancel.Reason.IsNotEmpty())
                    reason += ". " + cancel.Reason;

                ServiceLocator.SendCommand(new Application.Events.Write.CancelEvent(@event, reason, false));

                return JsonSuccess("OK");
            }
            catch (Exception ex)
            {
                return JsonError(ex);
            }
        }

        [HttpGet]
        [Route("api/events/classes")]
        [AllowAnonymous]
        public HttpResponseMessage Sessions(Guid organization)
        {
            var now = DateTimeOffset.UtcNow;

            var filter = new QEventFilter
            {
                OrganizationIdentifier = organization,
                EventScheduledSince = now
            };

            var list = ServiceLocator.EventSearch.GetEvents(filter, null, null, x => x.Registrations).Select(x =>
            {
                var isFull = x.CapacityMaximum.HasValue && x.CapacityMaximum.Value <= x.Registrations.Count
                    || (x.CapacityMinimum ?? 0) == 0 && !x.CapacityMaximum.HasValue;

                return new
                {
                    Identifier = x.EventIdentifier,
                    Title = x.EventTitle,
                    IsFull = isFull,
                    IsClosed = x.RegistrationDeadline.HasValue && x.RegistrationDeadline < now
                };
            }).ToArray();

            return JsonSuccess(list);
        }

        [HttpGet]
        [Route("api/events/scrap-paper")]
        [AllowAnonymous]
        public HttpResponseMessage CreateExamScrapPaper(string id)
        {
            var result = "OK";

            try
            {
                var eventId = Guid.Parse(id);

                var data = ScrapsPrint.RenderPdf(eventId);
                if (data != null)
                {
                    string filePath = null;
                    var storageId = TempFileStorage.Create();

                    TempFileStorage.Open(storageId, dir =>
                    {
                        filePath = Path.Combine(dir.FullName, "Scrap-Paper.pdf");
                        File.WriteAllBytes(filePath, data);
                    });

                    result = filePath;
                }
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
            }

            var response = Request.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(result, Encoding.UTF8);
            return response;
        }

        private static List<CalendarEvent> GetEvents(QEventFilter filter)
        {
            var timeZone = CurrentUser.TimeZone;

            return ServiceLocator.EventSearch
                .GetEvents(filter)
                .Select(v => GetCalendarEvent(v))
                .ToList();

            CalendarEvent GetCalendarEvent(QEvent v)
            {
                if (!_settings.TryGetValue(v.EventType, out var setting))
                    setting = _defaultSetting;

                return new CalendarEvent
                {
                    EventID = v.EventIdentifier,
                    Title = v.EventTitle,
                    AppointmentType = v.AppointmentType,
                    Description = v.EventDescription,
                    StartDate = DateToStr(v.EventScheduledStart),
                    EndDate = DateToStr(v.EventScheduledEnd ?? v.EventScheduledStart),
                    ThemeColor = v.EventCalendarColor.HasValue() ? GetEventColor(v.EventCalendarColor) : setting.Color,
                    Url = setting.Url + v.EventIdentifier
                };
            }

            string DateToStr(DateTimeOffset date)
            {
                var localizedDate = TimeZoneInfo.ConvertTime(date, timeZone);
                return localizedDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        private static string GetEventColor(string color)
        {
            var indicator = color.ToEnumNullable<Indicator>();
            return indicator?.GetContextualClass();
        }
    }
}