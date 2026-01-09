using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Questions.Controls
{
    public partial class QuestionExemplarEditor : BaseUserControl
    {
        public MultilingualString Text
        {
            get => EditorTranslation.Text;
            set => EditorTranslation.Text = value;
        }
    }
}