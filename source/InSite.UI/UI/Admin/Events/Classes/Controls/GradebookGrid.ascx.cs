using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Events.Classes.Controls
{
    public partial class GradebookGrid : SearchResultsGridViewController<QGradebookFilter>
    {
        protected override bool IsFinder => false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            var gradebookId = Guid.Parse(e.CommandArgument.ToString());
            var eventId = Filter.GradebookEventIdentifier ?? throw new ArgumentNullException("GradebookEventIdentifier");

            ServiceLocator.SendCommand(new RemoveGradebookEvent(gradebookId, eventId));

            SearchWithCurrentPageIndex(Filter);
        }

        public int LoadData(Guid eventIdentifier)
        {
            var filter = new QGradebookFilter { GradebookEventIdentifier = eventIdentifier };

            Search(filter);

            return RowCount;
        }

        protected override int SelectCount(QGradebookFilter filter)
        {
            return ServiceLocator.RecordSearch.CountGradebooks(filter);
        }

        protected override IListSource SelectData(QGradebookFilter filter)
        {
            return ServiceLocator.RecordSearch
                .GetGradebooks(filter, x => x.Items.Select(y => y.Achievement))
                .ToSearchResult();
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        protected string GetAchievements()
        {
            var gradebook = (QGradebook)Page.GetDataItem();
            var achievements = gradebook.Items.Where(x => x.Achievement != null)
                .GroupBy(x => x.Achievement.AchievementIdentifier)
                .Select(x => x.First().Achievement.AchievementTitle)
                .OrderBy(x => x).ToArray();

            return string.Join(", ", achievements);
        }

        protected bool CanDelete()
        {
            var gradebook = (QGradebook)Page.GetDataItem();
            if (gradebook.IsLocked)
                return false;

            return !ServiceLocator.RecordSearch.EnrollmentExists(gradebook.GradebookIdentifier);
        }
    }
}