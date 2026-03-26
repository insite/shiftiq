using System.Web.UI;

using InSite.UI.Portal.Records.Logbooks.Controls;

namespace InSite.UI.Admin.Records.Logbooks.Controls.ExperienceFields
{
    public partial class DropDown : UserControl, IExperienceDropDownField
    {
        public string Title
        {
            get => FieldTitle.Text;
            set
            {
                FieldTitle.Text = value;
                RequiredValidator.FieldName = value;
            }
        }

        public string Help
        {
            get => HelpText.InnerText;
            set => HelpText.InnerText = value;
        }

        public bool IsRequired
        {
            get => RequiredValidator.Enabled;
            set
            {
                RequiredValidator.Visible = value;
                RequiredValidator.Enabled = value;
            }
        }

        public string ValidationGroup
        {
            get => RequiredValidator.ValidationGroup;
            set => RequiredValidator.ValidationGroup = value;
        }

        public string Value
        {
            get => DropDownValue.Value;
            set => DropDownValue.Value = value;
        }
    }
}