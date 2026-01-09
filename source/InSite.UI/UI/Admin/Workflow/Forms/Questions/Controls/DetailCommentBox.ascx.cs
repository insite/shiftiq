using System.Web.UI;

using InSite.Domain.Surveys.Forms;

namespace InSite.Admin.Workflow.Forms.Questions.Controls
{
    public partial class DetailCommentBox : UserControl
    {
        public void SetInputValues(SurveyQuestion question)
        {
            TextLineCount.ValueAsInt = question.TextLineCount;
            TextCharacterLimit.ValueAsInt = question.TextCharacterLimit ?? 200;
            TextCharacterWarning.CssClass = question.TextCharacterLimit > 4000 ? string.Empty : "d-none";
        }

        public void GetInputValues(SurveyQuestion question)
        {
            question.TextLineCount = TextLineCount.ValueAsInt;
            question.TextCharacterLimit = TextCharacterLimit.ValueAsInt;
        }
    }
}