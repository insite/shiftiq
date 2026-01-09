using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Sites.Read;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.Persistence.Content;
using InSite.UI.Portal.Workflow.Forms.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Sites
{
    public partial class PageFolder : UserControl, IPortalPage
    {
        public PortalPageModel Model { get; set; }

        public PortalPageList Folder { get; set; }

        public void BindModelToControls(PortalPageModel model, ISecurityFramework identity)
        {
            Model = model;

            LoadModel(Model.Page, identity);

            BindBodyPanel(identity);
        }

        private void LoadModel(QPage folder, ISecurityFramework identity)
        {
            InitFolder(folder, identity);

            foreach (var child in folder.Children
                .Where(c => !string.Equals(c.PageType, "Block", StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Sequence))
            {
                var permissions = TGroupPermissionSearch.Select(x => x.ObjectIdentifier == child.PageIdentifier).ToList();
                TGroupPermissionSearch.SetIsAccessDenied(child, identity, permissions);

                if (child.IsAccessDenied || child.IsHidden)
                    continue;

                if (child.ObjectType == "Course" && child.ObjectIdentifier.HasValue && TGroupPermissionSearch.IsAccessDenied(child.ObjectIdentifier.Value, CurrentSessionState.Identity))
                    continue;

                var content = ServiceLocator.ContentSearch.GetBlock(
                    child.PageIdentifier,
                    identity.Language,
                    new[] { ContentLabel.Title, ContentLabel.Summary, ContentLabel.ImageUrl });

                var item = new LaunchCard
                {
                    Icon = child.PageIcon ?? (child.PageType == "Folder" ? "far fa-folder" : "far fa-file-alt"),
                    Title = content.Title.GetText(identity.Language).IfNullOrEmpty(child.Title),
                    Summary = content.Summary.GetText(identity.Language)
                };

                if (content.Exists("ImageUrl"))
                    item.Image = content.GetText(ContentLabel.ImageUrl, identity.Language);

                if (!string.IsNullOrEmpty(child.NavigateUrl))
                {
                    item.Url = child.NavigateUrl;
                }
                else if (!string.IsNullOrEmpty(child.ContentControl)
                    && child.ContentControl == "Survey"
                    && child.ObjectType == "Survey"
                    && child.ObjectIdentifier.HasValue)
                {
                    var survey = ServiceLocator.SurveySearch.GetSurveyState(child.ObjectIdentifier.Value);
                    if (survey != null)
                    {
                        var user = CurrentSessionState.Identity.User;

                        if (user != null)
                        {
                            item.Url = survey.Form.RequireUserIdentification
                                ? FormUrl.GetStartUrl(survey.Form.Asset, user.UserIdentifier)
                                : FormUrl.GetStartUrl(survey.Form.Asset);
                        }
                        else
                            item.Url = FormUrl.GetStartUrl(survey.Form.Asset);
                    }
                }
                else
                {
                    item.Url = ServiceLocator.PageSearch.GetPagePath(child.PageIdentifier, false);
                }

                item.Progress = new Flag { Text = LaunchCardBuilder.GetProgress(child.Hook, child.ObjectIdentifier) };

                AddFolderForDocumentSearch(folder, item);

                Folder.Items.Add(item);
            }
        }

        private void InitFolder(QPage folder, ISecurityFramework identity)
        {
            var timezone = identity.User?.TimeZone ?? identity.Organization.TimeZone;

            var content = ServiceLocator.ContentSearch.GetBlock(
                folder.PageIdentifier,
                identity.Language,
                new[] { ContentLabel.Title, ContentLabel.Summary, ContentLabel.Body });

            Folder = new PortalPageList
            {
                Icon = folder.PageIcon,
                Body = content.Body.GetHtml(identity.Language),
                Title = content.Title.GetText(identity.Language).IfNullOrEmpty(folder.Title),
                Summary = content.Summary.GetText(identity.Language),
                LastUpdated = "last updated " + TimeZones.Format(folder.LastChangeTime.Value, timezone, true)
            };

            Folder.Items = new List<LaunchCard>();
        }

        private void AddFolderForDocumentSearch(QPage folder, LaunchCard item)
        {
            if (string.IsNullOrEmpty(item.Url)
                || !item.Url.Contains("ui/portal/standards/documents/search")
                    && !item.Url.Contains("ui/portal/standards/documents/outline")
                    && !item.Url.Contains("ui/portal/standards/documents/analysis")
                    && !item.Url.Contains("ui/portal/standards/search")
                    && !item.Url.Contains("ui/portal/standards/outline")
                )
            {
                return;
            }

            var separator = item.Url.Contains("?") ? "&" : "?";

            item.Url += $"{separator}folder={folder.PageIdentifier}";
        }

        private void BindBodyPanel(ISecurityFramework identity)
        {
            ListBody.Visible = Model.Body.HasValue();
            ListBody.InnerHtml = Model.Body;

            Repeater.BindModelToControls(Folder.Items, identity);
        }
    }
}