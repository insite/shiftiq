using System;
using System.Linq;
using System.Web.Routing;
using System.Web.UI;

using InSite.Admin.Assets.Contents.Utilities;
using InSite.Application.Sites.Read;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.Persistence.Content;
using InSite.UI.Portal.Assessments.Attempts.Utilities;
using InSite.UI.Portal.Workflow.Forms.Models;
using InSite.Web.Routing;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Sites
{
    public class LaunchCardBuilder
    {
        public static PortalPageModel CreatePortalPage(
            RouteValueDictionary parameters,
            ISecurityFramework identity,
            InputTranslator translator,
            string language
            )
        {
            var path = parameters["path"];
            if (path == null)
                HttpResponseHelper.Redirect(RelativeUrl.PortalHomeUrl);

            var hasWriteAccess = identity.IsGranted(PermissionIdentifiers.Admin_Sites, PermissionOperation.Write);

            var appUrl = ServiceLocator.Urls.GetApplicationUrl(identity.Organization.Code);

            var model = new PortalPageModel();

            model.Language = language;
            model.Path = path.ToString();
            model.Page = GetPage(model.Path);

            if (model.Page.ObjectType == "Course" && model.Page.ObjectIdentifier.HasValue)
            {
                if (TGroupPermissionSearch.IsAccessAllowed(model.Page.ObjectIdentifier.Value, CurrentSessionState.Identity))
                    HttpResponseHelper.Redirect(RoutingConfiguration.PortalCourseUrl(model.Page.ObjectIdentifier.Value));
                else
                    HttpResponseHelper.SendHttp404(model.Path);
            }

            model.Control = GetUserControlPath(model.Page.PageType, model.Page.ContentControl);

            if (!model.Path.StartsWith("/portals/"))
                model.Path = "/portals/" + model.Path;

            var active = model.Path == model.NavigateUrl;
            Load(model, identity, translator, language, active);

            model.SetEditLinkUrl(identity.IsAuthenticated, identity.IsOperator, hasWriteAccess, appUrl);

            return model;
        }

        private static void Load(PortalPageModel model, ISecurityFramework identity, InputTranslator translator, string language, bool active)
        {
            var timezone = identity.User?.TimeZone ?? identity.Organization.TimeZone;

            var url = identity.Organization.PlatformCustomization.PlatformUrl;
            var supportUrl = url.Support ?? url.Contact ?? "#";

            if (model.Page == null)
            {
                model.Title = "Page Not Found";
                model.Icon = "far fa-bug";
                model.Identifier = Guid.Empty;
                model.Body = "The page you have requested cannot be found on this web site.";
                model.LastUpdated = "-";
                model.NavigateUrl = "#";
                model.SupportUrl = supportUrl;
                return;
            }

            var content = ServiceLocator.ContentSearch.GetBlock(
                model.Page.PageIdentifier,
                language,
                new[] { "Title", "Summary", "Body" });

            model.CssClass = active ? "active" : string.Empty;
            model.Title = (content.Title?.GetText(language)).IfNullOrEmpty(model.Page.Title);
            model.Icon = model.Page.PageIcon;
            model.Identifier = model.Page.PageIdentifier;
            model.Body = content.Body?.GetHtml(language);
            model.LastUpdated = translator.Translate("last updated").ToLower() + " " + TimeZones.Format(model.Page.LastChangeTime.Value, timezone, true);
            model.NavigateUrl = ServiceLocator.PageSearch.GetPagePath(model.Page.PageIdentifier, false);
            model.SupportUrl = supportUrl;

            if (model.Page.Parent != null)
            {
                var parentContent = ServiceLocator.ContentSearch.GetBlock(model.Page.Parent.PageIdentifier);
                model.ParentNavigateUrl = ServiceLocator.PageSearch.GetPagePath(model.Page.Parent.PageIdentifier, false);
                model.ParentTitle = parentContent.Title.GetText(language).IfNullOrEmpty(model.Page.Parent.Title);
            }

            model.Progress = GetProgress(model.Page.Hook, model.Page.ObjectIdentifier);
        }

        public static string GetProgress(string hook, Guid? course)
        {
            if (!hook.IsEmpty())
            {
                var user = CurrentSessionState.Identity.User.UserIdentifier;
                var item = ServiceLocator.RecordSearch.GetGradeItemByHook(hook);

                if (item == null)
                    return null;

                var progress = ServiceLocator.RecordSearch
                    .GetProgress(item.GradebookIdentifier, item.GradeItemIdentifier, user);

                if (progress == null)
                    return null;

                if (item.GradeItemType == "Score")
                {
                    if (progress.ProgressPercent == 1)
                        return "Completed";
                    else if (progress.ProgressPercent >= 0)
                        return "Started";
                }
                else if (item.GradeItemType == "Category")
                {
                    if (progress.ProgressPercent == 1)
                        return "Completed";
                    else if (progress.ProgressPercent > 0)
                        return "Started";
                }
            }
            else if (course.HasValue)
            {
                var user = CurrentSessionState.Identity.User.UserIdentifier;
                var achievementId = CourseSearch.BindCourseFirst(x => x.Gradebook.AchievementIdentifier, x => x.CourseIdentifier == course);
                if (!achievementId.HasValue)
                    return null;

                var certificate = ServiceLocator.AchievementSearch.GetCredential(achievementId.Value, user);
                if (certificate == null)
                    return null;

                return certificate.CredentialStatus;
            }

            return null;
        }

        private static QPage GetPage(string path)
        {
            var identity = CurrentSessionState.Identity;
            var organization = identity.Organization.Key;

            var node = VWebPageHierarchySearch.GetPage(organization, path);
            if (node == null)
                HttpResponseHelper.SendHttp404(path);

            var page = ServiceLocator.PageSearch.Select(node.WebPageIdentifier, x => x.Site, x => x.Parent, x => x.Children, x => x.Children.Select(y => y.Children));

            var containers = new Guid[] { page.PageIdentifier };

            var permissions = TGroupPermissionSearch.Select(x => containers.Any(y => y == x.ObjectIdentifier)).ToList();

            TGroupPermissionSearch.SetIsAccessDenied(page, identity, permissions);

            return page;
        }

        private static string GetUserControlPath(string pageType, string layoutType)
        {
            var control = "Article";

            if (pageType == "Folder")
                control = pageType;

            if (layoutType == "Catalog")
                control = layoutType;

            return $"~/UI/Portal/Content/Sites/Controls/{control}.ascx";
        }

        public static NavItem CreatePortalNavItem(Page page, LaunchCard portal, int columnSize, Nav nav)
        {
            var item = new NavItem { Title = portal.Title };
            if (portal.Icon != null)
                item.Icon = portal.Icon;

            var repeater = (Controls.LaunchCardRepeater)page.LoadControl("~/UI/Portal/Controls/LaunchCardRepeater.ascx");
            repeater.PortalIdentifier = portal.Identifier;
            repeater.PortalBodyHtml = portal.BodyHtml;
            repeater.PortalSlug = portal.Slug;
            repeater.ColumnSize = columnSize;
            item.Controls.Add(repeater);

            var identity = CurrentSessionState.Identity;

            var appUrl = ServiceLocator.Urls.GetApplicationUrl(identity.Organization.Code);

            var caller = FormCaller.CreatePortal(portal.Slug).Serialize();

            var helper = new LaunchCardAdapter();

            var cards = helper.GetLaunchCards(appUrl, portal.Identifier, portal.Title, identity, caller, LabelHelper.GetTranslation, GetProgress);

            item.Visible = cards.Count > 0;

            nav.AddItem(item);

            repeater.BindModelToControls(cards, identity);

            return item;
        }

        public static NavItem CreateAssessmentNavItem(Page page, ISecurityFramework identity, int columnSize, Nav nav)
        {
            var repeater = (Controls.LaunchCardRepeater)page.LoadControl("~/UI/Portal/Controls/LaunchCardRepeater.ascx");
            repeater.ColumnSize = columnSize;

            var item = new NavItem { Title = "Assessments", Icon = "fas fa-users-class" };
            item.Controls.Add(repeater);

            var helper = new LaunchCardAdapter();

            var cards = helper.GetAssessmentCards(identity, AttemptUrlForm.GetStartUrl);

            item.Visible = cards.Count > 0;

            nav.AddItem(item);

            repeater.BindModelToControls(cards, identity);

            return item;
        }
    }
}