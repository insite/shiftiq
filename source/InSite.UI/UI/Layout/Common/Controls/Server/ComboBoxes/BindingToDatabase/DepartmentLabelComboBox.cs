using System.Collections.Generic;

using InSite.Persistence;

using Shift.Common;

using ListItem = Shift.Common.ListItem;

namespace InSite.Common.Web.UI
{
    public class DepartmentLabelComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var labels = DepartmentSearch.SelectDepartmentLabels(CurrentSessionState.Identity.Organization.OrganizationIdentifier);

            var data = new List<ListItem>();
            foreach (var label in labels)
                data.Add(new ListItem
                {
                    Value = label,
                    Text = label
                });

            return new ListItemArray(data);
        }
    }
}