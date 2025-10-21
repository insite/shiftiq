using System.Web.UI;

using InSite.Domain.Surveys.Forms;

using Shift.Constant;

namespace InSite.Admin.Surveys.Questions.Controls
{
    public partial class DetailList : UserControl
    {
        public void SetDefaultInputValues(SurveyQuestionType questionType, string lang)
        {
            ListEnableBranchField.Visible = questionType == SurveyQuestionType.RadioList || questionType == SurveyQuestionType.Selection;
            ListEnableRandomizationField.Visible = questionType == SurveyQuestionType.RadioList || questionType == SurveyQuestionType.CheckList;
            ListEnableOtherTextField.Visible = questionType == SurveyQuestionType.RadioList || questionType == SurveyQuestionType.CheckList;

            OptionGrid.LoadData(new SurveyQuestion(), lang);
        }

        public int SetInputValues(SurveyQuestion question, string language)
        {
            switch (question.Type)
            {
                case SurveyQuestionType.RadioList:
                    ListEnableBranch.Checked = question.ListEnableBranch;
                    ListEnableRandomization.Checked = question.ListEnableRandomization;
                    ListEnableOtherText.Checked = question.ListEnableOtherText;
                    break;
                case SurveyQuestionType.Selection:
                    ListEnableBranch.Checked = question.ListEnableBranch;
                    break;
                case SurveyQuestionType.CheckList:
                    ListEnableRandomization.Checked = question.ListEnableRandomization;
                    ListEnableOtherText.Checked = question.ListEnableOtherText;
                    break;
            }

            ListEnableGroupMembership.Checked = question.ListEnableGroupMembership;

            return OptionGrid.LoadData(question, language);
        }

        public void GetInputValues(SurveyQuestion question)
        {
            switch (question.Type)
            {
                case SurveyQuestionType.RadioList:
                    question.ListEnableBranch = ListEnableBranch.Checked;
                    question.ListEnableRandomization = ListEnableRandomization.Checked;
                    question.ListEnableOtherText = ListEnableOtherText.Checked;
                    break;
                case SurveyQuestionType.Selection:
                    question.ListEnableBranch = ListEnableBranch.Checked;
                    break;
                case SurveyQuestionType.CheckList:
                    question.ListEnableRandomization = ListEnableRandomization.Checked;
                    question.ListEnableOtherText = ListEnableOtherText.Checked;
                    break;
            }

            question.ListEnableGroupMembership = ListEnableGroupMembership.Checked;

            OptionGrid.Save(question);
        }

        public void Translate(string fromLang, string[] toLangs)
        {
            OptionGrid.Translate(fromLang, toLangs);
        }
    }
}