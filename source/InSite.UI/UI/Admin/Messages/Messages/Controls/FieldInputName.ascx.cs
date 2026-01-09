using InSite.Common.Web.UI;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class FieldInputName : BaseUserControl
    {
        public string Value
        {
            get => InputValue.Text;
            set => InputValue.Text = value;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            InputValueValidator.ValidationGroup = groupName;
        }
    }
}