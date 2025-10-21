using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Controls
{
    public partial class ResultQuestionOutput : BaseUserControl
    {
        protected AttemptInfo Attempt { get; set; }
        protected QAttemptQuestion Question { get; set; }
        protected string ListBoxGroupClass { get; set; }
        protected string AnswerFileUrl { get; set; }

        #region Methods (binding)

        protected override void DataBindChildren()
        {
            if (Question != null)
                base.DataBindChildren();
        }

        public void LoadData(AttemptInfo attempt, QAttemptQuestion question)
        {
            Attempt = attempt;
            Question = question;
            AnswerFileUrl = AnswerQuestionOutput.GetFileUrl(question.AnswerFileIdentifier);

            var type = question.QuestionType.ToEnum<QuestionItemType>();
            var isListBox = type == QuestionItemType.MultipleCorrect;

            MultiView.ActiveViewIndex = -1;

            if (type.IsRadioList() || isListBox)
            {
                ListBoxGroupClass = isListBox ? "radio-list" : "checkbox-list";
                BindOptionRepeater(ListTableHeaderRepeater, ListOptionRepeater);
                MultiView.SetActiveView(ViewListBox);
            }
            else if (type == QuestionItemType.ComposedEssay)
            {
                MultiView.SetActiveView(ViewComposedEssay);
            }
            else if (type == QuestionItemType.ComposedVoice)
            {
                MultiView.SetActiveView(ViewComposedVoice);
            }
            else if (type == QuestionItemType.BooleanTable)
            {
                BindOptionRepeater(BooleanTableHeaderRepeater, BooleanOptionRepeater);
                MultiView.SetActiveView(ViewBooleanTable);
            }
            else if (type == QuestionItemType.Matching)
            {
                BindMatchesRepeater();
                MultiView.SetActiveView(ViewMatching);
            }
            else if (type == QuestionItemType.Likert)
            {
                BindLikertRepeater();
                MultiView.SetActiveView(ViewLikert);
            }
            else if (type.IsHotspot())
            {
                MultiView.SetActiveView(ViewHotspot);
            }
            else if (type == QuestionItemType.Ordering)
            {
                BindOrderingOptionRepeater();
                MultiView.SetActiveView(ViewOrdering);
            }

            DataBind();
        }

        private void BindOptionRepeater(Repeater headerRepeater, Repeater optionRepeater)
        {
            var options = Attempt.GetQuestionOptions(Question.QuestionIdentifier).ToArray();
            var bankQuestion = Attempt.Bank.FindQuestion(Question.QuestionIdentifier);
            var tableData = bankQuestion?.Layout.Type == OptionLayoutType.Table
                ? QuestionTable.Build(bankQuestion.Layout.Columns, options.Select(x => x.OptionText))
                : null;

            var tableHeader = tableData?.GetHeader();
            var tableBody = tableData?.GetBody();
            var optionHeaders = tableHeader != null && tableHeader.Any(cell => cell.Text.IsNotEmpty())
                ? tableHeader.Where(x => x != null).Select(x => new AttemptOptionCell(x)).ToArray()
                : null;

            headerRepeater.Visible = optionHeaders.IsNotEmpty();
            headerRepeater.DataSource = optionHeaders;

            optionRepeater.ItemDataBound += OptionRepeater_ItemDataBound;
            optionRepeater.DataSource = options.Select((o, i) => new
            {
                Option = o,
                Cells = tableBody?[i].Select(x => new AttemptOptionCell(x)).ToArray()
            });
        }

        private void BindMatchesRepeater()
        {
            MatchesRepeater.DataSource = Attempt.GetQuestionMatches(Question.QuestionIdentifier);
        }

        private void BindLikertRepeater()
        {
            var data = Attempt.GetSubQuestions(Question.QuestionIdentifier).Select(q => new
            {
                Question = q,
                Options = Attempt.GetQuestionOptions(q.QuestionIdentifier)
            }).ToArray();

            LikertColumnRepeater.DataSource = data.FirstOrDefault()?.Options;

            LikertRowRepeater.ItemDataBound += LikertRowRepeater_ItemDataBound;
            LikertRowRepeater.DataSource = data;
        }

        private void BindOrderingOptionRepeater()
        {
            OrderingOptionRepeater.DataSource = Attempt.GetQuestionOptions(Question.QuestionIdentifier)
                .OrderBy(x => x.OptionAnswerSequence).ThenBy(x => x.OptionSequence);
        }

        #endregion

        #region Methods (render)

        protected static string GetCircleOptionIcon(bool? isCorrect) => GetAnswerOptionIcon("circle", isCorrect);

        protected static string GetSquareOptionIcon(bool? isCorrect) => GetAnswerOptionIcon("square", isCorrect);

        private static string GetAnswerOptionIcon(string icon, bool? isCorrect)
        {
            if (!isCorrect.HasValue)
                return $"<i class='far fa-{icon} fs-4 text-input lh-1 align-middle'></i>";
            else if (isCorrect.Value)
                return $"<i class='text-success far fa-check-{icon} fs-4 lh-1 align-middle'></i>";
            else
                return $"<i class='text-danger far fa-times-{icon} fs-4 lh-1 align-middle'></i>";
        }

        protected string GetListBoxOptionIcon(string field)
        {
            var dataItem = Page.GetDataItem();
            var option = (QAttemptOption)(field == null ? dataItem : DataBinder.Eval(dataItem, field));
            var isCorrect = option.AnswerIsSelected == true
                ? option.OptionIsTrue.HasValue && option.OptionIsTrue.Value || !option.OptionIsTrue.HasValue && option.OptionPoints > 0
                : (bool?)null;

            return GetCircleOptionIcon(isCorrect);
        }

        protected string GetBooleanOptionIcon(bool value)
        {
            var option = (QAttemptOption)DataBinder.Eval(Page.GetDataItem(), "Option");
            var isCorrect = option.AnswerIsSelected == value && option.OptionIsTrue.HasValue
                ? value == option.OptionIsTrue.Value
                : (bool?)null;

            return GetCircleOptionIcon(isCorrect);
        }

        protected string GetMatchOptionIcon()
        {
            var match = (QAttemptMatch)Page.GetDataItem();
            var isCorrect = match.AnswerText.IsNotEmpty()
                ? match.AnswerText == match.MatchRightText
                : (bool?)null;

            return GetSquareOptionIcon(isCorrect);
        }

        protected string GetOrderingOptionClass()
        {
            var result = "bg-white border rounded py-2 px-3 mb-3";

            var option = (QAttemptOption)Page.GetDataItem();
            if (option.OptionPoints > 0)
                result += " border-success";
            else
                result += " border-danger";

            return result;
        }

        protected string GetHotspotShapes()
        {
            if (Attempt == null || Question.ShowShapes != true)
                return "null";

            var shapes = Attempt.GetQuestionOptions(Question.QuestionIdentifier).Select(x => x.OptionShape);
            return JsonConvert.SerializeObject(shapes);
        }

        protected string GetHotspotPins()
        {
            if (Attempt == null)
                return null;

            var pins = Attempt.GetQuestionPins(Question.QuestionIdentifier);
            return JsonConvert.SerializeObject(pins.Select(x => $"{((x.OptionPoints ?? 0) > 0 ? 1 : 0)},{x.PinX},{x.PinY}"));
        }

        #endregion

        #region Event handlers

        private void OptionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var cells = (AttemptOptionCell[])DataBinder.Eval(e.Item.DataItem, "Cells");
            var hasCells = cells.IsNotEmpty();

            var tableBodyRepeater = (Repeater)e.Item.FindControl("TableBodyRepeater");
            tableBodyRepeater.DataSource = cells;
            tableBodyRepeater.DataBind();
            tableBodyRepeater.Visible = hasCells;

            var optionText = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("OptionText");
            optionText.Visible = !hasCells;
        }

        private void LikertRowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = e.Item;

            var repeater = (Repeater)item.FindControl("LikertOptionRepeater");
            repeater.DataSource = DataBinder.Eval(item.DataItem, "Options");
            repeater.DataBind();
        }

        #endregion
    }
}