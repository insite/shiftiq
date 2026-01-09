using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Events.Exams.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QEventFilter>
    {
        protected override int SelectCount(QEventFilter filter)
        {
            return ServiceLocator.EventSearch.CountEvents(filter);
        }

        internal class DataItem
        {
            public Guid EventIdentifier { get; set; }
            public string EventClassCode { get; set; }
            public DateTimeOffset EventScheduledStart { get; set; }
            public string EventFormat { get; set; }
            public string EventTitle { get; set; }
            public int EventNumber { get; set; }
            public string EventBillingType { get; set; }
            public string EventRequisitionStatus { get; set; }
            public string EventSchedulingStatus { get; set; }
            public string ExamMaterialReturnShipmentCondition { get; set; }
            public string VenueOfficeName { get; set; }
            public string VenueLocationName { get; set; }
            public string VenueRoom { get; set; }
            public string ExamForms { get; set; }
            public int RegistrationCount { get; set; }
            public int? CapacityMaximum { get; set; }
            public int? InvigilatorMinimum { get; set; }
        }

        protected Dictionary<Guid, List<QRegistration>> Registrations;
        protected override IListSource SelectData(QEventFilter filter)
        {
            var exams = ServiceLocator.EventSearch
                .GetEvents(filter, x => x.Registrations, x => x.ExamForms.Select(y => y.Form), x => x.VenueOffice, x => x.VenueLocation)
                .Select(x => new DataItem
                {
                    EventIdentifier = x.EventIdentifier,
                    EventClassCode = x.EventClassCode,
                    EventScheduledStart = x.EventScheduledStart,
                    EventFormat = x.EventFormat,
                    EventTitle = x.EventTitle,
                    EventNumber = x.EventNumber,
                    EventBillingType = x.EventBillingType,
                    EventRequisitionStatus = x.EventRequisitionStatus,
                    EventSchedulingStatus = x.EventSchedulingStatus,
                    ExamMaterialReturnShipmentCondition = x.ExamMaterialReturnShipmentCondition,
                    VenueOfficeName = x.VenueOfficeName,
                    VenueLocationName = x.VenueLocationName,
                    VenueRoom = x.VenueRoom,
                    ExamForms = string.Join("\r\n", x.ExamForms.Select(y => y.FormName)),
                    RegistrationCount = x.Registrations.Count,
                    CapacityMaximum = x.CapacityMaximum,
                    InvigilatorMinimum = x.InvigilatorMinimum
                })
                .ToList();

            Registrations = ServiceLocator.RegistrationSearch.GetRegistrationsByEvents(exams.Select(x => x.EventIdentifier).ToList());

            return exams.ToSearchResult();
        }

        internal class ExportDataItem : DataItem
        {
            public string Accommodations { get; set; }
        }

        public override IListSource GetExportData(QEventFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<DataItem>().Select(x => new ExportDataItem
            {
                EventIdentifier = x.EventIdentifier,
                EventClassCode = x.EventClassCode,
                EventScheduledStart = x.EventScheduledStart,
                EventFormat = x.EventFormat,
                EventTitle = x.EventTitle,
                EventNumber = x.EventNumber,
                EventBillingType = x.EventBillingType,
                EventRequisitionStatus = x.EventRequisitionStatus,
                EventSchedulingStatus = x.EventSchedulingStatus,
                ExamMaterialReturnShipmentCondition = x.ExamMaterialReturnShipmentCondition,
                VenueOfficeName = x.VenueOfficeName,
                VenueLocationName = x.VenueLocationName,
                VenueRoom = x.VenueRoom,
                ExamForms = x.ExamForms,
                RegistrationCount = x.RegistrationCount,
                CapacityMaximum = x.CapacityMaximum,
                InvigilatorMinimum = x.InvigilatorMinimum,
                Accommodations = GetAccommodations(
                    x.EventIdentifier,
                    (string accommodation, int count) => $", {accommodation}: {count}").NullIfEmpty()?.Substring(2)
            }).ToList().ToSearchResult();
        }

        protected string GetCandidateLimit(int regCount, int? regMax)
        {
            return !regMax.HasValue
                ? null
                : regCount > regMax
                    ? $"<span class='badge bg-danger'>{regCount} / {regMax} - Over</span>"
                    : regCount == regMax
                        ? $"<span class='badge rounded-pill bg-danger'>{regCount} / {regMax} - Full</span>"
                        : $"<span class='badge rounded-pill bg-custom-default'>{regCount} / {regMax}</span>";
        }

        protected string GetEventFormat(string format)
        {
            return format == EventExamFormat.Online.Value
                ? $"<span class='badge rounded-pill bg-primary'>{format}</span>"
                : $"<span class='badge rounded-pill bg-custom-default'>{format}</span>";
        }

        private string GetAccommodations(Guid @event, Func<string, int, string> getItem)
        {
            if (!Registrations.TryGetValue(@event, out var registrations))
                return string.Empty;

            registrations = registrations
                .OrderBy(x => x.Candidate?.UserFullName)
                .ToList();

            var list = registrations.SelectMany(x => x.Accommodations)
                .GroupBy(x => x.AccommodationType)
                .ToList();

            var html = new StringBuilder();

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                html.Append(getItem(item.Key, item.Count()));
            }

            return html.ToString();
        }

        protected string GetAccommodationsHtml(Guid @event) => GetAccommodations(
            @event,
            (string accommodation, int count) => $"<div class='text-nowrap'>{accommodation}: {count}</div>").IfNullOrEmpty("None");

        protected string GetGradingStatusHtml(Guid @event)
        {
            if (!Registrations.TryGetValue(@event, out var registrations))
                return "None";

            registrations = registrations
                .OrderBy(x => x.Candidate?.UserFullName)
                .ToList();

            var list = registrations.Select(x => x.GradingStatus)
                .GroupBy(x => x ?? "Not Graded")
                .ToList();

            var html = new StringBuilder();

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                html.Append($"<div class='text-nowrap'>{item.Key}: {item.Count()}</div>");
            }

            return html.ToString().IfNullOrEmpty("None");
        }

        protected string GetFormName(string forms)
        {
            return forms.IsEmpty()
                ? forms
                : string.Concat(forms.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => $"<p>{HttpUtility.HtmlEncode(x)}</p>"));
        }
    }
}