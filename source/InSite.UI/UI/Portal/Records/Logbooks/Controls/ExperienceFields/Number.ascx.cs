using System;
using System.Web.UI;

namespace InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields
{
    public partial class Number : UserControl, IExperienceDecimalField
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

        public decimal? Value
        {
            get => NumberValue.ValueAsDecimal;
            set => NumberValue.ValueAsDecimal = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnInit(e);

            SetValidatorErrorMessage();
        }

        private void SetValidatorErrorMessage()
        {
            string errorMessage = $"<strong>{Title}</strong> field value must be greater than 0";
            NumericBoxCompareValidator.ErrorMessage = errorMessage;
        }
    }
}