using System;
using System.Web.UI.WebControls;

using InSite.Application.Courses.Read;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Courses.Courses
{
    public partial class Publish : AdminBasePage, IHasParentLinkParameters
    {
        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"course={CourseIdentifier}"
                : null;
        }

        protected Guid CourseIdentifier =>
            Guid.TryParse(Request.QueryString["course"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PublishMethod.AutoPostBack = true;
            PublishMethod.SelectedIndexChanged += PublishMethod_SelectedIndexChanged;

            WebSiteIdentifier.AutoPostBack = true;
            WebSiteIdentifier.ValueChanged += WebSiteIdentifier_ValueChanged;

            PublishButton.Click += PublishButton_Click;
            CancelButton.Click += CancelButton_Click;

            WebPageRequired.ServerValidate += WebPageRequired_ServerValidate;
        }

        private void PublishMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            PublishMethodSelected(PublishMethod.SelectedValue);
        }

        private void PublishMethodSelected(string method)
        {
            var isUpdate = method == "Update";

            WebPageLabel.Text = isUpdate
                ? "Web Site Page"
                : "Web Site Folder";

            WebPageHelp.InnerText = isUpdate
                ? "Select the web site page that is used to launch the course."
                : "Select the web site folder in which to publish the course.";

            WebPageRequired.ErrorMessage = isUpdate
                ? "Web Site Page is Required"
                : "Web Site Folder is Required";

            var filterPageType = isUpdate
                ? "Page"
                : "Folder";

            WebPageIdentifier.FixedPageType = filterPageType;
        }

        private void WebSiteIdentifier_ValueChanged(object sender, EventArgs e)
        {
            var siteId = WebSiteIdentifier.ValueAsGuid;

            WebPagePanel.Visible = siteId.HasValue;

            if (siteId.HasValue)
            {
                WebPageIdentifier.SiteId = siteId;

                if (WebPageIdentifier.Value.HasValue)
                {
                    var parentId = WebPageIdentifier.Value.Value;

                    if (!ServiceLocator.PageSearch.Exists(x => x.SiteIdentifier == siteId && x.PageIdentifier == parentId))
                        WebPageIdentifier.Value = null;
                }
                else
                {
                    WebPageIdentifier.Value = null;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var course = Persistence.CourseSearch.SelectCourse(CourseIdentifier);
            if (course == null)
                RedirectToSearch();

            SetInputValues(course);
        }

        private void SetInputValues(QCourse course)
        {
            PageHelper.AutoBindHeader(this, null, course.CourseName);

            CourseName.Text = course.CourseName;
            CourseAsset.Text = course.CourseAsset.ToString();

            PublishMethodSelected("Insert");
        }

        private void PublishButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToOutline();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToOutline();
        }

        private void WebPageRequired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = WebPageIdentifier.Value.HasValue;
        }

        private void Save()
        {
            var course = Persistence.CourseSearch.SelectCourse(CourseIdentifier);

            var isUpdate = PublishMethod.SelectedValue == "Update";

            if (isUpdate)
                UpdateWebPage(WebSiteIdentifier.ValueAsGuid.Value, WebPageIdentifier.Value.Value, course);
            else
                InsertWebPage(WebSiteIdentifier.ValueAsGuid.Value, WebPageIdentifier.Value.Value, course);
        }

        private void UpdateWebPage(Guid site, Guid pageId, QCourse course)
        {
            var page = ServiceLocator.PageSearch.Select(pageId);

            page.ContentControl = "Course";
            page.ObjectType = "Course";
            page.ObjectIdentifier = course.CourseIdentifier;
            page.NavigateUrl = null;
            page.Title = course.CourseName;
            page.PageSlug = course.CourseAsset.ToString();
            page.PageType = "Page";
            page.SiteIdentifier = site;
            page.AuthorDate = DateTimeOffset.Now;
            page.AuthorName = User.FullName;

            var commands = new PageCommandGenerator().
                GetDifferencePageSetupCommands(
                    ServiceLocator.PageSearch.Select(pageId),
                    page
                );
        }

        private void InsertWebPage(Guid site, Guid folder, QCourse course)
        {
            var page = new QPage
            {
                ContentControl = "Course",
                ObjectType = "Course",
                ObjectIdentifier = course.CourseIdentifier,
                Title = course.CourseName,
                OrganizationIdentifier = course.OrganizationIdentifier,
                ParentPageIdentifier = folder,
                PageSlug = course.CourseAsset.ToString(),
                PageIdentifier = UniqueIdentifier.Create(),
                PageType = "Page",
                SiteIdentifier = site,
                AuthorDate = DateTimeOffset.Now,
                AuthorName = User.FullName,
            };

            var commands = new PageCommandGenerator().GetCommands(page);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);
        }

        private void RedirectToOutline()
        {
            var url = GetOutlineUrl();
            HttpResponseHelper.Redirect(url, true);
        }

        private string GetOutlineUrl()
        {
            var url = $"/ui/admin/courses/manage?course={CourseIdentifier}";
            return url;
        }

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/courses/search", true);
    }
}
