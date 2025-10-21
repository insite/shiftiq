using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Domain.Attempts;
using InSite.UI.Portal.Assessments.Attempts.Controls;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPreviewComposedVoice : QuestionPreviewControl
    {
        public override void LoadData(PreviewQuestionModel model)
        {
            var question = (AttemptQuestionComposedVoice)model.AttemptQuestion;

            InputAudio.Bitrate = AnswerQuestionOutput.ComposedVoiceBitrate;
            InputAudio.TimeLimit = question.TimeLimit;
            InputAudio.AttemptLimit = question.AttemptLimit;

            OutputAudio.AttemptLimit = question.AttemptLimit;
        }
    }
}