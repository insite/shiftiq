using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CityComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var data = AddressSearch.SelectCities(CurrentSessionState.Identity.Organization.Identifier, null, null);

            return new ListItemArray(data);
        }
    }
}
