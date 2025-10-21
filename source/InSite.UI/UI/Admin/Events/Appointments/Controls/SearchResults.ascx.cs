using System;
using System.Collections.Generic;
using System.ComponentModel;

using Humanizer;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Events.Appointments.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QEventFilter>
    {
        protected override int SelectCount(QEventFilter filter)
        {
            return ServiceLocator.EventSearch.CountEvents(filter);
        }

        protected override IListSource SelectData(QEventFilter filter)
        {
            return ServiceLocator.EventSearch
                .GetEvents(filter)
                .ToSearchResult();
        }

        #region Export

        public class ExportDataItem
        {
            public string EventDate { get; set; }
            public string EventDescription { get; set; }
            public Guid EventIdentifier { get; set; }
            public DateTimeOffset? EventScheduledEnd { get; set; }
            public DateTimeOffset EventScheduledStart { get; set; }
            public string EventTitle { get; set; }
            public string EventType { get; set; }
            public string AppointmentType { get; set; }
            public DateTimeOffset? LastChangeTime { get; set; }
            public string LastChangeType { get; set; }
            public string LastChangeUser { get; set; }
        }

        public override IListSource GetExportData(QEventFilter filter, bool empty)
        {
            var data = ServiceLocator.EventSearch.GetEvents(filter, x => x.Achievement, x => x.Registrations);

            var result = new List<ExportDataItem>(data.Count);

            foreach (var dataItem in data)
            {
                var exportItem = new ExportDataItem();

                dataItem.ShallowCopyTo(exportItem);

                result.Add(exportItem);
            }

            return result.ToSearchResult();
        }

        #endregion

        #region Methods (render helpers)

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.Format(User.TimeZone, true);
        }

        protected static string FormatDescription(object item)
        {
            var description = (string)item;
            if (string.IsNullOrEmpty(description))
                return description;
            return StringHelper.ReplaceNewLinesWithHtmlBreaks(description);
        }


        protected string GetDurationHtml(object item)
        {
            var @event = (QEvent)item;
            if (@event.DurationQuantity.HasValue && @event.DurationUnit.HasValue())
                return @event.DurationUnit.ToQuantity(@event.DurationQuantity.Value);
            return string.Empty;
        }

        protected static string GetPublicationStatus(object status)
        {
            var isPublished = string.Equals(status?.ToString() ?? "", PublicationStatus.Published.GetDescription(), StringComparison.OrdinalIgnoreCase);
            if (isPublished) return "<span class=\"badge bg-success\">Published</span>";
            return "<span>Drafted</span>";
        }

        #endregion
    }
}