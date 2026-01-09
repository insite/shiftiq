using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Records.Rurbics.Controls
{
    public partial class RubricDetail : BaseUserControl
    {
        public bool ShowPoints
        {
            get => RubricPointsField.Visible;
            set => RubricPointsField.Visible = value;
        }

        public void SetInputValues(QRubric rubric)
        {
            RubricTitle.Text = rubric.RubricTitle;
            RubricDescription.Text = rubric.RubricDescription;
        }

        public void GetInputValues(QRubric rubric)
        {
            rubric.RubricTitle = RubricTitle.Text;
            rubric.RubricDescription = RubricDescription.Text;
        }

        public void ResetValues()
        {
            RubricTitle.Text = string.Empty;
            RubricDescription.Text = string.Empty;
        }
    }
}