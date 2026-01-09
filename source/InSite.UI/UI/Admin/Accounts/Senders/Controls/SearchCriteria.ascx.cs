using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Accounts.Senders.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TSenderFilter>
    {
        public override TSenderFilter Filter
        {
            get
            {
                var filter = new TSenderFilter
                {
                    SenderType = SenderType.Value,
                    SenderNickname = SenderNickname.Text,
                    SenderName = SenderName.Text,
                    SenderEmail = SenderEmail.Text,
                    SenderEnabled = SenderEnabled.ValueAsBoolean,
                    SystemMailbox = SystemMailbox.Text,
                    CompanyAddress = CompanyAddress.Text,
                    CompanyCity = CompanyCity.Text,
                    CompanyPostalCode = CompanyPostalCode.Text,
                    CompanyCountry = CompanyCountry.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                SetResourceType(value.SenderType);

                SenderNickname.Text = value.SenderNickname;
                SenderName.Text = value.SenderName;
                SenderEmail.Text = value.SenderEmail;
                SenderEnabled.ValueAsBoolean = value.SenderEnabled;
                SystemMailbox.Text = value.SystemMailbox;

                CompanyAddress.Text = value.CompanyAddress;
                CompanyCity.Text = value.CompanyCity;
                CompanyPostalCode.Text = value.CompanyPostalCode;
                CompanyCountry.Text = value.CompanyCountry;
            }
        }

        public override void Clear()
        {
            SenderType.Value = null;
            SenderNickname.Text = null;
            SenderName.Text = null;
            SenderEmail.Text = null;
            SenderEnabled.ClearSelection();

            SystemMailbox.Text = null;

            CompanyAddress.Text = null;
            CompanyCity.Text = null;
            CompanyPostalCode.Text = null;
            CompanyCountry.Text = null;
        }

        internal void SetResourceType(string value)
        {
            var option = SenderType.FindOptionByValue(value, true);
            if (option != null)
                option.Selected = true;
        }
    }
}