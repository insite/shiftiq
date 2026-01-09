using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Sites.Read;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.Persistence.Content;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Workflow.Forms.Models;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Portal.Sites
{
    public partial class PortalPage : PortalBasePage
    {
        private PortalPageModel _model;
        private System.Web.UI.Control _control;

        protected override void OnLoad(EventArgs e)
        {
            InitModel();
            BindModel();
            CreateControl();
            BindControl(CurrentSessionState.Identity);

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var master = (PortalMaster)Page.Master;
            var contentControl = Translate(_model.Page.ContentControl);
            master.Breadcrumbs.BindTitleAndSubtitleNoTranslate(_model.Title, $"{contentControl} {_model.LastUpdated} <span class='ms-2' title='Edit'>{_model.EditLinkUrl}</span>");
        }

        private void InitModel()
        {
            _model = LaunchCardBuilder.CreatePortalPage(
                Page.RouteData.Values,
                CurrentSessionState.Identity,
                Translator,
                CookieTokenModule.Current.Language);
        }

        private void BindModel()
        {
            if (_model.Page == null)
                return;

            Page.Title = _model.Page.Site.SiteTitle + " | " + _model.Page.Title;
        }

        private void CreateControl()
        {
            _control = LoadControl(_model.Control);
            BodyHolder.Controls.Add(_control);

            SupportPanel.Visible = _model.SupportUrl.HasValue();
            HelpRequestButton.NavigateUrl = _model.SupportUrl;

            var identity = CurrentSessionState.Identity;
            var translator = ((PortalBasePage)Page).Translator;
            var master = (PortalMaster)Page.Master;

            if (_model.Page.PageType == "xFolder")
            {
                master.SidebarVisible(false);
            }
            else
            {
                master.SidebarVisible(true);

                var siblings = _model.Page?.Parent != null
                    ? LoadSiblings()
                    : null;

                RelatedPageRepeater.DataSource = siblings;
                RelatedPageRepeater.DataBind();
                RelatedPageHeading.Visible = siblings.IsNotEmpty();
                RelatedPageHeading.InnerHtml = _model.ParentTitle ?? translator.Translate("Related Pages");
            }

            BindBreadcrumbs(_model.Page);
            BindSupportPanel(_model.Page, identity);
        }

        private void BindBreadcrumbs(QPage folder)
        {
            var page = VWebPageHierarchySearch.GetPage(folder.PageIdentifier);
            var gen = new BreadcrumbGenerator();
            var segments = gen.GetBreadcrumbs(page.PathUrl);
            var crumbs = new List<BreadcrumbItem>();

            for (var i = 1; i < segments.Count - 1; i++)
            {
                var href = segments[i].Substring(1);
                page = VWebPageHierarchySearch.GetPage(folder.OrganizationIdentifier, href);

                var pageTitle = ServiceLocator.ContentSearch.GetTitleText(page.WebPageIdentifier, CurrentLanguage).IfNullOrEmpty(page.WebPageTitle);
                crumbs.Add(new BreadcrumbItem(pageTitle, "/portals/" + href));
            }

            var folderTitle = ServiceLocator.ContentSearch.GetTitleText(folder.PageIdentifier, CurrentLanguage).IfNullOrEmpty(folder.Title);
            crumbs.Add(new BreadcrumbItem(folderTitle, null));

            var master = (PortalMaster)Page.Master;
            master.Breadcrumbs.BindBreadcrumbs(crumbs.ToArray());
        }

        private void BindControl(ISecurityFramework identity)
        {
            if (_control is IPortalPage p)
                p.BindModelToControls(_model, identity);
        }

        private void BindSupportPanel(QPage page, ISecurityFramework identity)
        {
            var supportUrl = ServiceLocator.ContentSearch.GetText(page.PageIdentifier, "Support URL");
            var url = identity.Organization.PlatformCustomization.PlatformUrl;
            _model.SupportUrl = supportUrl ?? url.Support ?? url.Contact ?? null;
            SupportPanel.Visible = _model.SupportUrl.HasValue();
            HelpRequestButton.NavigateUrl = _model.SupportUrl;
        }

        private List<LaunchCard> LoadSiblings()
        {
            var siblings = new List<LaunchCard>();

            if (_model.Page.Parent == null)
                return siblings;

            var identity = CurrentSessionState.Identity;

            var parent = ServiceLocator.PageSearch
                .Select(_model.Page.Parent.PageIdentifier, x => x.Children, x => x.Children.Select(y => y.Children));

            var containers = parent.Children.Select(x => x.PageIdentifier).ToList();

            var permissions = TGroupPermissionSearch.Select(x => containers.Any(y => y == x.ObjectIdentifier)).ToList();

            foreach (var child in parent.Children)
                TGroupPermissionSearch.SetIsAccessDenied(child, identity, permissions);

            foreach (var child in parent.Children.OrderBy(x => x.Sequence))
            {
                if (child.IsAccessDenied || child.IsHidden)
                    continue;

                var childContent = ServiceLocator.ContentSearch.GetBlock(
                    child.PageIdentifier,
                    CurrentLanguage,
                    new[] { ContentLabel.Title, ContentLabel.Summary, ContentLabel.ImageUrl });

                var item = new LaunchCard
                {
                    Icon = child.PageIcon ?? (child.PageType == "Folder" ? "far fa-folder" : "far fa-file-alt"),
                    Title = childContent.Title.GetText(CurrentLanguage).IfNullOrEmpty(child.Title),
                    Summary = childContent.Summary.GetText(CurrentLanguage)
                };

                if (childContent.Exists("ImageUrl"))
                    item.Image = childContent.GetText(ContentLabel.ImageUrl, CurrentLanguage);

                item.Url = !string.IsNullOrEmpty(child.NavigateUrl)
                    ? child.NavigateUrl
                    : GetPagePath(child);

                item.Active = child.PageIdentifier == _model.Page.PageIdentifier;
                siblings.Add(item);
            }

            return siblings;
        }

        public static string GetPagePath(QPage page)
        {
            if (page.ContentControl == "Survey" && page.ObjectType == "Survey" && page.ObjectIdentifier.HasValue)
            {
                var survey = ServiceLocator.SurveySearch.GetSurveyState(page.ObjectIdentifier.Value);
                if (survey != null)
                {
                    var user = CurrentSessionState.Identity.User;

                    if (user != null)
                    {
                        return survey.Form.RequireUserIdentification
                            ? FormUrl.GetStartUrl(survey.Form.Asset, user.UserIdentifier)
                            : FormUrl.GetStartUrl(survey.Form.Asset);
                    }
                    else
                        return FormUrl.GetStartUrl(survey.Form.Asset);
                }
            }

            var path = ServiceLocator.PageSearch.GetPagePath(page.PageIdentifier, false);
            if (path != null && page.OrganizationIdentifier == OrganizationIdentifiers.Global)
                return path.Replace("/portals/", "/help/");
            return path;
        }
    }

    public class BreadcrumbGenerator
    {
        public List<string> GetBreadcrumbs(string relativeUrl)
        {
            List<string> breadcrumbs = new List<string>();

            // Split the relative URL by '/' to get individual parts
            string[] urlParts = relativeUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // Add each part to the breadcrumbs list
            string accumulatedPath = string.Empty;
            foreach (string part in urlParts)
            {
                accumulatedPath += "/" + part; // Add the current part to the accumulated path
                breadcrumbs.Add(accumulatedPath);
            }

            return breadcrumbs;
        }
    }
}