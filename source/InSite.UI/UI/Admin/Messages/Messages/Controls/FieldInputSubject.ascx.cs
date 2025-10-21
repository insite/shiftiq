using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class FieldInputSubject : BaseUserControl
    {
        public MultilingualString Value
        {
            get => SubjectInput.Text;
            set => SubjectInput.Text = value;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            SubjectValidator.ValidationGroup = groupName;
        }
    }
}