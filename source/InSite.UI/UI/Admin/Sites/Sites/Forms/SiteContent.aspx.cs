using System;
using System.Web.UI;

using InSite.Admin.Sites.Pages.Controls;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Sites.Sites.Forms
{
    public partial class SiteContent : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties
        private Guid? SiteID => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        private string Tab => Request["tab"];

        private string[] Fields = { "Title", "Summary", "Shortcuts", "Copyright", "HtmlHead" };

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToParent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var site = SiteID.HasValue ? ServiceLocator.SiteSearch.Select(SiteID.Value) : null;
            if (site == null || site.OrganizationIdentifier != Organization.Identifier)
                RedirectToSearch();

            SetInputValues(site);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var @site = SiteID.HasValue ? ServiceLocator.SiteSearch.Select(SiteID.Value) : null;
            if (@site == null)
                return true;

            var content = new Shift.Common.ContentContainer();

            GetInputValues(content);

            ServiceLocator.SendCommand(new ChangeSiteContent(@site.SiteIdentifier, content));

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(QSite @site)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(@site.SiteIdentifier);

            PageHelper.AutoBindHeader(
                this, 
                qualifier: $"{@site.SiteTitle} <span class='form-text'>{@site.SiteDomain}</span>");

            if (ContentEditor.IsEmpty)
            {
                foreach (var name in Fields)
                {
                    if (name == "Title")
                    {
                        ContentEditor.Add(new AssetContentSection.SingleLine(name)
                        {
                            Title = name,
                            Value = content.Title.Text,
                            IsRequired = false
                        });
                    }
                    else if (name == "HtmlHead")
                    {
                        var item = content[name];

                        ContentEditor.Add(new ContentSectionHtmlCode.SectionSettings(name)
                        {
                            Title = "&lt;HEAD&gt;",
                            Value = item.Html,
                            IsRequired = false
                        });
                    }
                    else
                    {
                        var item = content[name];

                        ContentEditor.Add(new AssetContentSection.MarkdownAndHtml(name)
                        {
                            Title = name,
                            HtmlValue = item.Html,
                            MarkdownValue = item.Text,
                            IsRequired = false,
                            IsMultiValue = true
                        });
                    }
                }

                ContentEditor.SetLanguage("en");
                ContentEditor.OpenTab(Tab);
            }
        }

        #endregion

        #region Helper methods

        private void GetInputValues(Shift.Common.ContentContainer data)
        {
            foreach (var name in Fields)
            {
                var item = data[name];

                if (name == "Title")
                {
                    item.Text = ContentEditor.GetValue(name);
                }
                else if(name == "HtmlHead")
                {
                    item.Html = ContentEditor.GetValue(name);
                }
                else
                {
                    item.Html = ContentEditor.GetValue(name, ContentSectionDefault.BodyHtml);
                    item.Text = ContentEditor.GetValue(name, ContentSectionDefault.BodyText);
                }
            }
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/sites/sites/search", true);

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/sites/outline?id={SiteID}&panel=content&tab="
                + ContentEditor.GetCurrentTab().ToString().ToLower(), true);

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={SiteID}"
                : null;
        }

        #endregion
    }
}