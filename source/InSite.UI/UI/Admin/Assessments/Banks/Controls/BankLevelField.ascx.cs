using InSite.Common.Web.UI;
using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class BankLevelField : BaseUserControl
    {
        public Level Value
        {
            get
            {
                return new Level
                {
                    Type = Type,
                    Number = Number
                };
            }
            set
            {
                Type = value.Type;
                Number = value.Number;
            }
        }

        public string Type
        {
            get => TypeInput.Text;
            set => TypeInput.Text = value;
        }

        public int? Number
        {
            get => NumberInput.ValueAsInt;
            set => NumberInput.ValueAsInt = value;
        }
    }
}