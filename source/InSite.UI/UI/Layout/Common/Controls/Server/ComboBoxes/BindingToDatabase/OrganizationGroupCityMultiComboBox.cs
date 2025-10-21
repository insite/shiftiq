using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OrganizationGroupCityMultiComboBox : MultiComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var data = AddressSearch.SelectGroupCities(CurrentSessionState.Identity.Organization.Identifier, null);

            return new ListItemArray(data);
        }
    }
}