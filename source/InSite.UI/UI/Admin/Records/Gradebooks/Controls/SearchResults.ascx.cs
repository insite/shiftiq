using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QGradebookFilter>
    {
        protected override int SelectCount(QGradebookFilter filter)
        {
            return ServiceLocator.RecordSearch.CountGradebooks(filter);
        }

        protected override IListSource SelectData(QGradebookFilter filter)
        {
            return ServiceLocator.RecordSearch
                .GetGradebooks(filter, x => x.Event, x => x.Achievement, x => x.Enrollments)
                .ToSearchResult();
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        protected int GrantedAchievements(Guid? gradebookIdentifier, Guid? achievementIdentifier)
        {
            if (!gradebookIdentifier.HasValue || !achievementIdentifier.HasValue)
                return 0;

            var filter = new VCredentialFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                ItemGradebookIdentifier = gradebookIdentifier,
                UserGradebookIdentifier = gradebookIdentifier,
                IsGranted = true,
                AchievementIdentifier = null,
            };

            return ServiceLocator.AchievementSearch.CountCredentials(filter);
        }

        #region Export

        public class ExportDataItem : QGradebook
        {
            public string AchievementTitle { get; internal set; }
            public string ClassTitle { get; internal set; }
            public DateTimeOffset ClassScheduledStartDate { get; internal set; }
            public DateTimeOffset? ClassScheduledEndDate { get; internal set; }
            public string ClassInstructors { get; set; }
        }

        protected static string DateToHtml(object date)
            => (date == null) ? string.Empty : TimeZones.Format((DateTimeOffset)date, User.TimeZone, true);

        public override IListSource GetExportData(QGradebookFilter filter, bool empty)
        {
            var data = ServiceLocator.RecordSearch.GetGradebooks(
                filter,
                x => x.Event,
                x => x.Achievement);

            var result = new List<ExportDataItem>(data.Count);

            foreach (var dataItem in data)
            {
                var exportItem = new ExportDataItem();

                dataItem.ShallowCopyTo(exportItem);

                if (dataItem.Event != null)
                {
                    exportItem.ClassTitle = dataItem.Event.EventTitle;
                    exportItem.ClassScheduledStartDate = dataItem.Event.EventScheduledStart;
                    exportItem.ClassScheduledEndDate = dataItem.Event.EventScheduledEnd;

                    var instructors = ServiceLocator.EventSearch.GetAttendees(
                        new QEventAttendeeFilter { EventIdentifier = dataItem.Event.EventIdentifier, ContactRole = "Instructor" },
                        null, null, x => x.Person.User);

                    if (instructors.Count > 0)
                        exportItem.ClassInstructors = string.Join("; ", instructors.Select(x => x.UserFullName));
                }

                if (dataItem.Achievement != null)
                {
                    exportItem.AchievementTitle = dataItem.Achievement.AchievementTitle;
                }

                result.Add(exportItem);
            }

            return result.ToSearchResult();
        }

        #endregion
    }
}