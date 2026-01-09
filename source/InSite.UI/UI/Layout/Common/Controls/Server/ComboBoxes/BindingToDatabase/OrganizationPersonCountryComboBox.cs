using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OrganizationPersonCountryComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var data = AddressSearch.SelectPersonCountries(CurrentSessionState.Identity.Organization.Identifier, null);

            return new ListItemArray(data);
        }
    }
}