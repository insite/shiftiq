using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Domain.Attempts;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPreviewOrdering : QuestionPreviewControl
    {
        public override void LoadData(PreviewQuestionModel model)
        {
            var question = (AttemptQuestionOrdering)model.AttemptQuestion;

            TopLabel.Text = question.TopLabel.IsNotEmpty() ? Markdown.ToHtml(question.TopLabel) : null;
            BottomLabel.Text = question.TopLabel.IsNotEmpty() ? Markdown.ToHtml(question.BottomLabel) : null;

            OrderingOptionRepeater.DataSource = question.Options;
            OrderingOptionRepeater.DataBind();
        }
    }
}