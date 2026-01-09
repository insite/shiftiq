using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Sites.Utilities;
using InSite.Application.Pages.Write;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using static InSite.Admin.Sites.Utilities.SiteHelper;

namespace InSite.Admin.Sites.Sites.Forms
{
    public partial class Create : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private string OutlinePath => "/ui/admin/sites/outline";
        private string SearchRelativePath => "/ui/admin/sites/sites/search";

        private static readonly Regex RegexOutlineLinePattern = new Regex("^(?<Indent>#+)(?<Title>.+)$", RegexOptions.Compiled);

        #endregion

        #region Classes

        private class OutlineExampleInfo
        {
            public string Name { get; }
            public string Text { get; }

            public OutlineExampleInfo(string name, string text)
            {
                Name = name;
                Text = text;
            }
        }

        [Serializable]
        private class LineDataItem
        {
            #region Properties

            public int Indent { get; }

            public int Sequence { get; set; }

            public string Title { get; set; }

            public string Type { get; set; }
            public string ContentControl { get; set; }
            public string Slug { get; set; }
            public string NavigateUrl { get; set; }
            public ContentContainer Content { get; set; }

            public LineDataItem Parent { get; private set; }

            public IReadOnlyList<LineDataItem> Children => _children;

            #endregion

            #region Fields

            private List<LineDataItem> _children = new List<LineDataItem>();

            #endregion

            #region Construction

            public LineDataItem(int indent)
            {
                Indent = indent;
            }

            #endregion

            #region Methods

            public void Add(LineDataItem item)
            {
                if (item.Parent != null)
                    throw new ApplicationError("Parent != null");

                item.Parent = this;

                _children.Add(item);
            }

            public LineDataItem FindParent(int indent) =>
                Indent == indent ? this : Parent?.FindParent(indent);

            public LineDataItem GetRoot() =>
                Parent == null ? this : Parent.GetRoot();

            public LineDataItem[] ToArray()
            {
                var list = new List<LineDataItem>();

                AddItems(this);

                return list.ToArray();

                void AddItems(LineDataItem item)
                {
                    list.Add(item);

                    foreach (var child in item._children)
                        AddItems(child);
                }
            }

            #endregion
        }

        private class TreeViewDataItem
        {
            public string Type { get; set; }
            public string Icon => SiteHelper.GetIconCssClass(Type);
            public string Title { get; set; }

            public string HtmlPrefix { get; set; }
            public string HtmlPostfix { get; set; }
        }

        [Serializable]
        private class MultipleFormDataItem
        {
            public string Name { get; set; }
            public string Title { get; set; }
        }

        #endregion

        #region Properties

        private QSiteExport JsonData
        {
            get => (QSiteExport)ViewState[nameof(JsonData)];
            set => ViewState[nameof(JsonData)] = value;
        }

        private MultipleFormDataItem MultipleFormData
        {
            get => (MultipleFormDataItem)ViewState[nameof(MultipleFormData)];
            set => ViewState[nameof(MultipleFormData)] = value;
        }

        private LineDataItem[] RootItems
        {
            get => (LineDataItem[])ViewState[nameof(RootItems)];
            set => ViewState[nameof(RootItems)] = value;
        }

        private Guid? DuplicateId
        {
            get => (Guid?)ViewState[nameof(DuplicateId)];
            set => ViewState[nameof(DuplicateId)] = value;
        }

        private Guid SiteId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        private QSite DuplicateEntity
        {
            get
            {
                if (_isEntityLoaded)
                    return _duplicateEntity;

                if (DuplicateId.HasValue)
                    _duplicateEntity = ServiceLocator.SiteSearch.Select(DuplicateId.Value);
                if (WebSiteIdentifier.HasValue)
                    _duplicateEntity = ServiceLocator.SiteSearch.Select((Guid)WebSiteIdentifier);

                _isEntityLoaded = true;

                return _duplicateEntity;
            }
            set
            {
                this._duplicateEntity = value;
                _isEntityLoaded = true;
            }
        }

        #endregion

        #region Fields

        private Guid? WebSiteIdentifier => Guid.TryParse(Request["id"], out Guid result) ? result : (Guid?)null;
        private string Action => Request.QueryString["action"];
        private QSite _duplicateEntity;
        private bool _isEntityLoaded;


        #endregion

        #region Initalization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            NextButton.Click += NextButton_Click;

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += (s, a) => OnCreationTypeSelected();

            SiteComboBox.AutoPostBack = true;
            SiteComboBox.ValueChanged += SiteComboBox_ValueChanged;

            UploadFileUploaded.Click += UploadFileUploaded_Click;

