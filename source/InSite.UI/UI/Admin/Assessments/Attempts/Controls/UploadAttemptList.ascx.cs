using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common.File;

using RadioButton = System.Web.UI.WebControls.RadioButton;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class UploadAttemptList : BaseUserControl
    {
        private const int MaxScanCount = 4;

        protected int? SelectedAttemptIndex
        {
            get => (int?)ViewState[nameof(SelectedAttemptIndex)];
            set => ViewState[nameof(SelectedAttemptIndex)] = value;
        }

        private AttemptUploadAnswer[] AttemptItems
        {
            get => (AttemptUploadAnswer[])ViewState[nameof(AttemptItems)];
            set => ViewState[nameof(AttemptItems)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AttemptRepeater.ItemCreated += AttemptRepeater_ItemCreated;
            AttemptRepeater.ItemDataBound += AttemptRepeater_ItemDataBound;

            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                SaveToCache();
        }

        public void LoadData(AttemptUploadAnswer[] attempts)
        {
            AttemptItems = attempts;

            SelectedAttemptIndex = null;

            AttemptRepeater.DataSource = attempts;
            AttemptRepeater.DataBind();

            QuestionRepeater.Visible = false;
        }

        public AttemptUploadAnswer[] GetData() => AttemptItems;

        private void AttemptRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var selectedRadioButton = (RadioButton)e.Item.FindControl("Selected");
            selectedRadioButton.AutoPostBack = true;
            selectedRadioButton.CheckedChanged += SelectedRadioButton_CheckedChanged;
        }

        private void AttemptRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var attemptItem = (AttemptUploadAnswer)e.Item.DataItem;

            var selectedRadioButton = (RadioButton)e.Item.FindControl("Selected");
            selectedRadioButton.Checked = SelectedAttemptIndex == attemptItem.Sequence - 1;

            if (attemptItem.HasUserAccount && !attemptItem.IsValid)
                selectedRadioButton.Attributes["class"] = "invalid-attempt";
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var personItem = AttemptItems[SelectedAttemptIndex.Value];
                var scanCount = personItem.ScanCount > MaxScanCount ? MaxScanCount : personItem.ScanCount;

                var answerHeaders = new List<string>();

                if (personItem.HasCurrentAttempt)
                    answerHeaders.Add("Current");

                for (var i = 0; i < scanCount; i++)
                    answerHeaders.Add($"Scan {i + 1}");

                var answerRepeater = (Repeater)e.Item.FindControl("AnswerRepeater");
                answerRepeater.DataSource = answerHeaders;
                answerRepeater.DataBind();

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var personItem = AttemptItems[SelectedAttemptIndex.Value];
                var scanCount = personItem.ScanCount > MaxScanCount ? MaxScanCount : personItem.ScanCount;
                var question = (AttemptUploadAnswer.Question)e.Item.DataItem;

                var answers = new List<string>();

                if (personItem.HasCurrentAttempt)
                    answers.Add(question.CurrentAnswer);

                for (var i = 0; i < scanCount; i++)
                    answers.Add(question.Answers[i]);

                var answerRepeater = (Repeater)e.Item.FindControl("AnswerRepeater");
                answerRepeater.DataSource = answers;
                answerRepeater.DataBind();

                var correctIcon = (HtmlGenericControl)e.Item.FindControl("CorrectIcon");
                var incorrectIcon = (HtmlGenericControl)e.Item.FindControl("IncorrectIcon");

                // if (question.CorrectAnswer != null)
                {
                    var isCorrect = question.CorrectAnswer.Equals(question.FinalAnswer, StringComparison.OrdinalIgnoreCase);

                    correctIcon.Style["display"] = isCorrect ? "" : "none";
                    incorrectIcon.Style["display"] = !isCorrect ? "" : "none";
                }

                var finalAnswerTextBox = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("FinalAnswer");
                finalAnswerTextBox.Enabled = AttemptItems[SelectedAttemptIndex.Value].HasUserAccount;
            }
        }

        private void SelectedRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            var selectedRadioButton = (RadioButton)sender;
            var repeaterItem = (RepeaterItem)selectedRadioButton.NamingContainer;
            var attemptIndex = repeaterItem.ItemIndex;

            SelectedAttemptIndex = attemptIndex;

            AttemptRepeater.DataSource = AttemptItems;
            AttemptRepeater.DataBind();

            var attempt = AttemptItems[attemptIndex];

            QuestionRepeater.Visible = true;
            QuestionRepeater.DataSource = attempt.Questions;
            QuestionRepeater.DataBind();
        }

        private void SaveToCache()
        {
            if (SelectedAttemptIndex == null)
                return;

            var personItem = AttemptItems[SelectedAttemptIndex.Value];
            var questionIndex = 0;

            foreach (RepeaterItem repeaterItem in QuestionRepeater.Items)
            {
                var finalAnswerTextBox = (ITextControl)repeaterItem.FindControl("FinalAnswer");

                personItem.Questions[questionIndex].FinalAnswer = finalAnswerTextBox.Text;

                questionIndex++;
            }
        }
    }
}