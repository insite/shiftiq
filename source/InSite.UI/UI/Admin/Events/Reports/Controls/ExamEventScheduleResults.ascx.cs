using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Events.Reports.Controls
{
    public partial class ExamEventScheduleResults : SearchResultsGridViewController<VExamEventScheduleFilter>
    {
        private class ExportDataItem
        {
            public Guid EventIdentifier { get; set; }
            public Guid OrganizationIdentifier { get; set; }

            public string AccommodationSummary { get; set; }
            public string EventClassCode { get; set; }
            public string EventFormat { get; set; }
            public string EventSchedulingStatus { get; set; }
            public string Eligibility { get; set; }
            public string FormBankTypes { get; set; }
            public string FormBankLevels { get; set; }
            public string PhysicalAddress { get; set; }
            public string PhysicalCity { get; set; }
            public string PhysicalProvince { get; set; }
            public string Trades { get; set; }
            public string VenueName { get; set; }
            public string VenueOffice { get; set; }
            public string VenueRoom { get; set; }

            public int? AccommodationCount { get; set; }
            public int EventNumber { get; set; }
            public string EventBillingType { get; set; }
            public int? CandidateCount { get; set; }
            public int? CapacityMaximum { get; set; }
            public int? InvigilatorCount { get; set; }
            public int? InvigilatorMinimum { get; set; }

            public DateTimeOffset EventScheduledStart { get; set; }
        }

        protected override int SelectCount(VExamEventScheduleFilter filter)
        {
            return VExamEventScheduleSearch.CountByFilter(filter);
        }

        protected override IListSource SelectData(VExamEventScheduleFilter filter)
        {
            return VExamEventScheduleSearch
                .SelectByFilter(filter)
                .ToSearchResult();
        }

        public override IListSource GetExportData(VExamEventScheduleFilter filter, bool empty)
        {
            if (empty)
                return (new ExportDataItem[0]).ToSearchResult();

            var data = VExamEventScheduleSearch.SelectByFilter(filter);
            
            var accommodationSummaries = VExamEventScheduleSearch.SelectAccommodationSummaries(filter);
            var accommodationSummaryMap = accommodationSummaries
                .GroupBy(x => x.EventIdentifier)
                .ToDictionary(x => x.Key, x => x.ToList());

            var result = new List<ExportDataItem>(data.Count);

            foreach (var dataItem in data)
            {
                var exportItem = new ExportDataItem();
                dataItem.ShallowCopyTo(exportItem);

                if (accommodationSummaryMap.TryGetValue(dataItem.EventIdentifier, out var eventAccommodationSummaries))
                {
                    var list = eventAccommodationSummaries
                        .OrderBy(x => x.AccommodationType)
                        .Select(x => $"{x.AccommodationType}: {x.RegistrationCount}")
                        .ToList();

                    exportItem.AccommodationSummary = string.Join("; ", list);
                }

                result.Add(exportItem);
            }

            return result.ToSearchResult();
        }

        protected string GetDateString(DateTimeOffset date)
        {
            return TimeZones.Format(date, Identity.User.TimeZone, true, false);
        }

        protected string GetAccommodationsHtml(Guid @event)
        {
            var registrations = ServiceLocator.RegistrationSearch.GetRegistrationsByEvent(@event, null, null, null, true);
            var accommodations = registrations.SelectMany(x => x.Accommodations)
                .GroupBy(x => x.AccommodationType)
                .OrderBy(x => x.Key)
                .ToList();

            var html = new StringBuilder();

            if (accommodations.Count == 0)
                html.Append("None");
            else
                for (var i = 0; i < accommodations.Count; i++)
                {
                    var accommodation = accommodations[i];
                    html.Append($"<div>{accommodation.Key}: {accommodation.Count()}</div>");
                }
            
            return html.ToString();
        }
    }
}