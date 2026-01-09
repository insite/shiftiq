using InSite.Common.Web.UI;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class BankNameField : BaseUserControl
    {
        public string Value
        {
            get => InternalName.Text;
            set => InternalName.Text = value;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            BankNameRequiredValidator.ValidationGroup = groupName;
        }
    }
}