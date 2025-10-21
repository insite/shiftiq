using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Domain.CourseObjects;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Learning.Models;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Learning
{
    public partial class Progress : PortalBasePage
    {
        private string CourseTitle
        {
            get => (string)ViewState[nameof(CourseTitle)];
            set => ViewState[nameof(CourseTitle)] = value;
        }

        private ProgressState _progress;
        private Dictionary<Guid, string> _resultCssClasses;

        private ProgressModel CreateModel()
        {
            _progress = new ProgressState(Page.RouteData, Request.QueryString);

            if (!ValidateModel())
                HttpResponseHelper.Redirect(RelativeUrl.PortalHomeUrl);

            _progress.LoadModelProgress();

            GetResultCssClasses();

            return _progress.Model;
        }

        private bool ValidateModel()
        {
            if (!_progress.LoadModel())
                return false;

            var org = _progress.Model?.Course?.Organization;

            if (org == null)
                return false;

            if (org == Organization.Identifier || org == Organization.ParentOrganizationIdentifier)
                return true;

            return false;
        }

        protected void BindModelToControls(ProgressModel model)
        {
            var master = (PortalMaster)Master;
            master.Breadcrumbs.BindTitleAndSubtitle("Progress Report and Scores", model.Course.Content.Title.GetText());
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UnitRepeater.ItemDataBound += UnitRepeater_ItemDataBound;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            var model = CreateModel();

            BindModelToControls(model);

            CourseTitle = model.Course.Content.Title.GetText();

            UnitRepeater.DataSource = model.Course.Units;
            UnitRepeater.DataBind();
        }

        private void UnitRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e.Item))
                return;

            var modules = ((InSite.Domain.CourseObjects.Unit)e.Item.DataItem).Modules;
            var repeater = (Repeater)e.Item.FindControl("ModuleRepeater");

            repeater.ItemDataBound += ModuleRepeater_ItemDataBound;
            repeater.DataSource = modules;
            repeater.DataBind();
        }

        private void ModuleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e.Item))
                return;

            var activities = ((Module)e.Item.DataItem).Activities;
            var repeater = (Repeater)e.Item.FindControl("ActivityRepeater");

            repeater.DataSource = activities;
            repeater.DataBind();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();

            using (var stringWriter = new StringWriter(sb))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                {
                    UnitRepeater.RenderControl(htmlWriter);
                }
            }

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                MarginTop = 22,
                HeaderSpacing = 7,
            };

            var logo = PathHelper.ToAbsoluteUrl(Identity.Organization.PlatformCustomization?.PlatformUrl?.Logo);

            var title = CourseTitle;
            var html = sb.ToString();
            var body = GetFileContent("~/UI/Portal/Learning/Content/Progress.html");
            html = body
                .Replace("<!-- LearnerName -->", User.FullName)
                .Replace("<!-- LearnerEmail -->", User.Email)
                .Replace("<!-- TitleHtml -->", title)
                .Replace("<!-- BodyHtml -->", html);

            if (logo != null)
                html = html.Replace("<!-- OrganizationLogo -->", $"<img src='{logo}' />");
            else
                html = html.Replace("<!-- OrganizationLogo -->", string.Empty);

            var data = HtmlConverter.HtmlToPdf(html, settings);
            if (data == null)
                return;

            var filename = StringHelper.Sanitize(title, '-', false);

            Response.SendFile(filename, "pdf", data);

            string GetFileContent(string virtualPath)
            {
                var physPath = MapPath(virtualPath);

                return File.ReadAllText(physPath);
            }
        }

        protected string GetResultIcon(Guid activity)
        {
            if (!_progress.Model.Results.ContainsKey(activity))
                return "-";

            var result = _progress.Model.Results[activity];
            return $"<i class='{result.Icon}'></i>";
        }

        protected string GetResultCssClass(Guid id)
        {
            return _resultCssClasses.ContainsKey(id) ? _resultCssClasses[id] : string.Empty;
        }

        private void GetResultCssClasses()
        {
            var course = _progress.Model.Course;

            _resultCssClasses = new Dictionary<Guid, string>();

            foreach (var activity in course.GetAllSupportedActivities())
            {
                if (!activity.IsAdaptive)
                    continue;

                var result = _progress.Model.Results.FirstOrDefault(x => x.Key == activity.Identifier);
                if (result.Value == null || result.Value.IsLocked)
                    _resultCssClasses.Add(activity.Identifier, "d-none");
            }

            foreach (var module in course.GetModules())
            {
                if (module.Activities.All(a => _resultCssClasses.ContainsKey(a.Identifier)))
                    _resultCssClasses.Add(module.Identifier, "d-none");
            }

            foreach (var unit in course.Units)
            {
                if (unit.Modules.All(m => _resultCssClasses.ContainsKey(m.Identifier)))
                    _resultCssClasses.Add(unit.Identifier, "d-none");
            }
        }

        protected string GetResultScore(Guid activity)
        {
            if (!_progress.Model.Results.ContainsKey(activity))
                return "-";

            var result = _progress.Model.Results[activity];

            return result.Score.HasValue ? $"{result.Score:p0}" : result.Status;
        }

        protected string GetContentText(string name)
        {
            var dataItem = Page.GetDataItem();
            var content = (ContentContainer)DataBinder.Eval(dataItem, name);

            return content?.Title?.GetText();
        }
    }
}