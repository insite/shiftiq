using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;

namespace InSite.Admin.Contacts.Addresses.Controls
{
    public partial class AddressCreator : UserControl
    {
        public class Result
        {
            public QPersonAddress Home { get; set; }
            public QPersonAddress Shipping { get; set; }
        }

        public Result GetAddresses()
        {
            var result = new Result { Home = new QPersonAddress() };

            GetAddress(result.Home);

            if(UseAsShippingAddress.Checked)
            {
                result.Shipping = new QPersonAddress();
                GetAddress(result.Shipping);
            }

            return result;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var orgCountry = CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Country ?? "Canada";
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName(orgCountry)?.Identifier ?? null;
        }

        private void GetAddress(QPersonAddress address)
        {
            address.Description = Description.Text;
            address.Street1 = Street1.Text;
            address.Street2 = Street2.Text;
            address.City = City.Text;
            address.Province = Province.Text;
            address.PostalCode = PostalCode.Text;
            address.Country = ServiceLocator.CountrySearch.SelectById(CountrySelector.Value)?.Name ?? string.Empty;
        }
    }
}