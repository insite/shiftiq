using System;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Admin.Sites.Pages.Controls;
using InSite.Admin.Sites.Utilities;
using InSite.Application.Sites.Read;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Content;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Common.Contents;
using InSite.UI.Portal.Workflow.Forms.Models;
using InSite.Web.Routing;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

using ContentSection = Shift.Sdk.UI.AssetContentSection;

namespace InSite.Admin.Sites.Pages
{
    public partial class Outline : AdminBasePage
    {
        #region Properties

        public Guid? Id => Guid.TryParse(Request["id"], out var id) ? id : (Guid?)null;
        public string Panel => Request.QueryString["panel"];
        public string Tab => Request.QueryString["tab"];
        public string HelpUrl => Request.QueryString["helpUrl"];

        private Guid? SiteId
        {
            get => (Guid?)ViewState[nameof(SiteId)];
            set => ViewState[nameof(SiteId)] = value;
        }

        protected QPage Entity
        {
            get
            {
                if (_isEntityLoaded)
                    return _entity;

                var id = Id ?? CreateHelpPage();

                _entity = id.HasValue
                    ? ServiceLocator.PageSearch.Select(id.Value, x => x.Site, x => x.Parent)
                    : null;

                _isEntityLoaded = true;

                return _entity;
            }
        }

        private Guid? CreatedPageId
        {
            get => (Guid?)ViewState[nameof(CreatedPageId)];
            set => ViewState[nameof(CreatedPageId)] = value;
        }

        protected Guid? PageId => Entity?.PageIdentifier;

        private int ContentItemsCount
        {
            get => (int)(ViewState[nameof(ContentItemsCount)] ?? 0);
            set => ViewState[nameof(ContentItemsCount)] = value;
        }

        private string[] ContentFields
        {
            get => (string[])ViewState[nameof(ContentFields)];
            set => ViewState[nameof(ContentFields)] = value;
        }

        #endregion

        #region Fields

        private QPage _entity;
        private bool _isEntityLoaded;

        #endregion

        #region Initialization and PreRender

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreviewLink.Click += PreviewLink_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Open();

            if (!Page.IsPostBack)
            {
                CopyLink.NavigateUrl = $"/ui/admin/sites/pages/create?action=duplicate&site={SiteId}&page={PageId}";
                NewPageLink.NavigateUrl = new ReturnUrl($"id={PageId}&panel=setup")
                    .GetRedirectUrl($"/ui/admin/sites/pages/create?site={SiteId}&parent={PageId}");

                BindRepeater();
            }

            if (ContentEditor.GetSection("PageBlocks", true) is ContentBlocksManager blocksManager)
            {
                blocksManager.StatusUpdated += BlocksManager_Alert;
                blocksManager.BlockInserted += BlocksManager_BlockInserted;
                blocksManager.BlockDeleted += BlocksManager_BlockDeleted;
            }
        }

        #endregion

        #region Event handlers

