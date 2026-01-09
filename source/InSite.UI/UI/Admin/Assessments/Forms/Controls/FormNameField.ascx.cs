using InSite.Common.Web.UI;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class FormNameField : BaseUserControl
    {
        public string FormNameInput
        {
            get => FormName.Text;
            set => FormName.Text = value;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            FormNameValidator.ValidationGroup = groupName;
        }
    }
}