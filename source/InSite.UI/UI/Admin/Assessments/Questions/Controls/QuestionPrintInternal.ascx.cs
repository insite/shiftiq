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
using UserModel = InSite.Domain.Foundations.User;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPrintInternal : BaseUserControl
    {
        #region Classes

        public class BankOptions : Options
        {
            public Guid BankID { get; }

            public BankOptions(OrganizationState organization, UserModel user, Guid bankId, bool includeImages, bool includeAdminComments)
                : base(organization, user, includeImages, includeAdminComments)
            {
                BankID = bankId;
            }
        }

        public class FormOptions : Options
        {
            public Guid FormID { get; }

            public FormOptions(OrganizationState organization, UserModel user, Guid formId, bool includeImages, bool includeAdminComments)
                : base(organization, user, includeImages, includeAdminComments)
            {
                FormID = formId;
            }
        }

        public class Options
        {
            public Guid OrganizationID { get; }
            public TimeZoneInfo TimeZone { get; }

            public string CurrentUrl { get; }
            public string HeaderUrl { get; }

            public bool IncludeImages { get; }
            public bool IncludeAdminComments { get; }

            public Options(OrganizationState organization, UserModel user, bool includeImages, bool includeAdminComments)
            {
                OrganizationID = organization.Identifier;
                TimeZone = user.TimeZone;

                var request = HttpContext.Current.Request;

                CurrentUrl = request.Url.Scheme + "://" + request.Url.Host + request.RawUrl;
                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Assessments/Questions/Html/PrintInternalHeader.html");

                IncludeImages = includeImages;
                IncludeAdminComments = includeAdminComments;
            }
        }

        private class ControlData
        {
            public string AssetNumber { get; }
            public string Title { get; }
            public string Name { get; }
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
                AssetNumber = $"{form.Asset}.{form.AssetVersion}";
                Title = (form.Content.Title?.Default).IfNullOrEmpty(form.Name);
                Name = form.Name;
                Questions = QuestionPrintHelper.GetQuestions(form);
            }
        }

        #endregion

        #region Fields

        private QuestionTable _questionTable = null;
        private Options _options;

        #endregion

        #region Event handlers

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var info = (IQuestionInfo)e.Item.DataItem;

            BindProperties(e.Item, info);
            BindComments(e.Item, info);

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

        private void BindProperties(RepeaterItem item, IQuestionInfo info)
        {
            var propertyRepeater = (Repeater)item.FindControl("PropertyRepeater");
            propertyRepeater.DataSource = QuestionPrintHelper.EnumerateProperties(info);
            propertyRepeater.DataBind();
        }

        private void BindComments(RepeaterItem item, IQuestionInfo info)
        {
            if (!_options.IncludeAdminComments)
                return;

            var comments = info.GetAdminComments();
            if (comments.Count == 0)
                return;

            var commentRepeater = (Repeater)item.FindControl("CommentRepeater");
            commentRepeater.DataSource = comments;
            commentRepeater.DataBind();
            commentRepeater.Visible = true;
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

            var pairsRepeater = (Repeater)item.FindControl("MatchingPairsRepeater");
            pairsRepeater.Visible = question.Pairs.IsNotEmpty();
            pairsRepeater.DataSource = question.Pairs;
            pairsRepeater.DataBind();

            var distractorsRepeater = (Repeater)item.FindControl("MatchingDistractorsRepeater");
            distractorsRepeater.Visible = question.Distractors.IsNotEmpty();
            distractorsRepeater.DataSource = question.Distractors;
            distractorsRepeater.DataBind();
        }

        private void BindLikert(RepeaterItem item, AttemptQuestionLikert question)
        {
            SetActiveView(item, "LikertItemsView");

            var columns = question.Questions[0].Options;

            var likertColumnRepeater1 = (Repeater)item.FindControl("LikertColumnRepeater1");
            likertColumnRepeater1.DataSource = columns;
            likertColumnRepeater1.DataBind();

            var likertColumnRepeater2 = (Repeater)item.FindControl("LikertColumnRepeater2");
            likertColumnRepeater2.DataSource = columns;
            likertColumnRepeater2.DataBind();

            var likertRowRepeater = (Repeater)item.FindControl("LikertRowRepeater");
            likertRowRepeater.DataSource = question.Questions;
            likertRowRepeater.ItemDataBound += (s, a) =>
            {
                if (a.Item.ItemType != ListItemType.Item && a.Item.ItemType != ListItemType.AlternatingItem)
                    return;

                var repeater = (Repeater)a.Item.FindControl("LikertOptionRepeater");
                repeater.DataSource = DataBinder.Eval(a.Item.DataItem, "Options");
                repeater.DataBind();
            };
            likertRowRepeater.DataBind();
        }

        private void BindHotspot(RepeaterItem item, AttemptQuestionHotspot question)
        {
            SetActiveView(item, "HotspotItemsView");

            var data = HotspotImage.FromString(question.Image);

            var image = (ITextControl)item.FindControl("HotspotImage");
            image.Text = $"<img alt='' class='hotspot-image' src='{data.Url}' />";

            var itemRepeater = (Repeater)item.FindControl("HotspotItemRepeater");
            itemRepeater.DataSource = question.Options;
            itemRepeater.DataBind();
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

            var options = question.Options.Select((x, i) => new { Sequence = i + 1, Option = x }).ToArray();

            var solutionRepeater = (Repeater)item.FindControl("OrderingSolutionRepeater");
            solutionRepeater.DataSource = question.Solutions;
            solutionRepeater.ItemDataBound += (s, a) =>
            {
                if (!IsContentItem(a))
                    return;

                var solution = (AttemptQuestionOrderingSolution)a.Item.DataItem;

                var optionRepeater = (Repeater)a.Item.FindControl("OptionRepeater");
                optionRepeater.DataSource = solution.OptionsOrder.Select(key => options.First(o => o.Option.Key == key));
                optionRepeater.DataBind();
            };
            solutionRepeater.DataBind();
        }

        #endregion

        private void LoadData(ControlData data, Options options)
        {
            _options = options;

            QuestionPrintHelper.InitProperties(data.Questions, options.OrganizationID);

            PageTitle.InnerText = data.Title.IfNullOrEmpty("Untitled");
            ExcludeImagesStyle.Visible = !options.IncludeImages;

            var hasData = data.Questions.Length > 0;

            NoDataMessage.Visible = !hasData;

            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;

            QuestionRepeater.Visible = hasData;
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

                var report = (QuestionPrintInternal)page.LoadControl("~/UI/Admin/Assessments/Questions/Controls/QuestionPrintInternal.ascx");

                report.LoadData(data, options);

                var htmlBuilder = new StringBuilder();

                using (var writer = new StringWriter(htmlBuilder))
                {
                    using (var htmlWriter = new HtmlTextWriter(writer))
                        report.RenderControl(htmlWriter);
                }

                var htmlString = HtmlHelper.ResolveRelativePaths(options.CurrentUrl, htmlBuilder);

                var fileName = $"{StringHelper.Sanitize(data.Title, '_')}_internal_{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow:HHmmss}";

                var pdfData = HtmlConverter.HtmlToPdf(htmlString, new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    EnablePrintMediaType = true,
                    PageSize = PageSizeType.Letter,
                    MarginTop = 25f,
                    HeaderSpacing = 7,
                    HeaderUrl = options.HeaderUrl,
                    Variables = new HtmlConverterSettings.Variable[]
                    {
                        new HtmlConverterSettings.Variable("title", data.Title),
                        new HtmlConverterSettings.Variable("name", data.Name),
                        new HtmlConverterSettings.Variable("asset_number", data.AssetNumber),
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

        protected string GetSingleCorrectOptionIcon(decimal value) => value > 0
            ? $"<i class='far fa-check-circle'></i>"
            : $"<i class='far fa-circle'></i>";

        protected string GetMultipleCorrectOptionIcon(bool? isTrue) =>
            !isTrue.HasValue
                ? "<i class='far fa-exclamation-triangle' title='Not Configured'></i>"
                : isTrue.Value
                    ? "<i class='far fa-check-square'></i>"
                    : "<i class='far fa-square'></i>";

        protected string GetBooleanTableOptionIcon(bool? isTrue, bool answer) =>
            !isTrue.HasValue
                ? "<i class='far fa-exclamation-triangle' title='Not Configured'></i>"
                : isTrue.Value == answer
                    ? "<i class='far fa-dot-circle'></i>"
                    : "<i class='far fa-circle'></i>";

        protected string GetOptionPoints(decimal value) => $"{value:n2} points";

        protected string FormatDateTime(string name)
        {
            var dataItem = Page.GetDataItem();
            var value = (DateTimeOffset)DataBinder.Eval(dataItem, name);
            value = TimeZoneInfo.ConvertTime(value, _options.TimeZone);
            var tz = TimeZones.GetAbbreviation(_options.TimeZone)?.GetAbbreviation(value) ?? _options.TimeZone.Id;

            return string.Format("{0:MMM d, yyyy} at {0:h:mm tt} <span class='comment-timezone'>{1}</span>", value, tz);
        }

        #endregion
    }
}