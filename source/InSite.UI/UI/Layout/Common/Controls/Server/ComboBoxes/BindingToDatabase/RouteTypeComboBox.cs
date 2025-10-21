using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class RouteTypeComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (var name in TActionSearch.GetControllerTypes())
                list.Add(name);

            return list;
        }
    }
}