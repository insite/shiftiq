using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;
using InSite.Web.Infrastructure;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Trees;
using Shift.Constant;

namespace InSite.Admin.Standards.Standards.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/standards/edit";
        private const string SearchUrl = "/admin/standards/search";

        private static readonly Regex RegexOutlineLinePattern = new Regex("^(?<Indent>#+)(?<Title>.+)$", RegexOptions.Compiled);

        private const string DefaultTiers = "Framework, Area, Competency";

        private static readonly OutlineExampleInfo[] DefaultOutlines = new[]
        {
            new OutlineExampleInfo("4th Class Power Engineer Competency Framework", @"# 4th Class Power Engineer Competency Framework

## Part A

### Applied Mathematics

### Elementary Mechanics and Dynamics

### Elementary Thermodynamics

### Mechanical Drawing, Administration

### Industrial Legislation

### Workplace Hazardous Materials

### Plant Safety

### Plant Fire Protection

### Environment

### Materials and Welding

### Piping and Valves

### High Pressure Boiler Design

### High Pressure Boiler Parts and Fittings

### High Pressure Boiler Operation; Feedwater Treatment

## Part B

### Prime Movers and Engines

### Pumps and Compressors

### Lubrication

### Electricity

### Controls, Instrumentation and Computers

### Heating Boilers

### Heating Systems

### Heating Boilers and Heating System Controls

### Auxiliary Building Systems

### Vapour Compression Refrigeration

### Absorption Refrigeration

### Air Conditioning

### Air Conditioning Systems

### Boiler Maintenance

### Types of Plants"),
            new OutlineExampleInfo("Carpenter Competency Framework Level 1", @"# Carpenter Competency Framework Level 1

## Safe Work Practices

### Apply Shop and Site Safety Practices

### Apply Personal Safety Practices

## Documentation and Organizational Skills

### Describe Carpentry Trade

### Use Construction Drawings and Specifications

### Interpret Building Codes and Bylaws

### Plan and Organize Work

### Perform Trade Math

## Tools and Equipment

### Use Hand Tools

### Use Portable Power Tools

### Use Stationary Power Tools

## Survey Instruments and Equipment

### Use Levelling Instruments and Equipment

## Access, Rigging, and Hoisting Equipment

### Use Ladders, Scaffolds and Access Equipment

## Site Layout

### Lay Out Building Locations

## Concrete Formwork

### Use Concrete Types, Materials, Additives and Treatments

### Build Footing and Vertical Formwork

## Wood Frame Construction

### Describe Wood Frame Construction

### Select Framing Materials

### Build Floor Systems

### Build Wall Systems

### Build Stair Systems

### Build Roof Systems

## Building Science

### Control the Forces Acting on a Building
")
        };

        private class CodeAndTitle
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Label { get; set; }
        }

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
        private class MarkdownDataItem
        {

            public int Indent { get; }

            public string Type { get; set; }

            public string Label { get; set; }

            public string Summary { get; set; }

            public string Code { get; set; }

            public string Title { get; set; }

            public MarkdownDataItem Parent { get; private set; }

            public IReadOnlyList<MarkdownDataItem> Children => _children;

            private List<MarkdownDataItem> _children = new List<MarkdownDataItem>();

            public MarkdownDataItem(int indent)
            {
                Indent = indent;
            }

            public void Add(MarkdownDataItem item)
            {
                if (item.Parent != null)
                    throw new ApplicationError("Parent != null");

                item.Parent = this;

                _children.Add(item);
            }

            public MarkdownDataItem FindParent(int indent) =>
                Indent == indent ? this : Parent?.FindParent(indent);

            public MarkdownDataItem[] ToArray()
            {
                var list = new List<MarkdownDataItem>();

                AddItems(this);

                return list.ToArray();

                void AddItems(MarkdownDataItem item)
                {
                    list.Add(item);

                    foreach (var child in item._children)
                        AddItems(child);
                }
            }

            public CreateAssetTree ToAssetTree()
            {
                var index = -1;
                var tree = new CreateAssetTree();

                var root = GetNode(this, 1);
                tree.Add(root);

                AddItems(this, root);

                return tree;

                void AddItems(MarkdownDataItem itemParent, CreateAssetNode nodeParent)
                {
                    for (var i = 0; i < itemParent._children.Count; i++)
                    {
                        var chilItem = itemParent._children[i];
                        var childNode = GetNode(chilItem, i + 1);

                        tree[nodeParent].AddChild(childNode);

                        if (chilItem._children.Count > 0)
                            AddItems(chilItem, childNode);
                    }
                }

                CreateAssetNode GetNode(MarkdownDataItem item, int sequence)
                {
                    return new CreateAssetNode
                    {
                        Id = index++,
                        Sequence = sequence,
                        Type = item.Type,
                        Label = item.Label,
                        Summary = item.Summary,
                        Code = item.Code,
                        Title = item.Title,
                    };
                }
            }

        }

        private class TreeViewDataItem
        {
            public string Type { get; }
            public string Label { get; }
            public string Summary { get; }
            public string Code { get; }
            public string Title { get; }
            public int Depth { get; }

            public TreeViewDataItem Prev { get; private set; }
            public TreeViewDataItem Next { get; private set; }

            public string Icon { get; set; }

            private TreeViewDataItem(TreeViewDataItem prev)
            {
                if (prev == null)
                    return;

                prev.Next = this;
                this.Prev = prev;
            }

            public TreeViewDataItem(MarkdownDataItem data, TreeViewDataItem prev)
                : this(prev)
            {
                Type = data.Type;
                Label = data.Label;
                Summary = data.Summary;
                Code = data.Code;
                Title = data.Title;
                Depth = data.Indent;
            }

            public TreeViewDataItem(StandardModel data, TreeViewDataItem prev)
                : this(prev)
            {
                Type = data.StandardType;
                Summary = data.Summary;
                Code = data.Code;
                Title = data.Title;
                Depth = data.Depth;
            }
        }

        private class HierarchyItem
        {
            public QStandard Entity { get; }
            public HierarchyItem Parent { get; private set; }
            public IReadOnlyList<HierarchyItem> Children => _children;

            private List<HierarchyItem> _children = new List<HierarchyItem>();

            public HierarchyItem(QStandard entity)
            {
                Entity = entity;
            }

            public HierarchyItem AddChild(string standardType) =>
                AddChild(StandardFactory.Create(standardType));

            public HierarchyItem AddChild(QStandard entity)
            {
                var result = new HierarchyItem(entity) { Parent = this };
                _children.Add(result);
                return result;
            }
        }

        protected string[] ValidSubtypes => (string[])(ViewState[nameof(ValidSubtypes)]
            ?? (ViewState[nameof(ValidSubtypes)] = StandardSearch.GetAllTypeNames(Identity.Organization.Identifier).ToArray()));

        private MarkdownDataItem[] MarkdownData
        {
            get => (MarkdownDataItem[])ViewState[nameof(MarkdownData)];
            set => ViewState[nameof(MarkdownData)] = value;
        }

        private StandardPublicationModel JsonData
        {
            get => (StandardPublicationModel)ViewState[nameof(JsonData)];
            set => ViewState[nameof(JsonData)] = value;
        }

        private Guid? MultipleParentID
        {
            get => (Guid?)ViewState[nameof(MultipleParentID)];
            set => ViewState[nameof(MultipleParentID)] = value;
        }

        private Guid? _mdFileStorageId;
        private Guid? _jsonFileStorageId;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            TiersValidator.ServerValidate += TiersValidator_ServerValidate;

            OutlineNextButton.Click += OutlineNextButton_Click;
            ReviewSaveButton.Click += ReviewSaveButton_Click;

            TreeViewRepeater.ItemDataBound += TreeViewRepeater_ItemDataBound;

            UploadFileType.AutoPostBack = true;
            UploadFileType.ValueChanged += UploadFileType_ValueChanged;

            MarkdownFileUploadExtensionValidator.ServerValidate += MarkdownFileUploadExtensionValidator_ServerValidate;
            MarkdownZipFileValidator.ServerValidate += MarkdownZipFileValidator_ServerValidate;
            MarkdownFileUploadButton.Click += MarkdownFileUploadButton_Click;
            UploadMarkdownSaveButton.Click += UploadMarkdownSaveButton_Click;
            DefaultOutlineRepeater.ItemCommand += DefaultOutlineRepeater_ItemCommand;

            JsonFileUploadExtensionValidator.ServerValidate += JsonFileUploadExtensionValidator_ServerValidate;
            JsonZipFileValidator.ServerValidate += JsonZipFileValidator_ServerValidate;
            JsonFileUploadButton.Click += JsonFileUploadButton_Click;
            UploadJSONSaveButton.Click += UploadJSONSaveButton_Click;

            ReviewPrevButton.Click += (x, y) => MainMultiView.SetActiveView(ViewStandard);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write))
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, null, "[New Standard]");

            MainMultiView.SetActiveView(ViewStandard);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Outline, CreationTypeEnum.Upload);
            OnCreationTypeSelected();

            SetDefaultInputValues();
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            switch (CreationType.ValueAsEnum)
            {
                case CreationTypeEnum.One: StandardMultiView.SetActiveView(ViewStandardOne); break;
                case CreationTypeEnum.Outline: StandardMultiView.SetActiveView(ViewStandardOutline); break;
                case CreationTypeEnum.Upload: StandardMultiView.SetActiveView(ViewStandardUpload); break;
                default: StandardMultiView.ActiveViewIndex = -1; break;
            }
        }

        private void UploadFileType_ValueChanged(object sender, EventArgs e) => OnUploadFileTypeChanged();

        private void OnUploadFileTypeChanged()
        {
            var value = UploadFileType.Value;
            var isMarkdown = value == "MD";
            var isJson = value == "JSON";

            MarkdownUploadContainer.Visible = isMarkdown;
            UploadMarkdownSaveButton.Visible = isMarkdown;

            JsonUploadContainer.Visible = isJson;
            UploadJSONSaveButton.Visible = isJson;

            JsonInput.Text = "";
        }

        private void DefaultOutlineRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "LoadOutline")
            {
                var index = int.Parse((string)e.CommandArgument);
                Outline.Text = DefaultOutlines[index].Text;

                CreationType.ValueAsEnum = CreationTypeEnum.Outline;
                OnCreationTypeSelected();
            }
        }

        private void MarkdownFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs args) =>
            args.IsValid = args.Value == null || args.Value.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || args.Value.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

        private void MarkdownZipFileValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            EnsureFileUploaded(MarkdownFileUpload, ref _mdFileStorageId);

            args.IsValid = ValidateZip(_mdFileStorageId, (BaseValidator)source, new[] { ".txt" });
        }

        private void MarkdownFileUploadButton_Click(object sender, EventArgs e)
        {
            EnsureFileUploaded(MarkdownFileUpload, ref _mdFileStorageId);
            ReadUploadedFile(_mdFileStorageId, stream =>
            {
                if (stream == Stream.Null)
                    return;

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    JsonInput.Text = reader.ReadToEnd();
                }
            });

            if (JsonInput.Text == "")
            {
                CreatorStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }
        }

        private void UploadMarkdownSaveButton_Click(object sender, EventArgs e)
        {
            const string WrongFormatError = "Uploaded File Content has wrong format";
            const string TiersStart = "Tiers:";

            string tiers = null, text = null;

            if (JsonInput.Text == "")
            {
                CreatorStatus.AddMessage(AlertType.Error, "Uploaded File Content is empty");
                return;
            }

            using (var reader = new StringReader(JsonInput.Text))
            {
                tiers = reader.ReadLine();

                if (tiers == null || !tiers.StartsWith(TiersStart, StringComparison.OrdinalIgnoreCase) || tiers.Length == TiersStart.Length)
                {
                    CreatorStatus.AddMessage(AlertType.Error, WrongFormatError);
                    return;
                }

                if (reader.ReadLine() == null)
                {
                    CreatorStatus.AddMessage(AlertType.Error, WrongFormatError);
                    return;
                }

                text = reader.ReadToEnd();

            }

            Tiers.Text = tiers.Substring(TiersStart.Length).Trim();
            Outline.Text = text;
            CreationType.ValueAsEnum = CreationTypeEnum.Outline;
            OnCreationTypeSelected();
        }

        private void TiersValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            var validValues = new HashSet<string>(ValidSubtypes, StringComparer.OrdinalIgnoreCase);

            e.IsValid = string.IsNullOrEmpty(e.Value) || StringHelper.Split(e.Value).All(x => validValues.Contains(x));
        }

        private void JsonFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs args) =>
            args.IsValid = args.Value == null || args.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || args.Value.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

        private void JsonZipFileValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            EnsureFileUploaded(JsonFileUpload, ref _jsonFileStorageId);

            args.IsValid = ValidateZip(_jsonFileStorageId, (BaseValidator)source, new[] { ".json" });
        }

        private void JsonFileUploadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            EnsureFileUploaded(JsonFileUpload, ref _jsonFileStorageId);
            ReadUploadedFile(_jsonFileStorageId, stream =>
            {
                if (stream == Stream.Null)
                    return;

                try
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var json = reader.ReadToEnd();
                        JsonInput.Text = json;
                    }
                }
                catch (Exception ex)
                {
                    CreatorStatus.AddMessage(
                        AlertType.Error,
                        $"An error occurred during reading the uploaded JSON file:<p>{HttpUtility.HtmlEncode(ex.Message)}</p>");

                    return;
                }
            });

            if (JsonInput.Text == "")
            {
                CreatorStatus.AddMessage(AlertType.Error, "Uploaded JSON file is empty.");
                return;
            }
        }

        private void UploadJSONSaveButton_Click(object sender, EventArgs e)
        {
            ClearMultiple();

            try
            {
                var json = JsonInput.Text;

                var publication = JsonConvert.DeserializeObject<StandardPublicationModel>(json);
                if (publication.Asset != null && publication.Asset.Children.Count > 0)
                    JsonData = publication;
            }

            catch (Exception ex)
            {
                CreatorStatus.AddMessage(
                    AlertType.Error,
                    $"An error occurred during reading the uploaded JSON file:<p>{HttpUtility.HtmlEncode(ex.Message)}</p>");

                return;
            }

            if (JsonData == null)
            {
                CreatorStatus.AddMessage(AlertType.Error, "Uploaded File Content is empty.");
                return;
            }

            MainMultiView.SetActiveView(ViewReview);

            var dataSource = new List<TreeViewDataItem>();
            var subtypes = StandardSearch.GetAllTypeNames(Identity.Organization.Identifier).ToDictionary(x => x, x => x);

            TreeViewDataItem prevItem = null;

            foreach (var standard in EnumerateFlatten(JsonData.Asset))
            {
                if (standard.StandardType.IsEmpty())
                    standard.StandardType = Shift.Constant.StandardType.Competency;

                dataSource.Add(prevItem = new TreeViewDataItem(standard, prevItem)
                {
                    Icon = StandardSearch.GetStandardTypeIcon(standard.StandardType),
                });
            }

            TreeViewRepeater.DataSource = dataSource;
            TreeViewRepeater.DataBind();
        }

        private void OutlineNextButton_Click(object sender, EventArgs e)
        {
            ClearMultiple();

            if (!Page.IsValid || !ParseOutline(MultipleParentAssetID.Value.HasValue, out var rootItems))
                return;

            MarkdownData = rootItems;
            MainMultiView.SetActiveView(ViewReview);
            MultipleParentID = MultipleParentAssetID.Value;

            var dataSource = new List<TreeViewDataItem>();
            var subtypes = StandardSearch.GetAllTypeNames(Identity.Organization.Identifier).ToDictionary(x => x, x => x);

            TreeViewDataItem prevItem = null;

            foreach (var lineItem in MarkdownData.SelectMany(x => x.ToArray()))
            {
                dataSource.Add(prevItem = new TreeViewDataItem(lineItem, prevItem)
                {
                    Icon = StandardSearch.GetStandardTypeIcon(lineItem.Type),
                });
            }

            TreeViewRepeater.DataSource = dataSource;
            TreeViewRepeater.DataBind();
        }

        private void TreeViewRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var dataItem = (TreeViewDataItem)e.Item.DataItem;

            var typeSelector = (StandardTypeComboBox)e.Item.FindControl("TypeSelector");
            typeSelector.Value = dataItem.Type;

            var htmlPrefix = (ITextControl)e.Item.FindControl("HtmlPrefix");
            var htmlPostfix = (ITextControl)e.Item.FindControl("HtmlPostfix");

            if (dataItem.Prev == null)
                htmlPrefix.Text = $"<ul id='{TreeViewRepeater.ClientID}' class='tree-view' data-default-level='all'><li>";
            else if (dataItem.Prev.Depth == dataItem.Depth - 1)
                htmlPrefix.Text = "<ul class='tree-view'><li>";
            else if (dataItem.Prev.Depth == dataItem.Depth)
                htmlPrefix.Text = "<li>";
            else
                htmlPrefix.Text = BuildTreeEnd(dataItem.Prev.Depth - dataItem.Depth) + "<li>";

            if (dataItem.Prev != null && (dataItem.Next == null || dataItem.Next.Depth <= dataItem.Depth))
                htmlPostfix.Text = "</li>";

            if (dataItem.Next == null)
            {
                if (dataItem.Depth > 0)
                    htmlPostfix.Text += BuildTreeEnd(dataItem.Depth);

                htmlPostfix.Text += "</ul>";
            }

            string BuildTreeEnd(int levels) =>
                string.Concat(Enumerable.Repeat("</ul></li>", levels));
        }

        private void ReviewSaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (MarkdownData != null)
            {
                // Dec 12, 2024 - Oleg: It seems to do nothing
                var items = MarkdownData.SelectMany(x => x.ToArray()).ToArray();
                GetInputValues((index, title, type) =>
                {
                    var item = items[index];
                    item.Title = title;
                    item.Type = type;
                });
            }
            else if (JsonData != null)
            {
                // Dec 12, 2024 - Oleg: It seems to do nothing
                var items = EnumerateFlatten(JsonData.Asset).ToArray();
                GetInputValues((index, title, type) =>
                {
                    var item = items[index];
                    item.Title = title;
                    item.StandardType = type;
                });
            }

            SaveClicked();

            void GetInputValues(Action<int, string, string> update)
            {
                for (var i = 0; i < TreeViewRepeater.Items.Count; i++)
                {
                    var item = TreeViewRepeater.Items[i];
                    if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
                        return;

                    var titleInput = (ITextBox)item.FindControl("TitleInput");
                    var typeSelector = (StandardTypeComboBox)item.FindControl("TypeSelector");

                    update(i, titleInput.Text, typeSelector.Value);
                }
            }
        }

        private void SaveClicked()
        {
            Page.Validate();

            if (!Page.IsValid)
                return;

            var standardId = Save();
            if (standardId != null)
                HttpResponseHelper.Redirect($"{EditUrl}?id={standardId}&status=saved");
        }

        private void SetDefaultInputValues()
        {
            Tiers.Text = DefaultTiers;
            Outline.Text = DefaultOutlines[0].Text;
            AuthorName.Text = User.FullName;
            AuthorDate.Value = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, User.TimeZone);
            StandardType.Value = Shift.Constant.StandardType.Competency;

            DefaultOutlineRepeater.DataSource = DefaultOutlines;
            DefaultOutlineRepeater.DataBind();

            OnUploadFileTypeChanged();
        }

        private string[] GetTiersSubtypes()
        {
            var values = StringHelper.Split(Tiers.Text);

            var validTiers = ValidSubtypes.ToDictionary(x => x, x => x, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < values.Length; i++)
                values[i] = validTiers[values[i]];

            return values;
        }

        private bool ParseOutline(bool hasParent, out MarkdownDataItem[] roots)
        {
            var subtypes = GetTiersSubtypes();
            var allSubtypes = StandardSearch.GetAllTypeNames(Identity.Organization.Identifier).Select(x => x).ToList();
            var lines = Outline.Text.Replace("\r", string.Empty).Split('\n');

            var minIndent = hasParent ? 1 : 2;
            var isValid = true;
            MarkdownDataItem currentItem = null;
            var isReadingSummary = false;
            var rootList = new List<MarkdownDataItem>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var trimmedLine = StringHelper.TrimAndClean(line);

                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                var match = RegexOutlineLinePattern.Match(trimmedLine);

                if (!match.Success && isReadingSummary)
                {
                    if (string.IsNullOrEmpty(currentItem.Summary)) currentItem.Summary = line;
                    else currentItem.Summary += "\r\n" + line;
                    continue;
                }

                if (currentItem != null && trimmedLine == "Summary:")
                {
                    isReadingSummary = true;
                    continue;
                }

                isReadingSummary = false;

                if (!match.Success)
                {
                    ErrorOccurred("Invalid format");
                    break;
                }

                var item = new MarkdownDataItem(match.Groups["Indent"].Value.Length);

                if (currentItem == null)
                {
                    if (item.Indent != 1)
                    {
                        ErrorOccurred($"The indent of first item must equal 1");
                        break;
                    }

                    rootList.Add(item);
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
                }

                if (subtypes.Length < item.Indent)
                {
                    ErrorOccurred($"The line indent is {item.Indent} while count of defined subtypes is {subtypes.Length}");
                    break;
                }

                var codeAndTitle = SplitCodeAndTitle(allSubtypes, match.Groups["Title"].Value);

                item.Code = codeAndTitle.Code;
                item.Title = codeAndTitle.Name;
                item.Type = !string.IsNullOrEmpty(codeAndTitle.Type) ? codeAndTitle.Type : subtypes[item.Indent - 1];
                item.Label = codeAndTitle.Label;

                if (currentItem != null)
                {
                    var parent = currentItem.FindParent(item.Indent - 1);

                    if (parent != null)
                        parent.Add(item);
                    else if (hasParent)
                        rootList.Add(item);
                    else
                        throw new ApplicationError("Parent not found");
                }

                currentItem = item;

                void ErrorOccurred(string error)
                {
                    isValid = false;
                    CreatorStatus.AddMessage(AlertType.Error, $"<strong>Line {i + 1:n0}</strong> - {error}:<br/>{HttpUtility.HtmlEncode(trimmedLine)}");
                }
            }

            roots = isValid ? rootList.ToArray() : null;

            return isValid;
        }

        private static CodeAndTitle SplitCodeAndTitle(List<string> allSubtypes, string text)
        {
            var result = new CodeAndTitle();

            if (string.IsNullOrEmpty(text))
                return result;

            var periodIndex = text.IndexOf('.');

            if (periodIndex >= 0 && periodIndex != text.Length - 1)
            {
                result.Code = StringHelper.TrimAndClean(text.Substring(0, periodIndex));
                result.Name = StringHelper.TrimAndClean(text.Substring(periodIndex + 1));
            }
            else
            {
                result.Name = StringHelper.TrimAndClean(text);
            }

            SplitTitleAndType(allSubtypes, result);

            return result;
        }

        private static void SplitTitleAndType(List<string> allSubtypes, CodeAndTitle codeAndTitle)
        {
            var title = codeAndTitle.Name;

            if (title != null && title.EndsWith("]"))
            {
                var openingBracketIndex = title.LastIndexOf("[");

                if (openingBracketIndex > 0)
                {
                    var subtype = title.Substring(openingBracketIndex + 1, title.Length - openingBracketIndex - 2);

                    var match = allSubtypes.Find(x => x.Equals(subtype, StringComparison.OrdinalIgnoreCase));

                    if (match != null)
                    {
                        codeAndTitle.Name = StringHelper.TrimAndClean(title.Substring(0, openingBracketIndex));
                        codeAndTitle.Type = match;
                    }
                    else
                    {
                        codeAndTitle.Name = StringHelper.TrimAndClean(title.Substring(0, openingBracketIndex));
                        codeAndTitle.Label = subtype;
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var standardId = Save();
            if (standardId != null)
                HttpResponseHelper.Redirect($"{EditUrl}?id={standardId}&status=saved");
        }

        private Guid? Save()
        {
            try
            {
                if (CreationType.ValueAsEnum == CreationTypeEnum.One)
                    return SaveOne();
                else if (MarkdownData != null)
                    return SaveManyMarkdown();
                else if (JsonData != null)
                    return SaveManyJson();
            }
            catch (ApplicationError err)
            {
                if (StandardStore.IsDepthLimitException(err))
                    CreatorStatus.AddMessage(AlertType.Error, StandardStore.DepthLimitErrorText);
                else
                    throw;
            }

            return null;
        }

        private Guid? SaveOne()
        {
            var asset = StandardFactory.Create(StandardType.Value);
            asset.StandardIdentifier = UniqueIdentifier.Create();
            asset.StandardTier = AssetTier.Text;
            asset.StandardLabel = AssetLabel.Text;
            asset.ContentTitle = ContentName.Text;
            asset.ContentName = ContentName.Text;
            asset.Code = Code.Text;

            asset.ParentStandardIdentifier = SingleParentAssetID.Value;
            asset.SourceDescriptor = SourceDescriptor.Text;
            asset.AuthorName = AuthorName.Text;
            asset.AuthorDate = AuthorDate.Value;
            asset.DatePosted = DateTimeOffset.UtcNow;

            StandardStore.Insert(asset);

            return asset.StandardIdentifier;
        }

        private Guid? SaveManyMarkdown()
        {
            var rootItem = new HierarchyItem(new QStandard());

            foreach (var root in MarkdownData)
            {
                var assetTree = root.ToAssetTree();

                AddEntities(rootItem, assetTree);
            }

            if (!MultipleParentID.HasValue || rootItem.Children.Count == 1)
            {
                var item = rootItem.Children.First();

                InsertAsset(item);

                return item.Entity.StandardIdentifier;
            }
            else
            {
                foreach (var item in rootItem.Children)
                    InsertAsset(item);

                return StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.StandardIdentifier == MultipleParentID.Value);
            }

            void InsertAsset(HierarchyItem root)
            {
                foreach (var item in EnumerateFlatten(root))
                {
                    var parent = item.Parent;
                    var entity = item.Entity;

                    entity.ParentStandardIdentifier = parent == rootItem ? MultipleParentID : parent.Entity.StandardIdentifier;

                    StandardStore.Insert(entity);
                }
            }
        }

        private Guid? SaveManyJson()
        {
            var entities = new List<(QStandard, ContentContainer)>();

            AddEntities(null, JsonData.Asset, entities);

            foreach (var entity in entities)
                StandardStore.Insert(entity.Item1, entity.Item2);

            return entities[0].Item1.StandardIdentifier;
        }

        private static void AddEntities(HierarchyItem parent, CreateAssetTree tree)
        {
            foreach (var child in tree.Root.DirectChildren.Nodes)
                AddEntities(child, parent);
        }

        private static void AddEntities(INode<CreateAssetNode> node, HierarchyItem parentItem)
        {
            var childItem = parentItem.AddChild(node.Data.Type);
            var parentEntity = parentItem.Entity;
            var childEntity = childItem.Entity;

            childEntity.StandardIdentifier = UniqueIdentifier.Create();
            childEntity.StandardLabel = node.Data.Label;
            childEntity.IsHidden = parentEntity.IsHidden;
            childEntity.IsPublished = parentEntity.IsPublished;
            childEntity.Sequence = node.Data.Sequence;
            childEntity.Code = node.Data.Code;
            childEntity.ContentTitle = node.Data.Title;
            childEntity.ContentSummary = node.Data.Summary;
            childEntity.ContentDescription = node.Data.BodyText;
            childEntity.ContentName = node.Data.Title;

            foreach (var childNode in node.DirectChildren.Nodes)
                AddEntities(childNode, childItem);
        }

        private static void AddEntities(Guid? parentId, StandardModel model, List<(QStandard, ContentContainer)> list)
        {
            var entity = StandardFactory.Create(model.StandardType);

            entity.StandardIdentifier = UniqueIdentifier.Create();
            entity.ParentStandardIdentifier = parentId;
            entity.Code = model.Code;
            entity.StandardHook = model.Hook;
            entity.Icon = model.Icon;
            entity.IsHidden = model.IsHidden;
            entity.IsPractical = model.IsPractical;
            entity.IsPublished = model.IsPublished;
            entity.IsTheory = model.IsTheory;
            entity.ContentName = model.Name;
            entity.ContentSummary = model.Summary;
            entity.Sequence = model.Sequence;
            entity.SourceDescriptor = model.Source;

            var container = new ContentContainer();

            if (model.Content.IsNotEmpty())
            {
                foreach (var content in model.Content)
                {
                    var item = container[content.Label];
                    item.Text[content.Language] = content.Text;
                    item.Html[content.Language] = content.Html;
                }
            }
            else
            {
                container.Title.Text.Default = model.Title;
            }

            list.Add((entity, container));

            foreach (var childModel in model.Children)
                AddEntities(entity.StandardIdentifier, childModel, list);
        }

        private static bool ValidateZip(Guid? storageId, BaseValidator validator, IEnumerable<string> supportedTypes)
        {
            var result = false;

            if (storageId.HasValue)
            {
                GetUploadedFile(storageId.Value, file =>
                {
                    if (file == null || !file.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        result = true;
                        return;
                    }

                    try
                    {
                        using (var stream = file.Open(FileMode.Open, FileAccess.Read))
                        {
                            using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read))
                            {
                                if (zipArchive.Entries.Count == 0)
                                {
                                    validator.ErrorMessage = "The uploaded ZIP archive is empty.";
                                    return;
                                }

                                if (zipArchive.Entries.Count != 1)
                                {
                                    validator.ErrorMessage = "The uploaded ZIP archive must contain one file only.";
                                    return;
                                }

                                var entry = zipArchive.Entries[0];
                                if (supportedTypes.All(x => !entry.Name.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                                {
                                    validator.ErrorMessage = "The uploaded ZIP archive contains a file of unsupported type. File types supported: " + string.Join(" ", supportedTypes);
                                    return;
                                }
                            }
                        }

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        AppSentry.SentryError(ex);

                        validator.ErrorMessage = "The uploaded ZIP archive is broken.";
                    }
                });
            }

            return result;
        }

        private static bool EnsureFileUploaded(FileUpload upload, ref Guid? storageId)
        {
            if (storageId.HasValue)
                return true;

            if (upload.PostedFile == null || upload.PostedFile.ContentLength == 0)
                return false;

            storageId = TempFileStorage.Create();

            TempFileStorage.Open(storageId.Value, dir =>
            {
                var filePath = Path.Combine(dir.FullName, upload.PostedFile.FileName);
                upload.PostedFile.SaveAs(filePath);
            });

            return true;
        }

        private static void GetUploadedFile(Guid storageId, Action<FileInfo> action)
        {
            TempFileStorage.Open(storageId, dir =>
            {
                action(dir.GetFiles().SingleOrDefault());
            });
        }

        private static void ReadUploadedFile(Guid? storageId, Action<Stream> read)
        {
            if (!storageId.HasValue)
            {
                read(Stream.Null);
                return;
            }

            GetUploadedFile(storageId.Value, file =>
            {
                if (file == null)
                {
                    read(Stream.Null);
                    return;
                }

                using (var fs = file.Open(FileMode.Open, FileAccess.Read))
                {
                    if (file.Name.EndsWith(".zip"))
                    {
                        using (var zipArchive = new ZipArchive(fs))
                        {
                            using (var entryStream = zipArchive.Entries[0].Open())
                                read(entryStream);
                        }
                    }
                    else
                    {
                        read(fs);
                    }
                }
            });
        }

        private void ClearMultiple()
        {
            MarkdownData = null;
            JsonData = null;
            MultipleParentID = null;
        }

        private static IEnumerable<StandardModel> EnumerateFlatten(StandardModel item)
        {
            yield return item;

            if (item.Children != null)
                foreach (var child in item.Children)
                    foreach (var fChild in EnumerateFlatten(child))
                        yield return fChild;
        }

        private static IEnumerable<HierarchyItem> EnumerateFlatten(HierarchyItem entity)
        {
            yield return entity;

            if (entity.Children != null)
                foreach (var child in entity.Children)
                    foreach (var fChild in EnumerateFlatten(child))
                        yield return fChild;
        }
    }
}