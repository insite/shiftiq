using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.Domain.Organizations;
using InSite.UI.Portal.Assessments.Attempts.Utilities;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using IQuestionInfo = InSite.Admin.Assessments.Questions.Utilities.QuestionPrintHelper.IQuestionInfo;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPrintExternal : UserControl
    {
        #region Classes

        private class ControlData
        {
            public string AssetNumber { get; }
            public string Title { get; }
            public string Name { get; }
            public string Introduction { get; }
            public string Description { get; }
            public IQuestionInfo[] Questions { get; private set; }

            public ControlData(BankState bank)
            {
                AssetNumber = bank.Asset.ToString();
                Title = (bank.Content?.Title?.Default).IfNullOrEmpty(bank.Name);
                Name = bank.Name;
                Questions = QuestionPrintHelper.GetQuestions(bank);
            }

            public ControlData(Form form)
            {
                AssetNumber = $"{form.Code}{form.Asset}0{form.AssetVersion}";
                Title = (form.Content.Title?.Default).IfNullOrEmpty(form.Name);
                Name = form.Name;
                Introduction = form.Content.InstructionsForOnline?.Default;
                Description = form.Content.InstructionsForPaper?.Default;
                Questions = QuestionPrintHelper.GetQuestions(form);
            }
        }

        public class BankOptions : Options
        {
            public Guid BankID { get; }

            public BankOptions(OrganizationState organization, Guid bankId, bool includeImages)
                : base(organization, includeImages)
            {
                BankID = bankId;
            }
        }

        public class FormOptions : Options
        {
            public Guid FormID { get; }

            public FormOptions(OrganizationState organization, Guid formId, bool includeImages)
                : base(organization, includeImages)
            {
                FormID = formId;
            }
        }

        public class Options
        {
            public Guid OrganizationID { get; }

            public string CurrentUrl { get; }
            public string HeaderUrl { get; }
            public string FooterUrl { get; }

            public bool IncludeImages { get; }

            public Options(OrganizationState organization, bool includeImages)
            {
                OrganizationID = organization.Identifier;

                var request = HttpContext.Current.Request;

                CurrentUrl = request.Url.Scheme + "://" + request.Url.Host + request.RawUrl;
                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Assessments/Questions/Html/PrintExternalHeader.html");
                FooterUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Assessments/Questions/Html/PrintExternalFooter.html");

                IncludeImages = includeImages;
            }
        }

        #endregion

        #region Fields

        private Guid? _prevSectionId = null;
        private QuestionTable _questionTable = null;

        #endregion

        #region Event handlers

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var info = (IQuestionInfo)e.Item.DataItem;

            BindQuestionSection(e.Item, info);

            var bQuestion = info.BankQuestion;
            var aQuestion = info.AttemptQuestion;
            var aQuestionDefault = info.AttemptQuestion as AttemptQuestionDefault;

            _questionTable = aQuestionDefault != null && bQuestion.Layout.Type == OptionLayoutType.Table
                ? QuestionTable.Build(bQuestion.Layout.Columns, aQuestionDefault.Options.Select(x => x.Text))
                : null;

            if (aQuestion.Type == QuestionItemType.SingleCorrect)
                BindSingleCorrect(e.Item, aQuestionDefault);

            else if (aQuestion.Type == QuestionItemType.TrueOrFalse)
                BindTrueOrFalse(e.Item, aQuestionDefault);

            else if (aQuestion.Type == QuestionItemType.MultipleCorrect)
                BindMultipleCorrect(e.Item, aQuestionDefault);

            else if (aQuestion.Type.IsComposed())
                BindComposed(e.Item, (AttemptQuestionComposed)aQuestion);

            else if (aQuestion.Type == QuestionItemType.BooleanTable)
                BindBooleanTable(e.Item, aQuestionDefault);

            else if (aQuestion.Type == QuestionItemType.Matching)
                BindMatching(e.Item, (AttemptQuestionMatch)aQuestion);

            else if (aQuestion.Type == QuestionItemType.Likert)
                BindLikert(e.Item, (AttemptQuestionLikert)aQuestion);

            else if (aQuestion.Type.IsHotspot())
                BindHotspot(e.Item, (AttemptQuestionHotspot)aQuestion);

            else if (aQuestion.Type == QuestionItemType.Ordering)
                BindOrdering(e.Item, (AttemptQuestionOrdering)aQuestion);
        }

        private void BindQuestionSection(RepeaterItem item, IQuestionInfo info)
        {
            if (info.Section == null || _prevSectionId.HasValue && _prevSectionId == info.Section.Identifier)
                return;

            _prevSectionId = info.Section.Identifier;

            var title = info.Section.Content.Title?.Default;
            var summary = info.Section.Content.Summary?.Default;

            var hasTitle = title.IsNotEmpty();
            var hasSummary = summary.IsNotEmpty();

            var sectionContainer = (HtmlGenericControl)item.FindControl("SectionContainer");
            sectionContainer.Visible = hasSummary || hasTitle;

            if (hasTitle)
            {
                var output = (HtmlGenericControl)item.FindControl("SectionTitle");
                output.InnerText = title;
                output.Visible = true;
            }

            if (hasSummary)
            {
                var output = (HtmlGenericControl)item.FindControl("SectionSummary");
                output.InnerHtml = Markdown.ToHtml(summary);
                output.Visible = true;
            }
        }

        private void SetActiveView(RepeaterItem item, string viewId)
        {
            var multiView = (MultiView)item.FindControl("ItemsMultiView");
            var view = (View)item.FindControl(viewId);

            multiView.SetActiveView(view);
        }

        private void BindSingleCorrect(RepeaterItem item, AttemptQuestionDefault question)
        {
            SetActiveView(item, "SingleCorrectItemsView");

            var repeater = (Repeater)item.FindControl("SingleCorrectItemRepeater");
            repeater.DataSource = question.Options;
            repeater.DataBind();
        }

        private void BindTrueOrFalse(RepeaterItem item, AttemptQuestionDefault question)
        {
            SetActiveView(item, "TrueOrFalseItemsView");

            var repeater = (Repeater)item.FindControl("TrueOrFalseItemRepeater");
            repeater.DataSource = question.Options;
            repeater.DataBind();
        }

        private void BindMultipleCorrect(RepeaterItem item, AttemptQuestionDefault question)
        {
            SetActiveView(item, "MultipleCorrectItemsView");

            var repeater = (Repeater)item.FindControl("MultipleCorrectItemRepeater");
            repeater.DataSource = question.Options;
            repeater.DataBind();
        }

        private void BindComposed(RepeaterItem item, AttemptQuestionComposed question)
        {
            SetActiveView(item, "ComposedItemsView");
        }

        private void BindBooleanTable(RepeaterItem item, AttemptQuestionDefault question)
        {
            SetActiveView(item, "BooleanTableItemsView");

            var repeater = (Repeater)item.FindControl("BooleanTableItemRepeater");
            repeater.DataSource = question.Options;
            repeater.DataBind();
        }

        private void BindMatching(RepeaterItem item, AttemptQuestionMatch question)
        {
            SetActiveView(item, "MatchesItemsView");

            var matchOptions = question.Pairs.Select(x => x.RightText).Concat(question.Distractors).Distinct().ToArray();
            matchOptions.Shuffle();

            var leftRepeater = (Repeater)item.FindControl("MatchesLeftRepeater");
            leftRepeater.DataSource = question.Pairs;
            leftRepeater.DataBind();

            var rightRepeater = (Repeater)item.FindControl("MatchesRightRepeater");
            rightRepeater.DataSource = matchOptions;
            rightRepeater.DataBind();
        }

        private void BindLikert(RepeaterItem item, AttemptQuestionLikert question)
        {
            SetActiveView(item, "LikertItemsView");

            var columnRepeater = (Repeater)item.FindControl("LikertColumnRepeater");
            columnRepeater.DataSource = question.Questions[0].Options.Select(x => x.Text);
            columnRepeater.DataBind();

            var rowRepeater = (Repeater)item.FindControl("LikertRowRepeater");
            rowRepeater.ItemDataBound += LikertRowRepeater_ItemDataBound;
            rowRepeater.DataSource = question.Questions;
            rowRepeater.DataBind();
        }

        private void BindHotspot(RepeaterItem item, AttemptQuestionHotspot question)
        {
            SetActiveView(item, "HotspotItemsView");

            var data = HotspotImage.FromString(question.Image);

            var image = (ITextControl)item.FindControl("HotspotImage");
            image.Text = $"<img alt='' class='hotspot-image' src='{data.Url}' />";
        }

        private void BindOrdering(RepeaterItem item, AttemptQuestionOrdering question)
        {
            SetActiveView(item, "OrderingItemsView");

            if (question.TopLabel.IsNotEmpty())
            {
                var orderingTopLabel = (HtmlGenericControl)item.FindControl("OrderingTopLabel");
                orderingTopLabel.Visible = true;
                orderingTopLabel.InnerHtml = Markdown.ToHtml(question.TopLabel);
            }

            if (question.BottomLabel.IsNotEmpty())
            {
                var orderingBottomLabel = (HtmlGenericControl)item.FindControl("OrderingBottomLabel");
                orderingBottomLabel.Visible = true;
                orderingBottomLabel.InnerHtml = Markdown.ToHtml(question.BottomLabel);
            }

            var optionRepeater = (Repeater)item.FindControl("OrderingOptionRepeater");
            optionRepeater.DataSource = question.Options;
            optionRepeater.DataBind();
        }

        private void LikertRowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var repeater = (Repeater)e.Item.FindControl("LikertOptionRepeater");
            repeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Options");
            repeater.DataBind();
        }

        #endregion

        private void LoadData(ControlData data, bool includeImages)
        {
            ExcludeImagesStyle.Visible = !includeImages;

            PageTitle.InnerText = data.Title.IfNullOrEmpty("Untitled");
            FormTitle.InnerText = data.Title.IfNullOrEmpty("Untitled");
            FormDescription.Visible = data.Description.IsNotEmpty();
            FormDescription.InnerHtml = Markdown.ToHtml(data.Description);

            var hasData = data.Questions.Length > 0;

            NoDataMessage.Visible = !hasData;
            FormContainer.Visible = hasData;

            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
            QuestionRepeater.DataSource = data.Questions;
            QuestionRepeater.DataBind();
        }

        #region Methods (render PDF)

        public static PrintOutputFile RenderPdf(BankOptions options)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(options.BankID);
            if (bank == null || bank.Tenant != options.OrganizationID)
                return null;

            return RenderPdf(new ControlData(bank), options);
        }

        public static PrintOutputFile RenderPdf(FormOptions options)
        {
            var form = ServiceLocator.BankSearch.GetFormData(options.FormID);
            if (form == null || form.Specification.Bank.Tenant != options.OrganizationID)
                return null;

            return RenderPdf(new ControlData(form), options);
        }

        private static PrintOutputFile RenderPdf(ControlData data, Options options)
        {
            using (var page = new Page())
            {
                page.EnableEventValidation = false;
                page.EnableViewState = false;

                var report = (QuestionPrintExternal)page.LoadControl("~/UI/Admin/Assessments/Questions/Controls/QuestionPrintExternal.ascx");

                report.LoadData(data, options.IncludeImages);

                var htmlBuilder = new StringBuilder();

                using (var writer = new StringWriter(htmlBuilder))
                {
                    using (var htmlWriter = new HtmlTextWriter(writer))
                        report.RenderControl(htmlWriter);
                }

                var htmlString = HtmlHelper.ResolveRelativePaths(options.CurrentUrl, htmlBuilder);

                var fileName = $"{StringHelper.Sanitize(data.Title, '_')}_{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow:HHmmss}";
                var pdfData = HtmlConverter.HtmlToPdf(htmlString, new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    EnableSmartShrinking = false,
                    EnablePrintMediaType = true,

                    Dpi = 300,
                    PageSize = PageSizeType.Letter,
                    MarginTop = 25f,
                    MarginRight = 25f,
                    MarginBottom = 25f,
                    MarginLeft = 25f,
                    HeaderSpacing = 7,
                    Zoom = 0.89f,
                    HeaderUrl = options.HeaderUrl,
                    FooterUrl = options.FooterUrl,
                    Variables = new HtmlConverterSettings.Variable[]
                    {
                        new HtmlConverterSettings.Variable("footer_title", data.AssetNumber),
                        new HtmlConverterSettings.Variable("header_title", data.Title),
                    },
                });

                return new PrintOutputFile(fileName, pdfData);
            }
        }

        #endregion

        #region Methods (helpers)

        protected string GetOptionRepeaterTableHead(string prefix, string postfix)
        {
            if (_questionTable == null)
                return string.Empty;

            var html = new StringBuilder();

            html.Append("<thead><tr>").Append(prefix);

            foreach (var col in _questionTable.GetHeader())
                RenderOptionRepeaterCell(html, "th", col);

            html.Append(postfix).Append("</tr></thead>");

            return html.ToString();
        }

        private static void RenderOptionRepeaterCell(StringBuilder html, string tagName, QuestionTable.CellData cell)
        {
            html.Append("<").Append(tagName)
                .Append(" style='text-align:").Append(cell.Alignment.ToString().ToLower()).Append(";'")
                .Append(" class='").Append(ControlHelper.MergeCssClasses("otext", cell.CssClass)).Append("'")
                .Append(">");

            html.Append(Markdown.ToHtml(cell.Text));

            html.Append("</").Append(tagName).Append(">");
        }

        protected string GetOptionRepeaterText(RepeaterItem item)
        {
            if (_questionTable != null)
            {
                var html = new StringBuilder();

                for (var i = 0; i < _questionTable.ColumnsCount; i++)
                {
                    var cell = _questionTable.GetBody(item.ItemIndex, i);

                    RenderOptionRepeaterCell(html, "td", cell);
                }

                return html.ToString();
            }
            else
            {
                var text = (string)DataBinder.Eval(item.DataItem, "Text");

                return $"<td class='otext'>{Markdown.ToHtml(text)}</td>";
            }
        }

        protected string GetOptionRepeaterTableHeadTitleCols()
        {
            if (_questionTable == null)
                return "<th></th>";

            var html = new StringBuilder();

            foreach (var col in _questionTable.GetHeader())
                RenderOptionRepeaterCell(html, "th", col);

            return html.ToString();
        }

        #endregion
    }
}