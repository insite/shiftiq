using System.Web.UI;

namespace InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields
{
    public partial class Text : UserControl, IExperienceTextInputField
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
            get => HelpText.Text;
            set => HelpText.Text = value;
        }

        public int MaxLength
        {
            get => TextValue.MaxLength;
            set => TextValue.MaxLength = value;
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
            get => TextValue.Text;
            set => TextValue.Text = value;
        }
    }
}