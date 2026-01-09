using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Sites.Pages.Controls;
using InSite.Admin.Sites.Utilities;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Common.Contents;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Sites.Pages
{
    public partial class Create : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private static readonly Regex RegexOutlineLinePattern = new Regex("^(?<Indent>#+)(?<Title>.+)$", RegexOptions.Compiled);

        private static readonly OutlineExampleInfo[] DefaultOutlines = new[]
        {
            new OutlineExampleInfo("Example", @"# Folder A

## Page A.1

## Page A.2

## Page A.3
")
                };

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

            public string Layout { get; set; }

            public LineDataItem Parent { get; private set; }

            public IReadOnlyList<LineDataItem> Children => _children;

            #endregion

            #region Fields

            private readonly List<LineDataItem> _children = new List<LineDataItem>();

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
                {
                    throw new ApplicationError("Parent != null");
                }

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
                    {
                        AddItems(child);
                    }
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
            public Guid? WebSiteId { get; set; }
            public Guid? ParentWebPageId { get; set; }
        }

        #endregion

        #region Properties

        private string Action => Request.QueryString["action"];

        private Guid? SiteIdentifier => Guid.TryParse(Request.QueryString["site"], out var value) ? value : (Guid?)null;

        private Guid? ParentIdentifier => Guid.TryParse(Request.QueryString["parent"], out var value) ? value : (Guid?)null;

        protected Guid? PageIdentifier => Guid.TryParse(Request.QueryString["page"], out var result) ? result : (Guid?)null;

        private QPageExport JsonData
        {
            get => (QPageExport)ViewState[nameof(JsonData)];
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

        #endregion

        #region Initalization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            NextButton.Click += NextButton_Click;

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += (s, a) => OnCreationTypeSelected();

            UploadFileType.AutoPostBack = true;
            UploadFileType.ValueChanged += (x, y) => UploadFileTypeSelected();

            UploadFileUploaded.Click += UploadFileUploaded_Click;

            PageDuplicateValidator.ServerValidate += PageDuplicateValidator_ServerValidate;

            SingleWebSiteSelector.AutoPostBack = true;
            SingleWebSiteSelector.ValueChanged += SingleWebSiteSelector_ValueChanged;

            SinglePageType.AutoPostBack = true;
            SinglePageType.ValueChanged += SinglePageType_ValueChanged;

            CopyWebSiteSelector.AutoPostBack = true;
            CopyWebSiteSelector.ValueChanged += CopyWebSiteSelector_ValueChanged;

            MultipleWebSiteSelector.AutoPostBack = true;
            MultipleWebSiteSelector.ValueChanged += MultipleWebSiteSelector_ValueChanged;

            TreeViewRepeater.DataBinding += TreeViewRepeater_DataBinding;
            TreeViewRepeater.ItemDataBound += TreeViewRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                RedirectToSearch();

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Outline, CreationTypeEnum.Upload);

            if (Action == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;

                if (PageIdentifier.HasValue)
                {
                    CopyWebSiteSelector.ValueAsGuid = SiteIdentifier;
                    OnCopyWebSiteSelected();

                    CopyWebPageId.Value = PageIdentifier;
                }
            }

            OnCreationTypeSelected();
            SetDefaultInputValues();
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
            if (value == CreationTypeEnum.Upload || value == CreationTypeEnum.Outline)
                SaveOutline();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.Upload)
                UploadNextButton();
            if (value == CreationTypeEnum.Outline)
                OutlineMarkdownNextButton();
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
            if (value == CreationTypeEnum.Outline)
            {
                MultiView.SetActiveView(MultipleView);
                ShowNext();
            }
        }

        private void UploadFileTypeSelected()
        {
            CreateUploadFile.LabelText = UploadFileType.Value == "Markdown"
                ? "Select and Upload JSON File"
                : "Select and Upload MD File";
        }

        private void UploadFileUploaded_Click(object sender, EventArgs e)
        {
            if (!CreateUploadFile.HasFile)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }

            if (UploadFileType.Value == "MD" && !CreateUploadFile.FilePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                ScreenStatus.AddMessage(AlertType.Error, "Invalid file type. File type supported .txt");
                return;
            }
            else if (UploadFileType.Value == "JSON" && !CreateUploadFile.FilePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
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

        private void SinglePageType_ValueChanged(object sender, EventArgs e) => OnSinglePageTypeChanged();

        private void OnSinglePageTypeChanged()
        {
            var isBlock = SinglePageType.Value == "Block";

            SingleBlockControlField.Visible = isBlock;
        }

        private void PageDuplicateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var temp1 = args.Value;

            args.IsValid = CopyWebPageId.Value.HasValue && CopyView.Visible;

            if (!args.IsValid)
                PageDuplicateValidator.ErrorMessage = $@"Choose a Web Page to duplicate.";
        }

        private void SingleWebSiteSelector_ValueChanged(object sender, EventArgs e) => OnSingleWebSiteSelected();

        private void OnSingleWebSiteSelected(Guid? _site = null)
        {
            var siteId = SingleWebSiteSelector.ValueAsGuid;

            if (siteId != null)
                SingleParentPageId.SiteId = siteId;
            else if (siteId == null && _site.HasValue)
                SingleParentPageId.SiteId = _site.Value;

            if (!SingleParentPageId.Value.HasValue)
            {
                return;
            }

            var parentId = SingleParentPageId.Value.Value;
            if (!ServiceLocator.PageSearch.Exists(x => x.SiteIdentifier == siteId && x.PageIdentifier == parentId))
            {
                SingleParentPageId.Value = null;
            }
        }

        private void CopyWebSiteSelector_ValueChanged(object sender, EventArgs e) => OnCopyWebSiteSelected();

        private void OnCopyWebSiteSelected()
        {
            var siteId = CopyWebSiteSelector.ValueAsGuid;

            CopyWebPageId.SiteId = siteId;

            if (!CopyWebPageId.Value.HasValue)
            {
                return;
            }

            var pageId = CopyWebPageId.Value.Value;
            if (!ServiceLocator.PageSearch.Exists(x => x.SiteIdentifier == siteId && x.PageIdentifier == pageId))
            {
                CopyWebPageId.Value = null;
            }
        }

        private void MultipleWebSiteSelector_ValueChanged(object sender, EventArgs e) => OnMultipleWebSiteSelected();

        private void OnMultipleWebSiteSelected()
        {
            var siteId = MultipleWebSiteSelector.ValueAsGuid;

            MultipleParentPageId.SiteId = siteId;

            if (!MultipleParentPageId.Value.HasValue)
            {
                return;
            }

            var parentId = MultipleParentPageId.Value.Value;
            if (!ServiceLocator.PageSearch.Exists(x => x.SiteIdentifier == siteId && x.PageIdentifier == parentId))
            {
                MultipleParentPageId.Value = null;
            }
        }

        private void UploadNextButton()
        {
            if (UploadFileType.Value == "MD")
                OutlineMarkdownNextButton();
            else if (UploadFileType.Value == "JSON")
                OutlineJsonNextButton();
        }

        private void OutlineMarkdownNextButton()
        {
            PreviewContentLabelsField.Visible = true;
            ClearPreviewSection();

            if (!Page.IsValid || !ParseOutline(MultipleParentPageId.Value.HasValue, out var rootItems))
            {
                return;
            }

            RootItems = rootItems;
            PreviewSection.Visible = true;
            PreviewSection.Focus();
            ShowSave();

            MultipleFormData = new MultipleFormDataItem
            {
                WebSiteId = MultipleWebSiteSelector.ValueAsGuid,
                ParentWebPageId = MultipleParentPageId.Value,
            };

            TreeViewRepeater.DataBind();
        }

        private void TreeViewRepeater_DataBinding(object sender, EventArgs e)
        {
            var dataSource = new List<TreeViewDataItem>();

            if (RootItems != null)
            {
                var types = PageTypes.GetAll().ToDictionary(x => x, x => x);
                var lineItems = RootItems.SelectMany(x => x.ToArray()).Where(x => x.Title.HasValue());

                LineDataItem prevItem = null;

                foreach (var lineItem in lineItems)
                {
                    if (!types.TryGetValue(lineItem.Type, out var classification))
                        classification = "Page";

                    var dataItem = new TreeViewDataItem
                    {
                        Type = classification,
                        Title = lineItem.Title,
                    };

                    if (prevItem == null)
                    {
                        dataItem.HtmlPrefix = $"<ul id='{TreeViewRepeater.ClientID}' class='tree-view' data-default-level='all'><li>";
                    }
                    else if (prevItem == lineItem.Parent)
                    {
                        dataItem.HtmlPrefix = "<ul class='tree-view'><li>";
                    }
                    else if (prevItem.Parent == lineItem.Parent)
                    {
                        dataItem.HtmlPrefix = "<li>";
                    }
                    else
                    {
                        dataItem.HtmlPrefix = BuildTreeEnd(prevItem.Indent - lineItem.Indent) + "<li>";
                    }

                    if (prevItem != null && lineItem.Children.Count == 0)
                    {
                        dataItem.HtmlPostfix = $"</li>";
                    }

                    dataSource.Add(dataItem);

                    prevItem = lineItem;
                }

                var lastDataItem = dataSource[dataSource.Count - 1];
                if (prevItem != null && prevItem.Indent > 1)
                {
                    lastDataItem.HtmlPostfix += BuildTreeEnd(prevItem.Indent - 1);
                }

                lastDataItem.HtmlPostfix += "</ul>";

                string BuildTreeEnd(int levels) =>
                    string.Concat(Enumerable.Repeat("</ul></li>", levels));
            }

            TreeViewRepeater.DataSource = dataSource;
        }

        private void TreeViewRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var dataItem = (TreeViewDataItem)e.Item.DataItem;
            var typeSelector = (WebPageTypeComboBox)e.Item.FindControl("TypeSelector");
            typeSelector.EnsureDataBound();
            typeSelector.Value = dataItem.Type;
        }

        private void OutlineJsonNextButton()
        {
            var json = UploadJsonInput.Text;

            try
            {
                JsonData = null;
                if (json.IsNotEmpty())
                    JsonData = JsonConvert.DeserializeObject<QPageExport>(json);

                if (JsonData == null)
                {
                    ScreenStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                    return;
                }

                PreviewSection.Visible = true;
                PreviewSection.Focus();
                ShowSave();
                RootItems = null;

                LoadItemsFromJson(null, JsonData);

                MultipleFormData = new MultipleFormDataItem()
                { WebSiteId = SiteIdentifier, ParentWebPageId = ParentIdentifier };

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

        private void LoadItemsFromJson(LineDataItem parent, QPageExport page)
        {
            var indent = parent != null ? parent.Indent + 1 : 1;

            var item = new LineDataItem(indent)
            {
                Sequence = parent != null ? parent.Children.Count + 1 : 1,
                Title = page.Title,
                Type = page.PageType
            };

            if (parent != null)
                parent.Add(item);
            else
                RootItems = new LineDataItem[] { item };

            if (page.Children.IsNotEmpty())
            {
                foreach (var child in page.Children)
                    LoadItemsFromJson(item, child);
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
            SinglePageType.Value = "Page";

            var page = ParentIdentifier.HasValue
                ? ServiceLocator.PageSearch.Select(ParentIdentifier.Value)
                : null;
            var site = SiteIdentifier.HasValue
                ? ServiceLocator.SiteSearch.Select(SiteIdentifier.Value)
                : page != null && page.SiteIdentifier.HasValue
                    ? ServiceLocator.SiteSearch.Select(page.SiteIdentifier.Value)
                    : null;

            if (site != null)
            {
                SingleWebSiteSelector.Value = site.SiteIdentifier.ToString();
                OnSingleWebSiteSelected(site.SiteIdentifier);
                SingleWebSiteSelector.Enabled = false;

                MultipleWebSiteSelector.ValueAsGuid = site.SiteIdentifier;
                OnMultipleWebSiteSelected();
                MultipleWebSiteSelector.Enabled = false;

                if (page != null && (!page.SiteIdentifier.HasValue || page.SiteIdentifier != site.SiteIdentifier))
                {
                    page = null;
                }
            }

            if (page != null)
            {
                SingleParentPageId.Value = page.PageIdentifier;
                SingleParentPageId.AllowEdit = false;
                SingleWebSiteSelector.Enabled = false;

                MultipleParentPageId.Value = page.PageIdentifier;
                MultipleParentPageId.AllowEdit = false;
                MultipleWebSiteSelector.Enabled = false;
            }

            Outline.Text = DefaultOutlines[0].Text;
            ContentBlocksEditorBase.BindControlSelector(SingleBlockControl);

            CancelButton.NavigateUrl = GetBackUrl();
        }

        private bool ParseOutline(bool hasParent, out LineDataItem[] roots)
        {
            var lines = Outline.Text.Replace("\r", string.Empty).Split('\n');

            var minIndent = hasParent ? 1 : 2;
            var isValid = true;
            LineDataItem currentItem = null;
            var rootList = new List<LineDataItem>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var trimmedLine = StringHelper.TrimAndClean(line);

                if (string.IsNullOrEmpty(trimmedLine))
                {
                    continue;
                }

                var match = RegexOutlineLinePattern.Match(trimmedLine);

                if (!match.Success)
                {
                    ErrorOccurred("Invalid format");
                    break;
                }

                var item = new LineDataItem(match.Groups["Indent"].Value.Length);

                if (currentItem == null)
                {
                    if (item.Indent != 1)
                    {
                        ErrorOccurred($"The indent of first item must equal 1");
                        break;
                    }

                    rootList.Add(item);

                    item.Sequence = rootList.Count;
                }
                else
                {
                    if (item.Indent < minIndent)
                    {
                        ErrorOccurred($"The line indent can't be less than {minIndent}");
                        break;
                    }

                    if (item.Indent > currentItem.Indent + 1)
                    {
                        ErrorOccurred($"The line indent exceeds a maximum size of {currentItem.Indent + 1}");
                        break;
                    }

                    var parent = currentItem.FindParent(item.Indent - 1);

                    if (parent != null)
                    {
                        if (parent.Type.IsEmpty())
                            parent.Type = "Folder";
                        parent.Add(item);

                        item.Sequence = parent.Children.Count;
                    }
                    else
                    {
                        rootList.Add(item);

                        item.Sequence = rootList.Count;
                    }
                }

                item.Title = match.Groups["Title"].Value;
                item.Type = "Page";

                ParseType(item);

                currentItem = item;

                void ErrorOccurred(string error)
                {
                    isValid = false;
                    ScreenStatus.AddMessage(AlertType.Error, $"<strong>Line {i + 1:n0}</strong> - {error}:<br/>{HttpUtility.HtmlEncode(trimmedLine)}");
                }

                void ParseType(LineDataItem l)
                {
                    if (l.Title.IsEmpty())
                        return;

                    var p = @"^(.+)\s+\[(.+)\]$";
                    var m = Regex.Match(l.Title, p);
                    if (m.Success)
                    {
                        l.Title = m.Groups[1].Value.Trim();
                        l.Type = m.Groups[2].Value.Trim();

                        if (l.Type.Contains(":"))
                        {
                            var t = l.Type.Split(new char[] { ':' });
                            l.Type = t[0];
                            l.Layout = t[1];
                        }
                    }
                }
            }

            if (isValid)
            {
                roots = rootList.ToArray();
                foreach (var item in roots)
                {
                    if (!item.Children.All(x => StringHelper.Equals(x.Type, "Block")))
                        item.Type = item.Children.Count > 0 ? "Folder" : "Page";
                }
            }
            else
            {
                roots = null;
            }

            return isValid;
        }

        #endregion

        #region Database operations

        private void SaveOne()
        {
            var entity = SiteHelper.CreatePage(SinglePageType.Value, SingleTitle.Text);
            entity.SiteIdentifier = SingleWebSiteSelector.ValueAsGuid;
            entity.IsHidden = true;
            entity.PageSlug = SingleSlug.Text.IfNullOrEmpty(() => StringHelper.Sanitize(entity.Title, '-', true, new[] { '_' }));
            entity.AuthorName = CurrentSessionState.Identity.User.FullName;
            entity.AuthorDate = DateTimeOffset.Now;
            entity.ContentLabels = ContentLabels.Text;

            var isPage = entity.PageType == "Page";
            var isFolder = entity.PageType == "Folder";
            var isBlock = entity.PageType == "Block";

            if (isFolder)
                entity.ContentControl = TranslateContentControl(SinglePageType.Value);
            else if (isPage)
                entity.ContentControl = TranslateContentControl("Article");
            else if (isBlock)
                entity.ContentControl = SingleBlockControl.Value;

            if (SingleParentPageId.Value.HasValue)
                entity.ParentPageIdentifier = SingleParentPageId.Value;
            else if (ParentIdentifier.HasValue)
                entity.ParentPageIdentifier = ParentIdentifier.Value;

            var commands = new PageCommandGenerator().GetCommands(entity);
            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            if (isPage || isFolder)
            {
                var content = ServiceLocator.ContentSearch.GetBlock(entity.PageIdentifier);
                content.Title.Text.Default = entity.Title;
                ServiceLocator.SendCommand(new InSite.Application.Pages.Write.ChangePageContent(entity.PageIdentifier, content));
            }

            var redirectToId = isBlock ? entity.ParentPageIdentifier : entity.PageIdentifier;
            HttpResponseHelper.Redirect("/ui/admin/sites/pages/outline?id={0}".Format(redirectToId));
        }

        private void SaveDuplicate()
        {
            var p1 = ServiceLocator.PageSearch.Select(CopyWebPageId.Value.Value, x => x.Children, x => x.Children.Select(y => y.Children));

            var grandparent = SiteHelper.CopyPage(p1);
            grandparent.PageSlug += "-copy";

            var commands = new PageCommandGenerator().GetCommands(grandparent);
            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            var content = ServiceLocator.ContentSearch.GetContentContainerCopy(p1.PageIdentifier, GetContentFields(p1));
            ServiceLocator.SendCommand(new InSite.Application.Pages.Write.ChangePageContent(grandparent.PageIdentifier, content));

            foreach (var p2 in p1.Children.OrderBy(x => x.Sequence))
            {
                var parent = SiteHelper.CopyPage(p2);
                parent.ParentPageIdentifier = grandparent.PageIdentifier;

                var cmds = new PageCommandGenerator().GetCommands(parent);
                foreach (var command in cmds)
                    ServiceLocator.SendCommand(command);

                var p2Content = ServiceLocator.ContentSearch.GetContentContainerCopy(p2.PageIdentifier, GetContentFields(p2));
                ServiceLocator.SendCommand(new InSite.Application.Pages.Write.ChangePageContent(parent.PageIdentifier, p2Content));

                foreach (var p3 in p2.Children.OrderBy(x => x.Sequence))
                {
                    var child = SiteHelper.CopyPage(p3);
                    child.ParentPageIdentifier = parent.PageIdentifier;
                    var cmds2 = new PageCommandGenerator().GetCommands(child);
                    foreach (var command in cmds2)
                        ServiceLocator.SendCommand(command);

                    var p3Content = ServiceLocator.ContentSearch.GetContentContainerCopy(p3.PageIdentifier, GetContentFields(p3));
                    ServiceLocator.SendCommand(new InSite.Application.Pages.Write.ChangePageContent(child.PageIdentifier, p3Content));
                }
            }

            HttpResponseHelper.Redirect(String.Format("/ui/admin/sites/pages/outline?id={0}", grandparent.PageIdentifier));
        }

        private string[] GetContentFields(QPage entity)
        {
            if (entity.PageType == "Block")
                return ServiceLocator.ContentSearch.SelectContainerByLanguage(entity.PageIdentifier, "en")
                    .Select(x => x.ContentLabel)
                    .ToArray();

            return (entity.ContentLabels ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.IsNotEmpty())
                .ToArray();
        }

        private void SaveOutline()
        {
            GetTreeViewInputs();

            var entities = new List<QPage>();

            foreach (var item in RootItems)
            {
                var entity = GetPage(MultipleFormData.WebSiteId, item);
                entity.ParentPageIdentifier = MultipleFormData.ParentWebPageId;
                entities.Add(entity);
            }

            var isJsonUpload = CreationType.ValueAsEnum == CreationTypeEnum.Upload && UploadFileType.Value == "JSON";

            if (PreviewContentLabelsField.Visible && !isJsonUpload)
            {
                foreach (var entity in entities)
                    SetContentLabels(entity);
            }

            foreach (var entity in entities)
            {
                entity.IsHidden = true;
                entity.AuthorName = CurrentSessionState.Identity.User.FullName;
                entity.AuthorDate = DateTimeOffset.Now;

                var commands = new PageCommandGenerator().GetCommands(entity);

                foreach (var command in commands)
                    ServiceLocator.SendCommand(command);

                foreach (var child in entity.Children.OrderBy(x => x.Sequence))
                {
                    SetContentLabels(child);
                    child.IsHidden = true;
                    child.AuthorName = CurrentSessionState.Identity.User.FullName;
                    child.AuthorDate = DateTimeOffset.Now;
                    child.ParentPageIdentifier = entity.PageIdentifier;

                    var childCommands = new PageCommandGenerator().GetCommands(child);

                    foreach (var command in childCommands)
                        ServiceLocator.SendCommand(command);
                }
            }

            HttpResponseHelper.Redirect(String.Format("/ui/admin/sites/pages/outline?id={0}", entities[0].PageIdentifier));
        }

        private void GetTreeViewInputs()
        {
            var index = 0;
            var lineItems = RootItems.SelectMany(x => x.ToArray()).ToArray();

            foreach (RepeaterItem item in TreeViewRepeater.Items)
            {
                if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
                {
                    return;
                }

                var lineItem = lineItems[index++];
                var typeSelector = (WebPageTypeComboBox)item.FindControl("TypeSelector");

                lineItem.Type = typeSelector.Value;
            }
        }

        private void SetContentLabels(QPage page)
        {
            if (page.PageType == "Folder")
                page.ContentLabels = "Title, Summary, ImageUrl";

            else if (page.PageType == "Page")
                page.ContentLabels = ContentLabels.Text;

            foreach (var child in page.Children)
                SetContentLabels(child);
        }

        private static QPage GetPage(Guid? siteId, LineDataItem item)
        {
            var page = SiteHelper.CreatePage(item.Type, item.Title);

            page.Sequence = item.Sequence;
            page.SiteIdentifier = siteId;

            if (item.Layout.HasValue() && StringHelper.Equals(item.Type, "Block"))
                page.ContentControl = item.Layout;

            foreach (var childItem in item.Children)
            {
                var childPage = GetPage(siteId, childItem);

                page.Children.Add(childPage);
            }

            return page;
        }

        #endregion

        #region Methods (helpers)

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/sites/pages/search", true);

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

        private string TranslateContentControl(string contentControlTitle)
        {
            foreach (var info in ControlPath.PageControlTypes)
            {
                if (info.Title.Equals(contentControlTitle))
                    return info.Name;
            }
            return contentControlTitle;
        }

        #endregion

        #region IHasParentLinkParameters

        private string GetBackUrl() =>
            new ReturnUrl().GetReturnUrl() ?? GetReaderUrl();

        private string GetReaderUrl()
        {
            return HttpResponseHelper.BuildUrl("/ui/admin/sites/pages/search", "type=Page");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={PageIdentifier}"
                : null;
        }

        #endregion
    }
}