using System.Linq;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ContainerTypeComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var data = ServiceLocator.ContentSearch
                .SelectContainers(x => x.OrganizationIdentifier == CurrentSessionState.Identity.Organization.Identifier && !string.IsNullOrEmpty(x.ContainerType))
                .Select(x => x.ContainerType)
                .Distinct()
                .OrderBy(x => x);

            foreach (var type in data)
            {
                list.Add(new ListItem { Text = type, Value = type });
            }

            return list;
        }
    }
}