            TreeViewRepeater.DataBinding += TreeViewRepeater_DataBinding;
            TreeViewRepeater.ItemDataBound += TreeViewRepeater_ItemDataBound;

            UploadFileType.AutoPostBack = true;
            UploadFileType.ValueChanged += UploadFileType_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (IsPostBack)
                return;

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);

            PageHelper.AutoBindHeader(this);

            if (Action == "duplicate" && WebSiteIdentifier != Guid.Empty)
            {
                if (DuplicateEntity == null)
                    RedirectToSearch();

                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;
                SiteComboBox.ValueAsGuid = WebSiteIdentifier;
                SetCopiedInputValues(DuplicateEntity);
            }
            else
            {
                SetDefaultInputValues();
            }
            OnCreationTypeSelected();

            CancelButton.NavigateUrl = GetBackUrl();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                SaveOne();
            if (value == CreationTypeEnum.Duplicate)
                SaveDuplicate();
            if (value == CreationTypeEnum.Upload)
                SaveOutline();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.Upload)
                UploadNextButton();
        }

        private void SiteComboBox_ValueChanged(object sender, EventArgs e)
        {
            DuplicateId = SiteComboBox.ValueAsGuid;
            if (DuplicateId.HasValue)
                DuplicateEntity = ServiceLocator.SiteSearch.Select(DuplicateId.Value);
            SetCopiedInputValues(DuplicateEntity);
        }

        private void UploadFileUploaded_Click(object sender, EventArgs e)
        {
            if (!CreateUploadFile.HasFile)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }

            if (UploadFileType.Value == "JSON" && !CreateUploadFile.FilePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                ScreenStatus.AddMessage(AlertType.Error, "Invalid file type. File type supported .json");
                return;
            }

            using (var stream = CreateUploadFile.OpenFile())
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    UploadJsonInput.Text = reader.ReadToEnd();
            }
        }

        private void OnCreationTypeSelected()
        {
            var value = CreationType.ValueAsEnum;

            ClearPreviewSection();

            if (value == CreationTypeEnum.One)
            {
                MultiView.SetActiveView(OneView);
                ShowSave();
            }
            if (value == CreationTypeEnum.Duplicate)
            {
                MultiView.SetActiveView(CopyView);
                ShowSave();
            }
            if (value == CreationTypeEnum.Upload)
            {
                MultiView.SetActiveView(UploadView);
                ShowNext();
            }
        }

        private void UploadFileType_ValueChanged(object sender, EventArgs e) => OnUploadFileTypeChanged();

        private void OnUploadFileTypeChanged()
        {
            var value = UploadFileType.Value;
        }

        private void TreeViewRepeater_DataBinding(object sender, EventArgs e)
        {
            var dataSource = new List<TreeViewDataItem>();

            if (RootItems != null)
            {
                var lineItems = RootItems.SelectMany(x => x.ToArray());

                LineDataItem prevItem = null;

                foreach (var lineItem in lineItems)
                {
                    var dataItem = new TreeViewDataItem
                    {
                        Type = lineItem.Type,
                        Title = lineItem.Title,
                    };

                    if (prevItem == null)
                        dataItem.HtmlPrefix = $"<ul id='{TreeViewRepeater.ClientID}' class='tree-view' data-default-level='all'><li>";
                    else if (prevItem == lineItem.Parent)
                        dataItem.HtmlPrefix = "<ul class='tree-view'><li>";
                    else if (prevItem.Parent == lineItem.Parent)
                        dataItem.HtmlPrefix = "<li>";
                    else
                        dataItem.HtmlPrefix = BuildTreeEnd(prevItem.Indent - lineItem.Indent) + "<li>";

                    if (prevItem != null && lineItem.Children.Count == 0)
                        dataItem.HtmlPostfix = $"</li>";

                    dataSource.Add(dataItem);

                    prevItem = lineItem;
                }

                var lastDataItem = dataSource[dataSource.Count - 1];
                if (prevItem != null && prevItem.Indent > 1)
                    lastDataItem.HtmlPostfix += BuildTreeEnd(prevItem.Indent - 1);
                lastDataItem.HtmlPostfix += "</ul>";

                string BuildTreeEnd(int levels) =>
                    string.Concat(Enumerable.Repeat("</ul></li>", levels));
            }

            TreeViewRepeater.DataSource = dataSource;
        }

        private void TreeViewRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var dataItem = (TreeViewDataItem)e.Item.DataItem;

            var typeSelector = (WebPageTypeComboBox)e.Item.FindControl("TypeSelector");
            typeSelector.EnsureDataBound();
            typeSelector.Value = dataItem.Type;
        }

        private void GetTreeViewInputs()
        {
            var index = 0;
            var lineItems = RootItems.SelectMany(x => x.ToArray()).ToArray();

            foreach (RepeaterItem item in TreeViewRepeater.Items)
            {
                if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
                    return;

                var lineItem = lineItems[index++];
                var typeSelector = (WebPageTypeComboBox)item.FindControl("TypeSelector");

                lineItem.Type = typeSelector.Value;
            }
        }

        private void LoadItemsFromJson(int rootIndex, LineDataItem parent, QPageExport page)
        {
            var indent = parent != null ? parent.Indent + 1 : 1;

            var item = new LineDataItem(indent)
            {
                Sequence = parent != null ? parent.Children.Count + 1 : 1,
                Title = page.Title,
                Type = page.PageType,
                ContentControl = page.ContentControl,
                Slug = page.PageSlug,
                NavigateUrl = page.NavigateUrl,
                Content = page.Content
            };

            if (parent != null)
                parent.Add(item);
            else
                RootItems[rootIndex] = item;

            if (page.Children.IsNotEmpty())
            {
                foreach (var child in page.Children)
                    LoadItemsFromJson(-1, item, child);
            }
        }

        private void LoadItemsFromObject(int rootIndex, LineDataItem parent, QPage page)
        {
            var indent = parent != null ? parent.Indent + 1 : 1;

            var item = new LineDataItem(indent)
            {
                Sequence = parent != null ? parent.Children.Count + 1 : 1,
                Title = page.Title,
                Type = page.PageType,
                ContentControl = page.ContentControl,
                Slug = page.PageSlug,
                NavigateUrl = page.NavigateUrl,
            };

            if (parent != null)
                parent.Add(item);
            else
                RootItems[rootIndex] = item;

            if (page.Children.IsNotEmpty())
            {
                foreach (var child in page.Children)
                    LoadItemsFromObject(-1, item, child);
            }
        }

        private void UploadNextButton()
        {
            if (UploadFileType.Value == "JSON")
                OutlineJsonNextButton();
        }

        private void OutlineJsonNextButton()
        {
            var json = UploadJsonInput.Text;

            try
            {
                JsonData = null;
                if (json.IsNotEmpty())
                    JsonData = JsonConvert.DeserializeObject<QSiteExport>(json);

                if (JsonData == null)
                {
                    ScreenStatus.AddMessage(AlertType.Error, "Uploaded File Content is empty");
                    return;
                }

                if (JsonData.Title.HasNoValue())
                {
                    ScreenStatus.AddMessage(AlertType.Error, "Uploaded File Title is empty");
                    return;
                }

                if (JsonData.Name.HasNoValue())
                {
                    ScreenStatus.AddMessage(AlertType.Error, "Uploaded File Domain is empty");
                    return;
                }

                PreviewSection.Visible = true;
                PreviewSection.Focus();
                ShowSave();
                RootItems = null;

                if (JsonData.Pages.IsNotEmpty())
                {
                    RootItems = new LineDataItem[JsonData.Pages.Count];

                    for (int i = 0; i < JsonData.Pages.Count; i++)
                        LoadItemsFromJson(i, null, JsonData.Pages[i]);
                }

                MultipleFormData = new MultipleFormDataItem
                {
                    Name = JsonData.Name,
                    Title = JsonData.Title
                };

                TreeViewRepeater.DataBind();
            }
            catch (JsonReaderException ex)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"Your uploaded file has an unexpected format. {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Error during import: " + ex.Message);
                return;
            }
        }

        #endregion

        #region Setting and getting input values

        private void ClearPreviewSection()
        {
            RootItems = null;
            PreviewSection.Visible = false;
            MultipleFormData = null;
        }

        private void SetDefaultInputValues()
        {
            OnUploadFileTypeChanged();
        }

        private void SetCopiedInputValues(QSite entity)
        {
            CopyTitle.Text = entity.SiteTitle;
            CopyName.Text = entity.SiteDomain;
        }

        #endregion

        #region Database operations

        private void SaveOne()
        {
            var entity = SiteHelper.CreateSite(SingleName.Text, SingleTitle.Text);

            var commands = new SiteCommandGenerator().GetCommands(entity);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            RedirectToOutline(entity);
        }

        private void SaveDuplicate()
        {
            var entity = SiteHelper.CopySite(DuplicateEntity, CopyName.Text, CopyTitle.Text);

            var commands = new SiteCommandGenerator().GetCommands(entity);

            var siteTree = SiteHelper.GetQSiteTree(DuplicateEntity);

            var organization = CurrentSessionState.Identity.Organization;
            var user = CurrentSessionState.Identity.User;

            foreach (var item in siteTree.Pages)
                GetDuplicatePage(entity, item, commands, entity.SiteIdentifier, organization, user);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            RedirectToOutline(entity);
        }

        private void SaveOutline()
        {
            GetTreeViewInputs();

            var entity = SiteHelper.CreateSite(MultipleFormData.Name, MultipleFormData.Title);

            var commands = new SiteCommandGenerator().GetCommands(entity);

            foreach (var item in RootItems)
                GetPage(entity, item, commands, entity.SiteIdentifier);

            var isJsonUpload = CreationType.ValueAsEnum == CreationTypeEnum.Upload && UploadFileType.Value == "JSON";

            if (isJsonUpload)
            {
                var organization = CurrentSessionState.Identity.Organization;
                var user = CurrentSessionState.Identity.User;
                ServiceLocator.PageSearch.LoadSite(organization.ParentOrganizationIdentifier, organization.OrganizationIdentifier, user.UserIdentifier, JsonData, entity);
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            if (isJsonUpload)
            {
                if (JsonData.Content != null && !JsonData.Content.IsEmpty)
                    ServiceLocator.SendCommand(new ChangeSiteContent(entity.SiteIdentifier, JsonData.Content));
            }

            RedirectToOutline(entity);
        }

        #endregion

        #region Methods (helpers)

        private void ShowSave()
        {
            SaveButton.Visible = true;
            NextButton.Visible = false;
        }

        private void ShowNext()
        {
            SaveButton.Visible = false;
            NextButton.Visible = true;
        }

        private static QPage GetDuplicatePage(
            QSite site,
            QPageNode item,
            List<ICommand> commands,
            Guid siteId,
            OrganizationState organization,
            Domain.Foundations.User user)
        {
            var page = SiteHelper.CreatePage(item.PageType, item.Title);

            page.Sequence = item.Sequence;
            page.SiteIdentifier = siteId;
            page.NavigateUrl = item.NavigateUrl;
            page.ContentControl = item.ContentControl;
            page.ContentLabels = item.ContentLabels;
            page.PageIcon = item.PageIcon;
            page.IsNewTab = item.IsNewTab;
            page.IsHidden = item.IsHidden;
            page.IsAccessDenied = item.IsAccessDenied;
            page.Hook = item.Hook;
            page.OrganizationIdentifier = organization.Identifier;
            page.AuthorName = user.FullName;
            page.AuthorDate = DateTimeOffset.Now;

            commands.AddRange(new PageCommandGenerator().GetCommands(page));

            if (item.Content != null && !item.Content.IsEmpty)
                commands.Add(new InSite.Application.Pages.Write.ChangePageContent(page.PageIdentifier, item.Content));

            if (site != null)
                site.Pages.Add(page);

            if (item.Children != null)
                foreach (var childItem in item.Children)
                {
                    var childResource = GetDuplicatePage(null, childItem, commands, siteId, organization, user);
                    childResource.ParentPageIdentifier = page.PageIdentifier;
                    commands.Add(new ChangePageParent(childResource.PageIdentifier, page.PageIdentifier));
                    page.Children.Add(childResource);
                }

            return page;
        }

        private static QPage GetPage(QSite site, LineDataItem item, List<ICommand> commands, Guid siteId)
        {
            var page = SiteHelper.CreatePage(item.Type, item.Title);

            page.Sequence = item.Sequence;
            page.SiteIdentifier = siteId;
            page.PageSlug = item.Slug;
            page.NavigateUrl = item.NavigateUrl;
            page.ContentControl = item.ContentControl;

            commands.AddRange(new PageCommandGenerator().GetCommands(page));

            if (item.Content != null && !item.Content.IsEmpty)
                commands.Add(new InSite.Application.Pages.Write.ChangePageContent(page.PageIdentifier, item.Content));

            if (site != null)
                site.Pages.Add(page);

            foreach (var childItem in item.Children)
            {
                var childResource = GetPage(null, childItem, commands, siteId);
                childResource.ParentPageIdentifier = page.PageIdentifier;
                commands.Add(new ChangePageParent(childResource.PageIdentifier, page.PageIdentifier));
                page.Children.Add(childResource);
            }

            return page;
        }

        #endregion

        #region IHasParentLinkParameters

        private void RedirectToOutline(QSite entity)
            => HttpResponseHelper.Redirect(String.Format($"{OutlinePath}?id={entity.SiteIdentifier}"));

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"{SearchRelativePath}", true);

        private string GetBackUrl() =>
            new ReturnUrl().GetReturnUrl() ?? GetReaderUrl();

        private string GetReaderUrl()
        {
            return HttpResponseHelper.BuildUrl($"{SearchRelativePath}", "type=Site");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={SiteId}"
                : null;
        }

        #endregion
    }
}