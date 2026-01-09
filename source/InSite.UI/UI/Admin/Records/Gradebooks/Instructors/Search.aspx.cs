using System;

using InSite.Application.Records.Read;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Records.Instructors
{
    public partial class Search : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();
                GradebooksTitle.Text = $"{User.FullName}'s Gradebooks";
                GradebooksGuide.Text = "Select a gradebook to edit and manage scores.";
            }
        }

        private void LoadData()
        {
            var filter = new QGradebookFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                EventInstructorIdentifier = User.UserIdentifier,
                IsLocked = false,
                IsEventCancelled = false
            };

            var data = ServiceLocator.RecordSearch.GetGradebooks(filter, null, null, x => x.Event);

            GradebookRepeater.DataSource = data;
            GradebookRepeater.DataBind();

            NoGradebooks.Visible = (data.Count == 0);
            GradebookRepeater.Visible = !(data.Count == 0);
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.Format(User.TimeZone, nullValue: string.Empty);
        }
    }
}