using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Forms.Models;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Options.Controls
{
    public partial class OptionReadRepeater : BaseUserControl
    {
        #region Properties

        public bool AllowEditOptionText
        {
            get => ViewState[nameof(AllowEditOptionText)] != null && (bool)ViewState[nameof(AllowEditOptionText)];
            set => ViewState[nameof(AllowEditOptionText)] = value;
        }

        public bool AllowHtml
        {
            get => (bool)(ViewState[nameof(AllowHtml)] ?? true);
            set => ViewState[nameof(AllowHtml)] = value;
        }

        #endregion

        #region Fields

        private QuestionTable _currentQuestionTable = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(OptionReadRepeater).FullName + "." + nameof(CommonStyle);
            CommonScript.ContentKey = typeof(OptionReadRepeater).FullName + "." + nameof(CommonScript);
        }

        #endregion

        #region Data binding

        public bool LoadData(Question question)
        {
            var isTable = question.Layout.Type == OptionLayoutType.Table;

            _currentQuestionTable = isTable
                ? QuestionTable.Build(question.Layout.Columns, question.Options.Select(x => x.Content.Title.Default))
                : null;

            if (question.Type == QuestionItemType.TrueOrFalse)
                LoadTrueOrFalseItems(question);

            else if (question.Type == QuestionItemType.SingleCorrect)
                LoadSingleCorrectItems(question);

            else if (question.Type == QuestionItemType.MultipleCorrect)
                LoadMultipleCorrectItems(question);

            else if (question.Type.IsComposed())
                LoadComposedItems(question);

            else if (question.Type == QuestionItemType.BooleanTable)
                LoadBooleanTableItems(question);

            else if (question.Type == QuestionItemType.Matching)
                LoadMatchingItems(question);

            else if (question.Type == QuestionItemType.Likert)
                LoadLikertItems(question);

            else if (question.Type.IsHotspot())
                LoadHotspotItems(question);

            else if (question.Type == QuestionItemType.Ordering)
                LoadOrderingItems(question);

            else
                return false;

            return true;
        }

        private void LoadTrueOrFalseItems(Question question)
        {
            MultiView.SetActiveView(TrueOrFalseOptionView);

            TrueOrFalseOptionRepeater.DataSource = question.Options.Select(x => new
            {
                x.Points,
                x.Letter,
                Title = x.Content.Title?.Default,
                Option = x
            });
            TrueOrFalseOptionRepeater.DataBind();
        }

        private void LoadSingleCorrectItems(Question question)
        {
            MultiView.SetActiveView(SingleCorrectOptionView);

            SingleCorrectOptionRepeater.DataSource = question.Options.Select(x => new
            {
                x.Points,
                x.Letter,
                Title = x.Content.Title?.Default,
                Option = x
            });
            SingleCorrectOptionRepeater.DataBind();
        }

        private void LoadMultipleCorrectItems(Question question)
        {
            MultiView.SetActiveView(MultipleCorrectOptionView);

            MultipleCorrectOptionRepeater.DataSource = question.Options.Select(x => new
            {
                x.Letter,
                Title = x.Content.Title?.Default,
                x.Points,
                x.IsTrue,
                Option = x
            });
            MultipleCorrectOptionRepeater.DataBind();
        }

        private void LoadComposedItems(Question question)
        {
            MultiView.SetActiveView(ComposedRubricView);

            ComposedRubricOptionRepeater.DataSource = question.Options.Select(x => new
            {
                x.Letter,
                Title = x.Content.Title?.Default,
                x.Points,
                Option = x
            });
            ComposedRubricOptionRepeater.DataBind();
        }

        private void LoadBooleanTableItems(Question question)
        {
            MultiView.SetActiveView(BooleanTableOptionView);

            BooleanTableOptionRepeater.DataSource = question.Options.Select(x => new
            {
                x.Letter,
                Title = x.Content.Title?.Default,
                x.Points,
                x.IsTrue,
                Option = x
            });
            BooleanTableOptionRepeater.DataBind();
        }

        private void LoadMatchingItems(Question question)
        {
            MultiView.SetActiveView(MatchingView);

            MatchingPairsRepeater.Visible = (question.Matches?.Pairs).IsNotEmpty();
            MatchingPairsRepeater.DataSource = question.Matches.Pairs.Select(x => new { Left = x.Left.Title.Default, Right = x.Right.Title.Default, x.Points });
            MatchingPairsRepeater.DataBind();

            var distractors = question.Matches.Distractors
                .Where(x => !string.IsNullOrEmpty(x.Title.Default))
                .Select(x => new { Value = x.Title.Default })
                .ToList();

            MatchingDistractorsRepeater.Visible = distractors.Count > 0;
            MatchingDistractorsRepeater.DataSource = distractors;
            MatchingDistractorsRepeater.DataBind();
        }

        private void LoadLikertItems(Question question)
        {
            MultiView.SetActiveView(LikertView);

            LikertColumnRepeater1.DataSource = question.Likert.Columns;
            LikertColumnRepeater1.DataBind();

            LikertColumnRepeater2.DataSource = question.Likert.Columns;
            LikertColumnRepeater2.DataBind();

            LikertRowRepeater.DataSource = question.Likert.Rows;
            LikertRowRepeater.ItemDataBound += (s, a) =>
            {
                if (!IsContentItem(a))
                    return;

                var row = (LikertRow)a.Item.DataItem;

                var repeater = (Repeater)a.Item.FindControl("LikertOptionRepeater");
                repeater.DataSource = row.GetOptions();
                repeater.DataBind();
            };
            LikertRowRepeater.DataBind();
        }

        private void LoadHotspotItems(Question question)
        {
            MultiView.SetActiveView(HotspotView);

            var data = new
            {
                id = HotspotImage.ClientID,
                src = question.Hotspot.Image.Url,
                width = question.Hotspot.Image.Width,
                height = question.Hotspot.Image.Height,
                shapes = question.Hotspot.Options.Select(x => x.Letter + " " + x.Shape.ToString())
            };
            var json = JsonHelper.SerializeJsObject(data);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(SingleCorrectOptionRepeater),
                "init_hotspot_" + ClientID,
                $"hotspotView.set({json});",
                true);

            HotspotOptionRepeater.DataSource = question.Hotspot.Options;
            HotspotOptionRepeater.DataBind();
        }

        private void LoadOrderingItems(Question question)
        {
            MultiView.SetActiveView(OrderingView);

            var ordering = question.Ordering;
            var label = ordering.Label;
            var topLabel = label.TopContent.Title.Default;
            var bottomLabel = label.BottomContent.Title.Default;

            OrderingTopLabel.InnerHtml = ConvertToHtml(topLabel);
            OrderingTopLabel.Visible = label.Show && topLabel.IsNotEmpty();

            OrderingBottomLabel.InnerHtml = ConvertToHtml(bottomLabel);
            OrderingBottomLabel.Visible = label.Show && bottomLabel.IsNotEmpty();

            OrderingSolutionRepeater.DataSource = ordering.Solutions.Select((solution, sIndex) => new
            {
                Letter = Calculator.ToBase26(sIndex + 1),
                Points = solution.Points,
                Options = solution.Options.Select(oId =>
                {
                    var option = ordering.GetOption(oId);

                    return new
                    {
                        Sequence = ordering.GetOptionIndex(option) + 1,
                        Html = ConvertToHtml(option.Content.Title.Default)
                    };
                }),
            });
            OrderingSolutionRepeater.ItemDataBound += (s, a) =>
            {
                if (!IsContentItem(a))
                    return;

                var optionRepeater = (Repeater)a.Item.FindControl("OptionRepeater");
                optionRepeater.DataSource = DataBinder.Eval(a.Item.DataItem, "Options");
                optionRepeater.DataBind();
            };
            OrderingSolutionRepeater.DataBind();
        }

        #endregion

        #region Helper methods

        protected string GetOptionRepeaterTableHead(string prefix, string postfix)
        {
            if (_currentQuestionTable == null)
                return string.Empty;

            var html = new StringBuilder();

            html.Append("<thead><tr>").Append(prefix);

            foreach (var col in _currentQuestionTable.GetHeader())
                RenderOptionRepeaterCell(html, "th", col);

            html.Append(postfix).Append("</tr></thead>");

            return html.ToString();
        }

        protected string GetOptionRepeaterTableHeadTitleCols()
        {
            if (_currentQuestionTable == null)
                return "<th></th>";

            var html = new StringBuilder();

            foreach (var col in _currentQuestionTable.GetHeader())
                RenderOptionRepeaterCell(html, "th", col);

            return html.ToString();
        }

        private void RenderOptionRepeaterCell(StringBuilder html, string tagName, QuestionTable.CellData cell)
        {
            html.Append("<")
                .Append(tagName)
                .Append(" style='text-align:")
                .Append(cell.Alignment.ToString().ToLower())
                .Append(";'");

            if (!string.IsNullOrEmpty(cell.CssClass))
                html.Append(" class='").Append(cell.CssClass).Append("'");

            html.Append(">");
            html.Append(ConvertToHtml(cell.Text));

            html.Append("</")
                .Append(tagName)
                .Append(">");
        }

        protected string GetOptionRepeaterTitle(RepeaterItem item)
        {
            if (_currentQuestionTable != null)
            {
                var html = new StringBuilder();

                for (var i = 0; i < _currentQuestionTable.ColumnsCount; i++)
                {
                    var cell = _currentQuestionTable.GetBody(item.ItemIndex, i);

                    RenderOptionRepeaterCell(html, "td", cell);
                }

                return html.ToString();
            }
            else if (AllowEditOptionText)
            {
                var option = (Option)DataBinder.Eval(item.DataItem, "Option");
                var text = $@"
<a href='#' class='editable-input'
    data-name='{ElementUpdater.ElementTypes.OptionTitle}'
    data-type='text'
    data-pk='{option.Question.Set.Bank.Identifier}:{option.Question.Identifier}:{option.Number}'
>{ConvertToHtml(option.Content.Title != null ? option.Content.Title.Default : null)}</a>
";
                return $"<td class='option-title'>{text}</td>";
            }
            else
            {
                var text = (string)DataBinder.Eval(item.DataItem, "Title");

                return $"<td>{ConvertToHtml(text)}</td>";
            }
        }

        protected string GetTrueOrFalseOptionIcon(decimal value) => value > 0
            ? $"<i class='text-{FlagType.Green.GetContextualClass()} far fa-check-circle'></i>"
            : $"<i class='text-{FlagType.Red.GetContextualClass()} far fa-times-circle text-danger'></i>";

        protected string GetSingleCorrectOptionIcon(decimal value) => value > 0
            ? $"<i class='text-{FlagType.Green.GetContextualClass()} far fa-check-circle'></i>"
            : $"<i class='text-{FlagType.Red.GetContextualClass()} far fa-times-circle text-danger'></i>";

        protected string GetMultipleCorrectOptionIcon(bool? isTrue) =>
            !isTrue.HasValue
                ? "<i class='far fa-exclamation-triangle text-warning' title='Not Configured'></i>"
                : isTrue.Value
                    ? "<i class='far fa-check-square'></i>"
                    : "<i class='far fa-square'></i>";

        protected string GetBooleanTableOptionIcon(bool? isTrue, bool answer) =>
            !isTrue.HasValue
                ? "<i class='far fa-exclamation-triangle text-warning' title='Not Configured'></i>"
                : isTrue.Value == answer
                    ? "<i class='far fa-dot-circle'></i>"
                    : "<i class='far fa-circle'></i>";

        protected string GetOptionPoints(decimal value) => $"{value:n2} points";

        protected string ConvertToHtml(string text)
        {
            return AllowHtml
                ? Markdown.ToHtml(text)
                : $"<span style='white-space:pre-wrap; max-width:100%;'>{HttpUtility.HtmlEncode(text)}</span>";
        }

        #endregion
    }
}