using InSite.Common.Web.UI;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class FormCodeField : BaseUserControl
    {
        public string FormCode
        {
            get => Code.Text;
            set => Code.Text = value;
        }

        public void DisableFormCode()
        {
            Code.Enabled = false;
            CodeValidator.Enabled = false;
        }

        public string FormSource
        {
            get => Source.Text;
            set => Source.Text = value;
        }

        public void DisableFormSource() => Source.Enabled = false;

        public string FormOrigin
        {
            get => Origin.Text;
            set => Origin.Text = value;
        }

        public void DisableFormOrigin() => Origin.Enabled = false;

        public string FormHook
        {
            get => Hook.Text;
            set => Hook.Text = value;
        }

        public void DisableFormHook() => Hook.Enabled = false;

        protected override void SetupValidationGroup(string groupName)
        {
            CodeValidator.ValidationGroup = groupName;
        }
    }
}