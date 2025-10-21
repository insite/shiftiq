using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OrganizationPersonProvinceMultiComboBox : MultiComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var data = AddressSearch.SelectPersonProvinces(CurrentSessionState.Identity.Organization.Identifier, null);

            return new ListItemArray(data);
        }
    }
}