        private void PreviewButton_Click(object sender, EventArgs e)
        {
            var page = ServiceLocator.PageSearch.Select(PageId.Value, x => x.Site);
            var portal = page.Site?.SiteIsPortal ?? true;

            var url = portal
                ? ServiceLocator.PageSearch.GetPagePath(PageId.Value, false)
                : SiteSettings.GetUrl(PageId.Value, portal, true);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Outline),
                "open_url", $"window.open({HttpUtility.JavaScriptStringEncode(url, true)}, '_blank');",
                true);
        }

        private void BlocksManager_Alert(object sender, AlertArgs args)
        {
            EditorStatus.AddMessage(args.Type, args.Text);
        }

        private void BlocksManager_BlockInserted(object sender, EventArgs e)
        {
            SetInputValues(Entity);
        }

        private void BlocksManager_BlockDeleted(object sender, EventArgs e)
        {
            SetInputValues(Entity);
        }

        #endregion

        #region Load & Save

        private void Open()
        {
            _isEntityLoaded = false;

            if (Entity == null || Entity.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect("/ui/admin/sites/pages/search");

            var page = ServiceLocator.PageSearch.Select(Entity.PageIdentifier, x => x.Site);
            var portal = page.Site?.SiteIsPortal ?? true;

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{Entity.Title ?? Entity.PageSlug ?? "Untitled"} <span class='form-text'>{SiteSettings.GetUrl(Entity.PageIdentifier, portal, false)}</span>");

            SetInputValues(Entity);
            NavigateToPanelsTabs();
        }

        private Guid? CreateHelpPage()
        {
            if (CreatedPageId.HasValue)
                return CreatedPageId;

            if (string.IsNullOrEmpty(HelpUrl))
                return null;

            var action = TActionSearch.Get(HelpUrl);
            if (action == null)
                return null;

            var pageId = AdminHome.GetHelpPageId(action.ActionUrl);

            CreatedPageId = pageId
                ?? SiteHelper.CreateCustomHelpPage(action.ActionUrl, Identity.User, Identity.Organization, true);

            return CreatedPageId;
        }

        #endregion

        #region Methods (data binding)

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }

        public void NavigateToPanelsTabs()
        {
            if (string.IsNullOrEmpty(Panel))
                return;

            if (Panel.ToLower().Equals("content"))
                ContentSection.IsSelected = true;
            else if (Panel.ToLower().Equals("subpage"))
                SubpageSection.IsSelected = true;
            else if (Panel.ToLower().Equals("sitemap"))
                HierarchySection.IsSelected = true;
            else if (Panel.ToLower().Equals("privacy"))
                PrivacySection.IsSelected = true;
            else if (Panel.ToLower().Equals("setup"))
                PageSection.IsSelected = true;
        }

        public void SetInputValues(QPage entity)
        {
            PageSection.Icon = "far fa-" + SiteHelper.GetIconName(entity.PageType);
            SiteId = entity.SiteIdentifier;
            WebSiteSelector.Text = string.IsNullOrEmpty(entity.Site?.SiteDomain) ? "None" : entity.Site?.SiteDomain;
            PageType.Text = entity.PageType;
            ParentPageId.Text = string.IsNullOrEmpty(entity.Parent?.Title) ? "None" : entity.Parent?.Title;
            TitleOutput.Text = entity.Title;
            PageIdentifier.Text = PageId.ToString();
            PageSlug.Text = string.IsNullOrEmpty(entity.PageSlug) ? "None" : entity.PageSlug;
            AuthorDate.Text = entity.AuthorDate == null ? "None" : entity.AuthorDate.Value.Format(User.TimeZone);
            AuthorName.Text = string.IsNullOrEmpty(entity.AuthorName) ? "None" : entity.AuthorName;
            NavigateUrl.Text = string.IsNullOrEmpty(entity.NavigateUrl) ? "None" : entity.NavigateUrl;
            IsPublished.Text = entity.IsHidden ? "Unpublished" : "Published";
            IsNavigateUrlToNewTab.Text = entity.IsNewTab ? "Opens in a new browser window" : "Doesn't open in a new browser window";
            IsNavigateUrlToNewTab.Visible = !string.IsNullOrEmpty(entity.NavigateUrl);
            ContentControl.Text = string.IsNullOrEmpty(entity.ContentControl) ? "None" : TranslateContentControl(entity.ContentControl);

            CourseIdentifier.Text = entity.ObjectType == "Course" && entity.ObjectIdentifier.HasValue ? entity.ObjectIdentifier.ToString() : "None";
            ProgramIdentifier.Text = entity.ObjectType == "Program" && entity.ObjectIdentifier.HasValue ? entity.ObjectIdentifier.ToString() : "None";
            SurveyIdentifier.Text = entity.ObjectType == "Survey" && entity.ObjectIdentifier.HasValue ? entity.ObjectIdentifier.ToString() : "None";

            ContentLabels.Text = string.IsNullOrEmpty(entity.ContentLabels) ? "None" : entity.ContentLabels;
            Icon.Text = string.IsNullOrEmpty(entity.PageIcon) ? "None" : entity.PageIcon;
            Hook.Text = string.IsNullOrEmpty(entity.Hook) ? "None" : entity.Hook;
            CoursePanel.Visible = ContentControl.Text == "Course";
            ProgramPanel.Visible = ContentControl.Text == "Program";
            SurveyPanel.Visible = ContentControl.Text == "Survey";

            SetNavigationLinks(entity.PageIdentifier);

            PublishLink.Visible = entity.IsHidden;
            UnpublishLink.Visible = !entity.IsHidden;

            SetupContentSection(entity, "", "");

            SubpageList.LoadData(entity, ConnectionDirection.Outgoing);

            if (entity.SiteIdentifier.HasValue)
                TreeView.LoadDataBySiteId(entity.SiteIdentifier.Value, entity.PageIdentifier);
            else
                TreeView.LoadDataByPageId(entity.PageIdentifier, entity.PageIdentifier);
        }

        private string TranslateContentControl(string contentControl)
        {
            foreach (var info in ControlPath.PageControlTypes)
            {
                if (info.Name.Equals(contentControl))
                    return info.Title;
            }
            return contentControl;
        }

        private void SetNavigationLinks(Guid? PageId)
        {
            DownloadLink.NavigateUrl = $"/ui/admin/sites/pages/download?id={PageId}";
            DeleteLink.NavigateUrl = $"/ui/admin/sites/pages/delete?id={PageId}";


            PageTypeLink.NavigateUrl = $"/ui/admin/sites/pages/change-page-setup?id={PageId}";
            TitleLink.NavigateUrl = $"/ui/admin/sites/pages/change-page-setup?id={PageId}";

            PageSlugLink.NavigateUrl = $"/ui/admin/sites/pages/change-structure?id={PageId}";
            NavigateUrlLink.NavigateUrl = $"/ui/admin/sites/pages/change-structure?id={PageId}";

            WebSiteSelectorLink.NavigateUrl = $"/ui/admin/sites/pages/change-structure?id={PageId}";
            ParentPageIdLink.NavigateUrl = $"/ui/admin/sites/pages/change-structure?id={PageId}";

            IconLink.NavigateUrl = $"/ui/admin/sites/pages/change-settings?id={PageId}";
            ContentLabelsLink.NavigateUrl = $"/ui/admin/sites/pages/change-settings?id={PageId}";
            ContentControlnLink.NavigateUrl = $"/ui/admin/sites/pages/change-settings?id={PageId}";
            HookLink.NavigateUrl = $"/ui/admin/sites/pages/change-settings?id={PageId}";

            FilterGroupListLink.NavigateUrl = $"/ui/admin/sites/pages/change-privacy?id={PageId}";

            PublishLink.NavigateUrl = $"/ui/admin/sites/pages/publish?id={PageId}";
            UnpublishLink.NavigateUrl = $"/ui/admin/sites/pages/publish?id={PageId}";
        }

        private void PreviewLink_Click(object sender, EventArgs e)
        {
            if (!SiteId.HasValue || Entity == null)
                return;

            var site = ServiceLocator.SiteSearch.Select(SiteId.Value);
            var page = ServiceLocator.PageSearch.Select(PageId.Value);

            var appUrl = site.SiteIsPortal
                ? ServiceLocator.Urls.GetApplicationUrl(Organization.Code)
                : "https://www." + site.SiteDomain
                ;

            var caller = FormCaller.CreatePortal(string.Empty).Serialize();
            var helper = new LaunchCardAdapter();
            var url = helper.CreateUrl(appUrl, page.PageIdentifier, page.NavigateUrl, page.ObjectType, page.ObjectIdentifier, page.PageSlug, Identity.User.Identifier, caller, LabelHelper.GetTranslation);

            if (!site.SiteIsPortal)
                url = "https://www." + site.SiteDomain + url;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Outline),
                "open_url", $"window.open({HttpUtility.JavaScriptStringEncode(url, true)}, '_blank');",
                true);
        }

        private void BindRepeater()
        {
            var groupPermissions = TGroupPermissionSearch.Select(x => x.ObjectIdentifier == PageId, null, x => x.Group);

            GroupDataRepeater.Visible = groupPermissions.Length > 0;
            GroupDataRepeaterFooter.Visible = groupPermissions.Length == 0;
            GroupDataRepeater.DataSource = groupPermissions;
            GroupDataRepeater.DataBind();
        }

        private ContentContainer SetupContentSection(QPage page, string panel, string tab)
        {
            PreviewLink.Visible = page.SiteIdentifier.HasValue;

            ContentFields = (page.ContentLabels ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.IsNotEmpty() && (page.PageType != "Folder" || !x.Equals("PageBlocks", StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            ContentItemsCount = 0;
            ContentSection.Visible = ContentFields.Length > 0;
            var selectedTab = ContentEditor.GetCurrentTab();
            var selectedSection = (ContentEditor.GetSection("PageBlocks", true) as ContentBlocksManager)?.GetSectionItem();

            ContentEditor.Clear();
            ContentSection.Visible = ContentFields.Length > 0;
            var content = ServiceLocator.ContentSearch.GetBlock(page.PageIdentifier, labels: ContentFields);

            foreach (var name in ContentFields)
            {
                var item = content[name];

                if (name == "HtmlHead")
                {
                    ContentEditor.Add(new ContentSectionViewer.SectionSettings(name)
                    {
                        Title = "&lt;HEAD&gt;",
                        Value = item.Html,
                        IsRequired = false,
                        EditUrl = $"/ui/admin/sites/pages/content?id={page.PageIdentifier}&tab={HttpUtility.UrlEncode(name.ToLower())}"
                    });
                }
                else
                if (name == "PageBlocks")
                {
                    ContentEditor.Add(new ContentBlocksManager.SectionSettings(name, true)
                    {
                        ParentPage = page,
                        Title = "Blocks",
                        EditUrl = $"/ui/admin/sites/pages/content?id={page.PageIdentifier}&tab={HttpUtility.UrlEncode(name.ToLower())}",
                    });
                }
                else if (name == "Body")
                {

                    ContentEditor.Add(new ContentSectionViewer.SectionSettings(name)
                    {
                        Title = name,
                        Value = string.IsNullOrEmpty(item.Text.Default) ? item.Html : item.Text,
                        IsRequired = false,
                        EditUrl = $"/ui/admin/sites/pages/content?id={page.PageIdentifier}&tab={HttpUtility.UrlEncode(name.ToLower())}"
                    });
                }
                else
                {
                    ContentEditor.Add(new ContentSectionViewer.SectionSettings(name)
                    {
                        Title = name,
                        Value = item.Text,
                        IsRequired = false,
                        EditUrl = $"/ui/admin/sites/pages/content?id={page.PageIdentifier}&tab={HttpUtility.UrlEncode(name.ToLower())}"
                    });
                }
                if (name == "HtmlHead")
                {
                    var contentToReplace = LoadHtml(content[name]);
                    if (!string.IsNullOrEmpty(contentToReplace))
                        content[name].Html.Default = contentToReplace;
                }

                ContentItemsCount++;
            }

            ContentEditor.SetLanguage(CurrentSessionState.Identity.Language);
            ContentEditor.OpenTab(Request["tab"]);

            if (selectedTab != null)
                ContentEditor.OpenTab(selectedTab);

            if (selectedSection.HasValue)
                (ContentEditor.GetSection("PageBlocks", true) as ContentBlocksManager)?.OpenSectionItem(selectedSection.Value);

            return content;
        }

        #endregion

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
    }
}