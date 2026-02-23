using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class EnrollmentUserGrid : SearchResultsGridViewController<QEnrollmentFilter>
    {
        public event EventHandler Refreshed;

        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        protected override bool IsFinder => false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_RowDeleting;
        }

        private void Grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var learnerId = grid.GetDataKey<Guid>(e);

            ServiceLocator.SendCommand(new DeleteEnrollment(Filter.GradebookIdentifier, learnerId));

            Refresh();
            OnRefreshed();
        }

        public void LoadData(Guid gradebookId)
        {
            Search(new QEnrollmentFilter
            {
                GradebookIdentifier = gradebookId
            });
        }

        public void Refresh()
        {
            SearchWithCurrentPageIndex(Filter);
        }

        protected override int SelectCount(QEnrollmentFilter filter)
        {
            return ServiceLocator.RecordSearch.CountEnrollments(filter);
        }

        protected override IListSource SelectData(QEnrollmentFilter filter)
        {
            if (filter == null)
                return null;

            return ServiceLocator.RecordSearch.GetEnrollments(filter, x => x.Learner).ToSearchResult();
        }
    }
}