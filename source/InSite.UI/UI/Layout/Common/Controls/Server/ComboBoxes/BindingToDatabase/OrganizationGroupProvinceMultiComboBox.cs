using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OrganizationGroupProvinceMultiComboBox : MultiComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var data = AddressSearch.SelectGroupProvinces(CurrentSessionState.Identity.Organization.Identifier, null);

            return new ListItemArray(data);
        }
    }
}