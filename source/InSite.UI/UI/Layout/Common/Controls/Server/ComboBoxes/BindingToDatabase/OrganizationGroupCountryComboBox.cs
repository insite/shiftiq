using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OrganizationGroupCountryComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var data = AddressSearch.SelectGroupCountries(CurrentSessionState.Identity.Organization.Identifier, null);

            return new ListItemArray(data);
        }
    }
}