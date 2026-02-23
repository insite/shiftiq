using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class EnrollmentGroupGrid : SearchResultsGridViewController<QGroupEnrollmentFilter>
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
            var enrollmentId = grid.GetDataKey<Guid>(e);

            ServiceLocator.SendCommand(new RemoveGradebookGroupEnrollment(Filter.GradebookIdentifier.Value, enrollmentId));

            Refresh();
            OnRefreshed();
        }

        public void LoadData(Guid gradebookId)
        {
            Search(new QGroupEnrollmentFilter
            {
                GradebookIdentifier = gradebookId
            });
        }

        public void Refresh()
        {
            SearchWithCurrentPageIndex(Filter);
        }

        protected override int SelectCount(QGroupEnrollmentFilter filter)
        {
            return ServiceLocator.RecordSearch.CountGroupEnrollments(filter);
        }

        protected override IListSource SelectData(QGroupEnrollmentFilter filter)
        {
            if (filter == null)
                return null;

            return ServiceLocator.RecordSearch.GetGroupEnrollments(filter, x => x.Group.QMemberships).ToSearchResult();
        }
    }
}