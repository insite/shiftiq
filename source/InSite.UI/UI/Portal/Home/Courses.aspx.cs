using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Home
{
    public partial class Courses : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CoursesList.ItemDataBound += CoursesList_ItemDataBound;

            MyCoursesComboBox.AutoPostBack = true;
            MyCoursesComboBox.ValueChanged += MyCoursesComboBox_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            MyCoursesComboBox.RefreshData();

            BindMyCourses();

            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);
        }

        private void BindMyCourses()
        {
            var value = MyCoursesComboBox.Value;
            if (string.IsNullOrEmpty(value))
                return;

            var completed = CourseSearch.GetMyCompletedCourses(
                CurrentSessionState.Identity.User.UserIdentifier,
                CurrentSessionState.Identity.Organization.OrganizationIdentifier);

            if (value.ToLower().Equals("enrolled"))
            {
                var enrollment = CourseSearch.GetMyEnrolledCourses(
                    CurrentSessionState.Identity.User.UserIdentifier,
                    CurrentSessionState.Identity.Organization.OrganizationIdentifier);

                foreach (var item in enrollment)
                {
                    if (completed.FirstOrDefault(x => x.CourseIdentifier == item.CourseIdentifier) != null)
                        item.IsCompleted = true;
                }

                CoursesList.DataSource = enrollment;
                CoursesList.DataBind();

                if (enrollment.Length == 0)
                    StatusAlert.AddMessage(AlertType.Information, GetDisplayText("You have not been enrolled in a Course."));
            }
            else if (value.ToLower().Equals("completed"))
            {
                CoursesList.DataSource = completed;
                CoursesList.DataBind();

                if (completed.Length == 0)
                    StatusAlert.AddMessage(AlertType.Information, GetDisplayText("You have not yet completed a Course."));
            }
        }

        private void CoursesList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (CourseSearch.MyRecordedCourses)e.Item.DataItem;

                if (!string.IsNullOrEmpty(item.PageCourseImage))
                {
                    var image = $"<img class='card-img-top' src='{item.PageCourseImage}' alt=''>";
                    var cardImage = (ITextControl)e.Item.FindControl("CardImage");
                    cardImage.Text = image;
                }

                if (item.IsCompleted)
                {
                    var badge = $"<span class='position-absolute badge badge-success'>Completed</span>";
                    var cardBadge = (ITextControl)e.Item.FindControl("CardBadge");
                    cardBadge.Text = badge;
                }
            }
        }

        private void MyCoursesComboBox_ValueChanged(object sender, Shift.Sdk.UI.ComboBoxValueChangedEventArgs e)
        {
            BindMyCourses();
        }

        protected string GetCourseUrl()
        {
            var data = (CourseSearch.MyRecordedCourses)GetDataItem();
            return data.CourseUrl
                ?? $"/ui/portal/learning/course/{data.CourseIdentifier}";
        }
    }
}