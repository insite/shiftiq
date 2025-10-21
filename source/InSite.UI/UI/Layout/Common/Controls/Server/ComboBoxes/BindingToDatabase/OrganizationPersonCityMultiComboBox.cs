using System.Collections.Generic;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OrganizationPersonCityMultiComboBox : MultiComboBox
    {
        public List<string> Province => (List<string>)(ViewState[nameof(Province)]
            ?? (ViewState[nameof(Province)] = new List<string>()));

        protected override ListItemArray CreateDataSource()
        {
            var data = AddressSearch.SelectPersonCities(CurrentSessionState.Identity.Organization.Identifier, Province);

            return new ListItemArray(data);
        }
    }
}