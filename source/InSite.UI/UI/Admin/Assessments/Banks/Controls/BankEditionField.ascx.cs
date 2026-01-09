using InSite.Common.Web.UI;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class BankEditionField : BaseUserControl
    {
        public string Value
        {
            get => Edition.Text;
            set => Edition.Text = value;
        }
    }
}