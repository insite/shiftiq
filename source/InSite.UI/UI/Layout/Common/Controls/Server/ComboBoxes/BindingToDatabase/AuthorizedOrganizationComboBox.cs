using System.Linq;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class AuthorizedOrganizationComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (var organization in CurrentSessionState.Identity.Organizations.OrderBy(x => x.CompanyName))
                list.Add(organization.OrganizationIdentifier.ToString(), organization.CompanyName);

            return list;
        }
    }
}
