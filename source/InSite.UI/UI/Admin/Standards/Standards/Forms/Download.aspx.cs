using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using DocumentFormat.OpenXml.Wordprocessing;

using HtmlToOpenXml;

using Humanizer;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Admin.Standards.Occupations.Utilities.Competencies;
using InSite.Admin.Standards.Occupations.Utilities.Competencies.ExtensionMethods;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using CheckBoxList = System.Web.UI.WebControls.CheckBoxList;
using DocumentType = Shift.Sdk.UI.DocumentType;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace InSite.Admin.Standards.Standards.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {

        private static class FileType
        {
            public const string Outline = "Outline";
            public const string NumberHierarchy = "NumberHierarchy";
            public const string Document = "Document";
        }

        private static class FileFormat
        {
            public static class Json
            {
                public const string Text = "JSON (*.json)";
                public const string Value = "JSON";
            }

            public static class Markdown
            {
                public const string Text = "Markdown (*.txt)";
                public const string Value = "MD";
            }

            public static class Excel
            {
                public const string Text = "Microsoft Excel (*.xlsx)";
                public const string Value = "XLSX";
            }

            public static class Word
            {
                public const string Text = "Microsoft Word (*.docx)";
                public const string Value = "DOCX";
            }

            public static class Csv
            {
                public const string Text = "CSV (*.csv)";
                public const string Value = "CSV";
            }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class HierarchyItem
        {
            [JsonProperty(PropertyName = "title")]
            public string Title => _dataItem.ContentTitle;

            [JsonProperty(PropertyName = "type")]
            public string Type => _dataItem.StandardType;

            [JsonProperty(PropertyName = "asset")]
            public int Asset => _dataItem.AssetNumber;

            [JsonProperty(PropertyName = "codePath")]
            public string CodePath => (Parent == null ? string.Empty : Parent.CodePath + "/") + _dataItem.Code;

            [JsonProperty(PropertyName = "assetPath")]
            public string AssetPath => (Parent == null ? string.Empty : Parent.AssetPath + "/") + _dataItem.AssetNumber.ToString();

            [JsonProperty(PropertyName = "depth")]
            public int Depth => Parent == null ? 0 : Parent.Depth + 1;

            [JsonProperty(PropertyName = "id")]
            public Guid Identifier => _dataItem.StandardIdentifier;

            [JsonProperty(PropertyName = "children")]
            public IReadOnlyList<HierarchyItem> Children => _children;

            public string DepthIndentAndTitle => new string(' ', Depth * 4) + _dataItem.ContentTitle;

            public Guid RootIdentifier => Parent == null ? Identifier : Parent.RootIdentifier;

            public Dictionary<string, string> Contents => _dataItem.Contents;

            public HierarchyItem Parent { get; private set; }

            private HierarchyDataItem _dataItem;
            private List<HierarchyItem> _children = new List<HierarchyItem>();

            public void Add(HierarchyItem item)
            {
                if (item.Parent != null)
                    return;

                item.Parent = this;
                _children.Add(item);
            }

            public void SetDataItem(HierarchyDataItem dataItem)
            {
                if (_dataItem == null)
                    _dataItem = dataItem;
            }
        }

        private class HierarchyDataItem
        {
            public Guid StandardIdentifier { get; set; }
            public string StandardType { get; set; }
            public string ContentTitle { get; set; }
            public int AssetNumber { get; set; }
            public string Code { get; set; }
            public string Hook { get; set; }
            public Dictionary<string, string> Contents { get; set; }
        }

        private Guid StandardID => Guid.TryParse(Request.QueryString["asset"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            FileTypeSelector.AutoPostBack = true;
            FileTypeSelector.SelectedIndexChanged += FileTypeSelector_SelectedIndexChanged;

            FileFormatSelector.AutoPostBack = true;
            FileFormatSelector.SelectedIndexChanged += FileFormatSelector_SelectedIndexChanged;

            DocumentSettingsManager.NeedReport += DocumentSettingsManager_NeedReport;
            DocumentSettingsManager.ReportSelected += DocumentSettingsManager_ReportSelected;

            DownloadButton.Click += DownloadButton_Click;

            base.OnInit(e);
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var standard = StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardID);
            if (standard == null)
                HttpResponseHelper.Redirect("/ui/admin/standards/standards/search", true);

            PageHelper.AutoBindHeader(this, null,
                $"{StringHelper.FirstValue(standard.ContentTitle, standard.ContentName, "Untitled")} <span class='form-text'>{standard.StandardType} Asset #{standard.AssetNumber}</span>");

            if (HasDependencyCycle())
                return;

            DocumentLanguageSelector.Settings.IncludeLanguage = new[] { "en", "fr" };
            DocumentLanguageSelector.RefreshData();

            StandardDetails.BindStandard(standard);

            SetupDownloadSection(standard);

            if (standard.StandardType == StandardType.Document)
                SetupDocumentSection(standard);

            CancelLink.NavigateUrl = new ReturnUrl().GetReturnUrl()
                ?? $"/ui/admin/standards/edit?id={StandardID}";
        }

        private void SetupDownloadSection(Standard standard)
        {
            FileName.Text = StringHelper.Sanitize($"{standard.StandardType}-{standard.AssetNumber}", '-', false);

            FileTypeSelector.Items.Clear();
            FileTypeSelector.Items.Add(new ListItem("Standard Outline", FileType.Outline));
            FileTypeSelector.Items.Add(new ListItem("Asset Number Hierarchy", FileType.NumberHierarchy));

            if (standard.StandardType == StandardType.Document)
                FileTypeSelector.Items.Add(new ListItem("Document", FileType.Document) { Selected = true });
            else
                FileTypeSelector.SelectedValue = FileType.Outline;

            OnFileTypeChanged();

            if (DocumentSection.Visible)
                DocumentSection.IsSelected = true;
        }

        private void DocumentSettingsManager_NeedReport(object sender, BaseReportManager.NeedReportArgs args)
        {
            Page.Validate("Download");

            args.Cancelled = !Page.IsValid;
            if (args.Cancelled)
                return;

            var settings = new PrintSettings();

            GetDocumentSettings(settings);

            args.Report = settings;
        }

        private void DocumentSettingsManager_ReportSelected(object sender, BaseReportManager.ReportArgs args)
        {
            if (args.Report is PrintSettings settings)
                SetupDocumentSection(settings);
        }

        private void FileTypeSelector_SelectedIndexChanged(object sender, EventArgs e) => OnFileTypeChanged();

        private void OnFileTypeChanged()
        {
            FileFormatSelector.Items.Clear();

            if (FileTypeSelector.SelectedValue == FileType.Outline)
            {
                FileFormatField.Visible = true;
                FileFormatSelector.Items.Add(new ListItem(FileFormat.Json.Text, FileFormat.Json.Value) { Selected = true });
                FileFormatSelector.Items.Add(new ListItem(FileFormat.Markdown.Text, FileFormat.Markdown.Value));
            }
            else if (FileTypeSelector.SelectedValue == FileType.NumberHierarchy)
            {
                FileFormatField.Visible = true;
                FileFormatSelector.Items.Add(new ListItem(FileFormat.Csv.Text, FileFormat.Csv.Value) { Selected = true });
                FileFormatSelector.Items.Add(new ListItem(FileFormat.Excel.Text, FileFormat.Excel.Value));
                FileFormatSelector.Items.Add(new ListItem(FileFormat.Json.Text, FileFormat.Json.Value));
            }
            else if (FileTypeSelector.SelectedValue == FileType.Document)
            {
                FileFormatField.Visible = true;
                FileFormatSelector.Items.Add(new ListItem(FileFormat.Word.Text, FileFormat.Word.Value) { Selected = true });
            }
            else
            {
                FileFormatField.Visible = false;
            }

            OnFileFormatChanged();
        }

        private void FileFormatSelector_SelectedIndexChanged(object sender, EventArgs e) => OnFileFormatChanged();

        private void OnFileFormatChanged()
        {
            ObjectPropertiesField.Visible = false;
            ObjectPropertiesSelector.Items.Clear();

            DocumentSection.Visible = false;
            DocumentScript.Visible = false;

            if (FileTypeSelector.SelectedValue == FileType.Outline && FileFormatSelector.SelectedValue == FileFormat.Json.Value)
            {
                var resolver = new DefaultContractResolver();
                var contract = (JsonObjectContract)resolver.ResolveContract(typeof(StandardModel));

                foreach (var p in contract.Properties.Where(x => !x.Ignored).OrderBy(x => x.PropertyName))
                {
                    var item = new ListItem(
                        p.PropertyName.Humanize(LetterCasing.Title),
                        p.PropertyName)
                    {
                        Selected = true
                    };
                    ObjectPropertiesSelector.Items.Add(item);
                }

                ObjectPropertiesField.Visible = true;
            }
            else if (FileTypeSelector.SelectedValue == FileType.Document && FileFormatSelector.SelectedValue == FileFormat.Word.Value)
            {
                DocumentSection.Visible = true;
                DocumentScript.Visible = true;
            }
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var graph = StandardGraph<StandardGraphNode>.LoadOrganizationEdges(
                Organization.OrganizationIdentifier,
                id => new StandardGraphNode(id));
            if (HasDependencyCycle())
                return;

            var fileType = FileTypeSelector.SelectedValue;
            var fileFormat = FileFormatSelector.SelectedValue;

            if (fileType == FileType.Outline)
            {
                if (fileFormat == FileFormat.Json.Value)
                    SendOutlineJson();
                else if (fileFormat == FileFormat.Markdown.Value)
                    SendOutlineMarkdown();
            }
            else if (fileType == FileType.NumberHierarchy)
            {
                var gRoot = graph.GetNode(StandardID);
                if (gRoot == null)
                    graph.AddNode(gRoot = new StandardGraphNode(StandardID));

                var hRoot = BuildHierarchy(gRoot, LanguageSelection.Value);

                if (FileFormatSelector.SelectedValue == FileFormat.Csv.Value)
                    SendHierarchyCsv(hRoot);
                else if (FileFormatSelector.SelectedValue == FileFormat.Excel.Value)
                    SendHierarchyXlsx(hRoot);
                else if (FileFormatSelector.SelectedValue == FileFormat.Json.Value)
                    SendHierarchyJson(hRoot);
            }
            else if (fileType == FileType.Document)
            {
                if (FileFormatSelector.SelectedValue == FileFormat.Word.Value)
                    SendDocumentWord();
            }
        }

        private void SendOutlineJson()
        {
            var props = ObjectPropertiesSelector.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).Distinct().ToArray();
            if (props.Length == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected properties to import.");
                return;
            }

            var contractResolver = new DownloadContractResolver();
            contractResolver.AddProperties(typeof(StandardModel), props);

            var asset = StandardSearch.SelectModel(StandardID);
            var publication = new StandardPublicationModel(Organization.OrganizationIdentifier, User.FullName, "Download", asset);

            var json = JsonConvert.SerializeObject(publication, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            });
            var data = Encoding.UTF8.GetBytes(json);

            SendFile("json", data);
        }

        private void SendOutlineMarkdown()
        {
            var asset = StandardSearch.SelectModel(StandardID);
            var publication = new StandardPublicationModel(Organization.OrganizationIdentifier, User.FullName, "Download", asset);

            var text = publication.SerializeAsMarkdown();
            var data = Encoding.UTF8.GetBytes(text);

            SendFile("txt", data);
        }

        private void SendHierarchyCsv(HierarchyItem root)
        {
            var helper = new CsvExportHelper(EnumerateFlatten(root));

            helper.AddMapping(nameof(HierarchyItem.DepthIndentAndTitle), "Hierarchy Title");
            helper.AddMapping(nameof(HierarchyItem.Type), "Type");
            helper.AddMapping(nameof(HierarchyItem.Asset), "Asset");
            helper.AddMapping(nameof(HierarchyItem.CodePath), "Code");
            helper.AddMapping(nameof(HierarchyItem.AssetPath), "Path");
            helper.AddMapping(nameof(HierarchyItem.Depth), "Depth");
            helper.AddMapping(nameof(HierarchyItem.Identifier), "Identifier");
            helper.AddMapping(nameof(HierarchyItem.RootIdentifier), "Root");

            string[] labels = new[] { "Title", "Purpose", "Sources", "Contextual Variables", "Bloom's", "Red Seal Occupational Standards Codes" };
            foreach (var label in labels)
                helper.AddMapping(nameof(HierarchyItem.Contents), label, (object[] contents) => GetContent(contents, label));

            var data = helper.GetBytes(Encoding.UTF8);

            SendFile("csv", data);
        }

        private string GetContent(object[] parts, string name)
        {
            if (parts.Length != 1)
                return null;

            if (!(parts[0] is Dictionary<string, string> content))
                return null;

            if (content.ContainsKey(name))
                return content[name];

            return null;
        }

        private void SendHierarchyXlsx(HierarchyItem root)
        {
            var helper = new XlsxExportHelper();

            helper.Map(nameof(HierarchyItem.DepthIndentAndTitle), "Standard Title", 75, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.Type), "Standard Type", 15, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.Asset), "Standard Asset", 15, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.CodePath), "Standard Code Path");
            helper.Map(nameof(HierarchyItem.AssetPath), "Standard Asset Path");
            helper.Map(nameof(HierarchyItem.Depth), "Standard Depth", 15, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.Identifier), "Standard Identifier", 45, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.RootIdentifier), "RootStandard Identifier", 45, HorizontalAlignment.Left);

            var data = helper.GetXlsxBytes(EnumerateFlatten(root), "Sheet 1");

            SendFile("xlsx", data);
        }

        private void SendHierarchyJson(HierarchyItem root)
        {
            var json = JsonConvert.SerializeObject(root, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
            var data = Encoding.UTF8.GetBytes(json);

            SendFile("json", data);
        }

        private void SendFile(string ext, byte[] data)
        {
            if (CompressionMode.Value == "ZIP")
                SendZipFile(data, FileName.Text, ext);
            else
                Response.SendFile(FileName.Text, ext, data);
        }

        private void SetupDocumentSection(Standard standard)
        {
            var sections = SectionInfo.GetDocumentSections(standard.DocumentType);
            if (sections.Length == 0)
            {
                DocumentSectionStatus.AddMessage(
                    AlertType.Error,
                    $"Unexpected document type: " + standard.DocumentType.IfNullOrEmpty("NULL"));
                DocumentSectionContainer.Visible = false;
                return;
            }

            DocumentSectionContainer.Visible = true;

            DocumentLanguageSelector.Value = "en";

            DocumentIsPrintAsChecklist.Enabled = standard.DocumentType == DocumentType.SkillsChecklist
                || standard.DocumentType == DocumentType.CustomizedSkillsChecklist
                || standard.DocumentType == DocumentType.JobDescription;

            DocumentIsBulletedList.Visible = standard.DocumentType == DocumentType.JobDescription;

            DocumentCompetencyFields.Items.Clear();

            var labels = Organization.GetStandardContentLabels();
            foreach (var label in labels)
            {
                if (!string.Equals(label, "Title", StringComparison.OrdinalIgnoreCase))
                    DocumentCompetencyFields.Items.Add(CreateListItem(label, label, true));
            }

            {
                DocumentCompetencyPositionSelector.Items.Clear();
                foreach (var section in sections)
                    DocumentCompetencyPositionSelector.Items.Add(new ComboBoxOption(section.GetTitle(), section.ID));

                DocumentCompetencyPositionSelector.Items.GetOption(DocumentCompetencyPositionSelector.Items.Count - 1).Selected = true;
            }

            {
                DocumentCompetencyDepthFrom.Items.Clear();
                DocumentCompetencyDepthThru.Items.Clear();

                for (var depth = 1; depth <= 5; depth++)
                {
                    DocumentCompetencyDepthFrom.Items.Add(new ComboBoxOption(depth.ToString(), depth.ToString()));
                    DocumentCompetencyDepthThru.Items.Add(new ComboBoxOption(depth.ToString(), depth.ToString()));
                }

                DocumentCompetencyDepthFrom.Items.GetOption(0).Selected = true;
                DocumentCompetencyDepthThru.Items.GetOption(DocumentCompetencyDepthThru.Items.Count - 1).Selected = true;
            }

            var defaultSettings = DocumentSettingsManager.SetDocumentType(standard.DocumentType);
            if (defaultSettings != null)
                SetupDocumentSection(defaultSettings);

            if (standard.DocumentType == DocumentType.JobDescription)
            {
                foreach (ListItem item in DocumentCompetencyFields.Items)
                    item.Selected = false;

                DocumentCompetencyFieldsPanel.Visible = false;
            }

            ListItem CreateListItem(string labelId, string value, bool selected)
            {
                return new ListItem
                {
                    Text = LabelHelper.GetTranslation(labelId),
                    Value = value,
                    Selected = selected
                };
            }
        }

        private void SetupDocumentSection(PrintSettings settings)
        {
            DocumentLanguageSelector.Value = settings.Language;
            DocumentCompetencyPositionSelector.Value = settings.CompetencyPosition;
            DocumentCompetencyDepthFrom.ValueAsInt = settings.CompetencyDepthFrom;
            DocumentCompetencyDepthThru.ValueAsInt = settings.CompetencyDepthThru;
            DocumentFooterText.Text = settings.FooterText;
            SetCheckBoxList(DocumentCompetencyFields, settings.CompetencyFields);
            DocumentIsShowFieldHeading.Checked = settings.IsShowFieldHeading;
            DocumentIsOrderedList.Checked = settings.IsOrderedList;
            DocumentIsBulletedList.Checked = settings.IsBulletedList;
            DocumentIsPrintAsChecklist.Checked = settings.IsPrintAsChecklist;
            DocumentIsRenderPageNumbers.Checked = settings.IsRenderPageNumbers;
            DocumentIsRenderToc.Checked = settings.IsRenderToc;
            DocumentIsRenderPageBreaks.Checked = settings.IsRenderPageBreaks;

            void SetCheckBoxList(CheckBoxList list, string[] values)
            {
                var valuesSet = new HashSet<string>(values);
                foreach (ListItem item in list.Items)
                    item.Selected = valuesSet.Contains(item.Value);
            }
        }

        private void GetDocumentSettings(PrintSettings settings)
        {
            settings.Language = DocumentLanguageSelector.Value;
            settings.CompetencyPosition = DocumentCompetencyPositionSelector.Value;
            settings.CompetencyDepthFrom = DocumentCompetencyDepthFrom.ValueAsInt;
            settings.CompetencyDepthThru = DocumentCompetencyDepthThru.ValueAsInt;
            settings.FooterText = DocumentFooterText.Text;
            settings.CompetencyFields = DocumentCompetencyFields.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => x.Value).ToArray();
            settings.CompetencySettings = new[] { "Tags", "Complexity", "Criticality", "Frequency", "Recurrence", "Difficulty" };
            settings.IsShowFieldHeading = DocumentIsShowFieldHeading.Checked;
            settings.IsOrderedList = DocumentIsOrderedList.Checked;
            settings.IsBulletedList = DocumentIsBulletedList.Checked;
            settings.IsPrintAsChecklist = DocumentIsPrintAsChecklist.Checked;
            settings.IsRenderPageNumbers = DocumentIsRenderPageNumbers.Checked;
            settings.IsRenderToc = DocumentIsRenderToc.Checked;
            settings.IsRenderPageBreaks = DocumentIsRenderPageBreaks.Checked;

            if (settings.CompetencyDepthFrom.HasValue && settings.CompetencyDepthThru.HasValue && settings.CompetencyDepthFrom.Value > settings.CompetencyDepthThru.Value)
                settings.CompetencyDepthFrom = settings.CompetencyDepthThru;
        }

        private void SendDocumentWord()
        {
            var standard = StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardID);
            if (standard == null)
                return;

            var settings = new PrintSettings();

            GetDocumentSettings(settings);

            var content = ServiceLocator.ContentSearch.GetBlock(standard.StandardIdentifier);
            var title = content.Title.Text[settings.Language];

            var data = Utilities.DocumentPrint.CreateDocument(word =>
            {
                if (!string.IsNullOrEmpty(settings.FooterText))
                    Utilities.DocumentPrint.RenderFooterText(word, settings.FooterText);

                if (settings.IsRenderPageNumbers)
                    Utilities.DocumentPrint.RenderFooterNumber(word);

                word.MainDocumentPart.Document.Body.AppendChild(new Paragraph(
                    new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines
                        {
                            After = OxmlHelper.InchesToDxa<int>(0.33).ToString()
                        },
                        ParagraphStyleId = new ParagraphStyleId
                        {
                            Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Title.Id
                        }
                    },
                    new Run(new Text(title))
                ));

                var isChecklist = standard.DocumentType == DocumentType.SkillsChecklist
                    || standard.DocumentType == DocumentType.CustomizedSkillsChecklist;

                if (isChecklist && settings.IsPrintAsChecklist)
                {
                    var htmlConverter = new HtmlConverter(word.MainDocumentPart);

                    var sections = SectionInfo.GetDocumentSections(standard.DocumentType);
                    var isInsertPageBreak = false;

                    foreach (var section in sections)
                    {
                        var value = content[section.ID];
                        if (value.IsEmpty)
                            continue;

                        if (isInsertPageBreak)
                        {
                            word.MainDocumentPart.Document.Body.Append(
                                new Paragraph(
                                    new Run(
                                        new Break { Type = BreakValues.Page }
                                    )
                                )
                            );
                        }
                        else if (settings.IsRenderPageBreaks)
                        {
                            isInsertPageBreak = true;
                        }

                        Utilities.DocumentPrint.AddHeading1(word, section.GetTitle(settings.Language));

                        var html = value.Html[settings.Language];
                        if (string.IsNullOrEmpty(html))
                            html = Markdown.ToHtml(value.Text[settings.Language]);

                        try
                        {
                            htmlConverter.ParseBody(html);
                        }
                        catch (Exception)
                        {
                            htmlConverter.ParseBody("<p style='font-size:30pt; font-weight:bold; color:red;'>UNABLE TO PARSE HTML CODE</p>");
                        }
                    }

                    Utilities.ChecklistRenderer.Render(word, standard.StandardIdentifier, settings.Language);
                }
                else if (settings.IsBulletedList)
                {
                    var htmlConverter = new HtmlConverter(word.MainDocumentPart);

                    var hasContent = false;

                    {
                        var sections = SectionInfo.GetDocumentSections(standard.DocumentType);
                        var isInsertPageBreak = false;

                        foreach (var section in sections)
                        {
                            var value = content[section.ID];
                            if (value == null || value.IsEmpty)
                                continue;

                            if (isInsertPageBreak)
                            {
                                word.MainDocumentPart.Document.Body.Append(
                                    new Paragraph(
                                        new Run(
                                            new Break { Type = BreakValues.Page }
                                        )
                                    )
                                );
                            }
                            else if (settings.IsRenderPageBreaks)
                            {
                                isInsertPageBreak = true;
                            }

                            Utilities.DocumentPrint.AddHeading1(word, section.GetTitle(settings.Language));

                            var html = value.Html[settings.Language];
                            if (string.IsNullOrEmpty(html))
                                html = Markdown.ToHtml(value.Text[settings.Language]);

                            try
                            {
                                htmlConverter.ParseBody(html);
                            }
                            catch (Exception)
                            {
                                htmlConverter.ParseBody("<p style='font-size:30pt; font-weight:bold; color:red;'>UNABLE TO PARSE HTML CODE</p>");
                            }

                            hasContent = true;
                        }
                    }

                    if (hasContent && settings.IsRenderPageBreaks)
                    {
                        word.MainDocumentPart.Document.Body.Append(
                            new Paragraph(
                                new Run(
                                    new Break { Type = BreakValues.Page }
                                )
                            )
                        );
                    }

                    var containments = StandardContainmentSearch
                        .SelectCompetencyContainments(standard.StandardIdentifier, x => x.Child)
                        .Select(x => new Utilities.DocumentPrint.CompetencyModel(x))
                        .OrderBy(x => x.Sequence).ThenBy(x => x.Content.Title.Text[settings.Language]).ThenBy(x => x.Key)
                        .ToList();

                    var competencyTree = LoadCompetencies(StandardID).Where(x => x.StandardType == StandardType.Framework).ToArray();

                    var containemtsLeafs = new List<Utilities.DocumentPrint.CompetencyModel>();

                    foreach (var item in containments)
                    {
                        var competancyItem = competencyTree[0].Descendants()
                        .Where(node => node.StandardIdentifier ==
                        item.Key).FirstOrDefault();
                        if (competancyItem != null)
                        {
                            if (competancyItem.Children.Count == 0)
                                containemtsLeafs.Add(item);
                        }
                        else
                        {
                            var temp = item.Key;
                        }
                    }

                    if (containemtsLeafs.Count > 0)
                    {
                        Utilities.DocumentPrint.AddHeading1(word, "Competencies");
                        Utilities.DocumentPrint.RenderBulletedList(word, containemtsLeafs.ToArray(), settings.Language);
                    }
                }
                else if (settings.IsPrintAsChecklist)
                {
                    var context = new Utilities.DocumentPrint.RenderContext(word)
                    {
                        Fields = new HashSet<string>(settings.CompetencyFields, StringComparer.OrdinalIgnoreCase),
                        Settings = new HashSet<string>(settings.CompetencySettings, StringComparer.OrdinalIgnoreCase),
                        ShowFieldHeading = settings.IsShowFieldHeading,
                        Language = settings.Language,
                        Level = 0
                    };

                    if (settings.IsRenderToc)
                        Utilities.DocumentPrint.CreateTocContainer(
                            word,
                            LabelHelper.GetTranslation("Standards.Documents.Headings.TOC.Title", settings.Language));

                    var sectionSearchState = 0;
                    var sections = SectionInfo.GetDocumentSections(standard.DocumentType);
                    var isInsertPageBreak = false;

                    foreach (var section in sections)
                    {
                        if (sectionSearchState == 0 && section.ID == settings.CompetencyPosition)
                            sectionSearchState = 1;

                        var value = content[section.ID];
                        if (value.IsEmpty)
                            continue;

                        if (isInsertPageBreak)
                        {
                            word.MainDocumentPart.Document.Body.Append(
                                new Paragraph(
                                    new Run(
                                        new Break { Type = BreakValues.Page }
                                    )
                                )
                            );
                        }
                        else if (settings.IsRenderPageBreaks)
                        {
                            isInsertPageBreak = true;
                        }

                        var headingTitle = section.GetTitle(settings.Language);
                        var pHeader = Utilities.DocumentPrint.AddHeading1(word, headingTitle);
                        if (settings.IsRenderToc)
                            Utilities.DocumentPrint.AddTocItem(context, pHeader, headingTitle);

                        var html = value.Html[settings.Language];
                        if (string.IsNullOrEmpty(html))
                            html = Markdown.ToHtml(value.Text[settings.Language]);

                        try
                        {
                            context.HtmlConverter.ParseBody(html);
                        }
                        catch (Exception)
                        {
                            context.HtmlConverter.ParseBody("<p style='font-size:30pt; font-weight:bold; color:red;'>UNABLE TO PARSE HTML CODE</p>");
                        }

                        if (sectionSearchState == 1)
                        {
                            RenderChildsOnly(context, settings, standard);
                            sectionSearchState = 2;
                        }
                    }

                    if (sectionSearchState <= 1)
                        RenderChildsOnly(context, settings, standard);
                }
                else
                {
                    var context = new Utilities.DocumentPrint.RenderContext(word)
                    {
                        Fields = new HashSet<string>(settings.CompetencyFields, StringComparer.OrdinalIgnoreCase),
                        Settings = new HashSet<string>(settings.CompetencySettings, StringComparer.OrdinalIgnoreCase),
                        ShowFieldHeading = settings.IsShowFieldHeading,
                        Language = settings.Language,
                        Level = 0
                    };

                    if (settings.IsRenderToc)
                        Utilities.DocumentPrint.CreateTocContainer(
                            word,
                            LabelHelper.GetTranslation("Standards.Documents.Headings.TOC.Title", settings.Language));

                    var sectionSearchState = 0;
                    var sections = SectionInfo.GetDocumentSections(standard.DocumentType);
                    var isInsertPageBreak = false;

                    foreach (var section in sections)
                    {
                        if (sectionSearchState == 0 && section.ID == settings.CompetencyPosition)
                            sectionSearchState = 1;

                        var value = content[section.ID];
                        if (value.IsEmpty)
                            continue;

                        if (isInsertPageBreak)
                        {
                            word.MainDocumentPart.Document.Body.Append(
                                new Paragraph(
                                    new Run(
                                        new Break { Type = BreakValues.Page }
                                    )
                                )
                            );
                        }
                        else if (settings.IsRenderPageBreaks)
                        {
                            isInsertPageBreak = true;
                        }

                        var headingTitle = section.GetTitle(settings.Language);
                        var pHeader = Utilities.DocumentPrint.AddHeading1(word, headingTitle);
                        if (settings.IsRenderToc)
                            Utilities.DocumentPrint.AddTocItem(context, pHeader, headingTitle);

                        var html = value.Html[settings.Language];
                        if (string.IsNullOrEmpty(html))
                            html = Markdown.ToHtml(value.Text[settings.Language]);

                        try
                        {
                            context.HtmlConverter.ParseBody(html);
                        }
                        catch (Exception)
                        {
                            context.HtmlConverter.ParseBody("<p style='font-size:30pt; font-weight:bold; color:red;'>UNABLE TO PARSE HTML CODE</p>");
                        }

                        if (sectionSearchState == 1)
                        {
                            Render(context, settings, standard);
                            sectionSearchState = 2;
                        }
                    }

                    if (sectionSearchState <= 1)
                        Render(context, settings, standard);
                }

                word.PackageProperties.Title = title;
            });

            SendFile("docx", data);
        }

        private void RenderChildsOnly(Utilities.DocumentPrint.RenderContext context, PrintSettings settings, Standard standard)
        {
            if (settings.IsOrderedList)
                context.CompetencyNumberingID = Utilities.DocumentPrint.DefineStandardsDocumentOrderedList(context);

            var frameworks = Utilities.DocumentPrint.StandardModel.LoadTree(standard.StandardIdentifier, settings.Language, null);

            RenderFrameworkHeaders(context, settings, frameworks, 0);
        }

        private void RenderFrameworkHeaders(
            Utilities.DocumentPrint.RenderContext context,
            PrintSettings settings,
            IEnumerable<Utilities.DocumentPrint.StandardModel> standards,
            int level)
        {
            var contextLevel = level - (settings.CompetencyDepthFrom ?? 1) + 1;

            List<Utilities.DocumentPrint.StandardModel> toPrint = new List<Utilities.DocumentPrint.StandardModel>();
            string FrameworkName = string.Empty;
            Guid FrameworkId = Guid.Empty;

            foreach (var standard in standards)
            {
                context.Level = contextLevel;

                if (contextLevel == 0 && standard.StandardType == "Framework")
                {
                    if (FrameworkId == Guid.Empty &&
                        string.IsNullOrEmpty(FrameworkName) &&
                        toPrint.Count == 0)
                    {
                        ResetFrameworkToPrintValues(out toPrint, out FrameworkName, out FrameworkId, standard);

                        var pHeader = settings.IsOrderedList
                            ? Utilities.DocumentPrint.AddOrderedListHeader(context, standard.Title)
                            : Utilities.DocumentPrint.AddFieldHeader(context, standard.Title);

                        if (settings.IsRenderToc && context.Level >= 0 && context.Level <= 2)
                            Utilities.DocumentPrint.AddTocItem(context, pHeader, standard.Title);
                    }
                    else
                    {
                        PrintCompetenciesList(context, settings, toPrint);
                        ResetFrameworkToPrintValues(out toPrint, out FrameworkName, out FrameworkId, standard);
                    }
                }

                if (standard.Children.Count > 0 && level < settings.CompetencyDepthThru)
                    RenderChildsOnly(context, settings, standard.Children, toPrint, level + 1);
            }

            PrintCompetenciesList(context, settings, toPrint);
        }

        private static void ResetFrameworkToPrintValues(
            out List<Utilities.DocumentPrint.StandardModel> toPrint,
            out string FrameworkName, out Guid FrameworkId,
            Utilities.DocumentPrint.StandardModel standard)
        {
            FrameworkId = standard.StandardIdentifier;
            FrameworkName = standard.Title;
            toPrint = new List<Utilities.DocumentPrint.StandardModel>();
        }

        private static void PrintCompetenciesList(Utilities.DocumentPrint.RenderContext context,
            PrintSettings settings, List<Utilities.DocumentPrint.StandardModel> toPrint)
        {
            foreach (var standard in toPrint.OrderBy(x => x.Title).ToList())
            {
                context.Level = 1;
                var pHeader = settings.IsOrderedList
                    ? Utilities.DocumentPrint.AddOrderedListHeader(context, standard.Title)
                    : Utilities.DocumentPrint.AddFieldHeader(context, standard.Title);

                if (settings.IsRenderToc && context.Level >= 0 && context.Level <= 2)
                    Utilities.DocumentPrint.AddTocItem(context, pHeader, standard.Title);

                if (standard.StandardType == StandardType.Competency)
                {
                    context.Level = 0;

                    standard.RenderFields(context);
                    standard.RenderSettings(context);
                }
            }
        }

        private void RenderChildsOnly(
            Utilities.DocumentPrint.RenderContext context,
            PrintSettings settings,
            IEnumerable<Utilities.DocumentPrint.StandardModel> standards,
            List<Utilities.DocumentPrint.StandardModel> toPrint,
            int level)
        {
            var contextLevel = level - (settings.CompetencyDepthFrom ?? 1) + 1;

            foreach (var standard in standards)
            {
                context.Level = contextLevel;

                if (!settings.CompetencyDepthFrom.HasValue || level >= settings.CompetencyDepthFrom.Value - 1)
                {
                    if (standard.Children.Count == 0)
                        toPrint.Add(standard);
                }

                if (standard.Children.Count > 0 && level < settings.CompetencyDepthThru)
                    RenderChildsOnly(context, settings, standard.Children, toPrint, level + 1);
            }
        }

        private void Render(Utilities.DocumentPrint.RenderContext context, PrintSettings settings, Standard standard)
        {
            if (settings.IsOrderedList)
                context.CompetencyNumberingID = Utilities.DocumentPrint.DefineStandardsDocumentOrderedList(context);

            var frameworks = Utilities.DocumentPrint.StandardModel.LoadTree(standard.StandardIdentifier, settings.Language, null);

            Render(context, settings, frameworks, 0);
        }

        private void Render(
            Utilities.DocumentPrint.RenderContext context,
            PrintSettings settings,
            IEnumerable<Utilities.DocumentPrint.StandardModel> standards,
            int level)
        {
            var contextLevel = level - (settings.CompetencyDepthFrom ?? 1) + 1;

            foreach (var standard in standards)
            {
                context.Level = contextLevel;

                if (!settings.CompetencyDepthFrom.HasValue || level >= settings.CompetencyDepthFrom.Value - 1)
                {
                    var pHeader = settings.IsOrderedList
                        ? Utilities.DocumentPrint.AddOrderedListHeader(context, standard.Title)
                        : Utilities.DocumentPrint.AddFieldHeader(context, standard.Title);

                    if (settings.IsRenderToc && context.Level >= 0 && context.Level <= 2)
                        Utilities.DocumentPrint.AddTocItem(context, pHeader, standard.Title);

                    if (standard.StandardType == StandardType.Competency)
                    {
                        context.Level = 0;

                        standard.RenderFields(context);
                        standard.RenderSettings(context);
                    }
                }

                if (standard.Children.Count > 0 && level < settings.CompetencyDepthThru)
                    Render(context, settings, standard.Children, level + 1);
            }
        }

        private static List<StandardInfo> LoadCompetencies(Guid standardKey)
        {
            var data = StandardContainmentSearch.Bind(
                LinqExtensions1.Expr((StandardContainment x) => StandardInfo.Binder.Invoke(x.Child)).Expand(),
                x => x.ParentStandardIdentifier == standardKey
                  && x.Child.StandardType == StandardType.Competency);

            var accumulator = new Dictionary<Guid, StandardInfo>();
            foreach (var info in data)
                accumulator.Add(info.StandardIdentifier, info);

            LoadCompetencies(accumulator, data);

            var mapping = accumulator.Values
                .Where(x => x.ParentStandardIdentifier.HasValue)
                .GroupBy(x => x.ParentStandardIdentifier.Value)
                .ToDictionary(x => x.Key, x => x.AsQueryable().OrderBy(y => y.Sequence));

            IEnumerable<StandardInfo> topLevel = accumulator.Values
                .Where(x => x.StandardType == StandardType.Framework)
                .OrderBy(x => x.Title)
                .ToArray();
            IEnumerable<StandardInfo> prevLevel = topLevel;

            while (true)
            {
                var level = new List<StandardInfo>();

                foreach (var pInfo in prevLevel)
                {
                    if (!mapping.TryGetValue(pInfo.StandardIdentifier, out var children))
                        continue;

                    foreach (var cInfo in children)
                    {
                        var isCompetency = cInfo.StandardType == StandardType.Competency;
                        var isArea = cInfo.StandardType == StandardType.Area;
                        if (!isCompetency && !isArea || !isCompetency && pInfo.StandardType == StandardType.Competency)
                            continue;

                        cInfo.Parent = pInfo;
                        pInfo.Children.Add(cInfo);
                        level.Add(cInfo);
                    }
                }

                if (level.Count == 0)
                    break;

                prevLevel = level;
            }

            return topLevel.ToList();
        }

        private static void LoadCompetencies(Dictionary<Guid, StandardInfo> accumulator, IEnumerable<StandardInfo> children)
        {
            var typeFilter = new[] { StandardType.Framework, StandardType.Area, StandardType.Competency };
            var keyFilter = children
                .Where(x => x.ParentStandardIdentifier.HasValue && !accumulator.ContainsKey(x.ParentStandardIdentifier.Value))
                .Select(x => x.ParentStandardIdentifier.Value)
                .Distinct()
                .ToArray();

            if (keyFilter.Length == 0)
                return;

            var data = StandardSearch.Bind(
                LinqExtensions1.Expr((Standard x) => StandardInfo.Binder.Invoke(x)).Expand(),
                x => keyFilter.Contains(x.StandardIdentifier) && typeFilter.Contains(x.StandardType));

            if (data.Length == 0)
                return;

            foreach (var info in data)
                accumulator.Add(info.StandardIdentifier, info);

            LoadCompetencies(accumulator, data);
        }

        private static HierarchyItem BuildHierarchy(StandardGraphNode root, string language)
        {
            var result = new HierarchyItem();
            var mapping = new Dictionary<Guid, HierarchyItem> { { root.NodeId, result } };

            BuildHierarchy(result, root.OutgoingEdges.Select(x => x.ToNode), mapping);

            var dataItems = StandardSearch.Bind(x => new HierarchyDataItem
            {
                StandardIdentifier = x.StandardIdentifier,
                StandardType = x.StandardType,
                ContentTitle = x.ContentTitle,
                AssetNumber = x.AssetNumber,
                Code = x.Code,
                Hook = x.StandardHook
            }, x => mapping.Keys.Contains(x.StandardIdentifier), null, null);

            foreach (var dItem in dataItems)
            {
                dItem.Contents = ServiceLocator.ContentSearch
                    .SelectContainerByLanguage(dItem.StandardIdentifier, language)
                    .ToDictionary(x => x.ContentLabel, x => x.ContentText);

                mapping[dItem.StandardIdentifier].SetDataItem(dItem);
            }

            return result;
        }

        private static void BuildHierarchy(HierarchyItem parent, IEnumerable<StandardGraphNode> downstream, Dictionary<Guid, HierarchyItem> mapping)
        {
            foreach (var dNode in downstream)
            {
                if (mapping.ContainsKey(dNode.NodeId))
                    continue;

                var dItem = new HierarchyItem();

                mapping.Add(dNode.NodeId, dItem);
                parent.Add(dItem);

                BuildHierarchy(dItem, dNode.OutgoingEdges.Select(x => x.ToNode), mapping);
            }
        }

        private static IEnumerable<HierarchyItem> EnumerateFlatten(HierarchyItem item)
        {
            yield return item;

            foreach (var child in item.Children)
                foreach (var fi in EnumerateFlatten(child))
                    yield return fi;
        }

        private bool HasDependencyCycle()
        {
            var graph = StandardGraph<StandardGraphNode>.LoadOrganizationEdges(
                Organization.OrganizationIdentifier,
                id => new StandardGraphNode(id));

            return HasDependencyCycle(graph);
        }

        private bool HasDependencyCycle(StandardGraph<StandardGraphNode> graph)
        {
            var roots = graph.GetNode(StandardID)?.GetRootNodes();
            if (roots != null)
            {
                foreach (var node in roots)
                {
                    if (node != null)
                    {
                        var cyclePaths = node.FindCycles();
                        if (cyclePaths.Length > 0)
                        {
                            var message = StandardGraphHelper.BuildDependencyCycleHtmlErrorMessage(node.NodeId, cyclePaths);
                            ScreenStatus.AddMessage(AlertType.Error, message);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? "asset=" + StandardID
                : null;
        }

    }
}
