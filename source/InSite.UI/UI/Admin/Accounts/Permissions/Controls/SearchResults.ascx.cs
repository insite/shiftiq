using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Accounts.Permissions.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TGroupActionFilter>
    {
        protected override int SelectCount(TGroupActionFilter filter)
        {
            return TGroupPermissionSearch.Count(filter);
        }

        protected override IListSource SelectData(TGroupActionFilter filter)
        {
            return TGroupPermissionSearch.Select(filter);
        }

        protected string GetObjectHtml(object o)
        {
            var item = (TGroupPermissionSearchResult)o;
            if (item.ObjectType == "Action")
                return $"<a href=\"/ui/admin/platform/routes/edit?id={item.ObjectIdentifier}\">{item.ObjectName}</a>";
            else
                return item.ObjectName;
        }

        protected string GetPermissionEditHtml(object o)
        {
            var item = (TGroupPermissionSearchResult)o;
            if (item.ObjectType == "Action")
                return $"<a href=\"/ui/admin/accounts/permissions/edit?id={item.PermissionIdentifier}\"><i class='fas fa-pencil'></i></a>";
            else
                return string.Empty;
        }
    }
}