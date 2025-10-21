using System.Web.UI;

using InSite.UI.Portal.Records.Logbooks.Controls;

namespace InSite.UI.Admin.Records.Logbooks.Controls.ExperienceFields
{
    public partial class Number : UserControl, IExperienceDecimalField
    {
        public string Title
        {
            get => FieldTitle.Text;
            set => FieldTitle.Text = value;
        }

        public string Help
        {
            get => HelpText.Text;
            set => HelpText.Text = value;
        }

        public bool IsRequired
        {
            get => RequiredValidator.Visible;
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

        public decimal? Value
        {
            get => NumberValue.ValueAsDecimal;
            set => NumberValue.ValueAsDecimal = value;
        }
    }
}