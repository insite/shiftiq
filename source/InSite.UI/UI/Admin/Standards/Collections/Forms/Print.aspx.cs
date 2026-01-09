using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using DocumentFormat.OpenXml.Wordprocessing;

using InSite.Admin.Standards.Standards.Utilities;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Admin.Standards.Collections.Forms
{
    public partial class Print : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid StandardID => Guid.TryParse(Request.QueryString["asset"], out var value) ? value : Guid.Empty;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PrintButton.Click += PrintButton_Click;

            SettingsManager.NeedReport += SettingsManager_NeedReport;
            SettingsManager.ReportSelected += SettingsManager_ReportSelected;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void PrintButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var standard = StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardID);
            if (standard == null)
                RedirectToSearch();

            var settings = new PrintSettings();

            GetInputValues(settings);

            var title = ServiceLocator.ContentSearch
                .SelectContainer(standard.StandardIdentifier, ContentLabel.Title, settings.Language)?.ContentText;

            var data = DocumentPrint.CreateDocument(word =>
            {
                if (!string.IsNullOrEmpty(settings.FooterText))
                    DocumentPrint.RenderFooterText(word, settings.FooterText);

                if (settings.IsRenderPageNumbers)
                    DocumentPrint.RenderFooterNumber(word);

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

                if (settings.IsPrintAsChecklist)
                {
                    ChecklistRenderer.Render(word, standard.StandardIdentifier, settings.Language);
                }
                else if (settings.IsBulletedList)
                {
                    var containments = StandardContainmentSearch
                        .SelectCompetencyContainments(standard.StandardIdentifier, x => x.Child.Parent)
                        .Where(x => x.Parent != null)
                        .Select(x => new DocumentPrint.CompetencyModel(x))
                        .OrderBy(x => x.Sequence).ThenBy(x => x.Content.Title.Text[settings.Language]).ThenBy(x => x.Key)
                        .ToArray();

                    if (containments.Length > 0)
                    {
                        DocumentPrint.AddHeading1(word, "Competencies");
                        DocumentPrint.RenderBulletedList(word, containments, settings.Language);
                    }
                }
                else
                {
                    var context = new DocumentPrint.RenderContext(word)
                    {
                        Fields = new HashSet<string>(settings.CompetencyFields, StringComparer.OrdinalIgnoreCase),
                        Settings = new HashSet<string>(settings.CompetencySettings, StringComparer.OrdinalIgnoreCase),
                        ShowFieldHeading = settings.IsShowFieldHeading,
                        Language = settings.Language,
                        Level = 0
                    };

                    if (settings.IsRenderToc)
                        DocumentPrint.CreateTocContainer(
                            word,
                            LabelHelper.GetTranslation("Standards.Documents.Headings.TOC.Title", settings.Language));

                    Render(context, settings, standard);
                }

                word.PackageProperties.Title = title;
            });

            var fileName = StringHelper.Sanitize(title, '-') + ".docx";
            const string mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            Response.SendFile(fileName, data, mimeType);
        }

        private void SettingsManager_NeedReport(object sender, BaseReportManager.NeedReportArgs args)
        {
            Page.Validate("Documents");

            args.Cancelled = !Page.IsValid;
            if (args.Cancelled)
                return;

            var settings = new PrintSettings();

            GetInputValues(settings);

            args.Report = settings;
        }

        private void SettingsManager_ReportSelected(object sender, BaseReportManager.ReportArgs args)
        {
            if (args.Report is PrintSettings settings)
                SetInputValues(settings);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var standard = StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardID);
            if (standard == null)
                RedirectToSearch();

            SetInputValues(standard);
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Standard standard)
        {
            var title = $"{standard.ContentTitle ?? standard.ContentName ?? "Untitled"} " +
                $"<span class='form-text'>{standard.StandardType} Asset #{standard.AssetNumber}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            CompetencyFields.Items.Clear();

            var labels = Organization.GetStandardContentLabels();
            foreach (var label in labels)
                CompetencyFields.Items.Add(CreateListItem(label, label, true));

            {
                CompetencyDepthFrom.Items.Clear();
                CompetencyDepthThru.Items.Clear();

                for (var depth = 1; depth <= 5; depth++)
                {
                    CompetencyDepthFrom.Items.Add(new ComboBoxOption(depth.ToString(), depth.ToString()));
                    CompetencyDepthThru.Items.Add(new ComboBoxOption(depth.ToString(), depth.ToString()));
                }

                CompetencyDepthFrom.Items.GetOption(0).Selected = true;
                CompetencyDepthThru.Items.GetOption(CompetencyDepthThru.Items.Count - 1).Selected = true;
            }

            CancelLink.NavigateUrl = GetReaderUrl();

            System.Web.UI.WebControls.ListItem CreateListItem(string labelId, string value, bool selected)
            {
                return new System.Web.UI.WebControls.ListItem
                {
                    Text = LabelHelper.GetTranslation(labelId),
                    Value = value,
                    Selected = selected
                };
            }
        }

        private void GetInputValues(PrintSettings settings)
        {
            settings.CompetencyDepthFrom = CompetencyDepthFrom.ValueAsInt;
            settings.CompetencyDepthThru = CompetencyDepthThru.ValueAsInt;
            settings.FooterText = FooterText.Text;
            settings.CompetencyFields = CompetencyFields.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => x.Value).ToArray();
            settings.CompetencySettings = CompetencySettings.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => x.Value).ToArray();
            settings.IsShowFieldHeading = IsShowFieldHeading.Checked;
            settings.IsOrderedList = IsOrderedList.Checked;
            settings.IsBulletedList = IsBulletedList.Checked;
            settings.IsPrintAsChecklist = IsPrintAsChecklist.Checked;
            settings.IsRenderPageNumbers = IsRenderPageNumbers.Checked;
            settings.IsRenderToc = IsRenderToc.Checked;
            settings.IsRenderPageBreaks = IsRenderPageBreaks.Checked;

            if (settings.CompetencyDepthFrom.HasValue && settings.CompetencyDepthThru.HasValue && settings.CompetencyDepthFrom.Value > settings.CompetencyDepthThru.Value)
                settings.CompetencyDepthFrom = settings.CompetencyDepthThru;
        }

        private void SetInputValues(PrintSettings settings)
        {
            CompetencyDepthFrom.ValueAsInt = settings.CompetencyDepthFrom;
            CompetencyDepthThru.ValueAsInt = settings.CompetencyDepthThru;
            FooterText.Text = settings.FooterText;
            SetCheckBoxList(CompetencyFields, settings.CompetencyFields);
            SetCheckBoxList(CompetencySettings, settings.CompetencySettings);
            IsShowFieldHeading.Checked = settings.IsShowFieldHeading;
            IsOrderedList.Checked = settings.IsOrderedList;
            IsBulletedList.Checked = settings.IsBulletedList;
            IsPrintAsChecklist.Checked = settings.IsPrintAsChecklist;
            IsRenderPageNumbers.Checked = settings.IsRenderPageNumbers;
            IsRenderToc.Checked = settings.IsRenderToc;
            IsRenderPageBreaks.Checked = settings.IsRenderPageBreaks;

            void SetCheckBoxList(System.Web.UI.WebControls.CheckBoxList list, string[] values)
            {
                var valuesSet = new HashSet<string>(values);
                foreach (System.Web.UI.WebControls.ListItem item in list.Items)
                    item.Selected = valuesSet.Contains(item.Value);
            }
        }
        #endregion

        #region Methods (rendering)

        private void Render(DocumentPrint.RenderContext context, PrintSettings settings, Standard standard)
        {
            if (settings.IsOrderedList)
                context.CompetencyNumberingID = DocumentPrint.DefineStandardsDocumentOrderedList(context);

            var frameworks = DocumentPrint.StandardModel.LoadTree(standard.StandardIdentifier, "en", null);

            Render(context, settings, frameworks, 0);
        }

        private void Render(DocumentPrint.RenderContext context, PrintSettings settings, IEnumerable<DocumentPrint.StandardModel> standards, int level)
        {
            context.Level = level - (settings.CompetencyDepthFrom ?? 1) + 1;

            foreach (var standard in standards)
            {
                if (!settings.CompetencyDepthFrom.HasValue || level >= settings.CompetencyDepthFrom.Value - 1)
                {
                    var pHeader = settings.IsOrderedList
                        ? DocumentPrint.AddOrderedListHeader(context, standard.Title)
                        : DocumentPrint.AddFieldHeader(context, standard.Title);

                    if (settings.IsRenderToc && context.Level >= 0 && context.Level <= 2)
                        DocumentPrint.AddTocItem(context, pHeader, standard.Title);

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

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/standards/collections/search", true);

        private string GetReaderUrl()
        {
            var url = $"/ui/admin/standards/collections/outline?asset={StandardID}";

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"asset={StandardID}"
                : null;
        }

        #endregion
    }
}