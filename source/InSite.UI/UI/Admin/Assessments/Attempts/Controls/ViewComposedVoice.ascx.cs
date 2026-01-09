using System.Collections.Generic;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Portal.Assessments.Attempts.Controls;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class ViewComposedVoice : BaseUserControl
    {
        public void BindQuestions(BankState bank, IEnumerable<QAttemptQuestion> questions)
        {
            ComposedRepeater.ContainerDataBound += ComposedRepeater_ContainerDataBound;
            ComposedRepeater.BindQuestions(bank, questions);
        }

        private void ComposedRepeater_ContainerDataBound(object sender, ViewComposed.AnswerContainerEventArgs e)
        {
            var dataItem = e.Container.DataItem;
            var player = (OutputAudio)e.Container.FindControl("AudioPlayer");
            var audioUrl = AnswerQuestionOutput.GetFileUrl(dataItem.AnswerFileIdentifier);

            player.AttemptLimit = dataItem.AnswerAttemptLimit ?? 0;
            player.CurrentAttempt = dataItem.AnswerRequestAttempt ?? 0;
            player.AudioURL = audioUrl;
            player.Visible = audioUrl != null;
        }
    }
}