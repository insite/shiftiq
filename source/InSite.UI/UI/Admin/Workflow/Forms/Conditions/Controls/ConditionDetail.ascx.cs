using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Domain.Surveys.Forms;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Surveys.Forms.Controls
{
    public partial class SurveyConditionDetail : UserControl
    {
        public class Result
        {
            public Guid MaskingItem { get; set; }
            public Guid[] MaskedQuestions { get; set; }
        }

        private Guid SurveyIdentifier
        {
            get => (Guid)ViewState[nameof(SurveyIdentifier)];
            set => ViewState[nameof(SurveyIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionID.AutoPostBack = true;
            QuestionID.ValueChanged += QuestionID_ValueChanged;

            OptionListID.AutoPostBack = true;
            OptionListID.ValueChanged += OptionListID_ValueChanged;

            OptionID.AutoPostBack = true;
            OptionID.ValueChanged += OptionID_ValueChanged;

            HideValidator.ServerValidate += HideValidator_ServerValidate;
        }

        public void SetDefaultInputValues(SurveyForm form, Guid? defaultQuestionIdentifier, TimeZoneInfo tz)
        {
            SurveyIdentifier = form.Identifier;

            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);
            SurveyDetail.BindSurvey(survey, tz);

            QuestionID.SurveyIdentifier = SurveyIdentifier;
            QuestionID.HasOptions = true;
            QuestionID.RefreshData();

            if (defaultQuestionIdentifier.HasValue)
                QuestionID.ValueAsGuid = defaultQuestionIdentifier;

            OnQuestionChanged();
        }

        public void SetInputValues(SurveyOptionItem optionItem, SurveyState survey, TimeZoneInfo tz)
        {
            var form = optionItem.Question.Form;

            SurveyIdentifier = form.Identifier;

            QuestionID.SurveyIdentifier = SurveyIdentifier;
            QuestionID.HasOptions = true;
            QuestionID.RefreshData();
            QuestionID.ValueAsGuid = optionItem.Question.Identifier;

            OnQuestionChanged();

            OptionListID.ValueAsGuid = optionItem.List.Identifier;

            OnOptionListChanged();

            OptionID.ValueAsGuid = optionItem.Identifier;

            OnOptionChanged();

            foreach (var question in optionItem.MaskedQuestionIdentifiers)
            {
                var item = HideQuestionID.Items.FindByValue(question.ToString());

                if (item != null)
                    item.Selected = true;
            }

            SurveyDetail.BindSurvey(survey, tz);
        }

        public Result GetInputValues()
        {
            var maskedQuestions = new List<Guid>();

            foreach (System.Web.UI.WebControls.ListItem item in HideQuestionID.Items)
            {
                if (item.Selected)
                    maskedQuestions.Add(Guid.Parse(item.Value));
            }

            return new Result
            {
                MaskingItem = OptionID.ValueAsGuid.Value,
                MaskedQuestions = maskedQuestions.ToArray()
            };

        }

        private void HideValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            foreach (System.Web.UI.WebControls.ListItem item in HideQuestionID.Items)
            {
                if (item.Selected)
                {
                    args.IsValid = true;
                    return;
                }
            }

            args.IsValid = false;
        }

        private void QuestionID_ValueChanged(object sender, EventArgs e)
        {
            OnQuestionChanged();
        }

        private void OnQuestionChanged()
        {
            OptionListID.QuestionIdentifier = QuestionID.ValueAsGuid;
            OptionListID.RefreshData();

            OptionListField.Visible = OptionListID.Items.Count > 2;

            OptionID.OptionListIdentifier = OptionListID.Items.Count == 2 ? Guid.Parse(OptionListID.Items.GetOption(1).Value) : (Guid?)null;
            OptionID.RefreshData();

            SetupHideQuestionID(null);
        }

        private void OptionListID_ValueChanged(object sender, EventArgs e)
        {
            OnOptionListChanged();
        }

        private void OnOptionListChanged()
        {
            OptionID.OptionListIdentifier = OptionListID.ValueAsGuid;
            OptionID.RefreshData();

            SetupHideQuestionID(null);
        }

        private void OptionID_ValueChanged(object sender, EventArgs e)
        {
            OnOptionChanged();
        }

        private void OnOptionChanged()
        {
            SetupHideQuestionID(SurveyIdentifier);
        }

        private void SetupHideQuestionID(Guid? surveyIdentifier)
        {
            var data = new List<System.Web.UI.WebControls.ListItem>();

            if (surveyIdentifier.HasValue)
            {
                var survey = ServiceLocator.SurveySearch.GetSurveyState(surveyIdentifier.Value);

                List<SurveyQuestion> questions;

                if (OptionID.ValueAsGuid.HasValue)
                {
                    var option = survey.Form.FindOptionItem(OptionID.ValueAsGuid.Value);
                    var currentPage = survey.Form.GetPageNumber(option.Question.Identifier);
                    var pages = survey.Form.GetPages();

                    questions = pages
                        .Where(x => x.Sequence > currentPage)
                        .SelectMany(x => x.Questions)
                        .ToList();
                }
                else
                {
                    questions = survey.Form.Questions;
                }

                foreach (var question in questions)
                {
                    data.Add(new System.Web.UI.WebControls.ListItem
                    {
                        Value = question.Identifier.ToString(),
                        Text = GetText(question)
                    });
                }
            }

            HideQuestionID.DataValueField = "Value";
            HideQuestionID.DataTextField = "Text";
            HideQuestionID.DataSource = data;
            HideQuestionID.DataBind();
        }

        private string GetText(SurveyQuestion question)
        {
            var prefix = question.Code.IfNullOrEmpty(question.Sequence.ToString()) + ".";
            var title = question.Content?.Title.GetSnip();

            return title.HasValue()
                ? $"{prefix} {title}"
                : $"Question {prefix} {question.Type.GetDescription()}";
        }
    }
}