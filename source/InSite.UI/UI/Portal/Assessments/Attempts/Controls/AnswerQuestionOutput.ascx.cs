using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Controls
{
    public partial class AnswerQuestionOutput : BaseUserControl
    {
        protected class ControlData
        {
            public AttemptInfo Attempt { get; set; }
            public Question BankQuestion { get; set; }
            public QAttemptQuestion AttemptQuestion { get; set; }

            public int LimitAnswers { get; set; }
            public string TagsAndLabels { get; set; }
            public bool IsRadioList { get; set; }
            public bool IsCheckList { get; set; }
            public bool IsComposedEssay { get; set; }
            public bool IsComposedVoice { get; set; }
            public bool IsBooleanTable { get; set; }
            public bool IsMatching { get; set; }
            public bool IsLikert { get; set; }
            public bool IsHotspot { get; set; }
            public bool IsOrdering { get; set; }
        }

        public const AudioBitrateMode ComposedVoiceBitrate = AudioBitrateMode.kb_64;

        public int QuestionIndex => Data.AttemptQuestion.QuestionSequence - 1;

        public int? QuestionNumber => Data.AttemptQuestion.QuestionNumber;

        protected ControlData Data { get; private set; }

        protected override void DataBindChildren()
        {
            if (Data != null)
                base.DataBindChildren();
        }

        public void LoadData(AttemptInfo attempt, QAttemptQuestion question, bool includeQuestionSeparator)
        {
            BindData(attempt, question);

            QuestionStartComment.Visible = includeQuestionSeparator;
            QuestionEndComment.Visible = includeQuestionSeparator;

            if (Data.IsRadioList || Data.IsCheckList)
                BindOptionRepeater(ListTableHeaderRepeater, ListOptionRepeater);

            else if (Data.IsBooleanTable)
                BindOptionRepeater(BooleanTableHeaderRepeater, BooleanOptionRepeater);

            else if (Data.IsMatching)
                BindMatchesRepeater();

            else if (Data.IsLikert)
                BindLikertRepeater();

            else if (Data.IsOrdering)
                BindOrderingRepeater();

            DataBind();
        }

        private void BindData(AttemptInfo attempt, QAttemptQuestion question)
        {
            var environment = ServiceLocator.AppSettings.Environment.Name;
            var type = question.QuestionType.ToEnum<QuestionItemType>();

            var data = Data = new ControlData
            {
                Attempt = attempt,
                AttemptQuestion = question,

                IsRadioList = type.IsRadioList(),
                IsCheckList = type == QuestionItemType.MultipleCorrect,
                IsComposedEssay = type == QuestionItemType.ComposedEssay,
                IsComposedVoice = type == QuestionItemType.ComposedVoice,
                IsBooleanTable = type == QuestionItemType.BooleanTable,
                IsMatching = type == QuestionItemType.Matching,
                IsLikert = type == QuestionItemType.Likert,
                IsHotspot = type.IsHotspot(),
                IsOrdering = type == QuestionItemType.Ordering
            };

            var bankQuestion = attempt.Bank.FindQuestion(question.QuestionIdentifier);
            if (bankQuestion == null)
                return;

            data.BankQuestion = bankQuestion;
            data.LimitAnswers = bankQuestion.CalculationMethod == QuestionCalculationMethod.LimitedCorrect && bankQuestion.Type.IsCheckList()
                ? bankQuestion.Options.Count(x => x.IsTrue == true)
                : 0;
            data.TagsAndLabels = environment != EnvironmentName.Production && environment != EnvironmentName.Sandbox
                ? bankQuestion.Classification?.Tag.NullIfEmpty()
                : null;
        }

        private void BindOptionRepeater(Repeater headerRepeater, Repeater optionRepeater)
        {
            var options = Data.Attempt.GetQuestionOptions(Data.AttemptQuestion.QuestionIdentifier).ToArray();
            var tableData = Data.BankQuestion?.Layout.Type == OptionLayoutType.Table
                ? QuestionTable.Build(Data.BankQuestion.Layout.Columns, options.Select(x => x.OptionText))
                : null;

            var tableHeader = tableData?.GetHeader();
            var tableBody = tableData?.GetBody();
            var optionHeaders = tableHeader != null && tableHeader.Any(cell => cell.Text.IsNotEmpty())
                ? tableHeader.Where(x => x != null).Select(x => new AttemptOptionCell(x)).ToArray()
                : null;

            headerRepeater.Visible = optionHeaders.IsNotEmpty();
            headerRepeater.DataSource = optionHeaders;
            headerRepeater.DataBind();

            optionRepeater.ItemDataBound += OptionRepeater_ItemDataBound;
            optionRepeater.DataSource = options.Select((o, i) => new
            {
                OptionId = GetOptionId(o),
                Option = o,
                Cells = tableBody?[i].Select(x => new AttemptOptionCell(x)).ToArray()
            });
            optionRepeater.DataBind();
        }

        private void BindMatchesRepeater()
        {
            var matches = Data.Attempt.GetQuestionMatches(Data.AttemptQuestion.QuestionIdentifier).ToArray();

            var options = matches.Select(x => x.MatchRightText).Concat(Data.AttemptQuestion.GetMatchDistractors()).Distinct().ToArray();
            options.Shuffle();

            MatchesRepeater.ItemDataBound += MatchesRepeater_ItemDataBound;
            MatchesRepeater.DataSource = matches.Select(x => new
            {
                Match = x,
                Options = options
            });
        }

        private void BindLikertRepeater()
        {
            var data = Data.Attempt.GetSubQuestions(Data.AttemptQuestion.QuestionIdentifier).Select(q => new
            {
                Question = q,
                Options = Data.Attempt.GetQuestionOptions(q.QuestionIdentifier).ToArray()
            });

            LikertColumnRepeater.DataSource = data.FirstOrDefault()?.Options;

            LikertRowRepeater.ItemDataBound += LikertRowRepeater_ItemDataBound;
            LikertRowRepeater.DataSource = data;
        }

        private void BindOrderingRepeater()
        {
            OrderingOptionRepeater.DataSource = Data.Attempt.GetQuestionOptions(Data.AttemptQuestion.QuestionIdentifier)
                .OrderBy(x => x.OptionAnswerSequence).ThenBy(x => x.OptionSequence);
            OrderingOptionRepeater.DataBind();
        }

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

        private void MatchesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = e.Item.DataItem;
            var match = (QAttemptMatch)DataBinder.Eval(dataItem, "Match");
            var options = (string[])DataBinder.Eval(dataItem, "Options");

            var matchOptionsRepeater = (Repeater)e.Item.FindControl("MatchOptionsRepeater");
            matchOptionsRepeater.DataSource = options.Select(x => new
            {
                MatchSequence = match.MatchSequence,
                Value = x,
                IsSelected = match.AnswerText == x
            });
            matchOptionsRepeater.DataBind();
        }

        private void LikertRowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = e.Item.DataItem;
            var question = (QAttemptQuestion)DataBinder.Eval(dataItem, "Question");
            var options = (QAttemptOption[])DataBinder.Eval(dataItem, "Options");

            var repeater = (Repeater)e.Item.FindControl("LikertOptionRepeater");
            repeater.DataSource = options.Select(x => new
            {
                IsRadioList = question.QuestionType.ToEnum<QuestionItemType>().IsRadioList(),
                OptionId = GetOptionId(x),
                Option = x
            });
            repeater.DataBind();
        }

        public static string GetFileUrl(Guid? fileId)
        {
            var file = fileId.HasValue
                ? ServiceLocator.StorageService.GetFile(fileId.Value)
                : null;
            return file == null
                ? null
                : ServiceLocator.StorageService.GetFileUrl(file.FileIdentifier, file.FileName, false);
        }

        protected string GetHtml(string text)
        {
            return Data.Attempt.GetHtml(Data.AttemptQuestion, text);
        }

        protected string GetHotspotShapes()
        {
            var question = Data.AttemptQuestion;
            if (question.ShowShapes != true)
                return "null";

            var options = Data.Attempt.GetQuestionOptions(question.QuestionIdentifier);
            return Newtonsoft.Json.JsonConvert.SerializeObject(options.Select(x => x.OptionShape));
        }

        protected string GetHotspotPins()
        {
            var pins = Data.Attempt.GetQuestionPins(Data.AttemptQuestion.QuestionIdentifier);
            return Newtonsoft.Json.JsonConvert.SerializeObject(pins.Select(x => $"{x.PinX},{x.PinY}"));
        }

        protected string GetOptionId()
        {
            return GetOptionId((QAttemptOption)Page.GetDataItem());
        }

        private string GetOptionId(QAttemptOption option)
        {
            return option.QuestionSequence + "_" + option.OptionSequence;
        }
    }
}