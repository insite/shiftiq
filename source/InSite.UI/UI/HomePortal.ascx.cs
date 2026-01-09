using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using InSite.Application.Sites.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Content;
using InSite.UI.Portal.Sites;
using InSite.UI.Portal.Workflow.Forms.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI
{
    public partial class HomePortal : BaseUserControl
    {
        private AdminHomeSettings _userSettings;

        public static QSite CurrentSite
        {
            get
            {
                var organization = CurrentSessionState.Identity.Organization;

                var portalName = $"{organization.Code}.{ServiceLocator.AppSettings.Security.Domain}";

                return ServiceLocator.SiteSearch.BindFirst(x => x, x => x.SiteDomain == portalName);
            }
        }

        private AdminHomeSettings UserSettings => _userSettings
                                                  ?? (_userSettings =
                                                      PersonalizationRepository.GetValue<AdminHomeSettings>(Guid.Empty,
                                                          User.UserIdentifier, PersonalizationName.AdminHome, false) ??
                                                      new AdminHomeSettings());

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
                CheckRequiredUserFields();

            base.OnLoad(e);

            var tab = Page.Request.QueryString["tab"];

            var site = CurrentSite;
            Page.Title = site?.SiteTitle ?? "Shift iQ";

            var helper = new LaunchCardAdapter();

            var portals = helper.GetPortals(site?.SiteIdentifier, Identity);

            foreach (var portal in portals)
            {
                var item = LaunchCardBuilder.CreatePortalNavItem(Page, portal, 3, Nav);
                item.IsSelected = tab == portal.Slug;
            }

            var assessmentPanel = LaunchCardBuilder.CreateAssessmentNavItem(Page, Identity, 3, Nav);
            if (assessmentPanel.Visible)
            {
                assessmentPanel.IsSelected = tab == "assessment-pages";
            }

            if (!IsPostBack)
            {
                CheckForMandatorySurvey();
                LoadCustomContent();
            }
        }

        private void CheckRequiredUserFields()
        {
            if (User == null || Organization.Fields?.User == null)
                return;

            var user = UserSearch.Select(User.UserIdentifier);
            if (user == null)
                return;

            var type = user.GetType();

            foreach (var field in Organization.Fields.User)
            {
                if (field.IsRequired)
                {
                    var prop = type.GetProperty(field.FieldName);

                    if (prop != null)
                        if (prop.GetValue(user) == null)
                            Response.Redirect("/ui/portal/profile", true);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            HtmlGenericControl body = null;

            var master = Page.Master;
            while (body == null && master != null)
            {
                body = (HtmlGenericControl)master.FindControl("HtmlBody");
                master = master.Master;
            }

            if (body == null)
                return;

            if (UserSettings.WallpaperUrl.HasValue())
            {
                body.Style["background"] = $"no-repeat center top fixed url('{UserSettings.WallpaperUrl}')";
                body.Style["background-size"] = "cover !important";
            }
            else if (UserSettings.BackgroundColor.HasValue())
            {
                body.Style["background"] = UserSettings.BackgroundColor;
            }
        }

        private void LoadCustomContent()
        {
            if (Organization.Toolkits?.Accounts?.DisplayDashboardPrototype == true)
            {
                DashboardPrototype.Visible = true;
                return;
            }

            var domain = ServiceLocator.AppSettings.Security.Domain;
            var organization = CurrentSessionState.Identity.Organization;
            var host = $"{organization.Code}.{domain}";
            var site = ServiceLocator.SiteSearch.BindFirst(x => x.SiteIdentifier,
                x => x.OrganizationIdentifier == organization.Identifier && x.SiteDomain == host);

            if (site != Guid.Empty)
            {
                var block = ServiceLocator.ContentSearch.GetBlock(site);
                var title = block.Title.GetText();
                var summary = block.Summary.GetHtml();

                HomeTitle.Visible = title.HasValue();
                HomeTitle.InnerHtml = title;

                HomeBody.Visible = summary.HasValue();
                HomeBody.InnerHtml = Markdown.ToHtml(summary);
            }
        }

        private void CheckForMandatorySurvey()
        {
            foreach (var group in Identity.Groups)
            {
                var first = ServiceLocator.GroupSearch.GetGroup(group.Identifier)?.SurveyFormIdentifier;
                if (first == null)
                    continue;
                var form = ServiceLocator.SurveySearch.GetSurveyForm(first.Value);
                if (form == null)
                    continue;
                var sessions =
                    ServiceLocator.SurveySearch.GetResponseSessions(form.SurveyFormIdentifier, User.UserIdentifier);
                if (sessions.Count == 0)
                {
                    var url = FormUrl.GetStartUrl(form.AssetNumber, User.UserIdentifier);
                    HttpResponseHelper.Redirect(url);
                }
                else
                {
                    var session = sessions.FirstOrDefault(x => !x.ResponseSessionCompleted.HasValue);
                    if (session != null)
                    {
                        var url = FormUrl.GetResumeUrl(session.ResponseSessionIdentifier);
                        HttpResponseHelper.Redirect(url);
                    }
                }
            }
        }
    }
}