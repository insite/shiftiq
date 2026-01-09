using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class RouteTypeComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var types = TActionSearch.GetTypes();

            var list = new ListItemArray();

            foreach (var item in types)
            {
                list.Add(item);
            }

            return list;
        }
    }

    public class RouteListComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var lists = TActionSearch.GetLists();

            var list = new ListItemArray();

            foreach (var item in lists)
            {
                list.Add(item);
            }

            return list;
        }
    }

    public class RouteCategoryComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var categories = TActionSearch.GetCategories();

            var list = new ListItemArray();

            foreach (var item in categories)
            {
                list.Add(item);
            }

            return list;
        }
    }

    public class RouteAuthorityComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var authorities = TActionSearch.GetAuthorities();

            var list = new ListItemArray();

            foreach (var authority in authorities)
            {
                list.Add(authority);
            }

            return list;
        }
    }

    public class RouteAuthorizationComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var requirements = TActionSearch.GetAuthorizationRequirements();

            var list = new ListItemArray();

            foreach (var item in requirements)
            {
                list.Add(item);
            }

            return list;
        }
    }
}