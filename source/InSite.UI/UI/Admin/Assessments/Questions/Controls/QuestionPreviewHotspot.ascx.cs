using System.Linq;

using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Domain.Attempts;

using Newtonsoft.Json;

namespace InSite.UI.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPreviewHotspot : QuestionPreviewControl
    {
        public override void LoadData(PreviewQuestionModel model)
        {
            var question = (AttemptQuestionHotspot)model.AttemptQuestion;

            Container.Attributes["data-pin-limit"] = question.PinLimit.ToString();
            Container.Attributes["data-img"] = question.Image;
            Container.Attributes["data-shapes"] = question.ShowShapes
                ? JsonConvert.SerializeObject(question.Options.Select(x => x.Shape))
                : "null";
            Container.Attributes["data-pins"] = "null";
        }
    }
}