using System;
using System.Collections.Generic;
using System.ComponentModel;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Records.Scores.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QProgressFilter>
    {
        protected override int SelectCount(QProgressFilter filter)
        {
            return ServiceLocator.RecordSearch.CountGradebookScores(filter);
        }

        protected override IListSource SelectData(QProgressFilter filter)
        {
            return ServiceLocator
                .RecordSearch
                .GetGradebookScores(
                    filter,
                    x => x.Gradebook.Event, x => x.Gradebook.Achievement, x => x.Learner, x => x.GradeItem, x => x.Gradebook)
                .ToSearchResult();
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        protected string GetScoreValue(object score)
        {
            var progress = (QProgress)score;
            return GradebookHelper.GetScoreValue(progress, progress.GradeItem?.GradeItemFormat);
        }

        #region Export

        public class ExportDataItem : QProgress
        {
            public string GradebookTitle { get; internal set; }
            public string GradebookItemName { get; internal set; }
            public string GradeItemType { get; internal set; }
            public string GradeItemFormat { get; internal set; }
            public string LearnerCode { get; internal set; }
            public string LearnerEmail { get; internal set; }
            public string LearnerName { get; internal set; }
            public string ClassTitle { get; internal set; }
            public DateTimeOffset ClassScheduledStartDate { get; internal set; }
            public DateTimeOffset? ClassScheduledEndDate { get; internal set; }
            public string AchievementTitle { get; internal set; }
            public string Score { get; internal set; }
        }

        public override IListSource GetExportData(QProgressFilter filter, bool empty)
        {
            var data = ServiceLocator.RecordSearch.GetGradebookScores(
                filter,
                x => x.Gradebook.Event, x => x.Gradebook.Achievement, x => x.Learner, x => x.GradeItem
            );

            CallUpdateDateTimeOffsets(data, User.TimeZoneId);

            var result = new List<ExportDataItem>(data.Count);

            foreach (var dataItem in data)
            {
                var exportItem = new ExportDataItem();

                dataItem.ShallowCopyTo(exportItem);

                if (dataItem.Gradebook != null)
                {
                    exportItem.GradebookTitle = dataItem.Gradebook.GradebookTitle;
                }

                if (dataItem.GradeItem != null)
                {
                    exportItem.GradebookItemName = dataItem.GradeItem.GradeItemName;
                    exportItem.GradeItemType = dataItem.GradeItem.GradeItemType;
                    exportItem.GradeItemFormat = dataItem.GradeItem.GradeItemFormat;
                    exportItem.Score = GradebookHelper.GetScoreValue(dataItem, dataItem.GradeItem.GradeItemFormat);
                }

                if (dataItem.Learner != null)
                {
                    exportItem.LearnerCode = PersonCriteria.BindFirst(
                        x => x.PersonCode,
                        new PersonFilter
                        {
                            OrganizationIdentifier = Organization.Identifier,
                            UserIdentifier = dataItem.UserIdentifier
                        }); 
                    exportItem.LearnerEmail = dataItem.Learner.UserEmail;
                    exportItem.LearnerName = dataItem.Learner.UserFullName;
                }

                if (dataItem.Gradebook.Event != null)
                {
                    exportItem.ClassTitle = dataItem.Gradebook.Event.EventTitle;
                    exportItem.ClassScheduledStartDate = dataItem.Gradebook.Event.EventScheduledStart;
                    exportItem.ClassScheduledEndDate = dataItem.Gradebook.Event.EventScheduledEnd;
                }

                if (dataItem.Gradebook.Achievement != null)
                {
                    exportItem.AchievementTitle = dataItem.Gradebook.Achievement.AchievementTitle;
                }

                result.Add(exportItem);
            }

            return result.ToSearchResult();
        }

        #endregion
    }
}