using System;
using System.Web;
using System.Web.UI;

using InSite.Admin.Sites.Pages.Controls;
using InSite.Application.Sites.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;
using InSite.Web.Routing;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Sites.Sites.Forms
{
    public partial class Outline : AdminBasePage
    {
        #region Properties

        public Guid? SiteID => Guid.TryParse(Request["id"], out var siteID) ? siteID : (Guid?)null;

        private string[] Fields = { "Title", "Summary", "Shortcuts", "Copyright", "HtmlHead" };

        public string Panel => Request.QueryString["panel"];
        public string Tab => Request.QueryString["tab"];

        private Guid? SiteId
        {
            get => (Guid?)ViewState[nameof(SiteId)];
            set => ViewState[nameof(SiteId)] = value;
        }

        protected QSite Entity
        {
            get
            {
                if (_isEntityLoaded)
                    return _entity;

                _entity = ServiceLocator.SiteSearch.Select(SiteID.Value);
                _isEntityLoaded = true;

                return _entity;
            }
        }

        #endregion

        #region Fields

        private QSite _entity;
        private bool _isEntityLoaded;

        #endregion

        #region Initialization and PreRender

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreviewLink.Click += ViewSiteButton_Click;

            PageHelper.AutoBindHeader(this);
        }

        #endregion

        #region Event handlers

        private void ViewSiteButton_Click(object sender, EventArgs e)
        {
            var pageUrl = SiteSettings.GetUrl(new[] { Entity.SiteDomain });

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Outline),
                "open_url", $"window.open({HttpUtility.JavaScriptStringEncode(pageUrl, true)}, '_blank');",
                true);
        }

        #endregion

        #region Load & Save

        protected override void OnLoad(EventArgs e)
        {
            _isEntityLoaded = false;

            if (Entity == null || Entity.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect("/ui/admin/sites/sites/search");

            PageHelper.BindTitle(Page, $"{Entity.SiteTitle ?? Entity.SiteDomain ?? "Untitled"}");

            LoadData(Entity);
        }

        #endregion

        #region Methods (data binding)

        private void LoadData(QSite entity)
        {
            SiteId = entity.SiteIdentifier;

            SiteTitle.Text = entity.SiteTitle;
            SiteDomain.Text = entity.SiteDomain;
            SiteIdentifier.Text = SiteId.ToString();

            var content = ServiceLocator.ContentSearch.GetBlock(entity.SiteIdentifier);

            HtmlHeadTab.Title = "&lt;HEAD&gt;";
            ContentTitle.LoadData(content.Title);
            ContentSummary.LoadData(content.Summary);
            ContentShortcuts.LoadData(content[ContentLabel.Shortcuts]);
            ContentCopyright.LoadData(content[ContentLabel.Copyright]);
            ContentHead.Text = LoadHtml(content[ContentLabel.HtmlHead]);

            if (ContentEditor.IsEmpty)
            {
                AddMarkdownAndHtml("Title", "Title");
                AddMarkdownAndHtml("Summary", "Summary");
                AddMarkdownAndHtml("Shortcuts", "Shortcuts");
                AddMarkdownAndHtml("Copyright", "Copyright");
                AddHtmlCode("HtmlHead", "&lt;HEAD&gt;");

                void AddMarkdownAndHtml(string name, string title)
                {
                    var item = content[name];

                    ContentEditor.Add(new AssetContentSection.MarkdownAndHtml(name)
                    {
                        Title = title,
                        HtmlValue = item.Html,
                        MarkdownValue = item.Text,
                        IsMultiValue = true
                    });
                }

                void AddHtmlCode(string name, string title)
                {
                    var item = content[name];
                    item.Html.Default = LoadHtml(item);

                    ContentEditor.Add(new ContentSectionHtmlCode.SectionSettings(name)
                    {
                        Title = title,
                        Value = item.Html,
                        IsRequired = false
                    });
                }
                ContentEditor.SetLanguage("en");
            }
            PageList.LoadData(entity, ConnectionDirection.Outgoing);

            TreeView.LoadDataBySiteId(entity.SiteIdentifier, entity.SiteIdentifier);

            SetNavigationLinks(SiteId);
            NavigateToPanelsTabs();
        }

        public void NavigateToPanelsTabs()
        {
            if (String.IsNullOrEmpty(Panel))
                return;

            if (Panel.ToLower().Equals("content"))
            {
                ContentSection.IsSelected = true;
                if (!string.IsNullOrEmpty(Tab))
                {

                    foreach (var item in ContentNavigation.GetItems())
                    {
                        item.IsSelected = false;
                        if (item.Title.ToLower() == Tab.ToLower())
                            item.IsSelected = true;
                        else if (Tab.ToLower() == "htmlhead")
                        {
                            if (item.Title.ToLower() == "&lt;HEAD&gt;".ToLower())
                                item.IsSelected = true;
                        }
                    }
                }
            }
            else if (Panel.ToLower().Equals("setup"))
                GeneralSection.IsSelected = true;
            else if (Panel.ToLower().Equals("sitemap"))
                HierarchySection.IsSelected = true;
            else if (Panel.ToLower().Equals("page"))
                PageSection.IsSelected = true;
        }

        private void SetNavigationLinks(Guid? WebSiteId)
        {
            DownloadLink.NavigateUrl = $"/ui/admin/sites/download?id={WebSiteId}";
            DeleteLink.NavigateUrl = $"/ui/admin/sites/delete?id={WebSiteId}";
            DuplicateButton.NavigateUrl = new ReturnUrl($"id={WebSiteId}&panel=setup")
                    .GetRedirectUrl($"/ui/admin/sites/create?action=duplicate&id={WebSiteId}");

            SiteTitleLink.NavigateUrl = $"/ui/admin/sites/change?id={WebSiteId}";
            SiteDomainLink.NavigateUrl = $"/ui/admin/sites/change?id={WebSiteId}";

            EditContentTitleLink.NavigateUrl = $"/ui/admin/sites/content?id={WebSiteId}&tab=Title";
            EditContentSummaryLink.NavigateUrl = $"/ui/admin/sites/content?id={WebSiteId}&tab=Summary";
            EditContentCopyrightlsLink.NavigateUrl = $"/ui/admin/sites/content?id={WebSiteId}&tab=Copyright";
            EditContentShortcutsLink.NavigateUrl = $"/ui/admin/sites/content?id={WebSiteId}&tab=Shortcuts";
            EditContentHtmlHeadLink.NavigateUrl = $"/ui/admin/sites/content?id={WebSiteId}&tab=HtmlHead";

            NewSiteLink.NavigateUrl = new ReturnUrl($"id={WebSiteId}&panel=setup")
                    .GetRedirectUrl($"/ui/admin/sites/create?id={WebSiteId}");
        }

        #endregion

        #region Methods (menu)

        private string LoadHtml(ContentContainerItem item)
        {
            if (item == null)
                return "";

            if (item.Html == null)
                return "";

            if (item.Html.Default == null)
                return "";

            return item.Html.Default.Replace("\r\n", "<br>");
        }

        #endregion
    }
}