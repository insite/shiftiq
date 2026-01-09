using System;
using System.Collections.Generic;
using System.ComponentModel;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QJournalSetupFilter>
    {
        public class ExportDataItem
        {
            public Guid Identifier { get; set; }
            public string LogbookName { get; set; }
            public string AchievementTitle { get; internal set; }
            public string ClassTitle { get; internal set; }
            public DateTimeOffset? ClassScheduledStartDate { get; internal set; }
            public DateTimeOffset? ClassScheduledEndDate { get; internal set; }
            public bool IsLocked { get; set; }
        }

        public bool IsValidator { get; set; }

        protected string OutlineURL => IsValidator
            ? "/ui/admin/records/logbooks/validators/outline"
            : "/ui/admin/records/logbooks/outline";

        protected override int SelectCount(QJournalSetupFilter filter)
        {
            return ServiceLocator.JournalSearch.CountJournalSetups(filter);
        }

        protected override IListSource SelectData(QJournalSetupFilter filter)
        {
            return ServiceLocator.JournalSearch
                .GetJournalSetups(filter, x => x.Event, x => x.Achievement)
                .ToSearchResult();
        }

        public override IListSource GetExportData(QJournalSetupFilter filter, bool empty)
        {
            var data = ServiceLocator.JournalSearch.GetJournalSetups(
                filter,
                x => x.Event,
                x => x.Achievement);

            var result = new List<ExportDataItem>(data.Count);

            foreach (var dataItem in data)
            {
                var exportItem = new ExportDataItem
                {
                    Identifier = dataItem.JournalSetupIdentifier,
                    LogbookName = dataItem.JournalSetupName,
                    AchievementTitle = dataItem.Achievement?.AchievementTitle,
                    ClassTitle = dataItem.Event?.EventTitle,
                    ClassScheduledStartDate = dataItem.Event?.EventScheduledStart,
                    ClassScheduledEndDate = dataItem.Event?.EventScheduledEnd,
                    IsLocked = dataItem.JournalSetupLocked.HasValue
                };

                result.Add(exportItem);
            }

            return result.ToSearchResult();
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        protected static string GetLockedStatusEval(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.HasValue ? "<i class='text-danger fas fa-lock ps-2'></i>" : String.Empty;
        }
        
    }
}