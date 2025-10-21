using System;
using System.Web.UI;

namespace InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields
{
    public partial class TwoDates : UserControl, IExperienceTwoDatesField
    {
        public string Title
        {
            get => FieldTitle.Text;
            set
            {
                FieldTitle.Text = value;
                RequiredValidator1.FieldName = value;
                RequiredValidator2.FieldName = value;
            }
        }

        public string Help
        {
            get => HelpText.Text;
            set => HelpText.Text = value;
        }

        public bool IsRequired
        {
            get => RequiredValidator1.Enabled;
            set
            {
                RequiredValidator1.Visible = value;
                RequiredValidator1.Enabled = value;

                RequiredValidator2.Visible = value;
                RequiredValidator2.Enabled = value;
            }
        }

        public string ValidationGroup
        {
            get => RequiredValidator1.ValidationGroup;
            set
            {
                RequiredValidator1.ValidationGroup = value;
                RequiredValidator2.ValidationGroup = value;
            }
        }

        public DateTime? Value1
        {
            get => DateValue1.Value;
            set => DateValue1.Value = value;
        }

        public DateTime? Value2
        {
            get => DateValue2.Value;
            set => DateValue2.Value = value;
        }
    }
}